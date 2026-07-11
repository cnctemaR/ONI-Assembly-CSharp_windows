using System;
using System.Collections;
using System.Globalization;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;

namespace System
{
	public abstract class UriParser
	{
		private static global::System.Text.RegularExpressions.Match ParseAuthority(global::System.Text.RegularExpressions.Group g)
		{
			return global::System.UriParser.auth_regex.Match(g.Value);
		}

		protected internal virtual string GetComponents(global::System.Uri uri, global::System.UriComponents components, global::System.UriFormat format)
		{
			if (format < global::System.UriFormat.UriEscaped || format > global::System.UriFormat.SafeUnescaped)
			{
				throw new ArgumentOutOfRangeException("format");
			}
			global::System.Text.RegularExpressions.Match match = global::System.UriParser.uri_regex.Match(uri.OriginalString);
			string value = this.scheme_name;
			int defaultPort = this.default_port;
			if (value == null || value == "*")
			{
				value = match.Groups[2].Value;
				defaultPort = global::System.Uri.GetDefaultPort(value);
			}
			else if (string.Compare(value, match.Groups[2].Value, true) != 0)
			{
				throw new SystemException("URI Parser: scheme mismatch: " + value + " vs. " + match.Groups[2].Value);
			}
			global::System.UriComponents uriComponents = components;
			switch (uriComponents)
			{
			case global::System.UriComponents.Scheme:
				return value;
			case global::System.UriComponents.UserInfo:
				return global::System.UriParser.ParseAuthority(match.Groups[4]).Groups[2].Value;
			default:
			{
				if (uriComponents == global::System.UriComponents.Path)
				{
					return this.Format(this.IgnoreFirstCharIf(match.Groups[5].Value, '/'), format);
				}
				if (uriComponents == global::System.UriComponents.Query)
				{
					return this.Format(match.Groups[7].Value, format);
				}
				if (uriComponents == global::System.UriComponents.Fragment)
				{
					return this.Format(match.Groups[9].Value, format);
				}
				if (uriComponents != global::System.UriComponents.StrongPort)
				{
					if (uriComponents == global::System.UriComponents.SerializationInfoString)
					{
						components = global::System.UriComponents.AbsoluteUri;
					}
					global::System.Text.RegularExpressions.Match match2 = global::System.UriParser.ParseAuthority(match.Groups[4]);
					StringBuilder stringBuilder = new StringBuilder();
					if ((components & global::System.UriComponents.Scheme) != (global::System.UriComponents)0)
					{
						stringBuilder.Append(value);
						stringBuilder.Append(global::System.Uri.GetSchemeDelimiter(value));
					}
					if ((components & global::System.UriComponents.UserInfo) != (global::System.UriComponents)0)
					{
						stringBuilder.Append(match2.Groups[1].Value);
					}
					if ((components & global::System.UriComponents.Host) != (global::System.UriComponents)0)
					{
						stringBuilder.Append(match2.Groups[3].Value);
					}
					if ((components & global::System.UriComponents.StrongPort) != (global::System.UriComponents)0)
					{
						global::System.Text.RegularExpressions.Group group = match2.Groups[4];
						stringBuilder.Append((!group.Success) ? (":" + defaultPort) : group.Value);
					}
					if ((components & global::System.UriComponents.Port) != (global::System.UriComponents)0)
					{
						string value2 = match2.Groups[5].Value;
						if (value2 != null && value2 != string.Empty && value2 != defaultPort.ToString())
						{
							stringBuilder.Append(match2.Groups[4].Value);
						}
					}
					if ((components & global::System.UriComponents.Path) != (global::System.UriComponents)0)
					{
						stringBuilder.Append(match.Groups[5]);
					}
					if ((components & global::System.UriComponents.Query) != (global::System.UriComponents)0)
					{
						stringBuilder.Append(match.Groups[6]);
					}
					if ((components & global::System.UriComponents.Fragment) != (global::System.UriComponents)0)
					{
						stringBuilder.Append(match.Groups[8]);
					}
					return this.Format(stringBuilder.ToString(), format);
				}
				global::System.Text.RegularExpressions.Group group2 = global::System.UriParser.ParseAuthority(match.Groups[4]).Groups[5];
				return (!group2.Success) ? defaultPort.ToString() : group2.Value;
			}
			case global::System.UriComponents.Host:
				return global::System.UriParser.ParseAuthority(match.Groups[4]).Groups[3].Value;
			case global::System.UriComponents.Port:
			{
				string value3 = global::System.UriParser.ParseAuthority(match.Groups[4]).Groups[5].Value;
				if (value3 != null && value3 != string.Empty && value3 != defaultPort.ToString())
				{
					return value3;
				}
				return string.Empty;
			}
			}
		}

