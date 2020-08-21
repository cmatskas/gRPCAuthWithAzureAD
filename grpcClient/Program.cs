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
            var client = new Greeter.GreeterClient(channel);
            
            var headers = new Metadata();
            headers.Add("Authorization", $"Bearer {token}");
            
            var request = new HelloRequest()
            {
                Name = "SpongeBob"
            };

            var reply = await client.SayHelloAsync(request, headers);
            Console.WriteLine(reply.Message);
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
