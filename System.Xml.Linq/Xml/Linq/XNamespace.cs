using System;
using System.Collections.Generic;

namespace System.Xml.Linq
{
	public sealed class XNamespace
	{
		private XNamespace(string namespaceName)
		{
			if (namespaceName == null)
			{
				throw new ArgumentNullException("namespaceName");
			}
			this.uri = namespaceName;
		}

		public static XNamespace None
		{
			get
			{
				return XNamespace.blank;
			}
		}

		public static XNamespace Xml
		{
			get
			{
				return XNamespace.xml;
			}
		}

		public static XNamespace Xmlns
		{
			get
			{
				return XNamespace.xmlns;
			}
		}

		public static XNamespace Get(string uri)
		{
			Dictionary<string, XNamespace> dictionary = XNamespace.nstable;
			XNamespace xnamespace2;
			lock (dictionary)
			{
				XNamespace xnamespace;
				if (!XNamespace.nstable.TryGetValue(uri, out xnamespace))
				{
					xnamespace = new XNamespace(uri);
					XNamespace.nstable[uri] = xnamespace;
				}
				xnamespace2 = xnamespace;
			}
			return xnamespace2;
		}

		public XName GetName(string localName)
		{
			if (this.table == null)
			{
				this.table = new Dictionary<string, XName>();
			}
			Dictionary<string, XName> dictionary = this.table;
			XName xname2;
			lock (dictionary)
			{
				XName xname;
				if (!this.table.TryGetValue(localName, out xname))
				{
					xname = new XName(localName, this);
					this.table[localName] = xname;
				}
				xname2 = xname;
			}
			return xname2;
		}

		public string NamespaceName
		{
			get
			{
				return this.uri;
			}
		}

		public override bool Equals(object other)
		{
			if (object.ReferenceEquals(this, other))
			{
				return true;
			}
			XNamespace xnamespace = other as XNamespace;
			return xnamespace != null && this.uri == xnamespace.uri;
		}

		public override int GetHashCode()
		{
			return this.uri.GetHashCode();
		}

		public override string ToString()
		{
			return this.uri;
		}

		public static bool operator ==(XNamespace o1, XNamespace o2)
		{
			return (o1 == null) ? (o2 == null) : o1.Equals(o2);
		}

		public static bool operator !=(XNamespace o1, XNamespace o2)
		{
			return !(o1 == o2);
		}

		public static XName operator +(XNamespace ns, string localName)
		{
			return new XName(localName, ns.NamespaceName);
		}

		public static implicit operator XNamespace(string s)
		{
			return (s == null) ? null : XNamespace.Get(s);
		}

		private static readonly XNamespace blank = XNamespace.Get(string.Empty);

		private static readonly XNamespace xml = XNamespace.Get("http://www.w3.org/XML/1998/namespace");

		private static readonly XNamespace xmlns = XNamespace.Get("http://www.w3.org/2000/xmlns/");

		private static Dictionary<string, XNamespace> nstable = new Dictionary<string, XNamespace>();

		private string uri;

		private Dictionary<string, XName> table;
	}
}
