using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Mono.Net.Security.Private;
using Mono.Security.Interface;

namespace Mono.Net.Security
{
	internal class MonoTlsStream : IDisposable
	{
		internal HttpWebRequest Request
		{
			get
			{
				return this.request;
			}
		}

		internal SslStream SslStream
		{
			get
			{
				return this.sslStream;
			}
		}

		internal WebExceptionStatus ExceptionStatus
		{
			get
			{
				return this.status;
			}
		}

		internal bool CertificateValidationFailed { get; set; }

		public MonoTlsStream(HttpWebRequest request, NetworkStream networkStream)
		{
			this.request = request;
			this.networkStream = networkStream;
			this.settings = request.TlsSettings;
			if (this.settings == null)
			{
				this.settings = MonoTlsSettings.CopyDefaultSettings();
			}
			if (this.settings.RemoteCertificateValidationCallback == null)
			{
				this.settings.RemoteCertificateValidationCallback = CallbackHelpers.PublicToMono(request.ServerCertificateValidationCallback);
			}
			this.provider = request.TlsProvider ?? MonoTlsProviderFactory.GetProviderInternal();
			this.status = WebExceptionStatus.SecureChannelFailure;
			ChainValidationHelper.Create(this.provider, ref this.settings, this);
		}

		internal async Task<Stream> CreateStream(WebConnectionTunnel tunnel, CancellationToken cancellationToken)
		{
			Socket socket = this.networkStream.InternalSocket;
			this.sslStream = new SslStream(this.networkStream, false, this.provider, this.settings);
			try
			{
				string text = this.request.Host;
				if (!string.IsNullOrEmpty(text))
				{
					int num = text.IndexOf(':');
					if (num > 0)
					{
						text = text.Substring(0, num);
					}
				}
				await this.sslStream.AuthenticateAsClientAsync(text, this.request.ClientCertificates, (SslProtocols)ServicePointManager.SecurityProtocol, ServicePointManager.CheckCertificateRevocationList).ConfigureAwait(false);
				this.status = WebExceptionStatus.Success;
				this.request.ServicePoint.UpdateClientCertificate(this.sslStream.LocalCertificate);
			}
			catch (Exception)
			{
				if (socket.CleanedUp)
				{
					this.status = WebExceptionStatus.RequestCanceled;
				}
				else if (this.CertificateValidationFailed)
				{
					this.status = WebExceptionStatus.TrustFailure;
				}
				else
				{
					this.status = WebExceptionStatus.SecureChannelFailure;
				}
				this.request.ServicePoint.UpdateClientCertificate(null);
				this.CloseSslStream();
				throw;
			}
			try
			{
				if (((tunnel != null) ? tunnel.Data : null) != null)
				{
					await this.sslStream.WriteAsync(tunnel.Data, 0, tunnel.Data.Length, cancellationToken).ConfigureAwait(false);
				}
			}
			catch
			{
				this.status = WebExceptionStatus.SendFailure;
				this.CloseSslStream();
				throw;
			}
			return this.sslStream;
		}

		public void Dispose()
		{
			this.CloseSslStream();
		}

		private void CloseSslStream()
		{
			object obj = this.sslStreamLock;
			lock (obj)
			{
				if (this.sslStream != null)
				{
					this.sslStream.Dispose();
					this.sslStream = null;
				}
			}
		}

		private readonly MobileTlsProvider provider;

		private readonly NetworkStream networkStream;

		private readonly HttpWebRequest request;

		private readonly MonoTlsSettings settings;

		private SslStream sslStream;

		private readonly object sslStreamLock = new object();

		private WebExceptionStatus status;
	}
}
