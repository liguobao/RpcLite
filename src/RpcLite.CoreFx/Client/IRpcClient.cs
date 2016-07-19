﻿using RpcLite.Formatters;

namespace RpcLite.Client
{
	public interface IRpcClient
	{
		/// <summary>
		/// base url of service
		/// </summary>
		string BaseUrl { get; set; }

		/// <summary>
		/// Formatter
		/// </summary>
		IFormatter Formatter { get; set; }
	}
}