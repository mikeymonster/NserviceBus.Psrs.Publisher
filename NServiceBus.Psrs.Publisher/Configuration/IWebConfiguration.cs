using NServiceBus.Psrs.Publisher.Configuration;

namespace NServiceBus.Psrs.Publisher
{
    public interface IWebConfiguration
    {
        NServiceBusConfiguration NServiceBus { get; set; }
    }
}
