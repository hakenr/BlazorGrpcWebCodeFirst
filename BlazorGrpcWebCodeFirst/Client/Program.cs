using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Components;
using Grpc.Net.Client.Web;
using Grpc.Net.Client;
using BlazorGrpcWebCodeFirst.Shared;
using ProtoBuf.Grpc.Client;

namespace BlazorGrpcWebCodeFirst.Client
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("app");

			builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

			// gRPC Channel
			// Credits: https://github.com/grpc/grpc-dotnet/blob/master/examples/Blazor/Client/Program.cs
			builder.Services.AddSingleton(services =>
			{
				// Get the service address from appsettings.json
				var config = services.GetRequiredService<IConfiguration>();
				var backendUrl = config["BackendUrl"];

				// If no address is set then fallback to the current webpage URL
				if (string.IsNullOrEmpty(backendUrl))
				{
					var navigationManager = services.GetRequiredService<NavigationManager>();
					backendUrl = navigationManager.BaseUri;
				}

				// Create a channel with a GrpcWebHandler that is addressed to the backend server.
				//
				// GrpcWebText is used because server streaming requires it. If server streaming is not used in your app
				// then GrpcWeb is recommended because it produces smaller messages.
				var httpHandler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());

				return GrpcChannel.ForAddress(
					backendUrl,
					new GrpcChannelOptions
					{
						HttpHandler = httpHandler,
						//CompressionProviders = ...,
						//Credentials = ...,
						//DisposeHttpClient = ...,
						//HttpClient = ...,
						//LoggerFactory = ...,
						//MaxReceiveMessageSize = ...,
						//MaxSendMessageSize = ...,
						//ThrowOperationCanceledOnCancellation = ...,
					});
			});

			builder.Services.AddTransient<IMyService>(services =>
			{
				var grpcChannel = services.GetRequiredService<GrpcChannel>();
				return grpcChannel.CreateGrpcService<IMyService>();
			});

			await builder.Build().RunAsync();
		}
	}
}
