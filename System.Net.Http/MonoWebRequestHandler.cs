using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Mono.Net.Security;
using Mono.Security.Interface;

namespace System.Net.Http
{
	internal class MonoWebRequestHandler : IMonoHttpClientHandler, IDisposable
	{
		public MonoWebRequestHandler()
		{
			this.allowAutoRedirect = true;
			this.maxAutomaticRedirections = 50;
			this.maxRequestContentBufferSize = 2147483647L;
			this.useCookies = true;
			this.useProxy = true;
			this.allowPipelining = true;
			this.authenticationLevel = AuthenticationLevel.MutualAuthRequested;
			this.cachePolicy = WebRequest.DefaultCachePolicy;
			this.continueTimeout = TimeSpan.FromMilliseconds(350.0);
			this.impersonationLevel = TokenImpersonationLevel.Delegation;
			this.maxResponseHeadersLength = HttpWebRequest.DefaultMaximumResponseHeadersLength;
			this.readWriteTimeout = 300000;
			this.serverCertificateValidationCallback = null;
			this.unsafeAuthenticatedConnectionSharing = false;
			this.connectionGroupName = "HttpClientHandler" + Interlocked.Increment(ref MonoWebRequestHandler.groupCounter).ToString();
		}

		internal void EnsureModifiability()
		{
			if (this.sentRequest)
			{
				throw new InvalidOperationException("This instance has already started one or more requests. Properties can only be modified before sending the first request.");
			}
		}

		public bool AllowAutoRedirect
		{
			get
			{
				return this.allowAutoRedirect;
			}
			set
			{
				this.EnsureModifiability();
				this.allowAutoRedirect = value;
			}
		}

		public DecompressionMethods AutomaticDecompression
		{
			get
			{
				return this.automaticDecompression;
			}
			set
			{
				this.EnsureModifiability();
				this.automaticDecompression = value;
			}
		}

		public CookieContainer CookieContainer
		{
			get
			{
				CookieContainer cookieContainer;
				if ((cookieContainer = this.cookieContainer) == null)
				{
					cookieContainer = (this.cookieContainer = new CookieContainer());
				}
				return cookieContainer;
			}
			set
			{
				this.EnsureModifiability();
				this.cookieContainer = value;
			}
		}

		public ICredentials Credentials
		{
			get
			{
				return this.credentials;
			}
			set
			{
				this.EnsureModifiability();
				this.credentials = value;
			}
		}

		public int MaxAutomaticRedirections
		{
			get
			{
				return this.maxAutomaticRedirections;
			}
			set
			{
				this.EnsureModifiability();
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.maxAutomaticRedirections = value;
			}
		}

		public long MaxRequestContentBufferSize
		{
			get
			{
				return this.maxRequestContentBufferSize;
			}
			set
			{
				this.EnsureModifiability();
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.maxRequestContentBufferSize = value;
			}
		}

		public bool PreAuthenticate
		{
			get
			{
				return this.preAuthenticate;
			}
			set
			{
				this.EnsureModifiability();
				this.preAuthenticate = value;
			}
		}

		public IWebProxy Proxy
		{
			get
			{
				return this.proxy;
			}
			set
			{
				this.EnsureModifiability();
				if (!this.UseProxy)
				{
					throw new InvalidOperationException();
				}
				this.proxy = value;
			}
		}

		public virtual bool SupportsAutomaticDecompression
		{
			get
			{
				return true;
			}
		}

		public virtual bool SupportsProxy
		{
			get
			{
				return true;
			}
		}

		public virtual bool SupportsRedirectConfiguration
		{
			get
			{
				return true;
			}
		}

		public bool UseCookies
		{
			get
			{
				return this.useCookies;
			}
			set
			{
				this.EnsureModifiability();
				this.useCookies = value;
			}
		}

		public bool UseProxy
		{
			get
			{
				return this.useProxy;
			}
			set
			{
				this.EnsureModifiability();
				this.useProxy = value;
			}
		}

		public bool AllowPipelining
		{
			get
			{
				return this.allowPipelining;
			}
			set
			{
				this.EnsureModifiability();
				this.allowPipelining = value;
			}
		}

		public RequestCachePolicy CachePolicy
		{
			get
			{
				return this.cachePolicy;
			}
			set
			{
				this.EnsureModifiability();
				this.cachePolicy = value;
			}
		}

