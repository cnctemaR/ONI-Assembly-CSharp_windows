using System;
using System.Text.RegularExpressions;
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
			Uri uri = new Uri(localUrl);
			if (targetUrl.StartsWith("//"))
			{
				targetUrl = uri.Scheme + ":" + targetUrl;
			}
			if (targetUrl.StartsWith("/"))
			{
				targetUrl = uri.Scheme + "://" + uri.Host + targetUrl;
			}
			if (WebRequestUtils.domainRegex.IsMatch(targetUrl))
			{
				targetUrl = uri.Scheme + "://" + targetUrl;
			}
			Uri uri2 = null;
			try
			{
				uri2 = new Uri(targetUrl);
			}
			catch (FormatException ex)
			{
				try
				{
					uri2 = new Uri(uri, targetUrl);
				}
				catch (FormatException)
				{
					throw ex;
				}
			}
			return (!targetUrl.Contains("%")) ? uri2.AbsoluteUri : uri2.OriginalString;
		}

		private static Regex domainRegex = new Regex("^\\s*\\w+(?:\\.\\w+)+\\s*$");
	}
}
