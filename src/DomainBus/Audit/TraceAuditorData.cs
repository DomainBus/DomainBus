namespace DomainBus.Audit
{
//    public class TraceAuditorData : IStoreAuditorData
//    {
//        public static readonly TraceAuditorData Instance=new TraceAuditorData();

//        private const string Log = "DomainBus Auditor";
//        public void StoreError(Exception ex)
//        {
            
//            Log.LogError("{0}".ToFormat(ex.ToString()));           
//        }

//        public void StoreFailedHandler(MessageProcessedData data)
//        {
           
//            var sb = new StringBuilder();
//            sb.Append(@"
// --------------------------------
//|Endpoint: {0} 
//|Message:  {1}
// ---------------------------------
//".ToFormat(data.ProcessorId, data.Message));
//            foreach (var ex in data.Result.Exceptions)
//            {
//                sb.AppendFormat(@"
//++++++++++++++++++++++++++++++++++++
//+ Handler:  {0} 
//++++++++++++++++++++++++++++++++++++
//Exception: {1}

//-------------------
//",ex.HandlerType,ex.InnerException);
//            }
//            sb.Append(@"
//|********************************|");
//             Log.LogError(sb.ToString());
//        }

//        public void StorePoisonedData(PoisonedMessage data)
//        {
////            Log.LogError(@"
////========================================
////Poisoned message: {1}
////Endpoint: {0}
////========================================", data.ProcessorId, data.Message);
//        }

      
//    }
}