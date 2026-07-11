using System;
using System.Collections;
using System.Globalization;
using System.Text;

namespace System.Net
{
	[Serializable]
	public sealed class Cookie
	{
		public Cookie()
		{
			this.expires = DateTime.MinValue;
			this.timestamp = DateTime.Now;
			this.domain = string.Empty;
			this.name = string.Empty;
			this.val = string.Empty;
			this.comment = string.Empty;
			this.port = string.Empty;
		}

		public Cookie(string name, string value)
			: this()
		{
			this.Name = name;
			this.Value = value;
		}

		public Cookie(string name, string value, string path)
			: this(name, value)
		{
			this.Path = path;
		}

		public Cookie(string name, string value, string path, string domain)
			: this(name, value, path)
		{
			this.Domain = domain;
		}

		public string Comment
		{
			get
			{
				return this.comment;
			}
			set
			{
				this.comment = ((value != null) ? value : string.Empty);
			}
		}

		public global::System.Uri CommentUri
		{
			get
			{
				return this.commentUri;
			}
			set
			{
				this.commentUri = value;
			}
		}

		public bool Discard
		{
			get
			{
				return this.discard;
			}
			set
			{
				this.discard = value;
			}
		}

		public string Domain
		{
			get
			{
				return this.domain;
			}
			set
			{
				if (Cookie.IsNullOrEmpty(value))
				{
					this.domain = string.Empty;
					this.ExactDomain = true;
				}
				else
				{
					this.domain = value;
					this.ExactDomain = value[0] != '.';
				}
			}
		}

		internal bool ExactDomain
		{
			get
			{
				return this.exact_domain;
			}
			set
			{
				this.exact_domain = value;
			}
		}

		public bool Expired
		{
			get
			{
				return this.expires <= DateTime.Now && this.expires != DateTime.MinValue;
			}
			set
			{
				if (value)
				{
					this.expires = DateTime.Now;
				}
			}
		}

		public DateTime Expires
		{
			get
			{
				return this.expires;
			}
			set
			{
				this.expires = value;
			}
		}

		public bool HttpOnly
		{
			get
			{
				return this.httpOnly;
			}
			set
			{
				this.httpOnly = value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (Cookie.IsNullOrEmpty(value))
				{
					throw new CookieException("Name cannot be empty");
				}
				if (value[0] == '$' || value.IndexOfAny(Cookie.reservedCharsName) != -1)
				{
					this.name = string.Empty;
					throw new CookieException("Name contains invalid characters");
				}
				this.name = value;
			}
		}

		public string Path
		{
			get
			{
				return (this.path != null) ? this.path : string.Empty;
			}
			set
			{
				this.path = ((value != null) ? value : string.Empty);
			}
		}

		public string Port
		{
			get
			{
				return this.port;
			}
			set
			{
				if (Cookie.IsNullOrEmpty(value))
				{
					this.port = string.Empty;
					return;
				}
				if (value[0] != '"' || value[value.Length - 1] != '"')
				{
					throw new CookieException("The 'Port'='" + value + "' part of the cookie is invalid. Port must be enclosed by double quotes.");
				}
				this.port = value;
				string[] array = this.port.Split(Cookie.portSeparators);
				this.ports = new int[array.Length];
				for (int i = 0; i < this.ports.Length; i++)
				{
					this.ports[i] = int.MinValue;
					if (array[i].Length != 0)
					{
						try
						{
							this.ports[i] = int.Parse(array[i]);
						}
						catch (Exception ex)
						{
							throw new CookieException("The 'Port'='" + value + "' part of the cookie is invalid. Invalid value: " + array[i], ex);
						}
					}
				}
				this.Version = 1;
			}
		}

		internal int[] Ports
		{
			get
			{
				return this.ports;
			}
		}

		public bool Secure
		{
			get
			{
				return this.secure;
			}
			set
			{
				this.secure = value;
			}
		}

		public DateTime TimeStamp
		{
			get
			{
				return this.timestamp;
			}
		}

		public string Value
		{
			get
			{
				return this.val;
			}
			set
			{
				if (value == null)
				{
					this.val = string.Empty;
					return;
				}
				this.val = value;
			}
		}

