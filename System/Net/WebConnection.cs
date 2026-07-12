using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mono.Net.Security;

namespace System.Net
{
	internal class WebConnection : IDisposable
	{
		public ServicePoint ServicePoint { get; }

		public WebConnection(ServicePoint sPoint)
		{
			this.ServicePoint = sPoint;
		}

		[Conditional("MONO_WEB_DEBUG")]
		internal static void Debug(string message, params object[] args)
		{
		}

		[Conditional("MONO_WEB_DEBUG")]
		internal static void Debug(string message)
		{
		}

		private bool CanReuse()
		{
			return !this.socket.Poll(0, SelectMode.SelectRead);
		}

		private bool CheckReusable()
		{
			if (this.socket != null && this.socket.Connected)
			{
				try
				{
					if (this.CanReuse())
					{
						return true;
					}
				}
				catch
				{
				}
				return false;
			}
			return false;
		}

		private async Task Connect(WebOperation operation, CancellationToken cancellationToken)
		{
			IPHostEntry hostEntry = this.ServicePoint.HostEntry;
			if (hostEntry == null || hostEntry.AddressList.Length == 0)
			{
				throw WebConnection.GetException(this.ServicePoint.UsesProxy ? WebExceptionStatus.ProxyNameResolutionFailure : WebExceptionStatus.NameResolutionFailure, null);
			}
			Exception connectException = null;
			foreach (IPAddress ipaddress in hostEntry.AddressList)
			{
				operation.ThrowIfDisposed(cancellationToken);
				try
				{
					this.socket = new Socket(ipaddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				}
				catch (Exception ex)
				{
					throw WebConnection.GetException(WebExceptionStatus.ConnectFailure, ex);
				}
				IPEndPoint ipendPoint = new IPEndPoint(ipaddress, this.ServicePoint.Address.Port);
				this.socket.NoDelay = !this.ServicePoint.UseNagleAlgorithm;
				try
				{
					this.ServicePoint.KeepAliveSetup(this.socket);
				}
				catch
				{
				}
				if (!this.ServicePoint.CallEndPointDelegate(this.socket, ipendPoint))
				{
					Socket socket = Interlocked.Exchange<Socket>(ref this.socket, null);
					if (socket != null)
					{
						socket.Close();
					}
				}
				else
				{
					try
					{
						operation.ThrowIfDisposed(cancellationToken);
						await this.socket.ConnectAsync(ipendPoint).ConfigureAwait(false);
					}
					catch (ObjectDisposedException)
					{
						throw;
					}
					catch (Exception ex2)
					{
						Socket socket2 = Interlocked.Exchange<Socket>(ref this.socket, null);
						if (socket2 != null)
						{
							socket2.Close();
						}
						connectException = WebConnection.GetException(WebExceptionStatus.ConnectFailure, ex2);
						goto IL_01DA;
					}
					if (this.socket != null)
					{
						return;
					}
				}
				IL_01DA:;
			}
			IPAddress[] array = null;
			if (connectException == null)
			{
				connectException = WebConnection.GetException(WebExceptionStatus.ConnectFailure, null);
			}
			throw connectException;
		}

		private async Task<bool> CreateStream(WebOperation operation, bool reused, CancellationToken cancellationToken)
		{
			bool flag;
			try
			{
				NetworkStream stream = new NetworkStream(this.socket, false);
				if (operation.Request.Address.Scheme == Uri.UriSchemeHttps)
				{
					if (!reused || this.monoTlsStream == null)
					{
						if (this.ServicePoint.UseConnect)
						{
							if (this.tunnel == null)
							{
								this.tunnel = new WebConnectionTunnel(operation.Request, this.ServicePoint.Address);
							}
							await this.tunnel.Initialize(stream, cancellationToken).ConfigureAwait(false);
							if (!this.tunnel.Success)
							{
								return false;
							}
						}
						this.monoTlsStream = new MonoTlsStream(operation.Request, stream);
						this.networkStream = await this.monoTlsStream.CreateStream(this.tunnel, cancellationToken).ConfigureAwait(false);
					}
					flag = true;
				}
				else
				{
					this.networkStream = stream;
					flag = true;
				}
			}
			catch (Exception ex)
			{
				ex = HttpWebRequest.FlattenException(ex);
				if (operation.Aborted || this.monoTlsStream == null)
				{
					throw WebConnection.GetException(WebExceptionStatus.ConnectFailure, ex);
				}
				throw WebConnection.GetException(this.monoTlsStream.ExceptionStatus, ex);
			}
			finally
			{
			}
			return flag;
		}

		internal async Task<WebRequestStream> InitConnection(WebOperation operation, CancellationToken cancellationToken)
		{
			bool flag = true;
			for (;;)
			{
				operation.ThrowIfClosedOrDisposed(cancellationToken);
				bool reused = this.CheckReusable();
				if (!reused)
				{
					this.CloseSocket();
					if (flag)
					{
						this.Reset();
					}
					try
					{
						await this.Connect(operation, cancellationToken).ConfigureAwait(false);
					}
					catch (Exception)
					{
						throw;
					}
				}
				ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter = this.CreateStream(operation, reused, cancellationToken).ConfigureAwait(false).GetAwaiter();
				if (!configuredTaskAwaiter.IsCompleted)
				{
					await configuredTaskAwaiter;
					ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter configuredTaskAwaiter2;
					configuredTaskAwaiter = configuredTaskAwaiter2;
					configuredTaskAwaiter2 = default(ConfiguredTaskAwaitable<bool>.ConfiguredTaskAwaiter);
				}
				if (configuredTaskAwaiter.GetResult())
				{
					goto IL_0180;
				}
				WebConnectionTunnel webConnectionTunnel = this.tunnel;
				if (((webConnectionTunnel != null) ? webConnectionTunnel.Challenge : null) == null)
				{
					break;
				}
				if (this.tunnel.CloseConnection)
				{
					this.CloseSocket();
				}
				flag = false;
			}
			throw WebConnection.GetException(WebExceptionStatus.ProtocolError, null);
			IL_0180:
			return new WebRequestStream(this, operation, this.networkStream, this.tunnel);
		}

		internal static WebException GetException(WebExceptionStatus status, Exception error)
		{
			if (error == null)
			{
				return new WebException(string.Format("Error: {0}", status), status);
			}
			WebException ex;
			if ((ex = error as WebException) != null)
			{
				return ex;
			}
			return new WebException(string.Format("Error: {0} ({1})", status, error.Message), status, WebExceptionInternalStatus.RequestFatal, error);
		}

		internal static bool ReadLine(byte[] buffer, ref int start, int max, ref string output)
		{
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			while (start < max)
			{
				int num2 = start;
				start = num2 + 1;
				num = (int)buffer[num2];
				if (num == 10)
				{
					if (stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] == '\r')
					{
						StringBuilder stringBuilder2 = stringBuilder;
						num2 = stringBuilder2.Length;
						stringBuilder2.Length = num2 - 1;
					}
					flag = false;
					break;
				}
				if (flag)
				{
					StringBuilder stringBuilder3 = stringBuilder;
					num2 = stringBuilder3.Length;
					stringBuilder3.Length = num2 - 1;
					break;
				}
				if (num == 13)
				{
					flag = true;
				}
				stringBuilder.Append((char)num);
			}
			if (num != 10 && num != 13)
			{
				return false;
			}
			if (stringBuilder.Length == 0)
			{
				output = null;
				return num == 10 || num == 13;
			}
			if (flag)
			{
				StringBuilder stringBuilder4 = stringBuilder;
				int num2 = stringBuilder4.Length;
				stringBuilder4.Length = num2 - 1;
			}
			output = stringBuilder.ToString();
			return true;
		}

