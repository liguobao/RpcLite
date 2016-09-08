﻿using RpcLite.Formatters;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	public interface IRpcClient<TContract> : IRpcClient
	{
		/// <summary>
		/// Channel to transport data with service
		/// </summary>
		ICluster<TContract> Cluster { get; set; }
	}

	/// <summary>
	/// 
	/// </summary>
	public interface IRpcClient
	{
		/// <summary>
		/// base url of service
		/// </summary>
		string Address { get; set; }

		/// <summary>
		/// Formatter
		/// </summary>
		IFormatter Formatter { get; set; }
	}

}