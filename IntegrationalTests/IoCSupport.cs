using Autofac;
using Autofac.Core;
using AutoMapper;
using CRM.API.Configuration;
using CRM.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CRM.IntegrationTests
{
	public class IoCSupport<TModule> where TModule : IModule, new()
	{
		private IContainer container;
		protected IMapper mapper;

		public IoCSupport()
		{
			var builder = new ContainerBuilder();
			var config = InitConfiguration();
			builder.RegisterModule(new TModule());
			var storageOptionsValue = config.Get<StorageOptions>();
			var storageOptions = Options.Create(storageOptionsValue);
			builder.RegisterInstance(storageOptions).As<IOptions<StorageOptions>>();
			var urlOptionsValue= config.Get<UrlOptions>();
			var urlOptions= Options.Create(urlOptionsValue);
			builder.RegisterInstance(urlOptions).As<IOptions<UrlOptions>>();
			var mappingConfig = new MapperConfiguration(mc =>
			{
				mc.AddProfile(new AutomapperProfile());
			});
			mapper = mappingConfig.CreateMapper();
			builder.RegisterInstance(mapper).As<IMapper>();
			container = builder.Build();
		}

		protected TEntity Resolve<TEntity>()
		{
			return container.Resolve<TEntity>();
		}

		protected void ShutdownIoC()
		{
			container.Dispose();
		}

		public static IConfiguration InitConfiguration()
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile("config.test.json")
				.Build();
			return config;
		}
	}
}
