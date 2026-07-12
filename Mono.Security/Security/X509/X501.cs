using System;
using System.Globalization;
using System.Text;
using Mono.Security.Cryptography;

namespace Mono.Security.X509
{
	public sealed class X501
	{
		private X501()
		{
		}

		public static string ToString(ASN1 seq)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < seq.Count; i++)
			{
				ASN1 asn = seq[i];
				X501.AppendEntry(stringBuilder, asn, true);
				if (i < seq.Count - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			return stringBuilder.ToString();
		}

		public static string ToString(ASN1 seq, bool reversed, string separator, bool quotes)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (reversed)
			{
				for (int i = seq.Count - 1; i >= 0; i--)
				{
					ASN1 asn = seq[i];
					X501.AppendEntry(stringBuilder, asn, quotes);
					if (i > 0)
					{
						stringBuilder.Append(separator);
					}
				}
			}
			else
			{
				for (int j = 0; j < seq.Count; j++)
				{
					ASN1 asn2 = seq[j];
					X501.AppendEntry(stringBuilder, asn2, quotes);
					if (j < seq.Count - 1)
					{
						stringBuilder.Append(separator);
					}
				}
			}
			return stringBuilder.ToString();
		}

		private static void AppendEntry(StringBuilder sb, ASN1 entry, bool quotes)
		{
			for (int i = 0; i < entry.Count; i++)
			{
				ASN1 asn = entry[i];
				ASN1 asn2 = asn[1];
				if (asn2 != null)
				{
					ASN1 asn3 = asn[0];
					if (asn3 != null)
					{
						if (asn3.CompareValue(X501.countryName))
						{
							sb.Append("C=");
						}
						else if (asn3.CompareValue(X501.organizationName))
						{
							sb.Append("O=");
						}
						else if (asn3.CompareValue(X501.organizationalUnitName))
						{
							sb.Append("OU=");
						}
						else if (asn3.CompareValue(X501.commonName))
						{
							sb.Append("CN=");
						}
						else if (asn3.CompareValue(X501.localityName))
						{
							sb.Append("L=");
						}
						else if (asn3.CompareValue(X501.stateOrProvinceName))
						{
							sb.Append("S=");
						}
						else if (asn3.CompareValue(X501.streetAddress))
						{
							sb.Append("STREET=");
						}
						else if (asn3.CompareValue(X501.domainComponent))
						{
							sb.Append("DC=");
						}
						else if (asn3.CompareValue(X501.userid))
						{
							sb.Append("UID=");
						}
						else if (asn3.CompareValue(X501.email))
						{
							sb.Append("E=");
						}
						else if (asn3.CompareValue(X501.dnQualifier))
						{
							sb.Append("dnQualifier=");
						}
						else if (asn3.CompareValue(X501.title))
						{
							sb.Append("T=");
						}
						else if (asn3.CompareValue(X501.surname))
						{
							sb.Append("SN=");
						}
						else if (asn3.CompareValue(X501.givenName))
						{
							sb.Append("G=");
						}
						else if (asn3.CompareValue(X501.initial))
						{
							sb.Append("I=");
						}
						else if (asn3.CompareValue(X501.serialNumber))
						{
							sb.Append("SERIALNUMBER=");
						}
						else
						{
							sb.Append("OID.");
							sb.Append(ASN1Convert.ToOid(asn3));
							sb.Append("=");
						}
						string text;
						if (asn2.Tag == 30)
						{
							StringBuilder stringBuilder = new StringBuilder();
							for (int j = 1; j < asn2.Value.Length; j += 2)
							{
								stringBuilder.Append((char)asn2.Value[j]);
							}
							text = stringBuilder.ToString();
						}
						else if (asn2.Tag == 20)
						{
							text = Encoding.UTF7.GetString(asn2.Value);
						}
						else
						{
							text = Encoding.UTF8.GetString(asn2.Value);
						}
						char[] array = new char[] { ',', '+', '"', '=', '<', '>', ';', '#', '\n' };
						if (quotes && (text.IndexOfAny(array, 0, text.Length) > 0 || text.StartsWith(" ") || text.EndsWith(" ")))
						{
							text = "\"" + text.Replace("\"", "") + "\"";
						}
						sb.Append(text);
						if (i < entry.Count - 1)
						{
							sb.Append(", ");
						}
					}
				}
			}
		}

