using DomainBus.Configuration;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoNSubstitute;

namespace Tests
{
    public static class Setup
    {
         public static readonly EndpointId TestEndpoint=EndpointId.TestValue;
        public static IFixture Fixture=new Fixture().Customize(new AutoNSubstituteCustomization());
    }
}