		protected internal virtual void InitializeAndValidate(global::System.Uri uri, out global::System.UriFormatException parsingError)
		{
			if (uri.Scheme != this.scheme_name && this.scheme_name != "*")
			{
				parsingError = new global::System.UriFormatException("The argument Uri's scheme does not match");
			}
			else
			{
				parsingError = null;
			}
		}

		protected internal virtual bool IsBaseOf(global::System.Uri baseUri, global::System.Uri relativeUri)
		{
			if (global::System.Uri.Compare(baseUri, relativeUri, global::System.UriComponents.Scheme | global::System.UriComponents.UserInfo | global::System.UriComponents.Host | global::System.UriComponents.Port, global::System.UriFormat.Unescaped, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				return false;
			}
			string localPath = baseUri.LocalPath;
			int num = localPath.LastIndexOf('/') + 1;
			return string.Compare(localPath, 0, relativeUri.LocalPath, 0, num, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

		protected internal virtual bool IsWellFormedOriginalString(global::System.Uri uri)
		{
			return uri.IsWellFormedOriginalString();
		}

		protected internal virtual global::System.UriParser OnNewUri()
		{
			return this;
		}

		[global::System.MonoTODO]
		protected virtual void OnRegister(string schemeName, int defaultPort)
		{
		}

		[global::System.MonoTODO]
		protected internal virtual string Resolve(global::System.Uri baseUri, global::System.Uri relativeUri, out global::System.UriFormatException parsingError)
		{
			throw new NotImplementedException();
		}

		internal string SchemeName
		{
			get
			{
				return this.scheme_name;
			}
			set
			{
				this.scheme_name = value;
			}
		}

		internal int DefaultPort
		{
			get
			{
				return this.default_port;
			}
			set
			{
				this.default_port = value;
			}
		}

		private string IgnoreFirstCharIf(string s, char c)
		{
			if (s.Length == 0)
			{
				return string.Empty;
			}
			if (s[0] == c)
			{
				return s.Substring(1);
			}
			return s;
		}

		private string Format(string s, global::System.UriFormat format)
		{
			if (s.Length == 0)
			{
				return string.Empty;
			}
			switch (format)
			{
			case global::System.UriFormat.UriEscaped:
				return global::System.Uri.EscapeString(s, false, true, true);
			case global::System.UriFormat.Unescaped:
				return global::System.Uri.Unescape(s, false);
			case global::System.UriFormat.SafeUnescaped:
				s = global::System.Uri.Unescape(s, false);
				return s;
			default:
				throw new ArgumentOutOfRangeException("format");
			}
		}

		private static void CreateDefaults()
		{
			if (global::System.UriParser.table != null)
			{
				return;
			}
			Hashtable hashtable = new Hashtable();
			global::System.UriParser.InternalRegister(hashtable, new global::System.DefaultUriParser(), global::System.Uri.UriSchemeFile, -1);
			global::System.UriParser.InternalRegister(hashtable, new global::System.DefaultUriParser(), global::System.Uri.UriSchemeFtp, 21);
			global::System.UriParser.InternalRegister(hashtable, new global::System.DefaultUriParser(), global::System.Uri.UriSchemeGopher, 70);
			global::System.UriParser.InternalRegister(hashtable, new global::System.DefaultUriParser(), global::System.Uri.UriSchemeHttp, 80);
			global::System.UriParser.InternalRegister(hashtable, new global::System.DefaultUriParser(), global::System.Uri.UriSchemeHttps, 443);
			global::System.UriParser.InternalRegister(hashtable, new global::System.DefaultUriParser(), global::System.Uri.UriSchemeMailto, 25);
			global::System.UriParser.InternalRegister(hashtable, new global::System.DefaultUriParser(), global::System.Uri.UriSchemeNetPipe, -1);
			global::System.UriParser.InternalRegister(hashtable, new global::System.DefaultUriParser(), global::System.Uri.UriSchemeNetTcp, -1);
			global::System.UriParser.InternalRegister(hashtable, new global::System.DefaultUriParser(), global::System.Uri.UriSchemeNews, 119);
			global::System.UriParser.InternalRegister(hashtable, new global::System.DefaultUriParser(), global::System.Uri.UriSchemeNntp, 119);
			global::System.UriParser.InternalRegister(hashtable, new global::System.DefaultUriParser(), "ldap", 389);
			object obj = global::System.UriParser.lock_object;
			lock (obj)
			{
				if (global::System.UriParser.table == null)
				{
					global::System.UriParser.table = hashtable;
				}
			}
		}

		public static bool IsKnownScheme(string schemeName)
		{
			if (schemeName == null)
			{
				throw new ArgumentNullException("schemeName");
			}
			if (schemeName.Length == 0)
			{
				throw new ArgumentOutOfRangeException("schemeName");
			}
			global::System.UriParser.CreateDefaults();
			string text = schemeName.ToLower(CultureInfo.InvariantCulture);
			return global::System.UriParser.table[text] != null;
		}

		private static void InternalRegister(Hashtable table, global::System.UriParser uriParser, string schemeName, int defaultPort)
		{
			uriParser.SchemeName = schemeName;
			uriParser.DefaultPort = defaultPort;
			if (uriParser is global::System.GenericUriParser)
			{
				table.Add(schemeName, uriParser);
			}
			else
			{
				table.Add(schemeName, new global::System.DefaultUriParser
				{
					SchemeName = schemeName,
					DefaultPort = defaultPort
				});
			}
			uriParser.OnRegister(schemeName, defaultPort);
		}

		[PermissionSet(SecurityAction.Demand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"Infrastructure\"/>\n</PermissionSet>\n")]
		public static void Register(global::System.UriParser uriParser, string schemeName, int defaultPort)
		{
			if (uriParser == null)
			{
				throw new ArgumentNullException("uriParser");
			}
			if (schemeName == null)
			{
				throw new ArgumentNullException("schemeName");
			}
			if (defaultPort < -1 || defaultPort >= 65535)
			{
				throw new ArgumentOutOfRangeException("defaultPort");
			}
			global::System.UriParser.CreateDefaults();
			string text = schemeName.ToLower(CultureInfo.InvariantCulture);
			if (global::System.UriParser.table[text] != null)
			{
				string text2 = global::Locale.GetText("Scheme '{0}' is already registred.");
				throw new InvalidOperationException(text2);
			}
			global::System.UriParser.InternalRegister(global::System.UriParser.table, uriParser, text, defaultPort);
		}

		internal static global::System.UriParser GetParser(string schemeName)
		{
			if (schemeName == null)
			{
				return null;
			}
			global::System.UriParser.CreateDefaults();
			string text = schemeName.ToLower(CultureInfo.InvariantCulture);
			return (global::System.UriParser)global::System.UriParser.table[text];
		}

		private static object lock_object = new object();

		private static Hashtable table;

		internal string scheme_name;

		private int default_port;

		private static readonly global::System.Text.RegularExpressions.Regex uri_regex = new global::System.Text.RegularExpressions.Regex("^(([^:/?#]+):)?(//([^/?#]*))?([^?#]*)(\\?([^#]*))?(#(.*))?", global::System.Text.RegularExpressions.RegexOptions.Compiled);

		private static readonly global::System.Text.RegularExpressions.Regex auth_regex = new global::System.Text.RegularExpressions.Regex("^(([^@]+)@)?(.*?)(:([0-9]+))?$");
	}
}
