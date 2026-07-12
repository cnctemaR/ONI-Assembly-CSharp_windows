using System;

namespace Steamworks
{
	public static class SteamGameServerHTTP
	{
		public static HTTPRequestHandle CreateHTTPRequest(EHTTPMethod eHTTPRequestMethod, string pchAbsoluteURL)
		{
			InteropHelp.TestIfAvailableGameServer();
			HTTPRequestHandle httprequestHandle;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchAbsoluteURL))
			{
				httprequestHandle = (HTTPRequestHandle)NativeMethods.ISteamHTTP_CreateHTTPRequest(CSteamGameServerAPIContext.GetSteamHTTP(), eHTTPRequestMethod, utf8StringHandle);
			}
			return httprequestHandle;
		}

		public static bool SetHTTPRequestContextValue(HTTPRequestHandle hRequest, ulong ulContextValue)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamHTTP_SetHTTPRequestContextValue(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest, ulContextValue);
		}

		public static bool SetHTTPRequestNetworkActivityTimeout(HTTPRequestHandle hRequest, uint unTimeoutSeconds)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamHTTP_SetHTTPRequestNetworkActivityTimeout(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest, unTimeoutSeconds);
		}

		public static bool SetHTTPRequestHeaderValue(HTTPRequestHandle hRequest, string pchHeaderName, string pchHeaderValue)
		{
			InteropHelp.TestIfAvailableGameServer();
			bool flag;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchHeaderName))
			{
				using (InteropHelp.UTF8StringHandle utf8StringHandle2 = new InteropHelp.UTF8StringHandle(pchHeaderValue))
				{
					flag = NativeMethods.ISteamHTTP_SetHTTPRequestHeaderValue(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest, utf8StringHandle, utf8StringHandle2);
				}
			}
			return flag;
		}

		public static bool SetHTTPRequestGetOrPostParameter(HTTPRequestHandle hRequest, string pchParamName, string pchParamValue)
		{
			InteropHelp.TestIfAvailableGameServer();
			bool flag;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchParamName))
			{
				using (InteropHelp.UTF8StringHandle utf8StringHandle2 = new InteropHelp.UTF8StringHandle(pchParamValue))
				{
					flag = NativeMethods.ISteamHTTP_SetHTTPRequestGetOrPostParameter(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest, utf8StringHandle, utf8StringHandle2);
				}
			}
			return flag;
		}

		public static bool SendHTTPRequest(HTTPRequestHandle hRequest, out SteamAPICall_t pCallHandle)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamHTTP_SendHTTPRequest(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest, out pCallHandle);
		}

		public static bool SendHTTPRequestAndStreamResponse(HTTPRequestHandle hRequest, out SteamAPICall_t pCallHandle)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamHTTP_SendHTTPRequestAndStreamResponse(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest, out pCallHandle);
		}

		public static bool DeferHTTPRequest(HTTPRequestHandle hRequest)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamHTTP_DeferHTTPRequest(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest);
		}

		public static bool PrioritizeHTTPRequest(HTTPRequestHandle hRequest)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamHTTP_PrioritizeHTTPRequest(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest);
		}

		public static bool GetHTTPResponseHeaderSize(HTTPRequestHandle hRequest, string pchHeaderName, out uint unResponseHeaderSize)
		{
			InteropHelp.TestIfAvailableGameServer();
			bool flag;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchHeaderName))
			{
				flag = NativeMethods.ISteamHTTP_GetHTTPResponseHeaderSize(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest, utf8StringHandle, out unResponseHeaderSize);
			}
			return flag;
		}

		public static bool GetHTTPResponseHeaderValue(HTTPRequestHandle hRequest, string pchHeaderName, byte[] pHeaderValueBuffer, uint unBufferSize)
		{
			InteropHelp.TestIfAvailableGameServer();
			bool flag;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchHeaderName))
			{
				flag = NativeMethods.ISteamHTTP_GetHTTPResponseHeaderValue(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest, utf8StringHandle, pHeaderValueBuffer, unBufferSize);
			}
			return flag;
		}

		public static bool GetHTTPResponseBodySize(HTTPRequestHandle hRequest, out uint unBodySize)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamHTTP_GetHTTPResponseBodySize(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest, out unBodySize);
		}

		public static bool GetHTTPResponseBodyData(HTTPRequestHandle hRequest, byte[] pBodyDataBuffer, uint unBufferSize)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamHTTP_GetHTTPResponseBodyData(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest, pBodyDataBuffer, unBufferSize);
		}

		public static bool GetHTTPStreamingResponseBodyData(HTTPRequestHandle hRequest, uint cOffset, byte[] pBodyDataBuffer, uint unBufferSize)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamHTTP_GetHTTPStreamingResponseBodyData(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest, cOffset, pBodyDataBuffer, unBufferSize);
		}

		public static bool ReleaseHTTPRequest(HTTPRequestHandle hRequest)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamHTTP_ReleaseHTTPRequest(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest);
		}

		public static bool GetHTTPDownloadProgressPct(HTTPRequestHandle hRequest, out float pflPercentOut)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamHTTP_GetHTTPDownloadProgressPct(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest, out pflPercentOut);
		}

		public static bool SetHTTPRequestRawPostBody(HTTPRequestHandle hRequest, string pchContentType, byte[] pubBody, uint unBodyLen)
		{
			InteropHelp.TestIfAvailableGameServer();
			bool flag;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchContentType))
			{
				flag = NativeMethods.ISteamHTTP_SetHTTPRequestRawPostBody(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest, utf8StringHandle, pubBody, unBodyLen);
			}
			return flag;
		}

		public static HTTPCookieContainerHandle CreateCookieContainer(bool bAllowResponsesToModify)
		{
			InteropHelp.TestIfAvailableGameServer();
			return (HTTPCookieContainerHandle)NativeMethods.ISteamHTTP_CreateCookieContainer(CSteamGameServerAPIContext.GetSteamHTTP(), bAllowResponsesToModify);
		}

		public static bool ReleaseCookieContainer(HTTPCookieContainerHandle hCookieContainer)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamHTTP_ReleaseCookieContainer(CSteamGameServerAPIContext.GetSteamHTTP(), hCookieContainer);
		}

		public static bool SetCookie(HTTPCookieContainerHandle hCookieContainer, string pchHost, string pchUrl, string pchCookie)
		{
			InteropHelp.TestIfAvailableGameServer();
			bool flag;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchHost))
			{
				using (InteropHelp.UTF8StringHandle utf8StringHandle2 = new InteropHelp.UTF8StringHandle(pchUrl))
				{
					using (InteropHelp.UTF8StringHandle utf8StringHandle3 = new InteropHelp.UTF8StringHandle(pchCookie))
					{
						flag = NativeMethods.ISteamHTTP_SetCookie(CSteamGameServerAPIContext.GetSteamHTTP(), hCookieContainer, utf8StringHandle, utf8StringHandle2, utf8StringHandle3);
					}
				}
			}
			return flag;
		}

		public static bool SetHTTPRequestCookieContainer(HTTPRequestHandle hRequest, HTTPCookieContainerHandle hCookieContainer)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamHTTP_SetHTTPRequestCookieContainer(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest, hCookieContainer);
		}

		public static bool SetHTTPRequestUserAgentInfo(HTTPRequestHandle hRequest, string pchUserAgentInfo)
		{
			InteropHelp.TestIfAvailableGameServer();
			bool flag;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchUserAgentInfo))
			{
				flag = NativeMethods.ISteamHTTP_SetHTTPRequestUserAgentInfo(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest, utf8StringHandle);
			}
			return flag;
		}

		public static bool SetHTTPRequestRequiresVerifiedCertificate(HTTPRequestHandle hRequest, bool bRequireVerifiedCertificate)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamHTTP_SetHTTPRequestRequiresVerifiedCertificate(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest, bRequireVerifiedCertificate);
		}

		public static bool SetHTTPRequestAbsoluteTimeoutMS(HTTPRequestHandle hRequest, uint unMilliseconds)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamHTTP_SetHTTPRequestAbsoluteTimeoutMS(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest, unMilliseconds);
		}

		public static bool GetHTTPRequestWasTimedOut(HTTPRequestHandle hRequest, out bool pbWasTimedOut)
		{
			InteropHelp.TestIfAvailableGameServer();
			return NativeMethods.ISteamHTTP_GetHTTPRequestWasTimedOut(CSteamGameServerAPIContext.GetSteamHTTP(), hRequest, out pbWasTimedOut);
		}
	}
}
