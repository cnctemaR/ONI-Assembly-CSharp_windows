using System;
using System.Collections;

namespace System.Xml.Schema
{
	internal sealed class ConstraintStruct
	{
		internal int TableDim
		{
			get
			{
				return this.tableDim;
			}
		}

		internal ConstraintStruct(CompiledIdentityConstraint constraint)
		{
			this.constraint = constraint;
			this.tableDim = constraint.Fields.Length;
			this.axisFields = new ArrayList();
			this.axisSelector = new SelectorActiveAxis(constraint.Selector, this);
			if (this.constraint.Role != CompiledIdentityConstraint.ConstraintRole.Keyref)
			{
				this.qualifiedTable = new Hashtable();
			}
		}

		internal CompiledIdentityConstraint constraint;

		internal SelectorActiveAxis axisSelector;

		internal ArrayList axisFields;

		internal Hashtable qualifiedTable;

		internal Hashtable keyrefTable;

		private int tableDim;
	}
}