		public int Version
		{
			get
			{
				return this.version;
			}
			set
			{
				if (value < 0 || value > 10)
				{
					this.version = 0;
				}
				else
				{
					this.version = value;
				}
			}
		}

		public override bool Equals(object obj)
		{
			Cookie cookie = obj as Cookie;
			return cookie != null && string.Compare(this.name, cookie.name, true, CultureInfo.InvariantCulture) == 0 && string.Compare(this.val, cookie.val, false, CultureInfo.InvariantCulture) == 0 && string.Compare(this.Path, cookie.Path, false, CultureInfo.InvariantCulture) == 0 && string.Compare(this.domain, cookie.domain, true, CultureInfo.InvariantCulture) == 0 && this.version == cookie.version;
		}

		public override int GetHashCode()
		{
			return Cookie.hash(CaseInsensitiveHashCodeProvider.DefaultInvariant.GetHashCode(this.name), this.val.GetHashCode(), this.Path.GetHashCode(), CaseInsensitiveHashCodeProvider.DefaultInvariant.GetHashCode(this.domain), this.version);
		}

		private static int hash(int i, int j, int k, int l, int m)
		{
			return i ^ ((j << 13) | (j >> 19)) ^ ((k << 26) | (k >> 6)) ^ ((l << 7) | (l >> 25)) ^ ((m << 20) | (m >> 12));
		}

		public override string ToString()
		{
			return this.ToString(null);
		}

		internal string ToString(global::System.Uri uri)
		{
			if (this.name.Length == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(64);
			if (this.version > 0)
			{
				stringBuilder.Append("$Version=").Append(this.version).Append("; ");
			}
			stringBuilder.Append(this.name).Append("=").Append(this.val);
			if (this.version == 0)
			{
				return stringBuilder.ToString();
			}
			if (!Cookie.IsNullOrEmpty(this.path))
			{
				stringBuilder.Append("; $Path=").Append(this.path);
			}
			else if (uri != null)
			{
				stringBuilder.Append("; $Path=/").Append(this.path);
			}
			bool flag = uri == null || uri.Host != this.domain;
			if (flag && !Cookie.IsNullOrEmpty(this.domain))
			{
				stringBuilder.Append("; $Domain=").Append(this.domain);
			}
			if (this.port != null && this.port.Length != 0)
			{
				stringBuilder.Append("; $Port=").Append(this.port);
			}
			return stringBuilder.ToString();
		}

		internal string ToClientString()
		{
			if (this.name.Length == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(64);
			if (this.version > 0)
			{
				stringBuilder.Append("Version=").Append(this.version).Append(";");
			}
			stringBuilder.Append(this.name).Append("=").Append(this.val);
			if (this.path != null && this.path.Length != 0)
			{
				stringBuilder.Append(";Path=").Append(this.QuotedString(this.path));
			}
			if (this.domain != null && this.domain.Length != 0)
			{
				stringBuilder.Append(";Domain=").Append(this.QuotedString(this.domain));
			}
			if (this.port != null && this.port.Length != 0)
			{
				stringBuilder.Append(";Port=").Append(this.port);
			}
			return stringBuilder.ToString();
		}

		private string QuotedString(string value)
		{
			if (this.version == 0 || this.IsToken(value))
			{
				return value;
			}
			return "\"" + value.Replace("\"", "\\\"") + "\"";
		}

		private bool IsToken(string value)
		{
			int length = value.Length;
			for (int i = 0; i < length; i++)
			{
				char c = value[i];
				if (c < ' ' || c >= '\u007f' || Cookie.tspecials.IndexOf(c) != -1)
				{
					return false;
				}
			}
			return true;
		}

		private static bool IsNullOrEmpty(string s)
		{
			return s == null || s.Length == 0;
		}

		private string comment;

		private global::System.Uri commentUri;

		private bool discard;

		private string domain;

		private DateTime expires;

		private bool httpOnly;

		private string name;

		private string path;

		private string port;

		private int[] ports;

		private bool secure;

		private DateTime timestamp;

		private string val;

		private int version;

		private static char[] reservedCharsName = new char[] { ' ', '=', ';', ',', '\n', '\r', '\t' };

		private static char[] portSeparators = new char[] { '"', ',' };

		private static string tspecials = "()<>@,;:\\\"/[]?={} \t";

		private bool exact_domain;
	}
}