		private static X520.AttributeTypeAndValue GetAttributeFromOid(string attributeType)
		{
			string text = attributeType.ToUpper(CultureInfo.InvariantCulture).Trim();
			uint num = global::<PrivateImplementationDetails>.ComputeStringHash(text);
			if (num <= 3322673650U)
			{
				if (num <= 2078582897U)
				{
					if (num <= 1627558660U)
					{
						if (num != 902722544U)
						{
							if (num != 1627558660U)
							{
								goto IL_02BD;
							}
							if (!(text == "SN"))
							{
								goto IL_02BD;
							}
							return new X520.Surname();
						}
						else
						{
							if (!(text == "DC"))
							{
								goto IL_02BD;
							}
							return new X520.DomainComponent();
						}
					}
					else if (num != 1795334850U)
					{
						if (num != 2078582897U)
						{
							goto IL_02BD;
						}
						if (!(text == "OU"))
						{
							goto IL_02BD;
						}
						return new X520.OrganizationalUnitName();
					}
					else if (!(text == "ST"))
					{
						goto IL_02BD;
					}
				}
				else if (num <= 3222007936U)
				{
					if (num != 2161779444U)
					{
						if (num != 3222007936U)
						{
							goto IL_02BD;
						}
						if (!(text == "E"))
						{
							goto IL_02BD;
						}
						return new X520.EmailAddress();
					}
					else
					{
						if (!(text == "CN"))
						{
							goto IL_02BD;
						}
						return new X520.CommonName();
					}
				}
				else if (num != 3255563174U)
				{
					if (num != 3322673650U)
					{
						goto IL_02BD;
					}
					if (!(text == "C"))
					{
						goto IL_02BD;
					}
					return new X520.CountryName();
				}
				else
				{
					if (!(text == "G"))
					{
						goto IL_02BD;
					}
					return new X520.GivenName();
				}
			}
			else if (num <= 3423339364U)
			{
				if (num <= 3373006507U)
				{
					if (num != 3369459556U)
					{
						if (num != 3373006507U)
						{
							goto IL_02BD;
						}
						if (!(text == "L"))
						{
							goto IL_02BD;
						}
						return new X520.LocalityName();
					}
					else
					{
						if (!(text == "SERIALNUMBER"))
						{
							goto IL_02BD;
						}
						return new X520.SerialNumber();
					}
				}
				else if (num != 3389784126U)
				{
					if (num != 3423339364U)
					{
						goto IL_02BD;
					}
					if (!(text == "I"))
					{
						goto IL_02BD;
					}
					return new X520.Initial();
				}
				else
				{
					if (!(text == "O"))
					{
						goto IL_02BD;
					}
					return new X520.OrganizationName();
				}
			}
			else if (num <= 3591115554U)
			{
				if (num != 3507227459U)
				{
					if (num != 3591115554U)
					{
						goto IL_02BD;
					}
					if (!(text == "S"))
					{
						goto IL_02BD;
					}
				}
				else
				{
					if (!(text == "T"))
					{
						goto IL_02BD;
					}
					return new X520.Title();
				}
			}
			else if (num != 3751961261U)
			{
				if (num != 4293667421U)
				{
					goto IL_02BD;
				}
				if (!(text == "DNQUALIFIER"))
				{
					goto IL_02BD;
				}
				return new X520.DnQualifier();
			}
			else
			{
				if (!(text == "UID"))
				{
					goto IL_02BD;
				}
				return new X520.UserId();
			}
			return new X520.StateOrProvinceName();
			IL_02BD:
			if (text.StartsWith("OID."))
			{
				return new X520.Oid(text.Substring(4));
			}
			if (X501.IsOid(text))
			{
				return new X520.Oid(text);
			}
			return null;
		}

