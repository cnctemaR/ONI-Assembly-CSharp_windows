using System;
using System.Net.Http.Headers;

namespace System.Net.Http
{
	public class HttpMethod : IEquatable<HttpMethod>
	{
		public HttpMethod(string method)
		{
			if (string.IsNullOrEmpty(method))
			{
				throw new ArgumentException("method");
			}
			Parser.Token.Check(method);
			this.method = method;
		}

		public static HttpMethod Delete
		{
			get
			{
				return HttpMethod.delete_method;
			}
		}

		public static HttpMethod Get
		{
			get
			{
				return HttpMethod.get_method;
			}
		}

		public static HttpMethod Head
		{
			get
			{
				return HttpMethod.head_method;
			}
		}

		public string Method
		{
			get
			{
				return this.method;
			}
		}

		public static HttpMethod Options
		{
			get
			{
				return HttpMethod.options_method;
			}
		}

		public static HttpMethod Post
		{
			get
			{
				return HttpMethod.post_method;
			}
		}

		public static HttpMethod Put
		{
			get
			{
				return HttpMethod.put_method;
			}
		}

		public static HttpMethod Trace
		{
			get
			{
				return HttpMethod.trace_method;
			}
		}

		public static bool operator ==(HttpMethod left, HttpMethod right)
		{
			if (left == null || right == null)
			{
				return left == right;
			}
			return left.Equals(right);
		}

		public static bool operator !=(HttpMethod left, HttpMethod right)
		{
			return !(left == right);
		}

		public bool Equals(HttpMethod other)
		{
			return string.Equals(this.method, other.method, StringComparison.OrdinalIgnoreCase);
		}

		public override bool Equals(object obj)
		{
			HttpMethod httpMethod = obj as HttpMethod;
			return httpMethod != null && this.Equals(httpMethod);
		}

		public override int GetHashCode()
		{
			return this.method.GetHashCode();
		}

		public override string ToString()
		{
			return this.method;
		}

		public static HttpMethod Patch
		{
			get
			{
				throw new PlatformNotSupportedException();
			}
		}

		private static readonly HttpMethod delete_method = new HttpMethod("DELETE");

		private static readonly HttpMethod get_method = new HttpMethod("GET");

		private static readonly HttpMethod head_method = new HttpMethod("HEAD");

		private static readonly HttpMethod options_method = new HttpMethod("OPTIONS");

		private static readonly HttpMethod post_method = new HttpMethod("POST");

		private static readonly HttpMethod put_method = new HttpMethod("PUT");

		private static readonly HttpMethod trace_method = new HttpMethod("TRACE");

		private readonly string method;
	}
}
