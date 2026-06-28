using System;
using System.Configuration;
using System.IO;
using System.Net.Cache;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace System.Net
{
	[Serializable]
	public class HttpWebRequest : WebRequest, ISerializable
	{
		internal HttpWebRequest(global::System.Uri uri)
		{
			this.requestUri = uri;
			this.actualUri = uri;
			this.proxy = GlobalProxySelection.Select;
		}

		[Obsolete("Serialization is obsoleted for this type", false)]
		protected HttpWebRequest(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			this.requestUri = (global::System.Uri)serializationInfo.GetValue("requestUri", typeof(global::System.Uri));
			this.actualUri = (global::System.Uri)serializationInfo.GetValue("actualUri", typeof(global::System.Uri));
			this.allowAutoRedirect = serializationInfo.GetBoolean("allowAutoRedirect");
			this.allowBuffering = serializationInfo.GetBoolean("allowBuffering");
			this.certificates = (global::System.Security.Cryptography.X509Certificates.X509CertificateCollection)serializationInfo.GetValue("certificates", typeof(global::System.Security.Cryptography.X509Certificates.X509CertificateCollection));
			this.connectionGroup = serializationInfo.GetString("connectionGroup");
			this.contentLength = serializationInfo.GetInt64("contentLength");
			this.webHeaders = (WebHeaderCollection)serializationInfo.GetValue("webHeaders", typeof(WebHeaderCollection));
			this.keepAlive = serializationInfo.GetBoolean("keepAlive");
			this.maxAutoRedirect = serializationInfo.GetInt32("maxAutoRedirect");
			this.mediaType = serializationInfo.GetString("mediaType");
			this.method = serializationInfo.GetString("method");
			this.initialMethod = serializationInfo.GetString("initialMethod");
			this.pipelined = serializationInfo.GetBoolean("pipelined");
			this.version = (Version)serializationInfo.GetValue("version", typeof(Version));
			this.proxy = (IWebProxy)serializationInfo.GetValue("proxy", typeof(IWebProxy));
			this.sendChunked = serializationInfo.GetBoolean("sendChunked");
			this.timeout = serializationInfo.GetInt32("timeout");
			this.redirects = serializationInfo.GetInt32("redirects");
		}

		static HttpWebRequest()
		{
			NetConfig netConfig = global::System.Configuration.ConfigurationSettings.GetConfig("system.net/settings") as NetConfig;
			if (netConfig != null)
			{
				int num = netConfig.MaxResponseHeadersLength;
				if (num != -1)
				{
					num *= 64;
				}
				HttpWebRequest.defaultMaxResponseHeadersLength = num;
			}
		}

		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			this.GetObjectData(serializationInfo, streamingContext);
		}

		internal bool UsesNtlmAuthentication
		{
			get
			{
				return this.is_ntlm_auth;
			}
		}

		public string Accept
		{
			get
			{
				return this.webHeaders["Accept"];
			}
			set
			{
				this.CheckRequestStarted();
				this.webHeaders.RemoveAndAdd("Accept", value);
			}
		}

		public global::System.Uri Address
		{
			get
			{
				return this.actualUri;
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
				this.allowAutoRedirect = value;
			}
		}

		public bool AllowWriteStreamBuffering
		{
			get
			{
				return this.allowBuffering;
			}
			set
			{
				this.allowBuffering = value;
			}
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		public DecompressionMethods AutomaticDecompression
		{
			get
			{
				return this.auto_decomp;
			}
			set
			{
				this.CheckRequestStarted();
				this.auto_decomp = value;
			}
		}

		internal bool InternalAllowBuffering
		{
			get
			{
				return this.allowBuffering && (this.method != "HEAD" && this.method != "GET" && this.method != "MKCOL" && this.method != "CONNECT" && this.method != "DELETE") && this.method != "TRACE";
			}
		}

		public global::System.Security.Cryptography.X509Certificates.X509CertificateCollection ClientCertificates
		{
			get
			{
				if (this.certificates == null)
				{
					this.certificates = new global::System.Security.Cryptography.X509Certificates.X509CertificateCollection();
				}
				return this.certificates;
			}
			[global::System.MonoTODO]
			set
			{
				throw HttpWebRequest.GetMustImplement();
			}
		}

		public string Connection
		{
			get
			{
				return this.webHeaders["Connection"];
			}
			set
			{
				this.CheckRequestStarted();
				string text = value;
				if (text != null)
				{
					text = text.Trim().ToLower();
				}
				if (text == null || text.Length == 0)
				{
					this.webHeaders.RemoveInternal("Connection");
					return;
				}
				if (text == "keep-alive" || text == "close")
				{
					throw new ArgumentException("Keep-Alive and Close may not be set with this property");
				}
				if (this.keepAlive && text.IndexOf("keep-alive") == -1)
				{
					value += ", Keep-Alive";
				}
				this.webHeaders.RemoveAndAdd("Connection", value);
			}
		}

		public override string ConnectionGroupName
		{
			get
			{
				return this.connectionGroup;
			}
			set
			{
				this.connectionGroup = value;
			}
		}

		public override long ContentLength
		{
			get
			{
				return this.contentLength;
			}
			set
			{
				this.CheckRequestStarted();
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException("value", "Content-Length must be >= 0");
				}
				this.contentLength = value;
			}
		}

		internal long InternalContentLength
		{
			set
			{
				this.contentLength = value;
			}
		}

		public override string ContentType
		{
			get
			{
				return this.webHeaders["Content-Type"];
			}
			set
			{
				if (value == null || value.Trim().Length == 0)
				{
					this.webHeaders.RemoveInternal("Content-Type");
					return;
				}
				this.webHeaders.RemoveAndAdd("Content-Type", value);
			}
		}

		public HttpContinueDelegate ContinueDelegate
		{
			get
			{
				return this.continueDelegate;
			}
			set
			{
				this.continueDelegate = value;
			}
		}

		public CookieContainer CookieContainer
		{
			get
			{
				return this.cookieContainer;
			}
			set
			{
				this.cookieContainer = value;
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
				this.credentials = value;
			}
		}

		[global::System.MonoTODO]
		public new static global::System.Net.Cache.RequestCachePolicy DefaultCachePolicy
		{
			get
			{
				throw HttpWebRequest.GetMustImplement();
			}
			set
			{
				throw HttpWebRequest.GetMustImplement();
			}
		}

		[global::System.MonoTODO]
		public static int DefaultMaximumErrorResponseLength
		{
			get
			{
				throw HttpWebRequest.GetMustImplement();
			}
			set
			{
				throw HttpWebRequest.GetMustImplement();
			}
		}

		public string Expect
		{
			get
			{
				return this.webHeaders["Expect"];
			}
			set
			{
				this.CheckRequestStarted();
				string text = value;
				if (text != null)
				{
					text = text.Trim().ToLower();
				}
				if (text == null || text.Length == 0)
				{
					this.webHeaders.RemoveInternal("Expect");
					return;
				}
				if (text == "100-continue")
				{
					throw new ArgumentException("100-Continue cannot be set with this property.", "value");
				}
				this.webHeaders.RemoveAndAdd("Expect", value);
			}
		}

		public bool HaveResponse
		{
			get
			{
				return this.haveResponse;
			}
		}

		public override WebHeaderCollection Headers
		{
			get
			{
				return this.webHeaders;
			}
			set
			{
				this.CheckRequestStarted();
				WebHeaderCollection webHeaderCollection = new WebHeaderCollection(true);
				int count = value.Count;
				for (int i = 0; i < count; i++)
				{
					webHeaderCollection.Add(value.GetKey(i), value.Get(i));
				}
				this.webHeaders = webHeaderCollection;
			}
		}

		public DateTime IfModifiedSince
		{
			get
			{
				string text = this.webHeaders["If-Modified-Since"];
				if (text == null)
				{
					return DateTime.Now;
				}
				DateTime dateTime;
				try
				{
					dateTime = MonoHttpDate.Parse(text);
				}
				catch (Exception)
				{
					dateTime = DateTime.Now;
				}
				return dateTime;
			}
			set
			{
				this.CheckRequestStarted();
				this.webHeaders.SetInternal("If-Modified-Since", value.ToUniversalTime().ToString("r", null));
			}
		}

		public bool KeepAlive
		{
			get
			{
				return this.keepAlive;
			}
			set
			{
				this.keepAlive = value;
			}
		}

		public int MaximumAutomaticRedirections
		{
			get
			{
				return this.maxAutoRedirect;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentException("Must be > 0", "value");
				}
				this.maxAutoRedirect = value;
			}
		}

		[global::System.MonoTODO("Use this")]
		public int MaximumResponseHeadersLength
		{
			get
			{
				return this.maxResponseHeadersLength;
			}
			set
			{
				this.maxResponseHeadersLength = value;
			}
		}

		[global::System.MonoTODO("Use this")]
		public static int DefaultMaximumResponseHeadersLength
		{
			get
			{
				return HttpWebRequest.defaultMaxResponseHeadersLength;
			}
			set
			{
				HttpWebRequest.defaultMaxResponseHeadersLength = value;
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
				if (this.requestSent)
				{
					throw new InvalidOperationException("The request has already been sent.");
				}
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException("value", "Must be >= -1");
				}
				this.readWriteTimeout = value;
			}
		}

		public string MediaType
		{
			get
			{
				return this.mediaType;
			}
			set
			{
				this.mediaType = value;
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
				if (value == null || value.Trim() == string.Empty)
				{
					throw new ArgumentException("not a valid method");
				}
				this.method = value;
			}
		}

		public bool Pipelined
		{
			get
			{
				return this.pipelined;
			}
			set
			{
				this.pipelined = value;
			}
		}

		public override bool PreAuthenticate
		{
			get
			{
				return this.preAuthenticate;
			}
			set
			{
				this.preAuthenticate = value;
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				return this.version;
			}
			set
			{
				if (value != HttpVersion.Version10 && value != HttpVersion.Version11)
				{
					throw new ArgumentException("value");
				}
				this.version = value;
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
				this.servicePoint = null;
			}
		}

		public string Referer
		{
			get
			{
				return this.webHeaders["Referer"];
			}
			set
			{
				this.CheckRequestStarted();
				if (value == null || value.Trim().Length == 0)
				{
					this.webHeaders.RemoveInternal("Referer");
					return;
				}
				this.webHeaders.SetInternal("Referer", value);
			}
		}

		public override global::System.Uri RequestUri
		{
			get
			{
				return this.requestUri;
			}
		}

		public bool SendChunked
		{
			get
			{
				return this.sendChunked;
			}
			set
			{
				this.CheckRequestStarted();
				this.sendChunked = value;
			}
		}

		public ServicePoint ServicePoint
		{
			get
			{
				return this.GetServicePoint();
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
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.timeout = value;
			}
		}

		public string TransferEncoding
		{
			get
			{
				return this.webHeaders["Transfer-Encoding"];
			}
			set
			{
				this.CheckRequestStarted();
				string text = value;
				if (text != null)
				{
					text = text.Trim().ToLower();
				}
				if (text == null || text.Length == 0)
				{
					this.webHeaders.RemoveInternal("Transfer-Encoding");
					return;
				}
				if (text == "chunked")
				{
					throw new ArgumentException("Chunked encoding must be set with the SendChunked property");
				}
				if (!this.sendChunked)
				{
					throw new ArgumentException("SendChunked must be True", "value");
				}
				this.webHeaders.RemoveAndAdd("Transfer-Encoding", value);
			}
		}

		public override bool UseDefaultCredentials
		{
			get
			{
				return CredentialCache.DefaultCredentials == this.Credentials;
			}
			set
			{
				ICredentials credentials;
				if (value)
				{
					ICredentials defaultCredentials = CredentialCache.DefaultCredentials;
					credentials = defaultCredentials;
				}
				else
				{
					credentials = null;
				}
				this.Credentials = credentials;
			}
		}

		public string UserAgent
		{
			get
			{
				return this.webHeaders["User-Agent"];
			}
			set
			{
				this.webHeaders.SetInternal("User-Agent", value);
			}
		}

		public bool UnsafeAuthenticatedConnectionSharing
		{
			get
			{
				return this.unsafe_auth_blah;
			}
			set
			{
				this.unsafe_auth_blah = value;
			}
		}

		internal bool GotRequestStream
		{
			get
			{
				return this.gotRequestStream;
			}
		}

		internal bool ExpectContinue
		{
			get
			{
				return this.expectContinue;
			}
			set
			{
				this.expectContinue = value;
			}
		}

		internal global::System.Uri AuthUri
		{
			get
			{
				return this.actualUri;
			}
		}

		internal bool ProxyQuery
		{
			get
			{
				return this.servicePoint.UsesProxy && !this.servicePoint.UseConnect;
			}
		}

		internal ServicePoint GetServicePoint()
		{
			object obj = this.locker;
			lock (obj)
			{
				if (this.hostChanged || this.servicePoint == null)
				{
					this.servicePoint = ServicePointManager.FindServicePoint(this.actualUri, this.proxy);
					this.hostChanged = false;
				}
			}
			return this.servicePoint;
		}

		public void AddRange(int range)
		{
			this.AddRange("bytes", range);
		}

		public void AddRange(int from, int to)
		{
			this.AddRange("bytes", from, to);
		}

		public void AddRange(string rangeSpecifier, int range)
		{
			if (rangeSpecifier == null)
			{
				throw new ArgumentNullException("rangeSpecifier");
			}
			string text = this.webHeaders["Range"];
			if (text == null || text.Length == 0)
			{
				text = rangeSpecifier + "=";
			}
			else
			{
				if (!text.ToLower().StartsWith(rangeSpecifier.ToLower() + "="))
				{
					throw new InvalidOperationException("rangeSpecifier");
				}
				text += ",";
			}
			this.webHeaders.RemoveAndAdd("Range", text + range + "-");
		}

		public void AddRange(string rangeSpecifier, int from, int to)
		{
			if (rangeSpecifier == null)
			{
				throw new ArgumentNullException("rangeSpecifier");
			}
			if (from < 0 || to < 0 || from > to)
			{
				throw new ArgumentOutOfRangeException();
			}
			string text = this.webHeaders["Range"];
			if (text == null || text.Length == 0)
			{
				text = rangeSpecifier + "=";
			}
			else
			{
				if (!text.ToLower().StartsWith(rangeSpecifier.ToLower() + "="))
				{
					throw new InvalidOperationException("rangeSpecifier");
				}
				text += ",";
			}
			this.webHeaders.RemoveAndAdd("Range", string.Concat(new object[] { text, from, "-", to }));
		}

		public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
		{
			if (this.Aborted)
			{
				throw new WebException("The request was canceled.", WebExceptionStatus.RequestCanceled);
			}
			bool flag = !(this.method == "GET") && !(this.method == "CONNECT") && !(this.method == "HEAD") && !(this.method == "TRACE") && !(this.method == "DELETE");
			if (this.method == null || !flag)
			{
				throw new ProtocolViolationException("Cannot send data when method is: " + this.method);
			}
			if (this.contentLength == -1L && !this.sendChunked && !this.allowBuffering && this.KeepAlive)
			{
				throw new ProtocolViolationException("Content-Length not set");
			}
			string transferEncoding = this.TransferEncoding;
			if (!this.sendChunked && transferEncoding != null && transferEncoding.Trim() != string.Empty)
			{
				throw new ProtocolViolationException("SendChunked should be true.");
			}
			object obj = this.locker;
			IAsyncResult asyncResult;
			lock (obj)
			{
				if (this.asyncWrite != null)
				{
					throw new InvalidOperationException("Cannot re-call start of asynchronous method while a previous call is still in progress.");
				}
				this.asyncWrite = new WebAsyncResult(this, callback, state);
				this.initialMethod = this.method;
				if (this.haveRequest && this.writeStream != null)
				{
					this.asyncWrite.SetCompleted(true, this.writeStream);
					this.asyncWrite.DoCallback();
					asyncResult = this.asyncWrite;
				}
				else
				{
					this.gotRequestStream = true;
					WebAsyncResult webAsyncResult = this.asyncWrite;
					if (!this.requestSent)
					{
						this.requestSent = true;
						this.redirects = 0;
						this.servicePoint = this.GetServicePoint();
						this.abortHandler = this.servicePoint.SendRequest(this, this.connectionGroup);
					}
					asyncResult = webAsyncResult;
				}
			}
			return asyncResult;
		}

		public override Stream EndGetRequestStream(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			WebAsyncResult webAsyncResult = asyncResult as WebAsyncResult;
			if (webAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult");
			}
			this.asyncWrite = webAsyncResult;
			webAsyncResult.WaitUntilComplete();
			Exception exception = webAsyncResult.Exception;
			if (exception != null)
			{
				throw exception;
			}
			return webAsyncResult.WriteStream;
		}

		public override Stream GetRequestStream()
		{
			IAsyncResult asyncResult = this.asyncWrite;
			if (asyncResult == null)
			{
				asyncResult = this.BeginGetRequestStream(null, null);
				this.asyncWrite = (WebAsyncResult)asyncResult;
			}
			if (!asyncResult.IsCompleted && !asyncResult.AsyncWaitHandle.WaitOne(this.timeout, false))
			{
				this.Abort();
				throw new WebException("The request timed out", WebExceptionStatus.Timeout);
			}
			return this.EndGetRequestStream(asyncResult);
		}

		private void CheckIfForceWrite()
		{
			if (this.writeStream == null || this.writeStream.RequestWritten || this.contentLength < 0L || !this.InternalAllowBuffering)
			{
				return;
			}
			if ((long)this.writeStream.WriteBufferLength == this.contentLength)
			{
				this.writeStream.WriteRequest();
			}
		}

		public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
		{
			if (this.Aborted)
			{
				throw new WebException("The request was canceled.", WebExceptionStatus.RequestCanceled);
			}
			if (this.method == null)
			{
				throw new ProtocolViolationException("Method is null.");
			}
			string transferEncoding = this.TransferEncoding;
			if (!this.sendChunked && transferEncoding != null && transferEncoding.Trim() != string.Empty)
			{
				throw new ProtocolViolationException("SendChunked should be true.");
			}
			Monitor.Enter(this.locker);
			this.getResponseCalled = true;
			if (this.asyncRead != null && !this.haveResponse)
			{
				Monitor.Exit(this.locker);
				throw new InvalidOperationException("Cannot re-call start of asynchronous method while a previous call is still in progress.");
			}
			this.CheckIfForceWrite();
			this.asyncRead = new WebAsyncResult(this, callback, state);
			WebAsyncResult webAsyncResult = this.asyncRead;
			this.initialMethod = this.method;
			if (this.haveResponse)
			{
				Exception ex = this.saved_exc;
				if (this.webResponse != null)
				{
					Monitor.Exit(this.locker);
					if (ex == null)
					{
						webAsyncResult.SetCompleted(true, this.webResponse);
					}
					else
					{
						webAsyncResult.SetCompleted(true, ex);
					}
					webAsyncResult.DoCallback();
					return webAsyncResult;
				}
				if (ex != null)
				{
					Monitor.Exit(this.locker);
					webAsyncResult.SetCompleted(true, ex);
					webAsyncResult.DoCallback();
					return webAsyncResult;
				}
			}
			if (!this.requestSent)
			{
				this.requestSent = true;
				this.redirects = 0;
				this.servicePoint = this.GetServicePoint();
				this.abortHandler = this.servicePoint.SendRequest(this, this.connectionGroup);
			}
			Monitor.Exit(this.locker);
			return webAsyncResult;
		}

		public override WebResponse EndGetResponse(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			WebAsyncResult webAsyncResult = asyncResult as WebAsyncResult;
			if (webAsyncResult == null)
			{
				throw new ArgumentException("Invalid IAsyncResult", "asyncResult");
			}
			if (!webAsyncResult.WaitUntilComplete(this.timeout, false))
			{
				this.Abort();
				throw new WebException("The request timed out", WebExceptionStatus.Timeout);
			}
			if (webAsyncResult.GotException)
			{
				throw webAsyncResult.Exception;
			}
			return webAsyncResult.Response;
		}

		public override WebResponse GetResponse()
		{
			WebAsyncResult webAsyncResult = (WebAsyncResult)this.BeginGetResponse(null, null);
			return this.EndGetResponse(webAsyncResult);
		}

		internal bool FinishedReading
		{
			get
			{
				return this.finished_reading;
			}
			set
			{
				this.finished_reading = value;
			}
		}

		internal bool Aborted
		{
			get
			{
				return Interlocked.CompareExchange(ref this.aborted, 0, 0) == 1;
			}
		}

		public override void Abort()
		{
			if (Interlocked.CompareExchange(ref this.aborted, 1, 0) == 1)
			{
				return;
			}
			if (this.haveResponse && this.finished_reading)
			{
				return;
			}
			this.haveResponse = true;
			if (this.abortHandler != null)
			{
				try
				{
					this.abortHandler(this, EventArgs.Empty);
				}
				catch (Exception)
				{
				}
				this.abortHandler = null;
			}
			if (this.asyncWrite != null)
			{
				WebAsyncResult webAsyncResult = this.asyncWrite;
				if (!webAsyncResult.IsCompleted)
				{
					try
					{
						WebException ex = new WebException("Aborted.", WebExceptionStatus.RequestCanceled);
						webAsyncResult.SetCompleted(false, ex);
						webAsyncResult.DoCallback();
					}
					catch
					{
					}
				}
				this.asyncWrite = null;
			}
			if (this.asyncRead != null)
			{
				WebAsyncResult webAsyncResult2 = this.asyncRead;
				if (!webAsyncResult2.IsCompleted)
				{
					try
					{
						WebException ex2 = new WebException("Aborted.", WebExceptionStatus.RequestCanceled);
						webAsyncResult2.SetCompleted(false, ex2);
						webAsyncResult2.DoCallback();
					}
					catch
					{
					}
				}
				this.asyncRead = null;
			}
			if (this.writeStream != null)
			{
				try
				{
					this.writeStream.Close();
					this.writeStream = null;
				}
				catch
				{
				}
			}
			if (this.webResponse != null)
			{
				try
				{
					this.webResponse.Close();
					this.webResponse = null;
				}
				catch
				{
				}
			}
		}

		protected override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			serializationInfo.AddValue("requestUri", this.requestUri, typeof(global::System.Uri));
			serializationInfo.AddValue("actualUri", this.actualUri, typeof(global::System.Uri));
			serializationInfo.AddValue("allowAutoRedirect", this.allowAutoRedirect);
			serializationInfo.AddValue("allowBuffering", this.allowBuffering);
			serializationInfo.AddValue("certificates", this.certificates, typeof(global::System.Security.Cryptography.X509Certificates.X509CertificateCollection));
			serializationInfo.AddValue("connectionGroup", this.connectionGroup);
			serializationInfo.AddValue("contentLength", this.contentLength);
			serializationInfo.AddValue("webHeaders", this.webHeaders, typeof(WebHeaderCollection));
			serializationInfo.AddValue("keepAlive", this.keepAlive);
			serializationInfo.AddValue("maxAutoRedirect", this.maxAutoRedirect);
			serializationInfo.AddValue("mediaType", this.mediaType);
			serializationInfo.AddValue("method", this.method);
			serializationInfo.AddValue("initialMethod", this.initialMethod);
			serializationInfo.AddValue("pipelined", this.pipelined);
			serializationInfo.AddValue("version", this.version, typeof(Version));
			serializationInfo.AddValue("proxy", this.proxy, typeof(IWebProxy));
			serializationInfo.AddValue("sendChunked", this.sendChunked);
			serializationInfo.AddValue("timeout", this.timeout);
			serializationInfo.AddValue("redirects", this.redirects);
		}

		private void CheckRequestStarted()
		{
			if (this.requestSent)
			{
				throw new InvalidOperationException("request started");
			}
		}

		internal void DoContinueDelegate(int statusCode, WebHeaderCollection headers)
		{
			if (this.continueDelegate != null)
			{
				this.continueDelegate(statusCode, headers);
			}
		}

		private bool Redirect(WebAsyncResult result, HttpStatusCode code)
		{
			this.redirects++;
			Exception ex = null;
			string text = null;
			switch (code)
			{
			case HttpStatusCode.MultipleChoices:
				ex = new WebException("Ambiguous redirect.");
				goto IL_00E4;
			case HttpStatusCode.MovedPermanently:
			case HttpStatusCode.Found:
			case HttpStatusCode.TemporaryRedirect:
				this.contentLength = -1L;
				this.bodyBufferLength = 0;
				this.bodyBuffer = null;
				this.method = "GET";
				text = this.webResponse.Headers["Location"];
				goto IL_00E4;
			case HttpStatusCode.SeeOther:
				this.method = "GET";
				text = this.webResponse.Headers["Location"];
				goto IL_00E4;
			case HttpStatusCode.NotModified:
				return false;
			case HttpStatusCode.UseProxy:
				ex = new NotImplementedException("Proxy support not available.");
				goto IL_00E4;
			}
			ex = new ProtocolViolationException("Invalid status code: " + (int)code);
			IL_00E4:
			if (ex != null)
			{
				throw ex;
			}
			if (text == null)
			{
				throw new WebException("No Location header found for " + (int)code, WebExceptionStatus.ProtocolError);
			}
			global::System.Uri uri = this.actualUri;
			try
			{
				this.actualUri = new global::System.Uri(this.actualUri, text);
			}
			catch (Exception)
			{
				throw new WebException(string.Format("Invalid URL ({0}) for {1}", text, (int)code), WebExceptionStatus.ProtocolError);
			}
			this.hostChanged = this.actualUri.Scheme != uri.Scheme || this.actualUri.Host != uri.Host || this.actualUri.Port != uri.Port;
			return true;
		}

		private string GetHeaders()
		{
			bool flag = false;
			if (this.sendChunked)
			{
				flag = true;
				this.webHeaders.RemoveAndAdd("Transfer-Encoding", "chunked");
				this.webHeaders.RemoveInternal("Content-Length");
			}
			else if (this.contentLength != -1L)
			{
				if (this.contentLength > 0L)
				{
					flag = true;
				}
				this.webHeaders.SetInternal("Content-Length", this.contentLength.ToString());
				this.webHeaders.RemoveInternal("Transfer-Encoding");
			}
			if (this.actualVersion == HttpVersion.Version11 && flag && this.servicePoint.SendContinue)
			{
				this.webHeaders.RemoveAndAdd("Expect", "100-continue");
				this.expectContinue = true;
			}
			else
			{
				this.webHeaders.RemoveInternal("Expect");
				this.expectContinue = false;
			}
			bool proxyQuery = this.ProxyQuery;
			string text = ((!proxyQuery) ? "Connection" : "Proxy-Connection");
			this.webHeaders.RemoveInternal(proxyQuery ? "Connection" : "Proxy-Connection");
			Version protocolVersion = this.servicePoint.ProtocolVersion;
			bool flag2 = protocolVersion == null || protocolVersion == HttpVersion.Version10;
			if (this.keepAlive && (this.version == HttpVersion.Version10 || flag2))
			{
				this.webHeaders.RemoveAndAdd(text, "keep-alive");
			}
			else if (!this.keepAlive && this.version == HttpVersion.Version11)
			{
				this.webHeaders.RemoveAndAdd(text, "close");
			}
			this.webHeaders.SetInternal("Host", this.actualUri.Authority);
			if (this.cookieContainer != null)
			{
				string cookieHeader = this.cookieContainer.GetCookieHeader(this.actualUri);
				if (cookieHeader != string.Empty)
				{
					this.webHeaders.SetInternal("Cookie", cookieHeader);
				}
			}
			string text2 = null;
			if ((this.auto_decomp & DecompressionMethods.GZip) != DecompressionMethods.None)
			{
				text2 = "gzip";
			}
			if ((this.auto_decomp & DecompressionMethods.Deflate) != DecompressionMethods.None)
			{
				text2 = ((text2 == null) ? "deflate" : "gzip, deflate");
			}
			if (text2 != null)
			{
				this.webHeaders.RemoveAndAdd("Accept-Encoding", text2);
			}
			if (!this.usedPreAuth && this.preAuthenticate)
			{
				this.DoPreAuthenticate();
			}
			return this.webHeaders.ToString();
		}

		private void DoPreAuthenticate()
		{
			bool flag = this.proxy != null && !this.proxy.IsBypassed(this.actualUri);
			ICredentials credentials2;
			if (!flag || this.credentials != null)
			{
				ICredentials credentials = this.credentials;
				credentials2 = credentials;
			}
			else
			{
				credentials2 = this.proxy.Credentials;
			}
			ICredentials credentials3 = credentials2;
			Authorization authorization = AuthenticationManager.PreAuthenticate(this, credentials3);
			if (authorization == null)
			{
				return;
			}
			this.webHeaders.RemoveInternal("Proxy-Authorization");
			this.webHeaders.RemoveInternal("Authorization");
			string text = ((!flag || this.credentials != null) ? "Authorization" : "Proxy-Authorization");
			this.webHeaders[text] = authorization.Message;
			this.usedPreAuth = true;
		}

		internal void SetWriteStreamError(WebExceptionStatus status, Exception exc)
		{
			if (this.Aborted)
			{
				return;
			}
			WebAsyncResult webAsyncResult = this.asyncWrite;
			if (webAsyncResult == null)
			{
				webAsyncResult = this.asyncRead;
			}
			if (webAsyncResult != null)
			{
				WebException ex;
				if (exc == null)
				{
					string text = "Error: " + status;
					ex = new WebException(text, status);
				}
				else
				{
					string text = string.Format("Error: {0} ({1})", status, exc.Message);
					ex = new WebException(text, exc, status);
				}
				webAsyncResult.SetCompleted(false, ex);
				webAsyncResult.DoCallback();
			}
		}

		internal void SendRequestHeaders(bool propagate_error)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text;
			if (!this.ProxyQuery)
			{
				text = this.actualUri.PathAndQuery;
			}
			else if (this.actualUri.IsDefaultPort)
			{
				text = string.Format("{0}://{1}{2}", this.actualUri.Scheme, this.actualUri.Host, this.actualUri.PathAndQuery);
			}
			else
			{
				text = string.Format("{0}://{1}:{2}{3}", new object[]
				{
					this.actualUri.Scheme,
					this.actualUri.Host,
					this.actualUri.Port,
					this.actualUri.PathAndQuery
				});
			}
			if (this.servicePoint.ProtocolVersion != null && this.servicePoint.ProtocolVersion < this.version)
			{
				this.actualVersion = this.servicePoint.ProtocolVersion;
			}
			else
			{
				this.actualVersion = this.version;
			}
			stringBuilder.AppendFormat("{0} {1} HTTP/{2}.{3}\r\n", new object[]
			{
				this.method,
				text,
				this.actualVersion.Major,
				this.actualVersion.Minor
			});
			stringBuilder.Append(this.GetHeaders());
			string text2 = stringBuilder.ToString();
			byte[] bytes = Encoding.UTF8.GetBytes(text2);
			try
			{
				this.writeStream.SetHeaders(bytes);
			}
			catch (WebException ex)
			{
				this.SetWriteStreamError(ex.Status, ex);
				if (propagate_error)
				{
					throw;
				}
			}
			catch (Exception ex2)
			{
				this.SetWriteStreamError(WebExceptionStatus.SendFailure, ex2);
				if (propagate_error)
				{
					throw;
				}
			}
		}

		internal void SetWriteStream(WebConnectionStream stream)
		{
			if (this.Aborted)
			{
				return;
			}
			this.writeStream = stream;
			if (this.bodyBuffer != null)
			{
				this.webHeaders.RemoveInternal("Transfer-Encoding");
				this.contentLength = (long)this.bodyBufferLength;
				this.writeStream.SendChunked = false;
			}
			this.SendRequestHeaders(false);
			this.haveRequest = true;
			if (this.bodyBuffer != null)
			{
				this.writeStream.Write(this.bodyBuffer, 0, this.bodyBufferLength);
				this.bodyBuffer = null;
				this.writeStream.Close();
			}
			else if (this.method != "HEAD" && this.method != "GET" && this.method != "MKCOL" && this.method != "CONNECT" && this.method != "DELETE" && this.method != "TRACE" && this.getResponseCalled && !this.writeStream.RequestWritten)
			{
				this.writeStream.WriteRequest();
			}
			if (this.asyncWrite != null)
			{
				this.asyncWrite.SetCompleted(false, stream);
				this.asyncWrite.DoCallback();
				this.asyncWrite = null;
			}
		}

		internal void SetResponseError(WebExceptionStatus status, Exception e, string where)
		{
			if (this.Aborted)
			{
				return;
			}
			object obj = this.locker;
			lock (obj)
			{
				string text = string.Format("Error getting response stream ({0}): {1}", where, status);
				WebAsyncResult webAsyncResult = this.asyncRead;
				if (webAsyncResult == null)
				{
					webAsyncResult = this.asyncWrite;
				}
				WebException ex;
				if (e is WebException)
				{
					ex = (WebException)e;
				}
				else
				{
					ex = new WebException(text, e, status, null);
				}
				if (webAsyncResult != null)
				{
					if (!webAsyncResult.IsCompleted)
					{
						webAsyncResult.SetCompleted(false, ex);
						webAsyncResult.DoCallback();
					}
					else if (webAsyncResult == this.asyncWrite)
					{
						this.saved_exc = ex;
					}
					this.haveResponse = true;
					this.asyncRead = null;
					this.asyncWrite = null;
				}
				else
				{
					this.haveResponse = true;
					this.saved_exc = ex;
				}
			}
		}

		private void CheckSendError(WebConnectionData data)
		{
			int statusCode = data.StatusCode;
			if (statusCode < 400 || statusCode == 401 || statusCode == 407)
			{
				return;
			}
			if (this.writeStream != null && this.asyncRead == null && !this.writeStream.CompleteRequestWritten)
			{
				this.saved_exc = new WebException(data.StatusDescription, null, WebExceptionStatus.ProtocolError, this.webResponse);
				this.webResponse.ReadAll();
			}
		}

		private void HandleNtlmAuth(WebAsyncResult r)
		{
			WebConnectionStream webConnectionStream = this.webResponse.GetResponseStream() as WebConnectionStream;
			if (webConnectionStream != null)
			{
				WebConnection connection = webConnectionStream.Connection;
				connection.PriorityRequest = this;
				ICredentials credentials2;
				if (this.proxy == null || this.proxy.IsBypassed(this.actualUri))
				{
					ICredentials credentials = this.credentials;
					credentials2 = credentials;
				}
				else
				{
					credentials2 = this.proxy.Credentials;
				}
				ICredentials credentials3 = credentials2;
				if (credentials3 != null)
				{
					connection.NtlmCredential = credentials3.GetCredential(this.requestUri, "NTLM");
					connection.UnsafeAuthenticatedConnectionSharing = this.unsafe_auth_blah;
				}
			}
			r.Reset();
			this.haveResponse = false;
			this.webResponse.ReadAll();
			this.webResponse = null;
		}

		internal void SetResponseData(WebConnectionData data)
		{
			object obj = this.locker;
			lock (obj)
			{
				if (this.Aborted)
				{
					if (data.stream != null)
					{
						data.stream.Close();
					}
				}
				else
				{
					WebException ex = null;
					try
					{
						this.webResponse = new HttpWebResponse(this.actualUri, this.method, data, this.cookieContainer);
					}
					catch (Exception ex2)
					{
						ex = new WebException(ex2.Message, ex2, WebExceptionStatus.ProtocolError, null);
						if (data.stream != null)
						{
							data.stream.Close();
						}
					}
					if (ex == null && (this.method == "POST" || this.method == "PUT"))
					{
						object obj2 = this.locker;
						lock (obj2)
						{
							this.CheckSendError(data);
							if (this.saved_exc != null)
							{
								ex = (WebException)this.saved_exc;
							}
						}
					}
					WebAsyncResult webAsyncResult = this.asyncRead;
					bool flag = false;
					if (webAsyncResult == null && this.webResponse != null)
					{
						flag = true;
						webAsyncResult = new WebAsyncResult(null, null);
						webAsyncResult.SetCompleted(false, this.webResponse);
					}
					if (webAsyncResult != null)
					{
						if (ex != null)
						{
							webAsyncResult.SetCompleted(false, ex);
							webAsyncResult.DoCallback();
						}
						else
						{
							try
							{
								if (!this.CheckFinalStatus(webAsyncResult))
								{
									if (this.is_ntlm_auth && this.authCompleted && this.webResponse != null && this.webResponse.StatusCode < HttpStatusCode.BadRequest)
									{
										WebConnectionStream webConnectionStream = this.webResponse.GetResponseStream() as WebConnectionStream;
										if (webConnectionStream != null)
										{
											WebConnection connection = webConnectionStream.Connection;
											connection.NtlmAuthenticated = true;
										}
									}
									if (this.writeStream != null)
									{
										this.writeStream.KillBuffer();
									}
									this.haveResponse = true;
									webAsyncResult.SetCompleted(false, this.webResponse);
									webAsyncResult.DoCallback();
								}
								else
								{
									if (this.webResponse != null)
									{
										if (this.is_ntlm_auth)
										{
											this.HandleNtlmAuth(webAsyncResult);
											return;
										}
										this.webResponse.Close();
									}
									this.finished_reading = false;
									this.haveResponse = false;
									this.webResponse = null;
									webAsyncResult.Reset();
									this.servicePoint = this.GetServicePoint();
									this.abortHandler = this.servicePoint.SendRequest(this, this.connectionGroup);
								}
							}
							catch (WebException ex3)
							{
								if (flag)
								{
									this.saved_exc = ex3;
									this.haveResponse = true;
								}
								webAsyncResult.SetCompleted(false, ex3);
								webAsyncResult.DoCallback();
							}
							catch (Exception ex4)
							{
								ex = new WebException(ex4.Message, ex4, WebExceptionStatus.ProtocolError, null);
								if (flag)
								{
									this.saved_exc = ex;
									this.haveResponse = true;
								}
								webAsyncResult.SetCompleted(false, ex);
								webAsyncResult.DoCallback();
							}
						}
					}
				}
			}
		}

		private bool CheckAuthorization(WebResponse response, HttpStatusCode code)
		{
			this.authCompleted = false;
			if (code == HttpStatusCode.Unauthorized && this.credentials == null)
			{
				return false;
			}
			bool flag = code == HttpStatusCode.ProxyAuthenticationRequired;
			if (flag && (this.proxy == null || this.proxy.Credentials == null))
			{
				return false;
			}
			string[] values = response.Headers.GetValues((!flag) ? "WWW-Authenticate" : "Proxy-Authenticate");
			if (values == null || values.Length == 0)
			{
				return false;
			}
			ICredentials credentials2;
			if (!flag)
			{
				ICredentials credentials = this.credentials;
				credentials2 = credentials;
			}
			else
			{
				credentials2 = this.proxy.Credentials;
			}
			ICredentials credentials3 = credentials2;
			Authorization authorization = null;
			foreach (string text in values)
			{
				authorization = AuthenticationManager.Authenticate(text, this, credentials3);
				if (authorization != null)
				{
					break;
				}
			}
			if (authorization == null)
			{
				return false;
			}
			this.webHeaders[(!flag) ? "Authorization" : "Proxy-Authorization"] = authorization.Message;
			this.authCompleted = authorization.Complete;
			this.is_ntlm_auth = authorization.Module.AuthenticationType == "NTLM";
			return true;
		}

		private bool CheckFinalStatus(WebAsyncResult result)
		{
			if (result.GotException)
			{
				throw result.Exception;
			}
			Exception ex = result.Exception;
			this.bodyBuffer = null;
			HttpWebResponse response = result.Response;
			WebExceptionStatus webExceptionStatus = WebExceptionStatus.ProtocolError;
			HttpStatusCode httpStatusCode = (HttpStatusCode)0;
			if (ex == null && this.webResponse != null)
			{
				httpStatusCode = this.webResponse.StatusCode;
				if (!this.authCompleted && ((httpStatusCode == HttpStatusCode.Unauthorized && this.credentials != null) || (this.ProxyQuery && httpStatusCode == HttpStatusCode.ProxyAuthenticationRequired)) && !this.usedPreAuth && this.CheckAuthorization(this.webResponse, httpStatusCode))
				{
					if (this.InternalAllowBuffering)
					{
						this.bodyBuffer = this.writeStream.WriteBuffer;
						this.bodyBufferLength = this.writeStream.WriteBufferLength;
						return true;
					}
					if (this.method != "PUT" && this.method != "POST")
					{
						return true;
					}
					this.writeStream.InternalClose();
					this.writeStream = null;
					this.webResponse.Close();
					this.webResponse = null;
					throw new WebException("This request requires buffering of data for authentication or redirection to be sucessful.");
				}
				else if (httpStatusCode >= HttpStatusCode.BadRequest)
				{
					string text = string.Format("The remote server returned an error: ({0}) {1}.", (int)httpStatusCode, this.webResponse.StatusDescription);
					ex = new WebException(text, null, webExceptionStatus, this.webResponse);
					this.webResponse.ReadAll();
				}
				else if (httpStatusCode == HttpStatusCode.NotModified && this.allowAutoRedirect)
				{
					string text2 = string.Format("The remote server returned an error: ({0}) {1}.", (int)httpStatusCode, this.webResponse.StatusDescription);
					ex = new WebException(text2, null, webExceptionStatus, this.webResponse);
				}
				else if (httpStatusCode >= HttpStatusCode.MultipleChoices && this.allowAutoRedirect && this.redirects >= this.maxAutoRedirect)
				{
					ex = new WebException("Max. redirections exceeded.", null, webExceptionStatus, this.webResponse);
					this.webResponse.ReadAll();
				}
			}
			if (ex == null)
			{
				bool flag = false;
				int num = (int)httpStatusCode;
				if (this.allowAutoRedirect && num >= 300)
				{
					if (this.InternalAllowBuffering && this.writeStream.WriteBufferLength > 0)
					{
						this.bodyBuffer = this.writeStream.WriteBuffer;
						this.bodyBufferLength = this.writeStream.WriteBufferLength;
					}
					flag = this.Redirect(result, httpStatusCode);
				}
				if (response != null && num >= 300 && num != 304)
				{
					response.ReadAll();
				}
				return flag;
			}
			if (this.writeStream != null)
			{
				this.writeStream.InternalClose();
				this.writeStream = null;
			}
			this.webResponse = null;
			throw ex;
		}

		private global::System.Uri requestUri;

		private global::System.Uri actualUri;

		private bool hostChanged;

		private bool allowAutoRedirect = true;

		private bool allowBuffering = true;

		private global::System.Security.Cryptography.X509Certificates.X509CertificateCollection certificates;

		private string connectionGroup;

		private long contentLength = -1L;

		private HttpContinueDelegate continueDelegate;

		private CookieContainer cookieContainer;

		private ICredentials credentials;

		private bool haveResponse;

		private bool haveRequest;

		private bool requestSent;

		private WebHeaderCollection webHeaders = new WebHeaderCollection(true);

		private bool keepAlive = true;

		private int maxAutoRedirect = 50;

		private string mediaType = string.Empty;

		private string method = "GET";

		private string initialMethod = "GET";

		private bool pipelined = true;

		private bool preAuthenticate;

		private bool usedPreAuth;

		private Version version = HttpVersion.Version11;

		private Version actualVersion;

		private IWebProxy proxy;

		private bool sendChunked;

		private ServicePoint servicePoint;

		private int timeout = 100000;

		private WebConnectionStream writeStream;

		private HttpWebResponse webResponse;

		private WebAsyncResult asyncWrite;

		private WebAsyncResult asyncRead;

		private EventHandler abortHandler;

		private int aborted;

		private bool gotRequestStream;

		private int redirects;

		private bool expectContinue;

		private bool authCompleted;

		private byte[] bodyBuffer;

		private int bodyBufferLength;

		private bool getResponseCalled;

		private Exception saved_exc;

		private object locker = new object();

		private bool is_ntlm_auth;

		private bool finished_reading;

		internal WebConnection WebConnection;

		private DecompressionMethods auto_decomp;

		private int maxResponseHeadersLength;

		private static int defaultMaxResponseHeadersLength = 65536;

		private int readWriteTimeout = 300000;

		private bool unsafe_auth_blah;
	}
}
