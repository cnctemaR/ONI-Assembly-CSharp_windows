using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace System
{
	[global::System.ComponentModel.TypeConverter(typeof(global::System.UriTypeConverter))]
	[Serializable]
	public class Uri : ISerializable
	{
		public Uri(string uriString)
			: this(uriString, false)
		{
		}

		protected Uri(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: this(serializationInfo.GetString("AbsoluteUri"), true)
		{
		}

		public Uri(string uriString, global::System.UriKind uriKind)
		{
			this.scheme = string.Empty;
			this.host = string.Empty;
			this.port = -1;
			this.path = string.Empty;
			this.query = string.Empty;
			this.fragment = string.Empty;
			this.userinfo = string.Empty;
			this.isAbsoluteUri = true;
			base..ctor();
			this.source = uriString;
			this.ParseUri(uriKind);
			switch (uriKind)
			{
			case global::System.UriKind.RelativeOrAbsolute:
				break;
			case global::System.UriKind.Absolute:
				if (!this.IsAbsoluteUri)
				{
					throw new global::System.UriFormatException("Invalid URI: The format of the URI could not be determined.");
				}
				break;
			case global::System.UriKind.Relative:
				if (this.IsAbsoluteUri)
				{
					throw new global::System.UriFormatException("Invalid URI: The format of the URI could not be determined because the parameter 'uriString' represents an absolute URI.");
				}
				break;
			default:
			{
				string text = global::Locale.GetText("Invalid UriKind value '{0}'.", new object[] { uriKind });
				throw new ArgumentException(text);
			}
			}
		}

		private Uri(string uriString, global::System.UriKind uriKind, out bool success)
		{
			this.scheme = string.Empty;
			this.host = string.Empty;
			this.port = -1;
			this.path = string.Empty;
			this.query = string.Empty;
			this.fragment = string.Empty;
			this.userinfo = string.Empty;
			this.isAbsoluteUri = true;
			base..ctor();
			if (uriString == null)
			{
				success = false;
				return;
			}
			if (uriKind != global::System.UriKind.RelativeOrAbsolute && uriKind != global::System.UriKind.Absolute && uriKind != global::System.UriKind.Relative)
			{
				string text = global::Locale.GetText("Invalid UriKind value '{0}'.", new object[] { uriKind });
				throw new ArgumentException(text);
			}
			this.source = uriString;
			if (this.ParseNoExceptions(uriKind, uriString) != null)
			{
				success = false;
			}
			else
			{
				success = true;
				switch (uriKind)
				{
				case global::System.UriKind.RelativeOrAbsolute:
					break;
				case global::System.UriKind.Absolute:
					if (!this.IsAbsoluteUri)
					{
						success = false;
					}
					break;
				case global::System.UriKind.Relative:
					if (this.IsAbsoluteUri)
					{
						success = false;
					}
					break;
				default:
					success = false;
					break;
				}
			}
		}

		public Uri(global::System.Uri baseUri, global::System.Uri relativeUri)
		{
			this.scheme = string.Empty;
			this.host = string.Empty;
			this.port = -1;
			this.path = string.Empty;
			this.query = string.Empty;
			this.fragment = string.Empty;
			this.userinfo = string.Empty;
			this.isAbsoluteUri = true;
			base..ctor();
			this.Merge(baseUri, (!(relativeUri == null)) ? relativeUri.OriginalString : string.Empty);
		}

		[Obsolete]
		public Uri(string uriString, bool dontEscape)
		{
			this.scheme = string.Empty;
			this.host = string.Empty;
			this.port = -1;
			this.path = string.Empty;
			this.query = string.Empty;
			this.fragment = string.Empty;
			this.userinfo = string.Empty;
			this.isAbsoluteUri = true;
			base..ctor();
			this.userEscaped = dontEscape;
			this.source = uriString;
			this.ParseUri(global::System.UriKind.Absolute);
			if (!this.isAbsoluteUri)
			{
				throw new global::System.UriFormatException("Invalid URI: The format of the URI could not be determined: " + uriString);
			}
		}

		public Uri(global::System.Uri baseUri, string relativeUri)
		{
			this.scheme = string.Empty;
			this.host = string.Empty;
			this.port = -1;
			this.path = string.Empty;
			this.query = string.Empty;
			this.fragment = string.Empty;
			this.userinfo = string.Empty;
			this.isAbsoluteUri = true;
			base..ctor();
			this.Merge(baseUri, relativeUri);
		}

		[Obsolete("dontEscape is always false")]
		public Uri(global::System.Uri baseUri, string relativeUri, bool dontEscape)
		{
			this.scheme = string.Empty;
			this.host = string.Empty;
			this.port = -1;
			this.path = string.Empty;
			this.query = string.Empty;
			this.fragment = string.Empty;
			this.userinfo = string.Empty;
			this.isAbsoluteUri = true;
			base..ctor();
			this.userEscaped = dontEscape;
			this.Merge(baseUri, relativeUri);
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("AbsoluteUri", this.AbsoluteUri);
		}

		private void Merge(global::System.Uri baseUri, string relativeUri)
		{
			if (baseUri == null)
			{
				throw new ArgumentNullException("baseUri");
			}
			if (!baseUri.IsAbsoluteUri)
			{
				throw new ArgumentOutOfRangeException("baseUri");
			}
			if (relativeUri == null)
			{
				relativeUri = string.Empty;
			}
			if (relativeUri.Length >= 2 && relativeUri[0] == '\\' && relativeUri[1] == '\\')
			{
				this.source = relativeUri;
				this.ParseUri(global::System.UriKind.Absolute);
				return;
			}
			int num = relativeUri.IndexOf(':');
			if (num != -1)
			{
				int num2 = relativeUri.IndexOfAny(new char[] { '/', '\\', '?' });
				if (num2 > num || num2 < 0)
				{
					if (string.CompareOrdinal(baseUri.Scheme, 0, relativeUri, 0, num) != 0 || !global::System.Uri.IsPredefinedScheme(baseUri.Scheme) || (relativeUri.Length > num + 1 && relativeUri[num + 1] == '/'))
					{
						this.source = relativeUri;
						this.ParseUri(global::System.UriKind.Absolute);
						return;
					}
					relativeUri = relativeUri.Substring(num + 1);
				}
			}
			this.scheme = baseUri.scheme;
			this.host = baseUri.host;
			this.port = baseUri.port;
			this.userinfo = baseUri.userinfo;
			this.isUnc = baseUri.isUnc;
			this.isUnixFilePath = baseUri.isUnixFilePath;
			this.isOpaquePart = baseUri.isOpaquePart;
			if (relativeUri == string.Empty)
			{
				this.path = baseUri.path;
				this.query = baseUri.query;
				this.fragment = baseUri.fragment;
				return;
			}
			num = relativeUri.IndexOf('#');
			if (num != -1)
			{
				if (this.userEscaped)
				{
					this.fragment = relativeUri.Substring(num);
				}
				else
				{
					this.fragment = "#" + global::System.Uri.EscapeString(relativeUri.Substring(num + 1));
				}
				relativeUri = relativeUri.Substring(0, num);
			}
			num = relativeUri.IndexOf('?');
			if (num != -1)
			{
				this.query = relativeUri.Substring(num);
				if (!this.userEscaped)
				{
					this.query = global::System.Uri.EscapeString(this.query);
				}
				relativeUri = relativeUri.Substring(0, num);
			}
			if (relativeUri.Length > 0 && relativeUri[0] == '/')
			{
				if (relativeUri.Length > 1 && relativeUri[1] == '/')
				{
					this.source = this.scheme + ':' + relativeUri;
					this.ParseUri(global::System.UriKind.Absolute);
					return;
				}
				this.path = relativeUri;
				if (!this.userEscaped)
				{
					this.path = global::System.Uri.EscapeString(this.path);
				}
				return;
			}
			else
			{
				this.path = baseUri.path;
				if (relativeUri.Length > 0 || this.query.Length > 0)
				{
					num = this.path.LastIndexOf('/');
					if (num >= 0)
					{
						this.path = this.path.Substring(0, num + 1);
					}
				}
				if (relativeUri.Length == 0)
				{
					return;
				}
				this.path += relativeUri;
				int num3 = 0;
				for (;;)
				{
					num = this.path.IndexOf("./", num3);
					if (num == -1)
					{
						break;
					}
					if (num == 0)
					{
						this.path = this.path.Remove(0, 2);
					}
					else if (this.path[num - 1] != '.')
					{
						this.path = this.path.Remove(num, 2);
					}
					else
					{
						num3 = num + 1;
					}
				}
				if (this.path.Length > 1 && this.path[this.path.Length - 1] == '.' && this.path[this.path.Length - 2] == '/')
				{
					this.path = this.path.Remove(this.path.Length - 1, 1);
				}
				num3 = 0;
				for (;;)
				{
					num = this.path.IndexOf("/../", num3);
					if (num == -1)
					{
						break;
					}
					if (num == 0)
					{
						num3 = 3;
					}
					else
					{
						int num4 = this.path.LastIndexOf('/', num - 1);
						if (num4 == -1)
						{
							num3 = num + 1;
						}
						else if (this.path.Substring(num4 + 1, num - num4 - 1) != "..")
						{
							this.path = this.path.Remove(num4 + 1, num - num4 + 3);
						}
						else
						{
							num3 = num + 1;
						}
					}
				}
				if (this.path.Length > 3 && this.path.EndsWith("/.."))
				{
					num = this.path.LastIndexOf('/', this.path.Length - 4);
					if (num != -1 && this.path.Substring(num + 1, this.path.Length - num - 4) != "..")
					{
						this.path = this.path.Remove(num + 1, this.path.Length - num - 1);
					}
				}
				if (!this.userEscaped)
				{
					this.path = global::System.Uri.EscapeString(this.path);
				}
				return;
			}
		}

		public string AbsolutePath
		{
			get
			{
				this.EnsureAbsoluteUri();
				string text = this.Scheme;
				if (text != null)
				{
					if (global::System.Uri.<>f__switch$map1C == null)
					{
						global::System.Uri.<>f__switch$map1C = new Dictionary<string, int>(2)
						{
							{ "mailto", 0 },
							{ "file", 0 }
						};
					}
					int num;
					if (global::System.Uri.<>f__switch$map1C.TryGetValue(text, out num))
					{
						if (num == 0)
						{
							return this.path;
						}
					}
				}
				if (this.path.Length != 0)
				{
					return this.path;
				}
				string text2 = this.Scheme + global::System.Uri.SchemeDelimiter;
				if (this.path.StartsWith(text2))
				{
					return "/";
				}
				return string.Empty;
			}
		}

		public string AbsoluteUri
		{
			get
			{
				this.EnsureAbsoluteUri();
				if (this.cachedAbsoluteUri == null)
				{
					this.cachedAbsoluteUri = this.GetLeftPart(global::System.UriPartial.Path);
					if (this.query.Length > 0)
					{
						this.cachedAbsoluteUri += this.query;
					}
					if (this.fragment.Length > 0)
					{
						this.cachedAbsoluteUri += this.fragment;
					}
				}
				return this.cachedAbsoluteUri;
			}
		}

		public string Authority
		{
			get
			{
				this.EnsureAbsoluteUri();
				return (global::System.Uri.GetDefaultPort(this.Scheme) != this.port) ? (this.host + ":" + this.port) : this.host;
			}
		}

		public string Fragment
		{
			get
			{
				this.EnsureAbsoluteUri();
				return this.fragment;
			}
		}

		public string Host
		{
			get
			{
				this.EnsureAbsoluteUri();
				return this.host;
			}
		}

		public global::System.UriHostNameType HostNameType
		{
			get
			{
				this.EnsureAbsoluteUri();
				global::System.UriHostNameType uriHostNameType = global::System.Uri.CheckHostName(this.Host);
				if (uriHostNameType != global::System.UriHostNameType.Unknown)
				{
					return uriHostNameType;
				}
				string text = this.Scheme;
				if (text != null)
				{
					if (global::System.Uri.<>f__switch$map1D == null)
					{
						global::System.Uri.<>f__switch$map1D = new Dictionary<string, int>(1) { { "mailto", 0 } };
					}
					int num;
					if (global::System.Uri.<>f__switch$map1D.TryGetValue(text, out num))
					{
						if (num == 0)
						{
							return global::System.UriHostNameType.Basic;
						}
					}
				}
				return (!this.IsFile) ? uriHostNameType : global::System.UriHostNameType.Basic;
			}
		}

		public bool IsDefaultPort
		{
			get
			{
				this.EnsureAbsoluteUri();
				return global::System.Uri.GetDefaultPort(this.Scheme) == this.port;
			}
		}

		public bool IsFile
		{
			get
			{
				this.EnsureAbsoluteUri();
				return this.Scheme == global::System.Uri.UriSchemeFile;
			}
		}

		public bool IsLoopback
		{
			get
			{
				this.EnsureAbsoluteUri();
				if (this.Host.Length == 0)
				{
					return this.IsFile;
				}
				global::System.Net.IPAddress ipaddress;
				global::System.Net.IPv6Address pv6Address;
				return this.host == "loopback" || this.host == "localhost" || (global::System.Net.IPAddress.TryParse(this.host, out ipaddress) && global::System.Net.IPAddress.Loopback.Equals(ipaddress)) || (global::System.Net.IPv6Address.TryParse(this.host, out pv6Address) && global::System.Net.IPv6Address.IsLoopback(pv6Address));
			}
		}

		public bool IsUnc
		{
			get
			{
				this.EnsureAbsoluteUri();
				return this.isUnc;
			}
		}

		public string LocalPath
		{
			get
			{
				this.EnsureAbsoluteUri();
				if (this.cachedLocalPath != null)
				{
					return this.cachedLocalPath;
				}
				if (!this.IsFile)
				{
					return this.AbsolutePath;
				}
				bool flag = this.path.Length > 3 && this.path[1] == ':' && (this.path[2] == '\\' || this.path[2] == '/');
				if (!this.IsUnc)
				{
					string text = this.Unescape(this.path);
					bool flag2 = flag;
					if (flag2)
					{
						this.cachedLocalPath = text.Replace('/', '\\');
					}
					else
					{
						this.cachedLocalPath = text;
					}
				}
				else if (this.path.Length > 1 && this.path[1] == ':')
				{
					this.cachedLocalPath = this.Unescape(this.path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
				}
				else if (Path.DirectorySeparatorChar == '\\')
				{
					string text2 = this.host;
					if (this.path.Length > 0 && (this.path.Length > 1 || this.path[0] != '/'))
					{
						text2 += this.path.Replace('/', '\\');
					}
					this.cachedLocalPath = "\\\\" + this.Unescape(text2);
				}
				else
				{
					this.cachedLocalPath = this.Unescape(this.path);
				}
				if (this.cachedLocalPath.Length == 0)
				{
					this.cachedLocalPath = Path.DirectorySeparatorChar.ToString();
				}
				return this.cachedLocalPath;
			}
		}

		public string PathAndQuery
		{
			get
			{
				this.EnsureAbsoluteUri();
				return this.path + this.Query;
			}
		}

		public int Port
		{
			get
			{
				this.EnsureAbsoluteUri();
				return this.port;
			}
		}

		public string Query
		{
			get
			{
				this.EnsureAbsoluteUri();
				return this.query;
			}
		}

		public string Scheme
		{
			get
			{
				this.EnsureAbsoluteUri();
				return this.scheme;
			}
		}

		public string[] Segments
		{
			get
			{
				this.EnsureAbsoluteUri();
				if (this.segments != null)
				{
					return this.segments;
				}
				if (this.path.Length == 0)
				{
					this.segments = new string[0];
					return this.segments;
				}
				string[] array = this.path.Split(new char[] { '/' });
				this.segments = array;
				bool flag = this.path.EndsWith("/");
				if (array.Length > 0 && flag)
				{
					string[] array2 = new string[array.Length - 1];
					Array.Copy(array, 0, array2, 0, array.Length - 1);
					array = array2;
				}
				int i = 0;
				if (this.IsFile && this.path.Length > 1 && this.path[1] == ':')
				{
					string[] array3 = new string[array.Length + 1];
					Array.Copy(array, 1, array3, 2, array.Length - 1);
					array = array3;
					array[0] = this.path.Substring(0, 2);
					array[1] = string.Empty;
					i++;
				}
				int num = array.Length;
				while (i < num)
				{
					if (i != num - 1 || flag)
					{
						string[] array4 = array;
						int num2 = i;
						array4[num2] += '/';
					}
					i++;
				}
				this.segments = array;
				return this.segments;
			}
		}

		public bool UserEscaped
		{
			get
			{
				return this.userEscaped;
			}
		}

		public string UserInfo
		{
			get
			{
				this.EnsureAbsoluteUri();
				return this.userinfo;
			}
		}

		[global::System.MonoTODO("add support for IPv6 address")]
		public string DnsSafeHost
		{
			get
			{
				this.EnsureAbsoluteUri();
				return this.Unescape(this.Host);
			}
		}

		public bool IsAbsoluteUri
		{
			get
			{
				return this.isAbsoluteUri;
			}
		}

		public string OriginalString
		{
			get
			{
				return (this.source == null) ? this.ToString() : this.source;
			}
		}

		public static global::System.UriHostNameType CheckHostName(string name)
		{
			if (name == null || name.Length == 0)
			{
				return global::System.UriHostNameType.Unknown;
			}
			if (global::System.Uri.IsIPv4Address(name))
			{
				return global::System.UriHostNameType.IPv4;
			}
			if (global::System.Uri.IsDomainAddress(name))
			{
				return global::System.UriHostNameType.Dns;
			}
			global::System.Net.IPv6Address pv6Address;
			if (global::System.Net.IPv6Address.TryParse(name, out pv6Address))
			{
				return global::System.UriHostNameType.IPv6;
			}
			return global::System.UriHostNameType.Unknown;
		}

		internal static bool IsIPv4Address(string name)
		{
			string[] array = name.Split(new char[] { '.' });
			if (array.Length != 4)
			{
				return false;
			}
			for (int i = 0; i < 4; i++)
			{
				if (array[i].Length == 0)
				{
					return false;
				}
				uint num;
				if (!uint.TryParse(array[i], out num))
				{
					return false;
				}
				if (num > 255U)
				{
					return false;
				}
			}
			return true;
		}

		internal static bool IsDomainAddress(string name)
		{
			int length = name.Length;
			int num = 0;
			for (int i = 0; i < length; i++)
			{
				char c = name[i];
				if (num == 0)
				{
					if (!char.IsLetterOrDigit(c))
					{
						return false;
					}
				}
				else if (c == '.')
				{
					num = 0;
				}
				else if (!char.IsLetterOrDigit(c) && c != '-' && c != '_')
				{
					return false;
				}
				if (++num == 64)
				{
					return false;
				}
			}
			return true;
		}

		[Obsolete("This method does nothing, it has been obsoleted")]
		protected virtual void Canonicalize()
		{
		}

		[Obsolete]
		[global::System.MonoTODO("Find out what this should do")]
		protected virtual void CheckSecurity()
		{
		}

		public static bool CheckSchemeName(string schemeName)
		{
			if (schemeName == null || schemeName.Length == 0)
			{
				return false;
			}
			if (!global::System.Uri.IsAlpha(schemeName[0]))
			{
				return false;
			}
			int length = schemeName.Length;
			for (int i = 1; i < length; i++)
			{
				char c = schemeName[i];
				if (!char.IsDigit(c) && !global::System.Uri.IsAlpha(c) && c != '.' && c != '+' && c != '-')
				{
					return false;
				}
			}
			return true;
		}

		private static bool IsAlpha(char c)
		{
			return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
		}

		public override bool Equals(object comparant)
		{
			if (comparant == null)
			{
				return false;
			}
			global::System.Uri uri = comparant as global::System.Uri;
			if (uri == null)
			{
				string text = comparant as string;
				if (text == null)
				{
					return false;
				}
				uri = new global::System.Uri(text);
			}
			return this.InternalEquals(uri);
		}

		private bool InternalEquals(global::System.Uri uri)
		{
			if (this.isAbsoluteUri != uri.isAbsoluteUri)
			{
				return false;
			}
			if (!this.isAbsoluteUri)
			{
				return this.source == uri.source;
			}
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			return this.scheme.ToLower(invariantCulture) == uri.scheme.ToLower(invariantCulture) && this.host.ToLower(invariantCulture) == uri.host.ToLower(invariantCulture) && this.port == uri.port && this.query == uri.query && this.path == uri.path;
		}

		public override int GetHashCode()
		{
			if (this.cachedHashCode == 0)
			{
				CultureInfo invariantCulture = CultureInfo.InvariantCulture;
				if (this.isAbsoluteUri)
				{
					this.cachedHashCode = this.scheme.ToLower(invariantCulture).GetHashCode() ^ this.host.ToLower(invariantCulture).GetHashCode() ^ this.port ^ this.query.GetHashCode() ^ this.path.GetHashCode();
				}
				else
				{
					this.cachedHashCode = this.source.GetHashCode();
				}
			}
			return this.cachedHashCode;
		}

		public string GetLeftPart(global::System.UriPartial part)
		{
			this.EnsureAbsoluteUri();
			switch (part)
			{
			case global::System.UriPartial.Scheme:
				return this.scheme + this.GetOpaqueWiseSchemeDelimiter();
			case global::System.UriPartial.Authority:
			{
				if (this.scheme == global::System.Uri.UriSchemeMailto || this.scheme == global::System.Uri.UriSchemeNews)
				{
					return string.Empty;
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.scheme);
				stringBuilder.Append(this.GetOpaqueWiseSchemeDelimiter());
				if (this.path.Length > 1 && this.path[1] == ':' && global::System.Uri.UriSchemeFile == this.scheme)
				{
					stringBuilder.Append('/');
				}
				if (this.userinfo.Length > 0)
				{
					stringBuilder.Append(this.userinfo).Append('@');
				}
				stringBuilder.Append(this.host);
				int num = global::System.Uri.GetDefaultPort(this.scheme);
				if (this.port != -1 && this.port != num)
				{
					stringBuilder.Append(':').Append(this.port);
				}
				return stringBuilder.ToString();
			}
			case global::System.UriPartial.Path:
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append(this.scheme);
				stringBuilder2.Append(this.GetOpaqueWiseSchemeDelimiter());
				if (this.path.Length > 1 && this.path[1] == ':' && global::System.Uri.UriSchemeFile == this.scheme)
				{
					stringBuilder2.Append('/');
				}
				if (this.userinfo.Length > 0)
				{
					stringBuilder2.Append(this.userinfo).Append('@');
				}
				stringBuilder2.Append(this.host);
				int num = global::System.Uri.GetDefaultPort(this.scheme);
				if (this.port != -1 && this.port != num)
				{
					stringBuilder2.Append(':').Append(this.port);
				}
				if (this.path.Length > 0)
				{
					string text = this.Scheme;
					if (text != null)
					{
						if (global::System.Uri.<>f__switch$map1E == null)
						{
							global::System.Uri.<>f__switch$map1E = new Dictionary<string, int>(2)
							{
								{ "mailto", 0 },
								{ "news", 0 }
							};
						}
						int num2;
						if (global::System.Uri.<>f__switch$map1E.TryGetValue(text, out num2))
						{
							if (num2 == 0)
							{
								stringBuilder2.Append(this.path);
								goto IL_02A6;
							}
						}
					}
					stringBuilder2.Append(global::System.Uri.Reduce(this.path, global::System.Uri.CompactEscaped(this.Scheme)));
				}
				IL_02A6:
				return stringBuilder2.ToString();
			}
			default:
				return null;
			}
		}

		public static int FromHex(char digit)
		{
			if ('0' <= digit && digit <= '9')
			{
				return (int)(digit - '0');
			}
			if ('a' <= digit && digit <= 'f')
			{
				return (int)(digit - 'a' + '\n');
			}
			if ('A' <= digit && digit <= 'F')
			{
				return (int)(digit - 'A' + '\n');
			}
			throw new ArgumentException("digit");
		}

		public static string HexEscape(char character)
		{
			if (character > 'ÿ')
			{
				throw new ArgumentOutOfRangeException("character");
			}
			return "%" + global::System.Uri.hexUpperChars[(int)((character & 'ð') >> 4)] + global::System.Uri.hexUpperChars[(int)(character & '\u000f')];
		}

		public static char HexUnescape(string pattern, ref int index)
		{
			if (pattern == null)
			{
				throw new ArgumentException("pattern");
			}
			if (index < 0 || index >= pattern.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (!global::System.Uri.IsHexEncoding(pattern, index))
			{
				return pattern[index++];
			}
			index++;
			int num = global::System.Uri.FromHex(pattern[index++]);
			int num2 = global::System.Uri.FromHex(pattern[index++]);
			return (char)((num << 4) | num2);
		}

		public static bool IsHexDigit(char digit)
		{
			return ('0' <= digit && digit <= '9') || ('a' <= digit && digit <= 'f') || ('A' <= digit && digit <= 'F');
		}

		public static bool IsHexEncoding(string pattern, int index)
		{
			return index + 3 <= pattern.Length && (pattern[index++] == '%' && global::System.Uri.IsHexDigit(pattern[index++])) && global::System.Uri.IsHexDigit(pattern[index]);
		}

		public global::System.Uri MakeRelativeUri(global::System.Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			if (this.Host != uri.Host || this.Scheme != uri.Scheme)
			{
				return uri;
			}
			string text = string.Empty;
			if (this.path != uri.path)
			{
				string[] array = this.Segments;
				string[] array2 = uri.Segments;
				int i = 0;
				int num = Math.Min(array.Length, array2.Length);
				while (i < num)
				{
					if (array[i] != array2[i])
					{
						break;
					}
					i++;
				}
				for (int j = i + 1; j < array.Length; j++)
				{
					text += "../";
				}
				for (int k = i; k < array2.Length; k++)
				{
					text += array2[k];
				}
			}
			uri.AppendQueryAndFragment(ref text);
			return new global::System.Uri(text, global::System.UriKind.Relative);
		}

		[Obsolete("Use MakeRelativeUri(Uri uri) instead.")]
		public string MakeRelative(global::System.Uri toUri)
		{
			if (this.Scheme != toUri.Scheme || this.Authority != toUri.Authority)
			{
				return toUri.ToString();
			}
			string text = string.Empty;
			if (this.path != toUri.path)
			{
				string[] array = this.Segments;
				string[] array2 = toUri.Segments;
				int i = 0;
				int num = Math.Min(array.Length, array2.Length);
				while (i < num)
				{
					if (array[i] != array2[i])
					{
						break;
					}
					i++;
				}
				for (int j = i + 1; j < array.Length; j++)
				{
					text += "../";
				}
				for (int k = i; k < array2.Length; k++)
				{
					text += array2[k];
				}
			}
			return text;
		}

		private void AppendQueryAndFragment(ref string result)
		{
			if (this.query.Length > 0)
			{
				string text = ((this.query[0] != '?') ? global::System.Uri.Unescape(this.query, false) : ('?' + global::System.Uri.Unescape(this.query.Substring(1), false)));
				result += text;
			}
			if (this.fragment.Length > 0)
			{
				result += this.fragment;
			}
		}

		public override string ToString()
		{
			if (this.cachedToString != null)
			{
				return this.cachedToString;
			}
			if (this.isAbsoluteUri)
			{
				this.cachedToString = global::System.Uri.Unescape(this.GetLeftPart(global::System.UriPartial.Path), true);
			}
			else
			{
				this.cachedToString = this.Unescape(this.path);
			}
			this.AppendQueryAndFragment(ref this.cachedToString);
			return this.cachedToString;
		}

		protected void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("AbsoluteUri", this.AbsoluteUri);
		}

		[Obsolete]
		protected virtual void Escape()
		{
			this.path = global::System.Uri.EscapeString(this.path);
		}

		[Obsolete]
		protected static string EscapeString(string str)
		{
			return global::System.Uri.EscapeString(str, false, true, true);
		}

		internal static string EscapeString(string str, bool escapeReserved, bool escapeHex, bool escapeBrackets)
		{
			if (str == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int length = str.Length;
			for (int i = 0; i < length; i++)
			{
				if (global::System.Uri.IsHexEncoding(str, i))
				{
					stringBuilder.Append(str.Substring(i, 3));
					i += 2;
				}
				else
				{
					byte[] bytes = Encoding.UTF8.GetBytes(new char[] { str[i] });
					int num = bytes.Length;
					for (int j = 0; j < num; j++)
					{
						char c = (char)bytes[j];
						if (c <= ' ' || c >= '\u007f' || "<>%\"{}|\\^`".IndexOf(c) != -1 || (escapeHex && c == '#') || (escapeBrackets && (c == '[' || c == ']')) || (escapeReserved && ";/?:@&=+$,".IndexOf(c) != -1))
						{
							stringBuilder.Append(global::System.Uri.HexEscape(c));
						}
						else
						{
							stringBuilder.Append(c);
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		[Obsolete("The method has been deprecated. It is not used by the system.")]
		protected virtual void Parse()
		{
		}

		private void ParseUri(global::System.UriKind kind)
		{
			this.Parse(kind, this.source);
			if (this.userEscaped)
			{
				return;
			}
			this.host = global::System.Uri.EscapeString(this.host, false, true, false);
			if (this.host.Length > 1 && this.host[0] != '[' && this.host[this.host.Length - 1] != ']')
			{
				this.host = this.host.ToLower(CultureInfo.InvariantCulture);
			}
			if (this.path.Length > 0)
			{
				this.path = global::System.Uri.EscapeString(this.path);
			}
		}

		[Obsolete]
		protected virtual string Unescape(string str)
		{
			return global::System.Uri.Unescape(str, false);
		}

		internal static string Unescape(string str, bool excludeSpecial)
		{
			if (str == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int length = str.Length;
			for (int i = 0; i < length; i++)
			{
				char c = str[i];
				if (c == '%')
				{
					char c3;
					char c2 = global::System.Uri.HexUnescapeMultiByte(str, ref i, out c3);
					if (excludeSpecial && c2 == '#')
					{
						stringBuilder.Append("%23");
					}
					else if (excludeSpecial && c2 == '%')
					{
						stringBuilder.Append("%25");
					}
					else if (excludeSpecial && c2 == '?')
					{
						stringBuilder.Append("%3F");
					}
					else
					{
						stringBuilder.Append(c2);
						if (c3 != '\0')
						{
							stringBuilder.Append(c3);
						}
					}
					i--;
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		private void ParseAsWindowsUNC(string uriString)
		{
			this.scheme = global::System.Uri.UriSchemeFile;
			this.port = -1;
			this.fragment = string.Empty;
			this.query = string.Empty;
			this.isUnc = true;
			uriString = uriString.TrimStart(new char[] { '\\' });
			int num = uriString.IndexOf('\\');
			if (num > 0)
			{
				this.path = uriString.Substring(num);
				this.host = uriString.Substring(0, num);
			}
			else
			{
				this.host = uriString;
				this.path = string.Empty;
			}
			this.path = this.path.Replace("\\", "/");
		}

		private string ParseAsWindowsAbsoluteFilePath(string uriString)
		{
			if (uriString.Length > 2 && uriString[2] != '\\' && uriString[2] != '/')
			{
				return "Relative file path is not allowed.";
			}
			this.scheme = global::System.Uri.UriSchemeFile;
			this.host = string.Empty;
			this.port = -1;
			this.path = uriString.Replace("\\", "/");
			this.fragment = string.Empty;
			this.query = string.Empty;
			return null;
		}

		private void ParseAsUnixAbsoluteFilePath(string uriString)
		{
			this.isUnixFilePath = true;
			this.scheme = global::System.Uri.UriSchemeFile;
			this.port = -1;
			this.fragment = string.Empty;
			this.query = string.Empty;
			this.host = string.Empty;
			this.path = null;
			if (uriString.Length >= 2 && uriString[0] == '/' && uriString[1] == '/')
			{
				uriString = uriString.TrimStart(new char[] { '/' });
				this.path = '/' + uriString;
			}
			if (this.path == null)
			{
				this.path = uriString;
			}
		}

		private void Parse(global::System.UriKind kind, string uriString)
		{
			if (uriString == null)
			{
				throw new ArgumentNullException("uriString");
			}
			string text = this.ParseNoExceptions(kind, uriString);
			if (text != null)
			{
				throw new global::System.UriFormatException(text);
			}
		}

		private string ParseNoExceptions(global::System.UriKind kind, string uriString)
		{
			uriString = uriString.Trim();
			int length = uriString.Length;
			if (length == 0 && (kind == global::System.UriKind.Relative || kind == global::System.UriKind.RelativeOrAbsolute))
			{
				this.isAbsoluteUri = false;
				return null;
			}
			if (length <= 1 && kind != global::System.UriKind.Relative)
			{
				return "Absolute URI is too short";
			}
			int num = uriString.IndexOf(':');
			if (num == 0)
			{
				return "Invalid URI: The format of the URI could not be determined.";
			}
			if (num < 0)
			{
				if (uriString[0] == '/' && Path.DirectorySeparatorChar == '/')
				{
					this.ParseAsUnixAbsoluteFilePath(uriString);
					if (kind == global::System.UriKind.Relative)
					{
						this.isAbsoluteUri = false;
					}
				}
				else if (uriString.Length >= 2 && uriString[0] == '\\' && uriString[1] == '\\')
				{
					this.ParseAsWindowsUNC(uriString);
				}
				else
				{
					this.isAbsoluteUri = false;
					this.path = uriString;
				}
				return null;
			}
			if (num == 1)
			{
				if (!global::System.Uri.IsAlpha(uriString[0]))
				{
					return "URI scheme must start with a letter.";
				}
				string text = this.ParseAsWindowsAbsoluteFilePath(uriString);
				if (text != null)
				{
					return text;
				}
				return null;
			}
			else
			{
				this.scheme = uriString.Substring(0, num).ToLower(CultureInfo.InvariantCulture);
				if (!global::System.Uri.CheckSchemeName(this.scheme))
				{
					return global::Locale.GetText("URI scheme must start with a letter and must consist of one of alphabet, digits, '+', '-' or '.' character.");
				}
				int num2 = num + 1;
				int num3 = uriString.Length;
				num = uriString.IndexOf('#', num2);
				if (!this.IsUnc && num != -1)
				{
					if (this.userEscaped)
					{
						this.fragment = uriString.Substring(num);
					}
					else
					{
						this.fragment = "#" + global::System.Uri.EscapeString(uriString.Substring(num + 1));
					}
					num3 = num;
				}
				num = uriString.IndexOf('?', num2, num3 - num2);
				if (num != -1)
				{
					this.query = uriString.Substring(num, num3 - num);
					num3 = num;
					if (!this.userEscaped)
					{
						this.query = global::System.Uri.EscapeString(this.query);
					}
				}
				if (global::System.Uri.IsPredefinedScheme(this.scheme) && this.scheme != global::System.Uri.UriSchemeMailto && this.scheme != global::System.Uri.UriSchemeNews && (num3 - num2 < 2 || (num3 - num2 >= 2 && uriString[num2] == '/' && uriString[num2 + 1] != '/')))
				{
					return "Invalid URI: The Authority/Host could not be parsed.";
				}
				bool flag = num3 - num2 >= 2 && uriString[num2] == '/' && uriString[num2 + 1] == '/';
				bool flag2 = this.scheme == global::System.Uri.UriSchemeFile && flag && (num3 - num2 == 2 || uriString[num2 + 2] == '/');
				bool flag3 = false;
				if (flag)
				{
					if (kind == global::System.UriKind.Relative)
					{
						return "Absolute URI when we expected a relative one";
					}
					if (this.scheme != global::System.Uri.UriSchemeMailto && this.scheme != global::System.Uri.UriSchemeNews)
					{
						num2 += 2;
					}
					if (this.scheme == global::System.Uri.UriSchemeFile)
					{
						int num4 = 2;
						for (int i = num2; i < num3; i++)
						{
							if (uriString[i] != '/')
							{
								break;
							}
							num4++;
						}
						if (num4 >= 4)
						{
							flag2 = false;
							while (num2 < num3 && uriString[num2] == '/')
							{
								num2++;
							}
						}
						else if (num4 >= 3)
						{
							num2++;
						}
					}
					if (num3 - num2 > 1 && uriString[num2 + 1] == ':')
					{
						flag2 = false;
						flag3 = true;
					}
				}
				else if (!global::System.Uri.IsPredefinedScheme(this.scheme))
				{
					this.path = uriString.Substring(num2, num3 - num2);
					this.isOpaquePart = true;
					return null;
				}
				if (flag2)
				{
					num = -1;
				}
				else
				{
					num = uriString.IndexOf('/', num2, num3 - num2);
					if (num == -1 && flag3)
					{
						num = uriString.IndexOf('\\', num2, num3 - num2);
					}
				}
				if (num == -1)
				{
					if (this.scheme != global::System.Uri.UriSchemeMailto && this.scheme != global::System.Uri.UriSchemeNews)
					{
						this.path = "/";
					}
				}
				else
				{
					this.path = uriString.Substring(num, num3 - num);
					num3 = num;
				}
				if (flag2)
				{
					num = -1;
				}
				else
				{
					num = uriString.IndexOf('@', num2, num3 - num2);
				}
				if (num != -1)
				{
					this.userinfo = uriString.Substring(num2, num - num2);
					num2 = num + 1;
				}
				this.port = -1;
				if (flag2)
				{
					num = -1;
				}
				else
				{
					num = uriString.LastIndexOf(':', num3 - 1, num3 - num2);
				}
				if (num != -1 && num != num3 - 1)
				{
					string text2 = uriString.Substring(num + 1, num3 - (num + 1));
					if (text2.Length > 0 && text2[text2.Length - 1] != ']')
					{
						if (!int.TryParse(text2, NumberStyles.Integer, CultureInfo.InvariantCulture, out this.port) || this.port < 0 || this.port > 65535)
						{
							return "Invalid URI: Invalid port number";
						}
						num3 = num;
					}
					else if (this.port == -1)
					{
						this.port = global::System.Uri.GetDefaultPort(this.scheme);
					}
				}
				else if (this.port == -1)
				{
					this.port = global::System.Uri.GetDefaultPort(this.scheme);
				}
				uriString = uriString.Substring(num2, num3 - num2);
				this.host = uriString;
				if (flag2)
				{
					this.path = global::System.Uri.Reduce('/' + uriString, true);
					this.host = string.Empty;
				}
				else if (this.host.Length == 2 && this.host[1] == ':')
				{
					this.path = this.host + this.path;
					this.host = string.Empty;
				}
				else if (this.isUnixFilePath)
				{
					uriString = "//" + uriString;
					this.host = string.Empty;
				}
				else if (this.scheme == global::System.Uri.UriSchemeFile)
				{
					this.isUnc = true;
				}
				else if (this.scheme == global::System.Uri.UriSchemeNews)
				{
					if (this.host.Length > 0)
					{
						this.path = this.host;
						this.host = string.Empty;
					}
				}
				else if (this.host.Length == 0 && (this.scheme == global::System.Uri.UriSchemeHttp || this.scheme == global::System.Uri.UriSchemeGopher || this.scheme == global::System.Uri.UriSchemeNntp || this.scheme == global::System.Uri.UriSchemeHttps || this.scheme == global::System.Uri.UriSchemeFtp))
				{
					return "Invalid URI: The hostname could not be parsed";
				}
				bool flag4 = this.host.Length > 0 && global::System.Uri.CheckHostName(this.host) == global::System.UriHostNameType.Unknown;
				if (!flag4 && this.host.Length > 1 && this.host[0] == '[' && this.host[this.host.Length - 1] == ']')
				{
					global::System.Net.IPv6Address pv6Address;
					if (global::System.Net.IPv6Address.TryParse(this.host, out pv6Address))
					{
						this.host = "[" + pv6Address.ToString(true) + "]";
					}
					else
					{
						flag4 = true;
					}
				}
				if (flag4 && (this.Parser is global::System.DefaultUriParser || this.Parser == null))
				{
					return global::Locale.GetText("Invalid URI: The hostname could not be parsed. (" + this.host + ")");
				}
				global::System.UriFormatException ex = null;
				if (this.Parser != null)
				{
					this.Parser.InitializeAndValidate(this, out ex);
				}
				if (ex != null)
				{
					return ex.Message;
				}
				if (this.scheme != global::System.Uri.UriSchemeMailto && this.scheme != global::System.Uri.UriSchemeNews && this.scheme != global::System.Uri.UriSchemeFile)
				{
					this.path = global::System.Uri.Reduce(this.path, global::System.Uri.CompactEscaped(this.scheme));
				}
				return null;
			}
		}

		private static bool CompactEscaped(string scheme)
		{
			if (scheme != null)
			{
				if (global::System.Uri.<>f__switch$map1F == null)
				{
					global::System.Uri.<>f__switch$map1F = new Dictionary<string, int>(5)
					{
						{ "file", 0 },
						{ "http", 0 },
						{ "https", 0 },
						{ "net.pipe", 0 },
						{ "net.tcp", 0 }
					};
				}
				int num;
				if (global::System.Uri.<>f__switch$map1F.TryGetValue(scheme, out num))
				{
					if (num == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		private static string Reduce(string path, bool compact_escaped)
		{
			if (path == "/")
			{
				return path;
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (compact_escaped)
			{
				for (int i = 0; i < path.Length; i++)
				{
					char c = path[i];
					char c2 = c;
					if (c2 != '%')
					{
						if (c2 != '\\')
						{
							stringBuilder.Append(c);
						}
						else
						{
							stringBuilder.Append('/');
						}
					}
					else if (i < path.Length - 2)
					{
						char c3 = path[i + 1];
						char c4 = char.ToUpper(path[i + 2]);
						if ((c3 == '2' && c4 == 'F') || (c3 == '5' && c4 == 'C'))
						{
							stringBuilder.Append('/');
							i += 2;
						}
						else
						{
							stringBuilder.Append(c);
						}
					}
					else
					{
						stringBuilder.Append(c);
					}
				}
				path = stringBuilder.ToString();
			}
			else
			{
				path = path.Replace('\\', '/');
			}
			ArrayList arrayList = new ArrayList();
			int j = 0;
			while (j < path.Length)
			{
				int num = path.IndexOf('/', j);
				if (num == -1)
				{
					num = path.Length;
				}
				string text = path.Substring(j, num - j);
				j = num + 1;
				if (text.Length != 0 && !(text == "."))
				{
					if (text == "..")
					{
						int count = arrayList.Count;
						if (count != 0)
						{
							arrayList.RemoveAt(count - 1);
						}
					}
					else
					{
						arrayList.Add(text);
					}
				}
			}
			if (arrayList.Count == 0)
			{
				return "/";
			}
			stringBuilder.Length = 0;
			if (path[0] == '/')
			{
				stringBuilder.Append('/');
			}
			bool flag = true;
			foreach (object obj in arrayList)
			{
				string text2 = (string)obj;
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append('/');
				}
				stringBuilder.Append(text2);
			}
			if (path.EndsWith("/"))
			{
				stringBuilder.Append('/');
			}
			return stringBuilder.ToString();
		}

		private static char HexUnescapeMultiByte(string pattern, ref int index, out char surrogate)
		{
			surrogate = '\0';
			if (pattern == null)
			{
				throw new ArgumentException("pattern");
			}
			if (index < 0 || index >= pattern.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (!global::System.Uri.IsHexEncoding(pattern, index))
			{
				return pattern[index++];
			}
			int num = index++;
			int num2 = global::System.Uri.FromHex(pattern[index++]);
			int num3 = global::System.Uri.FromHex(pattern[index++]);
			int num4 = num2;
			int num5 = 0;
			while ((num4 & 8) == 8)
			{
				num5++;
				num4 <<= 1;
			}
			if (num5 <= 1)
			{
				return (char)((num2 << 4) | num3);
			}
			byte[] array = new byte[num5];
			bool flag = false;
			array[0] = (byte)((num2 << 4) | num3);
			for (int i = 1; i < num5; i++)
			{
				if (!global::System.Uri.IsHexEncoding(pattern, index++))
				{
					flag = true;
					break;
				}
				int num6 = global::System.Uri.FromHex(pattern[index++]);
				if ((num6 & 12) != 8)
				{
					flag = true;
					break;
				}
				int num7 = global::System.Uri.FromHex(pattern[index++]);
				array[i] = (byte)((num6 << 4) | num7);
			}
			if (flag)
			{
				index = num + 3;
				return (char)array[0];
			}
			byte b = byte.MaxValue;
			b = (byte)(b >> num5 + 1);
			int num8 = (int)(array[0] & b);
			for (int j = 1; j < num5; j++)
			{
				num8 <<= 6;
				num8 |= (int)(array[j] & 63);
			}
			if (num8 <= 65535)
			{
				return (char)num8;
			}
			num8 -= 65536;
			surrogate = (char)((num8 & 1023) | 56320);
			return (char)((num8 >> 10) | 55296);
		}

		internal static string GetSchemeDelimiter(string scheme)
		{
			for (int i = 0; i < global::System.Uri.schemes.Length; i++)
			{
				if (global::System.Uri.schemes[i].scheme == scheme)
				{
					return global::System.Uri.schemes[i].delimiter;
				}
			}
			return global::System.Uri.SchemeDelimiter;
		}

		internal static int GetDefaultPort(string scheme)
		{
			global::System.UriParser uriParser = global::System.UriParser.GetParser(scheme);
			if (uriParser == null)
			{
				return -1;
			}
			return uriParser.DefaultPort;
		}

		private string GetOpaqueWiseSchemeDelimiter()
		{
			if (this.isOpaquePart)
			{
				return ":";
			}
			return global::System.Uri.GetSchemeDelimiter(this.scheme);
		}

		[Obsolete]
		protected virtual bool IsBadFileSystemCharacter(char ch)
		{
			if (ch < ' ' || (ch < '@' && ch > '9'))
			{
				return true;
			}
			switch (ch)
			{
			case '*':
			case ',':
			case '/':
				break;
			default:
				switch (ch)
				{
				case '\\':
				case '^':
					break;
				default:
					if (ch != '\0' && ch != '"' && ch != '&' && ch != '|')
					{
						return false;
					}
					break;
				}
				break;
			}
			return true;
		}

		[Obsolete]
		protected static bool IsExcludedCharacter(char ch)
		{
			return ch <= ' ' || ch >= '\u007f' || (ch == '"' || ch == '#' || ch == '%' || ch == '<' || ch == '>' || ch == '[' || ch == '\\' || ch == ']' || ch == '^' || ch == '`' || ch == '{' || ch == '|' || ch == '}');
		}

		internal static bool MaybeUri(string s)
		{
			int num = s.IndexOf(':');
			return num != -1 && num < 10 && global::System.Uri.IsPredefinedScheme(s.Substring(0, num));
		}

		private static bool IsPredefinedScheme(string scheme)
		{
			if (scheme != null)
			{
				if (global::System.Uri.<>f__switch$map20 == null)
				{
					global::System.Uri.<>f__switch$map20 = new Dictionary<string, int>(10)
					{
						{ "http", 0 },
						{ "https", 0 },
						{ "file", 0 },
						{ "ftp", 0 },
						{ "nntp", 0 },
						{ "gopher", 0 },
						{ "mailto", 0 },
						{ "news", 0 },
						{ "net.pipe", 0 },
						{ "net.tcp", 0 }
					};
				}
				int num;
				if (global::System.Uri.<>f__switch$map20.TryGetValue(scheme, out num))
				{
					if (num == 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		[Obsolete]
		protected virtual bool IsReservedCharacter(char ch)
		{
			return ch == '$' || ch == '&' || ch == '+' || ch == ',' || ch == '/' || ch == ':' || ch == ';' || ch == '=' || ch == '@';
		}

		private global::System.UriParser Parser
		{
			get
			{
				if (this.parser == null)
				{
					this.parser = global::System.UriParser.GetParser(this.Scheme);
					if (this.parser == null)
					{
						this.parser = new global::System.DefaultUriParser("*");
					}
				}
				return this.parser;
			}
			set
			{
				this.parser = value;
			}
		}

		public string GetComponents(global::System.UriComponents components, global::System.UriFormat format)
		{
			return this.Parser.GetComponents(this, components, format);
		}

		public bool IsBaseOf(global::System.Uri uri)
		{
			return this.Parser.IsBaseOf(this, uri);
		}

		public bool IsWellFormedOriginalString()
		{
			return global::System.Uri.EscapeString(this.OriginalString) == this.OriginalString;
		}

		public static int Compare(global::System.Uri uri1, global::System.Uri uri2, global::System.UriComponents partsToCompare, global::System.UriFormat compareFormat, StringComparison comparisonType)
		{
			if (comparisonType < StringComparison.CurrentCulture || comparisonType > StringComparison.OrdinalIgnoreCase)
			{
				string text = global::Locale.GetText("Invalid StringComparison value '{0}'", new object[] { comparisonType });
				throw new ArgumentException("comparisonType", text);
			}
			if (uri1 == null && uri2 == null)
			{
				return 0;
			}
			string components = uri1.GetComponents(partsToCompare, compareFormat);
			string components2 = uri2.GetComponents(partsToCompare, compareFormat);
			return string.Compare(components, components2, comparisonType);
		}

		private static bool NeedToEscapeDataChar(char b)
		{
			return (b < 'A' || b > 'Z') && (b < 'a' || b > 'z') && (b < '0' || b > '9') && b != '_' && b != '~' && b != '!' && b != '\'' && b != '(' && b != ')' && b != '*' && b != '-' && b != '.';
		}

		public static string EscapeDataString(string stringToEscape)
		{
			if (stringToEscape == null)
			{
				throw new ArgumentNullException("stringToEscape");
			}
			if (stringToEscape.Length > 32766)
			{
				string text = global::Locale.GetText("Uri is longer than the maximum {0} characters.");
				throw new global::System.UriFormatException(text);
			}
			bool flag = false;
			foreach (char c in stringToEscape)
			{
				if (global::System.Uri.NeedToEscapeDataChar(c))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return stringToEscape;
			}
			StringBuilder stringBuilder = new StringBuilder();
			byte[] bytes = Encoding.UTF8.GetBytes(stringToEscape);
			foreach (byte b in bytes)
			{
				if (global::System.Uri.NeedToEscapeDataChar((char)b))
				{
					stringBuilder.Append(global::System.Uri.HexEscape((char)b));
				}
				else
				{
					stringBuilder.Append((char)b);
				}
			}
			return stringBuilder.ToString();
		}

		private static bool NeedToEscapeUriChar(char b)
		{
			return (b < 'A' || b > 'Z') && (b < 'a' || b > 'z') && (b < '&' || b > ';') && b != '!' && b != '#' && b != '$' && b != '=' && b != '?' && b != '@' && b != '_' && b != '~';
		}

		public static string EscapeUriString(string stringToEscape)
		{
			if (stringToEscape == null)
			{
				throw new ArgumentNullException("stringToEscape");
			}
			if (stringToEscape.Length > 32766)
			{
				string text = global::Locale.GetText("Uri is longer than the maximum {0} characters.");
				throw new global::System.UriFormatException(text);
			}
			bool flag = false;
			foreach (char c in stringToEscape)
			{
				if (global::System.Uri.NeedToEscapeUriChar(c))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return stringToEscape;
			}
			StringBuilder stringBuilder = new StringBuilder();
			byte[] bytes = Encoding.UTF8.GetBytes(stringToEscape);
			foreach (byte b in bytes)
			{
				if (global::System.Uri.NeedToEscapeUriChar((char)b))
				{
					stringBuilder.Append(global::System.Uri.HexEscape((char)b));
				}
				else
				{
					stringBuilder.Append((char)b);
				}
			}
			return stringBuilder.ToString();
		}

		public static bool IsWellFormedUriString(string uriString, global::System.UriKind uriKind)
		{
			global::System.Uri uri;
			return uriString != null && global::System.Uri.TryCreate(uriString, uriKind, out uri) && uri.IsWellFormedOriginalString();
		}

		public static bool TryCreate(string uriString, global::System.UriKind uriKind, out global::System.Uri result)
		{
			bool flag;
			global::System.Uri uri = new global::System.Uri(uriString, uriKind, out flag);
			if (flag)
			{
				result = uri;
				return true;
			}
			result = null;
			return false;
		}

		public static bool TryCreate(global::System.Uri baseUri, string relativeUri, out global::System.Uri result)
		{
			bool flag;
			try
			{
				result = new global::System.Uri(baseUri, relativeUri);
				flag = true;
			}
			catch (global::System.UriFormatException)
			{
				result = null;
				flag = false;
			}
			return flag;
		}

		public static bool TryCreate(global::System.Uri baseUri, global::System.Uri relativeUri, out global::System.Uri result)
		{
			bool flag;
			try
			{
				result = new global::System.Uri(baseUri, relativeUri.OriginalString);
				flag = true;
			}
			catch (global::System.UriFormatException)
			{
				result = null;
				flag = false;
			}
			return flag;
		}

		public static string UnescapeDataString(string stringToUnescape)
		{
			if (stringToUnescape == null)
			{
				throw new ArgumentNullException("stringToUnescape");
			}
			if (stringToUnescape.IndexOf('%') == -1 && stringToUnescape.IndexOf('+') == -1)
			{
				return stringToUnescape;
			}
			StringBuilder stringBuilder = new StringBuilder();
			long num = (long)stringToUnescape.Length;
			MemoryStream memoryStream = new MemoryStream();
			int num2 = 0;
			while ((long)num2 < num)
			{
				if (stringToUnescape[num2] == '%' && (long)(num2 + 2) < num && stringToUnescape[num2 + 1] != '%')
				{
					int num3;
					if (stringToUnescape[num2 + 1] == 'u' && (long)(num2 + 5) < num)
					{
						if (memoryStream.Length > 0L)
						{
							stringBuilder.Append(global::System.Uri.GetChars(memoryStream, Encoding.UTF8));
							memoryStream.SetLength(0L);
						}
						num3 = global::System.Uri.GetChar(stringToUnescape, num2 + 2, 4);
						if (num3 != -1)
						{
							stringBuilder.Append((char)num3);
							num2 += 5;
						}
						else
						{
							stringBuilder.Append('%');
						}
					}
					else if ((num3 = global::System.Uri.GetChar(stringToUnescape, num2 + 1, 2)) != -1)
					{
						memoryStream.WriteByte((byte)num3);
						num2 += 2;
					}
					else
					{
						stringBuilder.Append('%');
					}
				}
				else
				{
					if (memoryStream.Length > 0L)
					{
						stringBuilder.Append(global::System.Uri.GetChars(memoryStream, Encoding.UTF8));
						memoryStream.SetLength(0L);
					}
					stringBuilder.Append(stringToUnescape[num2]);
				}
				num2++;
			}
			if (memoryStream.Length > 0L)
			{
				stringBuilder.Append(global::System.Uri.GetChars(memoryStream, Encoding.UTF8));
			}
			return stringBuilder.ToString();
		}

		private static int GetInt(byte b)
		{
			char c = (char)b;
			if (c >= '0' && c <= '9')
			{
				return (int)(c - '0');
			}
			if (c >= 'a' && c <= 'f')
			{
				return (int)(c - 'a' + '\n');
			}
			if (c >= 'A' && c <= 'F')
			{
				return (int)(c - 'A' + '\n');
			}
			return -1;
		}

		private static int GetChar(string str, int offset, int length)
		{
			int num = 0;
			int num2 = length + offset;
			for (int i = offset; i < num2; i++)
			{
				char c = str[i];
				if (c > '\u007f')
				{
					return -1;
				}
				int @int = global::System.Uri.GetInt((byte)c);
				if (@int == -1)
				{
					return -1;
				}
				num = (num << 4) + @int;
			}
			return num;
		}

		private static char[] GetChars(MemoryStream b, Encoding e)
		{
			return e.GetChars(b.GetBuffer(), 0, (int)b.Length);
		}

		private void EnsureAbsoluteUri()
		{
			if (!this.IsAbsoluteUri)
			{
				throw new InvalidOperationException("This operation is not supported for a relative URI.");
			}
		}

		public static bool operator ==(global::System.Uri u1, global::System.Uri u2)
		{
			return object.Equals(u1, u2);
		}

		public static bool operator !=(global::System.Uri u1, global::System.Uri u2)
		{
			return !(u1 == u2);
		}

		private const int MaxUriLength = 32766;

		private bool isUnixFilePath;

		private string source;

		private string scheme;

		private string host;

		private int port;

		private string path;

		private string query;

		private string fragment;

		private string userinfo;

		private bool isUnc;

		private bool isOpaquePart;

		private bool isAbsoluteUri;

		private string[] segments;

		private bool userEscaped;

		private string cachedAbsoluteUri;

		private string cachedToString;

		private string cachedLocalPath;

		private int cachedHashCode;

		private static readonly string hexUpperChars = "0123456789ABCDEF";

		public static readonly string SchemeDelimiter = "://";

		public static readonly string UriSchemeFile = "file";

		public static readonly string UriSchemeFtp = "ftp";

		public static readonly string UriSchemeGopher = "gopher";

		public static readonly string UriSchemeHttp = "http";

		public static readonly string UriSchemeHttps = "https";

		public static readonly string UriSchemeMailto = "mailto";

		public static readonly string UriSchemeNews = "news";

		public static readonly string UriSchemeNntp = "nntp";

		public static readonly string UriSchemeNetPipe = "net.pipe";

		public static readonly string UriSchemeNetTcp = "net.tcp";

		private static global::System.Uri.UriScheme[] schemes = new global::System.Uri.UriScheme[]
		{
			new global::System.Uri.UriScheme(global::System.Uri.UriSchemeHttp, global::System.Uri.SchemeDelimiter, 80),
			new global::System.Uri.UriScheme(global::System.Uri.UriSchemeHttps, global::System.Uri.SchemeDelimiter, 443),
			new global::System.Uri.UriScheme(global::System.Uri.UriSchemeFtp, global::System.Uri.SchemeDelimiter, 21),
			new global::System.Uri.UriScheme(global::System.Uri.UriSchemeFile, global::System.Uri.SchemeDelimiter, -1),
			new global::System.Uri.UriScheme(global::System.Uri.UriSchemeMailto, ":", 25),
			new global::System.Uri.UriScheme(global::System.Uri.UriSchemeNews, ":", 119),
			new global::System.Uri.UriScheme(global::System.Uri.UriSchemeNntp, global::System.Uri.SchemeDelimiter, 119),
			new global::System.Uri.UriScheme(global::System.Uri.UriSchemeGopher, global::System.Uri.SchemeDelimiter, 70)
		};

		[NonSerialized]
		private global::System.UriParser parser;

		private struct UriScheme
		{
			public UriScheme(string s, string d, int p)
			{
				this.scheme = s;
				this.delimiter = d;
				this.defaultPort = p;
			}

			public string scheme;

			public string delimiter;

			public int defaultPort;
		}
	}
}
