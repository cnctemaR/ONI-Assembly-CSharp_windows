using System;
using System.Xml.XPath;

namespace System.Xml
{
	internal static class ValidateNames
	{
		internal static int ParseNmtoken(string s, int offset)
		{
			int num = offset;
			while (num < s.Length && (ValidateNames.xmlCharType.charProperties[(int)s[num]] & 8) != 0)
			{
				num++;
			}
			return num - offset;
		}

		internal static int ParseNmtokenNoNamespaces(string s, int offset)
		{
			int num = offset;
			while (num < s.Length && ((ValidateNames.xmlCharType.charProperties[(int)s[num]] & 8) != 0 || s[num] == ':'))
			{
				num++;
			}
			return num - offset;
		}

		internal static bool IsNmtokenNoNamespaces(string s)
		{
			int num = ValidateNames.ParseNmtokenNoNamespaces(s, 0);
			return num > 0 && num == s.Length;
		}

		internal static int ParseNameNoNamespaces(string s, int offset)
		{
			int num = offset;
			if (num < s.Length)
			{
				if ((ValidateNames.xmlCharType.charProperties[(int)s[num]] & 4) == 0 && s[num] != ':')
				{
					return 0;
				}
				num++;
				while (num < s.Length && ((ValidateNames.xmlCharType.charProperties[(int)s[num]] & 8) != 0 || s[num] == ':'))
				{
					num++;
				}
			}
			return num - offset;
		}

		internal static bool IsNameNoNamespaces(string s)
		{
			int num = ValidateNames.ParseNameNoNamespaces(s, 0);
			return num > 0 && num == s.Length;
		}

		internal static int ParseNCName(string s, int offset)
		{
			int num = offset;
			if (num < s.Length)
			{
				if ((ValidateNames.xmlCharType.charProperties[(int)s[num]] & 4) == 0)
				{
					return 0;
				}
				num++;
				while (num < s.Length && (ValidateNames.xmlCharType.charProperties[(int)s[num]] & 8) != 0)
				{
					num++;
				}
			}
			return num - offset;
		}

		internal static int ParseNCName(string s)
		{
			return ValidateNames.ParseNCName(s, 0);
		}

		internal static string ParseNCNameThrow(string s)
		{
			ValidateNames.ParseNCNameInternal(s, true);
			return s;
		}

		private static bool ParseNCNameInternal(string s, bool throwOnError)
		{
			int num = ValidateNames.ParseNCName(s, 0);
			if (num == 0 || num != s.Length)
			{
				if (throwOnError)
				{
					ValidateNames.ThrowInvalidName(s, 0, num);
				}
				return false;
			}
			return true;
		}

		internal static int ParseQName(string s, int offset, out int colonOffset)
		{
			colonOffset = 0;
			int num = ValidateNames.ParseNCName(s, offset);
			if (num != 0)
			{
				offset += num;
				if (offset < s.Length && s[offset] == ':')
				{
					int num2 = ValidateNames.ParseNCName(s, offset + 1);
					if (num2 != 0)
					{
						colonOffset = offset;
						num += num2 + 1;
					}
				}
			}
			return num;
		}

		internal static void ParseQNameThrow(string s, out string prefix, out string localName)
		{
			int num2;
			int num = ValidateNames.ParseQName(s, 0, out num2);
			if (num == 0 || num != s.Length)
			{
				ValidateNames.ThrowInvalidName(s, 0, num);
			}
			if (num2 != 0)
			{
				prefix = s.Substring(0, num2);
				localName = s.Substring(num2 + 1);
				return;
			}
			prefix = "";
			localName = s;
		}

