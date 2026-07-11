using System;

namespace System
{
	internal class UriData : global::System.IUriData
	{
		public UriData(global::System.Uri uri, global::System.UriParser parser)
		{
			this.uri = uri;
			this.parser = parser;
		}

		private string Lookup(ref string cache, global::System.UriComponents components)
		{
			return this.Lookup(ref cache, components, (!this.uri.UserEscaped) ? global::System.UriFormat.UriEscaped : global::System.UriFormat.Unescaped);
		}

		private string Lookup(ref string cache, global::System.UriComponents components, global::System.UriFormat format)
		{
			if (cache == null)
			{
				cache = this.parser.GetComponents(this.uri, components, format);
			}
			return cache;
		}

		public string AbsolutePath
		{
			get
			{
				return this.Lookup(ref this.absolute_path, global::System.UriComponents.Path | global::System.UriComponents.KeepDelimiter);
			}
		}

		public string AbsoluteUri
		{
			get
			{
				return this.Lookup(ref this.absolute_uri, global::System.UriComponents.AbsoluteUri);
			}
		}

		public string AbsoluteUri_SafeUnescaped
		{
			get
			{
				return this.Lookup(ref this.absolute_uri_unescaped, global::System.UriComponents.AbsoluteUri, global::System.UriFormat.SafeUnescaped);
			}
		}

		public string Authority
		{
			get
			{
				return this.Lookup(ref this.authority, global::System.UriComponents.Host | global::System.UriComponents.Port);
			}
		}

		public string Fragment
		{
			get
			{
				return this.Lookup(ref this.fragment, global::System.UriComponents.Fragment | global::System.UriComponents.KeepDelimiter);
			}
		}

		public string Host
		{
			get
			{
				return this.Lookup(ref this.host, global::System.UriComponents.Host);
			}
		}

		public string PathAndQuery
		{
			get
			{
				return this.Lookup(ref this.path_and_query, global::System.UriComponents.PathAndQuery);
			}
		}

		public string StrongPort
		{
			get
			{
				return this.Lookup(ref this.strong_port, global::System.UriComponents.StrongPort);
			}
		}

		public string Query
		{
			get
			{
				return this.Lookup(ref this.query, global::System.UriComponents.Query | global::System.UriComponents.KeepDelimiter);
			}
		}

		public string UserInfo
		{
			get
			{
				return this.Lookup(ref this.user_info, global::System.UriComponents.UserInfo);
			}
		}

		private global::System.Uri uri;

		private global::System.UriParser parser;

		private string absolute_path;

		private string absolute_uri;

		private string absolute_uri_unescaped;

		private string authority;

		private string fragment;

		private string host;

		private string path_and_query;

		private string strong_port;

		private string query;

		private string user_info;
	}
}
