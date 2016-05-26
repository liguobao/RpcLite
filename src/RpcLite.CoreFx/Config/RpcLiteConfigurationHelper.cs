﻿using System;
using System.Linq;

namespace RpcLite.Config
{
	/// <summary>
	/// used to get RpcLiteConfig from IConfiguration
	/// </summary>
	public class RpcLiteConfigurationHelper
	{
		/// <summary>
		/// get RpcLiteConfig from IConfiguration
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public static RpcLiteConfig GetConfig(IConfiguration config)
		{
			var version = GetVersion(config);

			IRpcLiteParser parser;
			if (version <= new Version(1, 0))
			{
				parser = new RpcLiteParserV1();
			}
			//else if (version == new Version(1, 1))
			//{
			//	parser = new RpcLiteParserV11();
			//}
			else
			{
				parser = new RpcLiteParserV11();
			}

			var instance = parser.GetConfig(config);
			return instance;
		}

		internal static Version GetVersion(IConfiguration config)
		{
			var versionText = config["version"];
			Version version;
			if (versionText == null || !Version.TryParse(versionText, out version))
				version = new Version("1.0");
			return version;
		}
	}

	interface IRpcLiteParser
	{
		RpcLiteConfig GetConfig(IConfiguration config);
	}

	internal class RpcLiteParserV1 : IRpcLiteParser
	{
		/// <summary>
		/// get RpcLiteConfig from IConfiguration
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public RpcLiteConfig GetConfig(IConfiguration config)
		{
			var instance = new RpcLiteConfig
			{
				AppId = config["appId"],
				Environment = config["environment"],
				Version = RpcLiteConfigurationHelper.GetVersion(config),
			};

			InitializeResolverConfig(config, instance);
			InitializeServiceConfig(config, instance);
			InitializeClientConfig(config, instance);

			return instance;
		}

		// ReSharper disable once FunctionComplexityOverflow
		private static void InitializeClientConfig(IConfiguration config, RpcLiteConfig instance)
		{
			var clientsNode = config.GetSection("clients");
			if (clientsNode != null)
			{
				var environment = clientsNode["environment"];
				instance.ClientEnvironment = !string.IsNullOrWhiteSpace(environment) ? environment : instance.Environment;

				var clients = clientsNode.GetChildren();
				foreach (var item in clients)
				{
					var name = item["name"]; // GetAttribute("name", item);
					var path = item["path"]; //GetAttribute("path", item);
					var type = item["type"]; //GetAttribute("type", item);
					var nameSpace = item["namespace"]; // GetAttribute("namespace", item);

					if (string.IsNullOrEmpty(name))
						throw new RpcLiteConfigurationErrorException("name of RpcLite configuration client node can't be null or empty");
					//if (string.IsNullOrEmpty(path))
					//	throw new ConfigurationErrorsException("path of RpcLite configuration node can't be null or empty");
					if (string.IsNullOrEmpty(type))
						throw new RpcLiteConfigurationErrorException(string.Format("type of RpcLite configuration client node '{0}' can't be null or empty", name));
					if (string.IsNullOrEmpty(nameSpace))
						throw new RpcLiteConfigurationErrorException(string.Format("namespace of RpcLite configuration client node '{0}' can't be null or empty", name));

					//try
					//{
					string typeName;
					string assemblyName;

					var splitorIndex = type.IndexOf(",", StringComparison.Ordinal);
					if (splitorIndex > -1)
					{
						typeName = type.Substring(0, splitorIndex);
						assemblyName = type.Substring(splitorIndex + 1);
					}
					else
					{
						typeName = type;
						assemblyName = null;
					}

					var serviceConfigItem = new ClientConfigItem
					{
						Name = name,
						Type = type,
						TypeName = typeName,
						AssemblyName = assemblyName,
						Path = path,
						Namespace = nameSpace,
					};
					instance.Clients.Add(serviceConfigItem);
				}
			}
		}

