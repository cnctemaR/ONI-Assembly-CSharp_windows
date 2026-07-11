using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Mono.Security
{
	internal class Uri
	{
		public Uri(string uriString)
			: this(uriString, false)
		{
		}

		public Uri(string uriString, bool dontEscape)
		{
			this.scheme = string.Empty;
			this.host = string.Empty;
			this.port = -1;
			this.path = string.Empty;
			this.query = string.Empty;
			this.fragment = string.Empty;
			this.userinfo = string.Empty;
			this.reduce = true;
			base..ctor();
			this.userEscaped = dontEscape;
			this.source = uriString;
			this.Parse();
		}

		public Uri(string uriString, bool dontEscape, bool reduce)
		{
			this.scheme = string.Empty;
			this.host = string.Empty;
			this.port = -1;
			this.path = string.Empty;
			this.query = string.Empty;
			this.fragment = string.Empty;
			this.userinfo = string.Empty;
			this.reduce = true;
			base..ctor();
			this.userEscaped = dontEscape;
			this.source = uriString;
			this.reduce = reduce;
			this.Parse();
		}

		public Uri(Uri baseUri, string relativeUri)
			: this(baseUri, relativeUri, false)
		{
		}

		public Uri(Uri baseUri, string relativeUri, bool dontEscape)
		{
			this.scheme = string.Empty;
			this.host = string.Empty;
			this.port = -1;
			this.path = string.Empty;
			this.query = string.Empty;
			this.fragment = string.Empty;
			this.userinfo = string.Empty;
			this.reduce = true;
			base..ctor();
			if (baseUri == null)
			{
				throw new NullReferenceException("baseUri");
			}
			this.userEscaped = dontEscape;
			if (relativeUri == null)
			{
				throw new NullReferenceException("relativeUri");
			}
			if (relativeUri.StartsWith("\\\\"))
			{
				this.source = relativeUri;
				this.Parse();
				return;
			}
			int num = relativeUri.IndexOf(':');
			if (num != -1)
			{
				int num2 = relativeUri.IndexOfAny(new char[] { '/', '\\', '?' });
				if (num2 > num || num2 < 0)
				{
					this.source = relativeUri;
					this.Parse();
					return;
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
				this.fragment = relativeUri.Substring(num);
				relativeUri = relativeUri.Substring(0, num);
			}
			num = relativeUri.IndexOf('?');
			if (num != -1)
			{
				this.query = relativeUri.Substring(num);
				if (!this.userEscaped)
				{
					this.query = Uri.EscapeString(this.query);
				}
				relativeUri = relativeUri.Substring(0, num);
			}
			if (relativeUri.Length > 0 && relativeUri[0] == '/')
			{
				if (relativeUri.Length > 1 && relativeUri[1] == '/')
				{
					this.source = this.scheme + ":" + relativeUri;
					this.Parse();
					return;
				}
				this.path = relativeUri;
				if (!this.userEscaped)
				{
					this.path = Uri.EscapeString(this.path);
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
					this.path = Uri.EscapeString(this.path);
				}
				return;
			}
		}

		public string AbsolutePath
		{
			get
			{
				return this.path;
			}
		}

		public string AbsoluteUri
		{
			get
			{
				if (this.cachedAbsoluteUri == null)
				{
					this.cachedAbsoluteUri = this.GetLeftPart(UriPartial.Path) + this.query + this.fragment;
				}
				return this.cachedAbsoluteUri;
			}
		}

		public string Authority
		{
			get
			{
				if (Uri.GetDefaultPort(this.scheme) != this.port)
				{
					return this.host + ":" + this.port;
				}
				return this.host;
			}
		}

		public string Fragment
		{
			get
			{
				return this.fragment;
			}
		}

		public string Host
		{
			get
			{
				return this.host;
			}
		}

		public bool IsDefaultPort
		{
			get
			{
				return Uri.GetDefaultPort(this.scheme) == this.port;
			}
		}

		public bool IsFile
		{
			get
			{
				return this.scheme == Uri.UriSchemeFile;
			}
		}

		public bool IsLoopback
		{
			get
			{
				return !(this.host == string.Empty) && (this.host == "loopback" || this.host == "localhost");
			}
		}

		public bool IsUnc
		{
			get
			{
				return this.isUnc;
			}
		}

		public string LocalPath
		{
			get
			{
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
					if (Path.DirectorySeparatorChar == '\\' || flag)
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
					this.cachedLocalPath = "\\\\" + this.Unescape(this.host + this.path.Replace('/', '\\'));
				}
				else
				{
					this.cachedLocalPath = this.Unescape(this.path);
				}
				if (this.cachedLocalPath == string.Empty)
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
				return this.path + this.query;
			}
		}

		public int Port
		{
			get
			{
				return this.port;
			}
		}

		public string Query
		{
			get
			{
				return this.query;
			}
		}

		public string Scheme
		{
			get
			{
				return this.scheme;
			}
		}

		public string[] Segments
		{
			get
			{
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
				if (array.Length != 0 && flag)
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
						array4[num2] += "/";
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
				return this.userinfo;
			}
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
				try
				{
					int num = int.Parse(array[i], CultureInfo.InvariantCulture);
					if (num < 0 || num > 255)
					{
						return false;
					}
				}
				catch (Exception)
				{
					return false;
				}
			}
			return true;
		}

		internal static bool IsDomainAddress(string name)
		{
			int length = name.Length;
			if (name[length - 1] == '.')
			{
				return false;
			}
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

		public static bool CheckSchemeName(string schemeName)
		{
			if (schemeName == null || schemeName.Length == 0)
			{
				return false;
			}
			if (!char.IsLetter(schemeName[0]))
			{
				return false;
			}
			int length = schemeName.Length;
			for (int i = 1; i < length; i++)
			{
				char c = schemeName[i];
				if (!char.IsLetterOrDigit(c) && c != '.' && c != '+' && c != '-')
				{
					return false;
				}
			}
			return true;
		}

		public override bool Equals(object comparant)
		{
			if (comparant == null)
			{
				return false;
			}
			Uri uri = comparant as Uri;
			if (uri == null)
			{
				string text = comparant as string;
				if (text == null)
				{
					return false;
				}
				uri = new Uri(text);
			}
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			return this.scheme.ToLower(invariantCulture) == uri.scheme.ToLower(invariantCulture) && this.userinfo.ToLower(invariantCulture) == uri.userinfo.ToLower(invariantCulture) && this.host.ToLower(invariantCulture) == uri.host.ToLower(invariantCulture) && this.port == uri.port && this.path == uri.path && this.query.ToLower(invariantCulture) == uri.query.ToLower(invariantCulture);
		}

		public override int GetHashCode()
		{
			if (this.cachedHashCode == 0)
			{
				this.cachedHashCode = this.scheme.GetHashCode() + this.userinfo.GetHashCode() + this.host.GetHashCode() + this.port + this.path.GetHashCode() + this.query.GetHashCode();
			}
			return this.cachedHashCode;
		}

		public string GetLeftPart(UriPartial part)
		{
			switch (part)
			{
			case UriPartial.Scheme:
				return this.scheme + this.GetOpaqueWiseSchemeDelimiter();
			case UriPartial.Authority:
			{
				if (this.host == string.Empty || this.scheme == Uri.UriSchemeMailto || this.scheme == Uri.UriSchemeNews)
				{
					return string.Empty;
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.scheme);
				stringBuilder.Append(this.GetOpaqueWiseSchemeDelimiter());
				if (this.path.Length > 1 && this.path[1] == ':' && Uri.UriSchemeFile == this.scheme)
				{
					stringBuilder.Append('/');
				}
				if (this.userinfo.Length > 0)
				{
					stringBuilder.Append(this.userinfo).Append('@');
				}
				stringBuilder.Append(this.host);
				int num = Uri.GetDefaultPort(this.scheme);
				if (this.port != -1 && this.port != num)
				{
					stringBuilder.Append(':').Append(this.port);
				}
				return stringBuilder.ToString();
			}
			case UriPartial.Path:
			{
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder2.Append(this.scheme);
				stringBuilder2.Append(this.GetOpaqueWiseSchemeDelimiter());
				if (this.path.Length > 1 && this.path[1] == ':' && Uri.UriSchemeFile == this.scheme)
				{
					stringBuilder2.Append('/');
				}
				if (this.userinfo.Length > 0)
				{
					stringBuilder2.Append(this.userinfo).Append('@');
				}
				stringBuilder2.Append(this.host);
				int num = Uri.GetDefaultPort(this.scheme);
				if (this.port != -1 && this.port != num)
				{
					stringBuilder2.Append(':').Append(this.port);
				}
				stringBuilder2.Append(this.path);
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
			return "%" + Uri.hexUpperChars[(int)((character & 'ð') >> 4)].ToString() + Uri.hexUpperChars[(int)(character & '\u000f')].ToString();
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
			int num = 0;
			int num2 = 0;
			while (index + 3 <= pattern.Length && pattern[index] == '%' && Uri.IsHexDigit(pattern[index + 1]) && Uri.IsHexDigit(pattern[index + 2]))
			{
				index++;
				int num3 = index;
				index = num3 + 1;
				int num4 = Uri.FromHex(pattern[num3]);
				num3 = index;
				index = num3 + 1;
				int num5 = Uri.FromHex(pattern[num3]);
				int num6 = (num4 << 4) + num5;
				if (num == 0)
				{
					if (num6 < 192)
					{
						return (char)num6;
					}
					if (num6 < 224)
					{
						num2 = num6 - 192;
						num = 2;
					}
					else if (num6 < 240)
					{
						num2 = num6 - 224;
						num = 3;
					}
					else if (num6 < 248)
					{
						num2 = num6 - 240;
						num = 4;
					}
					else if (num6 < 251)
					{
						num2 = num6 - 248;
						num = 5;
					}
					else if (num6 < 254)
					{
						num2 = num6 - 252;
						num = 6;
					}
					num2 <<= (num - 1) * 6;
				}
				else
				{
					num2 += num6 - 128 << (num - 1) * 6;
				}
				num--;
				if (num <= 0)
				{
					IL_0154:
					return (char)num2;
				}
			}
			if (num == 0)
			{
				int num3 = index;
				index = num3 + 1;
				return pattern[num3];
			}
			goto IL_0154;
		}

		public static bool IsHexDigit(char digit)
		{
			return ('0' <= digit && digit <= '9') || ('a' <= digit && digit <= 'f') || ('A' <= digit && digit <= 'F');
		}

		public static bool IsHexEncoding(string pattern, int index)
		{
			return index + 3 <= pattern.Length && (pattern[index++] == '%' && Uri.IsHexDigit(pattern[index++])) && Uri.IsHexDigit(pattern[index]);
		}

		public string MakeRelative(Uri toUri)
		{
			if (this.Scheme != toUri.Scheme || this.Authority != toUri.Authority)
			{
				return toUri.ToString();
			}
			if (this.path == toUri.path)
			{
				return string.Empty;
			}
			string[] array = this.Segments;
			string[] array2 = toUri.Segments;
			int num = 0;
			int num2 = Math.Min(array.Length, array2.Length);
			while (num < num2 && !(array[num] != array2[num]))
			{
				num++;
			}
			string text = string.Empty;
			for (int i = num + 1; i < array.Length; i++)
			{
				text += "../";
			}
			for (int j = num; j < array2.Length; j++)
			{
				text += array2[j];
			}
			return text;
		}

		public override string ToString()
		{
			if (this.cachedToString != null)
			{
				return this.cachedToString;
			}
			string text = (this.query.StartsWith("?") ? ("?" + this.Unescape(this.query.Substring(1))) : this.Unescape(this.query));
			this.cachedToString = this.Unescape(this.GetLeftPart(UriPartial.Path), true) + text + this.fragment;
			return this.cachedToString;
		}

		protected void Escape()
		{
			this.path = Uri.EscapeString(this.path);
		}

		protected static string EscapeString(string str)
		{
			return Uri.EscapeString(str, false, true, true);
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
				if (Uri.IsHexEncoding(str, i))
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
							stringBuilder.Append(Uri.HexEscape(c));
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

		protected void Parse()
		{
			this.Parse(this.source);
			if (this.userEscaped)
			{
				return;
			}
			this.host = Uri.EscapeString(this.host, false, true, false);
			this.path = Uri.EscapeString(this.path);
		}

		protected string Unescape(string str)
		{
			return this.Unescape(str, false);
		}

		internal string Unescape(string str, bool excludeSharp)
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
					char c2 = Uri.HexUnescape(str, ref i);
					if (excludeSharp && c2 == '#')
					{
						stringBuilder.Append("%23");
					}
					else
					{
						stringBuilder.Append(c2);
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
			this.scheme = Uri.UriSchemeFile;
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

		private void ParseAsWindowsAbsoluteFilePath(string uriString)
		{
			if (uriString.Length > 2 && uriString[2] != '\\' && uriString[2] != '/')
			{
				throw new FormatException("Relative file path is not allowed.");
			}
			this.scheme = Uri.UriSchemeFile;
			this.host = string.Empty;
			this.port = -1;
			this.path = uriString.Replace("\\", "/");
			this.fragment = string.Empty;
			this.query = string.Empty;
		}

		private void ParseAsUnixAbsoluteFilePath(string uriString)
		{
			this.isUnixFilePath = true;
			this.scheme = Uri.UriSchemeFile;
			this.port = -1;
			this.fragment = string.Empty;
			this.query = string.Empty;
			this.host = string.Empty;
			this.path = null;
			if (uriString.StartsWith("//"))
			{
				uriString = uriString.TrimStart(new char[] { '/' });
				this.path = "/" + uriString;
			}
			if (this.path == null)
			{
				this.path = uriString;
			}
		}

		private void Parse(string uriString)
		{
			if (uriString == null)
			{
				throw new ArgumentNullException("uriString");
			}
			if (uriString.Length <= 1)
			{
				throw new FormatException();
			}
			int num = uriString.IndexOf(':');
			if (num < 0)
			{
				if (uriString[0] == '/')
				{
					this.ParseAsUnixAbsoluteFilePath(uriString);
					return;
				}
				if (uriString.StartsWith("\\\\"))
				{
					this.ParseAsWindowsUNC(uriString);
					return;
				}
				throw new FormatException("URI scheme was not recognized, nor input string is not recognized as an absolute file path.");
			}
			else if (num == 1)
			{
				if (!char.IsLetter(uriString[0]))
				{
					throw new FormatException("URI scheme must start with alphabet character.");
				}
				this.ParseAsWindowsAbsoluteFilePath(uriString);
				return;
			}
			else
			{
				this.scheme = uriString.Substring(0, num).ToLower(CultureInfo.InvariantCulture);
				if (!char.IsLetter(this.scheme[0]))
				{
					throw new FormatException("URI scheme must start with alphabet character.");
				}
				for (int i = 1; i < this.scheme.Length; i++)
				{
					if (!char.IsLetterOrDigit(this.scheme, i))
					{
						switch (this.scheme[i])
						{
						case '+':
						case '-':
						case '.':
							break;
						default:
							throw new FormatException("URI scheme must consist of one of alphabet, digits, '+', '-' or '.' character.");
						}
					}
				}
				uriString = uriString.Substring(num + 1);
				num = uriString.IndexOf('#');
				if (!this.IsUnc && num != -1)
				{
					this.fragment = uriString.Substring(num);
					uriString = uriString.Substring(0, num);
				}
				num = uriString.IndexOf('?');
				if (num != -1)
				{
					this.query = uriString.Substring(num);
					uriString = uriString.Substring(0, num);
					if (!this.userEscaped)
					{
						this.query = Uri.EscapeString(this.query);
					}
				}
				bool flag = this.scheme == Uri.UriSchemeFile && uriString.StartsWith("///");
				if (uriString.StartsWith("//"))
				{
					if (uriString.StartsWith("////"))
					{
						flag = false;
					}
					uriString = uriString.TrimStart(new char[] { '/' });
					if (uriString.Length > 1 && uriString[1] == ':')
					{
						flag = false;
					}
				}
				else if (!Uri.IsPredefinedScheme(this.scheme))
				{
					this.path = uriString;
					this.isOpaquePart = true;
					return;
				}
				num = uriString.IndexOfAny(new char[] { '/', '\\' });
				if (flag)
				{
					num = -1;
				}
				if (num == -1)
				{
					if (this.scheme != Uri.UriSchemeMailto && this.scheme != Uri.UriSchemeNews && this.scheme != Uri.UriSchemeFile)
					{
						this.path = "/";
					}
				}
				else
				{
					this.path = uriString.Substring(num);
					uriString = uriString.Substring(0, num);
				}
				num = uriString.IndexOf("@");
				if (flag)
				{
					num = -1;
				}
				if (num != -1)
				{
					this.userinfo = uriString.Substring(0, num);
					uriString = uriString.Remove(0, num + 1);
				}
				this.port = -1;
				num = uriString.LastIndexOf(":");
				if (flag)
				{
					num = -1;
				}
				if (num == 1 && this.scheme == Uri.UriSchemeFile && char.IsLetter(uriString[0]))
				{
					num = -1;
				}
				if (num != -1 && num != uriString.Length - 1)
				{
					string text = uriString.Remove(0, num + 1);
					if (text.Length > 1 && text[text.Length - 1] != ']')
					{
						try
						{
							this.port = (int)uint.Parse(text, CultureInfo.InvariantCulture);
							uriString = uriString.Substring(0, num);
						}
						catch (Exception)
						{
							throw new FormatException("Invalid URI: invalid port number");
						}
					}
				}
				if (this.port == -1)
				{
					this.port = Uri.GetDefaultPort(this.scheme);
				}
				this.host = uriString;
				if (flag)
				{
					this.path = "/" + uriString;
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
				else
				{
					if (this.host.Length == 0)
					{
						throw new FormatException("Invalid URI: The hostname could not be parsed");
					}
					if (this.scheme == Uri.UriSchemeFile)
					{
						this.isUnc = true;
					}
				}
				if (this.scheme != Uri.UriSchemeMailto && this.scheme != Uri.UriSchemeNews && this.scheme != Uri.UriSchemeFile && this.reduce)
				{
					this.path = Uri.Reduce(this.path);
				}
				return;
			}
		}

		private static string Reduce(string path)
		{
			path = path.Replace('\\', '/');
			string[] array = path.Split(new char[] { '/' });
			List<string> list = new List<string>();
			int num = array.Length;
			for (int i = 0; i < num; i++)
			{
				string text = array[i];
				if (text.Length != 0 && !(text == "."))
				{
					if (text == "..")
					{
						if (list.Count == 0)
						{
							if (i != 1)
							{
								throw new Exception("Invalid path.");
							}
						}
						else
						{
							list.RemoveAt(list.Count - 1);
						}
					}
					else
					{
						list.Add(text);
					}
				}
			}
			if (list.Count == 0)
			{
				return "/";
			}
			list.Insert(0, string.Empty);
			string text2 = string.Join("/", list.ToArray());
			if (path.EndsWith("/"))
			{
				text2 += "/";
			}
			return text2;
		}

		internal static string GetSchemeDelimiter(string scheme)
		{
			for (int i = 0; i < Uri.schemes.Length; i++)
			{
				if (Uri.schemes[i].scheme == scheme)
				{
					return Uri.schemes[i].delimiter;
				}
			}
			return Uri.SchemeDelimiter;
		}

		internal static int GetDefaultPort(string scheme)
		{
			for (int i = 0; i < Uri.schemes.Length; i++)
			{
				if (Uri.schemes[i].scheme == scheme)
				{
					return Uri.schemes[i].defaultPort;
				}
			}
			return -1;
		}

		private string GetOpaqueWiseSchemeDelimiter()
		{
			if (this.isOpaquePart)
			{
				return ":";
			}
			return Uri.GetSchemeDelimiter(this.scheme);
		}

		protected bool IsBadFileSystemCharacter(char ch)
		{
			if (ch < ' ' || (ch < '@' && ch > '9'))
			{
				return true;
			}
			if (ch <= '*')
			{
				if (ch <= '"')
				{
					if (ch != '\0' && ch != '"')
					{
						return false;
					}
				}
				else if (ch != '&' && ch != '*')
				{
					return false;
				}
			}
			else if (ch <= '/')
			{
				if (ch != ',' && ch != '/')
				{
					return false;
				}
			}
			else if (ch != '\\' && ch != '^' && ch != '|')
			{
				return false;
			}
			return true;
		}

		protected static bool IsExcludedCharacter(char ch)
		{
			return ch <= ' ' || ch >= '\u007f' || (ch == '"' || ch == '#' || ch == '%' || ch == '<' || ch == '>' || ch == '[' || ch == '\\' || ch == ']' || ch == '^' || ch == '`' || ch == '{' || ch == '|' || ch == '}');
		}

		private static bool IsPredefinedScheme(string scheme)
		{
			uint num = <PrivateImplementationDetails>.ComputeStringHash(scheme);
			if (num <= 2867484483U)
			{
				if (num <= 1271381062U)
				{
					if (num != 227981521U)
					{
						if (num != 1271381062U)
						{
							return false;
						}
						if (!(scheme == "news"))
						{
							return false;
						}
					}
					else if (!(scheme == "nntp"))
					{
						return false;
					}
				}
				else if (num != 1315902419U)
				{
					if (num != 2867484483U)
					{
						return false;
					}
					if (!(scheme == "file"))
					{
						return false;
					}
				}
				else if (!(scheme == "mailto"))
				{
					return false;
				}
			}
			else if (num <= 3378792613U)
			{
				if (num != 3101544485U)
				{
					if (num != 3378792613U)
					{
						return false;
					}
					if (!(scheme == "http"))
					{
						return false;
					}
				}
				else if (!(scheme == "ftp"))
				{
					return false;
				}
			}
			else if (num != 3500961320U)
			{
				if (num != 3739134178U)
				{
					return false;
				}
				if (!(scheme == "https"))
				{
					return false;
				}
			}
			else if (!(scheme == "gopher"))
			{
				return false;
			}
			return true;
		}

		protected bool IsReservedCharacter(char ch)
		{
			return ch == '$' || ch == '&' || ch == '+' || ch == ',' || ch == '/' || ch == ':' || ch == ';' || ch == '=' || ch == '@';
		}

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

		private string[] segments;

		private bool userEscaped;

		private string cachedAbsoluteUri;

		private string cachedToString;

		private string cachedLocalPath;

		private int cachedHashCode;

		private bool reduce;

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

		private static Uri.UriScheme[] schemes = new Uri.UriScheme[]
		{
			new Uri.UriScheme(Uri.UriSchemeHttp, Uri.SchemeDelimiter, 80),
			new Uri.UriScheme(Uri.UriSchemeHttps, Uri.SchemeDelimiter, 443),
			new Uri.UriScheme(Uri.UriSchemeFtp, Uri.SchemeDelimiter, 21),
			new Uri.UriScheme(Uri.UriSchemeFile, Uri.SchemeDelimiter, -1),
			new Uri.UriScheme(Uri.UriSchemeMailto, ":", 25),
			new Uri.UriScheme(Uri.UriSchemeNews, ":", -1),
			new Uri.UriScheme(Uri.UriSchemeNntp, Uri.SchemeDelimiter, 119),
			new Uri.UriScheme(Uri.UriSchemeGopher, Uri.SchemeDelimiter, 70)
		};

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
