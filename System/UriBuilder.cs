using System;
using System.Text;

namespace System
{
	public class UriBuilder
	{
		public UriBuilder()
			: this(global::System.Uri.UriSchemeHttp, "localhost")
		{
		}

		public UriBuilder(string uri)
			: this(new global::System.Uri(uri))
		{
		}

		public UriBuilder(global::System.Uri uri)
		{
			this.scheme = uri.Scheme;
			this.host = uri.Host;
			this.port = uri.Port;
			this.path = uri.AbsolutePath;
			this.query = uri.Query;
			this.fragment = uri.Fragment;
			this.username = uri.UserInfo;
			int num = this.username.IndexOf(':');
			if (num != -1)
			{
				this.password = this.username.Substring(num + 1);
				this.username = this.username.Substring(0, num);
			}
			else
			{
				this.password = string.Empty;
			}
			this.modified = true;
		}

		public UriBuilder(string schemeName, string hostName)
		{
			this.Scheme = schemeName;
			this.Host = hostName;
			this.port = -1;
			this.Path = string.Empty;
			this.query = string.Empty;
			this.fragment = string.Empty;
			this.username = string.Empty;
			this.password = string.Empty;
			this.modified = true;
		}

		public UriBuilder(string scheme, string host, int portNumber)
			: this(scheme, host)
		{
			this.Port = portNumber;
		}

		public UriBuilder(string scheme, string host, int port, string pathValue)
			: this(scheme, host, port)
		{
			this.Path = pathValue;
		}

		public UriBuilder(string scheme, string host, int port, string pathValue, string extraValue)
			: this(scheme, host, port, pathValue)
		{
			if (extraValue == null || extraValue.Length == 0)
			{
				return;
			}
			if (extraValue[0] == '#')
			{
				this.Fragment = extraValue.Remove(0, 1);
			}
			else
			{
				if (extraValue[0] != '?')
				{
					throw new ArgumentException("extraValue");
				}
				this.Query = extraValue.Remove(0, 1);
			}
		}

		public string Fragment
		{
			get
			{
				return this.fragment;
			}
			set
			{
				this.fragment = value;
				if (this.fragment == null)
				{
					this.fragment = string.Empty;
				}
				else if (this.fragment.Length > 0)
				{
					this.fragment = "#" + value.Replace("%23", "#");
				}
				this.modified = true;
			}
		}

		public string Host
		{
			get
			{
				return this.host;
			}
			set
			{
				this.host = ((value != null) ? value : string.Empty);
				this.modified = true;
			}
		}

		public string Password
		{
			get
			{
				return this.password;
			}
			set
			{
				this.password = ((value != null) ? value : string.Empty);
				this.modified = true;
			}
		}

		public string Path
		{
			get
			{
				return this.path;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					this.path = "/";
				}
				else
				{
					this.path = global::System.Uri.EscapeString(value.Replace('\\', '/'), false, true, true);
				}
				this.modified = true;
			}
		}

		public int Port
		{
			get
			{
				return this.port;
			}
			set
			{
				if (value < -1)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.port = value;
				this.modified = true;
			}
		}

		public string Query
		{
			get
			{
				return this.query;
			}
			set
			{
				if (value == null || value.Length == 0)
				{
					this.query = string.Empty;
				}
				else
				{
					this.query = "?" + value;
				}
				this.modified = true;
			}
		}

		public string Scheme
		{
			get
			{
				return this.scheme;
			}
			set
			{
				if (value == null)
				{
					value = string.Empty;
				}
				int num = value.IndexOf(':');
				if (num != -1)
				{
					value = value.Substring(0, num);
				}
				this.scheme = value.ToLower();
				this.modified = true;
			}
		}

		public global::System.Uri Uri
		{
			get
			{
				if (!this.modified)
				{
					return this.uri;
				}
				this.uri = new global::System.Uri(this.ToString(), true);
				this.modified = false;
				return this.uri;
			}
		}

		public string UserName
		{
			get
			{
				return this.username;
			}
			set
			{
				this.username = ((value != null) ? value : string.Empty);
				this.modified = true;
			}
		}

		public override bool Equals(object rparam)
		{
			return rparam != null && this.Uri.Equals(rparam.ToString());
		}

		public override int GetHashCode()
		{
			return this.Uri.GetHashCode();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.scheme);
			stringBuilder.Append("://");
			if (this.username != string.Empty)
			{
				stringBuilder.Append(this.username);
				if (this.password != string.Empty)
				{
					stringBuilder.Append(":" + this.password);
				}
				stringBuilder.Append('@');
			}
			stringBuilder.Append(this.host);
			if (this.port > 0)
			{
				stringBuilder.Append(":" + this.port);
			}
			if (this.path != string.Empty && stringBuilder[stringBuilder.Length - 1] != '/' && this.path.Length > 0 && this.path[0] != '/')
			{
				stringBuilder.Append('/');
			}
			stringBuilder.Append(this.path);
			stringBuilder.Append(this.query);
			stringBuilder.Append(this.fragment);
			return stringBuilder.ToString();
		}

		private string scheme;

		private string host;

		private int port;

		private string path;

		private string query;

		private string fragment;

		private string username;

		private string password;

		private global::System.Uri uri;

		private bool modified;
	}
}