		private static void InitializeResolverConfig(IConfiguration config, RpcLiteConfig instance)
		{
			try
			{
				var resolverNode = config.GetSection("addressResolver");
				if (resolverNode != null && resolverNode.GetChildren().Any())
				{
					//foreach (XmlNode item in resolverNode.ChildNodes)
					{
						//if (resolverNode.Name != "add" || resolverNode.Attributes == null)
						//	continue;

						var name = resolverNode["name"]; //GetAttribute("name", resolverNode);
						var type = resolverNode["type"]; //GetAttribute("type", resolverNode);

						if (string.IsNullOrEmpty(name))
							throw new RpcLiteConfigurationErrorException("name of RpcLite configuration addressResolver node can't be null or empty");
						if (string.IsNullOrEmpty(type))
							throw new RpcLiteConfigurationErrorException("type of RpcLite configuration addressResolver node " + name + " can't be null or empty");

						string typeName;
						string assemblyName;

						var splitorIndex = type.IndexOf(",", StringComparison.Ordinal);
						if (splitorIndex > -1)
						{
							typeName = type.Substring(0, splitorIndex);
							assemblyName = type.Substring(splitorIndex + 1);
						}
						else
						{
							typeName = type;
							assemblyName = null;
						}

						var resolver = new ResolverConfigItem
						{
							Name = name,
							Type = type,
							TypeName = typeName,
							AssemblyName = assemblyName,
						};
						instance.Resover = resolver;
					}
				}
			}
			catch (Exception ex)
			{
				throw new ConfigException("Client Configuration Error", ex);
			}
		}

		private static void InitializeServiceConfig(IConfiguration config, RpcLiteConfig instance)
		{
			try
			{
				var servicesNode = config.GetSection("services");
				if (servicesNode != null)
				{
					var serviceItemNodes = servicesNode.GetChildren();
					foreach (var item in serviceItemNodes)
					{
						var name = item["name"];
						var path = item["path"];
						var type = item["type"];

						if (string.IsNullOrEmpty(name))
							throw new RpcLiteConfigurationErrorException("name of RpcLite service configuration node can't be null or empty");
						if (string.IsNullOrEmpty(path))
							throw new RpcLiteConfigurationErrorException("path of RpcLite service configuration node " + name + " can't be null or empty");
						if (string.IsNullOrEmpty(type))
							throw new RpcLiteConfigurationErrorException("type of RpcLite service configuration node " + name + " can't be null or empty");

						string typeName;
						string assemblyName;

						var splitorIndex = type.IndexOf(",", StringComparison.Ordinal);
						if (splitorIndex > -1)
						{
							typeName = type.Substring(0, splitorIndex);
							assemblyName = type.Substring(splitorIndex + 1);
						}
						else
						{
							typeName = type;
							assemblyName = null;
						}

						if (string.IsNullOrWhiteSpace(assemblyName))
							throw new RpcLiteConfigurationErrorException("assembly can't be null or empty, in RpcLite service configuration node " + name);

						var serviceConfigItem = new ServiceConfigItem
						{
							Name = name,
							Type = type,
							TypeName = typeName,
							AssemblyName = assemblyName,
							Path = path,
						};
						instance.Services.Add(serviceConfigItem);
					}
				}
			}
			catch (Exception ex)
			{
				throw new ConfigException("Service Configuration Error", ex);
			}
		}
	}

	internal class RpcLiteParserV11 : IRpcLiteParser
	{
		/// <summary>
		/// get RpcLiteConfig from IConfiguration
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public RpcLiteConfig GetConfig(IConfiguration config)
		{
			var instance = new RpcLiteConfig
			{
				AppId = config["appId"],
				Environment = config["environment"],
			};

			//InitializeResolverConfig(config, instance);
			InitializeServiceConfig(config, instance);
			InitializeClientConfig(config, instance);

			return instance;
		}

		// ReSharper disable once FunctionComplexityOverflow
		private static void InitializeClientConfig(IConfiguration config, RpcLiteConfig instance)
		{
			var clientNode = config.GetSection("client");
			if (clientNode == null) return;

			var clientsNode = config.GetSection("clients");
			if (clientsNode != null)
			{
				var environment = clientsNode["environment"];
				instance.ClientEnvironment = !string.IsNullOrWhiteSpace(environment) ? environment : instance.Environment;

				var clients = clientsNode.GetChildren();
				foreach (var item in clients)
				{
					var name = item["name"]; // GetAttribute("name", item);
					var path = item["path"]; //GetAttribute("path", item);
					var type = item["type"]; //GetAttribute("type", item);
					var nameSpace = item["namespace"]; // GetAttribute("namespace", item);

					if (string.IsNullOrEmpty(name))
						throw new RpcLiteConfigurationErrorException("name of RpcLite configuration client node can't be null or empty");
					//if (string.IsNullOrEmpty(path))
					//	throw new ConfigurationErrorsException("path of RpcLite configuration node can't be null or empty");
					if (string.IsNullOrEmpty(type))
						throw new RpcLiteConfigurationErrorException(string.Format("type of RpcLite configuration client node '{0}' can't be null or empty", name));
					if (string.IsNullOrEmpty(nameSpace))
						throw new RpcLiteConfigurationErrorException(string.Format("namespace of RpcLite configuration client node '{0}' can't be null or empty", name));

					//try
					//{
					string typeName;
					string assemblyName;

					var splitorIndex = type.IndexOf(",", StringComparison.Ordinal);
					if (splitorIndex > -1)
					{
						typeName = type.Substring(0, splitorIndex);
						assemblyName = type.Substring(splitorIndex + 1);
					}
					else
					{
						typeName = type;
						assemblyName = null;
					}

					var serviceConfigItem = new ClientConfigItem
					{
						Name = name,
						Type = type,
						TypeName = typeName,
						AssemblyName = assemblyName,
						Path = path,
						Namespace = nameSpace,
					};
					instance.Clients.Add(serviceConfigItem);
				}
			}

			var resolverNode = config.GetSection("resolver");
			if (resolverNode != null)
			{
				InitializeResolverConfig(resolverNode, instance);
			}
		}