		public AuthenticationLevel AuthenticationLevel
		{
			get
			{
				return this.authenticationLevel;
			}
			set
			{
				this.EnsureModifiability();
				this.authenticationLevel = value;
			}
		}

		[MonoTODO]
		public TimeSpan ContinueTimeout
		{
			get
			{
				return this.continueTimeout;
			}
			set
			{
				this.EnsureModifiability();
				this.continueTimeout = value;
			}
		}

		public TokenImpersonationLevel ImpersonationLevel
		{
			get
			{
				return this.impersonationLevel;
			}
			set
			{
				this.EnsureModifiability();
				this.impersonationLevel = value;
			}
		}

		public int MaxResponseHeadersLength
		{
			get
			{
				return this.maxResponseHeadersLength;
			}
			set
			{
				this.EnsureModifiability();
				this.maxResponseHeadersLength = value;
			}
		}

		public int ReadWriteTimeout
		{
			get
			{
				return this.readWriteTimeout;
			}
			set
			{
				this.EnsureModifiability();
				this.readWriteTimeout = value;
			}
		}

		public RemoteCertificateValidationCallback ServerCertificateValidationCallback
		{
			get
			{
				return this.serverCertificateValidationCallback;
			}
			set
			{
				this.EnsureModifiability();
				this.serverCertificateValidationCallback = value;
			}
		}

		public bool UnsafeAuthenticatedConnectionSharing
		{
			get
			{
				return this.unsafeAuthenticatedConnectionSharing;
			}
			set
			{
				this.EnsureModifiability();
				this.unsafeAuthenticatedConnectionSharing = value;
			}
		}

		public SslClientAuthenticationOptions SslOptions
		{
			get
			{
				SslClientAuthenticationOptions sslClientAuthenticationOptions;
				if ((sslClientAuthenticationOptions = this.sslOptions) == null)
				{
					sslClientAuthenticationOptions = (this.sslOptions = new SslClientAuthenticationOptions());
				}
				return sslClientAuthenticationOptions;
			}
			set
			{
				this.EnsureModifiability();
				this.sslOptions = value;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this.disposed)
			{
				Volatile.Write(ref this.disposed, true);
				ServicePointManager.CloseConnectionGroup(this.connectionGroupName);
			}
		}

		private bool GetConnectionKeepAlive(HttpRequestHeaders headers)
		{
			return headers.Connection.Any<string>((string l) => string.Equals(l, "Keep-Alive", StringComparison.OrdinalIgnoreCase));
		}

