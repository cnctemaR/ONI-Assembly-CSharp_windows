using System;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Threading;

namespace System.Net
{
	[Serializable]
	public class FileWebRequest : WebRequest, ISerializable
	{
		internal FileWebRequest(global::System.Uri uri)
		{
			this.uri = uri;
			this.webHeaders = new WebHeaderCollection();
		}

		[Obsolete("Serialization is obsoleted for this type", false)]
		protected FileWebRequest(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			this.webHeaders = (WebHeaderCollection)serializationInfo.GetValue("headers", typeof(WebHeaderCollection));
			this.proxy = (IWebProxy)serializationInfo.GetValue("proxy", typeof(IWebProxy));
			this.uri = (global::System.Uri)serializationInfo.GetValue("uri", typeof(global::System.Uri));
			this.connectionGroup = serializationInfo.GetString("connectionGroupName");
			this.method = serializationInfo.GetString("method");
			this.contentLength = serializationInfo.GetInt64("contentLength");
			this.timeout = serializationInfo.GetInt32("timeout");
			this.fileAccess = (FileAccess)((int)serializationInfo.GetValue("fileAccess", typeof(FileAccess)));
			this.preAuthenticate = serializationInfo.GetBoolean("preauthenticate");
		}

		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			this.GetObjectData(serializationInfo, streamingContext);
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
				if (value < 0L)
				{
					throw new ArgumentException("The Content-Length value must be greater than or equal to zero.", "value");
				}
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
				this.webHeaders["Content-Type"] = value;
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

