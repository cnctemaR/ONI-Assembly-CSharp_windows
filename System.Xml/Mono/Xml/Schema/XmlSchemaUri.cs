using System;

namespace Mono.Xml.Schema
{
	internal class XmlSchemaUri : Uri
	{
		public XmlSchemaUri(string src)
			: this(src, XmlSchemaUri.HasValidScheme(src))
		{
		}

		private XmlSchemaUri(string src, bool formal)
			: base((!formal) ? ("anyuri:" + src) : src, !formal)
		{
			this.value = src;
		}

		private static bool HasValidScheme(string src)
		{
			int num = src.IndexOf(':');
			if (num < 0)
			{
				return false;
			}
			int i = 0;
			while (i < num)
			{
				switch (src[i])
				{
				case '+':
				case '-':
				case '.':
					break;
				case ',':
					goto IL_0044;
				default:
					goto IL_0044;
				}
				IL_005C:
				i++;
				continue;
				IL_0044:
				if (char.IsLetterOrDigit(src[i]))
				{
					goto IL_005C;
				}
				return false;
			}
			return true;
		}

		public override bool Equals(object obj)
		{
			return obj is XmlSchemaUri && (XmlSchemaUri)obj == this;
		}

		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		public override string ToString()
		{
			return this.value;
		}

		public static bool operator ==(XmlSchemaUri v1, XmlSchemaUri v2)
		{
			return v1.value == v2.value;
		}

		public static bool operator !=(XmlSchemaUri v1, XmlSchemaUri v2)
		{
			return v1.value != v2.value;
		}

		public string value;
	}
}
