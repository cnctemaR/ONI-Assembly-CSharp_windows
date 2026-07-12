using System;
using System.Collections.Generic;

namespace System.Net.Http.Headers
{
	public sealed class HttpRequestHeaders : HttpHeaders
	{
		internal HttpRequestHeaders()
			: base(HttpHeaderKind.Request)
		{
		}

		public HttpHeaderValueCollection<MediaTypeWithQualityHeaderValue> Accept
		{
			get
			{
				return base.GetValues<MediaTypeWithQualityHeaderValue>("Accept");
			}
		}

		public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptCharset
		{
			get
			{
				return base.GetValues<StringWithQualityHeaderValue>("Accept-Charset");
			}
		}

		public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptEncoding
		{
			get
			{
				return base.GetValues<StringWithQualityHeaderValue>("Accept-Encoding");
			}
		}

		public HttpHeaderValueCollection<StringWithQualityHeaderValue> AcceptLanguage
		{
			get
			{
				return base.GetValues<StringWithQualityHeaderValue>("Accept-Language");
			}
		}

		public AuthenticationHeaderValue Authorization
		{
			get
			{
				return base.GetValue<AuthenticationHeaderValue>("Authorization");
			}
			set
			{
				base.AddOrRemove<AuthenticationHeaderValue>("Authorization", value, null);
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

		internal bool ConnectionKeepAlive
		{
			get
			{
				return this.Connection.Find((string l) => string.Equals(l, "Keep-Alive", StringComparison.OrdinalIgnoreCase)) != null;
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

		public HttpHeaderValueCollection<NameValueWithParametersHeaderValue> Expect
		{
			get
			{
				return base.GetValues<NameValueWithParametersHeaderValue>("Expect");
			}
		}

		public bool? ExpectContinue
		{
			get
			{
				if (this.expectContinue != null)
				{
					return this.expectContinue;
				}
				if (this.TransferEncoding.Find((TransferCodingHeaderValue l) => string.Equals(l.Value, "100-continue", StringComparison.OrdinalIgnoreCase)) == null)
				{
					return null;
				}
				return new bool?(true);
			}
			set
			{
				bool? flag = this.expectContinue;
				bool? flag2 = value;
				if ((flag.GetValueOrDefault() == flag2.GetValueOrDefault()) & (flag != null == (flag2 != null)))
				{
					return;
				}
				this.Expect.Remove((NameValueWithParametersHeaderValue l) => l.Name == "100-continue");
				flag2 = value;
				bool flag3 = true;
				if ((flag2.GetValueOrDefault() == flag3) & (flag2 != null))
				{
					this.Expect.Add(new NameValueWithParametersHeaderValue("100-continue"));
				}
				this.expectContinue = value;
			}
		}

		public string From
		{
			get
			{
				return base.GetValue<string>("From");
			}
			set
			{
				if (!string.IsNullOrEmpty(value) && !Parser.EmailAddress.TryParse(value, out value))
				{
					throw new FormatException();
				}
				base.AddOrRemove("From", value);
			}
		}

		public string Host
		{
			get
			{
				return base.GetValue<string>("Host");
			}
			set
			{
				base.AddOrRemove("Host", value);
			}
		}

		public HttpHeaderValueCollection<EntityTagHeaderValue> IfMatch
		{
			get
			{
				return base.GetValues<EntityTagHeaderValue>("If-Match");
			}
		}

		public DateTimeOffset? IfModifiedSince
		{
			get
			{
				return base.GetValue<DateTimeOffset?>("If-Modified-Since");
			}
			set
			{
				base.AddOrRemove<DateTimeOffset>("If-Modified-Since", value, Parser.DateTime.ToString);
			}
		}

		public HttpHeaderValueCollection<EntityTagHeaderValue> IfNoneMatch
		{
			get
			{
				return base.GetValues<EntityTagHeaderValue>("If-None-Match");
			}
		}

		public RangeConditionHeaderValue IfRange
		{
			get
			{
				return base.GetValue<RangeConditionHeaderValue>("If-Range");
			}
			set
			{
				base.AddOrRemove<RangeConditionHeaderValue>("If-Range", value, null);
			}
		}

		public DateTimeOffset? IfUnmodifiedSince
		{
			get
			{
				return base.GetValue<DateTimeOffset?>("If-Unmodified-Since");
			}
			set
			{
				base.AddOrRemove<DateTimeOffset>("If-Unmodified-Since", value, Parser.DateTime.ToString);
			}
		}

		public int? MaxForwards
		{
			get
			{
				return base.GetValue<int?>("Max-Forwards");
			}
			set
			{
				base.AddOrRemove<int>("Max-Forwards", value);
			}
		}

		public HttpHeaderValueCollection<NameValueHeaderValue> Pragma
		{
			get
			{
				return base.GetValues<NameValueHeaderValue>("Pragma");
			}
		}

		public AuthenticationHeaderValue ProxyAuthorization
		{
			get
			{
				return base.GetValue<AuthenticationHeaderValue>("Proxy-Authorization");
			}
			set
			{
				base.AddOrRemove<AuthenticationHeaderValue>("Proxy-Authorization", value, null);
			}
		}

		public RangeHeaderValue Range
		{
			get
			{
				return base.GetValue<RangeHeaderValue>("Range");
			}
			set
			{
				base.AddOrRemove<RangeHeaderValue>("Range", value, null);
			}
		}

		public Uri Referrer
		{
			get
			{
				return base.GetValue<Uri>("Referer");
			}
			set
			{
				base.AddOrRemove<Uri>("Referer", value, null);
			}
		}

		public HttpHeaderValueCollection<TransferCodingWithQualityHeaderValue> TE
		{
			get
			{
				return base.GetValues<TransferCodingWithQualityHeaderValue>("TE");
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
				if (this.TransferEncoding.Find((TransferCodingHeaderValue l) => string.Equals(l.Value, "chunked", StringComparison.OrdinalIgnoreCase)) == null)
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

		public HttpHeaderValueCollection<ProductInfoHeaderValue> UserAgent
		{
			get
			{
				return base.GetValues<ProductInfoHeaderValue>("User-Agent");
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

		internal void AddHeaders(HttpRequestHeaders headers)
		{
			foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in headers)
			{
				base.TryAddWithoutValidation(keyValuePair.Key, keyValuePair.Value);
			}
		}

		private bool? expectContinue;
	}
}
