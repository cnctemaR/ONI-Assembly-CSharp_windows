using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Net
{
	internal static class UnsafeNclNativeMethods
	{
		internal static class HttpApi
		{
			private const int HttpHeaderRequestMaximum = 41;

			private const int HttpHeaderResponseMaximum = 30;

			private static string[] m_Strings = new string[]
			{
				"Cache-Control", "Connection", "Date", "Keep-Alive", "Pragma", "Trailer", "Transfer-Encoding", "Upgrade", "Via", "Warning",
				"Allow", "Content-Length", "Content-Type", "Content-Encoding", "Content-Language", "Content-Location", "Content-MD5", "Content-Range", "Expires", "Last-Modified",
				"Accept-Ranges", "Age", "ETag", "Location", "Proxy-Authenticate", "Retry-After", "Server", "Set-Cookie", "Vary", "WWW-Authenticate"
			};

			internal static class HTTP_REQUEST_HEADER_ID
			{
				internal static string ToString(int position)
				{
					return UnsafeNclNativeMethods.HttpApi.HTTP_REQUEST_HEADER_ID.m_Strings[position];
				}

				private static string[] m_Strings = new string[]
				{
					"Cache-Control", "Connection", "Date", "Keep-Alive", "Pragma", "Trailer", "Transfer-Encoding", "Upgrade", "Via", "Warning",
					"Allow", "Content-Length", "Content-Type", "Content-Encoding", "Content-Language", "Content-Location", "Content-MD5", "Content-Range", "Expires", "Last-Modified",
					"Accept", "Accept-Charset", "Accept-Encoding", "Accept-Language", "Authorization", "Cookie", "Expect", "From", "Host", "If-Match",
					"If-Modified-Since", "If-None-Match", "If-Range", "If-Unmodified-Since", "Max-Forwards", "Proxy-Authorization", "Referer", "Range", "Te", "Translate",
					"User-Agent"
				};
			}

			internal static class HTTP_RESPONSE_HEADER_ID
			{
				static HTTP_RESPONSE_HEADER_ID()
				{
					for (int i = 0; i < 30; i++)
					{
						UnsafeNclNativeMethods.HttpApi.HTTP_RESPONSE_HEADER_ID.m_Hashtable.Add(UnsafeNclNativeMethods.HttpApi.m_Strings[i], i);
					}
				}

				internal static int IndexOfKnownHeader(string HeaderName)
				{
					object obj = UnsafeNclNativeMethods.HttpApi.HTTP_RESPONSE_HEADER_ID.m_Hashtable[HeaderName];
					if (obj != null)
					{
						return (int)obj;
					}
					return -1;
				}

				internal static string ToString(int position)
				{
					return UnsafeNclNativeMethods.HttpApi.m_Strings[position];
				}

				private static Hashtable m_Hashtable = new Hashtable(30);
			}

			internal enum Enum
			{
				HttpHeaderCacheControl,
				HttpHeaderConnection,
				HttpHeaderDate,
				HttpHeaderKeepAlive,
				HttpHeaderPragma,
				HttpHeaderTrailer,
				HttpHeaderTransferEncoding,
				HttpHeaderUpgrade,
				HttpHeaderVia,
				HttpHeaderWarning,
				HttpHeaderAllow,
				HttpHeaderContentLength,
				HttpHeaderContentType,
				HttpHeaderContentEncoding,
				HttpHeaderContentLanguage,
				HttpHeaderContentLocation,
				HttpHeaderContentMd5,
				HttpHeaderContentRange,
				HttpHeaderExpires,
				HttpHeaderLastModified,
				HttpHeaderAcceptRanges,
				HttpHeaderAge,
				HttpHeaderEtag,
				HttpHeaderLocation,
				HttpHeaderProxyAuthenticate,
				HttpHeaderRetryAfter,
				HttpHeaderServer,
				HttpHeaderSetCookie,
				HttpHeaderVary,
				HttpHeaderWwwAuthenticate,
				HttpHeaderResponseMaximum,
				HttpHeaderMaximum = 41
			}
		}

		internal static class SecureStringHelper
		{
			internal static string CreateString(SecureString secureString)
			{
				IntPtr intPtr = IntPtr.Zero;
				if (secureString == null || secureString.Length == 0)
				{
					return string.Empty;
				}
				string text;
				try
				{
					intPtr = Marshal.SecureStringToGlobalAllocUnicode(secureString);
					text = Marshal.PtrToStringUni(intPtr);
				}
				finally
				{
					if (intPtr != IntPtr.Zero)
					{
						Marshal.ZeroFreeGlobalAllocUnicode(intPtr);
					}
				}
				return text;
			}

			internal unsafe static SecureString CreateSecureString(string plainString)
			{
				if (plainString == null || plainString.Length == 0)
				{
					return new SecureString();
				}
				SecureString secureString;
				fixed (string text = plainString)
				{
					char* ptr = text;
					if (ptr != null)
					{
						ptr += RuntimeHelpers.OffsetToStringData / 2;
					}
					secureString = new SecureString(ptr, plainString.Length);
				}
				return secureString;
			}
		}
	}
}
