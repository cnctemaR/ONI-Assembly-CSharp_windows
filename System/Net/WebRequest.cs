using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Net.Cache;
using System.Net.Configuration;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	[Serializable]
	public abstract class WebRequest : MarshalByRefObject, ISerializable
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
		public virtual IWebRequestCreate CreatorInstance
		{
			get
			{
				return WebRequest.webRequestCreate;
			}
		}

		[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void RegisterPortableWebRequestCreator(IWebRequestCreate creator)
		{
		}

		private static object InternalSyncObject
		{
			get
			{
				if (WebRequest.s_InternalSyncObject == null)
				{
					object obj = new object();
					Interlocked.CompareExchange(ref WebRequest.s_InternalSyncObject, obj, null);
				}
				return WebRequest.s_InternalSyncObject;
			}
		}

		internal static TimerThread.Queue DefaultTimerQueue
		{
			get
			{
				return WebRequest.s_DefaultTimerQueue;
			}
		}

		private static WebRequest Create(Uri requestUri, bool useUriBase)
		{
			bool on = Logging.On;
			WebRequestPrefixElement webRequestPrefixElement = null;
			bool flag = false;
			string text;
			if (!useUriBase)
			{
				text = requestUri.AbsoluteUri;
			}
			else
			{
				text = requestUri.Scheme + ":";
			}
			int length = text.Length;
			ArrayList prefixList = WebRequest.PrefixList;
			for (int i = 0; i < prefixList.Count; i++)
			{
				webRequestPrefixElement = (WebRequestPrefixElement)prefixList[i];
				if (length >= webRequestPrefixElement.Prefix.Length && string.Compare(webRequestPrefixElement.Prefix, 0, text, 0, webRequestPrefixElement.Prefix.Length, StringComparison.OrdinalIgnoreCase) == 0)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				WebRequest webRequest = webRequestPrefixElement.Creator.Create(requestUri);
				bool on2 = Logging.On;
				return webRequest;
			}
			bool on3 = Logging.On;
			throw new NotSupportedException(SR.GetString("The URI prefix is not recognized."));
		}

		public static WebRequest Create(string requestUriString)
		{
			if (requestUriString == null)
			{
				throw new ArgumentNullException("requestUriString");
			}
			return WebRequest.Create(new Uri(requestUriString), false);
		}

		public static WebRequest Create(Uri requestUri)
		{
			if (requestUri == null)
			{
				throw new ArgumentNullException("requestUri");
			}
			return WebRequest.Create(requestUri, false);
		}

		public static WebRequest CreateDefault(Uri requestUri)
		{
			if (requestUri == null)
			{
				throw new ArgumentNullException("requestUri");
			}
			return WebRequest.Create(requestUri, true);
		}

		public static HttpWebRequest CreateHttp(string requestUriString)
		{
			if (requestUriString == null)
			{
				throw new ArgumentNullException("requestUriString");
			}
			return WebRequest.CreateHttp(new Uri(requestUriString));
		}

		public static HttpWebRequest CreateHttp(Uri requestUri)
		{
			if (requestUri == null)
			{
				throw new ArgumentNullException("requestUri");
			}
			if (requestUri.Scheme != Uri.UriSchemeHttp && requestUri.Scheme != Uri.UriSchemeHttps)
			{
				throw new NotSupportedException(SR.GetString("The URI prefix is not recognized."));
			}
			return (HttpWebRequest)WebRequest.CreateDefault(requestUri);
		}

		public static bool RegisterPrefix(string prefix, IWebRequestCreate creator)
		{
			bool flag = false;
			if (prefix == null)
			{
				throw new ArgumentNullException("prefix");
			}
			if (creator == null)
			{
				throw new ArgumentNullException("creator");
			}
			object internalSyncObject = WebRequest.InternalSyncObject;
			lock (internalSyncObject)
			{
				ArrayList arrayList = (ArrayList)WebRequest.PrefixList.Clone();
				Uri uri;
				if (Uri.TryCreate(prefix, UriKind.Absolute, out uri))
				{
					string text = uri.AbsoluteUri;
					if (!prefix.EndsWith("/", StringComparison.Ordinal) && uri.GetComponents(UriComponents.Path | UriComponents.Query | UriComponents.Fragment, UriFormat.UriEscaped).Equals("/"))
					{
						text = text.Substring(0, text.Length - 1);
					}
					prefix = text;
				}
				int i;
				for (i = 0; i < arrayList.Count; i++)
				{
					WebRequestPrefixElement webRequestPrefixElement = (WebRequestPrefixElement)arrayList[i];
					if (prefix.Length > webRequestPrefixElement.Prefix.Length)
					{
						break;
					}
					if (prefix.Length == webRequestPrefixElement.Prefix.Length && string.Compare(webRequestPrefixElement.Prefix, prefix, StringComparison.OrdinalIgnoreCase) == 0)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					arrayList.Insert(i, new WebRequestPrefixElement(prefix, creator));
					WebRequest.PrefixList = arrayList;
				}
			}
			return !flag;
		}

		internal static ArrayList PrefixList
		{
			get
			{
				if (WebRequest.s_PrefixList == null)
				{
					object internalSyncObject = WebRequest.InternalSyncObject;
					lock (internalSyncObject)
					{
						if (WebRequest.s_PrefixList == null)
						{
							WebRequest.s_PrefixList = WebRequest.PopulatePrefixList();
						}
					}
				}
				return WebRequest.s_PrefixList;
			}
			set
			{
				WebRequest.s_PrefixList = value;
			}
		}

		private static ArrayList PopulatePrefixList()
		{
			ArrayList arrayList = new ArrayList();
			if (Console.IsRunningOnAndroid)
			{
				IWebRequestCreate webRequestCreate = new HttpRequestCreator();
				arrayList.Add(new WebRequestPrefixElement("http", webRequestCreate));
				arrayList.Add(new WebRequestPrefixElement("https", webRequestCreate));
				arrayList.Add(new WebRequestPrefixElement("file", new FileWebRequestCreator()));
				arrayList.Add(new WebRequestPrefixElement("ftp", new FtpWebRequestCreator()));
			}
			else
			{
				WebRequestModulesSection webRequestModulesSection = ConfigurationManager.GetSection("system.net/webRequestModules") as WebRequestModulesSection;
				if (webRequestModulesSection != null)
				{
					foreach (object obj in webRequestModulesSection.WebRequestModules)
					{
						WebRequestModuleElement webRequestModuleElement = (WebRequestModuleElement)obj;
						arrayList.Add(new WebRequestPrefixElement(webRequestModuleElement.Prefix, webRequestModuleElement.Type));
					}
				}
			}
			return arrayList;
		}

		protected WebRequest()
		{
			this.m_ImpersonationLevel = TokenImpersonationLevel.Delegation;
			this.m_AuthenticationLevel = AuthenticationLevel.MutualAuthRequested;
		}

		protected WebRequest(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter, SerializationFormatter = true)]
		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			this.GetObjectData(serializationInfo, streamingContext);
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		protected virtual void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
		}

		public static RequestCachePolicy DefaultCachePolicy
		{
			get
			{
				return RequestCacheManager.GetBinding(string.Empty).Policy;
			}
			set
			{
				RequestCacheBinding binding = RequestCacheManager.GetBinding(string.Empty);
				RequestCacheManager.SetBinding(string.Empty, new RequestCacheBinding(binding.Cache, binding.Validator, value));
			}
		}

		public virtual RequestCachePolicy CachePolicy
		{
			get
			{
				return this.m_CachePolicy;
			}
			set
			{
				this.InternalSetCachePolicy(value);
			}
		}

		private void InternalSetCachePolicy(RequestCachePolicy policy)
		{
			if (this.m_CacheBinding != null && this.m_CacheBinding.Cache != null && this.m_CacheBinding.Validator != null && this.CacheProtocol == null && policy != null && policy.Level != RequestCacheLevel.BypassCache)
			{
				this.CacheProtocol = new RequestCacheProtocol(this.m_CacheBinding.Cache, this.m_CacheBinding.Validator.CreateValidator());
			}
			this.m_CachePolicy = policy;
		}

		public virtual string Method
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		public virtual Uri RequestUri
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		public virtual string ConnectionGroupName
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		public virtual WebHeaderCollection Headers
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		public virtual long ContentLength
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		public virtual string ContentType
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		public virtual ICredentials Credentials
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		public virtual bool UseDefaultCredentials
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		public virtual IWebProxy Proxy
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		public virtual bool PreAuthenticate
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		public virtual int Timeout
		{
			get
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
			set
			{
				throw ExceptionHelper.PropertyNotImplementedException;
			}
		}

		public virtual Stream GetRequestStream()
		{
			throw ExceptionHelper.MethodNotImplementedException;
		}

		public virtual WebResponse GetResponse()
		{
			throw ExceptionHelper.MethodNotImplementedException;
		}

		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public virtual IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
		{
			throw ExceptionHelper.MethodNotImplementedException;
		}

		public virtual WebResponse EndGetResponse(IAsyncResult asyncResult)
		{
			throw ExceptionHelper.MethodNotImplementedException;
		}

		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public virtual IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
		{
			throw ExceptionHelper.MethodNotImplementedException;
		}

		public virtual Stream EndGetRequestStream(IAsyncResult asyncResult)
		{
			throw ExceptionHelper.MethodNotImplementedException;
		}

		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public virtual Task<Stream> GetRequestStreamAsync()
		{
			IWebProxy webProxy = null;
			try
			{
				webProxy = this.Proxy;
			}
			catch (NotImplementedException)
			{
			}
			if (ExecutionContext.IsFlowSuppressed() && (this.UseDefaultCredentials || this.Credentials != null || (webProxy != null && webProxy.Credentials != null)))
			{
				WindowsIdentity currentUser = this.SafeCaptureIdenity();
				return Task.Run<Stream>(delegate
				{
					Task<Stream> task;
					using (currentUser)
					{
						using (currentUser.Impersonate())
						{
							task = Task<Stream>.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(this.BeginGetRequestStream), new Func<IAsyncResult, Stream>(this.EndGetRequestStream), null);
						}
					}
					return task;
				});
			}
			return Task.Run<Stream>(() => Task<Stream>.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(this.BeginGetRequestStream), new Func<IAsyncResult, Stream>(this.EndGetRequestStream), null));
		}

		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public virtual Task<WebResponse> GetResponseAsync()
		{
			IWebProxy webProxy = null;
			try
			{
				webProxy = this.Proxy;
			}
			catch (NotImplementedException)
			{
			}
			if (ExecutionContext.IsFlowSuppressed() && (this.UseDefaultCredentials || this.Credentials != null || (webProxy != null && webProxy.Credentials != null)))
			{
				WindowsIdentity currentUser = this.SafeCaptureIdenity();
				return Task.Run<WebResponse>(delegate
				{
					Task<WebResponse> task;
					using (currentUser)
					{
						using (currentUser.Impersonate())
						{
							task = Task<WebResponse>.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(this.BeginGetResponse), new Func<IAsyncResult, WebResponse>(this.EndGetResponse), null);
						}
					}
					return task;
				});
			}
			return Task.Run<WebResponse>(() => Task<WebResponse>.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(this.BeginGetResponse), new Func<IAsyncResult, WebResponse>(this.EndGetResponse), null));
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.ControlPrincipal)]
		private WindowsIdentity SafeCaptureIdenity()
		{
			return WindowsIdentity.GetCurrent();
		}

		public virtual void Abort()
		{
			throw ExceptionHelper.MethodNotImplementedException;
		}

		internal RequestCacheProtocol CacheProtocol
		{
			get
			{
				return this.m_CacheProtocol;
			}
			set
			{
				this.m_CacheProtocol = value;
			}
		}

		public AuthenticationLevel AuthenticationLevel
		{
			get
			{
				return this.m_AuthenticationLevel;
			}
			set
			{
				this.m_AuthenticationLevel = value;
			}
		}

		public TokenImpersonationLevel ImpersonationLevel
		{
			get
			{
				return this.m_ImpersonationLevel;
			}
			set
			{
				this.m_ImpersonationLevel = value;
			}
		}

		internal static IWebProxy InternalDefaultWebProxy
		{
			get
			{
				if (!WebRequest.s_DefaultWebProxyInitialized)
				{
					object internalSyncObject = WebRequest.InternalSyncObject;
					lock (internalSyncObject)
					{
						if (!WebRequest.s_DefaultWebProxyInitialized)
						{
							DefaultProxySectionInternal section = DefaultProxySectionInternal.GetSection();
							if (section != null)
							{
								WebRequest.s_DefaultWebProxy = section.WebProxy;
							}
							WebRequest.s_DefaultWebProxyInitialized = true;
						}
					}
				}
				return WebRequest.s_DefaultWebProxy;
			}
			set
			{
				if (!WebRequest.s_DefaultWebProxyInitialized)
				{
					object internalSyncObject = WebRequest.InternalSyncObject;
					lock (internalSyncObject)
					{
						WebRequest.s_DefaultWebProxy = value;
						WebRequest.s_DefaultWebProxyInitialized = true;
						return;
					}
				}
				WebRequest.s_DefaultWebProxy = value;
			}
		}

		public static IWebProxy DefaultWebProxy
		{
			get
			{
				return WebRequest.InternalDefaultWebProxy;
			}
			set
			{
				WebRequest.InternalDefaultWebProxy = value;
			}
		}

		public static IWebProxy GetSystemWebProxy()
		{
			return WebRequest.InternalGetSystemWebProxy();
		}

		internal static IWebProxy InternalGetSystemWebProxy()
		{
			return WebProxy.CreateDefaultProxy();
		}

		internal void SetupCacheProtocol(Uri uri)
		{
			this.m_CacheBinding = RequestCacheManager.GetBinding(uri.Scheme);
			this.InternalSetCachePolicy(this.m_CacheBinding.Policy);
			if (this.m_CachePolicy == null)
			{
				this.InternalSetCachePolicy(WebRequest.DefaultCachePolicy);
			}
		}

		internal const int DefaultTimeout = 100000;

		private static volatile ArrayList s_PrefixList;

		private static object s_InternalSyncObject;

		private static TimerThread.Queue s_DefaultTimerQueue = TimerThread.CreateQueue(100000);

		private AuthenticationLevel m_AuthenticationLevel;

		private TokenImpersonationLevel m_ImpersonationLevel;

		private RequestCachePolicy m_CachePolicy;

		private RequestCacheProtocol m_CacheProtocol;

		private RequestCacheBinding m_CacheBinding;

		private static WebRequest.DesignerWebRequestCreate webRequestCreate = new WebRequest.DesignerWebRequestCreate();

		private static volatile IWebProxy s_DefaultWebProxy;

		private static volatile bool s_DefaultWebProxyInitialized;

		internal class DesignerWebRequestCreate : IWebRequestCreate
		{
			public WebRequest Create(Uri uri)
			{
				return WebRequest.Create(uri);
			}
		}

		internal class WebProxyWrapperOpaque : IAutoWebProxy, IWebProxy
		{
			internal WebProxyWrapperOpaque(WebProxy webProxy)
			{
				this.webProxy = webProxy;
			}

			public Uri GetProxy(Uri destination)
			{
				return this.webProxy.GetProxy(destination);
			}

			public bool IsBypassed(Uri host)
			{
				return this.webProxy.IsBypassed(host);
			}

			public ICredentials Credentials
			{
				get
				{
					return this.webProxy.Credentials;
				}
				set
				{
					this.webProxy.Credentials = value;
				}
			}

			public ProxyChain GetProxies(Uri destination)
			{
				return ((IAutoWebProxy)this.webProxy).GetProxies(destination);
			}

			protected readonly WebProxy webProxy;
		}

		internal class WebProxyWrapper : WebRequest.WebProxyWrapperOpaque
		{
			internal WebProxyWrapper(WebProxy webProxy)
				: base(webProxy)
			{
			}

			internal WebProxy WebProxy
			{
				get
				{
					return this.webProxy;
				}
			}
		}
	}
}
