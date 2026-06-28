using System;
using System.Collections.Specialized;

namespace System.Xml.Serialization
{
	public class XmlSerializerNamespaces
	{
		public XmlSerializerNamespaces()
		{
			this.namespaces = new ListDictionary();
		}

		public XmlSerializerNamespaces(XmlQualifiedName[] namespaces)
			: this()
		{
			foreach (XmlQualifiedName xmlQualifiedName in namespaces)
			{
				this.namespaces[xmlQualifiedName.Name] = xmlQualifiedName;
			}
		}

		public XmlSerializerNamespaces(XmlSerializerNamespaces namespaces)
			: this(namespaces.ToArray())
		{
		}

		public void Add(string prefix, string ns)
		{
			XmlQualifiedName xmlQualifiedName = new XmlQualifiedName(prefix, ns);
			this.namespaces[xmlQualifiedName.Name] = xmlQualifiedName;
		}

		public XmlQualifiedName[] ToArray()
		{
			XmlQualifiedName[] array = new XmlQualifiedName[this.namespaces.Count];
			this.namespaces.Values.CopyTo(array, 0);
			return array;
		}

		public int Count
		{
			get
			{
				return this.namespaces.Count;
			}
		}

		internal string GetPrefix(string Ns)
		{
			foreach (object obj in this.namespaces.Keys)
			{
				string text = (string)obj;
				if (Ns == ((XmlQualifiedName)this.namespaces[text]).Namespace)
				{
					return text;
				}
			}
			return null;
		}

		internal ListDictionary Namespaces
		{
			get
			{
				return this.namespaces;
			}
		}

		private ListDictionary namespaces;
	}
}
