using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CavemanTools;
using CavemanTools.Logging;
using DomainBus.Abstractions;
using DomainBus.Audit;
using DomainBus.Configuration;

namespace DomainBus.Processing.Internals
{
    public class ProcessingService:IDisposable, IManageProcessingService
    {
        
        private readonly IStoreUnhandledMessages _storage;
        private readonly Func<IProcessMessage> _processorFactory;
    
        private ProcessorMessageCache _cache;

        private ITimer _timer;
        private TimeSpan _pollingInterval;


        public ProcessingService(IStoreUnhandledMessages storage, Func<IProcessMessage> processorFactory,
            BusAuditor busAuditor, IFailedMessagesQueue errors):this(new DefaultTimer(),storage,processorFactory,busAuditor,errors)
        {
            
        }

        public ProcessingService(ITimer timer,IStoreUnhandledMessages storage,Func<IProcessMessage> processorFactory,BusAuditor busAuditor,IFailedMessagesQueue errors)
        {
            storage.MustNotBeNull();
            timer.MustNotBeNull();
            _timer = timer;
            _timer.SetHandler(o=>LoadMessages());
            _storage = storage;
            _processorFactory = processorFactory;
            _busAuditor = busAuditor;
            _errors = errors;

            _cache = new ProcessorMessageCache();
            SetupDefaults();
         
        }

        private void SetupDefaults()
        {
           
            Name = "Default";
            PollingInterval = TimeSpan.FromMinutes(1);
            BufferSize = 50;
            IsPaused = true;      
        }


        

       

        private void Timer_Handler(object data)
        {
            if (_isDisposed) return;           
            LoadMessages();
        }

        private void LoadMessages()
        {
            try
            {
                var msgs = _storage.GetMessages(Name, BufferSize).ToArray();
                if (!msgs.Any()) return;
                _cache.Add(msgs);
                if (!IsPaused) SeedTasks();
            }
            catch (BusStorageException ex)
            {
                _logName.LogError(ex, "When trying to load messages for processing, the store threw exception.");
            }
            catch (Exception ex)
            {
                _logName.LogError(ex);
                throw;
            }
        }

        /// <summary>
        /// Gets/sets how often the storage is checked for pending messages. Default is 1 minute
        /// </summary>
        public TimeSpan PollingInterval
        {
            get { return _timer.Interval; }
            set
            {
                var old = _timer.IsRunning;
                _timer.Stop();
                _timer.Interval = value;
                if (old) _timer.Start();
            }
        }

        /// <summary>
        /// Get/sets how many messages should be loaded from the storage. Default is 50
        /// </summary>
        public int BufferSize { get; set; }

        public void Start(bool loadInitialMessages = true)
        {
           if (!IsPaused) return;
           
            _logName.LogInfo("Started");
            _cancelSource = new CancellationTokenSource();
            IsPaused = false;
            if (loadInitialMessages)LoadMessages();
            if (PollingEnabled) _timer.Start();
            else
            {
                this.LogInfo("Polling is disabled.");
            }
          
           
        }

        /// <summary>
        /// Enables or disables message polling. If disabled, only messages added in-process will be processed.
        /// Default is false
        /// </summary>
        public bool PollingEnabled { get; set; }

        object _sync=new object();
        private Task _worker;

        void SeedTasks()
        {
            
            lock (_sync)
            {                
                if (_worker!=null && !_worker.IsCompleted) return;
                _worker = Task
                            .Run(() => Processor(_cancelSource.Token),_cancelSource.Token)
                            .ContinueWith(AfterProcessorFinishes);                
            }
        }

        private void AfterProcessorFinishes(Task t)
        {
            if (!t.IsFaulted) return;            
            _logName.LogError(t.Exception,"Unhandled procesor exception");
        }

        public void Stop()
        {
            if (IsPaused) return;
            _cancelSource.Cancel();
            
            _timer.Stop();
            
            _logName.LogInfo("Stopped");
            IsPaused = true;
        }

        public bool IsPaused { get; private set; }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                _logName = "[Message processor: " + value+"]";
            }
        }

        public ProcessorMessageCache Cache => _cache;

        private string _logName;
        private string _name;
        private bool _isDisposed;
        private CancellationTokenSource _cancelSource;

        public async Task Queue(params IMessage[] msg)
        {
            if (msg.Length == 0) return;
            _logName.LogDebug("Storing messages");
            await _storage.Add(Name, msg).ConfigureAwait(false);

            _busAuditor.AddedToProcessingStorage(Name,msg);
            
            _cache.Add(msg);
            _logName.LogDebug("Messages added to processing cache");
            if (!IsPaused) SeedTasks();
        }
     
        private BusAuditor _busAuditor;
        private readonly IFailedMessagesQueue _errors;


        private void Processor(CancellationToken token)
        {
            var processor = _processorFactory();
            _logName.LogDebug("Starting processor task");
   
            while (!token.IsCancellationRequested)
            {
                var msg = _cache.GetNextMessage();
                if (msg == null)
                {
                    _logName.LogDebug("No message available, exiting");
                    break;
                }


                try
                {

                    _busAuditor.StartedProcessing(Name, msg);
                    processor.Process(msg, Name);

                    _storage.MarkMessageHandled(Name, msg.Id);

                

                }
                catch (BusStorageException ex)
                {
                    _logName.LogError(ex);
                }
                catch (SagaConfigurationException ex)
                {
                    _errors.MessageCantBeHandled(msg, ex);
                    _busAuditor.BusConfigurationError(msg, ex,_name);
                }

                catch (MissingHandlerException ex)
                {
                    _errors.MessageCantBeHandled(msg,ex);
                    _busAuditor.BusConfigurationError(msg, ex,_name);
                }

                catch (DiContainerException ex)
                {
                    _errors.MessageCantBeHandled(msg, ex);
                    _busAuditor.BusConfigurationError(msg, ex,_name);
                }

                finally
                {
                    Cache.MessageHandled(msg);
                    _busAuditor.MessageProcessed(Name, msg);
                }                
            }

            _logName.LogDebug("Processing ended");
        }

      
        /// <summary>
        /// Blocks thread until all workers complete their work.
        /// Used for testing/debugging
        /// </summary>
        public void WaitUntilWorkersFinish()
        {
            _worker?.Wait();
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            Stop();
            _timer.Dispose();
            _logName.LogInfo("Disposed");
        }
    }
}