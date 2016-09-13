﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using RpcLite.Config;
using RpcLite.Service;
using CoreConfig = Microsoft.Extensions.Configuration;

namespace RpcLite.AspNetCore
{
	/// <summary>
	/// 
	/// </summary>
	public class RpcLiteInitializer
	{
		/// <summary>
		/// initialize with default config file "rpclite.config.json"
		/// </summary>
		public static void Initialize()
		{
			Initialize(null, null);
		}

		/// <summary>
		/// initialize from configuration
		/// </summary>
		/// <param name="config"></param>
		public static void Initialize(CoreConfig.IConfiguration config)
		{
			var rpcConfig = RpcConfigHelper.GetConfig(new CoreConfigurationSection(config));
			//RpcLiteConfig.SetInstance(rpcConfig);

			RpcManager.Initialize(rpcConfig);
		}

		/// <summary>
		/// default initialize from rpclite.config.json
		/// </summary>
		/// <param name="app">used to UseMiddleware, keep null if not need set</param>
		public static void Initialize(IApplicationBuilder app)
		{
			//var jsonFile = "rpclite.config.json";
			Initialize(app, null);
		}

		/// <summary>
		/// <para>initialize with default config file "rpclite.config.json" in specific basePath</para>
		/// </summary>
		/// <param name="app">used to UseMiddleware, keep null if not need set</param>
		/// <param name="basePath">base path to search config file rpclite.config.json</param>
		public static void Initialize(IApplicationBuilder app, string basePath)
		{
			//Initilize(basePath);
			//app?.UseMiddleware<RpcLiteMiddleware>();

			var config = GetConfiguration(null);
			var rpcConfig = RpcConfigHelper.GetConfig(new CoreConfigurationSection(config));
			RpcManager.Initialize(rpcConfig);

			if (rpcConfig?.Service?.Paths != null)
			{
				var routers = new RouteBuilder(app);

				foreach (var path in rpcConfig.Service.Paths)
				{
					routers.MapRoute(path, context => RpcManager.ProcessAsync(new AspNetCoreServerContext(context)));
				}
				var routes1 = routers.Build();
				app.UseRouter(routes1);
			}
		}

		/// <summary>
		/// default initialize from rpclite.config.json
		/// </summary>
		/// <param name="routers">used to UseMiddleware, keep null if not need set</param>
		public static void Initialize(IRouteBuilder routers)
		{
			var config = GetConfiguration(null);
			var rpcConfig = RpcConfigHelper.GetConfig(new CoreConfigurationSection(config));
			RpcManager.Initialize(rpcConfig);

			if (rpcConfig?.Service?.Paths != null)
			{
				foreach (var path in rpcConfig.Service.Paths)
				{
					routers.MapRoute(path, context => RpcManager.ProcessAsync(new AspNetCoreServerContext(context)));
				}
			}
		}

		///// <summary>
		///// initialize RpcLite with config in specific basePath
		///// </summary>
		///// <param name="basePath"></param>
		//private static void Initilize(string basePath)
		//{
		//	var config = GetConfiguration(basePath);
		//	Initialize(config);
		//}

		private static IConfigurationRoot GetConfiguration(string basePath)
		{
			var configBuilder = new ConfigurationBuilder();

			if (!string.IsNullOrWhiteSpace(basePath))
				configBuilder.SetBasePath(basePath);

			configBuilder
				.AddJsonFile("rpclite.config.json");

			var config = configBuilder.Build();
			return config;
		}
	}
}
