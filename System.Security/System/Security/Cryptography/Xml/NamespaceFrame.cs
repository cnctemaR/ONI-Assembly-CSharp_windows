using System;
using System.Collections;
using System.Xml;

namespace System.Security.Cryptography.Xml
{
	internal class NamespaceFrame
	{
		internal NamespaceFrame()
		{
		}

		internal void AddRendered(XmlAttribute attr)
		{
			this._rendered.Add(Utils.GetNamespacePrefix(attr), attr);
		}

		internal XmlAttribute GetRendered(string nsPrefix)
		{
			return (XmlAttribute)this._rendered[nsPrefix];
		}

		internal void AddUnrendered(XmlAttribute attr)
		{
			this._unrendered.Add(Utils.GetNamespacePrefix(attr), attr);
		}

		internal XmlAttribute GetUnrendered(string nsPrefix)
		{
			return (XmlAttribute)this._unrendered[nsPrefix];
		}

		internal Hashtable GetUnrendered()
		{
			return this._unrendered;
		}

		private Hashtable _rendered = new Hashtable();

		private Hashtable _unrendered = new Hashtable();
	}
}
