using System;
using System.Collections;

namespace System.Net
{
	public class CredentialCache : IEnumerable, ICredentials, ICredentialsByHost
	{
		public CredentialCache()
		{
			this.cache = new Hashtable();
			this.cacheForHost = new Hashtable();
		}

		[global::System.MonoTODO("Need EnvironmentPermission implementation first")]
		public static ICredentials DefaultCredentials
		{
			get
			{
				return CredentialCache.empty;
			}
		}

		public static NetworkCredential DefaultNetworkCredentials
		{
			get
			{
				return CredentialCache.empty;
			}
		}

		public NetworkCredential GetCredential(global::System.Uri uriPrefix, string authType)
		{
			int num = -1;
			NetworkCredential networkCredential = null;
			if (uriPrefix == null || authType == null)
			{
				return null;
			}
			string text = uriPrefix.AbsolutePath;
			text = text.Substring(0, text.LastIndexOf('/'));
			IDictionaryEnumerator enumerator = this.cache.GetEnumerator();
			while (enumerator.MoveNext())
			{
				CredentialCache.CredentialCacheKey credentialCacheKey = enumerator.Key as CredentialCache.CredentialCacheKey;
				if (credentialCacheKey.Length > num)
				{
					if (string.Compare(credentialCacheKey.AuthType, authType, true) == 0)
					{
						global::System.Uri uriPrefix2 = credentialCacheKey.UriPrefix;
						if (!(uriPrefix2.Scheme != uriPrefix.Scheme))
						{
							if (uriPrefix2.Port == uriPrefix.Port)
							{
								if (!(uriPrefix2.Host != uriPrefix.Host))
								{
									if (text.StartsWith(credentialCacheKey.AbsPath))
									{
										num = credentialCacheKey.Length;
										networkCredential = (NetworkCredential)enumerator.Value;
									}
								}
							}
						}
					}
				}
			}
			return networkCredential;
		}

		public IEnumerator GetEnumerator()
		{
			return this.cache.Values.GetEnumerator();
		}

		public void Add(global::System.Uri uriPrefix, string authType, NetworkCredential cred)
		{
			if (uriPrefix == null)
			{
				throw new ArgumentNullException("uriPrefix");
			}
			if (authType == null)
			{
				throw new ArgumentNullException("authType");
			}
			this.cache.Add(new CredentialCache.CredentialCacheKey(uriPrefix, authType), cred);
		}

		public void Remove(global::System.Uri uriPrefix, string authType)
		{
			if (uriPrefix == null)
			{
				throw new ArgumentNullException("uriPrefix");
			}
			if (authType == null)
			{
				throw new ArgumentNullException("authType");
			}
			this.cache.Remove(new CredentialCache.CredentialCacheKey(uriPrefix, authType));
		}

		public NetworkCredential GetCredential(string host, int port, string authenticationType)
		{
			NetworkCredential networkCredential = null;
			if (host == null || port < 0 || authenticationType == null)
			{
				return null;
			}
			IDictionaryEnumerator enumerator = this.cacheForHost.GetEnumerator();
			while (enumerator.MoveNext())
			{
				CredentialCache.CredentialCacheForHostKey credentialCacheForHostKey = enumerator.Key as CredentialCache.CredentialCacheForHostKey;
				if (string.Compare(credentialCacheForHostKey.AuthType, authenticationType, true) == 0)
				{
					if (!(credentialCacheForHostKey.Host != host))
					{
						if (credentialCacheForHostKey.Port == port)
						{
							networkCredential = (NetworkCredential)enumerator.Value;
						}
					}
				}
			}
			return networkCredential;
		}

		public void Add(string host, int port, string authenticationType, NetworkCredential credential)
		{
			if (host == null)
			{
				throw new ArgumentNullException("host");
			}
			if (port < 0)
			{
				throw new ArgumentOutOfRangeException("port");
			}
			if (authenticationType == null)
			{
				throw new ArgumentOutOfRangeException("authenticationType");
			}
			this.cacheForHost.Add(new CredentialCache.CredentialCacheForHostKey(host, port, authenticationType), credential);
		}

		public void Remove(string host, int port, string authenticationType)
		{
			if (host == null)
			{
				return;
			}
			if (authenticationType == null)
			{
				return;
			}
			this.cacheForHost.Remove(new CredentialCache.CredentialCacheForHostKey(host, port, authenticationType));
		}

		private static NetworkCredential empty = new NetworkCredential(string.Empty, string.Empty, string.Empty);

		private Hashtable cache;

		private Hashtable cacheForHost;

		private class CredentialCacheKey
		{
			internal CredentialCacheKey(global::System.Uri uriPrefix, string authType)
			{
				this.uriPrefix = uriPrefix;
				this.authType = authType;
				this.absPath = uriPrefix.AbsolutePath;
				this.absPath = this.absPath.Substring(0, this.absPath.LastIndexOf('/'));
				this.len = uriPrefix.AbsoluteUri.Length;
				this.hash = uriPrefix.GetHashCode() + authType.GetHashCode();
			}

			public int Length
			{
				get
				{
					return this.len;
				}
			}

			public string AbsPath
			{
				get
				{
					return this.absPath;
				}
			}

			public global::System.Uri UriPrefix
			{
				get
				{
					return this.uriPrefix;
				}
			}

			public string AuthType
			{
				get
				{
					return this.authType;
				}
			}

			public override int GetHashCode()
			{
				return this.hash;
			}

			public override bool Equals(object obj)
			{
				CredentialCache.CredentialCacheKey credentialCacheKey = obj as CredentialCache.CredentialCacheKey;
				return credentialCacheKey != null && this.hash == credentialCacheKey.hash;
			}

			public override string ToString()
			{
				return string.Concat(new object[] { this.absPath, " : ", this.authType, " : len=", this.len });
			}

			private global::System.Uri uriPrefix;

			private string authType;

			private string absPath;

			private int len;

			private int hash;
		}

		private class CredentialCacheForHostKey
		{
			internal CredentialCacheForHostKey(string host, int port, string authType)
			{
				this.host = host;
				this.port = port;
				this.authType = authType;
				this.hash = host.GetHashCode() + port.GetHashCode() + authType.GetHashCode();
			}

			public string Host
			{
				get
				{
					return this.host;
				}
			}

			public int Port
			{
				get
				{
					return this.port;
				}
			}

			public string AuthType
			{
				get
				{
					return this.authType;
				}
			}

			public override int GetHashCode()
			{
				return this.hash;
			}

			public override bool Equals(object obj)
			{
				CredentialCache.CredentialCacheForHostKey credentialCacheForHostKey = obj as CredentialCache.CredentialCacheForHostKey;
				return credentialCacheForHostKey != null && this.hash == credentialCacheForHostKey.hash;
			}

			public override string ToString()
			{
				return this.host + " : " + this.authType;
			}

			private string host;

			private int port;

			private string authType;

			private int hash;
		}
	}
}
