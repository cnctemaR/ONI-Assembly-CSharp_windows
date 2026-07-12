using System;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Scripting;

namespace UnityEngineInternal
{
	internal static class WebRequestUtils
	{
		[RequiredByNativeCode]
		internal static string RedirectTo(string baseUri, string redirectUri)
		{
			bool flag = redirectUri[0] == '/';
			Uri uri;
			if (flag)
			{
				uri = new Uri(redirectUri, UriKind.Relative);
			}
			else
			{
				uri = new Uri(redirectUri, UriKind.RelativeOrAbsolute);
			}
			bool isAbsoluteUri = uri.IsAbsoluteUri;
			string text;
			if (isAbsoluteUri)
			{
				text = uri.AbsoluteUri;
			}
			else
			{
				Uri uri2 = new Uri(baseUri, UriKind.Absolute);
				Uri uri3 = new Uri(uri2, uri);
				text = uri3.AbsoluteUri;
			}
			return text;
		}

		internal static string MakeInitialUrl(string targetUrl, string localUrl)
		{
			bool flag = string.IsNullOrEmpty(targetUrl);
			string text;
			if (flag)
			{
				text = "";
			}
			else
			{
				bool flag2 = false;
				Uri uri = new Uri(localUrl);
				Uri uri2 = null;
				bool flag3 = targetUrl[0] == '/';
				if (flag3)
				{
					uri2 = new Uri(uri, targetUrl);
					flag2 = true;
				}
				bool flag4 = uri2 == null && WebRequestUtils.domainRegex.IsMatch(targetUrl);
				if (flag4)
				{
					targetUrl = uri.Scheme + "://" + targetUrl;
					flag2 = true;
				}
				FormatException ex = null;
				try
				{
					bool flag5 = uri2 == null && targetUrl[0] != '.';
					if (flag5)
					{
						uri2 = new Uri(targetUrl);
					}
				}
				catch (FormatException ex2)
				{
					ex = ex2;
				}
				bool flag6 = uri2 == null;
				if (flag6)
				{
					try
					{
						uri2 = new Uri(uri, targetUrl);
						flag2 = true;
					}
					catch (FormatException)
					{
						throw ex;
					}
				}
				text = WebRequestUtils.MakeUriString(uri2, targetUrl, flag2);
			}
			return text;
		}

		internal static string MakeUriString(Uri targetUri, string targetUrl, bool prependProtocol)
		{
			bool isFile = targetUri.IsFile;
			string text;
			if (isFile)
			{
				bool flag = !targetUri.IsLoopback;
				if (flag)
				{
					text = targetUri.OriginalString;
				}
				else
				{
					string text2 = targetUri.AbsolutePath;
					bool flag2 = text2.Contains("%");
					if (flag2)
					{
						bool flag3 = text2.Contains('+');
						if (flag3)
						{
							string originalString = targetUri.OriginalString;
							bool flag4 = !originalString.StartsWith("file:");
							if (flag4)
							{
								return "file:///" + originalString.Replace('\\', '/');
							}
						}
						text2 = WebRequestUtils.URLDecode(text2);
					}
					bool flag5 = text2.Length > 0 && text2[0] != '/';
					if (flag5)
					{
						text2 = "/" + text2;
					}
					text = "file://" + text2;
				}
			}
			else
			{
				string scheme = targetUri.Scheme;
				bool flag6 = !prependProtocol && targetUrl.Length >= scheme.Length + 2 && targetUrl[scheme.Length + 1] != '/';
				if (flag6)
				{
					StringBuilder stringBuilder = new StringBuilder(scheme, targetUrl.Length);
					stringBuilder.Append(':');
					bool flag7 = scheme == "jar";
					if (flag7)
					{
						string text3 = targetUri.AbsolutePath;
						bool flag8 = text3.Contains("%");
						if (flag8)
						{
							text3 = WebRequestUtils.URLDecode(text3);
						}
						bool flag9 = text3.StartsWith("file:/") && text3.Length > 6 && text3[6] != '/';
						if (flag9)
						{
							stringBuilder.Append("file://");
							stringBuilder.Append(text3.Substring(5));
						}
						else
						{
							stringBuilder.Append(text3);
						}
						text = stringBuilder.ToString();
					}
					else
					{
						stringBuilder.Append(targetUri.PathAndQuery);
						stringBuilder.Append(targetUri.Fragment);
						text = stringBuilder.ToString();
					}
				}
				else
				{
					bool flag10 = targetUrl.Contains("%");
					if (flag10)
					{
						text = targetUri.OriginalString;
					}
					else
					{
						text = targetUri.AbsoluteUri;
					}
				}
			}
			return text;
		}

		private static string URLDecode(string encoded)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(encoded);
			byte[] array = WWWTranscoder.URLDecode(bytes);
			return Encoding.UTF8.GetString(array);
		}

		private static Regex domainRegex = new Regex("^\\s*\\w+(?:\\.\\w+)+(\\/.*)?$");
	}
}