		internal static void ParseNameTestThrow(string s, out string prefix, out string localName)
		{
			int num;
			if (s.Length != 0 && s[0] == '*')
			{
				string text;
				localName = (text = null);
				prefix = text;
				num = 1;
			}
			else
			{
				num = ValidateNames.ParseNCName(s, 0);
				if (num != 0)
				{
					localName = s.Substring(0, num);
					if (num < s.Length && s[num] == ':')
					{
						prefix = localName;
						int num2 = num + 1;
						if (num2 < s.Length && s[num2] == '*')
						{
							localName = null;
							num += 2;
						}
						else
						{
							int num3 = ValidateNames.ParseNCName(s, num2);
							if (num3 != 0)
							{
								localName = s.Substring(num2, num3);
								num += num3 + 1;
							}
						}
					}
					else
					{
						prefix = string.Empty;
					}
				}
				else
				{
					string text;
					localName = (text = null);
					prefix = text;
				}
			}
			if (num == 0 || num != s.Length)
			{
				ValidateNames.ThrowInvalidName(s, 0, num);
			}
		}

		internal static void ThrowInvalidName(string s, int offsetStartChar, int offsetBadChar)
		{
			if (offsetStartChar >= s.Length)
			{
				throw new XmlException("The empty string '' is not a valid name.", string.Empty);
			}
			if (ValidateNames.xmlCharType.IsNCNameSingleChar(s[offsetBadChar]) && !XmlCharType.Instance.IsStartNCNameSingleChar(s[offsetBadChar]))
			{
				throw new XmlException("Name cannot begin with the '{0}' character, hexadecimal value {1}.", XmlException.BuildCharExceptionArgs(s, offsetBadChar));
			}
			throw new XmlException("The '{0}' character, hexadecimal value {1}, cannot be included in a name.", XmlException.BuildCharExceptionArgs(s, offsetBadChar));
		}

		internal static Exception GetInvalidNameException(string s, int offsetStartChar, int offsetBadChar)
		{
			if (offsetStartChar >= s.Length)
			{
				return new XmlException("The empty string '' is not a valid name.", string.Empty);
			}
			if (ValidateNames.xmlCharType.IsNCNameSingleChar(s[offsetBadChar]) && !ValidateNames.xmlCharType.IsStartNCNameSingleChar(s[offsetBadChar]))
			{
				return new XmlException("Name cannot begin with the '{0}' character, hexadecimal value {1}.", XmlException.BuildCharExceptionArgs(s, offsetBadChar));
			}
			return new XmlException("The '{0}' character, hexadecimal value {1}, cannot be included in a name.", XmlException.BuildCharExceptionArgs(s, offsetBadChar));
		}

		internal static bool StartsWithXml(string s)
		{
			return s.Length >= 3 && (s[0] == 'x' || s[0] == 'X') && (s[1] == 'm' || s[1] == 'M') && (s[2] == 'l' || s[2] == 'L');
		}

		internal static bool IsReservedNamespace(string s)
		{
			return s.Equals("http://www.w3.org/XML/1998/namespace") || s.Equals("http://www.w3.org/2000/xmlns/");
		}

		internal static void ValidateNameThrow(string prefix, string localName, string ns, XPathNodeType nodeKind, ValidateNames.Flags flags)
		{
			ValidateNames.ValidateNameInternal(prefix, localName, ns, nodeKind, flags, true);
		}

		internal static bool ValidateName(string prefix, string localName, string ns, XPathNodeType nodeKind, ValidateNames.Flags flags)
		{
			return ValidateNames.ValidateNameInternal(prefix, localName, ns, nodeKind, flags, false);
		}

