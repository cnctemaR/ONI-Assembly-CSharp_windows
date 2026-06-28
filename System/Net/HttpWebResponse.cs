using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;

namespace System.Net
{
	[Serializable]
	public class HttpWebResponse : WebResponse, IDisposable, ISerializable
	{
		internal HttpWebResponse(global::System.Uri uri, string method, WebConnectionData data, CookieContainer container)
		{
			this.uri = uri;
			this.method = method;
			this.webHeaders = data.Headers;
			this.version = data.Version;
			this.statusCode = (HttpStatusCode)data.StatusCode;
			this.statusDescription = data.StatusDescription;
			this.stream = data.stream;
			this.contentLength = -1L;
			try
			{
				string text = this.webHeaders["Content-Length"];
				if (string.IsNullOrEmpty(text) || !long.TryParse(text, out this.contentLength))
				{
					this.contentLength = -1L;
				}
			}
			catch (Exception)
			{
				this.contentLength = -1L;
			}
			if (container != null)
			{
				this.cookie_container = container;
				this.FillCookies();
			}
			string text2 = this.webHeaders["Content-Encoding"];
			if (text2 == "gzip" && (data.request.AutomaticDecompression & DecompressionMethods.GZip) != DecompressionMethods.None)
			{
				this.stream = new global::System.IO.Compression.GZipStream(this.stream, global::System.IO.Compression.CompressionMode.Decompress);
			}
			else if (text2 == "deflate" && (data.request.AutomaticDecompression & DecompressionMethods.Deflate) != DecompressionMethods.None)
			{
				this.stream = new global::System.IO.Compression.DeflateStream(this.stream, global::System.IO.Compression.CompressionMode.Decompress);
			}
		}

		[Obsolete("Serialization is obsoleted for this type", false)]
		protected HttpWebResponse(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			this.uri = (global::System.Uri)serializationInfo.GetValue("uri", typeof(global::System.Uri));
			this.contentLength = serializationInfo.GetInt64("contentLength");
			this.contentType = serializationInfo.GetString("contentType");
			this.method = serializationInfo.GetString("method");
			this.statusDescription = serializationInfo.GetString("statusDescription");
			this.cookieCollection = (CookieCollection)serializationInfo.GetValue("cookieCollection", typeof(CookieCollection));
			this.version = (Version)serializationInfo.GetValue("version", typeof(Version));
			this.statusCode = (HttpStatusCode)((int)serializationInfo.GetValue("statusCode", typeof(HttpStatusCode)));
		}

		void ISerializable.GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			this.GetObjectData(serializationInfo, streamingContext);
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public string CharacterSet
		{
			get
			{
				string text = this.ContentType;
				if (text == null)
				{
					return "ISO-8859-1";
				}
				string text2 = text.ToLower();
				int num = text2.IndexOf("charset=");
				if (num == -1)
				{
					return "ISO-8859-1";
				}
				num += 8;
				int num2 = text2.IndexOf(';', num);
				return (num2 != -1) ? text.Substring(num, num2 - num) : text.Substring(num);
			}
		}

		public string ContentEncoding
		{
			get
			{
				this.CheckDisposed();
				string text = this.webHeaders["Content-Encoding"];
				return (text == null) ? string.Empty : text;
			}
		}

		public override long ContentLength
		{
			get
			{
				return this.contentLength;
			}
		}

		public override string ContentType
		{
			get
			{
				this.CheckDisposed();
				if (this.contentType == null)
				{
					this.contentType = this.webHeaders["Content-Type"];
				}
				return this.contentType;
			}
		}

		public CookieCollection Cookies
		{
			get
			{
				this.CheckDisposed();
				if (this.cookieCollection == null)
				{
					this.cookieCollection = new CookieCollection();
				}
				return this.cookieCollection;
			}
			set
			{
				this.CheckDisposed();
				this.cookieCollection = value;
			}
		}

		public override WebHeaderCollection Headers
		{
			get
			{
				return this.webHeaders;
			}
		}

		private static Exception GetMustImplement()
		{
			return new NotImplementedException();
		}

		[global::System.MonoTODO]
		public override bool IsMutuallyAuthenticated
		{
			get
			{
				throw HttpWebResponse.GetMustImplement();
			}
		}

		public DateTime LastModified
		{
			get
			{
				this.CheckDisposed();
				DateTime dateTime;
				try
				{
					string text = this.webHeaders["Last-Modified"];
					dateTime = MonoHttpDate.Parse(text);
				}
				catch (Exception)
				{
					dateTime = DateTime.Now;
				}
				return dateTime;
			}
		}

		public string Method
		{
			get
			{
				this.CheckDisposed();
				return this.method;
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				this.CheckDisposed();
				return this.version;
			}
		}

		public override global::System.Uri ResponseUri
		{
			get
			{
				this.CheckDisposed();
				return this.uri;
			}
		}

		public string Server
		{
			get
			{
				this.CheckDisposed();
				return this.webHeaders["Server"];
			}
		}

		public HttpStatusCode StatusCode
		{
			get
			{
				return this.statusCode;
			}
		}

		public string StatusDescription
		{
			get
			{
				this.CheckDisposed();
				return this.statusDescription;
			}
		}

		public string GetResponseHeader(string headerName)
		{
			this.CheckDisposed();
			string text = this.webHeaders[headerName];
			return (text == null) ? string.Empty : text;
		}

