using System;
using System.Globalization;
using System.IO;
using System.Net.Cache;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Mono.Net.Security;
using Mono.Security.Interface;
using Unity;

namespace System.Net
{
	public sealed class FtpWebRequest : WebRequest
	{
		internal FtpWebRequest(Uri uri)
		{
			this.timeout = 100000;
			this.rwTimeout = 300000;
			this.binary = true;
			this.usePassive = true;
			this.method = "RETR";
			this.locker = new object();
			this.dataEncoding = Encoding.UTF8;
			base..ctor();
			this.requestUri = uri;
			this.proxy = GlobalProxySelection.Select;
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		[MonoTODO]
		public X509CertificateCollection ClientCertificates
		{
			get
			{
				throw FtpWebRequest.GetMustImplement();
			}
			set
			{
				throw FtpWebRequest.GetMustImplement();
			}
		}

		[MonoTODO]
		public override string ConnectionGroupName
		{
			get
			{
				throw FtpWebRequest.GetMustImplement();
			}
			set
			{
				throw FtpWebRequest.GetMustImplement();
			}
		}

		public override string ContentType
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override long ContentLength
		{
			get
			{
				return 0L;
			}
			set
			{
			}
		}

		public long ContentOffset
		{
			get
			{
				return this.offset;
			}
			set
			{
				this.CheckRequestStarted();
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.offset = value;
			}
		}

		public override ICredentials Credentials
		{
			get
			{
				return this.credentials;
			}
			set
			{
				this.CheckRequestStarted();
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				if (!(value is NetworkCredential))
				{
					throw new ArgumentException();
				}
				this.credentials = value as NetworkCredential;
			}
		}

		[MonoTODO]
		public new static RequestCachePolicy DefaultCachePolicy
		{
			get
			{
				throw FtpWebRequest.GetMustImplement();
			}
			set
			{
				throw FtpWebRequest.GetMustImplement();
			}
		}

		public bool EnableSsl
		{
			get
			{
				return this.enableSsl;
			}
			set
			{
				this.CheckRequestStarted();
				this.enableSsl = value;
			}
		}

		[MonoTODO]
		public override WebHeaderCollection Headers
		{
			get
			{
				throw FtpWebRequest.GetMustImplement();
			}
			set
			{
				throw FtpWebRequest.GetMustImplement();
			}
		}

		[MonoTODO("We don't support KeepAlive = true")]
		public bool KeepAlive
		{
			get
			{
				return this.keepAlive;
			}
			set
			{
				this.CheckRequestStarted();
			}
		}

		public override string Method
		{
			get
			{
				return this.method;
			}
			set
			{
				this.CheckRequestStarted();
				if (value == null)
				{
					throw new ArgumentNullException("Method string cannot be null");
				}
				if (value.Length == 0 || Array.BinarySearch<string>(FtpWebRequest.supportedCommands, value) < 0)
				{
					throw new ArgumentException("Method not supported", "value");
				}
				this.method = value;
			}
		}

		public override bool PreAuthenticate
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override IWebProxy Proxy
		{
			get
			{
				return this.proxy;
			}
			set
			{
				this.CheckRequestStarted();
				this.proxy = value;
			}
		}

		public int ReadWriteTimeout
		{
			get
			{
				return this.rwTimeout;
			}
			set
			{
				this.CheckRequestStarted();
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.rwTimeout = value;
			}
		}

		public string RenameTo
		{
			get
			{
				return this.renameTo;
			}
			set
			{
				this.CheckRequestStarted();
				if (value == null || value.Length == 0)
				{
					throw new ArgumentException("RenameTo value can't be null or empty", "RenameTo");
				}
				this.renameTo = value;
			}
		}

		public override Uri RequestUri
		{
			get
			{
				return this.requestUri;
			}
		}

		public ServicePoint ServicePoint
		{
			get
			{
				return this.GetServicePoint();
			}
		}

		public bool UsePassive
		{
			get
			{
				return this.usePassive;
			}
			set
			{
				this.CheckRequestStarted();
				this.usePassive = value;
			}
		}

		[MonoTODO]
		public override bool UseDefaultCredentials
		{
			get
			{
				throw FtpWebRequest.GetMustImplement();
			}
			set
			{
				throw FtpWebRequest.GetMustImplement();
			}
		}

		public bool UseBinary
		{
			get
			{
				return this.binary;
			}
			set
			{
				this.CheckRequestStarted();
				this.binary = value;
			}
		}

		public override int Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				this.CheckRequestStarted();
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.timeout = value;
			}
		}

		private string DataType
		{
			get
			{
				if (!this.binary)
				{
					return "A";
				}
				return "I";
			}
		}

