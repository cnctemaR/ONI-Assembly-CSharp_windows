using System;

namespace System.Net.Http.Headers
{
	public sealed class HttpResponseHeaders : HttpHeaders
	{
		internal HttpResponseHeaders()
			: base(HttpHeaderKind.Response)
		{
		}

		public HttpHeaderValueCollection<string> AcceptRanges
		{
			get
			{
				return base.GetValues<string>("Accept-Ranges");
			}
		}

		public TimeSpan? Age
		{
			get
			{
				return base.GetValue<TimeSpan?>("Age");
			}
			set
			{
				base.AddOrRemove<TimeSpan>("Age", value, (object l) => ((long)((TimeSpan)l).TotalSeconds).ToString());
			}
		}

		public CacheControlHeaderValue CacheControl
		{
			get
			{
				return base.GetValue<CacheControlHeaderValue>("Cache-Control");
			}
			set
			{
				base.AddOrRemove<CacheControlHeaderValue>("Cache-Control", value, null);
			}
		}

		public HttpHeaderValueCollection<string> Connection
		{
			get
			{
				return base.GetValues<string>("Connection");
			}
		}

		public bool? ConnectionClose
		{
			get
			{
				bool? connectionclose = this.connectionclose;
				bool flag = true;
				if (!((connectionclose.GetValueOrDefault() == flag) & (connectionclose != null)))
				{
					if (this.Connection.Find((string l) => string.Equals(l, "close", StringComparison.OrdinalIgnoreCase)) == null)
					{
						return this.connectionclose;
					}
				}
				return new bool?(true);
			}
			set
			{
				bool? connectionclose = this.connectionclose;
				bool? flag = value;
				if ((connectionclose.GetValueOrDefault() == flag.GetValueOrDefault()) & (connectionclose != null == (flag != null)))
				{
					return;
				}
				this.Connection.Remove("close");
				flag = value;
				bool flag2 = true;
				if ((flag.GetValueOrDefault() == flag2) & (flag != null))
				{
					this.Connection.Add("close");
				}
				this.connectionclose = value;
			}
		}

		public DateTimeOffset? Date
		{
			get
			{
				return base.GetValue<DateTimeOffset?>("Date");
			}
			set
			{
				base.AddOrRemove<DateTimeOffset>("Date", value, Parser.DateTime.ToString);
			}
		}

		public EntityTagHeaderValue ETag
		{
			get
			{
				return base.GetValue<EntityTagHeaderValue>("ETag");
			}
			set
			{
				base.AddOrRemove<EntityTagHeaderValue>("ETag", value, null);
			}
		}

		public Uri Location
		{
			get
			{
				return base.GetValue<Uri>("Location");
			}
			set
			{
				base.AddOrRemove<Uri>("Location", value, null);
			}
		}

		public HttpHeaderValueCollection<NameValueHeaderValue> Pragma
		{
			get
			{
				return base.GetValues<NameValueHeaderValue>("Pragma");
			}
		}

		public HttpHeaderValueCollection<AuthenticationHeaderValue> ProxyAuthenticate
		{
			get
			{
				return base.GetValues<AuthenticationHeaderValue>("Proxy-Authenticate");
			}
		}

		public RetryConditionHeaderValue RetryAfter
		{
			get
			{
				return base.GetValue<RetryConditionHeaderValue>("Retry-After");
			}
			set
			{
				base.AddOrRemove<RetryConditionHeaderValue>("Retry-After", value, null);
			}
		}

		public HttpHeaderValueCollection<ProductInfoHeaderValue> Server
		{
			get
			{
				return base.GetValues<ProductInfoHeaderValue>("Server");
			}
		}

		public HttpHeaderValueCollection<string> Trailer
		{
			get
			{
				return base.GetValues<string>("Trailer");
			}
		}

		public HttpHeaderValueCollection<TransferCodingHeaderValue> TransferEncoding
		{
			get
			{
				return base.GetValues<TransferCodingHeaderValue>("Transfer-Encoding");
			}
		}

		public bool? TransferEncodingChunked
		{
			get
			{
				if (this.transferEncodingChunked != null)
				{
					return this.transferEncodingChunked;
				}
				if (this.TransferEncoding.Find((TransferCodingHeaderValue l) => StringComparer.OrdinalIgnoreCase.Equals(l.Value, "chunked")) == null)
				{
					return null;
				}
				return new bool?(true);
			}
			set
			{
				bool? flag = value;
				bool? flag2 = this.transferEncodingChunked;
				if ((flag.GetValueOrDefault() == flag2.GetValueOrDefault()) & (flag != null == (flag2 != null)))
				{
					return;
				}
				this.TransferEncoding.Remove((TransferCodingHeaderValue l) => l.Value == "chunked");
				flag2 = value;
				bool flag3 = true;
				if ((flag2.GetValueOrDefault() == flag3) & (flag2 != null))
				{
					this.TransferEncoding.Add(new TransferCodingHeaderValue("chunked"));
				}
				this.transferEncodingChunked = value;
			}
		}

		public HttpHeaderValueCollection<ProductHeaderValue> Upgrade
		{
			get
			{
				return base.GetValues<ProductHeaderValue>("Upgrade");
			}
		}

		public HttpHeaderValueCollection<string> Vary
		{
			get
			{
				return base.GetValues<string>("Vary");
			}
		}

		public HttpHeaderValueCollection<ViaHeaderValue> Via
		{
			get
			{
				return base.GetValues<ViaHeaderValue>("Via");
			}
		}

		public HttpHeaderValueCollection<WarningHeaderValue> Warning
		{
			get
			{
				return base.GetValues<WarningHeaderValue>("Warning");
			}
		}

		public HttpHeaderValueCollection<AuthenticationHeaderValue> WwwAuthenticate
		{
			get
			{
				return base.GetValues<AuthenticationHeaderValue>("WWW-Authenticate");
			}
		}
	}
}
