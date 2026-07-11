using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net.Cache;
using System.Net.Security;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using Mono.Security.Interface;
using Unity;

namespace System.Net
{
	[Serializable]
	public class HttpWebRequest : WebRequest, ISerializable
	{
		static HttpWebRequest()
		{
			NetConfig netConfig = ConfigurationSettings.GetConfig("system.net/settings") as NetConfig;
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

		internal HttpWebRequest(Uri uri)
		{
			this.allowAutoRedirect = true;
			this.allowBuffering = true;
			this.contentLength = -1L;
			this.keepAlive = true;
			this.maxAutoRedirect = 50;
			this.mediaType = string.Empty;
			this.method = "GET";
			this.initialMethod = "GET";
			this.pipelined = true;
			this.version = HttpVersion.Version11;
			this.timeout = 100000;
			this.locker = new object();
			this.readWriteTimeout = 300000;
			base..ctor();
			this.requestUri = uri;
			this.actualUri = uri;
			this.proxy = WebRequest.InternalDefaultWebProxy;
			this.webHeaders = new WebHeaderCollection(WebHeaderCollectionType.HttpWebRequest);
			this.ThrowOnError = true;
			this.ResetAuthorization();
		}

		internal HttpWebRequest(Uri uri, MonoTlsProvider tlsProvider, MonoTlsSettings settings = null)
			: this(uri)
		{
			this.tlsProvider = tlsProvider;
			this.tlsSettings = settings;
		}

		[Obsolete("Serialization is obsoleted for this type", false)]
		protected HttpWebRequest(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			this.allowAutoRedirect = true;
			this.allowBuffering = true;
			this.contentLength = -1L;
			this.keepAlive = true;
			this.maxAutoRedirect = 50;
			this.mediaType = string.Empty;
			this.method = "GET";
			this.initialMethod = "GET";
			this.pipelined = true;
			this.version = HttpVersion.Version11;
			this.timeout = 100000;
			this.locker = new object();
			this.readWriteTimeout = 300000;
			base..ctor();
			this.requestUri = (Uri)serializationInfo.GetValue("requestUri", typeof(Uri));
			this.actualUri = (Uri)serializationInfo.GetValue("actualUri", typeof(Uri));
			this.allowAutoRedirect = serializationInfo.GetBoolean("allowAutoRedirect");
			this.allowBuffering = serializationInfo.GetBoolean("allowBuffering");
			this.certificates = (X509CertificateCollection)serializationInfo.GetValue("certificates", typeof(X509CertificateCollection));
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
			this.host = serializationInfo.GetString("host");
			this.ResetAuthorization();
		}

		private void ResetAuthorization()
		{
			this.auth_state = new HttpWebRequest.AuthorizationState(this, false);
			this.proxy_auth_state = new HttpWebRequest.AuthorizationState(this, true);
		}

		private void SetSpecialHeaders(string HeaderName, string value)
		{
			value = WebHeaderCollection.CheckBadChars(value, true);
			this.webHeaders.RemoveInternal(HeaderName);
			if (value.Length != 0)
			{
				this.webHeaders.AddInternal(HeaderName, value);
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
				this.SetSpecialHeaders("Accept", value);
			}
		}

		public Uri Address
		{
			get
			{
				return this.actualUri;
			}
			internal set
			{
				this.actualUri = value;
			}
		}

		public virtual bool AllowAutoRedirect
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

		public virtual bool AllowWriteStreamBuffering
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

		public virtual bool AllowReadStreamBuffering
		{
			get
			{
				return false;
			}
			set
			{
				if (value)
				{
					throw new InvalidOperationException();
				}
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
				return this.allowBuffering && this.MethodWithBuffer;
			}
		}

		private bool MethodWithBuffer
		{
			get
			{
				return this.method != "HEAD" && this.method != "GET" && this.method != "MKCOL" && this.method != "CONNECT" && this.method != "TRACE";
			}
		}

		internal MonoTlsProvider TlsProvider
		{
			get
			{
				return this.tlsProvider;
			}
		}

		internal MonoTlsSettings TlsSettings
		{
			get
			{
				return this.tlsSettings;
			}
		}

		public X509CertificateCollection ClientCertificates
		{
			get
			{
				if (this.certificates == null)
				{
					this.certificates = new X509CertificateCollection();
				}
				return this.certificates;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.certificates = value;
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
				if (string.IsNullOrEmpty(value))
				{
					this.webHeaders.RemoveInternal("Connection");
					return;
				}
				string text = value.ToLowerInvariant();
				if (text.Contains("keep-alive") || text.Contains("close"))
				{
					throw new ArgumentException("Keep-Alive and Close may not be set with this property");
				}
				if (this.keepAlive)
				{
					value += ", Keep-Alive";
				}
				this.webHeaders.CheckUpdate("Connection", value);
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
				this.haveContentLength = true;
			}
		}

		internal long InternalContentLength
		{
			set
			{
				this.contentLength = value;
			}
		}

		internal bool ThrowOnError { get; set; }

		public override string ContentType
		{
			get
			{
				return this.webHeaders["Content-Type"];
			}
			set
			{
				this.SetSpecialHeaders("Content-Type", value);
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

		public virtual CookieContainer CookieContainer
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

		public DateTime Date
		{
			get
			{
				string text = this.webHeaders["Date"];
				if (text == null)
				{
					return DateTime.MinValue;
				}
				return DateTime.ParseExact(text, "r", CultureInfo.InvariantCulture).ToLocalTime();
			}
			set
			{
				this.SetDateHeaderHelper("Date", value);
			}
		}

		private void SetDateHeaderHelper(string headerName, DateTime dateTime)
		{
			if (dateTime == DateTime.MinValue)
			{
				this.SetSpecialHeaders(headerName, null);
				return;
			}
			this.SetSpecialHeaders(headerName, HttpProtocolUtils.date2string(dateTime));
		}

		[MonoTODO]
		public new static RequestCachePolicy DefaultCachePolicy
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

		[MonoTODO]
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
				this.webHeaders.CheckUpdate("Expect", value);
			}
		}

		public virtual bool HaveResponse
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
				WebHeaderCollection webHeaderCollection = new WebHeaderCollection(WebHeaderCollectionType.HttpWebRequest);
				foreach (string text in value.AllKeys)
				{
					webHeaderCollection.Add(text, value[text]);
				}
				this.webHeaders = webHeaderCollection;
			}
		}

