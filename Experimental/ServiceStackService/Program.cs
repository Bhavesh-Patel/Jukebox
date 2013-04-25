using System;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.Configuration;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.WebHost.Endpoints;

namespace ServiceStackService
{
	public class Program
	{
		[Authenticate]
		public class Hello
		{
			public string Name { get; set; }
		}

		public class HelloResponse
		{
			public string Result { get; set; }
		}

		public class HelloService : Service
		{
			public object Any(Hello request)
			{
				HelloResponse result = new HelloResponse {Result = string.Format("Hello, {0}", request.Name)};
				//IAuthSession authSession = this.GetSession();
				//if (authSession != null) {
				//	string displayName = authSession.DisplayName;
				//	result.Result += string.Format("(or is it {0})", displayName);
				//}
				Console.WriteLine("Request: " + request.Name);
				return result;
			}

			public object Post(Hello request)
			{
				object result = Any(request);
				return result;
			}
		}

		//Define the Web Services AppHost
		public class AppHost : AppHostHttpListenerBase
		{
			private HelloService helloService;

			public AppHost() : base("StarterTemplate HttpListener", typeof(HelloService).Assembly) { }

			public HelloService HelloService
			{
				get { return helloService; }
			}

			public override void Configure(Funq.Container container)
			{
				Routes
					.Add<Hello>("/hello")
					.Add<Hello>("/hello/{Name}");
				helloService = container.Resolve<HelloService>();

				AuthFeature authFeature = new AuthFeature(() => new AuthUserSession(), new IAuthProvider[] {new BasicAuthProvider(), new TwitterAuthProvider(new AppSettings()),});
				//authFeature.HtmlRedirect 
				Plugins.Add(authFeature);
				MemoryCacheClient memoryCacheClient = new MemoryCacheClient();
				container.Register<ICacheClient>(memoryCacheClient);

				var userRepository = new InMemoryAuthRepository();
				container.Register<IUserAuthRepository>(userRepository);

				string hash;
				string salt;
				string password = "password";
				new SaltedHash().GetHashAndSaltString(password, out hash, out salt);
				userRepository.CreateUserAuth(
					new UserAuth {
						Id = 1,
						DisplayName = "JoeUser",
						Email = "Joe@user.com",
						UserName = "juser",
						FirstName = "Joe",
						LastName = "User",
						PasswordHash = hash,
						Salt = salt,
					}, password);
				//IHttpResult authenticationRequired = helloService.AuthenticationRequired(); // ????
			}

			public void SayHello(string text)
			{
				HelloService.Post(new Hello() {Name = text});
			}
		}

		//Run it!
		static void Main(string[] args)
		{
			var listeningOn = args.Length == 0 ? "http://*:1337/" : args[0];
			var appHost = new AppHost();
			appHost.Init();
			// currently the following line will throw a HttpListnerException with "Access is denied" message
			// to avoid this, run VisualStudio as administrator
			appHost.Start(listeningOn);

			Console.WriteLine("AppHost Created at {0}, listening on {1}", DateTime.Now, listeningOn);
			while (true) {
				Console.ReadKey();
				//appHost.SayHello("Bhavesh");
			}
		}
	}
}
