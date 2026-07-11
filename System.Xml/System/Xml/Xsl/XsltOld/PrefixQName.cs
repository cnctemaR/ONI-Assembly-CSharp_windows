using System;

namespace System.Xml.Xsl.XsltOld
{
	internal sealed class PrefixQName
	{
		internal void ClearPrefix()
		{
			this.Prefix = string.Empty;
		}

		internal void SetQName(string qname)
		{
			PrefixQName.ParseQualifiedName(qname, out this.Prefix, out this.Name);
		}

		public static void ParseQualifiedName(string qname, out string prefix, out string local)
		{
			prefix = string.Empty;
			local = string.Empty;
			int num = ValidateNames.ParseNCName(qname);
			if (num == 0)
			{
				throw XsltException.Create("'{0}' is an invalid QName.", new string[] { qname });
			}
			local = qname.Substring(0, num);
			if (num < qname.Length)
			{
				if (qname[num] == ':')
				{
					int num2;
					num = (num2 = num + 1);
					prefix = local;
					int num3 = ValidateNames.ParseNCName(qname, num);
					num += num3;
					if (num3 == 0)
					{
						throw XsltException.Create("'{0}' is an invalid QName.", new string[] { qname });
					}
					local = qname.Substring(num2, num3);
				}
				if (num < qname.Length)
				{
					throw XsltException.Create("'{0}' is an invalid QName.", new string[] { qname });
				}
			}
		}

		public static bool ValidatePrefix(string prefix)
		{
			return prefix.Length != 0 && ValidateNames.ParseNCName(prefix, 0) == prefix.Length;
		}

		public string Prefix;

		public string Name;

		public string Namespace;
	}
}