		internal virtual HttpWebRequest CreateWebRequest(HttpRequestMessage request)
		{
			HttpWebRequest httpWebRequest;
			if (HttpUtilities.IsSupportedSecureScheme(request.RequestUri.Scheme))
			{
				httpWebRequest = new HttpWebRequest(request.RequestUri, Mono.Net.Security.MonoTlsProviderFactory.GetProviderInternal(), MonoTlsSettings.CopyDefaultSettings());
				httpWebRequest.TlsSettings.ClientCertificateSelectionCallback = (string t, X509CertificateCollection lc, X509Certificate rc, string[] ai) => this.SslOptions.LocalCertificateSelectionCallback(this, t, lc, rc, ai);
			}
			else
			{
				httpWebRequest = new HttpWebRequest(request.RequestUri);
			}
			httpWebRequest.ThrowOnError = false;
			httpWebRequest.AllowWriteStreamBuffering = false;
			if (request.Version == HttpVersion.Version20)
			{
				httpWebRequest.ProtocolVersion = HttpVersion.Version11;
			}
			else
			{
				httpWebRequest.ProtocolVersion = request.Version;
			}
			httpWebRequest.ConnectionGroupName = this.connectionGroupName;
			httpWebRequest.Method = request.Method.Method;
			bool? flag;
			bool flag2;
			if (httpWebRequest.ProtocolVersion == HttpVersion.Version10)
			{
				httpWebRequest.KeepAlive = this.GetConnectionKeepAlive(request.Headers);
			}
			else
			{
				HttpWebRequest httpWebRequest2 = httpWebRequest;
				flag = request.Headers.ConnectionClose;
				flag2 = true;
				httpWebRequest2.KeepAlive = !((flag.GetValueOrDefault() == flag2) & (flag != null));
			}
			if (this.allowAutoRedirect)
			{
				httpWebRequest.AllowAutoRedirect = true;
				httpWebRequest.MaximumAutomaticRedirections = this.maxAutomaticRedirections;
			}
			else
			{
				httpWebRequest.AllowAutoRedirect = false;
			}
			httpWebRequest.AutomaticDecompression = this.automaticDecompression;
			httpWebRequest.PreAuthenticate = this.preAuthenticate;
			if (this.useCookies)
			{
				httpWebRequest.CookieContainer = this.CookieContainer;
			}
			httpWebRequest.Credentials = this.credentials;
			if (this.useProxy)
			{
				httpWebRequest.Proxy = this.proxy;
			}
			else
			{
				httpWebRequest.Proxy = null;
			}
			ServicePoint servicePoint = httpWebRequest.ServicePoint;
			flag = request.Headers.ExpectContinue;
			flag2 = true;
			servicePoint.Expect100Continue = (flag.GetValueOrDefault() == flag2) & (flag != null);
			if (this.timeout != null)
			{
				httpWebRequest.Timeout = (int)this.timeout.Value.TotalMilliseconds;
			}
			httpWebRequest.ServerCertificateValidationCallback = this.SslOptions.RemoteCertificateValidationCallback;
			WebHeaderCollection headers = httpWebRequest.Headers;
			foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in request.Headers)
			{
				IEnumerable<string> enumerable = keyValuePair.Value;
				if (keyValuePair.Key == "Host")
				{
					httpWebRequest.Host = request.Headers.Host;
				}
				else
				{
					if (keyValuePair.Key == "Transfer-Encoding")
					{
						enumerable = enumerable.Where<string>((string l) => l != "chunked");
					}
					string singleHeaderString = PlatformHelper.GetSingleHeaderString(keyValuePair.Key, enumerable);
					if (singleHeaderString != null)
					{
						headers.AddInternal(keyValuePair.Key, singleHeaderString);
					}
				}
			}
			return httpWebRequest;
		}

		private HttpResponseMessage CreateResponseMessage(HttpWebResponse wr, HttpRequestMessage requestMessage, CancellationToken cancellationToken)
		{
			HttpResponseMessage httpResponseMessage = new HttpResponseMessage(wr.StatusCode);
			httpResponseMessage.RequestMessage = requestMessage;
			httpResponseMessage.ReasonPhrase = wr.StatusDescription;
			httpResponseMessage.Content = PlatformHelper.CreateStreamContent(wr.GetResponseStream(), cancellationToken);
			WebHeaderCollection headers = wr.Headers;
			for (int i = 0; i < headers.Count; i++)
			{
				string key = headers.GetKey(i);
				string[] values = headers.GetValues(i);
				HttpHeaders httpHeaders;
				if (PlatformHelper.IsContentHeader(key))
				{
					httpHeaders = httpResponseMessage.Content.Headers;
				}
				else
				{
					httpHeaders = httpResponseMessage.Headers;
				}
				httpHeaders.TryAddWithoutValidation(key, values);
			}
			requestMessage.RequestUri = wr.ResponseUri;
			return httpResponseMessage;
		}

		private static bool MethodHasBody(HttpMethod method)
		{
			string method2 = method.Method;
			return !(method2 == "HEAD") && !(method2 == "GET") && !(method2 == "MKCOL") && !(method2 == "CONNECT") && !(method2 == "TRACE");
		}

