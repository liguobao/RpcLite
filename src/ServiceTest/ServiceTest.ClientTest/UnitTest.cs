﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using RpcLite;
using RpcLite.Client;
using RpcLite.Config;
using RpcLite.Monitor.Http;
using RpcLite.Registry.Http;
using ServiceTest.Contract;
using ServiceTest.ServiceImpl;

namespace ServiceTest.ClientTest
{
	public class UnitTest
	{
		internal static void Test()
		{
			Test2();
		}

		public static void Test2()
		{
			#region prepare config

			//var config1 = new ConfigurationBuilder()
			//	.AddJsonFile("rpclite.config.json")
			//	.Build();
			//var config2 = RpcConfigHelper.GetConfig(new CoreConfiguration(config1));

			var config = new RpcLiteConfig
			{
				AppId = "10000",
				Registry = new RegistryConfigItem("HttpRegistry", typeof(HttpRegistry), "http://localhost:12974/api/service/"),
				Monitor = new MonitorConfigItem("ConsoleMonitor", typeof(HttpMonitor), "http://localhost:6201/api/service/"),
				Services = new List<ServiceConfigItem>
				{
					new ServiceConfigItem("ProductService", typeof(ProductService), "/service/"),
				},
				Clients = new List<ClientConfigItem>
				{
					new ClientConfigItem("ProductService", typeof(IProductService), "/service/"),
				}
			};

			#endregion

			var appHost = new AppHost(config);
			appHost.Initialize();

			//appHost.AddFilter(new LogTimeFilter());
			//appHost.AddFilter(new LogRequestTimeFilter());

			appHost.AddFilter(new EmptyFilter());
			appHost.AddFilter(new EmptyFilter());


			var client = appHost.ClientFactory.GetInstance<IProductService>();
			((IRpcClient)client).Channel = new MemoryClientChannel(appHost)
			{
				Address = ((IRpcClient)client).Channel.Address,
			};

			Console.WriteLine("start test");

			var id1 = client.Add(new Product
			{
				Id = 1,
			});
			Assert.AreEqual(id1, 1);

			var exObj = new PlatformNotSupportedException("win31");
			try
			{
				client.ThrowException(exObj);
			}
			catch (Exception ex)
			{
				Assert.AreEqual(ex.GetType(), exObj.GetType());
				Assert.AreEqual(ex.Message, exObj.Message);
			}

			var ps = client.GetByIdAsync(1).Result;
			Assert.AreEqual(ps.Id, 1);


			var products = client.GetAll();
			while (true)
			{
				Console.WriteLine();
				Console.Write("press enter to start 10000 test");
				Console.ReadLine();
				Console.WriteLine("testing...");

				var stopwatch = Stopwatch.StartNew();
				var times = 10000;
				for (int i = 0; i < times; i++)
				{
					var products2 = client.GetAll();
				}

				stopwatch.Stop();
				Console.WriteLine($"Elapsed: {stopwatch.Elapsed.TotalMilliseconds}, {times * 1000 / stopwatch.Elapsed.TotalMilliseconds} tps");
			}

			Console.ReadLine();
		}

		public static void Test1()
		{
			Console.WriteLine("start test");
			//Console.ReadLine();

			var config = new ConfigurationBuilder()
				.AddJsonFile("rpclite.config.json")
				.Build();

			var appHost = new AppHost(config);
			appHost.Initialize();

			var client = appHost.ClientFactory.GetInstance<IProductService>();

			var channel = new MemoryClientChannel(appHost) { Address = "/api/service/" };

			((IRpcClient)client).Channel = channel;
			var products = client.GetAll();

			while (true)
			{
				Console.WriteLine("press enter to start 10000 test");
				Console.ReadLine();

				var stopwatch = Stopwatch.StartNew();
				var times = 10000;
				for (int i = 0; i < times; i++)
				{
					var products2 = client.GetAll();
				}

				stopwatch.Stop();
				Console.WriteLine($"Elapsed: {stopwatch.Elapsed.TotalMilliseconds}, {times * 1000 / stopwatch.Elapsed.TotalMilliseconds} tps");
			}

			Console.ReadLine();
		}
	}
}