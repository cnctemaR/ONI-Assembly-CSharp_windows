using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Net
{
	internal class FtpControlStream : CommandStream
	{
		internal NetworkCredential Credentials
		{
			get
			{
				if (this._credentials != null && this._credentials.IsAlive)
				{
					return (NetworkCredential)this._credentials.Target;
				}
				return null;
			}
			set
			{
				if (this._credentials == null)
				{
					this._credentials = new WeakReference(null);
				}
				this._credentials.Target = value;
			}
		}

		internal FtpControlStream(TcpClient client)
			: base(client)
		{
		}

		internal void AbortConnect()
		{
			Socket dataSocket = this._dataSocket;
			if (dataSocket != null)
			{
				try
				{
					dataSocket.Close();
				}
				catch (ObjectDisposedException)
				{
				}
			}
		}

		private static void AcceptCallback(IAsyncResult asyncResult)
		{
			FtpControlStream ftpControlStream = (FtpControlStream)asyncResult.AsyncState;
			Socket dataSocket = ftpControlStream._dataSocket;
			try
			{
				ftpControlStream._dataSocket = dataSocket.EndAccept(asyncResult);
				if (!ftpControlStream.ServerAddress.Equals(((IPEndPoint)ftpControlStream._dataSocket.RemoteEndPoint).Address))
				{
					ftpControlStream._dataSocket.Close();
					throw new WebException("The data connection was made from an address that is different than the address to which the FTP connection was made.", WebExceptionStatus.ProtocolError);
				}
				ftpControlStream.ContinueCommandPipeline();
			}
			catch (Exception ex)
			{
				ftpControlStream.CloseSocket();
				ftpControlStream.InvokeRequestCallback(ex);
			}
			finally
			{
				dataSocket.Close();
			}
		}

		private static void ConnectCallback(IAsyncResult asyncResult)
		{
			FtpControlStream ftpControlStream = (FtpControlStream)asyncResult.AsyncState;
			try
			{
				ftpControlStream._dataSocket.EndConnect(asyncResult);
				ftpControlStream.ContinueCommandPipeline();
			}
			catch (Exception ex)
			{
				ftpControlStream.CloseSocket();
				ftpControlStream.InvokeRequestCallback(ex);
			}
		}

		private static void SSLHandshakeCallback(IAsyncResult asyncResult)
		{
			FtpControlStream ftpControlStream = (FtpControlStream)asyncResult.AsyncState;
			try
			{
				ftpControlStream._tlsStream.EndAuthenticateAsClient(asyncResult);
				ftpControlStream.ContinueCommandPipeline();
			}
			catch (Exception ex)
			{
				ftpControlStream.CloseSocket();
				ftpControlStream.InvokeRequestCallback(ex);
			}
		}

		private CommandStream.PipelineInstruction QueueOrCreateFtpDataStream(ref Stream stream)
		{
			if (this._dataSocket == null)
			{
				throw new InternalException();
			}
			if (this._tlsStream != null)
			{
				stream = new FtpDataStream(this._tlsStream, (FtpWebRequest)this._request, this.IsFtpDataStreamWriteable());
				this._tlsStream = null;
				return CommandStream.PipelineInstruction.GiveStream;
			}
			NetworkStream networkStream = new NetworkStream(this._dataSocket, true);
			if (base.UsingSecureStream)
			{
				FtpWebRequest ftpWebRequest = (FtpWebRequest)this._request;
				TlsStream tlsStream = new TlsStream(networkStream, this._dataSocket, ftpWebRequest.RequestUri.Host, ftpWebRequest.ClientCertificates);
				networkStream = tlsStream;
				if (this._isAsync)
				{
					this._tlsStream = tlsStream;
					tlsStream.BeginAuthenticateAsClient(FtpControlStream.s_SSLHandshakeCallback, this);
					return CommandStream.PipelineInstruction.Pause;
				}
				tlsStream.AuthenticateAsClient();
			}
			stream = new FtpDataStream(networkStream, (FtpWebRequest)this._request, this.IsFtpDataStreamWriteable());
			return CommandStream.PipelineInstruction.GiveStream;
		}

		protected override void ClearState()
		{
			this._contentLength = -1L;
			this._lastModified = DateTime.MinValue;
			this._responseUri = null;
			this._dataHandshakeStarted = false;
			this.StatusCode = FtpStatusCode.Undefined;
			this.StatusLine = null;
			this._dataSocket = null;
			this._passiveEndPoint = null;
			this._tlsStream = null;
			base.ClearState();
		}

		protected override CommandStream.PipelineInstruction PipelineCallback(CommandStream.PipelineEntry entry, ResponseDescription response, bool timeout, ref Stream stream)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Info(this, FormattableStringFactory.Create("Command:{0} Description:{1}", new object[]
				{
					(entry != null) ? entry.Command : null,
					(response != null) ? response.StatusDescription : null
				}), "PipelineCallback");
			}
			if (response == null)
			{
				return CommandStream.PipelineInstruction.Abort;
			}
			FtpStatusCode status = (FtpStatusCode)response.Status;
			if (status != FtpStatusCode.ClosingControl)
			{
				this.StatusCode = status;
				this.StatusLine = response.StatusDescription;
			}
			if (response.InvalidStatusCode)
			{
				throw new WebException("The server returned a status code outside the valid range of 100-599.", WebExceptionStatus.ProtocolError);
			}
			if (this._index == -1)
			{
				if (status == FtpStatusCode.SendUserCommand)
				{
					this._bannerMessage = new StringBuilder();
					this._bannerMessage.Append(this.StatusLine);
					return CommandStream.PipelineInstruction.Advance;
				}
				if (status == FtpStatusCode.ServiceTemporarilyNotAvailable)
				{
					return CommandStream.PipelineInstruction.Reread;
				}
				throw base.GenerateException(status, response.StatusDescription, null);
			}
			else
			{
				if (entry.Command == "OPTS utf8 on\r\n")
				{
					if (response.PositiveCompletion)
					{
						base.Encoding = Encoding.UTF8;
					}
					else
					{
						base.Encoding = Encoding.Default;
					}
					return CommandStream.PipelineInstruction.Advance;
				}
				if (entry.Command.IndexOf("USER") != -1 && status == FtpStatusCode.LoggedInProceed)
				{
					this._loginState = FtpLoginState.LoggedIn;
					this._index++;
				}
				if (response.TransientFailure || response.PermanentFailure)
				{
					if (status == FtpStatusCode.ServiceNotAvailable)
					{
						base.MarkAsRecoverableFailure();
					}
					throw base.GenerateException(status, response.StatusDescription, null);
				}
				if (this._loginState != FtpLoginState.LoggedIn && entry.Command.IndexOf("PASS") != -1)
				{
					if (status != FtpStatusCode.NeedLoginAccount && status != FtpStatusCode.LoggedInProceed)
					{
						throw base.GenerateException(status, response.StatusDescription, null);
					}
					this._loginState = FtpLoginState.LoggedIn;
				}
				if (entry.HasFlag(CommandStream.PipelineEntryFlags.CreateDataConnection) && (response.PositiveCompletion || response.PositiveIntermediate))
				{
					bool flag;
					CommandStream.PipelineInstruction pipelineInstruction = this.QueueOrCreateDataConection(entry, response, timeout, ref stream, out flag);
					if (!flag)
					{
						return pipelineInstruction;
					}
				}
				if (status == FtpStatusCode.OpeningData || status == FtpStatusCode.DataAlreadyOpen)
				{
					if (this._dataSocket == null)
					{
						return CommandStream.PipelineInstruction.Abort;
					}
					if (!entry.HasFlag(CommandStream.PipelineEntryFlags.GiveDataStream))
					{
						this._abortReason = SR.Format("The status response ({0}) is not expected in response to '{1}' command.", status, entry.Command);
						return CommandStream.PipelineInstruction.Abort;
					}
					this.TryUpdateContentLength(response.StatusDescription);
					FtpWebRequest ftpWebRequest = (FtpWebRequest)this._request;
					if (ftpWebRequest.MethodInfo.ShouldParseForResponseUri)
					{
						this.TryUpdateResponseUri(response.StatusDescription, ftpWebRequest);
					}
					return this.QueueOrCreateFtpDataStream(ref stream);
				}
				else
				{
					if (status == FtpStatusCode.LoggedInProceed)
					{
						this._welcomeMessage.Append(this.StatusLine);
					}
					else if (status == FtpStatusCode.ClosingControl)
					{
						this._exitMessage.Append(response.StatusDescription);
						base.CloseSocket();
					}
					else if (status == FtpStatusCode.ServerWantsSecureSession)
					{
						if (!(base.NetworkStream is TlsStream))
						{
							FtpWebRequest ftpWebRequest2 = (FtpWebRequest)this._request;
							TlsStream tlsStream = new TlsStream(base.NetworkStream, base.Socket, ftpWebRequest2.RequestUri.Host, ftpWebRequest2.ClientCertificates);
							if (this._isAsync)
							{
								tlsStream.BeginAuthenticateAsClient(delegate(IAsyncResult ar)
								{
									try
									{
										tlsStream.EndAuthenticateAsClient(ar);
										this.NetworkStream = tlsStream;
										this.ContinueCommandPipeline();
									}
									catch (Exception ex)
									{
										this.CloseSocket();
										this.InvokeRequestCallback(ex);
									}
								}, null);
								return CommandStream.PipelineInstruction.Pause;
							}
							tlsStream.AuthenticateAsClient();
							base.NetworkStream = tlsStream;
						}
					}
					else if (status == FtpStatusCode.FileStatus)
					{
						FtpWebRequest ftpWebRequest3 = (FtpWebRequest)this._request;
						if (entry.Command.StartsWith("SIZE "))
						{
							this._contentLength = this.GetContentLengthFrom213Response(response.StatusDescription);
						}
						else if (entry.Command.StartsWith("MDTM "))
						{
							this._lastModified = this.GetLastModifiedFrom213Response(response.StatusDescription);
						}
					}
					else if (status == FtpStatusCode.PathnameCreated)
					{
						if (entry.Command == "PWD\r\n" && !entry.HasFlag(CommandStream.PipelineEntryFlags.UserCommand))
						{
							this._loginDirectory = this.GetLoginDirectory(response.StatusDescription);
						}
					}
					else if (entry.Command.IndexOf("CWD") != -1)
					{
						this._establishedServerDirectory = this._requestedServerDirectory;
					}
					if (response.PositiveIntermediate || (!base.UsingSecureStream && entry.Command == "AUTH TLS\r\n"))
					{
						return CommandStream.PipelineInstruction.Reread;
					}
					return CommandStream.PipelineInstruction.Advance;
				}
			}
		}

		protected override CommandStream.PipelineEntry[] BuildCommandsList(WebRequest req)
		{
			bool flag = false;
			FtpWebRequest ftpWebRequest = (FtpWebRequest)req;
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Info(this, null, "BuildCommandsList");
			}
			this._responseUri = ftpWebRequest.RequestUri;
			ArrayList arrayList = new ArrayList();
			if (ftpWebRequest.EnableSsl && !base.UsingSecureStream)
			{
				arrayList.Add(new CommandStream.PipelineEntry(this.FormatFtpCommand("AUTH", "TLS")));
				flag = true;
			}
			if (flag)
			{
				this._loginDirectory = null;
				this._establishedServerDirectory = null;
				this._requestedServerDirectory = null;
				this._currentTypeSetting = string.Empty;
				if (this._loginState == FtpLoginState.LoggedIn)
				{
					this._loginState = FtpLoginState.LoggedInButNeedsRelogin;
				}
			}
			if (this._loginState != FtpLoginState.LoggedIn)
			{
				this.Credentials = ftpWebRequest.Credentials.GetCredential(ftpWebRequest.RequestUri, "basic");
				this._welcomeMessage = new StringBuilder();
				this._exitMessage = new StringBuilder();
				string text = string.Empty;
				string text2 = string.Empty;
				if (this.Credentials != null)
				{
					text = this.Credentials.UserName;
					string domain = this.Credentials.Domain;
					if (!string.IsNullOrEmpty(domain))
					{
						text = domain + "\\" + text;
					}
					text2 = this.Credentials.Password;
				}
				if (text.Length == 0 && text2.Length == 0)
				{
					text = "anonymous";
					text2 = "anonymous@";
				}
				arrayList.Add(new CommandStream.PipelineEntry(this.FormatFtpCommand("USER", text)));
				arrayList.Add(new CommandStream.PipelineEntry(this.FormatFtpCommand("PASS", text2), CommandStream.PipelineEntryFlags.DontLogParameter));
				if (ftpWebRequest.EnableSsl && !base.UsingSecureStream)
				{
					arrayList.Add(new CommandStream.PipelineEntry(this.FormatFtpCommand("PBSZ", "0")));
					arrayList.Add(new CommandStream.PipelineEntry(this.FormatFtpCommand("PROT", "P")));
				}
				arrayList.Add(new CommandStream.PipelineEntry(this.FormatFtpCommand("OPTS", "utf8 on")));
				arrayList.Add(new CommandStream.PipelineEntry(this.FormatFtpCommand("PWD", null)));
			}
			FtpControlStream.GetPathOption getPathOption = FtpControlStream.GetPathOption.Normal;
			if (ftpWebRequest.MethodInfo.HasFlag(FtpMethodFlags.DoesNotTakeParameter))
			{
				getPathOption = FtpControlStream.GetPathOption.AssumeNoFilename;
			}
			else if (ftpWebRequest.MethodInfo.HasFlag(FtpMethodFlags.ParameterIsDirectory))
			{
				getPathOption = FtpControlStream.GetPathOption.AssumeFilename;
			}
			string text3;
			string text4;
			string text5;
			FtpControlStream.GetPathInfo(getPathOption, ftpWebRequest.RequestUri, out text3, out text4, out text5);
			if (text5.Length == 0 && ftpWebRequest.MethodInfo.HasFlag(FtpMethodFlags.TakesParameter))
			{
				throw new WebException("The requested URI is invalid for this FTP command.");
			}
			if (this._establishedServerDirectory != null && this._loginDirectory != null && this._establishedServerDirectory != this._loginDirectory)
			{
				arrayList.Add(new CommandStream.PipelineEntry(this.FormatFtpCommand("CWD", this._loginDirectory), CommandStream.PipelineEntryFlags.UserCommand));
				this._requestedServerDirectory = this._loginDirectory;
			}
			if (ftpWebRequest.MethodInfo.HasFlag(FtpMethodFlags.MustChangeWorkingDirectoryToPath) && text4.Length > 0)
			{
				arrayList.Add(new CommandStream.PipelineEntry(this.FormatFtpCommand("CWD", text4), CommandStream.PipelineEntryFlags.UserCommand));
				this._requestedServerDirectory = text4;
			}
			if (!ftpWebRequest.MethodInfo.IsCommandOnly)
			{
				string text6 = (ftpWebRequest.UseBinary ? "I" : "A");
				if (this._currentTypeSetting != text6)
				{
					arrayList.Add(new CommandStream.PipelineEntry(this.FormatFtpCommand("TYPE", text6)));
					this._currentTypeSetting = text6;
				}
				if (ftpWebRequest.UsePassive)
				{
					string text7 = ((base.ServerAddress.AddressFamily == AddressFamily.InterNetwork) ? "PASV" : "EPSV");
					arrayList.Add(new CommandStream.PipelineEntry(this.FormatFtpCommand(text7, null), CommandStream.PipelineEntryFlags.CreateDataConnection));
				}
				else
				{
					string text8 = ((base.ServerAddress.AddressFamily == AddressFamily.InterNetwork) ? "PORT" : "EPRT");
					this.CreateFtpListenerSocket(ftpWebRequest);
					arrayList.Add(new CommandStream.PipelineEntry(this.FormatFtpCommand(text8, this.GetPortCommandLine(ftpWebRequest))));
				}
				if (ftpWebRequest.ContentOffset > 0L)
				{
					arrayList.Add(new CommandStream.PipelineEntry(this.FormatFtpCommand("REST", ftpWebRequest.ContentOffset.ToString(CultureInfo.InvariantCulture))));
				}
			}
			CommandStream.PipelineEntryFlags pipelineEntryFlags = CommandStream.PipelineEntryFlags.UserCommand;
			if (!ftpWebRequest.MethodInfo.IsCommandOnly)
			{
				pipelineEntryFlags |= CommandStream.PipelineEntryFlags.GiveDataStream;
				if (!ftpWebRequest.UsePassive)
				{
					pipelineEntryFlags |= CommandStream.PipelineEntryFlags.CreateDataConnection;
				}
			}
			if (ftpWebRequest.MethodInfo.Operation == FtpOperation.Rename)
			{
				string text9 = ((text4 == string.Empty) ? string.Empty : (text4 + "/"));
				arrayList.Add(new CommandStream.PipelineEntry(this.FormatFtpCommand("RNFR", text9 + text5), pipelineEntryFlags));
				string text10;
				if (!string.IsNullOrEmpty(ftpWebRequest.RenameTo) && ftpWebRequest.RenameTo.StartsWith("/", StringComparison.OrdinalIgnoreCase))
				{
					text10 = ftpWebRequest.RenameTo;
				}
				else
				{
					text10 = text9 + ftpWebRequest.RenameTo;
				}
				arrayList.Add(new CommandStream.PipelineEntry(this.FormatFtpCommand("RNTO", text10), pipelineEntryFlags));
			}
			else if (ftpWebRequest.MethodInfo.HasFlag(FtpMethodFlags.DoesNotTakeParameter))
			{
				arrayList.Add(new CommandStream.PipelineEntry(this.FormatFtpCommand(ftpWebRequest.Method, string.Empty), pipelineEntryFlags));
			}
			else if (ftpWebRequest.MethodInfo.HasFlag(FtpMethodFlags.MustChangeWorkingDirectoryToPath))
			{
				arrayList.Add(new CommandStream.PipelineEntry(this.FormatFtpCommand(ftpWebRequest.Method, text5), pipelineEntryFlags));
			}
			else
			{
				arrayList.Add(new CommandStream.PipelineEntry(this.FormatFtpCommand(ftpWebRequest.Method, text3), pipelineEntryFlags));
			}
			arrayList.Add(new CommandStream.PipelineEntry(this.FormatFtpCommand("QUIT", null)));
			return (CommandStream.PipelineEntry[])arrayList.ToArray(typeof(CommandStream.PipelineEntry));
		}

		private CommandStream.PipelineInstruction QueueOrCreateDataConection(CommandStream.PipelineEntry entry, ResponseDescription response, bool timeout, ref Stream stream, out bool isSocketReady)
		{
			isSocketReady = false;
			if (this._dataHandshakeStarted)
			{
				isSocketReady = true;
				return CommandStream.PipelineInstruction.Pause;
			}
			this._dataHandshakeStarted = true;
			bool flag = false;
			int num = -1;
			if (entry.Command == "PASV\r\n" || entry.Command == "EPSV\r\n")
			{
				if (!response.PositiveCompletion)
				{
					this._abortReason = SR.Format("The server failed the passive mode request with status response ({0}).", response.Status);
					return CommandStream.PipelineInstruction.Abort;
				}
				if (entry.Command == "PASV\r\n")
				{
					num = this.GetPortV4(response.StatusDescription);
				}
				else
				{
					num = this.GetPortV6(response.StatusDescription);
				}
				flag = true;
			}
			if (flag)
			{
				if (num == -1)
				{
					NetEventSource.Fail(this, "'port' not set.", "QueueOrCreateDataConection");
				}
				try
				{
					this._dataSocket = this.CreateFtpDataSocket((FtpWebRequest)this._request, base.Socket);
				}
				catch (ObjectDisposedException)
				{
					throw ExceptionHelper.RequestAbortedException;
				}
				IPEndPoint ipendPoint = new IPEndPoint(((IPEndPoint)base.Socket.LocalEndPoint).Address, 0);
				this._dataSocket.Bind(ipendPoint);
				this._passiveEndPoint = new IPEndPoint(base.ServerAddress, num);
			}
			CommandStream.PipelineInstruction pipelineInstruction;
			if (this._passiveEndPoint != null)
			{
				IPEndPoint passiveEndPoint = this._passiveEndPoint;
				this._passiveEndPoint = null;
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Info(this, "starting Connect()", "QueueOrCreateDataConection");
				}
				if (this._isAsync)
				{
					this._dataSocket.BeginConnect(passiveEndPoint, FtpControlStream.s_connectCallbackDelegate, this);
					pipelineInstruction = CommandStream.PipelineInstruction.Pause;
				}
				else
				{
					this._dataSocket.Connect(passiveEndPoint);
					pipelineInstruction = CommandStream.PipelineInstruction.Advance;
				}
			}
			else
			{
				if (NetEventSource.IsEnabled)
				{
					NetEventSource.Info(this, "starting Accept()", "QueueOrCreateDataConection");
				}
				if (this._isAsync)
				{
					this._dataSocket.BeginAccept(FtpControlStream.s_acceptCallbackDelegate, this);
					pipelineInstruction = CommandStream.PipelineInstruction.Pause;
				}
				else
				{
					Socket dataSocket = this._dataSocket;
					try
					{
						this._dataSocket = this._dataSocket.Accept();
						if (!base.ServerAddress.Equals(((IPEndPoint)this._dataSocket.RemoteEndPoint).Address))
						{
							this._dataSocket.Close();
							throw new WebException("The data connection was made from an address that is different than the address to which the FTP connection was made.", WebExceptionStatus.ProtocolError);
						}
						isSocketReady = true;
						pipelineInstruction = CommandStream.PipelineInstruction.Pause;
					}
					finally
					{
						dataSocket.Close();
					}
				}
			}
			return pipelineInstruction;
		}

		private static void GetPathInfo(FtpControlStream.GetPathOption pathOption, Uri uri, out string path, out string directory, out string filename)
		{
			path = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped);
			int num = path.LastIndexOf('/');
			if (pathOption == FtpControlStream.GetPathOption.AssumeFilename && num != -1 && num == path.Length - 1)
			{
				path = path.Substring(0, path.Length - 1);
				num = path.LastIndexOf('/');
			}
			if (pathOption == FtpControlStream.GetPathOption.AssumeNoFilename)
			{
				directory = path;
				filename = string.Empty;
			}
			else
			{
				directory = path.Substring(0, num + 1);
				filename = path.Substring(num + 1, path.Length - (num + 1));
			}
			if (directory.Length > 1 && directory[directory.Length - 1] == '/')
			{
				directory = directory.Substring(0, directory.Length - 1);
			}
		}

		private string FormatAddress(IPAddress address, int Port)
		{
			byte[] addressBytes = address.GetAddressBytes();
			StringBuilder stringBuilder = new StringBuilder(32);
			foreach (byte b in addressBytes)
			{
				stringBuilder.Append(b);
				stringBuilder.Append(',');
			}
			stringBuilder.Append(Port / 256);
			stringBuilder.Append(',');
			stringBuilder.Append(Port % 256);
			return stringBuilder.ToString();
		}

		private string FormatAddressV6(IPAddress address, int port)
		{
			StringBuilder stringBuilder = new StringBuilder(43);
			string text = address.ToString();
			stringBuilder.Append("|2|");
			stringBuilder.Append(text);
			stringBuilder.Append('|');
			stringBuilder.Append(port.ToString(NumberFormatInfo.InvariantInfo));
			stringBuilder.Append('|');
			return stringBuilder.ToString();
		}

		internal long ContentLength
		{
			get
			{
				return this._contentLength;
			}
		}

		internal DateTime LastModified
		{
			get
			{
				return this._lastModified;
			}
		}

		internal Uri ResponseUri
		{
			get
			{
				return this._responseUri;
			}
		}

		internal string BannerMessage
		{
			get
			{
				if (this._bannerMessage == null)
				{
					return null;
				}
				return this._bannerMessage.ToString();
			}
		}

		internal string WelcomeMessage
		{
			get
			{
				if (this._welcomeMessage == null)
				{
					return null;
				}
				return this._welcomeMessage.ToString();
			}
		}

		internal string ExitMessage
		{
			get
			{
				if (this._exitMessage == null)
				{
					return null;
				}
				return this._exitMessage.ToString();
			}
		}

		private long GetContentLengthFrom213Response(string responseString)
		{
			string[] array = responseString.Split(new char[] { ' ' });
			if (array.Length < 2)
			{
				throw new FormatException(SR.Format("The response string '{0}' has invalid format.", responseString));
			}
			return Convert.ToInt64(array[1], NumberFormatInfo.InvariantInfo);
		}

		private DateTime GetLastModifiedFrom213Response(string str)
		{
			DateTime dateTime = this._lastModified;
			string[] array = str.Split(new char[] { ' ', '.' });
			if (array.Length < 2)
			{
				return dateTime;
			}
			string text = array[1];
			if (text.Length < 14)
			{
				return dateTime;
			}
			int num = Convert.ToInt32(text.Substring(0, 4), NumberFormatInfo.InvariantInfo);
			int num2 = (int)Convert.ToInt16(text.Substring(4, 2), NumberFormatInfo.InvariantInfo);
			int num3 = (int)Convert.ToInt16(text.Substring(6, 2), NumberFormatInfo.InvariantInfo);
			int num4 = (int)Convert.ToInt16(text.Substring(8, 2), NumberFormatInfo.InvariantInfo);
			int num5 = (int)Convert.ToInt16(text.Substring(10, 2), NumberFormatInfo.InvariantInfo);
			int num6 = (int)Convert.ToInt16(text.Substring(12, 2), NumberFormatInfo.InvariantInfo);
			int num7 = 0;
			if (array.Length > 2)
			{
				num7 = (int)Convert.ToInt16(array[2], NumberFormatInfo.InvariantInfo);
			}
			try
			{
				dateTime = new DateTime(num, num2, num3, num4, num5, num6, num7);
				dateTime = dateTime.ToLocalTime();
			}
			catch (ArgumentOutOfRangeException)
			{
			}
			catch (ArgumentException)
			{
			}
			return dateTime;
		}

		private void TryUpdateResponseUri(string str, FtpWebRequest request)
		{
			Uri uri = request.RequestUri;
			int num = str.IndexOf("for ");
			if (num == -1)
			{
				return;
			}
			num += 4;
			int num2 = str.LastIndexOf('(');
			if (num2 == -1)
			{
				num2 = str.Length;
			}
			if (num2 <= num)
			{
				return;
			}
			string text = str.Substring(num, num2 - num);
			text = text.TrimEnd(new char[] { ' ', '.', '\r', '\n' });
			string text2 = text.Replace("%", "%25");
			text2 = text2.Replace("#", "%23");
			string absolutePath = uri.AbsolutePath;
			if (absolutePath.Length > 0 && absolutePath[absolutePath.Length - 1] != '/')
			{
				uri = new UriBuilder(uri)
				{
					Path = absolutePath + "/"
				}.Uri;
			}
			Uri uri2;
			if (!Uri.TryCreate(uri, text2, out uri2))
			{
				throw new FormatException(SR.Format("The server returned the filename ({0}) which is not valid.", text));
			}
			if (!uri.IsBaseOf(uri2) || uri.Segments.Length != uri2.Segments.Length - 1)
			{
				throw new FormatException(SR.Format("The server returned the filename ({0}) which is not valid.", text));
			}
			this._responseUri = uri2;
		}

		private void TryUpdateContentLength(string str)
		{
			int num = str.LastIndexOf("(");
			if (num != -1)
			{
				int num2 = str.IndexOf(" bytes).");
				if (num2 != -1 && num2 > num)
				{
					num++;
					long num3;
					if (long.TryParse(str.Substring(num, num2 - num), NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo, out num3))
					{
						this._contentLength = num3;
					}
				}
			}
		}

		private string GetLoginDirectory(string str)
		{
			int num = str.IndexOf('"');
			int num2 = str.LastIndexOf('"');
			if (num != -1 && num2 != -1 && num != num2)
			{
				return str.Substring(num + 1, num2 - num - 1);
			}
			return string.Empty;
		}

		private int GetPortV4(string responseString)
		{
			string[] array = responseString.Split(new char[] { ' ', '(', ',', ')' });
			if (array.Length <= 7)
			{
				throw new FormatException(SR.Format("The response string '{0}' has invalid format.", responseString));
			}
			int num = array.Length - 1;
			if (!char.IsNumber(array[num], 0))
			{
				num--;
			}
			return (int)Convert.ToByte(array[num--], NumberFormatInfo.InvariantInfo) | ((int)Convert.ToByte(array[num--], NumberFormatInfo.InvariantInfo) << 8);
		}

		private int GetPortV6(string responseString)
		{
			int num = responseString.LastIndexOf("(");
			int num2 = responseString.LastIndexOf(")");
			if (num == -1 || num2 <= num)
			{
				throw new FormatException(SR.Format("The response string '{0}' has invalid format.", responseString));
			}
			string[] array = responseString.Substring(num + 1, num2 - num - 1).Split(new char[] { '|' });
			if (array.Length < 4)
			{
				throw new FormatException(SR.Format("The response string '{0}' has invalid format.", responseString));
			}
			return Convert.ToInt32(array[3], NumberFormatInfo.InvariantInfo);
		}

		private void CreateFtpListenerSocket(FtpWebRequest request)
		{
			IPEndPoint ipendPoint = new IPEndPoint(((IPEndPoint)base.Socket.LocalEndPoint).Address, 0);
			try
			{
				this._dataSocket = this.CreateFtpDataSocket(request, base.Socket);
			}
			catch (ObjectDisposedException)
			{
				throw ExceptionHelper.RequestAbortedException;
			}
			this._dataSocket.Bind(ipendPoint);
			this._dataSocket.Listen(1);
		}

		private string GetPortCommandLine(FtpWebRequest request)
		{
			string text;
			try
			{
				IPEndPoint ipendPoint = (IPEndPoint)this._dataSocket.LocalEndPoint;
				if (base.ServerAddress.AddressFamily == AddressFamily.InterNetwork)
				{
					text = this.FormatAddress(ipendPoint.Address, ipendPoint.Port);
				}
				else
				{
					if (base.ServerAddress.AddressFamily != AddressFamily.InterNetworkV6)
					{
						throw new InternalException();
					}
					text = this.FormatAddressV6(ipendPoint.Address, ipendPoint.Port);
				}
			}
			catch (Exception ex)
			{
				throw base.GenerateException("The underlying connection was closed: The server committed a protocol violation", WebExceptionStatus.ProtocolError, ex);
			}
			return text;
		}

		private string FormatFtpCommand(string command, string parameter)
		{
			StringBuilder stringBuilder = new StringBuilder(command.Length + ((parameter != null) ? parameter.Length : 0) + 3);
			stringBuilder.Append(command);
			if (!string.IsNullOrEmpty(parameter))
			{
				stringBuilder.Append(' ');
				stringBuilder.Append(parameter);
			}
			stringBuilder.Append("\r\n");
			return stringBuilder.ToString();
		}

		protected Socket CreateFtpDataSocket(FtpWebRequest request, Socket templateSocket)
		{
			return new Socket(templateSocket.AddressFamily, templateSocket.SocketType, templateSocket.ProtocolType);
		}

		protected override bool CheckValid(ResponseDescription response, ref int validThrough, ref int completeLength)
		{
			if (NetEventSource.IsEnabled)
			{
				NetEventSource.Info(this, FormattableStringFactory.Create("CheckValid({0})", new object[] { response.StatusBuffer }), "CheckValid");
			}
			if (response.StatusBuffer.Length < 4)
			{
				return true;
			}
			string text = response.StatusBuffer.ToString();
			if (response.Status == -1)
			{
				if (!char.IsDigit(text[0]) || !char.IsDigit(text[1]) || !char.IsDigit(text[2]) || (text[3] != ' ' && text[3] != '-'))
				{
					return false;
				}
				response.StatusCodeString = text.Substring(0, 3);
				response.Status = (int)Convert.ToInt16(response.StatusCodeString, NumberFormatInfo.InvariantInfo);
				if (text[3] == '-')
				{
					response.Multiline = true;
				}
			}
			int num;
			while ((num = text.IndexOf("\r\n", validThrough)) != -1)
			{
				int num2 = validThrough;
				validThrough = num + 2;
				if (!response.Multiline)
				{
					completeLength = validThrough;
					return true;
				}
				if (text.Length > num2 + 4 && text.Substring(num2, 3) == response.StatusCodeString && text[num2 + 3] == ' ')
				{
					completeLength = validThrough;
					return true;
				}
			}
			return true;
		}

		private TriState IsFtpDataStreamWriteable()
		{
			FtpWebRequest ftpWebRequest = this._request as FtpWebRequest;
			if (ftpWebRequest != null)
			{
				if (ftpWebRequest.MethodInfo.IsUpload)
				{
					return TriState.True;
				}
				if (ftpWebRequest.MethodInfo.IsDownload)
				{
					return TriState.False;
				}
			}
			return TriState.Unspecified;
		}

		private Socket _dataSocket;

		private IPEndPoint _passiveEndPoint;

		private TlsStream _tlsStream;

		private StringBuilder _bannerMessage;

		private StringBuilder _welcomeMessage;

		private StringBuilder _exitMessage;

		private WeakReference _credentials;

		private string _currentTypeSetting = string.Empty;

		private long _contentLength = -1L;

		private DateTime _lastModified;

		private bool _dataHandshakeStarted;

		private string _loginDirectory;

		private string _establishedServerDirectory;

		private string _requestedServerDirectory;

		private Uri _responseUri;

		private FtpLoginState _loginState;

		internal FtpStatusCode StatusCode;

		internal string StatusLine;

		private static readonly AsyncCallback s_acceptCallbackDelegate = new AsyncCallback(FtpControlStream.AcceptCallback);

		private static readonly AsyncCallback s_connectCallbackDelegate = new AsyncCallback(FtpControlStream.ConnectCallback);

		private static readonly AsyncCallback s_SSLHandshakeCallback = new AsyncCallback(FtpControlStream.SSLHandshakeCallback);

		private enum GetPathOption
		{
			Normal,
			AssumeFilename,
			AssumeNoFilename
		}
	}
}
