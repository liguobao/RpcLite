﻿using System;
using System.Web;

namespace RpcLite
{
	public class RpcLiteModule : IHttpModule
	{
		#region IHttpModule Members

		public void Dispose()
		{
		}

		public void Init(HttpApplication context)
		{
			//context.PostResolveRequestCache += context_PostResolveRequestCache;
			context.MapRequestHandler += context_MapRequestHandler;
		}

		void context_MapRequestHandler(object sender, EventArgs e)
		{
			//HttpContext.Current.RemapHandler()
		}

		void context_PostResolveRequestCache(object sender, EventArgs e)
		{
			//			HttpContext.Current.RemapHandler(
		}

		#endregion
	}
}