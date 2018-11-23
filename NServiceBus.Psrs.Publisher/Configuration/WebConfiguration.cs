namespace NServiceBus.Psrs.Publisher.Configuration
{
    public class WebConfiguration : IWebConfiguration
    {
       public NServiceBusConfiguration NServiceBus { get; set; }
    }
}
