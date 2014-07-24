﻿using System;
using System.IO;
using Newtonsoft.Json;

namespace RpcLite.Utility
{
	public static class JsonHelper
	{
		public static object Deserialize(Stream stream, Type targetType)
		{
			using (var reader = new StreamReader(stream))
			{
				return Deserialize(targetType, reader);
			}
		}

		private static object Deserialize(Type targetType, TextReader reader)
		{
			using (var jsonReader = new JsonTextReader(reader))
			{
				var jsonSerializer = new JsonSerializer();
				var obj = jsonSerializer.Deserialize(jsonReader, targetType);
				return obj;
			}
		}

		public static void Serialize(Stream stream, object source)
		{
			using (var writer = new StreamWriter(stream))
			{
				Serialize(source, writer);
			}
		}

		private static void Serialize(object source, TextWriter writer)
		{
			using (var jsonWriter = new JsonTextWriter(writer))
			{
				var jsonSerializer = new JsonSerializer();
				jsonSerializer.Serialize(jsonWriter, source);
			}
		}
	}
}
