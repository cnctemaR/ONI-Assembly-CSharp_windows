using System;

namespace Mono.Net
{
	internal class CFProxy
	{
		static CFProxy()
		{
			IntPtr intPtr = CFObject.dlopen("/System/Library/Frameworks/CoreServices.framework/Frameworks/CFNetwork.framework/CFNetwork", 0);
			CFProxy.kCFProxyAutoConfigurationJavaScriptKey = CFObject.GetCFObjectHandle(intPtr, "kCFProxyAutoConfigurationJavaScriptKey");
			CFProxy.kCFProxyAutoConfigurationURLKey = CFObject.GetCFObjectHandle(intPtr, "kCFProxyAutoConfigurationURLKey");
			CFProxy.kCFProxyHostNameKey = CFObject.GetCFObjectHandle(intPtr, "kCFProxyHostNameKey");
			CFProxy.kCFProxyPasswordKey = CFObject.GetCFObjectHandle(intPtr, "kCFProxyPasswordKey");
			CFProxy.kCFProxyPortNumberKey = CFObject.GetCFObjectHandle(intPtr, "kCFProxyPortNumberKey");
			CFProxy.kCFProxyTypeKey = CFObject.GetCFObjectHandle(intPtr, "kCFProxyTypeKey");
			CFProxy.kCFProxyUsernameKey = CFObject.GetCFObjectHandle(intPtr, "kCFProxyUsernameKey");
			CFProxy.kCFProxyTypeAutoConfigurationURL = CFObject.GetCFObjectHandle(intPtr, "kCFProxyTypeAutoConfigurationURL");
			CFProxy.kCFProxyTypeAutoConfigurationJavaScript = CFObject.GetCFObjectHandle(intPtr, "kCFProxyTypeAutoConfigurationJavaScript");
			CFProxy.kCFProxyTypeFTP = CFObject.GetCFObjectHandle(intPtr, "kCFProxyTypeFTP");
			CFProxy.kCFProxyTypeHTTP = CFObject.GetCFObjectHandle(intPtr, "kCFProxyTypeHTTP");
			CFProxy.kCFProxyTypeHTTPS = CFObject.GetCFObjectHandle(intPtr, "kCFProxyTypeHTTPS");
			CFProxy.kCFProxyTypeSOCKS = CFObject.GetCFObjectHandle(intPtr, "kCFProxyTypeSOCKS");
			CFObject.dlclose(intPtr);
		}

		internal CFProxy(CFDictionary settings)
		{
			this.settings = settings;
		}

		private static CFProxyType CFProxyTypeToEnum(IntPtr type)
		{
			if (type == CFProxy.kCFProxyTypeAutoConfigurationJavaScript)
			{
				return CFProxyType.AutoConfigurationJavaScript;
			}
			if (type == CFProxy.kCFProxyTypeAutoConfigurationURL)
			{
				return CFProxyType.AutoConfigurationUrl;
			}
			if (type == CFProxy.kCFProxyTypeFTP)
			{
				return CFProxyType.FTP;
			}
			if (type == CFProxy.kCFProxyTypeHTTP)
			{
				return CFProxyType.HTTP;
			}
			if (type == CFProxy.kCFProxyTypeHTTPS)
			{
				return CFProxyType.HTTPS;
			}
			if (type == CFProxy.kCFProxyTypeSOCKS)
			{
				return CFProxyType.SOCKS;
			}
			if (CFString.Compare(type, CFProxy.kCFProxyTypeAutoConfigurationJavaScript, 0) == 0)
			{
				return CFProxyType.AutoConfigurationJavaScript;
			}
			if (CFString.Compare(type, CFProxy.kCFProxyTypeAutoConfigurationURL, 0) == 0)
			{
				return CFProxyType.AutoConfigurationUrl;
			}
			if (CFString.Compare(type, CFProxy.kCFProxyTypeFTP, 0) == 0)
			{
				return CFProxyType.FTP;
			}
			if (CFString.Compare(type, CFProxy.kCFProxyTypeHTTP, 0) == 0)
			{
				return CFProxyType.HTTP;
			}
			if (CFString.Compare(type, CFProxy.kCFProxyTypeHTTPS, 0) == 0)
			{
				return CFProxyType.HTTPS;
			}
			if (CFString.Compare(type, CFProxy.kCFProxyTypeSOCKS, 0) == 0)
			{
				return CFProxyType.SOCKS;
			}
			return CFProxyType.None;
		}

		public IntPtr AutoConfigurationJavaScript
		{
			get
			{
				if (CFProxy.kCFProxyAutoConfigurationJavaScriptKey == IntPtr.Zero)
				{
					return IntPtr.Zero;
				}
				return this.settings[CFProxy.kCFProxyAutoConfigurationJavaScriptKey];
			}
		}

		public IntPtr AutoConfigurationUrl
		{
			get
			{
				if (CFProxy.kCFProxyAutoConfigurationURLKey == IntPtr.Zero)
				{
					return IntPtr.Zero;
				}
				return this.settings[CFProxy.kCFProxyAutoConfigurationURLKey];
			}
		}

		public string HostName
		{
			get
			{
				if (CFProxy.kCFProxyHostNameKey == IntPtr.Zero)
				{
					return null;
				}
				return CFString.AsString(this.settings[CFProxy.kCFProxyHostNameKey]);
			}
		}

		public string Password
		{
			get
			{
				if (CFProxy.kCFProxyPasswordKey == IntPtr.Zero)
				{
					return null;
				}
				return CFString.AsString(this.settings[CFProxy.kCFProxyPasswordKey]);
			}
		}

		public int Port
		{
			get
			{
				if (CFProxy.kCFProxyPortNumberKey == IntPtr.Zero)
				{
					return 0;
				}
				return CFNumber.AsInt32(this.settings[CFProxy.kCFProxyPortNumberKey]);
			}
		}

		public CFProxyType ProxyType
		{
			get
			{
				if (CFProxy.kCFProxyTypeKey == IntPtr.Zero)
				{
					return CFProxyType.None;
				}
				return CFProxy.CFProxyTypeToEnum(this.settings[CFProxy.kCFProxyTypeKey]);
			}
		}

		public string Username
		{
			get
			{
				if (CFProxy.kCFProxyUsernameKey == IntPtr.Zero)
				{
					return null;
				}
				return CFString.AsString(this.settings[CFProxy.kCFProxyUsernameKey]);
			}
		}

		private static IntPtr kCFProxyAutoConfigurationJavaScriptKey;

		private static IntPtr kCFProxyAutoConfigurationURLKey;

		private static IntPtr kCFProxyHostNameKey;

		private static IntPtr kCFProxyPasswordKey;

		private static IntPtr kCFProxyPortNumberKey;

		private static IntPtr kCFProxyTypeKey;

		private static IntPtr kCFProxyUsernameKey;

		private static IntPtr kCFProxyTypeAutoConfigurationURL;

		private static IntPtr kCFProxyTypeAutoConfigurationJavaScript;

		private static IntPtr kCFProxyTypeFTP;

		private static IntPtr kCFProxyTypeHTTP;

		private static IntPtr kCFProxyTypeHTTPS;

		private static IntPtr kCFProxyTypeSOCKS;

		private CFDictionary settings;
	}
}
