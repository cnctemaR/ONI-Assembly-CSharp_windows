using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace System.Net.Http
{
	public class MultipartContent : HttpContent, IEnumerable<HttpContent>, IEnumerable
	{
		public MultipartContent()
			: this("mixed")
		{
		}

		public MultipartContent(string subtype)
			: this(subtype, Guid.NewGuid().ToString("D", CultureInfo.InvariantCulture))
		{
		}

		public MultipartContent(string subtype, string boundary)
		{
			if (string.IsNullOrWhiteSpace(subtype))
			{
				throw new ArgumentException("boundary");
			}
			if (string.IsNullOrWhiteSpace(boundary))
			{
				throw new ArgumentException("boundary");
			}
			if (boundary.Length > 70)
			{
				throw new ArgumentOutOfRangeException("boundary");
			}
			if (boundary.Last<char>() == ' ' || !MultipartContent.IsValidRFC2049(boundary))
			{
				throw new ArgumentException("boundary");
			}
			this.boundary = boundary;
			this.nested_content = new List<HttpContent>(2);
			base.Headers.ContentType = new MediaTypeHeaderValue("multipart/" + subtype)
			{
				Parameters = 
				{
					new NameValueHeaderValue("boundary", "\"" + boundary + "\"")
				}
			};
		}

		private static bool IsValidRFC2049(string s)
		{
			foreach (char c in s)
			{
				if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && (c < '0' || c > '9'))
				{
					if (c <= ':')
					{
						switch (c)
						{
						case '\'':
						case '(':
						case ')':
						case '+':
						case ',':
						case '-':
						case '.':
						case '/':
							goto IL_0071;
						case '*':
							break;
						default:
							if (c == ':')
							{
								goto IL_0071;
							}
							break;
						}
					}
					else if (c == '=' || c == '?')
					{
						goto IL_0071;
					}
					return false;
				}
				IL_0071:;
			}
			return true;
		}

		public virtual void Add(HttpContent content)
		{
			if (content == null)
			{
				throw new ArgumentNullException("content");
			}
			if (this.nested_content == null)
			{
				this.nested_content = new List<HttpContent>();
			}
			this.nested_content.Add(content);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				foreach (HttpContent httpContent in this.nested_content)
				{
					httpContent.Dispose();
				}
				this.nested_content = null;
			}
			base.Dispose(disposing);
		}

		protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append('-').Append('-');
			sb.Append(this.boundary);
			sb.Append('\r').Append('\n');
			byte[] array;
			for (int i = 0; i < this.nested_content.Count; i++)
			{
				HttpContent c = this.nested_content[i];
				foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in c.Headers)
				{
					sb.Append(keyValuePair.Key);
					sb.Append(':').Append(' ');
					foreach (string text in keyValuePair.Value)
					{
						sb.Append(text);
					}
					sb.Append('\r').Append('\n');
				}
				sb.Append('\r').Append('\n');
				array = Encoding.ASCII.GetBytes(sb.ToString());
				sb.Clear();
				await stream.WriteAsync(array, 0, array.Length).ConfigureAwait(false);
				await c.SerializeToStreamAsync_internal(stream, context).ConfigureAwait(false);
				if (i != this.nested_content.Count - 1)
				{
					sb.Append('\r').Append('\n');
					sb.Append('-').Append('-');
					sb.Append(this.boundary);
					sb.Append('\r').Append('\n');
				}
				c = null;
			}
			sb.Append('\r').Append('\n');
			sb.Append('-').Append('-');
			sb.Append(this.boundary);
			sb.Append('-').Append('-');
			sb.Append('\r').Append('\n');
			array = Encoding.ASCII.GetBytes(sb.ToString());
			await stream.WriteAsync(array, 0, array.Length).ConfigureAwait(false);
		}

		protected internal override bool TryComputeLength(out long length)
		{
			length = (long)(12 + 2 * this.boundary.Length);
			for (int i = 0; i < this.nested_content.Count; i++)
			{
				HttpContent httpContent = this.nested_content[i];
				foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in httpContent.Headers)
				{
					length += (long)keyValuePair.Key.Length;
					length += 4L;
					foreach (string text in keyValuePair.Value)
					{
						length += (long)text.Length;
					}
				}
				long num;
				if (!httpContent.TryComputeLength(out num))
				{
					return false;
				}
				length += 2L;
				length += num;
				if (i != this.nested_content.Count - 1)
				{
					length += 6L;
					length += (long)this.boundary.Length;
				}
			}
			return true;
		}

		public IEnumerator<HttpContent> GetEnumerator()
		{
			return this.nested_content.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.nested_content.GetEnumerator();
		}

		private List<HttpContent> nested_content;

		private readonly string boundary;
	}
}
