using System;

namespace System.Xml.Xsl
{
	internal class XmlQualifiedNameTest : XmlQualifiedName
	{
		public static XmlQualifiedNameTest Wildcard
		{
			get
			{
				return XmlQualifiedNameTest.wc;
			}
		}

		private XmlQualifiedNameTest(string name, string ns, bool exclude)
			: base(name, ns)
		{
			this.exclude = exclude;
		}

		public static XmlQualifiedNameTest New(string name, string ns)
		{
			if (ns == null && name == null)
			{
				return XmlQualifiedNameTest.Wildcard;
			}
			return new XmlQualifiedNameTest((name == null) ? "*" : name, (ns == null) ? "*" : ns, false);
		}

		public bool IsWildcard
		{
			get
			{
				return this == XmlQualifiedNameTest.Wildcard;
			}
		}

		public bool IsNameWildcard
		{
			get
			{
				return base.Name == "*";
			}
		}

		public bool IsNamespaceWildcard
		{
			get
			{
				return base.Namespace == "*";
			}
		}

		private bool IsNameSubsetOf(XmlQualifiedNameTest other)
		{
			return other.IsNameWildcard || base.Name == other.Name;
		}

		private bool IsNamespaceSubsetOf(XmlQualifiedNameTest other)
		{
			return other.IsNamespaceWildcard || (this.exclude == other.exclude && base.Namespace == other.Namespace) || (other.exclude && !this.exclude && base.Namespace != other.Namespace);
		}

		public bool IsSubsetOf(XmlQualifiedNameTest other)
		{
			return this.IsNameSubsetOf(other) && this.IsNamespaceSubsetOf(other);
		}

		public bool HasIntersection(XmlQualifiedNameTest other)
		{
			return (this.IsNamespaceSubsetOf(other) || other.IsNamespaceSubsetOf(this)) && (this.IsNameSubsetOf(other) || other.IsNameSubsetOf(this));
		}

		public override string ToString()
		{
			if (this == XmlQualifiedNameTest.Wildcard)
			{
				return "*";
			}
			if (base.Namespace.Length == 0)
			{
				return base.Name;
			}
			if (base.Namespace == "*")
			{
				return "*:" + base.Name;
			}
			if (this.exclude)
			{
				return "{~" + base.Namespace + "}:" + base.Name;
			}
			return "{" + base.Namespace + "}:" + base.Name;
		}

		private bool exclude;

		private const string wildcard = "*";

		private static XmlQualifiedNameTest wc = XmlQualifiedNameTest.New("*", "*");
	}
}
