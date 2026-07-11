using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Cache;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace System.Net
{
	public sealed class FtpWebRequest : WebRequest
	{
		internal FtpWebRequest(global::System.Uri uri)
		{
			this.requestUri = uri;
			this.proxy = GlobalProxySelection.Select;
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		[global::System.MonoTODO]
		public global::System.Security.Cryptography.X509Certificates.X509CertificateCollection ClientCertificates
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

		[global::System.MonoTODO]
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

		[global::System.MonoTODO]
		public new static global::System.Net.Cache.RequestCachePolicy DefaultCachePolicy
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

		[global::System.MonoTODO]
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

		[global::System.MonoTODO("We don't support KeepAlive = true")]
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
				if (value == null)
				{
					throw new ArgumentNullException();
				}
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

		public override global::System.Uri RequestUri
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

		[global::System.MonoTODO]
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
				return (!this.binary) ? "A" : "I";
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
					this.SendCommand(false, "ABOR", new string[0]);
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
					Thread thread = new Thread(new ThreadStart(this.ProcessRequest));
					thread.Start();
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
			Thread thread = new Thread(new ThreadStart(this.ProcessRequest));
			thread.Start();
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
				}
				catch (Exception ex)
				{
					this.State = FtpWebRequest.RequestState.Error;
					this.SetCompleteWithError(ex);
				}
			}
			else
			{
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

		private string GetRemoteFolderPath(global::System.Uri uri)
		{
			string text = global::System.Uri.UnescapeDataString(uri.LocalPath);
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
				global::System.Uri uri2 = new global::System.Uri("ftp://dummy-host" + this.initial_path);
				text2 = new global::System.Uri(uri2, text).LocalPath;
			}
			int num = text2.LastIndexOf('/');
			if (num == -1)
			{
				return null;
			}
			return text2.Substring(0, num + 1);
		}

		private void CWDAndSetFileName(global::System.Uri uri)
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
					this.file_name = global::System.Uri.UnescapeDataString(uri.LocalPath.Substring(num + 1));
				}
			}
		}

		private void ProcessMethod()
		{
			this.State = FtpWebRequest.RequestState.Connecting;
			this.ResolveHost();
			this.OpenControlConnection();
			this.CWDAndSetFileName(this.requestUri);
			this.SetType();
			string text = this.method;
			if (text != null)
			{
				if (FtpWebRequest.<>f__switch$mapA == null)
				{
					FtpWebRequest.<>f__switch$mapA = new Dictionary<string, int>(12)
					{
						{ "RETR", 0 },
						{ "NLST", 0 },
						{ "LIST", 0 },
						{ "APPE", 1 },
						{ "STOR", 1 },
						{ "STOU", 1 },
						{ "SIZE", 2 },
						{ "MDTM", 2 },
						{ "PWD", 2 },
						{ "MKD", 2 },
						{ "RENAME", 2 },
						{ "DELE", 2 }
					};
				}
				int num;
				if (FtpWebRequest.<>f__switch$mapA.TryGetValue(text, out num))
				{
					switch (num)
					{
					case 0:
						this.DownloadData();
						break;
					case 1:
						this.UploadData();
						break;
					case 2:
						this.ProcessSimpleMethod();
						break;
					default:
						goto IL_0124;
					}
					this.CheckIfAborted();
					return;
				}
			}
			IL_0124:
			throw new Exception(string.Format("Support for command {0} not implemented yet", this.method));
		}

		private void CloseControlConnection()
		{
			if (this.controlStream != null)
			{
				this.SendCommand("QUIT", new string[0]);
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
			switch (text)
			{
			case "SIZE":
			{
				if (ftpStatus.StatusCode != FtpStatusCode.FileStatus)
				{
					throw this.CreateExceptionFromResponse(ftpStatus);
				}
				int num2 = 4;
				int num3 = 0;
				while (num2 < statusDescription.Length && char.IsDigit(statusDescription[num2]))
				{
					num2++;
					num3++;
				}
				if (num3 == 0)
				{
					throw new WebException("Bad format for server response in " + this.method);
				}
				long num4;
				if (!long.TryParse(statusDescription.Substring(4, num3), out num4))
				{
					throw new WebException("Bad format for server response in " + this.method);
				}
				this.ftpResponse.contentLength = num4;
				break;
			}
			case "MDTM":
				if (ftpStatus.StatusCode != FtpStatusCode.FileStatus)
				{
					throw this.CreateExceptionFromResponse(ftpStatus);
				}
				this.ftpResponse.LastModified = DateTime.ParseExact(statusDescription.Substring(4), "yyyyMMddHHmmss", null);
				break;
			case "MKD":
				if (ftpStatus.StatusCode != FtpStatusCode.PathnameCreated)
				{
					throw this.CreateExceptionFromResponse(ftpStatus);
				}
				break;
			case "CWD":
				this.method = "PWD";
				if (ftpStatus.StatusCode != FtpStatusCode.FileActionOK)
				{
					throw this.CreateExceptionFromResponse(ftpStatus);
				}
				ftpStatus = this.SendCommand(this.method, new string[0]);
				if (ftpStatus.StatusCode != FtpStatusCode.PathnameCreated)
				{
					throw this.CreateExceptionFromResponse(ftpStatus);
				}
				break;
			case "RNFR":
				this.method = "RENAME";
				if (ftpStatus.StatusCode != FtpStatusCode.FileCommandPending)
				{
					throw this.CreateExceptionFromResponse(ftpStatus);
				}
				ftpStatus = this.SendCommand("RNTO", new string[] { (this.renameTo == null) ? string.Empty : this.renameTo });
				if (ftpStatus.StatusCode != FtpStatusCode.FileActionOK)
				{
					throw this.CreateExceptionFromResponse(ftpStatus);
				}
				break;
			case "DELE":
				if (ftpStatus.StatusCode != FtpStatusCode.FileActionOK)
				{
					throw this.CreateExceptionFromResponse(ftpStatus);
				}
				break;
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
			global::System.Net.Sockets.Socket socket = null;
			foreach (IPAddress ipaddress in this.hostEntry.AddressList)
			{
				socket = new global::System.Net.Sockets.Socket(ipaddress.AddressFamily, global::System.Net.Sockets.SocketType.Stream, global::System.Net.Sockets.ProtocolType.Tcp);
				IPEndPoint ipendPoint = new IPEndPoint(ipaddress, this.requestUri.Port);
				if (!this.ServicePoint.CallEndPointDelegate(socket, ipendPoint))
				{
					socket.Close();
					socket = null;
				}
				else
				{
					try
					{
						socket.Connect(ipendPoint);
						this.localEndPoint = (IPEndPoint)socket.LocalEndPoint;
						break;
					}
					catch (global::System.Net.Sockets.SocketException ex2)
					{
						ex = ex2;
						socket.Close();
						socket = null;
					}
				}
			}
			if (socket == null)
			{
				throw new WebException("Unable to connect to remote server", ex, WebExceptionStatus.UnknownError, this.ftpResponse);
			}
			this.controlStream = new global::System.Net.Sockets.NetworkStream(socket);
			this.controlReader = new StreamReader(this.controlStream, Encoding.ASCII);
			this.State = FtpWebRequest.RequestState.Authenticating;
			this.Authenticate();
			FtpStatus ftpStatus = this.SendCommand("OPTS", new string[] { "utf8", "on" });
			ftpStatus = this.SendCommand("PWD", new string[0]);
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

		private global::System.Net.Sockets.Socket SetupPassiveConnection(string statusDescription)
		{
			if (statusDescription.Length < 4)
			{
				throw new WebException("Cannot open passive data connection");
			}
			int num = 3;
			while (num < statusDescription.Length && !char.IsDigit(statusDescription[num]))
			{
				num++;
			}
			if (num >= statusDescription.Length)
			{
				throw new WebException("Cannot open passive data connection");
			}
			string[] array = statusDescription.Substring(num).Split(new char[] { ',' }, 6);
			if (array.Length != 6)
			{
				throw new WebException("Cannot open passive data connection");
			}
			int num2 = array[5].Length - 1;
			while (num2 >= 0 && !char.IsDigit(array[5][num2]))
			{
				num2--;
			}
			if (num2 < 0)
			{
				throw new WebException("Cannot open passive data connection");
			}
			array[5] = array[5].Substring(0, num2 + 1);
			IPAddress ipaddress;
			try
			{
				ipaddress = IPAddress.Parse(string.Join(".", array, 0, 4));
			}
			catch (FormatException)
			{
				throw new WebException("Cannot open passive data connection");
			}
			int num3;
			int num4;
			if (!int.TryParse(array[4], out num3) || !int.TryParse(array[5], out num4))
			{
				throw new WebException("Cannot open passive data connection");
			}
			int num5 = (num3 << 8) + num4;
			if (num5 < 0 || num5 > 65535)
			{
				throw new WebException("Cannot open passive data connection");
			}
			IPEndPoint ipendPoint = new IPEndPoint(ipaddress, num5);
			global::System.Net.Sockets.Socket socket = new global::System.Net.Sockets.Socket(ipendPoint.AddressFamily, global::System.Net.Sockets.SocketType.Stream, global::System.Net.Sockets.ProtocolType.Tcp);
			try
			{
				socket.Connect(ipendPoint);
			}
			catch (global::System.Net.Sockets.SocketException)
			{
				socket.Close();
				throw new WebException("Cannot open passive data connection");
			}
			return socket;
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

		private global::System.Net.Sockets.Socket InitDataConnection()
		{
			if (this.usePassive)
			{
				FtpStatus ftpStatus = this.SendCommand("PASV", new string[0]);
				if (ftpStatus.StatusCode != FtpStatusCode.EnteringPassive)
				{
					throw this.CreateExceptionFromResponse(ftpStatus);
				}
				return this.SetupPassiveConnection(ftpStatus.StatusDescription);
			}
			else
			{
				global::System.Net.Sockets.Socket socket = new global::System.Net.Sockets.Socket(global::System.Net.Sockets.AddressFamily.InterNetwork, global::System.Net.Sockets.SocketType.Stream, global::System.Net.Sockets.ProtocolType.Tcp);
				try
				{
					socket.Bind(new IPEndPoint(this.localEndPoint.Address, 0));
					socket.Listen(1);
				}
				catch (global::System.Net.Sockets.SocketException ex)
				{
					socket.Close();
					throw new WebException("Couldn't open listening socket on client", ex);
				}
				IPEndPoint ipendPoint = (IPEndPoint)socket.LocalEndPoint;
				string text = ipendPoint.Address.ToString().Replace('.', ',');
				int num = ipendPoint.Port >> 8;
				int num2 = ipendPoint.Port % 256;
				string text2 = string.Concat(new object[] { text, ",", num, ",", num2 });
				FtpStatus ftpStatus = this.SendCommand("PORT", new string[] { text2 });
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
			global::System.Net.Sockets.Socket socket = this.InitDataConnection();
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
				ftpStatus = this.SendCommand(this.method, new string[0]);
			}
			if (ftpStatus.StatusCode != FtpStatusCode.OpeningData && ftpStatus.StatusCode != FtpStatusCode.DataAlreadyOpen)
			{
				throw this.CreateExceptionFromResponse(ftpStatus);
			}
			if (this.usePassive)
			{
				this.origDataStream = new global::System.Net.Sockets.NetworkStream(socket, true);
				this.dataStream = this.origDataStream;
				if (this.EnableSsl)
				{
					this.ChangeToSSLSocket(ref this.dataStream);
				}
			}
			else
			{
				global::System.Net.Sockets.Socket socket2 = null;
				try
				{
					socket2 = socket.Accept();
				}
				catch (global::System.Net.Sockets.SocketException)
				{
					socket.Close();
					if (socket2 != null)
					{
						socket2.Close();
					}
					throw new ProtocolViolationException("Server commited a protocol violation.");
				}
				socket.Close();
				this.origDataStream = new global::System.Net.Sockets.NetworkStream(socket, true);
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
				text = text3 + '\\' + text;
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
				ftpStatus = new FtpStatus(FtpStatusCode.SendUserCommand, string.Empty);
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
			if (parameters.Length > 0)
			{
				text = text + " " + string.Join(" ", parameters);
			}
			text += "\r\n";
			byte[] bytes = Encoding.ASCII.GetBytes(text);
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
				string text3 = num.ToString() + ' ';
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
						goto Block_8;
					}
				}
				return FtpWebRequest.ServiceNotAvailable();
				Block_8:;
			}
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
			global::System.Net.Security.SslStream sslStream = new global::System.Net.Security.SslStream(stream, true, this.callback, null);
			sslStream.AuthenticateAsClient(this.requestUri.Host, null, global::System.Security.Authentication.SslProtocols.Default, false);
			stream = sslStream;
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

		private const string ChangeDir = "CWD";

		private const string UserCommand = "USER";

		private const string PasswordCommand = "PASS";

		private const string TypeCommand = "TYPE";

		private const string PassiveCommand = "PASV";

		private const string PortCommand = "PORT";

		private const string AbortCommand = "ABOR";

		private const string AuthCommand = "AUTH";

		private const string RestCommand = "REST";

		private const string RenameFromCommand = "RNFR";

		private const string RenameToCommand = "RNTO";

		private const string QuitCommand = "QUIT";

		private const string EOL = "\r\n";

		private global::System.Uri requestUri;

		private string file_name;

		private ServicePoint servicePoint;

		private Stream origDataStream;

		private Stream dataStream;

		private Stream controlStream;

		private StreamReader controlReader;

		private NetworkCredential credentials;

		private IPHostEntry hostEntry;

		private IPEndPoint localEndPoint;

		private IWebProxy proxy;

		private int timeout = 100000;

		private int rwTimeout = 300000;

		private long offset;

		private bool binary = true;

		private bool enableSsl;

		private bool usePassive = true;

		private bool keepAlive;

		private string method = "RETR";

		private string renameTo;

		private object locker = new object();

		private FtpWebRequest.RequestState requestState;

		private FtpAsyncResult asyncResult;

		private FtpWebResponse ftpResponse;

		private Stream requestStream;

		private string initial_path;

		private static readonly string[] supportedCommands = new string[]
		{
			"APPE", "DELE", "LIST", "MDTM", "MKD", "NLST", "PWD", "RENAME", "RETR", "RMD",
			"SIZE", "STOR", "STOU"
		};

		private global::System.Net.Security.RemoteCertificateValidationCallback callback = delegate(object sender, X509Certificate certificate, global::System.Security.Cryptography.X509Certificates.X509Chain chain, global::System.Net.Security.SslPolicyErrors sslPolicyErrors)
		{
			if (ServicePointManager.ServerCertificateValidationCallback != null)
			{
				return ServicePointManager.ServerCertificateValidationCallback(sender, certificate, chain, sslPolicyErrors);
			}
			if (sslPolicyErrors != global::System.Net.Security.SslPolicyErrors.None)
			{
				throw new InvalidOperationException("SSL authentication error: " + sslPolicyErrors);
			}
			return true;
		};

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