		internal void ReadAll()
		{
			WebConnectionStream webConnectionStream = this.stream as WebConnectionStream;
			if (webConnectionStream == null)
			{
				return;
			}
			try
			{
				webConnectionStream.ReadAll();
			}
			catch
			{
			}
		}

		public override Stream GetResponseStream()
		{
			this.CheckDisposed();
			if (this.stream == null)
			{
				return Stream.Null;
			}
			if (string.Compare(this.method, "HEAD", true) == 0)
			{
				return Stream.Null;
			}
			return this.stream;
		}

		protected override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			serializationInfo.AddValue("uri", this.uri);
			serializationInfo.AddValue("contentLength", this.contentLength);
			serializationInfo.AddValue("contentType", this.contentType);
			serializationInfo.AddValue("method", this.method);
			serializationInfo.AddValue("statusDescription", this.statusDescription);
			serializationInfo.AddValue("cookieCollection", this.cookieCollection);
			serializationInfo.AddValue("version", this.version);
			serializationInfo.AddValue("statusCode", this.statusCode);
		}

		public override void Close()
		{
			((IDisposable)this).Dispose();
		}

		private void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			this.disposed = true;
			if (disposing)
			{
				this.uri = null;
				this.cookieCollection = null;
				this.method = null;
				this.version = null;
				this.statusDescription = null;
			}
			Stream stream = this.stream;
			this.stream = null;
			if (stream != null)
			{
				stream.Close();
			}
		}

		private void CheckDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
		}

		private void FillCookies()
		{
			if (this.webHeaders == null)
			{
				return;
			}
			string[] array = this.webHeaders.GetValues("Set-Cookie");
			if (array != null)
			{
				foreach (string text in array)
				{
					this.SetCookie(text);
				}
			}
			array = this.webHeaders.GetValues("Set-Cookie2");
			if (array != null)
			{
				foreach (string text2 in array)
				{
					this.SetCookie2(text2);
				}
			}
		}

		private void SetCookie(string header)
		{
			Cookie cookie = null;
			CookieParser cookieParser = new CookieParser(header);
			string text;
			string text2;
			while (cookieParser.GetNextNameValue(out text, out text2))
			{
				if ((text != null && !(text == string.Empty)) || cookie != null)
				{
					if (cookie == null)
					{
						cookie = new Cookie(text, text2);
					}
					else
					{
						text = text.ToUpper();
						string text3 = text;
						switch (text3)
						{
						case "COMMENT":
							if (cookie.Comment == null)
							{
								cookie.Comment = text2;
							}
							break;
						case "COMMENTURL":
							if (cookie.CommentUri == null)
							{
								cookie.CommentUri = new global::System.Uri(text2);
							}
							break;
						case "DISCARD":
							cookie.Discard = true;
							break;
						case "DOMAIN":
							if (cookie.Domain == string.Empty)
							{
								cookie.Domain = text2;
							}
							break;
						case "HTTPONLY":
							cookie.HttpOnly = true;
							break;
						case "MAX-AGE":
							if (cookie.Expires == DateTime.MinValue)
							{
								try
								{
									cookie.Expires = cookie.TimeStamp.AddSeconds(uint.Parse(text2));
								}
								catch
								{
								}
							}
							break;
						case "EXPIRES":
							if (!(cookie.Expires != DateTime.MinValue))
							{
								cookie.Expires = this.TryParseCookieExpires(text2);
							}
							break;
						case "PATH":
							cookie.Path = text2;
							break;
						case "PORT":
							if (cookie.Port == null)
							{
								cookie.Port = text2;
							}
							break;
						case "SECURE":
							cookie.Secure = true;
							break;
						case "VERSION":
							try
							{
								cookie.Version = (int)uint.Parse(text2);
							}
							catch
							{
							}
							break;
						}
					}
				}
			}
			if (cookie == null)
			{
				return;
			}
			if (this.cookieCollection == null)
			{
				this.cookieCollection = new CookieCollection();
			}
			if (cookie.Domain == string.Empty)
			{
				cookie.Domain = this.uri.Host;
			}
			this.cookieCollection.Add(cookie);
			if (this.cookie_container != null)
			{
				this.cookie_container.Add(this.uri, cookie);
			}
		}

		private void SetCookie2(string cookies_str)
		{
			string[] array = cookies_str.Split(new char[] { ',' });
			foreach (string text in array)
			{
				this.SetCookie(text);
			}
		}

		private DateTime TryParseCookieExpires(string value)
		{
			if (value == null || value.Length == 0)
			{
				return DateTime.MinValue;
			}
			for (int i = 0; i < this.cookieExpiresFormats.Length; i++)
			{
				try
				{
					DateTime dateTime = DateTime.ParseExact(value, this.cookieExpiresFormats[i], CultureInfo.InvariantCulture);
					dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
					return TimeZone.CurrentTimeZone.ToLocalTime(dateTime);
				}
				catch
				{
				}
			}
			return DateTime.MinValue;
		}

		private global::System.Uri uri;

		private WebHeaderCollection webHeaders;

		private CookieCollection cookieCollection;

		private string method;

		private Version version;

		private HttpStatusCode statusCode;

		private string statusDescription;

		private long contentLength;

		private string contentType;

		private CookieContainer cookie_container;

		private bool disposed;

		private Stream stream;

		private string[] cookieExpiresFormats = new string[] { "r", "ddd, dd'-'MMM'-'yyyy HH':'mm':'ss 'GMT'", "ddd, dd'-'MMM'-'yy HH':'mm':'ss 'GMT'" };
	}
}
