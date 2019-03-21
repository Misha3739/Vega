using System;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using vega.Controllers.Repositories;
using vega.Persistence;
using vega.Persistence.Repositories;
using vega.Utility;

namespace vega {
	public class Startup {
		public Startup(IConfiguration configuration) {
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services) {
			services.AddAutoMapper();

			services.AddSingleton<IEncryption, EncryptionUtility>();
			services.AddSingleton<IToken, TokenUtility>();

			services.AddScoped<IVegaDbContext, VegaDbContext>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();

			services.AddTransient<IVehiclesRepository, VehiclesRepository>();
			services.AddTransient<IFeaturesRepository, FeaturesRepository>();
			services.AddTransient<IMakesRepository, MakesRepository>();
			services.AddTransient<IAccountRepository, AccountRepository>();
			
			

			services.AddDbContext<VegaDbContext>(options =>
				options.UseSqlServer(Configuration.GetConnectionString("Default")));

			services.AddMvc();

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(jwtBearerOptions => {
					jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters {
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(new EncryptionUtility().GetPrivateTokenKey()),
						ValidateIssuer = false,
						ValidateAudience = false,
						ValidateLifetime = true,
						ClockSkew = TimeSpan.FromMinutes(5)
					};
				});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
				app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {
					HotModuleReplacement = true
				});
			}
			else {
				app.UseExceptionHandler("/Home/Error");
			}

			app.UseStaticFiles();

			app.UseAuthentication();
			app.UseMvc(routes => {
				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");

				routes.MapSpaFallbackRoute(
					name: "spa-fallback",
					defaults: new {controller = "Home", action = "Index"});
			});
		}
	}
}