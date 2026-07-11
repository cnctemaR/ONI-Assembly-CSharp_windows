using System;

namespace Mono.Net
{
	internal class CFProxySettings
	{
		static CFProxySettings()
		{
			IntPtr intPtr = CFObject.dlopen("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork", 0);
			CFProxySettings.kCFNetworkProxiesHTTPEnable = CFObject.GetCFObjectHandle(intPtr, "kCFNetworkProxiesHTTPEnable");
			CFProxySettings.kCFNetworkProxiesHTTPPort = CFObject.GetCFObjectHandle(intPtr, "kCFNetworkProxiesHTTPPort");
			CFProxySettings.kCFNetworkProxiesHTTPProxy = CFObject.GetCFObjectHandle(intPtr, "kCFNetworkProxiesHTTPProxy");
			CFProxySettings.kCFNetworkProxiesProxyAutoConfigEnable = CFObject.GetCFObjectHandle(intPtr, "kCFNetworkProxiesProxyAutoConfigEnable");
			CFProxySettings.kCFNetworkProxiesProxyAutoConfigJavaScript = CFObject.GetCFObjectHandle(intPtr, "kCFNetworkProxiesProxyAutoConfigJavaScript");
			CFProxySettings.kCFNetworkProxiesProxyAutoConfigURLString = CFObject.GetCFObjectHandle(intPtr, "kCFNetworkProxiesProxyAutoConfigURLString");
			CFObject.dlclose(intPtr);
		}

		public CFProxySettings(CFDictionary settings)
		{
			this.settings = settings;
		}

		public CFDictionary Dictionary
		{
			get
			{
				return this.settings;
			}
		}

		public bool HTTPEnable
		{
			get
			{
				return !(CFProxySettings.kCFNetworkProxiesHTTPEnable == IntPtr.Zero) && CFNumber.AsBool(this.settings[CFProxySettings.kCFNetworkProxiesHTTPEnable]);
			}
		}

		public int HTTPPort
		{
			get
			{
				if (CFProxySettings.kCFNetworkProxiesHTTPPort == IntPtr.Zero)
				{
					return 0;
				}
				return CFNumber.AsInt32(this.settings[CFProxySettings.kCFNetworkProxiesHTTPPort]);
			}
		}

		public string HTTPProxy
		{
			get
			{
				if (CFProxySettings.kCFNetworkProxiesHTTPProxy == IntPtr.Zero)
				{
					return null;
				}
				return CFString.AsString(this.settings[CFProxySettings.kCFNetworkProxiesHTTPProxy]);
			}
		}

		public bool ProxyAutoConfigEnable
		{
			get
			{
				return !(CFProxySettings.kCFNetworkProxiesProxyAutoConfigEnable == IntPtr.Zero) && CFNumber.AsBool(this.settings[CFProxySettings.kCFNetworkProxiesProxyAutoConfigEnable]);
			}
		}

		public string ProxyAutoConfigJavaScript
		{
			get
			{
				if (CFProxySettings.kCFNetworkProxiesProxyAutoConfigJavaScript == IntPtr.Zero)
				{
					return null;
				}
				return CFString.AsString(this.settings[CFProxySettings.kCFNetworkProxiesProxyAutoConfigJavaScript]);
			}
		}

		public string ProxyAutoConfigURLString
		{
			get
			{
				if (CFProxySettings.kCFNetworkProxiesProxyAutoConfigURLString == IntPtr.Zero)
				{
					return null;
				}
				return CFString.AsString(this.settings[CFProxySettings.kCFNetworkProxiesProxyAutoConfigURLString]);
			}
		}

		private static IntPtr kCFNetworkProxiesHTTPEnable;

		private static IntPtr kCFNetworkProxiesHTTPPort;

		private static IntPtr kCFNetworkProxiesHTTPProxy;

		private static IntPtr kCFNetworkProxiesProxyAutoConfigEnable;

		private static IntPtr kCFNetworkProxiesProxyAutoConfigJavaScript;

		private static IntPtr kCFNetworkProxiesProxyAutoConfigURLString;

		private CFDictionary settings;
	}
}