		public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
			FieldInfo fieldInfo = typeof(CancellationToken).GetField("_source", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
			CancellationTokenSource cancellationTokenSource = (CancellationTokenSource)fieldInfo.GetValue(cancellationToken);
			fieldInfo = typeof(CancellationTokenSource).GetField("_timer", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
			Timer timer = (Timer)fieldInfo.GetValue(cancellationTokenSource);
			if (timer != null)
			{
				fieldInfo = typeof(Timer).GetField("due_time_ms", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic);
				this.timeout = new TimeSpan?(TimeSpan.FromMilliseconds((double)((long)fieldInfo.GetValue(timer))));
			}
			Volatile.Write(ref this.sentRequest, true);
			HttpWebRequest wrequest = this.CreateWebRequest(request);
			HttpWebResponse wresponse = null;
			try
			{
				using (cancellationToken.Register(delegate(object l)
				{
					((HttpWebRequest)l).Abort();
				}, wrequest))
				{
					HttpContent content = request.Content;
					if (content != null)
					{
						WebHeaderCollection headers = wrequest.Headers;
						foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in content.Headers)
						{
							foreach (string text in keyValuePair.Value)
							{
								headers.AddInternal(keyValuePair.Key, text);
							}
						}
						bool? transferEncodingChunked = request.Headers.TransferEncodingChunked;
						bool flag = true;
						if ((transferEncodingChunked.GetValueOrDefault() == flag) & (transferEncodingChunked != null))
						{
							wrequest.SendChunked = true;
						}
						else
						{
							long? contentLength = content.Headers.ContentLength;
							if (contentLength != null)
							{
								wrequest.ContentLength = contentLength.Value;
							}
							else
							{
								if (this.MaxRequestContentBufferSize == 0L)
								{
									throw new InvalidOperationException("The content length of the request content can't be determined. Either set TransferEncodingChunked to true, load content into buffer, or set MaxRequestContentBufferSize.");
								}
								await content.LoadIntoBufferAsync(this.MaxRequestContentBufferSize).ConfigureAwait(false);
								wrequest.ContentLength = content.Headers.ContentLength.Value;
							}
						}
						wrequest.ResendContentFactory = new Func<Stream, Task>(content.CopyToAsync);
						using (Stream stream = await wrequest.GetRequestStreamAsync().ConfigureAwait(false))
						{
							await request.Content.CopyToAsync(stream).ConfigureAwait(false);
						}
						Stream stream = null;
					}
					else if (MonoWebRequestHandler.MethodHasBody(request.Method))
					{
						wrequest.ContentLength = 0L;
					}
					wresponse = (HttpWebResponse)(await wrequest.GetResponseAsync().ConfigureAwait(false));
					content = null;
				}
				CancellationTokenRegistration cancellationTokenRegistration = default(CancellationTokenRegistration);
			}
			catch (WebException ex)
			{
				if (ex.Status != WebExceptionStatus.RequestCanceled)
				{
					throw new HttpRequestException("An error occurred while sending the request", ex);
				}
			}
			catch (IOException ex2)
			{
				throw new HttpRequestException("An error occurred while sending the request", ex2);
			}
			HttpResponseMessage httpResponseMessage;
			if (cancellationToken.IsCancellationRequested)
			{
				TaskCompletionSource<HttpResponseMessage> taskCompletionSource = new TaskCompletionSource<HttpResponseMessage>();
				taskCompletionSource.SetCanceled();
				httpResponseMessage = await taskCompletionSource.Task;
			}
			else
			{
				httpResponseMessage = this.CreateResponseMessage(wresponse, request, cancellationToken);
			}
			return httpResponseMessage;
		}

		public ICredentials DefaultProxyCredentials
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public int MaxConnectionsPerServer
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public IDictionary<string, object> Properties
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		void IMonoHttpClientHandler.SetWebRequestTimeout(TimeSpan timeout)
		{
			this.timeout = new TimeSpan?(timeout);
		}

		private static long groupCounter;

		private bool allowAutoRedirect;

		private DecompressionMethods automaticDecompression;

		private CookieContainer cookieContainer;

		private ICredentials credentials;

		private int maxAutomaticRedirections;

		private long maxRequestContentBufferSize;

		private bool preAuthenticate;

		private IWebProxy proxy;

		private bool useCookies;

		private bool useProxy;

		private SslClientAuthenticationOptions sslOptions;

		private bool allowPipelining;

		private RequestCachePolicy cachePolicy;

		private AuthenticationLevel authenticationLevel;

		private TimeSpan continueTimeout;

		private TokenImpersonationLevel impersonationLevel;

		private int maxResponseHeadersLength;

		private int readWriteTimeout;

		private RemoteCertificateValidationCallback serverCertificateValidationCallback;

		private bool unsafeAuthenticatedConnectionSharing;

		private bool sentRequest;

		private string connectionGroupName;

		private TimeSpan? timeout;

		private bool disposed;
	}
}
