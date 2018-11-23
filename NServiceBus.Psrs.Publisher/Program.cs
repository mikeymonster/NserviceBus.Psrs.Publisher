using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NServiceBus.Psrs.Publisher.Services;
using Sfa.Roatp.Events;
using Sfa.Roatp.Events.Types;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.AzureServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.PSRService.Messages.Events;

namespace NServiceBus.Psrs.Publisher
{
    class Program
    {
        internal static IEndpointInstance EndpointInstance { get; private set; }

        internal static IWebConfiguration Configuration { get; set; }

        static async Task Main(string[] args)
        {
            BuildConfiguration();

            await ConfigureEndpointInstance();

            var runOptionKey = GetRunOption();
            while (runOptionKey != ConsoleKey.Q && runOptionKey != ConsoleKey.Escape)
            {
                try
                {
                    switch (runOptionKey)
                    {
                        case ConsoleKey.D1:
                        case ConsoleKey.NumPad1:
                        case ConsoleKey.Enter: //Default
                            await SendReportCreatedMessage();
                            break;
                        case ConsoleKey.D2:
                        case ConsoleKey.NumPad2:
                            await SendReportUpdatedMessage();
                            break;
                        case ConsoleKey.D3:
                        case ConsoleKey.NumPad3:
                            await SendReportSubmittedMessage();
                            break;
                        case ConsoleKey.D4:
                        case ConsoleKey.NumPad4:
                            await SendRoatpUpdatedMessage();
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print($"An exception has occurred. {ex}");
                    System.Console.WriteLine($"An exception has occurred. {ex}");
                }

                Console.WriteLine("....");
                runOptionKey = GetRunOption();
            }
        }

        private static void BuildConfiguration()
        {
            try
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");

                var localConfiguration = builder.Build();

                //IWebConfiguration Configuration { get; }
                Configuration = ConfigurationService.GetConfig(
                        localConfiguration["EnvironmentName"],
                        localConfiguration["ConfigurationStorageConnectionString"],
                        localConfiguration["Version"],
                        localConfiguration["ServiceName"])
                    .Result;

                Console.WriteLine($"Retrieved configuration . NServiceBus endpoint is {Configuration.NServiceBus.Endpoint}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private static ConsoleKey GetRunOption()
        {
            System.Console.WriteLine("Options:");
            System.Console.WriteLine("    1 to send a Report Submitted event");
            System.Console.WriteLine("    2 to send Report Updated event");
            System.Console.WriteLine("    3 to send Report Submitted event");
            System.Console.WriteLine("    4 to send Roatpreboot event");
            System.Console.WriteLine("    Esc or Q to exit.");

            return System.Console.ReadKey().Key;
        }

        private static ReportCreated ConstructReportCreatedMessage()
        {
            return new ReportCreated
            {
                Id = Guid.NewGuid(),
                EmployerId = "XYZ1234",
                ReportingPeriod = "1819",
                Created = DateTime.UtcNow
            };
        }

        private static ReportUpdated ConstructReportUpdatedMessage()
        {
            return new ReportUpdated
            {
                Id = Guid.NewGuid(),
                EmployerId = "XYZ1234",
                ReportingPeriod = "1819",
                Answers = new Answers
                {
                    OrganisationName = "My Organisation",
                    YourEmployees = new YourEmployees
                    {
                        AtStart = "3",
                        AtEnd = "5",
                        NewThisPeriod = "2"
                    },
                    YourApprentices = new YourApprentices
                    {
                        AtStart = "2",
                        AtEnd = "3",
                        NewThisPeriod = "1"
                    }
                },
                ReportingPercentages = new ReportingPercentages
                {
                    EmploymentStarts = "10",
                    NewThisPeriod = "5",
                    TotalHeadCount = "12"
                },
                Created = DateTime.UtcNow
            };
        }

        private static ReportSubmitted ConstructReportSubmittedMessage()
        {
            return new ReportSubmitted
            {
                Id = Guid.NewGuid(),
                EmployerId = "XYZ1234",
                ReportingPeriod = "1819",
                Submitter = new Submitter
                {
                    UserId = "Mike",
                    Name = "Mike",
                    Email = "mike.wild@digital.education.gov.uk"
                },
                Answers = new Answers
                {
                    OrganisationName = "My Organisation",
                    Challenges = "Challenges",
                    YourEmployees = new YourEmployees
                    {
                        AtStart = "3",
                        AtEnd = "5",
                        NewThisPeriod = "2"
                    },
                    YourApprentices = new YourApprentices
                    {
                        AtStart = "2",
                        AtEnd = "3",
                        NewThisPeriod = "1"
                    },
                    FullTimeEquivalents = "8",
                    OutlineActions = "Outline actions",
                    TargetPlans = "Target plans",
                    AnythingElse = "Anything else"
                },
                ReportingPercentages = new ReportingPercentages
                {
                    EmploymentStarts = "10",
                    NewThisPeriod = "5",
                    TotalHeadCount = "12"
                },
                Created = DateTime.UtcNow
            };
        }

        private static RoatpProviderUpdated ConstructRoatpUpdatedMessage()
        {
            return new RoatpProviderUpdated
            {
                MessageType = MessageType.Added,
                Ukprn = "123456789",
                Name = "My Provider",
                ProviderType = ProviderType.SupportingProvider,
                RequiresAgreement = true,
                ContractedForNonLeviedEmployers = true,
                ParentCompanyGuarantee = false,
                NewOrganisationWithoutFinancialTrackRecord = true,
                StartDate = new DateTime(2018, 10, 01),
                EndDate = null,
                CurrentlyNotStartingNewApprentices = false,
                Created = DateTime.UtcNow
            };
        }

        public static async Task SendReportCreatedMessage()
        {
            var message = ConstructReportCreatedMessage();
            await EndpointInstance.Publish(message).ConfigureAwait(false);
            System.Console.WriteLine($"Published Report Created event: {message}");
        }

        public static async Task SendReportUpdatedMessage()
        {
            var message = ConstructReportUpdatedMessage();
            await EndpointInstance.Publish(message).ConfigureAwait(false);
            System.Console.WriteLine($"Published Report Updated event: {message}");
        }

        public static async Task SendReportSubmittedMessage()
        {
            var message = ConstructReportSubmittedMessage();
            await EndpointInstance.Publish(message).ConfigureAwait(false);
            System.Console.WriteLine($"Published Report Submitted event: {message}");
        }

        public static async Task SendRoatpUpdatedMessage()
        {
            var message = ConstructRoatpUpdatedMessage();
            await EndpointInstance.Publish(message).ConfigureAwait(false);
            System.Console.WriteLine($"Published Roatp Updated event: {message}");
        }

        public static async Task ConfigureEndpointInstance(bool useLearningTransport = false)
        {
            var endpointConfiguration = new EndpointConfiguration(Configuration.NServiceBus.Endpoint)
                .UseLicense(Configuration.NServiceBus.LicenceText)
                .UseAzureServiceBusTransport(false, () => Configuration.NServiceBus.ServiceBusConnectionString,
                    r =>
                    {
                        //r.RouteToEndpoint(typeof(StringMessageEvent), nServiceBusSettings.Endpoint);
                    })
                .UseNewtonsoftJsonSerializer();

            endpointConfiguration.SendOnly();

            EndpointInstance = await Endpoint.Start(endpointConfiguration)
                .ConfigureAwait(false);

            //var options = new PublishOptions();

            System.Console.WriteLine($"Created endpoint instance.");
        }

        //TODO: How do we run this? Needs SFA.DAS.NServiceBus.ClientOutbox
        //public static Task ProcessOutboxMessages(IProcessClientOutboxMessagesJob job)
        //{
        //    return job.RunAsync();
        //}
    }
}
