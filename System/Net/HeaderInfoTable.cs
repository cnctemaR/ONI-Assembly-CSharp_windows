using System;
using System.Collections;
using System.Collections.Specialized;

namespace System.Net
{
	internal class HeaderInfoTable
	{
		private static string[] ParseSingleValue(string value)
		{
			return new string[] { value };
		}

		private static string[] ParseMultiValue(string value)
		{
			StringCollection stringCollection = new StringCollection();
			bool flag = false;
			int num = 0;
			char[] array = new char[value.Length];
			int i = 0;
			while (i < value.Length)
			{
				if (value[i] == '"')
				{
					flag = !flag;
					goto IL_0059;
				}
				if (value[i] != ',' || flag)
				{
					goto IL_0059;
				}
				string text = new string(array, 0, num);
				stringCollection.Add(text.Trim());
				num = 0;
				IL_0068:
				i++;
				continue;
				IL_0059:
				array[num++] = value[i];
				goto IL_0068;
			}
			if (num != 0)
			{
				string text = new string(array, 0, num);
				stringCollection.Add(text.Trim());
			}
			string[] array2 = new string[stringCollection.Count];
			stringCollection.CopyTo(array2, 0);
			return array2;
		}

		static HeaderInfoTable()
		{
			HeaderInfo[] array = new HeaderInfo[]
			{
				new HeaderInfo("Age", false, false, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("Allow", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Accept", true, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Authorization", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Accept-Ranges", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Accept-Charset", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Accept-Encoding", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Accept-Language", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Cookie", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Connection", true, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Content-MD5", false, false, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("Content-Type", true, false, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("Cache-Control", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Content-Range", false, false, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("Content-Length", true, true, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("Content-Encoding", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Content-Language", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Content-Location", false, false, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("Date", true, false, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("ETag", false, false, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("Expect", true, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Expires", false, false, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("From", false, false, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("Host", true, false, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("If-Match", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("If-Range", false, false, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("If-None-Match", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("If-Modified-Since", true, false, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("If-Unmodified-Since", false, false, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("Keep-Alive", false, true, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("Location", false, false, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("Last-Modified", false, false, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("Max-Forwards", false, false, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("Pragma", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Proxy-Authenticate", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Proxy-Authorization", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Proxy-Connection", true, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Range", true, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Referer", true, false, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("Retry-After", false, false, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("Server", false, false, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("Set-Cookie", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Set-Cookie2", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("TE", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Trailer", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Transfer-Encoding", true, true, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Upgrade", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("User-Agent", true, false, false, HeaderInfoTable.SingleParser),
				new HeaderInfo("Via", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Vary", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("Warning", false, false, true, HeaderInfoTable.MultiParser),
				new HeaderInfo("WWW-Authenticate", false, true, true, HeaderInfoTable.SingleParser)
			};
			HeaderInfoTable.HeaderHashTable = new Hashtable(array.Length * 2, CaseInsensitiveAscii.StaticInstance);
			for (int i = 0; i < array.Length; i++)
			{
				HeaderInfoTable.HeaderHashTable[array[i].HeaderName] = array[i];
			}
		}

		internal HeaderInfo this[string name]
		{
			get
			{
				HeaderInfo headerInfo = (HeaderInfo)HeaderInfoTable.HeaderHashTable[name];
				if (headerInfo == null)
				{
					return HeaderInfoTable.UnknownHeaderInfo;
				}
				return headerInfo;
			}
		}

		private static Hashtable HeaderHashTable;

		private static HeaderInfo UnknownHeaderInfo = new HeaderInfo(string.Empty, false, false, false, HeaderInfoTable.SingleParser);

		private static HeaderParser SingleParser = new HeaderParser(HeaderInfoTable.ParseSingleValue);

		private static HeaderParser MultiParser = new HeaderParser(HeaderInfoTable.ParseMultiValue);
	}
}