		private static bool IsOid(string oid)
		{
			bool flag;
			try
			{
				flag = ASN1Convert.FromOid(oid).Tag == 6;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		private static X520.AttributeTypeAndValue ReadAttribute(string value, ref int pos)
		{
			while (value[pos] == ' ' && pos < value.Length)
			{
				pos++;
			}
			int num = value.IndexOf('=', pos);
			if (num == -1)
			{
				throw new FormatException(Locale.GetText("No attribute found."));
			}
			string text = value.Substring(pos, num - pos);
			X520.AttributeTypeAndValue attributeFromOid = X501.GetAttributeFromOid(text);
			if (attributeFromOid == null)
			{
				throw new FormatException(string.Format(Locale.GetText("Unknown attribute '{0}'."), text));
			}
			pos = num + 1;
			return attributeFromOid;
		}

		private static bool IsHex(char c)
		{
			if (char.IsDigit(c))
			{
				return true;
			}
			char c2 = char.ToUpper(c, CultureInfo.InvariantCulture);
			return c2 >= 'A' && c2 <= 'F';
		}

		private static string ReadHex(string value, ref int pos)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = stringBuilder;
			int num = pos;
			pos = num + 1;
			stringBuilder2.Append(value[num]);
			stringBuilder.Append(value[pos]);
			if (pos < value.Length - 4 && value[pos + 1] == '\\' && X501.IsHex(value[pos + 2]))
			{
				pos += 2;
				StringBuilder stringBuilder3 = stringBuilder;
				num = pos;
				pos = num + 1;
				stringBuilder3.Append(value[num]);
				stringBuilder.Append(value[pos]);
			}
			byte[] array = CryptoConvert.FromHex(stringBuilder.ToString());
			return Encoding.UTF8.GetString(array);
		}

		private static int ReadEscaped(StringBuilder sb, string value, int pos)
		{
			char c = value[pos];
			if (c <= '+')
			{
				if (c != '"' && c != '#' && c != '+')
				{
					goto IL_0051;
				}
			}
			else if (c != ',')
			{
				switch (c)
				{
				case ';':
				case '<':
				case '=':
				case '>':
					break;
				default:
					if (c != '\\')
					{
						goto IL_0051;
					}
					break;
				}
			}
			sb.Append(value[pos]);
			return pos;
			IL_0051:
			if (pos >= value.Length - 2)
			{
				throw new FormatException(string.Format(Locale.GetText("Malformed escaped value '{0}'."), value.Substring(pos)));
			}
			sb.Append(X501.ReadHex(value, ref pos));
			return pos;
		}

		private static int ReadQuoted(StringBuilder sb, string value, int pos)
		{
			int num = pos;
			while (pos <= value.Length)
			{
				char c = value[pos];
				if (c == '"')
				{
					return pos;
				}
				if (c == '\\')
				{
					return X501.ReadEscaped(sb, value, pos);
				}
				sb.Append(value[pos]);
				pos++;
			}
			throw new FormatException(string.Format(Locale.GetText("Malformed quoted value '{0}'."), value.Substring(num)));
		}

		private static string ReadValue(string value, ref int pos)
		{
			int num = pos;
			StringBuilder stringBuilder = new StringBuilder();
			while (pos < value.Length)
			{
				char c = value[pos];
				if (c <= '+')
				{
					if (c != '"')
					{
						if (c != '#' && c != '+')
						{
							goto IL_00B7;
						}
						throw new NotImplementedException();
					}
					else
					{
						StringBuilder stringBuilder2 = stringBuilder;
						int num2 = pos + 1;
						pos = num2;
						pos = X501.ReadQuoted(stringBuilder2, value, num2);
					}
				}
				else
				{
					if (c == ',')
					{
						pos++;
						return stringBuilder.ToString();
					}
					switch (c)
					{
					case ';':
					case '<':
					case '=':
					case '>':
						throw new FormatException(string.Format(Locale.GetText("Malformed value '{0}' contains '{1}' outside quotes."), value.Substring(num), value[pos]));
					default:
					{
						if (c != '\\')
						{
							goto IL_00B7;
						}
						StringBuilder stringBuilder3 = stringBuilder;
						int num2 = pos + 1;
						pos = num2;
						pos = X501.ReadEscaped(stringBuilder3, value, num2);
						break;
					}
					}
				}
				IL_00C6:
				pos++;
				continue;
				IL_00B7:
				stringBuilder.Append(value[pos]);
				goto IL_00C6;
			}
			return stringBuilder.ToString();
		}

		public static ASN1 FromString(string rdn)
		{
			if (rdn == null)
			{
				throw new ArgumentNullException("rdn");
			}
			int i = 0;
			ASN1 asn = new ASN1(48);
			while (i < rdn.Length)
			{
				X520.AttributeTypeAndValue attributeTypeAndValue = X501.ReadAttribute(rdn, ref i);
				attributeTypeAndValue.Value = X501.ReadValue(rdn, ref i);
				ASN1 asn2 = new ASN1(49);
				asn2.Add(attributeTypeAndValue.GetASN1());
				asn.Add(asn2);
			}
			return asn;
		}

		private static byte[] countryName = new byte[] { 85, 4, 6 };

		private static byte[] organizationName = new byte[] { 85, 4, 10 };

		private static byte[] organizationalUnitName = new byte[] { 85, 4, 11 };

		private static byte[] commonName = new byte[] { 85, 4, 3 };

		private static byte[] localityName = new byte[] { 85, 4, 7 };

		private static byte[] stateOrProvinceName = new byte[] { 85, 4, 8 };

		private static byte[] streetAddress = new byte[] { 85, 4, 9 };

		private static byte[] serialNumber = new byte[] { 85, 4, 5 };

		private static byte[] domainComponent = new byte[] { 9, 146, 38, 137, 147, 242, 44, 100, 1, 25 };

		private static byte[] userid = new byte[] { 9, 146, 38, 137, 147, 242, 44, 100, 1, 1 };

		private static byte[] email = new byte[] { 42, 134, 72, 134, 247, 13, 1, 9, 1 };

		private static byte[] dnQualifier = new byte[] { 85, 4, 46 };

		private static byte[] title = new byte[] { 85, 4, 12 };

		private static byte[] surname = new byte[] { 85, 4, 4 };

		private static byte[] givenName = new byte[] { 85, 4, 42 };

		private static byte[] initial = new byte[] { 85, 4, 43 };
	}
}
