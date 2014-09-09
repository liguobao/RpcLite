﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Hrj.Net
{
	public class WebRequestHelper
	{
		public static string PostData(string url, string postData, Encoding encoding, Dictionary<string, string> headDic)
		{
			var request = WebRequest.Create(url) as HttpWebRequest;
			request.Method = "POST";
			if (headDic != null)
			{
				foreach (var head in headDic)
				{
					if (head.Key == "Content-Type")
						request.ContentType = head.Value;
					else if (head.Key == "Accept")
						request.Accept = head.Value;
					else
						request.Headers.Add(head.Key, head.Value);
				}
			}

			if (!string.IsNullOrEmpty(postData))
			{
				var requestStream = request.GetRequestStream();
				if (encoding == null)
					encoding = Encoding.UTF8;
				var writer = new StreamWriter(requestStream, encoding);
				writer.Write(postData);
				writer.Close();
			}

			var response = request.GetResponse();

			if (response.ContentLength > 0)
			{
				var reader = new StreamReader(response.GetResponseStream(), encoding);
				var jsonResult = reader.ReadToEnd();
				return jsonResult;
			}
			response.Close();
			return null;
		}
	}
}