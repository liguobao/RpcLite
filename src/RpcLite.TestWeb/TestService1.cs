﻿using System;
using System.Globalization;

namespace RpcLite.TestWeb
{
	public class TestService1
	{
		public string GetDateTimeString()
		{
			return DateTime.Now.ToString(CultureInfo.InvariantCulture);

		}
	}
}