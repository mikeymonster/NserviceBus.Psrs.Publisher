using System;
using System.Collections.Generic;
using System.Text;

namespace NServiceBus.Psrs.Publisher.Configuration
{
    public class NServiceBusConfiguration
    {
        public string Endpoint { get; set; }
        public string LicenceText { get; set; }
        public string ServiceBusConnectionString { get; set; }
    }
}
