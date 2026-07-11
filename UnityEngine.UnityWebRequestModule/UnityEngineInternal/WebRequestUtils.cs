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
			Uri uri;
			if (redirectUri[0] == '/')
			{
				uri = new Uri(redirectUri, UriKind.Relative);
			}
			else
			{
				uri = new Uri(redirectUri, UriKind.RelativeOrAbsolute);
			}
			string text;
			if (uri.IsAbsoluteUri)
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
			string text;
			if (string.IsNullOrEmpty(targetUrl))
			{
				text = "";
			}
			else
			{
				bool flag = false;
				Uri uri = new Uri(localUrl);
				Uri uri2 = null;
				if (targetUrl[0] == '/')
				{
					uri2 = new Uri(uri, targetUrl);
					flag = true;
				}
				if (uri2 == null && WebRequestUtils.domainRegex.IsMatch(targetUrl))
				{
					targetUrl = uri.Scheme + "://" + targetUrl;
					flag = true;
				}
				FormatException ex = null;
				try
				{
					if (uri2 == null && targetUrl[0] != '.')
					{
						uri2 = new Uri(targetUrl);
					}
				}
				catch (FormatException ex2)
				{
					ex = ex2;
				}
				if (uri2 == null)
				{
					try
					{
						uri2 = new Uri(uri, targetUrl);
						flag = true;
					}
					catch (FormatException)
					{
						throw ex;
					}
				}
				text = WebRequestUtils.MakeUriString(uri2, targetUrl, flag);
			}
			return text;
		}

		internal static string MakeUriString(Uri targetUri, string targetUrl, bool prependProtocol)
		{
			string text;
			if (targetUri.IsFile)
			{
				if (!targetUri.IsLoopback)
				{
					text = targetUri.OriginalString;
				}
				else
				{
					string text2 = targetUri.AbsolutePath;
					if (text2.Contains("%"))
					{
						text2 = WebRequestUtils.URLDecode(text2);
					}
					if (text2.Length > 0 && text2[0] != '/')
					{
						text2 = '/' + text2;
					}
					text = "file://" + text2;
				}
			}
			else if (targetUrl.Contains("%"))
			{
				text = targetUri.OriginalString;
			}
			else
			{
				string scheme = targetUri.Scheme;
				if (!prependProtocol && targetUrl.Length >= scheme.Length + 2 && targetUrl[scheme.Length + 1] != '/')
				{
					StringBuilder stringBuilder = new StringBuilder(scheme, targetUrl.Length);
					stringBuilder.Append(':');
					if (scheme == "jar")
					{
						string text3 = targetUri.AbsolutePath;
						if (text3.Contains("%"))
						{
							text3 = WebRequestUtils.URLDecode(text3);
						}
						stringBuilder.Append(text3);
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
					text = targetUri.AbsoluteUri;
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