		private static bool ValidateNameInternal(string prefix, string localName, string ns, XPathNodeType nodeKind, ValidateNames.Flags flags, bool throwOnError)
		{
			if ((flags & ValidateNames.Flags.NCNames) != (ValidateNames.Flags)0)
			{
				if (prefix.Length != 0 && !ValidateNames.ParseNCNameInternal(prefix, throwOnError))
				{
					return false;
				}
				if (localName.Length != 0 && !ValidateNames.ParseNCNameInternal(localName, throwOnError))
				{
					return false;
				}
			}
			if ((flags & ValidateNames.Flags.CheckLocalName) != (ValidateNames.Flags)0)
			{
				if (nodeKind != XPathNodeType.Element)
				{
					if (nodeKind != XPathNodeType.Attribute)
					{
						if (nodeKind != XPathNodeType.ProcessingInstruction)
						{
							if (localName.Length == 0)
							{
								goto IL_00FA;
							}
							if (throwOnError)
							{
								throw new XmlException("A node of type '{0}' cannot have a name.", nodeKind.ToString());
							}
							return false;
						}
						else
						{
							if (localName.Length != 0 && (localName.Length != 3 || !ValidateNames.StartsWithXml(localName)))
							{
								goto IL_00FA;
							}
							if (throwOnError)
							{
								throw new XmlException("'{0}' is an invalid name for processing instructions.", localName);
							}
							return false;
						}
					}
					else if (ns.Length == 0 && localName.Equals("xmlns"))
					{
						if (throwOnError)
						{
							throw new XmlException("A node of type '{0}' cannot have the name '{1}'.", new string[]
							{
								nodeKind.ToString(),
								localName
							});
						}
						return false;
					}
				}
				if (localName.Length == 0)
				{
					if (throwOnError)
					{
						throw new XmlException("The local name for elements or attributes cannot be null or an empty string.", string.Empty);
					}
					return false;
				}
			}
			IL_00FA:
			if ((flags & ValidateNames.Flags.CheckPrefixMapping) != (ValidateNames.Flags)0)
			{
				if (nodeKind - XPathNodeType.Element > 2)
				{
					if (nodeKind != XPathNodeType.ProcessingInstruction)
					{
						if (prefix.Length != 0 || ns.Length != 0)
						{
							if (throwOnError)
							{
								throw new XmlException("A node of type '{0}' cannot have a name.", nodeKind.ToString());
							}
							return false;
						}
					}
					else if (prefix.Length != 0 || ns.Length != 0)
					{
						if (throwOnError)
						{
							throw new XmlException("'{0}' is an invalid name for processing instructions.", ValidateNames.CreateName(prefix, localName));
						}
						return false;
					}
				}
				else if (ns.Length == 0)
				{
					if (prefix.Length != 0)
					{
						if (throwOnError)
						{
							throw new XmlException("Cannot use a prefix with an empty namespace.", string.Empty);
						}
						return false;
					}
				}
				else if (prefix.Length == 0 && nodeKind == XPathNodeType.Attribute)
				{
					if (throwOnError)
					{
						throw new XmlException("A node of type '{0}' cannot have the name '{1}'.", new string[]
						{
							nodeKind.ToString(),
							localName
						});
					}
					return false;
				}
				else if (prefix.Equals("xml"))
				{
					if (!ns.Equals("http://www.w3.org/XML/1998/namespace"))
					{
						if (throwOnError)
						{
							throw new XmlException("Prefix \"xml\" is reserved for use by XML and can be mapped only to namespace name \"http://www.w3.org/XML/1998/namespace\".", string.Empty);
						}
						return false;
					}
				}
				else if (prefix.Equals("xmlns"))
				{
					if (throwOnError)
					{
						throw new XmlException("Prefix \"xmlns\" is reserved for use by XML.", string.Empty);
					}
					return false;
				}
				else if (ValidateNames.IsReservedNamespace(ns))
				{
					if (throwOnError)
					{
						throw new XmlException("Prefix '{0}' cannot be mapped to namespace name reserved for \"xml\" or \"xmlns\".", string.Empty);
					}
					return false;
				}
			}
			return true;
		}

		private static string CreateName(string prefix, string localName)
		{
			if (prefix.Length == 0)
			{
				return localName;
			}
			return prefix + ":" + localName;
		}

		internal static void SplitQName(string name, out string prefix, out string lname)
		{
			int num = name.IndexOf(':');
			if (-1 == num)
			{
				prefix = string.Empty;
				lname = name;
				return;
			}
			if (num == 0 || name.Length - 1 == num)
			{
				throw new ArgumentException(Res.GetString("The '{0}' character, hexadecimal value {1}, cannot be included in a name.", XmlException.BuildCharExceptionArgs(':', '\0')), "name");
			}
			prefix = name.Substring(0, num);
			num++;
			lname = name.Substring(num, name.Length - num);
		}

		private static XmlCharType xmlCharType = XmlCharType.Instance;

		internal enum Flags
		{
			NCNames = 1,
			CheckLocalName,
			CheckPrefixMapping = 4,
			All = 7,
			AllExceptNCNames = 6,
			AllExceptPrefixMapping = 3
		}
	}
}
