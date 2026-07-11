using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Mono.Net.Security;

namespace System.Net
{
	internal class WebConnection
	{
		internal MonoChunkStream MonoChunkStream
		{
			get
			{
				return this.chunkStream;
			}
		}

		public WebConnection(IWebConnectionState wcs, ServicePoint sPoint)
		{
			this.state = wcs;
			this.sPoint = sPoint;
			this.buffer = new byte[4096];
			this.Data = new WebConnectionData();
			this.queue = wcs.Group.Queue;
			this.abortHelper = new WebConnection.AbortHelper();
			this.abortHelper.Connection = this;
			this.abortHandler = new EventHandler(this.abortHelper.Abort);
		}

		private bool CanReuse()
		{
			return !this.socket.Poll(0, SelectMode.SelectRead);
		}

		private void Connect(HttpWebRequest request)
		{
			object obj = this.socketLock;
			lock (obj)
			{
				if (this.socket != null && this.socket.Connected && this.status == WebExceptionStatus.Success && this.CanReuse() && this.CompleteChunkedRead())
				{
					this.reused = true;
				}
				else
				{
					this.reused = false;
					if (this.socket != null)
					{
						this.socket.Close();
						this.socket = null;
					}
					this.chunkStream = null;
					IPHostEntry hostEntry = this.sPoint.HostEntry;
					if (hostEntry == null)
					{
						this.status = (this.sPoint.UsesProxy ? WebExceptionStatus.ProxyNameResolutionFailure : WebExceptionStatus.NameResolutionFailure);
					}
					else
					{
						foreach (IPAddress ipaddress in hostEntry.AddressList)
						{
							try
							{
								this.socket = new Socket(ipaddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
							}
							catch (Exception ex)
							{
								if (!request.Aborted)
								{
									this.status = WebExceptionStatus.ConnectFailure;
								}
								this.connect_exception = ex;
								break;
							}
							IPEndPoint ipendPoint = new IPEndPoint(ipaddress, this.sPoint.Address.Port);
							this.socket.NoDelay = !this.sPoint.UseNagleAlgorithm;
							try
							{
								this.sPoint.KeepAliveSetup(this.socket);
							}
							catch
							{
							}
							if (!this.sPoint.CallEndPointDelegate(this.socket, ipendPoint))
							{
								this.socket.Close();
								this.socket = null;
								this.status = WebExceptionStatus.ConnectFailure;
							}
							else
							{
								try
								{
									if (request.Aborted)
									{
										break;
									}
									this.socket.Connect(ipendPoint);
									this.status = WebExceptionStatus.Success;
									break;
								}
								catch (ThreadAbortException)
								{
									Socket socket = this.socket;
									this.socket = null;
									if (socket != null)
									{
										socket.Close();
									}
									break;
								}
								catch (ObjectDisposedException)
								{
									break;
								}
								catch (Exception ex2)
								{
									Socket socket2 = this.socket;
									this.socket = null;
									if (socket2 != null)
									{
										socket2.Close();
									}
									if (!request.Aborted)
									{
										this.status = WebExceptionStatus.ConnectFailure;
									}
									this.connect_exception = ex2;
								}
							}
						}
					}
				}
			}
		}

		private bool CreateTunnel(HttpWebRequest request, Uri connectUri, Stream stream, out byte[] buffer)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("CONNECT ");
			stringBuilder.Append(request.Address.Host);
			stringBuilder.Append(':');
			stringBuilder.Append(request.Address.Port);
			stringBuilder.Append(" HTTP/");
			if (request.ServicePoint.ProtocolVersion == HttpVersion.Version11)
			{
				stringBuilder.Append("1.1");
			}
			else
			{
				stringBuilder.Append("1.0");
			}
			stringBuilder.Append("\r\nHost: ");
			stringBuilder.Append(request.Address.Authority);
			bool flag = false;
			string[] challenge = this.Data.Challenge;
			this.Data.Challenge = null;
			string text = request.Headers["Proxy-Authorization"];
			bool flag2 = text != null;
			if (flag2)
			{
				stringBuilder.Append("\r\nProxy-Authorization: ");
				stringBuilder.Append(text);
				flag = text.ToUpper().Contains("NTLM");
			}
			else if (challenge != null && this.Data.StatusCode == 407)
			{
				ICredentials credentials = request.Proxy.Credentials;
				flag2 = true;
				if (this.connect_request == null)
				{
					this.connect_request = (HttpWebRequest)WebRequest.Create(string.Concat(new object[] { connectUri.Scheme, "://", connectUri.Host, ":", connectUri.Port, "/" }));
					this.connect_request.Method = "CONNECT";
					this.connect_request.Credentials = credentials;
				}
				if (credentials != null)
				{
					for (int i = 0; i < challenge.Length; i++)
					{
						Authorization authorization = AuthenticationManager.Authenticate(challenge[i], this.connect_request, credentials);
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
				this.connect_ntlm_auth_state++;
			}
			stringBuilder.Append("\r\n\r\n");
			this.Data.StatusCode = 0;
			byte[] bytes = Encoding.Default.GetBytes(stringBuilder.ToString());
			stream.Write(bytes, 0, bytes.Length);
			int num;
			WebHeaderCollection webHeaderCollection = this.ReadHeaders(stream, out buffer, out num);
			if ((!flag2 || this.connect_ntlm_auth_state == WebConnection.NtlmAuthState.Challenge) && webHeaderCollection != null && num == 407)
			{
				string text2 = webHeaderCollection["Connection"];
				if (this.socket != null && !string.IsNullOrEmpty(text2) && text2.ToLower() == "close")
				{
					this.socket.Close();
					this.socket = null;
				}
				this.Data.StatusCode = num;
				this.Data.Challenge = webHeaderCollection.GetValues("Proxy-Authenticate");
				this.Data.Headers = webHeaderCollection;
				return false;
			}
			if (num != 200)
			{
				this.Data.StatusCode = num;
				this.Data.Headers = webHeaderCollection;
				return false;
			}
			return webHeaderCollection != null;
		}

		private WebHeaderCollection ReadHeaders(Stream stream, out byte[] retBuffer, out int status)
		{
			retBuffer = null;
			status = 200;
			byte[] array = new byte[1024];
			MemoryStream memoryStream = new MemoryStream();
			int num2;
			WebHeaderCollection webHeaderCollection;
			for (;;)
			{
				int num = stream.Read(array, 0, 1024);
				if (num == 0)
				{
					break;
				}
				memoryStream.Write(array, 0, num);
				num2 = 0;
				string text = null;
				bool flag = false;
				webHeaderCollection = new WebHeaderCollection();
				while (WebConnection.ReadLine(memoryStream.GetBuffer(), ref num2, (int)memoryStream.Length, ref text))
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
						string[] array2 = text.Split(new char[] { ' ' });
						if (array2.Length < 2)
						{
							goto Block_6;
						}
						if (string.Compare(array2[0], "HTTP/1.1", true) == 0)
						{
							this.Data.ProxyVersion = HttpVersion.Version11;
						}
						else
						{
							if (string.Compare(array2[0], "HTTP/1.0", true) != 0)
							{
								goto IL_0153;
							}
							this.Data.ProxyVersion = HttpVersion.Version10;
						}
						status = (int)uint.Parse(array2[1]);
						if (array2.Length >= 3)
						{
							this.Data.StatusDescription = string.Join(" ", array2, 2, array2.Length - 2);
						}
						flag = true;
					}
				}
			}
			this.HandleError(WebExceptionStatus.ServerProtocolViolation, null, "ReadHeaders");
			return null;
			Block_2:
			int num3 = 0;
			try
			{
				num3 = int.Parse(webHeaderCollection["Content-Length"]);
			}
			catch
			{
				num3 = 0;
			}
			if (memoryStream.Length - (long)num2 - (long)num3 > 0L)
			{
				retBuffer = new byte[memoryStream.Length - (long)num2 - (long)num3];
				Buffer.BlockCopy(memoryStream.GetBuffer(), num2 + num3, retBuffer, 0, retBuffer.Length);
			}
			else
			{
				this.FlushContents(stream, num3 - (int)(memoryStream.Length - (long)num2));
			}
			return webHeaderCollection;
			Block_6:
			this.HandleError(WebExceptionStatus.ServerProtocolViolation, null, "ReadHeaders2");
			return null;
			IL_0153:
			this.HandleError(WebExceptionStatus.ServerProtocolViolation, null, "ReadHeaders2");
			return null;
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

		private bool CreateStream(HttpWebRequest request)
		{
			try
			{
				NetworkStream networkStream = new NetworkStream(this.socket, false);
				if (request.Address.Scheme == Uri.UriSchemeHttps)
				{
					if (!this.reused || this.nstream == null || this.tlsStream == null)
					{
						byte[] array = null;
						if (this.sPoint.UseConnect && !this.CreateTunnel(request, this.sPoint.Address, networkStream, out array))
						{
							return false;
						}
						this.tlsStream = new MonoTlsStream(request, networkStream);
						this.nstream = this.tlsStream.CreateStream(array);
					}
				}
				else
				{
					this.nstream = networkStream;
				}
			}
			catch (Exception ex)
			{
				if (this.tlsStream != null)
				{
					this.status = this.tlsStream.ExceptionStatus;
				}
				else if (!request.Aborted)
				{
					this.status = WebExceptionStatus.ConnectFailure;
				}
				this.connect_exception = ex;
				return false;
			}
			return true;
		}

		private void HandleError(WebExceptionStatus st, Exception ex, string where)
		{
			this.status = st;
			lock (this)
			{
				if (st == WebExceptionStatus.RequestCanceled)
				{
					this.Data = new WebConnectionData();
				}
			}
			if (ex == null)
			{
				try
				{
					throw new Exception(new StackTrace().ToString());
				}
				catch (Exception ex)
				{
				}
			}
			HttpWebRequest httpWebRequest = null;
			if (this.Data != null && this.Data.request != null)
			{
				httpWebRequest = this.Data.request;
			}
			this.Close(true);
			if (httpWebRequest != null)
			{
				httpWebRequest.FinishedReading = true;
				httpWebRequest.SetResponseError(st, ex, where);
			}
		}

		private void ReadDone(IAsyncResult result)
		{
			WebConnectionData data = this.Data;
			Stream stream = this.nstream;
			if (stream == null)
			{
				this.Close(true);
				return;
			}
			int num = -1;
			try
			{
				num = stream.EndRead(result);
			}
			catch (ObjectDisposedException)
			{
				return;
			}
			catch (Exception ex)
			{
				if (ex.InnerException is ObjectDisposedException)
				{
					return;
				}
				this.HandleError(WebExceptionStatus.ReceiveFailure, ex, "ReadDone1");
				return;
			}
			if (num == 0)
			{
				this.HandleError(WebExceptionStatus.ReceiveFailure, null, "ReadDone2");
				return;
			}
			if (num < 0)
			{
				this.HandleError(WebExceptionStatus.ServerProtocolViolation, null, "ReadDone3");
				return;
			}
			int num2 = -1;
			num += this.position;
			if (data.ReadState == ReadState.None)
			{
				Exception ex2 = null;
				try
				{
					num2 = WebConnection.GetResponse(data, this.sPoint, this.buffer, num);
				}
				catch (Exception ex2)
				{
				}
				if (ex2 != null || num2 == -1)
				{
					this.HandleError(WebExceptionStatus.ServerProtocolViolation, ex2, "ReadDone4");
					return;
				}
			}
			if (data.ReadState == ReadState.Aborted)
			{
				this.HandleError(WebExceptionStatus.RequestCanceled, null, "ReadDone");
				return;
			}
			if (data.ReadState != ReadState.Content)
			{
				int num3 = num * 2;
				byte[] array = new byte[(num3 < this.buffer.Length) ? this.buffer.Length : num3];
				Buffer.BlockCopy(this.buffer, 0, array, 0, num);
				this.buffer = array;
				this.position = num;
				data.ReadState = ReadState.None;
				this.InitRead();
				return;
			}
			this.position = 0;
			WebConnectionStream webConnectionStream = new WebConnectionStream(this, data);
			bool flag = WebConnection.ExpectContent(data.StatusCode, data.request.Method);
			string text = null;
			if (flag)
			{
				text = data.Headers["Transfer-Encoding"];
			}
			this.chunkedRead = text != null && text.IndexOf("chunked", StringComparison.OrdinalIgnoreCase) != -1;
			if (!this.chunkedRead)
			{
				webConnectionStream.ReadBuffer = this.buffer;
				webConnectionStream.ReadBufferOffset = num2;
				webConnectionStream.ReadBufferSize = num;
				try
				{
					webConnectionStream.CheckResponseInBuffer();
					goto IL_023A;
				}
				catch (Exception ex3)
				{
					this.HandleError(WebExceptionStatus.ReceiveFailure, ex3, "ReadDone7");
					goto IL_023A;
				}
			}
			if (this.chunkStream == null)
			{
				try
				{
					this.chunkStream = new MonoChunkStream(this.buffer, num2, num, data.Headers);
					goto IL_023A;
				}
				catch (Exception ex4)
				{
					this.HandleError(WebExceptionStatus.ServerProtocolViolation, ex4, "ReadDone5");
					return;
				}
			}
			this.chunkStream.ResetBuffer();
			try
			{
				this.chunkStream.Write(this.buffer, num2, num);
			}
			catch (Exception ex5)
			{
				this.HandleError(WebExceptionStatus.ServerProtocolViolation, ex5, "ReadDone6");
				return;
			}
			IL_023A:
			data.stream = webConnectionStream;
			if (!flag)
			{
				webConnectionStream.ForceCompletion();
			}
			data.request.SetResponseData(data);
		}

		private static bool ExpectContent(int statusCode, string method)
		{
			return !(method == "HEAD") && (statusCode >= 200 && statusCode != 204) && statusCode != 304;
		}

		internal void InitRead()
		{
			Stream stream = this.nstream;
			try
			{
				int num = this.buffer.Length - this.position;
				stream.BeginRead(this.buffer, this.position, num, new AsyncCallback(this.ReadDone), null);
			}
			catch (Exception ex)
			{
				this.HandleError(WebExceptionStatus.ReceiveFailure, ex, "InitRead");
			}
		}

		private static int GetResponse(WebConnectionData data, ServicePoint sPoint, byte[] buffer, int max)
		{
			int num = 0;
			string text = null;
			bool flag = false;
			bool flag2 = false;
			while (data.ReadState != ReadState.Aborted)
			{
				if (data.ReadState != ReadState.None)
				{
					goto IL_00DD;
				}
				if (!WebConnection.ReadLine(buffer, ref num, max, ref text))
				{
					return 0;
				}
				if (text == null)
				{
					flag2 = true;
				}
				else
				{
					flag2 = false;
					data.ReadState = ReadState.Status;
					string[] array = text.Split(new char[] { ' ' });
					if (array.Length < 2)
					{
						return -1;
					}
					if (string.Compare(array[0], "HTTP/1.1", true) == 0)
					{
						data.Version = HttpVersion.Version11;
						sPoint.SetVersion(HttpVersion.Version11);
					}
					else
					{
						data.Version = HttpVersion.Version10;
						sPoint.SetVersion(HttpVersion.Version10);
					}
					data.StatusCode = (int)uint.Parse(array[1]);
					if (array.Length >= 3)
					{
						data.StatusDescription = string.Join(" ", array, 2, array.Length - 2);
					}
					else
					{
						data.StatusDescription = "";
					}
					if (num >= max)
					{
						return num;
					}
					goto IL_00DD;
				}
				IL_0278:
				if (!flag2 && !flag)
				{
					return -1;
				}
				continue;
				IL_00DD:
				flag2 = false;
				if (data.ReadState != ReadState.Status)
				{
					goto IL_0278;
				}
				data.ReadState = ReadState.Headers;
				data.Headers = new WebHeaderCollection();
				ArrayList arrayList = new ArrayList();
				bool flag3 = false;
				while (!flag3 && WebConnection.ReadLine(buffer, ref num, max, ref text))
				{
					if (text == null)
					{
						flag3 = true;
					}
					else if (text.Length > 0 && (text[0] == ' ' || text[0] == '\t'))
					{
						int num2 = arrayList.Count - 1;
						if (num2 < 0)
						{
							break;
						}
						string text2 = (string)arrayList[num2] + text;
						arrayList[num2] = text2;
					}
					else
					{
						arrayList.Add(text);
					}
				}
				if (!flag3)
				{
					return 0;
				}
				foreach (object obj in arrayList)
				{
					string text3 = (string)obj;
					int num3 = text3.IndexOf(':');
					if (num3 == -1)
					{
						throw new ArgumentException("no colon found", "header");
					}
					string text4 = text3.Substring(0, num3);
					string text5 = text3.Substring(num3 + 1).Trim();
					WebHeaderCollection headers = data.Headers;
					if (WebHeaderCollection.AllowMultiValues(text4))
					{
						headers.AddInternal(text4, text5);
					}
					else
					{
						headers.SetInternal(text4, text5);
					}
				}
				if (data.StatusCode != 100)
				{
					data.ReadState = ReadState.Content;
					return num;
				}
				sPoint.SendContinue = true;
				if (num >= max)
				{
					return num;
				}
				if (data.request.ExpectContinue)
				{
					data.request.DoContinueDelegate(data.StatusCode, data.Headers);
					data.request.ExpectContinue = false;
				}
				data.ReadState = ReadState.None;
				flag = true;
				goto IL_0278;
			}
			return -1;
		}

		private void InitConnection(HttpWebRequest request)
		{
			request.WebConnection = this;
			if (request.ReuseConnection)
			{
				request.StoredConnection = this;
			}
			if (request.Aborted)
			{
				return;
			}
			this.keepAlive = request.KeepAlive;
			this.Data = new WebConnectionData(request);
			WebExceptionStatus webExceptionStatus;
			for (;;)
			{
				this.Connect(request);
				if (request.Aborted)
				{
					break;
				}
				if (this.status != WebExceptionStatus.Success)
				{
					goto Block_4;
				}
				if (this.CreateStream(request))
				{
					goto IL_0145;
				}
				if (request.Aborted)
				{
					return;
				}
				webExceptionStatus = this.status;
				if (this.Data.Challenge == null)
				{
					goto Block_8;
				}
			}
			return;
			Block_4:
			if (!request.Aborted)
			{
				request.SetWriteStreamError(this.status, this.connect_exception);
				this.Close(true);
			}
			return;
			Block_8:
			Exception ex = this.connect_exception;
			if (ex == null && (this.Data.StatusCode == 401 || this.Data.StatusCode == 407))
			{
				webExceptionStatus = WebExceptionStatus.ProtocolError;
				if (this.Data.Headers == null)
				{
					this.Data.Headers = new WebHeaderCollection();
				}
				HttpWebResponse httpWebResponse = new HttpWebResponse(this.sPoint.Address, "CONNECT", this.Data, null);
				ex = new WebException((this.Data.StatusCode == 407) ? "(407) Proxy Authentication Required" : "(401) Unauthorized", null, webExceptionStatus, httpWebResponse);
			}
			this.connect_exception = null;
			request.SetWriteStreamError(webExceptionStatus, ex);
			this.Close(true);
			return;
			IL_0145:
			request.SetWriteStream(new WebConnectionStream(this, request));
		}

		internal EventHandler SendRequest(HttpWebRequest request)
		{
			if (request.Aborted)
			{
				return null;
			}
			lock (this)
			{
				if (this.state.TrySetBusy())
				{
					this.status = WebExceptionStatus.Success;
					ThreadPool.QueueUserWorkItem(delegate(object o)
					{
						try
						{
							this.InitConnection((HttpWebRequest)o);
						}
						catch
						{
						}
					}, request);
				}
				else
				{
					Queue queue = this.queue;
					lock (queue)
					{
						this.queue.Enqueue(request);
					}
				}
			}
			return this.abortHandler;
		}

		private void SendNext()
		{
			Queue queue = this.queue;
			lock (queue)
			{
				if (this.queue.Count > 0)
				{
					this.SendRequest((HttpWebRequest)this.queue.Dequeue());
				}
			}
		}

		internal void NextRead()
		{
			lock (this)
			{
				if (this.Data.request != null)
				{
					this.Data.request.FinishedReading = true;
				}
				string text = (this.sPoint.UsesProxy ? "Proxy-Connection" : "Connection");
				string text2 = ((this.Data.Headers != null) ? this.Data.Headers[text] : null);
				bool flag2 = this.Data.Version == HttpVersion.Version11 && this.keepAlive;
				if (this.Data.ProxyVersion != null && this.Data.ProxyVersion != HttpVersion.Version11)
				{
					flag2 = false;
				}
				if (text2 != null)
				{
					text2 = text2.ToLower();
					flag2 = this.keepAlive && text2.IndexOf("keep-alive", StringComparison.Ordinal) != -1;
				}
				if ((this.socket != null && !this.socket.Connected) || !flag2 || (text2 != null && text2.IndexOf("close", StringComparison.Ordinal) != -1))
				{
					this.Close(false);
				}
				this.state.SetIdle();
				if (this.priority_request != null)
				{
					this.SendRequest(this.priority_request);
					this.priority_request = null;
				}
				else
				{
					this.SendNext();
				}
			}
		}

		private static bool ReadLine(byte[] buffer, ref int start, int max, ref string output)
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

		internal IAsyncResult BeginRead(HttpWebRequest request, byte[] buffer, int offset, int size, AsyncCallback cb, object state)
		{
			Stream stream = null;
			lock (this)
			{
				if (this.Data.request != request)
				{
					throw new ObjectDisposedException(typeof(NetworkStream).FullName);
				}
				if (this.nstream == null)
				{
					return null;
				}
				stream = this.nstream;
			}
			IAsyncResult asyncResult = null;
			if (!this.chunkedRead || (!this.chunkStream.DataAvailable && this.chunkStream.WantMore))
			{
				try
				{
					asyncResult = stream.BeginRead(buffer, offset, size, cb, state);
					cb = null;
				}
				catch (Exception)
				{
					this.HandleError(WebExceptionStatus.ReceiveFailure, null, "chunked BeginRead");
					throw;
				}
			}
			if (this.chunkedRead)
			{
				WebAsyncResult webAsyncResult = new WebAsyncResult(cb, state, buffer, offset, size);
				webAsyncResult.InnerAsyncResult = asyncResult;
				if (asyncResult == null)
				{
					webAsyncResult.SetCompleted(true, null);
					webAsyncResult.DoCallback();
				}
				return webAsyncResult;
			}
			return asyncResult;
		}

		internal int EndRead(HttpWebRequest request, IAsyncResult result)
		{
			Stream stream = null;
			lock (this)
			{
				if (request.Aborted)
				{
					throw new WebException("Request aborted", WebExceptionStatus.RequestCanceled);
				}
				if (this.Data.request != request)
				{
					throw new ObjectDisposedException(typeof(NetworkStream).FullName);
				}
				if (this.nstream == null)
				{
					throw new ObjectDisposedException(typeof(NetworkStream).FullName);
				}
				stream = this.nstream;
			}
			int num = 0;
			bool flag2 = false;
			WebAsyncResult webAsyncResult = null;
			IAsyncResult innerAsyncResult = ((WebAsyncResult)result).InnerAsyncResult;
			if (this.chunkedRead && innerAsyncResult is WebAsyncResult)
			{
				webAsyncResult = (WebAsyncResult)innerAsyncResult;
				IAsyncResult innerAsyncResult2 = webAsyncResult.InnerAsyncResult;
				if (innerAsyncResult2 != null && !(innerAsyncResult2 is WebAsyncResult))
				{
					num = stream.EndRead(innerAsyncResult2);
					flag2 = num == 0;
				}
			}
			else if (!(innerAsyncResult is WebAsyncResult))
			{
				num = stream.EndRead(innerAsyncResult);
				webAsyncResult = (WebAsyncResult)result;
				flag2 = num == 0;
			}
			if (this.chunkedRead)
			{
				try
				{
					this.chunkStream.WriteAndReadBack(webAsyncResult.Buffer, webAsyncResult.Offset, webAsyncResult.Size, ref num);
					if (!flag2 && num == 0 && this.chunkStream.WantMore)
					{
						num = this.EnsureRead(webAsyncResult.Buffer, webAsyncResult.Offset, webAsyncResult.Size);
					}
				}
				catch (Exception ex)
				{
					if (ex is WebException)
					{
						throw ex;
					}
					throw new WebException("Invalid chunked data.", ex, WebExceptionStatus.ServerProtocolViolation, null);
				}
				if ((flag2 || num == 0) && this.chunkStream.ChunkLeft != 0)
				{
					this.HandleError(WebExceptionStatus.ReceiveFailure, null, "chunked EndRead");
					throw new WebException("Read error", null, WebExceptionStatus.ReceiveFailure, null);
				}
			}
			if (num == 0)
			{
				return -1;
			}
			return num;
		}

		private int EnsureRead(byte[] buffer, int offset, int size)
		{
			byte[] array = null;
			int num = 0;
			while (num == 0 && this.chunkStream.WantMore)
			{
				int num2 = this.chunkStream.ChunkLeft;
				if (num2 <= 0)
				{
					num2 = 1024;
				}
				else if (num2 > 16384)
				{
					num2 = 16384;
				}
				if (array == null || array.Length < num2)
				{
					array = new byte[num2];
				}
				int num3 = this.nstream.Read(array, 0, num2);
				if (num3 <= 0)
				{
					return 0;
				}
				this.chunkStream.Write(array, 0, num3);
				num += this.chunkStream.Read(buffer, offset + num, size - num);
			}
			return num;
		}

		private bool CompleteChunkedRead()
		{
			if (!this.chunkedRead || this.chunkStream == null)
			{
				return true;
			}
			while (this.chunkStream.WantMore)
			{
				int num = this.nstream.Read(this.buffer, 0, this.buffer.Length);
				if (num <= 0)
				{
					return false;
				}
				this.chunkStream.Write(this.buffer, 0, num);
			}
			return true;
		}

		internal IAsyncResult BeginWrite(HttpWebRequest request, byte[] buffer, int offset, int size, AsyncCallback cb, object state)
		{
			Stream stream = null;
			WebConnection webConnection = this;
			lock (webConnection)
			{
				if (this.Data.request != request)
				{
					throw new ObjectDisposedException(typeof(NetworkStream).FullName);
				}
				if (this.nstream == null)
				{
					return null;
				}
				stream = this.nstream;
			}
			IAsyncResult asyncResult = null;
			try
			{
				asyncResult = stream.BeginWrite(buffer, offset, size, cb, state);
			}
			catch (ObjectDisposedException)
			{
				webConnection = this;
				lock (webConnection)
				{
					if (this.Data.request != request)
					{
						return null;
					}
				}
				throw;
			}
			catch (IOException ex)
			{
				SocketException ex2 = ex.InnerException as SocketException;
				if (ex2 != null && ex2.SocketErrorCode == SocketError.NotConnected)
				{
					return null;
				}
				throw;
			}
			catch (Exception)
			{
				this.status = WebExceptionStatus.SendFailure;
				throw;
			}
			return asyncResult;
		}

		internal bool EndWrite(HttpWebRequest request, bool throwOnError, IAsyncResult result)
		{
			Stream stream = null;
			lock (this)
			{
				if (this.status == WebExceptionStatus.RequestCanceled)
				{
					return true;
				}
				if (this.Data.request != request)
				{
					throw new ObjectDisposedException(typeof(NetworkStream).FullName);
				}
				if (this.nstream == null)
				{
					throw new ObjectDisposedException(typeof(NetworkStream).FullName);
				}
				stream = this.nstream;
			}
			bool flag2;
			try
			{
				stream.EndWrite(result);
				flag2 = true;
			}
			catch (Exception ex)
			{
				this.status = WebExceptionStatus.SendFailure;
				if (throwOnError && ex.InnerException != null)
				{
					throw ex.InnerException;
				}
				flag2 = false;
			}
			return flag2;
		}

		internal int Read(HttpWebRequest request, byte[] buffer, int offset, int size)
		{
			Stream stream = null;
			lock (this)
			{
				if (this.Data.request != request)
				{
					throw new ObjectDisposedException(typeof(NetworkStream).FullName);
				}
				if (this.nstream == null)
				{
					return 0;
				}
				stream = this.nstream;
			}
			int num = 0;
			try
			{
				bool flag2 = false;
				if (!this.chunkedRead)
				{
					num = stream.Read(buffer, offset, size);
					flag2 = num == 0;
				}
				if (this.chunkedRead)
				{
					try
					{
						this.chunkStream.WriteAndReadBack(buffer, offset, size, ref num);
						if (!flag2 && num == 0 && this.chunkStream.WantMore)
						{
							num = this.EnsureRead(buffer, offset, size);
						}
					}
					catch (Exception ex)
					{
						this.HandleError(WebExceptionStatus.ReceiveFailure, ex, "chunked Read1");
						throw;
					}
					if ((flag2 || num == 0) && this.chunkStream.WantMore)
					{
						this.HandleError(WebExceptionStatus.ReceiveFailure, null, "chunked Read2");
						throw new WebException("Read error", null, WebExceptionStatus.ReceiveFailure, null);
					}
				}
			}
			catch (Exception ex2)
			{
				this.HandleError(WebExceptionStatus.ReceiveFailure, ex2, "Read");
			}
			return num;
		}

		internal bool Write(HttpWebRequest request, byte[] buffer, int offset, int size, ref string err_msg)
		{
			err_msg = null;
			Stream stream = null;
			lock (this)
			{
				if (this.Data.request != request)
				{
					throw new ObjectDisposedException(typeof(NetworkStream).FullName);
				}
				stream = this.nstream;
				if (stream == null)
				{
					return false;
				}
			}
			try
			{
				stream.Write(buffer, offset, size);
			}
			catch (Exception ex)
			{
				err_msg = ex.Message;
				WebExceptionStatus webExceptionStatus = WebExceptionStatus.SendFailure;
				string text = "Write: " + err_msg;
				WebException ex2 = ex as WebException;
				this.HandleError(webExceptionStatus, ex, text);
				return false;
			}
			return true;
		}

		internal void Close(bool sendNext)
		{
			lock (this)
			{
				if (this.Data != null && this.Data.request != null && this.Data.request.ReuseConnection)
				{
					this.Data.request.ReuseConnection = false;
				}
				else
				{
					if (this.nstream != null)
					{
						try
						{
							this.nstream.Close();
						}
						catch
						{
						}
						this.nstream = null;
					}
					if (this.socket != null)
					{
						try
						{
							this.socket.Close();
						}
						catch
						{
						}
						this.socket = null;
					}
					if (this.ntlm_authenticated)
					{
						this.ResetNtlm();
					}
					if (this.Data != null)
					{
						WebConnectionData data = this.Data;
						lock (data)
						{
							this.Data.ReadState = ReadState.Aborted;
						}
					}
					this.state.SetIdle();
					this.Data = new WebConnectionData();
					if (sendNext)
					{
						this.SendNext();
					}
					this.connect_request = null;
					this.connect_ntlm_auth_state = WebConnection.NtlmAuthState.None;
				}
			}
		}

		private void Abort(object sender, EventArgs args)
		{
			lock (this)
			{
				Queue queue = this.queue;
				lock (queue)
				{
					HttpWebRequest httpWebRequest = (HttpWebRequest)sender;
					if (this.Data.request == httpWebRequest || this.Data.request == null)
					{
						if (!httpWebRequest.FinishedReading)
						{
							this.status = WebExceptionStatus.RequestCanceled;
							this.Close(false);
							if (this.queue.Count > 0)
							{
								this.Data.request = (HttpWebRequest)this.queue.Dequeue();
								this.SendRequest(this.Data.request);
							}
						}
					}
					else
					{
						httpWebRequest.FinishedReading = true;
						httpWebRequest.SetResponseError(WebExceptionStatus.RequestCanceled, null, "User aborted");
						if (this.queue.Count > 0 && this.queue.Peek() == sender)
						{
							this.queue.Dequeue();
						}
						else if (this.queue.Count > 0)
						{
							object[] array = this.queue.ToArray();
							this.queue.Clear();
							for (int i = array.Length - 1; i >= 0; i--)
							{
								if (array[i] != sender)
								{
									this.queue.Enqueue(array[i]);
								}
							}
						}
					}
				}
			}
		}

		internal void ResetNtlm()
		{
			this.ntlm_authenticated = false;
			this.ntlm_credentials = null;
			this.unsafe_sharing = false;
		}

		internal bool Connected
		{
			get
			{
				bool flag2;
				lock (this)
				{
					flag2 = this.socket != null && this.socket.Connected;
				}
				return flag2;
			}
		}

		internal HttpWebRequest PriorityRequest
		{
			set
			{
				this.priority_request = value;
			}
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

		private ServicePoint sPoint;

		private Stream nstream;

		internal Socket socket;

		private object socketLock = new object();

		private IWebConnectionState state;

		private WebExceptionStatus status;

		private bool keepAlive;

		private byte[] buffer;

		private EventHandler abortHandler;

		private WebConnection.AbortHelper abortHelper;

		internal WebConnectionData Data;

		private bool chunkedRead;

		private MonoChunkStream chunkStream;

		private Queue queue;

		private bool reused;

		private int position;

		private HttpWebRequest priority_request;

		private NetworkCredential ntlm_credentials;

		private bool ntlm_authenticated;

		private bool unsafe_sharing;

		private WebConnection.NtlmAuthState connect_ntlm_auth_state;

		private HttpWebRequest connect_request;

		private Exception connect_exception;

		private MonoTlsStream tlsStream;

		private enum NtlmAuthState
		{
			None,
			Challenge,
			Response
		}

		private class AbortHelper
		{
			public void Abort(object sender, EventArgs args)
			{
				WebConnection webConnection = ((HttpWebRequest)sender).WebConnection;
				if (webConnection == null)
				{
					webConnection = this.Connection;
				}
				webConnection.Abort(sender, args);
			}

			public WebConnection Connection;
		}
	}
}
