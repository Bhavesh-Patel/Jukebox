using System;
using ServiceStack.ServiceClient.Web;

namespace ServiceStackClient
{
	class Program
	{
		static void Main(string[] args)
		{
			// The host ASP.NET MVC is working OK! When I visit this URL I see the metadata page.
			const string BaseUrl = "http://localhost:1337/";

			var restClient = new JsonServiceClient(BaseUrl);

			restClient.SetCredentials("juser", "password");
			restClient.AlwaysSendBasicAuthHeader = true;

			Console.WriteLine("Enter name:");
			string name = Console.ReadLine();
			while (!string.IsNullOrEmpty(name)) {
				ServiceStackService.Program.HelloResponse response = restClient.Get<ServiceStackService.Program.HelloResponse>("/hello/" + name);
				//ServiceStackService.Program.HelloResponse response = restClient.Get<ServiceStackService.Program.HelloResponse>("/hello/bhavesh");
				Console.WriteLine(response.Result);
				name = Console.ReadLine();
			}
			
		}
	}
}
