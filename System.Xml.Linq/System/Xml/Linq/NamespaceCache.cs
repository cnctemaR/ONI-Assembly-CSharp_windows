using System;

namespace System.Xml.Linq
{
	internal struct NamespaceCache
	{
		public XNamespace Get(string namespaceName)
		{
			if (namespaceName == this.namespaceName)
			{
				return this.ns;
			}
			this.namespaceName = namespaceName;
			this.ns = XNamespace.Get(namespaceName);
			return this.ns;
		}

		private XNamespace ns;

		private string namespaceName;
	}
}
