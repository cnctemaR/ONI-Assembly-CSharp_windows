using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
	public class HttpClientHandler : HttpMessageHandler
	{
		private static IMonoHttpClientHandler CreateDefaultHandler()
		{
			return new MonoWebRequestHandler();
		}

		public static Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> DangerousAcceptAnyServerCertificateValidator
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		public HttpClientHandler()
			: this(HttpClientHandler.CreateDefaultHandler())
		{
		}

		internal HttpClientHandler(IMonoHttpClientHandler handler)
		{
			this._delegatingHandler = handler;
			this.ClientCertificateOptions = ClientCertificateOption.Manual;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this._delegatingHandler.Dispose();
			}
			base.Dispose(disposing);
		}

		public virtual bool SupportsAutomaticDecompression
		{
			get
			{
				return this._delegatingHandler.SupportsAutomaticDecompression;
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
				return this._delegatingHandler.UseCookies;
			}
			set
			{
				this._delegatingHandler.UseCookies = value;
			}
		}

		public CookieContainer CookieContainer
		{
			get
			{
				return this._delegatingHandler.CookieContainer;
			}
			set
			{
				this._delegatingHandler.CookieContainer = value;
			}
		}

		private void ThrowForModifiedManagedSslOptionsIfStarted()
		{
			this._delegatingHandler.SslOptions = this._delegatingHandler.SslOptions;
		}

		public ClientCertificateOption ClientCertificateOptions
		{
			get
			{
				return this._clientCertificateOptions;
			}
			set
			{
				if (value == ClientCertificateOption.Manual)
				{
					this.ThrowForModifiedManagedSslOptionsIfStarted();
					this._clientCertificateOptions = value;
					this._delegatingHandler.SslOptions.LocalCertificateSelectionCallback = (object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers) => CertificateHelper.GetEligibleClientCertificate(this.ClientCertificates);
					return;
				}
				if (value != ClientCertificateOption.Automatic)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.ThrowForModifiedManagedSslOptionsIfStarted();
				this._clientCertificateOptions = value;
				this._delegatingHandler.SslOptions.LocalCertificateSelectionCallback = (object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers) => CertificateHelper.GetEligibleClientCertificate();
			}
		}

		public X509CertificateCollection ClientCertificates
		{
			get
			{
				if (this.ClientCertificateOptions != ClientCertificateOption.Manual)
				{
					throw new InvalidOperationException(SR.Format("The {0} property must be set to '{1}' to use this property.", "ClientCertificateOptions", "Manual"));
				}
				X509CertificateCollection x509CertificateCollection;
				if ((x509CertificateCollection = this._delegatingHandler.SslOptions.ClientCertificates) == null)
				{
					x509CertificateCollection = (this._delegatingHandler.SslOptions.ClientCertificates = new X509CertificateCollection());
				}
				return x509CertificateCollection;
			}
		}

		public Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> ServerCertificateCustomValidationCallback
		{
			get
			{
				RemoteCertificateValidationCallback remoteCertificateValidationCallback = this._delegatingHandler.SslOptions.RemoteCertificateValidationCallback;
				ConnectHelper.CertificateCallbackMapper certificateCallbackMapper = ((remoteCertificateValidationCallback != null) ? remoteCertificateValidationCallback.Target : null) as ConnectHelper.CertificateCallbackMapper;
				if (certificateCallbackMapper == null)
				{
					return null;
				}
				return certificateCallbackMapper.FromHttpClientHandler;
			}
			set
			{
				this.ThrowForModifiedManagedSslOptionsIfStarted();
				this._delegatingHandler.SslOptions.RemoteCertificateValidationCallback = ((value != null) ? new ConnectHelper.CertificateCallbackMapper(value).ForSocketsHttpHandler : null);
			}
		}

		public bool CheckCertificateRevocationList
		{
			get
			{
				return this._delegatingHandler.SslOptions.CertificateRevocationCheckMode == X509RevocationMode.Online;
			}
			set
			{
				this.ThrowForModifiedManagedSslOptionsIfStarted();
				this._delegatingHandler.SslOptions.CertificateRevocationCheckMode = (value ? X509RevocationMode.Online : X509RevocationMode.NoCheck);
			}
		}

		public SslProtocols SslProtocols
		{
			get
			{
				return this._delegatingHandler.SslOptions.EnabledSslProtocols;
			}
			set
			{
				this.ThrowForModifiedManagedSslOptionsIfStarted();
				this._delegatingHandler.SslOptions.EnabledSslProtocols = value;
			}
		}

		public DecompressionMethods AutomaticDecompression
		{
			get
			{
				return this._delegatingHandler.AutomaticDecompression;
			}
			set
			{
				this._delegatingHandler.AutomaticDecompression = value;
			}
		}

		public bool UseProxy
		{
			get
			{
				return this._delegatingHandler.UseProxy;
			}
			set
			{
				this._delegatingHandler.UseProxy = value;
			}
		}

		public IWebProxy Proxy
		{
			get
			{
				return this._delegatingHandler.Proxy;
			}
			set
			{
				this._delegatingHandler.Proxy = value;
			}
		}

		public ICredentials DefaultProxyCredentials
		{
			get
			{
				return this._delegatingHandler.DefaultProxyCredentials;
			}
			set
			{
				this._delegatingHandler.DefaultProxyCredentials = value;
			}
		}

		public bool PreAuthenticate
		{
			get
			{
				return this._delegatingHandler.PreAuthenticate;
			}
			set
			{
				this._delegatingHandler.PreAuthenticate = value;
			}
		}

		public bool UseDefaultCredentials
		{
			get
			{
				return this._delegatingHandler.Credentials == CredentialCache.DefaultCredentials;
			}
			set
			{
				if (value)
				{
					this._delegatingHandler.Credentials = CredentialCache.DefaultCredentials;
					return;
				}
				if (this._delegatingHandler.Credentials == CredentialCache.DefaultCredentials)
				{
					this._delegatingHandler.Credentials = null;
				}
			}
		}

		public ICredentials Credentials
		{
			get
			{
				return this._delegatingHandler.Credentials;
			}
			set
			{
				this._delegatingHandler.Credentials = value;
			}
		}

		public bool AllowAutoRedirect
		{
			get
			{
				return this._delegatingHandler.AllowAutoRedirect;
			}
			set
			{
				this._delegatingHandler.AllowAutoRedirect = value;
			}
		}

		public int MaxAutomaticRedirections
		{
			get
			{
				return this._delegatingHandler.MaxAutomaticRedirections;
			}
			set
			{
				this._delegatingHandler.MaxAutomaticRedirections = value;
			}
		}

		public int MaxConnectionsPerServer
		{
			get
			{
				return this._delegatingHandler.MaxConnectionsPerServer;
			}
			set
			{
				this._delegatingHandler.MaxConnectionsPerServer = value;
			}
		}

		public int MaxResponseHeadersLength
		{
			get
			{
				return this._delegatingHandler.MaxResponseHeadersLength;
			}
			set
			{
				this._delegatingHandler.MaxResponseHeadersLength = value;
			}
		}

		public long MaxRequestContentBufferSize
		{
			get
			{
				return this._delegatingHandler.MaxRequestContentBufferSize;
			}
			set
			{
				this._delegatingHandler.MaxRequestContentBufferSize = value;
			}
		}

		public IDictionary<string, object> Properties
		{
			get
			{
				return this._delegatingHandler.Properties;
			}
		}

		internal void SetWebRequestTimeout(TimeSpan timeout)
		{
			this._delegatingHandler.SetWebRequestTimeout(timeout);
		}

		protected internal override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			return this._delegatingHandler.SendAsync(request, cancellationToken);
		}

		private readonly IMonoHttpClientHandler _delegatingHandler;

		private ClientCertificateOption _clientCertificateOptions;
	}
}
