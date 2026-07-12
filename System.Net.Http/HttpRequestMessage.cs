using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace System.Net.Http
{
	public class HttpRequestMessage : IDisposable
	{
		public HttpRequestMessage()
		{
			this.method = HttpMethod.Get;
		}

		public HttpRequestMessage(HttpMethod method, string requestUri)
			: this(method, string.IsNullOrEmpty(requestUri) ? null : new Uri(requestUri, UriKind.RelativeOrAbsolute))
		{
		}

		public HttpRequestMessage(HttpMethod method, Uri requestUri)
		{
			this.Method = method;
			this.RequestUri = requestUri;
		}

		public HttpContent Content { get; set; }

		public HttpRequestHeaders Headers
		{
			get
			{
				HttpRequestHeaders httpRequestHeaders;
				if ((httpRequestHeaders = this.headers) == null)
				{
					httpRequestHeaders = (this.headers = new HttpRequestHeaders());
				}
				return httpRequestHeaders;
			}
		}

		public HttpMethod Method
		{
			get
			{
				return this.method;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("method");
				}
				this.method = value;
			}
		}

		public IDictionary<string, object> Properties
		{
			get
			{
				Dictionary<string, object> dictionary;
				if ((dictionary = this.properties) == null)
				{
					dictionary = (this.properties = new Dictionary<string, object>());
				}
				return dictionary;
			}
		}

		public Uri RequestUri
		{
			get
			{
				return this.uri;
			}
			set
			{
				if (value != null && value.IsAbsoluteUri && !HttpRequestMessage.IsAllowedAbsoluteUri(value))
				{
					throw new ArgumentException("Only http or https scheme is allowed");
				}
				this.uri = value;
			}
		}

		private static bool IsAllowedAbsoluteUri(Uri uri)
		{
			return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps || (uri.Scheme == Uri.UriSchemeFile && uri.OriginalString.StartsWith("/", StringComparison.Ordinal));
		}

		public Version Version
		{
			get
			{
				return this.version ?? HttpVersion.Version11;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Version");
				}
				this.version = value;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing && !this.disposed)
			{
				this.disposed = true;
				if (this.Content != null)
				{
					this.Content.Dispose();
				}
			}
		}

		internal bool SetIsUsed()
		{
			if (this.is_used)
			{
				return true;
			}
			this.is_used = true;
			return false;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("Method: ").Append(this.method);
			stringBuilder.Append(", RequestUri: '").Append((this.RequestUri != null) ? this.RequestUri.ToString() : "<null>");
			stringBuilder.Append("', Version: ").Append(this.Version);
			stringBuilder.Append(", Content: ").Append((this.Content != null) ? this.Content.ToString() : "<null>");
			stringBuilder.Append(", Headers:\r\n{\r\n").Append(this.Headers);
			if (this.Content != null)
			{
				stringBuilder.Append(this.Content.Headers);
			}
			stringBuilder.Append("}");
			return stringBuilder.ToString();
		}

		private HttpRequestHeaders headers;

		private HttpMethod method;

		private Version version;

		private Dictionary<string, object> properties;

		private Uri uri;

		private bool is_used;

		private bool disposed;
	}
}
