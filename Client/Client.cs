using System;
using System.Net.Http;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcDotNetDemoPackage;

namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = await GetClient();

            var response = await client.SayHelloAsync(new HelloRequest
            {
                Name = "Mustakim"
            });
            
            Console.WriteLine(response.Hello);
        }
        
        private static Task<DemoService.DemoServiceClient> GetClient()
        {
            // This disables HTTPS
            // https://github.com/aspnet/AspNetCore.Docs/issues/13120
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5000")
            };

            var client = GrpcClient.Create<DemoService.DemoServiceClient>(httpClient);
            return Task.FromResult(client);
        }
    }
}