		public override WebHeaderCollection Headers
		{
			get
			{
				return this.webHeaders;
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
				if (value == null || value.Length == 0)
				{
					throw new ArgumentException("Cannot set null or blank methods on request.", "value");
				}
				this.method = value;
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

		public override IWebProxy Proxy
		{
			get
			{
				return this.proxy;
			}
			set
			{
				this.proxy = value;
			}
		}

		public override global::System.Uri RequestUri
		{
			get
			{
				return this.uri;
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
					throw new ArgumentOutOfRangeException("Timeout can be only set to 'System.Threading.Timeout.Infinite' or a value >= 0.");
				}
				this.timeout = value;
			}
		}

		public override bool UseDefaultCredentials
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

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		[global::System.MonoTODO]
		public override void Abort()
		{
			throw FileWebRequest.GetMustImplement();
		}

		public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
		{
			if (string.Compare("GET", this.method, true) == 0 || string.Compare("HEAD", this.method, true) == 0 || string.Compare("CONNECT", this.method, true) == 0)
			{
				throw new ProtocolViolationException("Cannot send a content-body with this verb-type.");
			}
			lock (this)
			{
				if (this.asyncResponding || this.webResponse != null)
				{
					throw new InvalidOperationException("This operation cannot be performed after the request has been submitted.");
				}
				if (this.requesting)
				{
					throw new InvalidOperationException("Cannot re-call start of asynchronous method while a previous call is still in progress.");
				}
				this.requesting = true;
			}
			FileWebRequest.GetRequestStreamCallback getRequestStreamCallback = new FileWebRequest.GetRequestStreamCallback(this.GetRequestStreamInternal);
			return getRequestStreamCallback.BeginInvoke(callback, state);
		}

		public override Stream EndGetRequestStream(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (!asyncResult.IsCompleted)
			{
				asyncResult.AsyncWaitHandle.WaitOne();
			}
			AsyncResult asyncResult2 = (AsyncResult)asyncResult;
			FileWebRequest.GetRequestStreamCallback getRequestStreamCallback = (FileWebRequest.GetRequestStreamCallback)asyncResult2.AsyncDelegate;
			return getRequestStreamCallback.EndInvoke(asyncResult);
		}

		public override Stream GetRequestStream()
		{
			IAsyncResult asyncResult = this.BeginGetRequestStream(null, null);
			if (!asyncResult.AsyncWaitHandle.WaitOne(this.timeout, false))
			{
				throw new WebException("The request timed out", WebExceptionStatus.Timeout);
			}
			return this.EndGetRequestStream(asyncResult);
		}

		internal Stream GetRequestStreamInternal()
		{
			this.requestStream = new FileWebRequest.FileWebStream(this, FileMode.Create, FileAccess.Write, FileShare.Read);
			return this.requestStream;
		}

		public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
		{
			lock (this)
			{
				if (this.asyncResponding)
				{
					throw new InvalidOperationException("Cannot re-call start of asynchronous method while a previous call is still in progress.");
				}
				this.asyncResponding = true;
			}
			FileWebRequest.GetResponseCallback getResponseCallback = new FileWebRequest.GetResponseCallback(this.GetResponseInternal);
			return getResponseCallback.BeginInvoke(callback, state);
		}

		public override WebResponse EndGetResponse(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (!asyncResult.IsCompleted)
			{
				asyncResult.AsyncWaitHandle.WaitOne();
			}
			AsyncResult asyncResult2 = (AsyncResult)asyncResult;
			FileWebRequest.GetResponseCallback getResponseCallback = (FileWebRequest.GetResponseCallback)asyncResult2.AsyncDelegate;
			WebResponse webResponse = getResponseCallback.EndInvoke(asyncResult);
			this.asyncResponding = false;
			return webResponse;
		}

		public override WebResponse GetResponse()
		{
			IAsyncResult asyncResult = this.BeginGetResponse(null, null);
			if (!asyncResult.AsyncWaitHandle.WaitOne(this.timeout, false))
			{
				throw new WebException("The request timed out", WebExceptionStatus.Timeout);
			}
			return this.EndGetResponse(asyncResult);
		}

		private WebResponse GetResponseInternal()
		{
			if (this.webResponse != null)
			{
				return this.webResponse;
			}
			lock (this)
			{
				if (this.requesting)
				{
					this.requestEndEvent = new AutoResetEvent(false);
				}
			}
			if (this.requestEndEvent != null)
			{
				this.requestEndEvent.WaitOne();
			}
			FileStream fileStream = null;
			try
			{
				fileStream = new FileWebRequest.FileWebStream(this, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
			catch (Exception ex)
			{
				throw new WebException(ex.Message, ex);
			}
			this.webResponse = new FileWebResponse(this.uri, fileStream);
			return this.webResponse;
		}

		protected override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			serializationInfo.AddValue("headers", this.webHeaders, typeof(WebHeaderCollection));
			serializationInfo.AddValue("proxy", this.proxy, typeof(IWebProxy));
			serializationInfo.AddValue("uri", this.uri, typeof(global::System.Uri));
			serializationInfo.AddValue("connectionGroupName", this.connectionGroup);
			serializationInfo.AddValue("method", this.method);
			serializationInfo.AddValue("contentLength", this.contentLength);
			serializationInfo.AddValue("timeout", this.timeout);
			serializationInfo.AddValue("fileAccess", this.fileAccess);
			serializationInfo.AddValue("preauthenticate", false);
		}

		internal void Close()
		{
			lock (this)
			{
				this.requesting = false;
				if (this.requestEndEvent != null)
				{
					this.requestEndEvent.Set();
				}
			}
		}

		private global::System.Uri uri;

		private WebHeaderCollection webHeaders;

		private ICredentials credentials;

		private string connectionGroup;

		private long contentLength;

		private FileAccess fileAccess = FileAccess.Read;

		private string method = "GET";

		private IWebProxy proxy;

		private bool preAuthenticate;

		private int timeout = 100000;

		private Stream requestStream;

		private FileWebResponse webResponse;

		private AutoResetEvent requestEndEvent;

		private bool requesting;

		private bool asyncResponding;

		internal class FileWebStream : FileStream
		{
			internal FileWebStream(FileWebRequest webRequest, FileMode mode, FileAccess access, FileShare share)
				: base(webRequest.RequestUri.LocalPath, mode, access, share)
			{
				this.webRequest = webRequest;
			}

			public override void Close()
			{
				base.Close();
				FileWebRequest fileWebRequest = this.webRequest;
				this.webRequest = null;
				if (fileWebRequest != null)
				{
					fileWebRequest.Close();
				}
			}

			private FileWebRequest webRequest;
		}

		private delegate Stream GetRequestStreamCallback();

		private delegate WebResponse GetResponseCallback();
	}
}