		private FtpWebRequest.RequestState State
		{
			get
			{
				object obj = this.locker;
				FtpWebRequest.RequestState requestState;
				lock (obj)
				{
					requestState = this.requestState;
				}
				return requestState;
			}
			set
			{
				object obj = this.locker;
				lock (obj)
				{
					this.CheckIfAborted();
					this.CheckFinalState();
					this.requestState = value;
				}
			}
		}

		public override void Abort()
		{
			object obj = this.locker;
			lock (obj)
			{
				if (this.State == FtpWebRequest.RequestState.TransferInProgress)
				{
					this.SendCommand(false, "ABOR", Array.Empty<string>());
				}
				if (!this.InFinalState())
				{
					this.State = FtpWebRequest.RequestState.Aborted;
					this.ftpResponse = new FtpWebResponse(this, this.requestUri, this.method, FtpStatusCode.FileActionAborted, "Aborted by request");
				}
			}
		}

		public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
		{
			if (this.asyncResult != null && !this.asyncResult.IsCompleted)
			{
				throw new InvalidOperationException("Cannot re-call BeginGetRequestStream/BeginGetResponse while a previous call is still in progress");
			}
			this.CheckIfAborted();
			this.asyncResult = new FtpAsyncResult(callback, state);
			object obj = this.locker;
			lock (obj)
			{
				if (this.InFinalState())
				{
					this.asyncResult.SetCompleted(true, this.ftpResponse);
				}
				else
				{
					if (this.State == FtpWebRequest.RequestState.Before)
					{
						this.State = FtpWebRequest.RequestState.Scheduled;
					}
					new Thread(new ThreadStart(this.ProcessRequest))
					{
						IsBackground = true
					}.Start();
				}
			}
			return this.asyncResult;
		}

