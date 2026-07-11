using System;

namespace Steamworks
{
	public static class SteamHTMLSurface
	{
		public static bool Init()
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamHTMLSurface_Init(CSteamAPIContext.GetSteamHTMLSurface());
		}

		public static bool Shutdown()
		{
			InteropHelp.TestIfAvailableClient();
			return NativeMethods.ISteamHTMLSurface_Shutdown(CSteamAPIContext.GetSteamHTMLSurface());
		}

		public static SteamAPICall_t CreateBrowser(string pchUserAgent, string pchUserCSS)
		{
			InteropHelp.TestIfAvailableClient();
			SteamAPICall_t steamAPICall_t;
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchUserAgent))
			{
				using (InteropHelp.UTF8StringHandle utf8StringHandle2 = new InteropHelp.UTF8StringHandle(pchUserCSS))
				{
					steamAPICall_t = (SteamAPICall_t)NativeMethods.ISteamHTMLSurface_CreateBrowser(CSteamAPIContext.GetSteamHTMLSurface(), utf8StringHandle, utf8StringHandle2);
				}
			}
			return steamAPICall_t;
		}

		public static void RemoveBrowser(HHTMLBrowser unBrowserHandle)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_RemoveBrowser(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle);
		}

		public static void LoadURL(HHTMLBrowser unBrowserHandle, string pchURL, string pchPostData)
		{
			InteropHelp.TestIfAvailableClient();
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchURL))
			{
				using (InteropHelp.UTF8StringHandle utf8StringHandle2 = new InteropHelp.UTF8StringHandle(pchPostData))
				{
					NativeMethods.ISteamHTMLSurface_LoadURL(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, utf8StringHandle, utf8StringHandle2);
				}
			}
		}

		public static void SetSize(HHTMLBrowser unBrowserHandle, uint unWidth, uint unHeight)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_SetSize(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, unWidth, unHeight);
		}

		public static void StopLoad(HHTMLBrowser unBrowserHandle)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_StopLoad(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle);
		}

		public static void Reload(HHTMLBrowser unBrowserHandle)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_Reload(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle);
		}

		public static void GoBack(HHTMLBrowser unBrowserHandle)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_GoBack(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle);
		}

		public static void GoForward(HHTMLBrowser unBrowserHandle)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_GoForward(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle);
		}

		public static void AddHeader(HHTMLBrowser unBrowserHandle, string pchKey, string pchValue)
		{
			InteropHelp.TestIfAvailableClient();
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchKey))
			{
				using (InteropHelp.UTF8StringHandle utf8StringHandle2 = new InteropHelp.UTF8StringHandle(pchValue))
				{
					NativeMethods.ISteamHTMLSurface_AddHeader(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, utf8StringHandle, utf8StringHandle2);
				}
			}
		}

		public static void ExecuteJavascript(HHTMLBrowser unBrowserHandle, string pchScript)
		{
			InteropHelp.TestIfAvailableClient();
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchScript))
			{
				NativeMethods.ISteamHTMLSurface_ExecuteJavascript(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, utf8StringHandle);
			}
		}

		public static void MouseUp(HHTMLBrowser unBrowserHandle, EHTMLMouseButton eMouseButton)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_MouseUp(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, eMouseButton);
		}

		public static void MouseDown(HHTMLBrowser unBrowserHandle, EHTMLMouseButton eMouseButton)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_MouseDown(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, eMouseButton);
		}

		public static void MouseDoubleClick(HHTMLBrowser unBrowserHandle, EHTMLMouseButton eMouseButton)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_MouseDoubleClick(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, eMouseButton);
		}

		public static void MouseMove(HHTMLBrowser unBrowserHandle, int x, int y)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_MouseMove(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, x, y);
		}

		public static void MouseWheel(HHTMLBrowser unBrowserHandle, int nDelta)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_MouseWheel(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, nDelta);
		}

		public static void KeyDown(HHTMLBrowser unBrowserHandle, uint nNativeKeyCode, EHTMLKeyModifiers eHTMLKeyModifiers, bool bIsSystemKey = false)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_KeyDown(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, nNativeKeyCode, eHTMLKeyModifiers, bIsSystemKey);
		}

		public static void KeyUp(HHTMLBrowser unBrowserHandle, uint nNativeKeyCode, EHTMLKeyModifiers eHTMLKeyModifiers)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_KeyUp(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, nNativeKeyCode, eHTMLKeyModifiers);
		}

		public static void KeyChar(HHTMLBrowser unBrowserHandle, uint cUnicodeChar, EHTMLKeyModifiers eHTMLKeyModifiers)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_KeyChar(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, cUnicodeChar, eHTMLKeyModifiers);
		}

		public static void SetHorizontalScroll(HHTMLBrowser unBrowserHandle, uint nAbsolutePixelScroll)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_SetHorizontalScroll(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, nAbsolutePixelScroll);
		}

		public static void SetVerticalScroll(HHTMLBrowser unBrowserHandle, uint nAbsolutePixelScroll)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_SetVerticalScroll(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, nAbsolutePixelScroll);
		}

		public static void SetKeyFocus(HHTMLBrowser unBrowserHandle, bool bHasKeyFocus)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_SetKeyFocus(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, bHasKeyFocus);
		}

		public static void ViewSource(HHTMLBrowser unBrowserHandle)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_ViewSource(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle);
		}

		public static void CopyToClipboard(HHTMLBrowser unBrowserHandle)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_CopyToClipboard(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle);
		}

		public static void PasteFromClipboard(HHTMLBrowser unBrowserHandle)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_PasteFromClipboard(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle);
		}

		public static void Find(HHTMLBrowser unBrowserHandle, string pchSearchStr, bool bCurrentlyInFind, bool bReverse)
		{
			InteropHelp.TestIfAvailableClient();
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchSearchStr))
			{
				NativeMethods.ISteamHTMLSurface_Find(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, utf8StringHandle, bCurrentlyInFind, bReverse);
			}
		}

		public static void StopFind(HHTMLBrowser unBrowserHandle)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_StopFind(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle);
		}

		public static void GetLinkAtPosition(HHTMLBrowser unBrowserHandle, int x, int y)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_GetLinkAtPosition(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, x, y);
		}

		public static void SetCookie(string pchHostname, string pchKey, string pchValue, string pchPath = "/", uint nExpires = 0U, bool bSecure = false, bool bHTTPOnly = false)
		{
			InteropHelp.TestIfAvailableClient();
			using (InteropHelp.UTF8StringHandle utf8StringHandle = new InteropHelp.UTF8StringHandle(pchHostname))
			{
				using (InteropHelp.UTF8StringHandle utf8StringHandle2 = new InteropHelp.UTF8StringHandle(pchKey))
				{
					using (InteropHelp.UTF8StringHandle utf8StringHandle3 = new InteropHelp.UTF8StringHandle(pchValue))
					{
						using (InteropHelp.UTF8StringHandle utf8StringHandle4 = new InteropHelp.UTF8StringHandle(pchPath))
						{
							NativeMethods.ISteamHTMLSurface_SetCookie(CSteamAPIContext.GetSteamHTMLSurface(), utf8StringHandle, utf8StringHandle2, utf8StringHandle3, utf8StringHandle4, nExpires, bSecure, bHTTPOnly);
						}
					}
				}
			}
		}

		public static void SetPageScaleFactor(HHTMLBrowser unBrowserHandle, float flZoom, int nPointX, int nPointY)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_SetPageScaleFactor(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, flZoom, nPointX, nPointY);
		}

		public static void SetBackgroundMode(HHTMLBrowser unBrowserHandle, bool bBackgroundMode)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_SetBackgroundMode(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, bBackgroundMode);
		}

		public static void SetDPIScalingFactor(HHTMLBrowser unBrowserHandle, float flDPIScaling)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_SetDPIScalingFactor(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, flDPIScaling);
		}

		public static void OpenDeveloperTools(HHTMLBrowser unBrowserHandle)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_OpenDeveloperTools(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle);
		}

		public static void AllowStartRequest(HHTMLBrowser unBrowserHandle, bool bAllowed)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_AllowStartRequest(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, bAllowed);
		}

		public static void JSDialogResponse(HHTMLBrowser unBrowserHandle, bool bResult)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_JSDialogResponse(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, bResult);
		}

		public static void FileLoadDialogResponse(HHTMLBrowser unBrowserHandle, IntPtr pchSelectedFiles)
		{
			InteropHelp.TestIfAvailableClient();
			NativeMethods.ISteamHTMLSurface_FileLoadDialogResponse(CSteamAPIContext.GetSteamHTMLSurface(), unBrowserHandle, pchSelectedFiles);
		}
	}
}