		public string Host
		{
			get
			{
				if (this.host == null)
				{
					return this.actualUri.Authority;
				}
				return this.host;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (!HttpWebRequest.CheckValidHost(this.actualUri.Scheme, value))
				{
					throw new ArgumentException("Invalid host: " + value);
				}
				this.host = value;
			}
		}

		private static bool CheckValidHost(string scheme, string val)
		{
			IPAddress ipaddress;
			return val.Length != 0 && val[0] != '.' && val.IndexOf('/') < 0 && (IPAddress.TryParse(val, out ipaddress) || Uri.IsWellFormedUriString(scheme + "://" + val + "/", UriKind.Absolute));
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

		[MonoTODO("Use this")]
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

		[MonoTODO("Use this")]
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

		[MonoTODO]
		public int ContinueTimeout
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
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
				if (value == null || value.Trim() == "")
				{
					throw new ArgumentException("not a valid method");
				}
				this.method = value.ToUpperInvariant();
				if (this.method != "HEAD" && this.method != "GET" && this.method != "POST" && this.method != "PUT" && this.method != "DELETE" && this.method != "CONNECT" && this.method != "TRACE" && this.method != "MKCOL")
				{
					this.method = value;
				}
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
				this.force_version = true;
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
				this.GetServicePoint();
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

		public override Uri RequestUri
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

		internal ServicePoint ServicePointNoLock
		{
			get
			{
				return this.servicePoint;
			}
		}

		public virtual bool SupportsCookieContainer
		{
			get
			{
				return true;
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
				this.webHeaders.CheckUpdate("Transfer-Encoding", value);
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
				this.Credentials = (value ? CredentialCache.DefaultCredentials : null);
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

		internal Uri AuthUri
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

		internal ServerCertValidationCallback ServerCertValidationCallback
		{
			get
			{
				return this.certValidationCallback;
			}
		}

		public RemoteCertificateValidationCallback ServerCertificateValidationCallback
		{
			get
			{
				if (this.certValidationCallback == null)
				{
					return null;
				}
				return this.certValidationCallback.ValidationCallback;
			}
			set
			{
				if (value == null)
				{
					this.certValidationCallback = null;
					return;
				}
				this.certValidationCallback = new ServerCertValidationCallback(value);
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
			this.AddRange("bytes", (long)range);
		}

		public void AddRange(int from, int to)
		{
			this.AddRange("bytes", (long)from, (long)to);
		}

		public void AddRange(string rangeSpecifier, int range)
		{
			this.AddRange(rangeSpecifier, (long)range);
		}

		public void AddRange(string rangeSpecifier, int from, int to)
		{
			this.AddRange(rangeSpecifier, (long)from, (long)to);
		}

		public void AddRange(long range)
		{
			this.AddRange("bytes", range);
		}

		public void AddRange(long from, long to)
		{
			this.AddRange("bytes", from, to);
		}

		public void AddRange(string rangeSpecifier, long range)
		{
			if (rangeSpecifier == null)
			{
				throw new ArgumentNullException("rangeSpecifier");
			}
			if (!WebHeaderCollection.IsValidToken(rangeSpecifier))
			{
				throw new ArgumentException("Invalid range specifier", "rangeSpecifier");
			}
			string text = this.webHeaders["Range"];
			if (text == null)
			{
				text = rangeSpecifier + "=";
			}
			else
			{
				if (string.Compare(text.Substring(0, text.IndexOf('=')), rangeSpecifier, StringComparison.OrdinalIgnoreCase) != 0)
				{
					throw new InvalidOperationException("A different range specifier is already in use");
				}
				text += ",";
			}
			string text2 = range.ToString(CultureInfo.InvariantCulture);
			if (range < 0L)
			{
				text = text + "0" + text2;
			}
			else
			{
				text = text + text2 + "-";
			}
			this.webHeaders.ChangeInternal("Range", text);
		}

		public void AddRange(string rangeSpecifier, long from, long to)
		{
			if (rangeSpecifier == null)
			{
				throw new ArgumentNullException("rangeSpecifier");
			}
			if (!WebHeaderCollection.IsValidToken(rangeSpecifier))
			{
				throw new ArgumentException("Invalid range specifier", "rangeSpecifier");
			}
			if (from > to || from < 0L)
			{
				throw new ArgumentOutOfRangeException("from");
			}
			if (to < 0L)
			{
				throw new ArgumentOutOfRangeException("to");
			}
			string text = this.webHeaders["Range"];
			if (text == null)
			{
				text = rangeSpecifier + "=";
			}
			else
			{
				text += ",";
			}
			text = string.Format("{0}{1}-{2}", text, from, to);
			this.webHeaders.ChangeInternal("Range", text);
		}

		public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
		{
			if (this.Aborted)
			{
				throw new WebException("The request was canceled.", WebExceptionStatus.RequestCanceled);
			}
			bool flag = !(this.method == "GET") && !(this.method == "CONNECT") && !(this.method == "HEAD") && !(this.method == "TRACE");
			if (this.method == null || !flag)
			{
				throw new ProtocolViolationException("Cannot send data when method is: " + this.method);
			}
			if (this.contentLength == -1L && !this.sendChunked && !this.allowBuffering && this.KeepAlive)
			{
				throw new ProtocolViolationException("Content-Length not set");
			}
			string transferEncoding = this.TransferEncoding;
			if (!this.sendChunked && transferEncoding != null && transferEncoding.Trim() != "")
			{
				throw new ProtocolViolationException("SendChunked should be true.");
			}
			object obj = this.locker;
			IAsyncResult asyncResult;
			lock (obj)
			{
				if (this.getResponseCalled)
				{
					throw new InvalidOperationException("The operation cannot be performed once the request has been submitted.");
				}
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
					IAsyncResult asyncResult2 = this.asyncWrite;
					if (!this.requestSent)
					{
						this.requestSent = true;
						this.redirects = 0;
						this.servicePoint = this.GetServicePoint();
						this.abortHandler = this.servicePoint.SendRequest(this, this.connectionGroup);
					}
					asyncResult = asyncResult2;
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

		[MonoTODO]
		public Stream GetRequestStream(out TransportContext context)
		{
			throw new NotImplementedException();
		}

		private bool CheckIfForceWrite(SimpleAsyncResult result)
		{
			if (this.writeStream == null || this.writeStream.RequestWritten || !this.InternalAllowBuffering)
			{
				return false;
			}
			if (this.contentLength < 0L && this.writeStream.CanWrite && this.writeStream.WriteBufferLength < 0)
			{
				return false;
			}
			if (this.contentLength < 0L && this.writeStream.WriteBufferLength >= 0)
			{
				this.InternalContentLength = (long)this.writeStream.WriteBufferLength;
			}
			return ((long)this.writeStream.WriteBufferLength == this.contentLength || (this.contentLength == -1L && !this.writeStream.CanWrite)) && this.writeStream.WriteRequestAsync(result);
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
			if (!this.sendChunked && transferEncoding != null && transferEncoding.Trim() != "")
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
			this.asyncRead = new WebAsyncResult(this, callback, state);
			WebAsyncResult aread = this.asyncRead;
			this.initialMethod = this.method;
			SimpleAsyncResult.RunWithLock(this.locker, new Func<SimpleAsyncResult, bool>(this.CheckIfForceWrite), delegate(SimpleAsyncResult inner)
			{
				bool completedSynchronouslyPeek = inner.CompletedSynchronouslyPeek;
				if (inner.GotException)
				{
					aread.SetCompleted(completedSynchronouslyPeek, inner.Exception);
					aread.DoCallback();
					return;
				}
				if (this.haveResponse)
				{
					Exception ex = this.saved_exc;
					if (this.webResponse != null)
					{
						if (ex == null)
						{
							aread.SetCompleted(completedSynchronouslyPeek, this.webResponse);
						}
						else
						{
							aread.SetCompleted(completedSynchronouslyPeek, ex);
						}
						aread.DoCallback();
						return;
					}
					if (ex != null)
					{
						aread.SetCompleted(completedSynchronouslyPeek, ex);
						aread.DoCallback();
						return;
					}
				}
				if (this.requestSent)
				{
					return;
				}
				try
				{
					this.requestSent = true;
					this.redirects = 0;
					this.servicePoint = this.GetServicePoint();
					this.abortHandler = this.servicePoint.SendRequest(this, this.connectionGroup);
				}
				catch (Exception ex2)
				{
					aread.SetCompleted(completedSynchronouslyPeek, ex2);
					aread.DoCallback();
				}
			});
			return aread;
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

		public Stream EndGetRequestStream(IAsyncResult asyncResult, out TransportContext context)
		{
			context = null;
			return this.EndGetRequestStream(asyncResult);
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

		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			this.GetObjectData(serializationInfo, streamingContext);
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		protected override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			serializationInfo.AddValue("requestUri", this.requestUri, typeof(Uri));
			serializationInfo.AddValue("actualUri", this.actualUri, typeof(Uri));
			serializationInfo.AddValue("allowAutoRedirect", this.allowAutoRedirect);
			serializationInfo.AddValue("allowBuffering", this.allowBuffering);
			serializationInfo.AddValue("certificates", this.certificates, typeof(X509CertificateCollection));
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
			serializationInfo.AddValue("host", this.host);
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

		private void RewriteRedirectToGet()
		{
			this.method = "GET";
			this.webHeaders.RemoveInternal("Transfer-Encoding");
			this.sendChunked = false;
		}

		private bool Redirect(WebAsyncResult result, HttpStatusCode code, WebResponse response)
		{
			this.redirects++;
			Exception ex = null;
			string text = null;
			switch (code)
			{
			case HttpStatusCode.MultipleChoices:
				ex = new WebException("Ambiguous redirect.");
				goto IL_0094;
			case HttpStatusCode.MovedPermanently:
			case HttpStatusCode.Found:
				if (this.method == "POST")
				{
					this.RewriteRedirectToGet();
					goto IL_0094;
				}
				goto IL_0094;
			case HttpStatusCode.SeeOther:
				this.RewriteRedirectToGet();
				goto IL_0094;
			case HttpStatusCode.NotModified:
				return false;
			case HttpStatusCode.UseProxy:
				ex = new NotImplementedException("Proxy support not available.");
				goto IL_0094;
			case HttpStatusCode.TemporaryRedirect:
				goto IL_0094;
			}
			ex = new ProtocolViolationException("Invalid status code: " + (int)code);
			IL_0094:
			if (this.method != "GET" && !this.InternalAllowBuffering && (this.writeStream.WriteBufferLength > 0 || this.contentLength > 0L))
			{
				ex = new WebException("The request requires buffering data to succeed.", null, WebExceptionStatus.ProtocolError, this.webResponse);
			}
			if (ex != null)
			{
				throw ex;
			}
			if (this.AllowWriteStreamBuffering || this.method == "GET")
			{
				this.contentLength = -1L;
			}
			text = this.webResponse.Headers["Location"];
			if (text == null)
			{
				throw new WebException("No Location header found for " + (int)code, WebExceptionStatus.ProtocolError);
			}
			Uri uri = this.actualUri;
			try
			{
				this.actualUri = new Uri(this.actualUri, text);
			}
			catch (Exception)
			{
				throw new WebException(string.Format("Invalid URL ({0}) for {1}", text, (int)code), WebExceptionStatus.ProtocolError);
			}
			this.hostChanged = this.actualUri.Scheme != uri.Scheme || this.Host != uri.Authority;
			return true;
		}

		private string GetHeaders()
		{
			bool flag = false;
			if (this.sendChunked)
			{
				flag = true;
				this.webHeaders.ChangeInternal("Transfer-Encoding", "chunked");
				this.webHeaders.RemoveInternal("Content-Length");
			}
			else if (this.contentLength != -1L)
			{
				if (this.auth_state.NtlmAuthState == HttpWebRequest.NtlmAuthState.Challenge || this.proxy_auth_state.NtlmAuthState == HttpWebRequest.NtlmAuthState.Challenge)
				{
					if (this.haveContentLength || this.gotRequestStream || this.contentLength > 0L)
					{
						this.webHeaders.SetInternal("Content-Length", "0");
					}
					else
					{
						this.webHeaders.RemoveInternal("Content-Length");
					}
				}
				else
				{
					if (this.contentLength > 0L)
					{
						flag = true;
					}
					if (this.haveContentLength || this.gotRequestStream || this.contentLength > 0L)
					{
						this.webHeaders.SetInternal("Content-Length", this.contentLength.ToString());
					}
				}
				this.webHeaders.RemoveInternal("Transfer-Encoding");
			}
			else
			{
				this.webHeaders.RemoveInternal("Content-Length");
			}
			if (this.actualVersion == HttpVersion.Version11 && flag && this.servicePoint.SendContinue)
			{
				this.webHeaders.ChangeInternal("Expect", "100-continue");
				this.expectContinue = true;
			}
			else
			{
				this.webHeaders.RemoveInternal("Expect");
				this.expectContinue = false;
			}
			bool proxyQuery = this.ProxyQuery;
			string text = (proxyQuery ? "Proxy-Connection" : "Connection");
			this.webHeaders.RemoveInternal((!proxyQuery) ? "Proxy-Connection" : "Connection");
			Version protocolVersion = this.servicePoint.ProtocolVersion;
			bool flag2 = protocolVersion == null || protocolVersion == HttpVersion.Version10;
			if (this.keepAlive && (this.version == HttpVersion.Version10 || flag2))
			{
				if (this.webHeaders[text] == null || this.webHeaders[text].IndexOf("keep-alive", StringComparison.OrdinalIgnoreCase) == -1)
				{
					this.webHeaders.ChangeInternal(text, "keep-alive");
				}
			}
			else if (!this.keepAlive && this.version == HttpVersion.Version11)
			{
				this.webHeaders.ChangeInternal(text, "close");
			}
			this.webHeaders.SetInternal("Host", this.Host);
			if (this.cookieContainer != null)
			{
				string cookieHeader = this.cookieContainer.GetCookieHeader(this.actualUri);
				if (cookieHeader != "")
				{
					this.webHeaders.ChangeInternal("Cookie", cookieHeader);
				}
				else
				{
					this.webHeaders.RemoveInternal("Cookie");
				}
			}
			string text2 = null;
			if ((this.auto_decomp & DecompressionMethods.GZip) != DecompressionMethods.None)
			{
				text2 = "gzip";
			}
			if ((this.auto_decomp & DecompressionMethods.Deflate) != DecompressionMethods.None)
			{
				text2 = ((text2 != null) ? "gzip, deflate" : "deflate");
			}
			if (text2 != null)
			{
				this.webHeaders.ChangeInternal("Accept-Encoding", text2);
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
			ICredentials credentials = ((!flag || this.credentials != null) ? this.credentials : this.proxy.Credentials);
			Authorization authorization = AuthenticationManager.PreAuthenticate(this, credentials);
			if (authorization == null)
			{
				return;
			}
			this.webHeaders.RemoveInternal("Proxy-Authorization");
			this.webHeaders.RemoveInternal("Authorization");
			string text = ((flag && this.credentials == null) ? "Proxy-Authorization" : "Authorization");
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
					ex = new WebException("Error: " + status, status);
				}
				else
				{
					ex = exc as WebException;
					if (ex == null)
					{
						ex = new WebException(string.Format("Error: {0} ({1})", status, exc.Message), status, WebExceptionInternalStatus.RequestFatal, exc);
					}
				}
				webAsyncResult.SetCompleted(false, ex);
				webAsyncResult.DoCallback();
			}
		}

		internal byte[] GetRequestHeaders()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text;
			if (!this.ProxyQuery)
			{
				text = this.actualUri.PathAndQuery;
			}
			else
			{
				text = string.Format("{0}://{1}{2}", this.actualUri.Scheme, this.Host, this.actualUri.PathAndQuery);
			}
			if (!this.force_version && this.servicePoint.ProtocolVersion != null && this.servicePoint.ProtocolVersion < this.version)
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
			return Encoding.UTF8.GetBytes(text2);
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
			this.writeStream.SetHeadersAsync(false, delegate(SimpleAsyncResult result)
			{
				if (result.GotException)
				{
					this.SetWriteStreamError(result.Exception);
					return;
				}
				this.haveRequest = true;
				this.SetWriteStreamInner(delegate(SimpleAsyncResult inner)
				{
					if (inner.GotException)
					{
						this.SetWriteStreamError(inner.Exception);
						return;
					}
					if (this.asyncWrite != null)
					{
						this.asyncWrite.SetCompleted(inner.CompletedSynchronouslyPeek, this.writeStream);
						this.asyncWrite.DoCallback();
						this.asyncWrite = null;
					}
				});
			});
		}

		private void SetWriteStreamInner(SimpleAsyncCallback callback)
		{
			SimpleAsyncResult.Run(delegate(SimpleAsyncResult result)
			{
				if (this.bodyBuffer != null)
				{
					if (this.auth_state.NtlmAuthState != HttpWebRequest.NtlmAuthState.Challenge && this.proxy_auth_state.NtlmAuthState != HttpWebRequest.NtlmAuthState.Challenge)
					{
						this.writeStream.Write(this.bodyBuffer, 0, this.bodyBufferLength);
						this.bodyBuffer = null;
						this.writeStream.Close();
					}
				}
				else if (this.MethodWithBuffer && this.getResponseCalled && !this.writeStream.RequestWritten)
				{
					return this.writeStream.WriteRequestAsync(result);
				}
				return false;
			}, callback);
		}

		private void SetWriteStreamError(Exception exc)
		{
			WebException ex = exc as WebException;
			if (ex != null)
			{
				this.SetWriteStreamError(ex.Status, ex);
				return;
			}
			this.SetWriteStreamError(WebExceptionStatus.SendFailure, exc);
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
				if (this.allowBuffering || this.sendChunked || this.writeStream.totalWritten >= this.contentLength)
				{
					this.webResponse.ReadAll();
					return;
				}
				this.writeStream.IgnoreIOErrors = true;
			}
		}

		private bool HandleNtlmAuth(WebAsyncResult r)
		{
			bool flag = this.webResponse.StatusCode == HttpStatusCode.ProxyAuthenticationRequired;
			if ((flag ? this.proxy_auth_state.NtlmAuthState : this.auth_state.NtlmAuthState) == HttpWebRequest.NtlmAuthState.None)
			{
				return false;
			}
			WebConnectionStream webConnectionStream = this.webResponse.GetResponseStream() as WebConnectionStream;
			if (webConnectionStream != null)
			{
				WebConnection connection = webConnectionStream.Connection;
				connection.PriorityRequest = this;
				ICredentials credentials = ((!flag || this.proxy == null) ? this.credentials : this.proxy.Credentials);
				if (credentials != null)
				{
					connection.NtlmCredential = credentials.GetCredential(this.requestUri, "NTLM");
					connection.UnsafeAuthenticatedConnectionSharing = this.unsafe_auth_blah;
				}
			}
			r.Reset();
			this.finished_reading = false;
			this.haveResponse = false;
			this.webResponse.ReadAll();
			this.webResponse = null;
			return true;
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
						this.CheckSendError(data);
						if (this.saved_exc != null)
						{
							ex = (WebException)this.saved_exc;
						}
					}
					WebAsyncResult webAsyncResult = this.asyncRead;
					bool flag2 = false;
					if (webAsyncResult == null && this.webResponse != null)
					{
						flag2 = true;
						webAsyncResult = new WebAsyncResult(null, null);
						webAsyncResult.SetCompleted(false, this.webResponse);
					}
					if (webAsyncResult != null)
					{
						if (ex != null)
						{
							this.haveResponse = true;
							if (!webAsyncResult.IsCompleted)
							{
								webAsyncResult.SetCompleted(false, ex);
							}
							webAsyncResult.DoCallback();
						}
						else
						{
							bool flag3 = this.ProxyQuery && this.proxy != null && !this.proxy.IsBypassed(this.actualUri);
							try
							{
								if (!this.CheckFinalStatus(webAsyncResult))
								{
									if ((flag3 ? this.proxy_auth_state.IsNtlmAuthenticated : this.auth_state.IsNtlmAuthenticated) && this.webResponse != null && this.webResponse.StatusCode < HttpStatusCode.BadRequest)
									{
										WebConnectionStream webConnectionStream = this.webResponse.GetResponseStream() as WebConnectionStream;
										if (webConnectionStream != null)
										{
											webConnectionStream.Connection.NtlmAuthenticated = true;
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
									if (this.sendChunked)
									{
										this.sendChunked = false;
										this.webHeaders.RemoveInternal("Transfer-Encoding");
									}
									if (this.webResponse != null)
									{
										if (this.HandleNtlmAuth(webAsyncResult))
										{
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
								if (flag2)
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
								if (flag2)
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
			if (code != HttpStatusCode.ProxyAuthenticationRequired)
			{
				return this.auth_state.CheckAuthorization(response, code);
			}
			return this.proxy_auth_state.CheckAuthorization(response, code);
		}

		private bool CheckFinalStatus(WebAsyncResult result)
		{
			if (result.GotException)
			{
				this.bodyBuffer = null;
				throw result.Exception;
			}
			Exception ex = result.Exception;
			HttpWebResponse response = result.Response;
			WebExceptionStatus webExceptionStatus = WebExceptionStatus.ProtocolError;
			HttpStatusCode httpStatusCode = (HttpStatusCode)0;
			if (ex == null && this.webResponse != null)
			{
				httpStatusCode = this.webResponse.StatusCode;
				if (((!this.auth_state.IsCompleted && httpStatusCode == HttpStatusCode.Unauthorized && this.credentials != null) || (this.ProxyQuery && !this.proxy_auth_state.IsCompleted && httpStatusCode == HttpStatusCode.ProxyAuthenticationRequired)) && !this.usedPreAuth && this.CheckAuthorization(this.webResponse, httpStatusCode))
				{
					if (this.MethodWithBuffer)
					{
						if (this.AllowWriteStreamBuffering)
						{
							if (this.writeStream.WriteBufferLength > 0)
							{
								this.bodyBuffer = this.writeStream.WriteBuffer;
								this.bodyBufferLength = this.writeStream.WriteBufferLength;
							}
							return true;
						}
						if (this.ResendContentFactory != null)
						{
							using (MemoryStream memoryStream = new MemoryStream())
							{
								this.ResendContentFactory(memoryStream);
								this.bodyBuffer = memoryStream.ToArray();
								this.bodyBufferLength = this.bodyBuffer.Length;
							}
							return true;
						}
					}
					else if (this.method != "PUT" && this.method != "POST")
					{
						this.bodyBuffer = null;
						return true;
					}
					if (!this.ThrowOnError)
					{
						return false;
					}
					this.writeStream.InternalClose();
					this.writeStream = null;
					this.webResponse.Close();
					this.webResponse = null;
					this.bodyBuffer = null;
					throw new WebException("This request requires buffering of data for authentication or redirection to be sucessful.");
				}
				else
				{
					this.bodyBuffer = null;
					if (httpStatusCode >= HttpStatusCode.BadRequest)
					{
						ex = new WebException(string.Format("The remote server returned an error: ({0}) {1}.", (int)httpStatusCode, this.webResponse.StatusDescription), null, webExceptionStatus, this.webResponse);
						this.webResponse.ReadAll();
					}
					else if (httpStatusCode == HttpStatusCode.NotModified && this.allowAutoRedirect)
					{
						ex = new WebException(string.Format("The remote server returned an error: ({0}) {1}.", (int)httpStatusCode, this.webResponse.StatusDescription), null, webExceptionStatus, this.webResponse);
					}
					else if (httpStatusCode >= HttpStatusCode.MultipleChoices && this.allowAutoRedirect && this.redirects >= this.maxAutoRedirect)
					{
						ex = new WebException("Max. redirections exceeded.", null, webExceptionStatus, this.webResponse);
						this.webResponse.ReadAll();
					}
				}
			}
			this.bodyBuffer = null;
			if (ex == null)
			{
				bool flag = false;
				int num = (int)httpStatusCode;
				if (this.allowAutoRedirect && num >= 300)
				{
					flag = this.Redirect(result, httpStatusCode, this.webResponse);
					if (this.InternalAllowBuffering && this.writeStream.WriteBufferLength > 0)
					{
						this.bodyBuffer = this.writeStream.WriteBuffer;
						this.bodyBufferLength = this.writeStream.WriteBufferLength;
					}
					if (flag && !this.unsafe_auth_blah)
					{
						this.auth_state.Reset();
						this.proxy_auth_state.Reset();
					}
				}
				if (response != null && num >= 300 && num != 304)
				{
					response.ReadAll();
				}
				return flag;
			}
			if (!this.ThrowOnError)
			{
				return false;
			}
			if (this.writeStream != null)
			{
				this.writeStream.InternalClose();
				this.writeStream = null;
			}
			this.webResponse = null;
			throw ex;
		}

		internal bool ReuseConnection { get; set; }

		internal static StringBuilder GenerateConnectionGroup(string connectionGroupName, bool unsafeConnectionGroup, bool isInternalGroup)
		{
			StringBuilder stringBuilder = new StringBuilder(connectionGroupName);
			stringBuilder.Append(unsafeConnectionGroup ? "U>" : "S>");
			if (isInternalGroup)
			{
				stringBuilder.Append("I>");
			}
			return stringBuilder;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
		public HttpWebRequest()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private Uri requestUri;

		private Uri actualUri;

		private bool hostChanged;

		private bool allowAutoRedirect;

		private bool allowBuffering;

		private X509CertificateCollection certificates;

		private string connectionGroup;

		private bool haveContentLength;

		private long contentLength;

		private HttpContinueDelegate continueDelegate;

		private CookieContainer cookieContainer;

		private ICredentials credentials;

		private bool haveResponse;

		private bool haveRequest;

		private bool requestSent;

		private WebHeaderCollection webHeaders;

		private bool keepAlive;

		private int maxAutoRedirect;

		private string mediaType;

		private string method;

		private string initialMethod;

		private bool pipelined;

		private bool preAuthenticate;

		private bool usedPreAuth;

		private Version version;

		private bool force_version;

		private Version actualVersion;

		private IWebProxy proxy;

		private bool sendChunked;

		private ServicePoint servicePoint;

		private int timeout;

		private WebConnectionStream writeStream;

		private HttpWebResponse webResponse;

		private WebAsyncResult asyncWrite;

		private WebAsyncResult asyncRead;

		private EventHandler abortHandler;

		private int aborted;

		private bool gotRequestStream;

		private int redirects;

		private bool expectContinue;

		private byte[] bodyBuffer;

		private int bodyBufferLength;

		private bool getResponseCalled;

		private Exception saved_exc;

		private object locker;

		private bool finished_reading;

		internal WebConnection WebConnection;

		private DecompressionMethods auto_decomp;

		private int maxResponseHeadersLength;

		private static int defaultMaxResponseHeadersLength = 65536;

		private int readWriteTimeout;

		private MonoTlsProvider tlsProvider;

		private MonoTlsSettings tlsSettings;

		private ServerCertValidationCallback certValidationCallback;

		private HttpWebRequest.AuthorizationState auth_state;

		private HttpWebRequest.AuthorizationState proxy_auth_state;

		private string host;

		[NonSerialized]
		internal Action<Stream> ResendContentFactory;

		private bool unsafe_auth_blah;

		internal WebConnection StoredConnection;

		private enum NtlmAuthState
		{
			None,
			Challenge,
			Response
		}

		private struct AuthorizationState
		{
			public bool IsCompleted
			{
				get
				{
					return this.isCompleted;
				}
			}

			public HttpWebRequest.NtlmAuthState NtlmAuthState
			{
				get
				{
					return this.ntlm_auth_state;
				}
			}

			public bool IsNtlmAuthenticated
			{
				get
				{
					return this.isCompleted && this.ntlm_auth_state > HttpWebRequest.NtlmAuthState.None;
				}
			}

			public AuthorizationState(HttpWebRequest request, bool isProxy)
			{
				this.request = request;
				this.isProxy = isProxy;
				this.isCompleted = false;
				this.ntlm_auth_state = HttpWebRequest.NtlmAuthState.None;
			}

			public bool CheckAuthorization(WebResponse response, HttpStatusCode code)
			{
				this.isCompleted = false;
				if (code == HttpStatusCode.Unauthorized && this.request.credentials == null)
				{
					return false;
				}
				if (this.isProxy != (code == HttpStatusCode.ProxyAuthenticationRequired))
				{
					return false;
				}
				if (this.isProxy && (this.request.proxy == null || this.request.proxy.Credentials == null))
				{
					return false;
				}
				string[] values = response.Headers.GetValues(this.isProxy ? "Proxy-Authenticate" : "WWW-Authenticate");
				if (values == null || values.Length == 0)
				{
					return false;
				}
				ICredentials credentials = ((!this.isProxy) ? this.request.credentials : this.request.proxy.Credentials);
				Authorization authorization = null;
				string[] array = values;
				for (int i = 0; i < array.Length; i++)
				{
					authorization = AuthenticationManager.Authenticate(array[i], this.request, credentials);
					if (authorization != null)
					{
						break;
					}
				}
				if (authorization == null)
				{
					return false;
				}
				this.request.webHeaders[this.isProxy ? "Proxy-Authorization" : "Authorization"] = authorization.Message;
				this.isCompleted = authorization.Complete;
				if (authorization.ModuleAuthenticationType == "NTLM")
				{
					this.ntlm_auth_state++;
				}
				return true;
			}

			public void Reset()
			{
				this.isCompleted = false;
				this.ntlm_auth_state = HttpWebRequest.NtlmAuthState.None;
				this.request.webHeaders.RemoveInternal(this.isProxy ? "Proxy-Authorization" : "Authorization");
			}

			public override string ToString()
			{
				return string.Format("{0}AuthState [{1}:{2}]", this.isProxy ? "Proxy" : "", this.isCompleted, this.ntlm_auth_state);
			}

			private readonly HttpWebRequest request;

			private readonly bool isProxy;

			private bool isCompleted;

			private HttpWebRequest.NtlmAuthState ntlm_auth_state;
		}
	}
}
