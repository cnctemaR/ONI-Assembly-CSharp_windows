using System;
using System.Xml.Schema;

namespace Mono.Xml
{
	internal class DTDElementDeclarationCollection : DTDCollectionBase
	{
		public DTDElementDeclarationCollection(DTDObjectModel root)
			: base(root)
		{
		}

		public DTDElementDeclaration this[string name]
		{
			get
			{
				return this.Get(name);
			}
		}

		public DTDElementDeclaration Get(string name)
		{
			return base.BaseGet(name) as DTDElementDeclaration;
		}

		public void Add(string name, DTDElementDeclaration decl)
		{
			if (base.Contains(name))
			{
				base.Root.AddError(new XmlSchemaException(string.Format("Element declaration for {0} was already added.", name), null));
				return;
			}
			decl.SetRoot(base.Root);
			base.BaseAdd(name, decl);
		}
	}
}
