using BlazorGrpcWebCodeFirst.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorGrpcWebCodeFirst.Server.Services;

public class MyService : IMyService
{
	public Task<MyServiceResult> DoSomething(MyServiceRequest request)
	{
		return Task.FromResult(new MyServiceResult()
		{
			NewText = request.Text + " from server",
			NewValue = request.Value + 1
		});
	}
}

