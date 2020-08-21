using System;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using grpcWithAuth;
using Microsoft.Extensions.Configuration;

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
            var greeterClient = new Greeter.GreeterClient(channel);
            var graphClient = new GraphService.GraphServiceClient(channel);
            
            var headers = new Metadata();
            headers.Add("Authorization", $"Bearer {token}");
            
            var greeterRequest = new HelloRequest()
            {
                Name = "SpongeBob"
            };

            var reply = await greeterClient.SayHelloAsync(greeterRequest, headers);
            Console.WriteLine(reply.Message);
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Fetching calendar data");
            
            var calendarRequest = new CalendarRequest()
            {
                Name = "Christos"
            };

            var calendarResponse = await graphClient.GetCalendarAsync(calendarRequest, headers);
            Console.WriteLine(calendarResponse.Message);
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
