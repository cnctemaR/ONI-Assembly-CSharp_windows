using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Mono.Security.Interface;

namespace Mono.Net.Security
{
	internal class MonoTlsStream
	{
		internal HttpWebRequest Request
		{
			get
			{
				return this.request;
			}
		}

		internal IMonoSslStream SslStream
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
			this.provider = request.TlsProvider ?? MonoTlsProviderFactory.GetProviderInternal();
			this.status = WebExceptionStatus.SecureChannelFailure;
			ChainValidationHelper.Create(this.provider, ref this.settings, this);
		}

		internal async Task<Stream> CreateStream(WebConnectionTunnel tunnel, CancellationToken cancellationToken)
		{
			Socket socket = this.networkStream.InternalSocket;
			this.sslStream = this.provider.CreateSslStream(this.networkStream, false, this.settings);
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
			}
			catch (Exception)
			{
				if (socket.CleanedUp)
				{
					this.status = WebExceptionStatus.RequestCanceled;
				}
				else
				{
					this.status = WebExceptionStatus.SecureChannelFailure;
				}
				throw;
			}
			finally
			{
				if (this.CertificateValidationFailed)
				{
					this.status = WebExceptionStatus.TrustFailure;
				}
				if (this.status == WebExceptionStatus.Success)
				{
					this.request.ServicePoint.UpdateClientCertificate(this.sslStream.InternalLocalCertificate);
				}
				else
				{
					this.request.ServicePoint.UpdateClientCertificate(null);
					this.sslStream = null;
				}
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
				this.sslStream = null;
				throw;
			}
			return this.sslStream.AuthenticatedStream;
		}

		private readonly MonoTlsProvider provider;

		private readonly NetworkStream networkStream;

		private readonly HttpWebRequest request;

		private readonly MonoTlsSettings settings;

		private IMonoSslStream sslStream;

		private WebExceptionStatus status;
	}
}