		internal bool CanReuseConnection(WebOperation operation)
		{
			bool flag2;
			lock (this)
			{
				if (this.Closed || this.currentOperation != null)
				{
					flag2 = false;
				}
				else if (!this.NtlmAuthenticated)
				{
					flag2 = true;
				}
				else
				{
					NetworkCredential ntlmCredential = this.NtlmCredential;
					HttpWebRequest request = operation.Request;
					ICredentials credentials = ((request.Proxy == null || request.Proxy.IsBypassed(request.RequestUri)) ? request.Credentials : request.Proxy.Credentials);
					NetworkCredential networkCredential = ((credentials != null) ? credentials.GetCredential(request.RequestUri, "NTLM") : null);
					if (ntlmCredential == null || networkCredential == null || ntlmCredential.Domain != networkCredential.Domain || ntlmCredential.UserName != networkCredential.UserName || ntlmCredential.Password != networkCredential.Password)
					{
						flag2 = false;
					}
					else
					{
						bool unsafeAuthenticatedConnectionSharing = request.UnsafeAuthenticatedConnectionSharing;
						bool unsafeAuthenticatedConnectionSharing2 = this.UnsafeAuthenticatedConnectionSharing;
						flag2 = unsafeAuthenticatedConnectionSharing && unsafeAuthenticatedConnectionSharing == unsafeAuthenticatedConnectionSharing2;
					}
				}
			}
			return flag2;
		}

