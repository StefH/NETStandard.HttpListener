# NETStandard.HttpListener .NET Core and UWP
A simple library based on (https://github.com/robertsundstrom/HttpListener) that essentially allows for building your own HTTP server on .NET Core and the Universal Windows Platform (UWP).

[![Build status](https://ci.appveyor.com/api/projects/status/192if73p5og2o2yq?svg=true)](https://ci.appveyor.com/project/StefH/netstandard-httplistener)
[![codecov](https://codecov.io/gh/StefH/NETStandard.HttpListener/branch/master/graph/badge.svg)](https://codecov.io/gh/StefH/NETStandard.HttpListener)
[![Coverage Status](https://coveralls.io/repos/github/StefH/NETStandard.HttpListener/badge.svg?branch=master)](https://coveralls.io/github/StefH/NETStandard.HttpListener?branch=master)
[![NuGet Badge](https://buildstats.info/nuget/NETStandard.HttpListener)](https://www.nuget.org/packages/NETStandard.HttpListener)

## Overview

This library fills the void left by the missing System.Net.Http.HttpListener in .NET Core and Universal Windows Platform (UWP).

By targetting .NET Core (NETStandard 1.3) and UWP (10.0.10240.0), this API enables HTTP server scenarios on Windows 10 for IoT on Raspberry Pi (2 & 3).

Taking a modern approach, this API is not meant to be entirely compatible with the HttpListener found in the full .NET Framework on Windows desktop.

Please, be aware that this is an early concept, and thus not ready for production.

Contributions are most welcome.

## Solution

The solution consists of two projects with a common core targetting:

1. .NET Core project - Windows, Linux and Mac OS X.
2. Universal Windows Platform (UWP) - Windows 10 and up. (10.0.10240.0)

The API:s are generally similar, but may differ slightly on each platform due to their respective API constraints. However, the core concepts remain the same.

On .NET Core it uses .NET:s TcpListener and TcpClient.

On UWP it uses Windows Runtime's StreamSocketListener and StreamSocket.

## Sample
Add the using statement.

```CSharp
...
using System.Net.Http;
```

The code used in this sample should be the same on any platform.

```CSharp
var listener = new HttpListener(IPAddress.Parse("127.0.0.1"), 8081);
try 
{
	listener.Request += async (sender, context) => {
		var request = context.Request;
		var response = context.Response;
		if(request.Method == HttpMethod.Get) 
		{
			await response.WriteAsync($"Hello from Server at: {DateTime.Now}\r\n");
		}
		else
		{
			response.MethodNotAllowed();
		}
		// Close the HttpResponse to send it back to the client.
		response.Close();
	};
	listener.Start();

	Console.WriteLine("Press any key to exit.");
	Console.ReadKey();
}
catch(Exception exc) 
{
	Console.WriteLine(exc.ToString());
}
finally 
{
	listener.Close();
}
```

Visit 127.0.0.1:8081 in your browser.

Also consider having a look at the unit tests.


## Todo
Here are some things to consider doing in the future:

* Rewrite the HttpRequest parser and implement missing features, like authentication and the handling of content types.