		public override WebResponse EndGetResponse(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("AsyncResult cannot be null!");
			}
			if (!(asyncResult is FtpAsyncResult) || asyncResult != this.asyncResult)
			{
				throw new ArgumentException("AsyncResult is from another request!");
			}
			FtpAsyncResult ftpAsyncResult = (FtpAsyncResult)asyncResult;
			if (!ftpAsyncResult.WaitUntilComplete(this.timeout, false))
			{
				this.Abort();
				throw new WebException("Transfer timed out.", WebExceptionStatus.Timeout);
			}
			this.CheckIfAborted();
			asyncResult = null;
			if (ftpAsyncResult.GotException)
			{
				throw ftpAsyncResult.Exception;
			}
			return ftpAsyncResult.Response;
		}

		public override WebResponse GetResponse()
		{
			IAsyncResult asyncResult = this.BeginGetResponse(null, null);
			return this.EndGetResponse(asyncResult);
		}

		public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
		{
			if (this.method != "STOR" && this.method != "STOU" && this.method != "APPE")
			{
				throw new ProtocolViolationException();
			}
			object obj = this.locker;
			lock (obj)
			{
				this.CheckIfAborted();
				if (this.State != FtpWebRequest.RequestState.Before)
				{
					throw new InvalidOperationException("Cannot re-call BeginGetRequestStream/BeginGetResponse while a previous call is still in progress");
				}
				this.State = FtpWebRequest.RequestState.Scheduled;
			}
			this.asyncResult = new FtpAsyncResult(callback, state);
			new Thread(new ThreadStart(this.ProcessRequest))
			{
				IsBackground = true
			}.Start();
			return this.asyncResult;
		}

		public override Stream EndGetRequestStream(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (!(asyncResult is FtpAsyncResult))
			{
				throw new ArgumentException("asyncResult");
			}
			if (this.State == FtpWebRequest.RequestState.Aborted)
			{
				throw new WebException("Request aborted", WebExceptionStatus.RequestCanceled);
			}
			if (asyncResult != this.asyncResult)
			{
				throw new ArgumentException("AsyncResult is from another request!");
			}
			FtpAsyncResult ftpAsyncResult = (FtpAsyncResult)asyncResult;
			if (!ftpAsyncResult.WaitUntilComplete(this.timeout, false))
			{
				this.Abort();
				throw new WebException("Request timed out");
			}
			if (ftpAsyncResult.GotException)
			{
				throw ftpAsyncResult.Exception;
			}
			return ftpAsyncResult.Stream;
		}

		public override Stream GetRequestStream()
		{
			IAsyncResult asyncResult = this.BeginGetRequestStream(null, null);
			return this.EndGetRequestStream(asyncResult);
		}

		private ServicePoint GetServicePoint()
		{
			if (this.servicePoint == null)
			{
				this.servicePoint = ServicePointManager.FindServicePoint(this.requestUri, this.proxy);
			}
			return this.servicePoint;
		}

		private void ResolveHost()
		{
			this.CheckIfAborted();
			this.hostEntry = this.GetServicePoint().HostEntry;
			if (this.hostEntry == null)
			{
				this.ftpResponse.UpdateStatus(new FtpStatus(FtpStatusCode.ActionAbortedLocalProcessingError, "Cannot resolve server name"));
				throw new WebException("The remote server name could not be resolved: " + this.requestUri, null, WebExceptionStatus.NameResolutionFailure, this.ftpResponse);
			}
		}

		private void ProcessRequest()
		{
			if (this.State == FtpWebRequest.RequestState.Scheduled)
			{
				this.ftpResponse = new FtpWebResponse(this, this.requestUri, this.method, this.keepAlive);
				try
				{
					this.ProcessMethod();
					this.asyncResult.SetCompleted(false, this.ftpResponse);
					return;
				}
				catch (Exception ex)
				{
					if (!this.GetServicePoint().UsesProxy)
					{
						this.State = FtpWebRequest.RequestState.Error;
					}
					this.SetCompleteWithError(ex);
					return;
				}
			}
			if (this.InProgress())
			{
				FtpStatus responseStatus = this.GetResponseStatus();
				this.ftpResponse.UpdateStatus(responseStatus);
				if (this.ftpResponse.IsFinal())
				{
					this.State = FtpWebRequest.RequestState.Finished;
				}
			}
			this.asyncResult.SetCompleted(false, this.ftpResponse);
		}

		private void SetType()
		{
			if (this.binary)
			{
				FtpStatus ftpStatus = this.SendCommand("TYPE", new string[] { this.DataType });
				if (ftpStatus.StatusCode < FtpStatusCode.CommandOK || ftpStatus.StatusCode >= (FtpStatusCode)300)
				{
					throw this.CreateExceptionFromResponse(ftpStatus);
				}
			}
		}

		private string GetRemoteFolderPath(Uri uri)
		{
			string text = Uri.UnescapeDataString(uri.LocalPath);
			string text2;
			if (this.initial_path == null || this.initial_path == "/")
			{
				text2 = text;
			}
			else
			{
				if (text[0] == '/')
				{
					text = text.Substring(1);
				}
				text2 = new Uri(new UriBuilder
				{
					Scheme = "ftp",
					Host = "dummy-host",
					Path = this.initial_path
				}.Uri, text).LocalPath;
			}
			int num = text2.LastIndexOf('/');
			if (num == -1)
			{
				return null;
			}
			return text2.Substring(0, num + 1);
		}

		private void CWDAndSetFileName(Uri uri)
		{
			string remoteFolderPath = this.GetRemoteFolderPath(uri);
			if (remoteFolderPath != null)
			{
				FtpStatus ftpStatus = this.SendCommand("CWD", new string[] { remoteFolderPath });
				if (ftpStatus.StatusCode < FtpStatusCode.CommandOK || ftpStatus.StatusCode >= (FtpStatusCode)300)
				{
					throw this.CreateExceptionFromResponse(ftpStatus);
				}
				int num = uri.LocalPath.LastIndexOf('/');
				if (num >= 0)
				{
					this.file_name = Uri.UnescapeDataString(uri.LocalPath.Substring(num + 1));
				}
			}
		}

		private void ProcessMethod()
		{
			if (!this.GetServicePoint().UsesProxy)
			{
				this.State = FtpWebRequest.RequestState.Connecting;
				this.ResolveHost();
				this.OpenControlConnection();
				this.CWDAndSetFileName(this.requestUri);
				this.SetType();
				string text = this.method;
				uint num = global::<PrivateImplementationDetails>.ComputeStringHash(text);
				if (num <= 1636987420U)
				{
					if (num <= 172932033U)
					{
						if (num != 61167622U)
						{
							if (num != 111500479U)
							{
								if (num != 172932033U)
								{
									goto IL_0248;
								}
								if (!(text == "LIST"))
								{
									goto IL_0248;
								}
							}
							else
							{
								if (!(text == "STOR"))
								{
									goto IL_0248;
								}
								goto IL_0238;
							}
						}
						else
						{
							if (!(text == "STOU"))
							{
								goto IL_0248;
							}
							goto IL_0238;
						}
					}
					else if (num != 540800083U)
					{
						if (num != 1414193175U)
						{
							if (num != 1636987420U)
							{
								goto IL_0248;
							}
							if (!(text == "SIZE"))
							{
								goto IL_0248;
							}
							goto IL_0240;
						}
						else
						{
							if (!(text == "MKD"))
							{
								goto IL_0248;
							}
							goto IL_0240;
						}
					}
					else
					{
						if (!(text == "RENAME"))
						{
							goto IL_0248;
						}
						goto IL_0240;
					}
				}
				else if (num <= 2586094756U)
				{
					if (num != 2190452587U)
					{
						if (num != 2192893693U)
						{
							if (num != 2586094756U)
							{
								goto IL_0248;
							}
							if (!(text == "PWD"))
							{
								goto IL_0248;
							}
							goto IL_0240;
						}
						else
						{
							if (!(text == "DELE"))
							{
								goto IL_0248;
							}
							goto IL_0240;
						}
					}
					else
					{
						if (!(text == "APPE"))
						{
							goto IL_0248;
						}
						goto IL_0238;
					}
				}
				else if (num != 3129138359U)
				{
					if (num != 3960558266U)
					{
						if (num != 4117911256U)
						{
							goto IL_0248;
						}
						if (!(text == "NLST"))
						{
							goto IL_0248;
						}
					}
					else if (!(text == "RETR"))
					{
						goto IL_0248;
					}
				}
				else
				{
					if (!(text == "MDTM"))
					{
						goto IL_0248;
					}
					goto IL_0240;
				}
				this.DownloadData();
				goto IL_025E;
				IL_0238:
				this.UploadData();
				goto IL_025E;
				IL_0240:
				this.ProcessSimpleMethod();
				goto IL_025E;
				IL_0248:
				throw new Exception(string.Format("Support for command {0} not implemented yet", this.method));
				IL_025E:
				this.CheckIfAborted();
				return;
			}
			if (this.method != "RETR")
			{
				throw new NotSupportedException("FTP+proxy only supports RETR");
			}
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this.proxy.GetProxy(this.requestUri));
			httpWebRequest.Address = this.requestUri;
			this.requestState = FtpWebRequest.RequestState.Finished;
			WebResponse response = httpWebRequest.GetResponse();
			this.ftpResponse.Stream = new FtpDataStream(this, response.GetResponseStream(), true);
			this.ftpResponse.StatusCode = FtpStatusCode.CommandOK;
		}

		private void CloseControlConnection()
		{
			if (this.controlStream != null)
			{
				this.SendCommand("QUIT", Array.Empty<string>());
				this.controlStream.Close();
				this.controlStream = null;
			}
		}

		internal void CloseDataConnection()
		{
			if (this.origDataStream != null)
			{
				this.origDataStream.Close();
				this.origDataStream = null;
			}
		}

		private void CloseConnection()
		{
			this.CloseControlConnection();
			this.CloseDataConnection();
		}

		private void ProcessSimpleMethod()
		{
			this.State = FtpWebRequest.RequestState.TransferInProgress;
			if (this.method == "PWD")
			{
				this.method = "PWD";
			}
			if (this.method == "RENAME")
			{
				this.method = "RNFR";
			}
			FtpStatus ftpStatus = this.SendCommand(this.method, new string[] { this.file_name });
			this.ftpResponse.Stream = Stream.Null;
			string statusDescription = ftpStatus.StatusDescription;
			string text = this.method;
			if (!(text == "SIZE"))
			{
				if (!(text == "MDTM"))
				{
					if (!(text == "MKD"))
					{
						if (!(text == "CWD"))
						{
							if (!(text == "RNFR"))
							{
								if (text == "DELE")
								{
									if (ftpStatus.StatusCode != FtpStatusCode.FileActionOK)
									{
										throw this.CreateExceptionFromResponse(ftpStatus);
									}
								}
							}
							else
							{
								this.method = "RENAME";
								if (ftpStatus.StatusCode != FtpStatusCode.FileCommandPending)
								{
									throw this.CreateExceptionFromResponse(ftpStatus);
								}
								ftpStatus = this.SendCommand("RNTO", new string[] { (this.renameTo != null) ? this.renameTo : string.Empty });
								if (ftpStatus.StatusCode != FtpStatusCode.FileActionOK)
								{
									throw this.CreateExceptionFromResponse(ftpStatus);
								}
							}
						}
						else
						{
							this.method = "PWD";
							if (ftpStatus.StatusCode != FtpStatusCode.FileActionOK)
							{
								throw this.CreateExceptionFromResponse(ftpStatus);
							}
							ftpStatus = this.SendCommand(this.method, Array.Empty<string>());
							if (ftpStatus.StatusCode != FtpStatusCode.PathnameCreated)
							{
								throw this.CreateExceptionFromResponse(ftpStatus);
							}
						}
					}
					else if (ftpStatus.StatusCode != FtpStatusCode.PathnameCreated)
					{
						throw this.CreateExceptionFromResponse(ftpStatus);
					}
				}
				else
				{
					if (ftpStatus.StatusCode != FtpStatusCode.FileStatus)
					{
						throw this.CreateExceptionFromResponse(ftpStatus);
					}
					this.ftpResponse.LastModified = DateTime.ParseExact(statusDescription.Substring(4), "yyyyMMddHHmmss", null);
				}
			}
			else
			{
				if (ftpStatus.StatusCode != FtpStatusCode.FileStatus)
				{
					throw this.CreateExceptionFromResponse(ftpStatus);
				}
				int num = 4;
				int num2 = 0;
				while (num < statusDescription.Length && char.IsDigit(statusDescription[num]))
				{
					num++;
					num2++;
				}
				if (num2 == 0)
				{
					throw new WebException("Bad format for server response in " + this.method);
				}
				long num3;
				if (!long.TryParse(statusDescription.Substring(4, num2), out num3))
				{
					throw new WebException("Bad format for server response in " + this.method);
				}
				this.ftpResponse.contentLength = num3;
			}
			this.State = FtpWebRequest.RequestState.Finished;
		}

		private void UploadData()
		{
			this.State = FtpWebRequest.RequestState.OpeningData;
			this.OpenDataConnection();
			this.State = FtpWebRequest.RequestState.TransferInProgress;
			this.requestStream = new FtpDataStream(this, this.dataStream, false);
			this.asyncResult.Stream = this.requestStream;
		}

		private void DownloadData()
		{
			this.State = FtpWebRequest.RequestState.OpeningData;
			this.OpenDataConnection();
			this.State = FtpWebRequest.RequestState.TransferInProgress;
			this.ftpResponse.Stream = new FtpDataStream(this, this.dataStream, true);
		}

		private void CheckRequestStarted()
		{
			if (this.State != FtpWebRequest.RequestState.Before)
			{
				throw new InvalidOperationException("There is a request currently in progress");
			}
		}

		private void OpenControlConnection()
		{
			Exception ex = null;
			Socket socket = null;
			foreach (IPAddress ipaddress in this.hostEntry.AddressList)
			{
				socket = new Socket(ipaddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				this.remoteEndPoint = new IPEndPoint(ipaddress, this.requestUri.Port);
				if (!this.ServicePoint.CallEndPointDelegate(socket, this.remoteEndPoint))
				{
					socket.Close();
					socket = null;
				}
				else
				{
					try
					{
						socket.Connect(this.remoteEndPoint);
						this.localEndPoint = (IPEndPoint)socket.LocalEndPoint;
						break;
					}
					catch (SocketException ex)
					{
						socket.Close();
						socket = null;
					}
				}
			}
			if (socket == null)
			{
				throw new WebException("Unable to connect to remote server", ex, WebExceptionStatus.UnknownError, this.ftpResponse);
			}
			this.controlStream = new NetworkStream(socket);
			this.controlReader = new StreamReader(this.controlStream, Encoding.ASCII);
			this.State = FtpWebRequest.RequestState.Authenticating;
			this.Authenticate();
			FtpStatus ftpStatus = this.SendCommand("OPTS", new string[] { "utf8", "on" });
			if (ftpStatus.StatusCode < FtpStatusCode.CommandOK || ftpStatus.StatusCode > (FtpStatusCode)300)
			{
				this.dataEncoding = Encoding.Default;
			}
			else
			{
				this.dataEncoding = Encoding.UTF8;
			}
			ftpStatus = this.SendCommand("PWD", Array.Empty<string>());
			this.initial_path = FtpWebRequest.GetInitialPath(ftpStatus);
		}

		private static string GetInitialPath(FtpStatus status)
		{
			int statusCode = (int)status.StatusCode;
			if (statusCode < 200 || statusCode > 300 || status.StatusDescription.Length <= 4)
			{
				throw new WebException("Error getting current directory: " + status.StatusDescription, null, WebExceptionStatus.UnknownError, null);
			}
			string text = status.StatusDescription.Substring(4);
			if (text[0] == '"')
			{
				int num = text.IndexOf('"', 1);
				if (num == -1)
				{
					throw new WebException("Error getting current directory: PWD -> " + status.StatusDescription, null, WebExceptionStatus.UnknownError, null);
				}
				text = text.Substring(1, num - 1);
			}
			if (!text.EndsWith("/"))
			{
				text += "/";
			}
			return text;
		}

		private Socket SetupPassiveConnection(string statusDescription, bool ipv6)
		{
			if (statusDescription.Length < 4)
			{
				throw new WebException("Cannot open passive data connection");
			}
			int num = (ipv6 ? this.GetPortV6(statusDescription) : this.GetPortV4(statusDescription));
			if (num < 0 || num > 65535)
			{
				throw new WebException("Cannot open passive data connection");
			}
			IPEndPoint ipendPoint = new IPEndPoint(this.remoteEndPoint.Address, num);
			Socket socket = new Socket(ipendPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			try
			{
				socket.Connect(ipendPoint);
			}
			catch (SocketException)
			{
				socket.Close();
				throw new WebException("Cannot open passive data connection");
			}
			return socket;
		}

		private int GetPortV4(string responseString)
		{
			string[] array = responseString.Split(new char[] { ' ', '(', ',', ')' });
			if (array.Length <= 7)
			{
				throw new FormatException(global::SR.GetString("The response string '{0}' has invalid format.", new object[] { responseString }));
			}
			int num = array.Length - 1;
			if (array[num] == "" || !char.IsNumber(array[num], 0))
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
				throw new FormatException(global::SR.GetString("The response string '{0}' has invalid format.", new object[] { responseString }));
			}
			string[] array = responseString.Substring(num + 1, num2 - num - 1).Split(new char[] { '|' });
			if (array.Length < 4)
			{
				throw new FormatException(global::SR.GetString("The response string '{0}' has invalid format.", new object[] { responseString }));
			}
			return Convert.ToInt32(array[3], NumberFormatInfo.InvariantInfo);
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

		private Exception CreateExceptionFromResponse(FtpStatus status)
		{
			FtpWebResponse ftpWebResponse = new FtpWebResponse(this, this.requestUri, this.method, status);
			return new WebException("Server returned an error: " + status.StatusDescription, null, WebExceptionStatus.ProtocolError, ftpWebResponse);
		}

		internal void SetTransferCompleted()
		{
			if (this.InFinalState())
			{
				return;
			}
			this.State = FtpWebRequest.RequestState.Finished;
			FtpStatus responseStatus = this.GetResponseStatus();
			this.ftpResponse.UpdateStatus(responseStatus);
			if (!this.keepAlive)
			{
				this.CloseConnection();
			}
		}

		internal void OperationCompleted()
		{
			if (!this.keepAlive)
			{
				this.CloseConnection();
			}
		}

		private void SetCompleteWithError(Exception exc)
		{
			if (this.asyncResult != null)
			{
				this.asyncResult.SetCompleted(false, exc);
			}
		}

		private Socket InitDataConnection()
		{
			bool flag = this.remoteEndPoint.AddressFamily == AddressFamily.InterNetworkV6;
			if (this.usePassive)
			{
				FtpStatus ftpStatus = this.SendCommand(flag ? "EPSV" : "PASV", Array.Empty<string>());
				if (ftpStatus.StatusCode != (flag ? ((FtpStatusCode)229) : FtpStatusCode.EnteringPassive))
				{
					throw this.CreateExceptionFromResponse(ftpStatus);
				}
				return this.SetupPassiveConnection(ftpStatus.StatusDescription, flag);
			}
			else
			{
				Socket socket = new Socket(this.remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				try
				{
					socket.Bind(new IPEndPoint(this.localEndPoint.Address, 0));
					socket.Listen(1);
				}
				catch (SocketException ex)
				{
					socket.Close();
					throw new WebException("Couldn't open listening socket on client", ex);
				}
				IPEndPoint ipendPoint = (IPEndPoint)socket.LocalEndPoint;
				string text = (flag ? this.FormatAddressV6(ipendPoint.Address, ipendPoint.Port) : this.FormatAddress(ipendPoint.Address, ipendPoint.Port));
				FtpStatus ftpStatus = this.SendCommand(flag ? "EPRT" : "PORT", new string[] { text });
				if (ftpStatus.StatusCode != FtpStatusCode.CommandOK)
				{
					socket.Close();
					throw this.CreateExceptionFromResponse(ftpStatus);
				}
				return socket;
			}
		}

		private void OpenDataConnection()
		{
			Socket socket = this.InitDataConnection();
			FtpStatus ftpStatus;
			if (this.offset > 0L)
			{
				ftpStatus = this.SendCommand("REST", new string[] { this.offset.ToString() });
				if (ftpStatus.StatusCode != FtpStatusCode.FileCommandPending)
				{
					throw this.CreateExceptionFromResponse(ftpStatus);
				}
			}
			if (this.method != "NLST" && this.method != "LIST" && this.method != "STOU")
			{
				ftpStatus = this.SendCommand(this.method, new string[] { this.file_name });
			}
			else
			{
				ftpStatus = this.SendCommand(this.method, Array.Empty<string>());
			}
			if (ftpStatus.StatusCode != FtpStatusCode.OpeningData && ftpStatus.StatusCode != FtpStatusCode.DataAlreadyOpen)
			{
				throw this.CreateExceptionFromResponse(ftpStatus);
			}
			if (this.usePassive)
			{
				this.origDataStream = new NetworkStream(socket, true);
				this.dataStream = this.origDataStream;
				if (this.EnableSsl)
				{
					this.ChangeToSSLSocket(ref this.dataStream);
				}
			}
			else
			{
				Socket socket2 = null;
				try
				{
					socket2 = socket.Accept();
				}
				catch (SocketException)
				{
					socket.Close();
					if (socket2 != null)
					{
						socket2.Close();
					}
					throw new ProtocolViolationException("Server commited a protocol violation.");
				}
				socket.Close();
				this.origDataStream = new NetworkStream(socket2, true);
				this.dataStream = this.origDataStream;
				if (this.EnableSsl)
				{
					this.ChangeToSSLSocket(ref this.dataStream);
				}
			}
			this.ftpResponse.UpdateStatus(ftpStatus);
		}

		private void Authenticate()
		{
			string text = null;
			string text2 = null;
			string text3 = null;
			if (this.credentials != null)
			{
				text = this.credentials.UserName;
				text2 = this.credentials.Password;
				text3 = this.credentials.Domain;
			}
			if (text == null)
			{
				text = "anonymous";
			}
			if (text2 == null)
			{
				text2 = "@anonymous";
			}
			if (!string.IsNullOrEmpty(text3))
			{
				text = text3 + "\\" + text;
			}
			FtpStatus ftpStatus = this.GetResponseStatus();
			this.ftpResponse.BannerMessage = ftpStatus.StatusDescription;
			if (this.EnableSsl)
			{
				this.InitiateSecureConnection(ref this.controlStream);
				this.controlReader = new StreamReader(this.controlStream, Encoding.ASCII);
				ftpStatus = this.SendCommand("PBSZ", new string[] { "0" });
				int num = (int)ftpStatus.StatusCode;
				if (num < 200 || num >= 300)
				{
					throw this.CreateExceptionFromResponse(ftpStatus);
				}
				ftpStatus = this.SendCommand("PROT", new string[] { "P" });
				num = (int)ftpStatus.StatusCode;
				if (num < 200 || num >= 300)
				{
					throw this.CreateExceptionFromResponse(ftpStatus);
				}
				ftpStatus = new FtpStatus(FtpStatusCode.SendUserCommand, "");
			}
			if (ftpStatus.StatusCode != FtpStatusCode.SendUserCommand)
			{
				throw this.CreateExceptionFromResponse(ftpStatus);
			}
			ftpStatus = this.SendCommand("USER", new string[] { text });
			FtpStatusCode statusCode = ftpStatus.StatusCode;
			if (statusCode != FtpStatusCode.LoggedInProceed)
			{
				if (statusCode != FtpStatusCode.SendPasswordCommand)
				{
					throw this.CreateExceptionFromResponse(ftpStatus);
				}
				ftpStatus = this.SendCommand("PASS", new string[] { text2 });
				if (ftpStatus.StatusCode != FtpStatusCode.LoggedInProceed)
				{
					throw this.CreateExceptionFromResponse(ftpStatus);
				}
			}
			this.ftpResponse.WelcomeMessage = ftpStatus.StatusDescription;
			this.ftpResponse.UpdateStatus(ftpStatus);
		}

		private FtpStatus SendCommand(string command, params string[] parameters)
		{
			return this.SendCommand(true, command, parameters);
		}

		private FtpStatus SendCommand(bool waitResponse, string command, params string[] parameters)
		{
			string text = command;
			if (parameters.Length != 0)
			{
				text = text + " " + string.Join(" ", parameters);
			}
			text += "\r\n";
			byte[] bytes = this.dataEncoding.GetBytes(text);
			try
			{
				this.controlStream.Write(bytes, 0, bytes.Length);
			}
			catch (IOException)
			{
				return new FtpStatus(FtpStatusCode.ServiceNotAvailable, "Write failed");
			}
			if (!waitResponse)
			{
				return null;
			}
			FtpStatus responseStatus = this.GetResponseStatus();
			if (this.ftpResponse != null)
			{
				this.ftpResponse.UpdateStatus(responseStatus);
			}
			return responseStatus;
		}

		internal static FtpStatus ServiceNotAvailable()
		{
			return new FtpStatus(FtpStatusCode.ServiceNotAvailable, global::Locale.GetText("Invalid response from server"));
		}

		internal FtpStatus GetResponseStatus()
		{
			string text = null;
			try
			{
				text = this.controlReader.ReadLine();
			}
			catch (IOException)
			{
			}
			if (text == null || text.Length < 3)
			{
				return FtpWebRequest.ServiceNotAvailable();
			}
			int num;
			if (!int.TryParse(text.Substring(0, 3), out num))
			{
				return FtpWebRequest.ServiceNotAvailable();
			}
			if (text.Length > 3 && text[3] == '-')
			{
				string text2 = null;
				string text3 = num.ToString() + " ";
				for (;;)
				{
					text2 = null;
					try
					{
						text2 = this.controlReader.ReadLine();
					}
					catch (IOException)
					{
					}
					if (text2 == null)
					{
						break;
					}
					text = text + Environment.NewLine + text2;
					if (text2.StartsWith(text3, StringComparison.Ordinal))
					{
						goto IL_0097;
					}
				}
				return FtpWebRequest.ServiceNotAvailable();
			}
			IL_0097:
			return new FtpStatus((FtpStatusCode)num, text);
		}

		private void InitiateSecureConnection(ref Stream stream)
		{
			FtpStatus ftpStatus = this.SendCommand("AUTH", new string[] { "TLS" });
			if (ftpStatus.StatusCode != FtpStatusCode.ServerWantsSecureSession)
			{
				throw this.CreateExceptionFromResponse(ftpStatus);
			}
			this.ChangeToSSLSocket(ref stream);
		}

		internal bool ChangeToSSLSocket(ref Stream stream)
		{
			MonoTlsProvider providerInternal = Mono.Net.Security.MonoTlsProviderFactory.GetProviderInternal();
			MonoTlsSettings monoTlsSettings = MonoTlsSettings.CopyDefaultSettings();
			monoTlsSettings.UseServicePointManagerCallback = new bool?(true);
			IMonoSslStream monoSslStream = providerInternal.CreateSslStream(stream, true, monoTlsSettings);
			monoSslStream.AuthenticateAsClient(this.requestUri.Host, null, SslProtocols.Default, false);
			stream = monoSslStream.AuthenticatedStream;
			return true;
		}

		private bool InFinalState()
		{
			return this.State == FtpWebRequest.RequestState.Aborted || this.State == FtpWebRequest.RequestState.Error || this.State == FtpWebRequest.RequestState.Finished;
		}

		private bool InProgress()
		{
			return this.State != FtpWebRequest.RequestState.Before && !this.InFinalState();
		}

		internal void CheckIfAborted()
		{
			if (this.State == FtpWebRequest.RequestState.Aborted)
			{
				throw new WebException("Request aborted", WebExceptionStatus.RequestCanceled);
			}
		}

		private void CheckFinalState()
		{
			if (this.InFinalState())
			{
				throw new InvalidOperationException("Cannot change final state");
			}
		}

		internal FtpWebRequest()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private Uri requestUri;

		private string file_name;

		private ServicePoint servicePoint;

		private Stream origDataStream;

		private Stream dataStream;

		private Stream controlStream;

		private StreamReader controlReader;

		private NetworkCredential credentials;

		private IPHostEntry hostEntry;

		private IPEndPoint localEndPoint;

		private IPEndPoint remoteEndPoint;

		private IWebProxy proxy;

		private int timeout;

		private int rwTimeout;

		private long offset;

		private bool binary;

		private bool enableSsl;

		private bool usePassive;

		private bool keepAlive;

		private string method;

		private string renameTo;

		private object locker;

		private FtpWebRequest.RequestState requestState;

		private FtpAsyncResult asyncResult;

		private FtpWebResponse ftpResponse;

		private Stream requestStream;

		private string initial_path;

		private const string ChangeDir = "CWD";

		private const string UserCommand = "USER";

		private const string PasswordCommand = "PASS";

		private const string TypeCommand = "TYPE";

		private const string PassiveCommand = "PASV";

		private const string ExtendedPassiveCommand = "EPSV";

		private const string PortCommand = "PORT";

		private const string ExtendedPortCommand = "EPRT";

		private const string AbortCommand = "ABOR";

		private const string AuthCommand = "AUTH";

		private const string RestCommand = "REST";

		private const string RenameFromCommand = "RNFR";

		private const string RenameToCommand = "RNTO";

		private const string QuitCommand = "QUIT";

		private const string EOL = "\r\n";

		private static readonly string[] supportedCommands = new string[]
		{
			"APPE", "DELE", "LIST", "MDTM", "MKD", "NLST", "PWD", "RENAME", "RETR", "RMD",
			"SIZE", "STOR", "STOU"
		};

		private Encoding dataEncoding;

		private enum RequestState
		{
			Before,
			Scheduled,
			Connecting,
			Authenticating,
			OpeningData,
			TransferInProgress,
			Finished,
			Aborted,
			Error
		}
	}
}
