using System;

namespace DomainBus.Configuration
{
    public class EndpointId : IEquatable<EndpointId>
    {
       
        public static readonly EndpointId TestValue=new EndpointId("test","localtest");

        private readonly string _processor = "test";
        private readonly string _host = "testhost";

        /// <summary>
        /// String should be like 'id@server' 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static EndpointId Parse(string data)
        {
            var d = data.Split('@');
            return new EndpointId(d[0],d[1]);
        }

        
        public EndpointId(string processor, string host)
        {
            processor.MustNotBeEmpty();
            host.MustNotBeEmpty();
            _processor = processor;
            _host = host;
        }

        public string Processor => _processor;

        public string Host => _host;


        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(EndpointId other)
        {
            return other!=null && other._host == _host && other._processor == _processor;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EndpointId);
        }

        public override int GetHashCode()
        {
            return _processor.GetHashCode()*17 + _host.GetHashCode();
        }

        public override string ToString()
        {
            return Processor + "@" + Host;
        }

        
        public static implicit operator EndpointId(string d)
        {
            return Parse(d);
        }

        public static implicit operator string(EndpointId d) => d.ToString();
        

        public static bool operator ==(EndpointId first, EndpointId second)
        {
            if (ReferenceEquals(first, null)) return ReferenceEquals(second,null);
            return first.Equals(second);
        }

        public static bool operator !=(EndpointId first, EndpointId second) => !(first == second);
    }
}