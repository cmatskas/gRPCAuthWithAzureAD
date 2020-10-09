using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using grpcWithAuth;
using Microsoft.Extensions.Configuration;

using static System.Console;

namespace grpcClient
{
    class Program
    {
        private static IConfiguration configuration;
        static async Task Main(string[] args)
        {
            LoadAppSettings();

            var authProvider = new DeviceCodeAuthProvider(configuration);
            var token = await authProvider.GetAccessToken(new string[] {configuration["scope"]});

            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var graphClient = new GraphService.GraphServiceClient(channel);
            
            var headers = new Metadata();
            headers.Add("Authorization", $"Bearer {token}");
            
            var calendarRequest = new CalendarRequest()
            {
                Name = "Christos"
            };

            var calendarResponse = await graphClient.GetCalendarAsync(calendarRequest, headers);
            WriteLine("*************** New event found ***************");
            WriteLine($"Calendar event subject: {calendarResponse.Subject}");
            WriteLine($"Calendar event body preview: {calendarResponse.BodyPreview}");
            WriteLine($"Calendar event start date/time: {calendarResponse.Start}");
            WriteLine($"Calendar event end date/time: {calendarResponse.End }");
        }

        static void LoadAppSettings()
        {
            configuration = new ConfigurationBuilder()
                            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            .Build();
        }
    }
}
