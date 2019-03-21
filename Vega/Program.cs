using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace vega {
	public class Program {
		public static void Main(string[] args) {
			BuildWebHost(args).Run();
		}

		public static IWebHost BuildWebHost(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.UseStartup<Startup>()
				.UseKestrel(options => {
					options.Listen(IPAddress.Loopback, 5000);
					options.Listen(IPAddress.Loopback, 44360, listenOptions => {
						var serverCertificate = LoadCertificate();
						listenOptions.UseHttps(serverCertificate); // <- Configures SSL
					});
				})
				.Build();

		private static X509Certificate2 LoadCertificate() {
			var assembly = typeof(Startup).GetTypeInfo().Assembly;
			var embeddedFileProvider = new EmbeddedFileProvider(assembly, "vega");
			var certificateFileInfo = embeddedFileProvider.GetFileInfo(Path.Combine("Certificates","localhost.pfx"));
			using (var certificateStream = certificateFileInfo.CreateReadStream()) {
				byte[] certificatePayload;
				using (var memoryStream = new MemoryStream()) {
					certificateStream.CopyTo(memoryStream);
					certificatePayload = memoryStream.ToArray();
				}

				return new X509Certificate2(certificatePayload, "passw0rd!");
			}
		}
	}

	
}
