# Blazor WebAssembly with gRPC-Web code-first approach
Do you like WCF-like approach and need to cover communication in between ASP.NET Core service and Blazor WebAssembly client? Use code-first with gRPC-Web! You can try the it right now by following a few simple steps (<a href="https://github.com/hakenr/BlazorGrpcWebCodeFirst/commit/e2bbbbb0aa8b9c9709145e271ee25cb41a4effae">commit</a>):
## 1. Blazor.Server - Prepare the ASP.NET Core host<
Add NuGet packages:
* [Grpc.AspNetCore.Web](https://www.nuget.org/packages/Grpc.AspNetCore.Web) (prerelease)
* [protobuf-net.Grpc.AspNetCore](https://www.nuget.org/packages/protobuf-net.Grpc.AspNetCore/)

Register `CodeFirstGrpc()` and `GrpcWeb()` services in `Startup.cs` ConfigureServices() method:

```csharp
services.AddCodeFirstGrpc(config => { config.ResponseCompressionLevel = System.IO.Compression.CompressionLevel.Optimal; });
```

Add `GrpcWeb` middleware in between `UseRouting()` and `UseEndpoints()`:

```csharp
app.UseGrpcWeb(new GrpcWebOptions() { DefaultEnabled = true });
```

## 2. Blazor.Shared - Define the service contract (code-first)
Add [System.ServiceModel.Primitives](https://www.nuget.org/packages/System.ServiceModel.Primitives/) NuGet package.

Define the interface of your service:

```csharp
[ServiceContract]
public interface IMyService
{
	Task DoSomething(MyServiceRequest request);

}

[DataContract]
public class MyServiceResult
{
	[DataMember(Order = 1)]
	public string NewText { get; set; }

	[DataMember(Order = 2)]
	public int NewValue { get; set; }
}

[DataContract]
public class MyServiceRequest
{
	[DataMember(Order = 1)]
	public string Text { get; set; }

	[DataMember(Order = 2)]
	public int Value { get; set; }
}
```

## 3. Blazor.Server - Implement and publish the service
Implement your service:

```csharp
public class MyService : IMyService
{
	public Task DoSomething(MyServiceRequest request)
	{
		return Task.FromResult(new MyServiceResult()
		{
			NewText = request.Text + " from server",
			NewValue = request.Value + 1
		});
	}
}
```

Publish the service in `Startup.cs`:

```csharp
app.UseEndpoints(endpoints =>
{
	endpoints.MapGrpcService<MyService>();
	// ...
}
```

## 4. Blazor.Client (Blazor Web Assembly) - consume the service
Add NuGet packages:
* [Grpc.Net.Client](https://www.nuget.org/packages/Grpc.Net.Client)
* [Grpc.Net.Client.Web](https://www.nuget.org/packages/Grpc.Net.Client.Web) (prerelease)
* [protobuf-net.Grpc](https://www.nuget.org/packages/protobuf-net.Grpc)

Consume the service in your razor file:

```csharp
var handler = new Grpc.Net.Client.Web.GrpcWebHandler(Grpc.Net.Client.Web.GrpcWebMode.GrpcWeb, new HttpClientHandler());
using (var channel = Grpc.Net.Client.GrpcChannel.ForAddress("https://localhost:44383/", new Grpc.Net.Client.GrpcChannelOptions() { HttpClient = new HttpClient(handler) }))
{
	var testFacade = channel.CreateGrpcService<IMyService>();
	this.result = await testFacade.DoSomething(request);
}
```
(You can move the plumbing to `ConfigureServices()` as a factory and use pure dependency injection in your razor files.)

# References
* [Steve Sanderson: Using gRPC-Web with Blazor WebAssembly](https://blog.stevensanderson.com/2020/01/15/2020-01-15-grpc-web-in-blazor-webassembly/)
* [Use gRPC in browser apps | Microsoft Docs](https://docs.microsoft.com/en-us/aspnet/core/grpc/browser)
* [protobuf-net.Grpc - Getting Started](https://protobuf-net.github.io/protobuf-net.Grpc/gettingstarted)