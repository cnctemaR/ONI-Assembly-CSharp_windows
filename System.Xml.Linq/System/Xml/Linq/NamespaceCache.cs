using System;

namespace System.Xml.Linq
{
	internal struct NamespaceCache
	{
		public XNamespace Get(string namespaceName)
		{
			if (namespaceName == this._namespaceName)
			{
				return this._ns;
			}
			this._namespaceName = namespaceName;
			this._ns = XNamespace.Get(namespaceName);
			return this._ns;
		}

		private XNamespace _ns;

		private string _namespaceName;
	}
}
