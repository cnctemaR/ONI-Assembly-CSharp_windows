using System;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net.Cache;
using System.Net.Configuration;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Security.Principal;

namespace System.Net
{
	[Serializable]
	public abstract class WebRequest : MarshalByRefObject, ISerializable
	{
		protected WebRequest()
		{
		}

		protected WebRequest(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
		}

		static WebRequest()
		{
			object section = ConfigurationManager.GetSection("system.net/webRequestModules");
			global::System.Net.Configuration.WebRequestModulesSection webRequestModulesSection = section as global::System.Net.Configuration.WebRequestModulesSection;
			if (webRequestModulesSection != null)
			{
				foreach (object obj in webRequestModulesSection.WebRequestModules)
				{
					global::System.Net.Configuration.WebRequestModuleElement webRequestModuleElement = (global::System.Net.Configuration.WebRequestModuleElement)obj;
					WebRequest.AddPrefix(webRequestModuleElement.Prefix, webRequestModuleElement.Type);
				}
				return;
			}
			global::System.Configuration.ConfigurationSettings.GetConfig("system.net/webRequestModules");
		}

		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			throw new NotSupportedException();
		}

		private static void AddDynamicPrefix(string protocol, string implementor)
		{
			Type type = typeof(WebRequest).Assembly.GetType("System.Net." + implementor);
			if (type == null)
			{
				return;
			}
			WebRequest.AddPrefix(protocol, type);
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException("This method must be implemented in derived classes");
		}

		public global::System.Net.Security.AuthenticationLevel AuthenticationLevel
		{
			get
			{
				return this.authentication_level;
			}
			set
			{
				this.authentication_level = value;
			}
		}

		public virtual global::System.Net.Cache.RequestCachePolicy CachePolicy
		{
			get
			{
				throw WebRequest.GetMustImplement();
			}
			set
			{
			}
		}

		public virtual string ConnectionGroupName
		{
			get
			{
				throw WebRequest.GetMustImplement();
			}
			set
			{
				throw WebRequest.GetMustImplement();
			}
		}

		public virtual long ContentLength
		{
			get
			{
				throw WebRequest.GetMustImplement();
			}
			set
			{
				throw WebRequest.GetMustImplement();
			}
		}

		public virtual string ContentType
		{
			get
			{
				throw WebRequest.GetMustImplement();
			}
			set
			{
				throw WebRequest.GetMustImplement();
			}
		}

		public virtual ICredentials Credentials
		{
			get
			{
				throw WebRequest.GetMustImplement();
			}
			set
			{
				throw WebRequest.GetMustImplement();
			}
		}

		public static global::System.Net.Cache.RequestCachePolicy DefaultCachePolicy
		{
			get
			{
				throw WebRequest.GetMustImplement();
			}
			set
			{
				throw WebRequest.GetMustImplement();
			}
		}

		public virtual WebHeaderCollection Headers
		{
			get
			{
				throw WebRequest.GetMustImplement();
			}
			set
			{
				throw WebRequest.GetMustImplement();
			}
		}

		public TokenImpersonationLevel ImpersonationLevel
		{
			get
			{
				throw WebRequest.GetMustImplement();
			}
			set
			{
				throw WebRequest.GetMustImplement();
			}
		}

		public virtual string Method
		{
			get
			{
				throw WebRequest.GetMustImplement();
			}
			set
			{
				throw WebRequest.GetMustImplement();
			}
		}

		public virtual bool PreAuthenticate
		{
			get
			{
				throw WebRequest.GetMustImplement();
			}
			set
			{
				throw WebRequest.GetMustImplement();
			}
		}

		public virtual IWebProxy Proxy
		{
			get
			{
				throw WebRequest.GetMustImplement();
			}
			set
			{
				throw WebRequest.GetMustImplement();
			}
		}

		public virtual global::System.Uri RequestUri
		{
			get
			{
				throw WebRequest.GetMustImplement();
			}
		}

		public virtual int Timeout
		{
			get
			{
				throw WebRequest.GetMustImplement();
			}
			set
			{
				throw WebRequest.GetMustImplement();
			}
		}

		public virtual bool UseDefaultCredentials
		{
			get
			{
				throw WebRequest.GetMustImplement();
			}
			set
			{
				throw WebRequest.GetMustImplement();
			}
		}

		public static IWebProxy DefaultWebProxy
		{
			get
			{
				if (!WebRequest.isDefaultWebProxySet)
				{
					object obj = WebRequest.lockobj;
					lock (obj)
					{
						if (WebRequest.defaultWebProxy == null)
						{
							WebRequest.defaultWebProxy = WebRequest.GetDefaultWebProxy();
						}
					}
				}
				return WebRequest.defaultWebProxy;
			}
			set
			{
				WebRequest.defaultWebProxy = value;
				WebRequest.isDefaultWebProxySet = true;
			}
		}

		[global::System.MonoTODO("Needs to respect Module, Proxy.AutoDetect, and Proxy.ScriptLocation config settings")]
		private static IWebProxy GetDefaultWebProxy()
		{
			global::System.Net.Configuration.DefaultProxySection defaultProxySection = ConfigurationManager.GetSection("system.net/defaultProxy") as global::System.Net.Configuration.DefaultProxySection;
			if (defaultProxySection == null)
			{
				return WebRequest.GetSystemWebProxy();
			}
			global::System.Net.Configuration.ProxyElement proxy = defaultProxySection.Proxy;
			WebProxy webProxy;
			if (proxy.UseSystemDefault != global::System.Net.Configuration.ProxyElement.UseSystemDefaultValues.False && proxy.ProxyAddress == null)
			{
				webProxy = (WebProxy)WebRequest.GetSystemWebProxy();
			}
			else
			{
				webProxy = new WebProxy();
			}
			if (proxy.ProxyAddress != null)
			{
				webProxy.Address = proxy.ProxyAddress;
			}
			if (proxy.BypassOnLocal != global::System.Net.Configuration.ProxyElement.BypassOnLocalValues.Unspecified)
			{
				webProxy.BypassProxyOnLocal = proxy.BypassOnLocal == global::System.Net.Configuration.ProxyElement.BypassOnLocalValues.True;
			}
			return webProxy;
		}

