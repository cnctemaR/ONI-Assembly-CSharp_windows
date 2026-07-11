using System;
using System.Threading;
using Unity;

namespace System.Xml.Linq
{
	public sealed class XNamespace
	{
		internal XNamespace(string namespaceName)
		{
			this.namespaceName = namespaceName;
			this.hashCode = namespaceName.GetHashCode();
			this.names = new XHashtable<XName>(new XHashtable<XName>.ExtractKeyDelegate(XNamespace.ExtractLocalName), 8);
		}

		public string NamespaceName
		{
			get
			{
				return this.namespaceName;
			}
		}

		public XName GetName(string localName)
		{
			if (localName == null)
			{
				throw new ArgumentNullException("localName");
			}
			return this.GetName(localName, 0, localName.Length);
		}

		public override string ToString()
		{
			return this.namespaceName;
		}

		public static XNamespace None
		{
			get
			{
				return XNamespace.EnsureNamespace(ref XNamespace.refNone, string.Empty);
			}
		}

		public static XNamespace Xml
		{
			get
			{
				return XNamespace.EnsureNamespace(ref XNamespace.refXml, "http://www.w3.org/XML/1998/namespace");
			}
		}

		public static XNamespace Xmlns
		{
			get
			{
				return XNamespace.EnsureNamespace(ref XNamespace.refXmlns, "http://www.w3.org/2000/xmlns/");
			}
		}

		public static XNamespace Get(string namespaceName)
		{
			if (namespaceName == null)
			{
				throw new ArgumentNullException("namespaceName");
			}
			return XNamespace.Get(namespaceName, 0, namespaceName.Length);
		}

		[CLSCompliant(false)]
		public static implicit operator XNamespace(string namespaceName)
		{
			if (namespaceName == null)
			{
				return null;
			}
			return XNamespace.Get(namespaceName);
		}

		public static XName operator +(XNamespace ns, string localName)
		{
			if (ns == null)
			{
				throw new ArgumentNullException("ns");
			}
			return ns.GetName(localName);
		}

		public override bool Equals(object obj)
		{
			return this == obj;
		}

		public override int GetHashCode()
		{
			return this.hashCode;
		}

		public static bool operator ==(XNamespace left, XNamespace right)
		{
			return left == right;
		}

		public static bool operator !=(XNamespace left, XNamespace right)
		{
			return left != right;
		}

		internal XName GetName(string localName, int index, int count)
		{
			XName xname;
			if (this.names.TryGetValue(localName, index, count, out xname))
			{
				return xname;
			}
			return this.names.Add(new XName(this, localName.Substring(index, count)));
		}

		internal static XNamespace Get(string namespaceName, int index, int count)
		{
			if (count == 0)
			{
				return XNamespace.None;
			}
			if (XNamespace.namespaces == null)
			{
				Interlocked.CompareExchange<XHashtable<WeakReference>>(ref XNamespace.namespaces, new XHashtable<WeakReference>(new XHashtable<WeakReference>.ExtractKeyDelegate(XNamespace.ExtractNamespace), 32), null);
			}
			for (;;)
			{
				WeakReference weakReference;
				if (!XNamespace.namespaces.TryGetValue(namespaceName, index, count, out weakReference))
				{
					if (count == "http://www.w3.org/XML/1998/namespace".Length && string.CompareOrdinal(namespaceName, index, "http://www.w3.org/XML/1998/namespace", 0, count) == 0)
					{
						break;
					}
					if (count == "http://www.w3.org/2000/xmlns/".Length && string.CompareOrdinal(namespaceName, index, "http://www.w3.org/2000/xmlns/", 0, count) == 0)
					{
						goto Block_7;
					}
					weakReference = XNamespace.namespaces.Add(new WeakReference(new XNamespace(namespaceName.Substring(index, count))));
				}
				XNamespace xnamespace = ((weakReference != null) ? ((XNamespace)weakReference.Target) : null);
				if (!(xnamespace == null))
				{
					return xnamespace;
				}
			}
			return XNamespace.Xml;
			Block_7:
			return XNamespace.Xmlns;
		}

		private static string ExtractLocalName(XName n)
		{
			return n.LocalName;
		}

		private static string ExtractNamespace(WeakReference r)
		{
			XNamespace xnamespace;
			if (r == null || (xnamespace = (XNamespace)r.Target) == null)
			{
				return null;
			}
			return xnamespace.NamespaceName;
		}

		private static XNamespace EnsureNamespace(ref WeakReference refNmsp, string namespaceName)
		{
			XNamespace xnamespace;
			for (;;)
			{
				WeakReference weakReference = refNmsp;
				if (weakReference != null)
				{
					xnamespace = (XNamespace)weakReference.Target;
					if (xnamespace != null)
					{
						break;
					}
				}
				Interlocked.CompareExchange<WeakReference>(ref refNmsp, new WeakReference(new XNamespace(namespaceName)), weakReference);
			}
			return xnamespace;
		}

		internal XNamespace()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		internal const string xmlPrefixNamespace = "http://www.w3.org/XML/1998/namespace";

		internal const string xmlnsPrefixNamespace = "http://www.w3.org/2000/xmlns/";

		private static XHashtable<WeakReference> namespaces;

		private static WeakReference refNone;

		private static WeakReference refXml;

		private static WeakReference refXmlns;

		private string namespaceName;

		private int hashCode;

		private XHashtable<XName> names;

		private const int NamesCapacity = 8;

		private const int NamespacesCapacity = 32;
	}
}