		private static void InitializeResolverConfig(IConfiguration config, RpcLiteConfig instance)
		{
			try
			{
				var resolverNode = config.GetSection("resolver");
				if (resolverNode != null && resolverNode.GetChildren().Any())
				{
					//foreach (XmlNode item in resolverNode.ChildNodes)
					{
						//if (resolverNode.Name != "add" || resolverNode.Attributes == null)
						//	continue;

						var name = resolverNode["name"]; //GetAttribute("name", resolverNode);
						var type = resolverNode["type"]; //GetAttribute("type", resolverNode);

						if (string.IsNullOrEmpty(name))
							throw new RpcLiteConfigurationErrorException("name of RpcLite configuration addressResolver node can't be null or empty");
						if (string.IsNullOrEmpty(type))
							throw new RpcLiteConfigurationErrorException("type of RpcLite configuration addressResolver node " + name + " can't be null or empty");

						string typeName;
						string assemblyName;

						var splitorIndex = type.IndexOf(",", StringComparison.Ordinal);
						if (splitorIndex > -1)
						{
							typeName = type.Substring(0, splitorIndex);
							assemblyName = type.Substring(splitorIndex + 1);
						}
						else
						{
							typeName = type;
							assemblyName = null;
						}

						var resolver = new ResolverConfigItem
						{
							Name = name,
							Type = type,
							TypeName = typeName,
							AssemblyName = assemblyName,
						};
						instance.Resover = resolver;
					}
				}
			}
			catch (Exception ex)
			{
				throw new ConfigException("Client Configuration Error", ex);
			}
		}

		private static void InitializeServiceConfig(IConfiguration config, RpcLiteConfig instance)
		{
			try
			{
				var serviceNode = config.GetSection("service");
				if (serviceNode == null) return;

				var servicesNode = config.GetSection("services");
				if (servicesNode != null)
				{
					var serviceItemNodes = servicesNode.GetChildren();
					foreach (var item in serviceItemNodes)
					{
						var name = item["name"];
						var path = item["path"];
						var type = item["type"];

						if (string.IsNullOrEmpty(name))
							throw new RpcLiteConfigurationErrorException("name of RpcLite service configuration node can't be null or empty");
						if (string.IsNullOrEmpty(path))
							throw new RpcLiteConfigurationErrorException("path of RpcLite service configuration node " + name + " can't be null or empty");
						if (string.IsNullOrEmpty(type))
							throw new RpcLiteConfigurationErrorException("type of RpcLite service configuration node " + name + " can't be null or empty");

						string typeName;
						string assemblyName;

						var splitorIndex = type.IndexOf(",", StringComparison.Ordinal);
						if (splitorIndex > -1)
						{
							typeName = type.Substring(0, splitorIndex);
							assemblyName = type.Substring(splitorIndex + 1);
						}
						else
						{
							typeName = type;
							assemblyName = null;
						}

						if (string.IsNullOrWhiteSpace(assemblyName))
							throw new RpcLiteConfigurationErrorException("assembly can't be null or empty, in RpcLite service configuration node " + name);

						var serviceConfigItem = new ServiceConfigItem
						{
							Name = name,
							Type = type,
							TypeName = typeName,
							AssemblyName = assemblyName,
							Path = path,
						};
						instance.Services.Add(serviceConfigItem);
					}
				}
			}
			catch (Exception ex)
			{
				throw new ConfigException("Service Configuration Error", ex);
			}
		}
	}
}