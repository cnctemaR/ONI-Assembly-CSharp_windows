using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace System.Net
{
	internal class AutoWebProxyScriptEngine
	{
		public AutoWebProxyScriptEngine(WebProxy proxy, bool useRegistry)
		{
		}

		public Uri AutomaticConfigurationScript { get; set; }

		public bool AutomaticallyDetectSettings { get; set; }

		public bool GetProxies(Uri destination, out IList<string> proxyList)
		{
			int num = 0;
			return this.GetProxies(destination, out proxyList, ref num);
		}

		public bool GetProxies(Uri destination, out IList<string> proxyList, ref int syncStatus)
		{
			proxyList = null;
			return false;
		}

		public void Close()
		{
		}

		public void Abort(ref int syncStatus)
		{
		}

		public void CheckForChanges()
		{
		}

		public WebProxyData GetWebProxyData()
		{
			try
			{
				WebProxyData webProxyData;
				if (AutoWebProxyScriptEngine.IsWindows())
				{
					webProxyData = this.InitializeRegistryGlobalProxy();
					if (webProxyData != null)
					{
						return webProxyData;
					}
				}
				webProxyData = this.ReadEnvVariables();
				if (webProxyData != null)
				{
					return webProxyData;
				}
			}
			catch (DllNotFoundException)
			{
			}
			return new WebProxyData();
		}

		private WebProxyData ReadEnvVariables()
		{
			string text = Environment.GetEnvironmentVariable("http_proxy") ?? Environment.GetEnvironmentVariable("HTTP_PROXY");
			if (text != null)
			{
				try
				{
					if (!text.StartsWith("http://"))
					{
						text = "http://" + text;
					}
					Uri uri = new Uri(text);
					IPAddress ipaddress;
					if (IPAddress.TryParse(uri.Host, out ipaddress))
					{
						if (IPAddress.Any.Equals(ipaddress))
						{
							uri = new UriBuilder(uri)
							{
								Host = "127.0.0.1"
							}.Uri;
						}
						else if (IPAddress.IPv6Any.Equals(ipaddress))
						{
							uri = new UriBuilder(uri)
							{
								Host = "[::1]"
							}.Uri;
						}
					}
					bool flag = false;
					ArrayList arrayList = new ArrayList();
					string text2 = Environment.GetEnvironmentVariable("no_proxy") ?? Environment.GetEnvironmentVariable("NO_PROXY");
					if (text2 != null)
					{
						foreach (string text3 in text2.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
						{
							if (text3 != "*.local")
							{
								arrayList.Add(text3);
							}
							else
							{
								flag = true;
							}
						}
					}
					return new WebProxyData
					{
						proxyAddress = uri,
						bypassOnLocal = flag,
						bypassList = AutoWebProxyScriptEngine.CreateBypassList(arrayList)
					};
				}
				catch (UriFormatException)
				{
				}
			}
			return null;
		}

		private static bool IsWindows()
		{
			return Environment.OSVersion.Platform < PlatformID.Unix;
		}

		private WebProxyData InitializeRegistryGlobalProxy()
		{
			if ((int)Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", "ProxyEnable", 0) <= 0)
			{
				return null;
			}
			string text = "";
			bool flag = false;
			ArrayList arrayList = new ArrayList();
			string text2 = (string)Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", "ProxyServer", null);
			string text3 = (string)Registry.GetValue("HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", "ProxyOverride", null);
			if (text2 == null)
			{
				return null;
			}
			if (text2.Contains("="))
			{
				foreach (string text4 in text2.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
				{
					if (text4.StartsWith("http="))
					{
						text = text4.Substring(5);
						break;
					}
				}
			}
			else
			{
				text = text2;
			}
			if (text3 != null)
			{
				foreach (string text5 in text3.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
				{
					if (text5 != "<local>")
					{
						arrayList.Add(text5);
					}
					else
					{
						flag = true;
					}
				}
			}
			return new WebProxyData
			{
				proxyAddress = AutoWebProxyScriptEngine.ToUri(text),
				bypassOnLocal = flag,
				bypassList = AutoWebProxyScriptEngine.CreateBypassList(arrayList)
			};
		}

		private static Uri ToUri(string address)
		{
			if (address == null)
			{
				return null;
			}
			if (address.IndexOf("://", StringComparison.Ordinal) == -1)
			{
				address = "http://" + address;
			}
			return new Uri(address);
		}

		private static ArrayList CreateBypassList(ArrayList al)
		{
			string[] array = al.ToArray(typeof(string)) as string[];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = "^" + Regex.Escape(array[i]).Replace("\\*", ".*").Replace("\\?", ".") + "$";
			}
			return new ArrayList(array);
		}
	}
}
