﻿using System;
#if !NETCORE
using System.Runtime.Serialization;
#endif

namespace RpcLite
{
	/// <summary>
	/// Respresents request error that occored during application in server side
	/// </summary>
	public class RequestException : ServiceException
	{
		/// <summary>
		/// Initializes a new instance of RpcLite.RequestException class
		/// </summary>
		public RequestException() { }

#if NETCORE
#else
		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected RequestException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
#endif

		/// <summary>
		/// Initializes a new instance of RpcLite.RequestException class with specifid message and inner exception
		/// </summary>
		/// <param name="message">message</param>
		/// <param name="innerException">inner exception</param>
		public RequestException(string message, Exception innerException)
			: base(message, innerException)
		{ }

		/// <summary>
		/// Initializes a new instance of RpcLite.RequestException class with specifid message
		/// </summary>
		/// <param name="message">message</param>
		public RequestException(string message)
			: base(message)
		{ }
	}
}