		public virtual void Abort()
		{
			throw WebRequest.GetMustImplement();
		}

		public virtual IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
		{
			throw WebRequest.GetMustImplement();
		}

		public virtual IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
		{
			throw WebRequest.GetMustImplement();
		}

		public static WebRequest Create(string requestUriString)
		{
			if (requestUriString == null)
			{
				throw new ArgumentNullException("requestUriString");
			}
			return WebRequest.Create(new global::System.Uri(requestUriString));
		}

		public static WebRequest Create(global::System.Uri requestUri)
		{
			if (requestUri == null)
			{
				throw new ArgumentNullException("requestUri");
			}
			return WebRequest.GetCreator(requestUri.AbsoluteUri).Create(requestUri);
		}

		public static WebRequest CreateDefault(global::System.Uri requestUri)
		{
			if (requestUri == null)
			{
				throw new ArgumentNullException("requestUri");
			}
			return WebRequest.GetCreator(requestUri.Scheme).Create(requestUri);
		}

		public virtual Stream EndGetRequestStream(IAsyncResult asyncResult)
		{
			throw WebRequest.GetMustImplement();
		}

		public virtual WebResponse EndGetResponse(IAsyncResult asyncResult)
		{
			throw WebRequest.GetMustImplement();
		}

		public virtual Stream GetRequestStream()
		{
			throw WebRequest.GetMustImplement();
		}

		public virtual WebResponse GetResponse()
		{
			throw WebRequest.GetMustImplement();
		}

		[global::System.MonoTODO("Look in other places for proxy config info")]
		public static IWebProxy GetSystemWebProxy()
		{
			string text = Environment.GetEnvironmentVariable("http_proxy");
			if (text == null)
			{
				text = Environment.GetEnvironmentVariable("HTTP_PROXY");
			}
			if (text != null)
			{
				try
				{
					if (!text.StartsWith("http://"))
					{
						text = "http://" + text;
					}
					global::System.Uri uri = new global::System.Uri(text);
					IPAddress ipaddress;
					if (IPAddress.TryParse(uri.Host, out ipaddress))
					{
						if (IPAddress.Any.Equals(ipaddress))
						{
							uri = new global::System.UriBuilder(uri)
							{
								Host = "127.0.0.1"
							}.Uri;
						}
						else if (IPAddress.IPv6Any.Equals(ipaddress))
						{
							uri = new global::System.UriBuilder(uri)
							{
								Host = "[::1]"
							}.Uri;
						}
					}
					return new WebProxy(uri);
				}
				catch (global::System.UriFormatException)
				{
				}
			}
			return new WebProxy();
		}

		protected virtual void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			throw WebRequest.GetMustImplement();
		}

		public static bool RegisterPrefix(string prefix, IWebRequestCreate creator)
		{
			if (prefix == null)
			{
				throw new ArgumentNullException("prefix");
			}
			if (creator == null)
			{
				throw new ArgumentNullException("creator");
			}
			object syncRoot = WebRequest.prefixes.SyncRoot;
			lock (syncRoot)
			{
				string text = prefix.ToLower(CultureInfo.InvariantCulture);
				if (WebRequest.prefixes.Contains(text))
				{
					return false;
				}
				WebRequest.prefixes.Add(text, creator);
			}
			return true;
		}

		private static IWebRequestCreate GetCreator(string prefix)
		{
			int num = -1;
			IWebRequestCreate webRequestCreate = null;
			prefix = prefix.ToLower(CultureInfo.InvariantCulture);
			IDictionaryEnumerator enumerator = WebRequest.prefixes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string text = enumerator.Key as string;
				if (text.Length > num)
				{
					if (prefix.StartsWith(text))
					{
						num = text.Length;
						webRequestCreate = (IWebRequestCreate)enumerator.Value;
					}
				}
			}
			if (webRequestCreate == null)
			{
				throw new NotSupportedException(prefix);
			}
			return webRequestCreate;
		}

		internal static void ClearPrefixes()
		{
			WebRequest.prefixes.Clear();
		}

		internal static void RemovePrefix(string prefix)
		{
			WebRequest.prefixes.Remove(prefix);
		}

		internal static void AddPrefix(string prefix, string typeName)
		{
			Type type = Type.GetType(typeName);
			if (type == null)
			{
				throw new global::System.Configuration.ConfigurationException(string.Format("Type {0} not found", typeName));
			}
			WebRequest.AddPrefix(prefix, type);
		}

		internal static void AddPrefix(string prefix, Type type)
		{
			object obj = Activator.CreateInstance(type, true);
			WebRequest.prefixes[prefix] = obj;
		}

		private static global::System.Collections.Specialized.HybridDictionary prefixes = new global::System.Collections.Specialized.HybridDictionary();

		private static bool isDefaultWebProxySet;

		private static IWebProxy defaultWebProxy;

		private global::System.Net.Security.AuthenticationLevel authentication_level = global::System.Net.Security.AuthenticationLevel.MutualAuthRequested;

		private static readonly object lockobj = new object();
	}
}
