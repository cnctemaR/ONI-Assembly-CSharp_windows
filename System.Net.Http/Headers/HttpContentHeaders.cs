using System;
using System.Collections.Generic;
using Unity;

namespace System.Net.Http.Headers
{
	public sealed class HttpContentHeaders : HttpHeaders
	{
		internal HttpContentHeaders(HttpContent content)
			: base(HttpHeaderKind.Content)
		{
			this.content = content;
		}

		public ICollection<string> Allow
		{
			get
			{
				return base.GetValues<string>("Allow");
			}
		}

		public ICollection<string> ContentEncoding
		{
			get
			{
				return base.GetValues<string>("Content-Encoding");
			}
		}

		public ContentDispositionHeaderValue ContentDisposition
		{
			get
			{
				return base.GetValue<ContentDispositionHeaderValue>("Content-Disposition");
			}
			set
			{
				base.AddOrRemove<ContentDispositionHeaderValue>("Content-Disposition", value, null);
			}
		}

		public ICollection<string> ContentLanguage
		{
			get
			{
				return base.GetValues<string>("Content-Language");
			}
		}

		public long? ContentLength
		{
			get
			{
				long? num = base.GetValue<long?>("Content-Length");
				if (num != null)
				{
					return num;
				}
				num = this.content.LoadedBufferLength;
				if (num != null)
				{
					return num;
				}
				long num2;
				if (this.content.TryComputeLength(out num2))
				{
					base.SetValue<long>("Content-Length", num2, null);
					return new long?(num2);
				}
				return null;
			}
			set
			{
				base.AddOrRemove<long>("Content-Length", value);
			}
		}

		public Uri ContentLocation
		{
			get
			{
				return base.GetValue<Uri>("Content-Location");
			}
			set
			{
				base.AddOrRemove<Uri>("Content-Location", value, null);
			}
		}

		public byte[] ContentMD5
		{
			get
			{
				return base.GetValue<byte[]>("Content-MD5");
			}
			set
			{
				base.AddOrRemove<byte[]>("Content-MD5", value, Parser.MD5.ToString);
			}
		}

		public ContentRangeHeaderValue ContentRange
		{
			get
			{
				return base.GetValue<ContentRangeHeaderValue>("Content-Range");
			}
			set
			{
				base.AddOrRemove<ContentRangeHeaderValue>("Content-Range", value, null);
			}
		}

		public MediaTypeHeaderValue ContentType
		{
			get
			{
				return base.GetValue<MediaTypeHeaderValue>("Content-Type");
			}
			set
			{
				base.AddOrRemove<MediaTypeHeaderValue>("Content-Type", value, null);
			}
		}

		public DateTimeOffset? Expires
		{
			get
			{
				return base.GetValue<DateTimeOffset?>("Expires");
			}
			set
			{
				base.AddOrRemove<DateTimeOffset>("Expires", value, Parser.DateTime.ToString);
			}
		}

		public DateTimeOffset? LastModified
		{
			get
			{
				return base.GetValue<DateTimeOffset?>("Last-Modified");
			}
			set
			{
				base.AddOrRemove<DateTimeOffset>("Last-Modified", value, Parser.DateTime.ToString);
			}
		}

		internal HttpContentHeaders()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private readonly HttpContent content;
	}
}
