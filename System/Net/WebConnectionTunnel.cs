using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net
{
	internal class WebConnectionTunnel
	{
		public HttpWebRequest Request { get; }

		public Uri ConnectUri { get; }

		public WebConnectionTunnel(HttpWebRequest request, Uri connectUri)
		{
			this.Request = request;
			this.ConnectUri = connectUri;
		}

		public bool Success { get; private set; }

		public bool CloseConnection { get; private set; }

		public int StatusCode { get; private set; }

		public string StatusDescription { get; private set; }

		public string[] Challenge { get; private set; }

		public WebHeaderCollection Headers { get; private set; }

		public Version ProxyVersion { get; private set; }

		public byte[] Data { get; private set; }

		internal async Task Initialize(Stream stream, CancellationToken cancellationToken)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("CONNECT ");
			stringBuilder.Append(this.Request.Address.Host);
			stringBuilder.Append(':');
			stringBuilder.Append(this.Request.Address.Port);
			stringBuilder.Append(" HTTP/");
			if (this.Request.ProtocolVersion == HttpVersion.Version11)
			{
				stringBuilder.Append("1.1");
			}
			else
			{
				stringBuilder.Append("1.0");
			}
			stringBuilder.Append("\r\nHost: ");
			stringBuilder.Append(this.Request.Address.Authority);
			bool flag = false;
			string[] challenge = this.Challenge;
			this.Challenge = null;
			string text = this.Request.Headers["Proxy-Authorization"];
			bool have_auth = text != null;
			if (have_auth)
			{
				stringBuilder.Append("\r\nProxy-Authorization: ");
				stringBuilder.Append(text);
				flag = text.ToUpper().Contains("NTLM");
			}
			else if (challenge != null && this.StatusCode == 407)
			{
				ICredentials credentials = this.Request.Proxy.Credentials;
				have_auth = true;
				if (this.connectRequest == null)
				{
					this.connectRequest = (HttpWebRequest)WebRequest.Create(string.Concat(new string[]
					{
						this.ConnectUri.Scheme,
						"://",
						this.ConnectUri.Host,
						":",
						this.ConnectUri.Port.ToString(),
						"/"
					}));
					this.connectRequest.Method = "CONNECT";
					this.connectRequest.Credentials = credentials;
				}
				if (credentials != null)
				{
					for (int i = 0; i < challenge.Length; i++)
					{
						Authorization authorization = AuthenticationManager.Authenticate(challenge[i], this.connectRequest, credentials);
						if (authorization != null)
						{
							flag = authorization.ModuleAuthenticationType == "NTLM";
							stringBuilder.Append("\r\nProxy-Authorization: ");
							stringBuilder.Append(authorization.Message);
							break;
						}
					}
				}
			}
			if (flag)
			{
				stringBuilder.Append("\r\nProxy-Connection: keep-alive");
				this.ntlmAuthState++;
			}
			stringBuilder.Append("\r\n\r\n");
			this.StatusCode = 0;
			byte[] bytes = Encoding.Default.GetBytes(stringBuilder.ToString());
			await stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken).ConfigureAwait(false);
			ValueTuple<WebHeaderCollection, byte[], int> valueTuple = await this.ReadHeaders(stream, cancellationToken).ConfigureAwait(false);
			this.Headers = valueTuple.Item1;
			this.Data = valueTuple.Item2;
			this.StatusCode = valueTuple.Item3;
			if ((!have_auth || this.ntlmAuthState == WebConnectionTunnel.NtlmAuthState.Challenge) && this.Headers != null && this.StatusCode == 407)
			{
				string text2 = this.Headers["Connection"];
				if (!string.IsNullOrEmpty(text2) && text2.ToLower() == "close")
				{
					this.CloseConnection = true;
				}
				this.Challenge = this.Headers.GetValues("Proxy-Authenticate");
				this.Success = false;
			}
			else
			{
				this.Success = this.StatusCode == 200 && this.Headers != null;
			}
			if (this.Challenge == null && (this.StatusCode == 401 || this.StatusCode == 407))
			{
				HttpWebResponse httpWebResponse = new HttpWebResponse(this.ConnectUri, "CONNECT", (HttpStatusCode)this.StatusCode, this.Headers);
				throw new WebException((this.StatusCode == 407) ? "(407) Proxy Authentication Required" : "(401) Unauthorized", null, WebExceptionStatus.ProtocolError, httpWebResponse);
			}
		}

		private async Task<ValueTuple<WebHeaderCollection, byte[], int>> ReadHeaders(Stream stream, CancellationToken cancellationToken)
		{
			byte[] retBuffer = null;
			int status = 200;
			byte[] buffer = new byte[1024];
			MemoryStream ms = new MemoryStream();
			int num2;
			WebHeaderCollection webHeaderCollection;
			for (;;)
			{
				cancellationToken.ThrowIfCancellationRequested();
				int num = await stream.ReadAsync(buffer, 0, 1024, cancellationToken).ConfigureAwait(false);
				if (num == 0)
				{
					break;
				}
				ms.Write(buffer, 0, num);
				num2 = 0;
				string text = null;
				bool flag = false;
				webHeaderCollection = new WebHeaderCollection();
				while (WebConnection.ReadLine(ms.GetBuffer(), ref num2, (int)ms.Length, ref text))
				{
					if (text == null)
					{
						goto Block_2;
					}
					if (flag)
					{
						webHeaderCollection.Add(text);
					}
					else
					{
						string[] array = text.Split(' ', StringSplitOptions.None);
						if (array.Length < 2)
						{
							goto Block_6;
						}
						if (string.Compare(array[0], "HTTP/1.1", true) == 0)
						{
							this.ProxyVersion = HttpVersion.Version11;
						}
						else
						{
							if (string.Compare(array[0], "HTTP/1.0", true) != 0)
							{
								goto IL_022A;
							}
							this.ProxyVersion = HttpVersion.Version10;
						}
						status = (int)uint.Parse(array[1]);
						if (array.Length >= 3)
						{
							this.StatusDescription = string.Join(" ", array, 2, array.Length - 2);
						}
						flag = true;
					}
				}
			}
			throw WebConnection.GetException(WebExceptionStatus.ServerProtocolViolation, null);
			Block_2:
			string text2 = webHeaderCollection["Content-Length"];
			int num3;
			if (string.IsNullOrEmpty(text2) || !int.TryParse(text2, out num3))
			{
				num3 = 0;
			}
			if (ms.Length - (long)num2 - (long)num3 > 0L)
			{
				retBuffer = new byte[ms.Length - (long)num2 - (long)num3];
				Buffer.BlockCopy(ms.GetBuffer(), num2 + num3, retBuffer, 0, retBuffer.Length);
			}
			else
			{
				this.FlushContents(stream, num3 - (int)(ms.Length - (long)num2));
			}
			return new ValueTuple<WebHeaderCollection, byte[], int>(webHeaderCollection, retBuffer, status);
			Block_6:
			throw WebConnection.GetException(WebExceptionStatus.ServerProtocolViolation, null);
			IL_022A:
			throw WebConnection.GetException(WebExceptionStatus.ServerProtocolViolation, null);
		}

		private void FlushContents(Stream stream, int contentLength)
		{
			while (contentLength > 0)
			{
				byte[] array = new byte[contentLength];
				int num = stream.Read(array, 0, contentLength);
				if (num <= 0)
				{
					break;
				}
				contentLength -= num;
			}
		}

		private HttpWebRequest connectRequest;

		private WebConnectionTunnel.NtlmAuthState ntlmAuthState;

		private enum NtlmAuthState
		{
			None,
			Challenge,
			Response
		}
	}
}
