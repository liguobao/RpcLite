﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace RpcLite.Formatters
{

	/// <summary>
	/// 
	/// </summary>
	public class XmlFormatter : IFormatter
	{
		/// <summary>
		/// 
		/// </summary>
		public XmlFormatter()
		{
			SupportMimes.Add("text/xml");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="inputStream"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		public object Deserialize(Stream inputStream, Type targetType)
		{
			var reader = new StreamReader(inputStream);
			return Deserialize(reader, targetType);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="targetType"></param>
		/// <returns></returns>
		public object Deserialize(TextReader reader, Type targetType)
		{
			var serializer = new XmlSerializer(targetType);
			return serializer.Deserialize(reader);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="outputStream"></param>
		/// <param name="source"></param>
		public void Serialize(Stream outputStream, object source)
		{
			var writer = new StreamWriter(outputStream);
			Serialize(writer, source);
			writer.Flush();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="source"></param>
		public void Serialize(TextWriter writer, object source)
		{
			var serializer = new XmlSerializer(source.GetType());
			serializer.Serialize(writer, source);
		}

		/// <summary>
		/// 
		/// </summary>
		public List<string> SupportMimes { get; } = new List<string>();
	}
}
