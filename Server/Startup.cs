using System.Net;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcDotNetDemoPackage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureKestrel(c =>
                        {
                            // TLS with HTTP/2 isn't supported in mac
                            // Wait for next version of macOS
                            c.Listen(IPEndPoint.Parse("0.0.0.0:5000"), l => l.Protocols = HttpProtocols.Http2);
                        })
                        .UseStartup<Startup>();
                });
        }
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddGrpc();
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<DemoServiceImpl>();
            });
        }
    }
    
    public class DemoServiceImpl : DemoService.DemoServiceBase
    {
        public override Task<HelloResponse> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloResponse
            {
                Hello = $"Hello {request.Name}"
            });
        }

        public override Task<HelloResponse> SayHelloToNobody(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new HelloResponse
            {
                Hello = "Hello Nobody!"
            });
        }
    }
}