		private bool PrepareSharingNtlm(WebOperation operation)
		{
			if (operation == null || !this.NtlmAuthenticated)
			{
				return true;
			}
			bool flag = false;
			NetworkCredential ntlmCredential = this.NtlmCredential;
			HttpWebRequest request = operation.Request;
			ICredentials credentials = ((request.Proxy == null || request.Proxy.IsBypassed(request.RequestUri)) ? request.Credentials : request.Proxy.Credentials);
			NetworkCredential networkCredential = ((credentials != null) ? credentials.GetCredential(request.RequestUri, "NTLM") : null);
			if (ntlmCredential == null || networkCredential == null || ntlmCredential.Domain != networkCredential.Domain || ntlmCredential.UserName != networkCredential.UserName || ntlmCredential.Password != networkCredential.Password)
			{
				flag = true;
			}
			if (!flag)
			{
				bool unsafeAuthenticatedConnectionSharing = request.UnsafeAuthenticatedConnectionSharing;
				bool unsafeAuthenticatedConnectionSharing2 = this.UnsafeAuthenticatedConnectionSharing;
				flag = !unsafeAuthenticatedConnectionSharing || unsafeAuthenticatedConnectionSharing != unsafeAuthenticatedConnectionSharing2;
			}
			return flag;
		}

		private void Reset()
		{
			lock (this)
			{
				this.tunnel = null;
				this.ResetNtlm();
			}
		}

		private void Close(bool reset)
		{
			lock (this)
			{
				this.CloseSocket();
				if (reset)
				{
					this.Reset();
				}
			}
		}

		private void CloseSocket()
		{
			lock (this)
			{
				if (this.networkStream != null)
				{
					try
					{
						this.networkStream.Dispose();
					}
					catch
					{
					}
					this.networkStream = null;
				}
				if (this.socket != null)
				{
					try
					{
						this.socket.Dispose();
					}
					catch
					{
					}
					this.socket = null;
				}
				this.monoTlsStream = null;
			}
		}

		public bool Closed
		{
			get
			{
				return this.disposed != 0;
			}
		}

		public bool Busy
		{
			get
			{
				return this.currentOperation != null;
			}
		}

		public DateTime IdleSince
		{
			get
			{
				return this.idleSince;
			}
		}

		public bool StartOperation(WebOperation operation, bool reused)
		{
			lock (this)
			{
				if (this.Closed)
				{
					return false;
				}
				if (Interlocked.CompareExchange<WebOperation>(ref this.currentOperation, operation, null) != null)
				{
					return false;
				}
				this.idleSince = DateTime.UtcNow + TimeSpan.FromDays(3650.0);
				if (reused && !this.PrepareSharingNtlm(operation))
				{
					this.Close(true);
				}
				operation.RegisterRequest(this.ServicePoint, this);
			}
			operation.Run();
			return true;
		}

		public bool Continue(WebOperation next)
		{
			lock (this)
			{
				if (this.Closed)
				{
					return false;
				}
				if (this.socket == null || !this.socket.Connected || !this.PrepareSharingNtlm(next))
				{
					this.Close(true);
					return false;
				}
				this.currentOperation = next;
				if (next == null)
				{
					return true;
				}
				next.RegisterRequest(this.ServicePoint, this);
			}
			next.Run();
			return true;
		}

		private void Dispose(bool disposing)
		{
			if (Interlocked.CompareExchange(ref this.disposed, 1, 0) != 0)
			{
				return;
			}
			this.Close(true);
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		private void ResetNtlm()
		{
			this.ntlm_authenticated = false;
			this.ntlm_credentials = null;
			this.unsafe_sharing = false;
		}

		internal bool NtlmAuthenticated
		{
			get
			{
				return this.ntlm_authenticated;
			}
			set
			{
				this.ntlm_authenticated = value;
			}
		}

		internal NetworkCredential NtlmCredential
		{
			get
			{
				return this.ntlm_credentials;
			}
			set
			{
				this.ntlm_credentials = value;
			}
		}

		internal bool UnsafeAuthenticatedConnectionSharing
		{
			get
			{
				return this.unsafe_sharing;
			}
			set
			{
				this.unsafe_sharing = value;
			}
		}

		private NetworkCredential ntlm_credentials;

		private bool ntlm_authenticated;

		private bool unsafe_sharing;

		private Stream networkStream;

		private Socket socket;

		private MonoTlsStream monoTlsStream;

		private WebConnectionTunnel tunnel;

		private int disposed;

		internal readonly int ID;

		private DateTime idleSince;

		private WebOperation currentOperation;
	}
}
