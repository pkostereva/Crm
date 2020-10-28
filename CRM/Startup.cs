using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Autofac;
using CRM.Core;
using AutoMapper;
using CRM.API.Configuration;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System;
using System.IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CRM.API.Auth;
using CRM.Core.ConfigurationOptions;
using System.Configuration;

namespace CRM.API
{
	public class Startup
	{

		const string salt = "f7365a98-8b9e-41ec-811b-050c73f69594";
		public Startup(IWebHostEnvironment env)
		{
			var builder = new ConfigurationBuilder();

			builder.AddJsonFile("config.json", false, true);
			if (env.IsDevelopment())
				builder.AddJsonFile("config.development.json", false, true);

			builder.AddUserSecrets(salt);

			Configuration = builder.Build();
		}

		public IConfiguration Configuration { get; set; }

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseSwagger();

			app.UseSwaggerUI(c => c.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "CRM.API V1"));

			app.UseStaticFiles();

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
		public void ConfigureContainer(ContainerBuilder builder)
		{
			builder.RegisterModule(new AutofacModule());

		}
		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();

			services.Configure<StorageOptions>(Configuration);
			services.Configure<UrlOptions>(Configuration);
			services.Configure<PayPalOptions>(Configuration);

			//services.AddAuthentication(options =>
			//{
			//	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
			//	options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
			//})
			//.AddCookie()
			//.AddGoogle(options =>
			//{
			//	IConfigurationSection googleAuthNSection =
			//		Configuration.GetSection("Authentication:Google");

			//	options.ClientId = googleAuthNSection["ClientId"];
			//	options.ClientSecret = googleAuthNSection["ClientSecret"];

			//});

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
					.AddJwtBearer(options =>
					{
						options.RequireHttpsMetadata = false;
						options.TokenValidationParameters = new TokenValidationParameters
						{
							ValidateIssuer = true,
							ValidIssuer = AuthOptions.ISSUER,
							ValidAudience = AuthOptions.AUDIENCE,
							ValidateLifetime = true,
							IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
							ValidateIssuerSigningKey = true,
						};
					});

			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc(name: "v1", new OpenApiInfo { Title = "CRM.API", Version = "v1" });

				// Set the comments path for the Swagger JSON and UI.
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);
			});
			// Auto Mapper Configurations
			var mappingConfig = new MapperConfiguration(mc =>
			{
				mc.AddProfile(new AutomapperProfile());
			});

			IMapper mapper = mappingConfig.CreateMapper();
			services.AddSingleton(mapper);

		}
	}
}
