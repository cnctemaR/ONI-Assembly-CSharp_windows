using System;

namespace System.Xml.Schema
{
	internal class CompiledIdentityConstraint
	{
		public CompiledIdentityConstraint.ConstraintRole Role
		{
			get
			{
				return this.role;
			}
		}

		public Asttree Selector
		{
			get
			{
				return this.selector;
			}
		}

		public Asttree[] Fields
		{
			get
			{
				return this.fields;
			}
		}

		private CompiledIdentityConstraint()
		{
		}

		public CompiledIdentityConstraint(XmlSchemaIdentityConstraint constraint, XmlNamespaceManager nsmgr)
		{
			this.name = constraint.QualifiedName;
			try
			{
				this.selector = new Asttree(constraint.Selector.XPath, false, nsmgr);
			}
			catch (XmlSchemaException ex)
			{
				ex.SetSource(constraint.Selector);
				throw ex;
			}
			XmlSchemaObjectCollection xmlSchemaObjectCollection = constraint.Fields;
			this.fields = new Asttree[xmlSchemaObjectCollection.Count];
			for (int i = 0; i < xmlSchemaObjectCollection.Count; i++)
			{
				try
				{
					this.fields[i] = new Asttree(((XmlSchemaXPath)xmlSchemaObjectCollection[i]).XPath, true, nsmgr);
				}
				catch (XmlSchemaException ex2)
				{
					ex2.SetSource(constraint.Fields[i]);
					throw ex2;
				}
			}
			if (constraint is XmlSchemaUnique)
			{
				this.role = CompiledIdentityConstraint.ConstraintRole.Unique;
				return;
			}
			if (constraint is XmlSchemaKey)
			{
				this.role = CompiledIdentityConstraint.ConstraintRole.Key;
				return;
			}
			this.role = CompiledIdentityConstraint.ConstraintRole.Keyref;
			this.refer = ((XmlSchemaKeyref)constraint).Refer;
		}

		internal XmlQualifiedName name = XmlQualifiedName.Empty;

		private CompiledIdentityConstraint.ConstraintRole role;

		private Asttree selector;

		private Asttree[] fields;

		internal XmlQualifiedName refer = XmlQualifiedName.Empty;

		public static readonly CompiledIdentityConstraint Empty = new CompiledIdentityConstraint();

		public enum ConstraintRole
		{
			Unique,
			Key,
			Keyref
		}
	}
}
