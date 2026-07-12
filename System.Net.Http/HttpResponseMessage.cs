using System;
using System.Net.Http.Headers;
using System.Text;

namespace System.Net.Http
{
	public class HttpResponseMessage : IDisposable
	{
		public HttpResponseMessage()
			: this(HttpStatusCode.OK)
		{
		}

		public HttpResponseMessage(HttpStatusCode statusCode)
		{
			this.StatusCode = statusCode;
		}

		public HttpContent Content { get; set; }

		public HttpResponseHeaders Headers
		{
			get
			{
				HttpResponseHeaders httpResponseHeaders;
				if ((httpResponseHeaders = this.headers) == null)
				{
					httpResponseHeaders = (this.headers = new HttpResponseHeaders());
				}
				return httpResponseHeaders;
			}
		}

		public bool IsSuccessStatusCode
		{
			get
			{
				return this.statusCode >= HttpStatusCode.OK && this.statusCode < HttpStatusCode.MultipleChoices;
			}
		}

		public string ReasonPhrase
		{
			get
			{
				return this.reasonPhrase ?? HttpStatusDescription.Get(this.statusCode);
			}
			set
			{
				this.reasonPhrase = value;
			}
		}

		public HttpRequestMessage RequestMessage { get; set; }

		public HttpStatusCode StatusCode
		{
			get
			{
				return this.statusCode;
			}
			set
			{
				if (value < (HttpStatusCode)0)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.statusCode = value;
			}
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

		public HttpResponseMessage EnsureSuccessStatusCode()
		{
			if (this.IsSuccessStatusCode)
			{
				return this;
			}
			throw new HttpRequestException(string.Format("{0} ({1})", (int)this.statusCode, this.ReasonPhrase));
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("StatusCode: ").Append((int)this.StatusCode);
			stringBuilder.Append(", ReasonPhrase: '").Append(this.ReasonPhrase ?? "<null>");
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

		public HttpResponseHeaders TrailingHeaders
		{
			get
			{
				if (this.trailingHeaders == null)
				{
					this.trailingHeaders = new HttpResponseHeaders();
				}
				return this.trailingHeaders;
			}
		}

		private HttpResponseHeaders headers;

		private HttpResponseHeaders trailingHeaders;

		private string reasonPhrase;

		private HttpStatusCode statusCode;

		private Version version;

		private bool disposed;
	}
}
