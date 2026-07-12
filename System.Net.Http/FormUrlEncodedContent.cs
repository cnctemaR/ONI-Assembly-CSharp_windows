using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace System.Net.Http
{
	public class FormUrlEncodedContent : ByteArrayContent
	{
		public FormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
			: base(FormUrlEncodedContent.EncodeContent(nameValueCollection))
		{
			base.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
		}

		private static byte[] EncodeContent(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
		{
			if (nameValueCollection == null)
			{
				throw new ArgumentNullException("nameValueCollection");
			}
			List<byte> list = new List<byte>();
			foreach (KeyValuePair<string, string> keyValuePair in nameValueCollection)
			{
				if (list.Count != 0)
				{
					list.Add(38);
				}
				byte[] array = FormUrlEncodedContent.SerializeValue(keyValuePair.Key);
				if (array != null)
				{
					list.AddRange(array);
				}
				list.Add(61);
				array = FormUrlEncodedContent.SerializeValue(keyValuePair.Value);
				if (array != null)
				{
					list.AddRange(array);
				}
			}
			return list.ToArray();
		}

		private static byte[] SerializeValue(string value)
		{
			if (value == null)
			{
				return null;
			}
			value = Uri.EscapeDataString(value).Replace("%20", "+");
			return Encoding.ASCII.GetBytes(value);
		}
	}
}
