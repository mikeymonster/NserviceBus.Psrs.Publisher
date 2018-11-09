using NServiceBus.Psrs.Publisher.Configuration;

namespace NServiceBus.Psrs.Publisher
{
    public class WebConfiguration : IWebConfiguration
    {
       public NServiceBusConfiguration NServiceBus { get; set; }
    }
}
