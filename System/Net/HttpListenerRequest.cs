using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace System.Net
{
	public sealed class HttpListenerRequest
	{
		internal HttpListenerRequest(HttpListenerContext context)
		{
			this.context = context;
			this.headers = new WebHeaderCollection();
			this.input_stream = Stream.Null;
			this.version = HttpVersion.Version10;
		}

		internal void SetRequestLine(string req)
		{
			string[] array = req.Split(HttpListenerRequest.separators, 3);
			if (array.Length != 3)
			{
				this.context.ErrorMessage = "Invalid request line (parts).";
				return;
			}
			this.method = array[0];
			foreach (char c in this.method)
			{
				int num = (int)c;
				if ((num < 65 || num > 90) && (num <= 32 || c >= '\u007f' || c == '(' || c == ')' || c == '<' || c == '<' || c == '>' || c == '@' || c == ',' || c == ';' || c == ':' || c == '\\' || c == '"' || c == '/' || c == '[' || c == ']' || c == '?' || c == '=' || c == '{' || c == '}'))
				{
					this.context.ErrorMessage = "(Invalid verb)";
					return;
				}
			}
			this.raw_url = array[1];
			if (array[2].Length != 8 || !array[2].StartsWith("HTTP/"))
			{
				this.context.ErrorMessage = "Invalid request line (version).";
				return;
			}
			try
			{
				this.version = new Version(array[2].Substring(5));
				if (this.version.Major < 1)
				{
					throw new Exception();
				}
			}
			catch
			{
				this.context.ErrorMessage = "Invalid request line (version).";
			}
		}

		private void CreateQueryString(string query)
		{
			this.query_string = new global::System.Collections.Specialized.NameValueCollection();
			if (query == null || query.Length == 0)
			{
				return;
			}
			if (query[0] == '?')
			{
				query = query.Substring(1);
			}
			string[] array = query.Split(new char[] { '&' });
			foreach (string text in array)
			{
				int num = text.IndexOf('=');
				if (num == -1)
				{
					this.query_string.Add(null, HttpUtility.UrlDecode(text));
				}
				else
				{
					string text2 = HttpUtility.UrlDecode(text.Substring(0, num));
					string text3 = HttpUtility.UrlDecode(text.Substring(num + 1));
					this.query_string.Add(text2, text3);
				}
			}
		}

		internal void FinishInitialization()
		{
			string text = this.UserHostName;
			if (this.version > HttpVersion.Version10 && (text == null || text.Length == 0))
			{
				this.context.ErrorMessage = "Invalid host name";
				return;
			}
			global::System.Uri uri;
			string pathAndQuery;
			if (global::System.Uri.MaybeUri(this.raw_url) && global::System.Uri.TryCreate(this.raw_url, global::System.UriKind.Absolute, out uri))
			{
				pathAndQuery = uri.PathAndQuery;
			}
			else
			{
				pathAndQuery = this.raw_url;
			}
			if (text == null || text.Length == 0)
			{
				text = this.UserHostAddress;
			}
			if (uri != null)
			{
				text = uri.Host;
			}
			int num = text.IndexOf(':');
			if (num >= 0)
			{
				text = text.Substring(0, num);
			}
			string text2 = string.Format("{0}://{1}:{2}", (!this.IsSecureConnection) ? "http" : "https", text, this.LocalEndPoint.Port);
			if (!global::System.Uri.TryCreate(text2 + pathAndQuery, global::System.UriKind.Absolute, out this.url))
			{
				this.context.ErrorMessage = "Invalid url: " + text2 + pathAndQuery;
				return;
			}
			this.CreateQueryString(this.url.Query);
			string text3 = null;
			if (this.version >= HttpVersion.Version11)
			{
				text3 = this.Headers["Transfer-Encoding"];
				if (text3 != null && text3 != "chunked")
				{
					this.context.Connection.SendError(null, 501);
					return;
				}
			}
			this.is_chunked = text3 == "chunked";
			foreach (string text4 in HttpListenerRequest.no_body_methods)
			{
				if (string.Compare(this.method, text4, StringComparison.InvariantCultureIgnoreCase) == 0)
				{
					return;
				}
			}
			if (!this.is_chunked && !this.cl_set)
			{
				this.context.Connection.SendError(null, 411);
				return;
			}
			if (this.is_chunked || this.content_length > 0L)
			{
				this.input_stream = this.context.Connection.GetRequestStream(this.is_chunked, this.content_length);
			}
			if (this.Headers["Expect"] == "100-continue")
			{
				ResponseStream responseStream = this.context.Connection.GetResponseStream();
				responseStream.InternalWrite(HttpListenerRequest._100continue, 0, HttpListenerRequest._100continue.Length);
			}
		}

		internal static string Unquote(string str)
		{
			int num = str.IndexOf('"');
			int num2 = str.LastIndexOf('"');
			if (num >= 0 && num2 >= 0)
			{
				str = str.Substring(num + 1, num2 - 1);
			}
			return str.Trim();
		}

		internal void AddHeader(string header)
		{
			int num = header.IndexOf(':');
			if (num == -1 || num == 0)
			{
				this.context.ErrorMessage = "Bad Request";
				this.context.ErrorStatus = 400;
				return;
			}
			string text = header.Substring(0, num).Trim();
			string text2 = header.Substring(num + 1).Trim();
			string text3 = text.ToLower(CultureInfo.InvariantCulture);
			this.headers.SetInternal(text, text2);
			string text4 = text3;
			switch (text4)
			{
			case "accept-language":
				this.user_languages = text2.Split(new char[] { ',' });
				break;
			case "accept":
				this.accept_types = text2.Split(new char[] { ',' });
				break;
			case "content-length":
				try
				{
					this.content_length = long.Parse(text2.Trim());
					if (this.content_length < 0L)
					{
						this.context.ErrorMessage = "Invalid Content-Length.";
					}
					this.cl_set = true;
				}
				catch
				{
					this.context.ErrorMessage = "Invalid Content-Length.";
				}
				break;
			case "referer":
				try
				{
					this.referrer = new global::System.Uri(text2);
				}
				catch
				{
					this.referrer = new global::System.Uri("http://someone.is.screwing.with.the.headers.com/");
				}
				break;
			case "cookie":
			{
				if (this.cookies == null)
				{
					this.cookies = new CookieCollection();
				}
				string[] array = text2.Split(new char[] { ',', ';' });
				Cookie cookie = null;
				int num3 = 0;
				foreach (string text5 in array)
				{
					string text6 = text5.Trim();
					if (text6.Length != 0)
					{
						if (text6.StartsWith("$Version"))
						{
							num3 = int.Parse(HttpListenerRequest.Unquote(text6.Substring(text6.IndexOf("=") + 1)));
						}
						else if (text6.StartsWith("$Path"))
						{
							if (cookie != null)
							{
								cookie.Path = text6.Substring(text6.IndexOf("=") + 1).Trim();
							}
						}
						else if (text6.StartsWith("$Domain"))
						{
							if (cookie != null)
							{
								cookie.Domain = text6.Substring(text6.IndexOf("=") + 1).Trim();
							}
						}
						else if (text6.StartsWith("$Port"))
						{
							if (cookie != null)
							{
								cookie.Port = text6.Substring(text6.IndexOf("=") + 1).Trim();
							}
						}
						else
						{
							if (cookie != null)
							{
								this.cookies.Add(cookie);
							}
							cookie = new Cookie();
							int num4 = text6.IndexOf("=");
							if (num4 > 0)
							{
								cookie.Name = text6.Substring(0, num4).Trim();
								cookie.Value = text6.Substring(num4 + 1).Trim();
							}
							else
							{
								cookie.Name = text6.Trim();
								cookie.Value = string.Empty;
							}
							cookie.Version = num3;
						}
					}
				}
				if (cookie != null)
				{
					this.cookies.Add(cookie);
				}
				break;
			}
			}
		}

		internal bool FlushInput()
		{
			if (!this.HasEntityBody)
			{
				return true;
			}
			int num = 2048;
			if (this.content_length > 0L)
			{
				num = (int)Math.Min(this.content_length, (long)num);
			}
			byte[] array = new byte[num];
			bool flag;
			for (;;)
			{
				try
				{
					if (this.InputStream.Read(array, 0, num) <= 0)
					{
						flag = true;
						break;
					}
				}
				catch
				{
					flag = false;
					break;
				}
			}
			return flag;
		}

		public string[] AcceptTypes
		{
			get
			{
				return this.accept_types;
			}
		}

		[global::System.MonoTODO("Always returns 0")]
		public int ClientCertificateError
		{
			get
			{
				return 0;
			}
		}

		public Encoding ContentEncoding
		{
			get
			{
				if (this.content_encoding == null)
				{
					this.content_encoding = Encoding.Default;
				}
				return this.content_encoding;
			}
		}

		public long ContentLength64
		{
			get
			{
				return this.content_length;
			}
		}

		public string ContentType
		{
			get
			{
				return this.headers["content-type"];
			}
		}

		public CookieCollection Cookies
		{
			get
			{
				if (this.cookies == null)
				{
					this.cookies = new CookieCollection();
				}
				return this.cookies;
			}
		}

		public bool HasEntityBody
		{
			get
			{
				return this.content_length > 0L || this.is_chunked;
			}
		}

		public global::System.Collections.Specialized.NameValueCollection Headers
		{
			get
			{
				return this.headers;
			}
		}

		public string HttpMethod
		{
			get
			{
				return this.method;
			}
		}

		public Stream InputStream
		{
			get
			{
				return this.input_stream;
			}
		}

		[global::System.MonoTODO("Always returns false")]
		public bool IsAuthenticated
		{
			get
			{
				return false;
			}
		}

		public bool IsLocal
		{
			get
			{
				return IPAddress.IsLoopback(this.RemoteEndPoint.Address);
			}
		}

		public bool IsSecureConnection
		{
			get
			{
				return this.context.Connection.IsSecure;
			}
		}

		public bool KeepAlive
		{
			get
			{
				return false;
			}
		}

		public IPEndPoint LocalEndPoint
		{
			get
			{
				return this.context.Connection.LocalEndPoint;
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				return this.version;
			}
		}

		public global::System.Collections.Specialized.NameValueCollection QueryString
		{
			get
			{
				return this.query_string;
			}
		}

		public string RawUrl
		{
			get
			{
				return this.raw_url;
			}
		}

		public IPEndPoint RemoteEndPoint
		{
			get
			{
				return this.context.Connection.RemoteEndPoint;
			}
		}

		public Guid RequestTraceIdentifier
		{
			get
			{
				return this.identifier;
			}
		}

		public global::System.Uri Url
		{
			get
			{
				return this.url;
			}
		}

		public global::System.Uri UrlReferrer
		{
			get
			{
				return this.referrer;
			}
		}

		public string UserAgent
		{
			get
			{
				return this.headers["user-agent"];
			}
		}

		public string UserHostAddress
		{
			get
			{
				return this.LocalEndPoint.ToString();
			}
		}

		public string UserHostName
		{
			get
			{
				return this.headers["host"];
			}
		}

		public string[] UserLanguages
		{
			get
			{
				return this.user_languages;
			}
		}

		public IAsyncResult BeginGetClientCertificate(AsyncCallback requestCallback, object state)
		{
			return null;
		}

		public global::System.Security.Cryptography.X509Certificates.X509Certificate2 EndGetClientCertificate(IAsyncResult asyncResult)
		{
			return null;
		}

		public global::System.Security.Cryptography.X509Certificates.X509Certificate2 GetClientCertificate()
		{
			return null;
		}

		private string[] accept_types;

		private Encoding content_encoding;

		private long content_length;

		private bool cl_set;

		private CookieCollection cookies;

		private WebHeaderCollection headers;

		private string method;

		private Stream input_stream;

		private Version version;

		private global::System.Collections.Specialized.NameValueCollection query_string;

		private string raw_url;

		private Guid identifier;

		private global::System.Uri url;

		private global::System.Uri referrer;

		private string[] user_languages;

		private HttpListenerContext context;

		private bool is_chunked;

		private static byte[] _100continue = Encoding.ASCII.GetBytes("HTTP/1.1 100 Continue\r\n\r\n");

		private static readonly string[] no_body_methods = new string[] { "GET", "HEAD", "DELETE" };

		private static char[] separators = new char[] { ' ' };
	}
}
