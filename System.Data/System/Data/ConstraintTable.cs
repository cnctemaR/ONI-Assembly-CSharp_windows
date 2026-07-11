using System;
using System.Xml.Schema;

namespace System.Data
{
	internal sealed class ConstraintTable
	{
		public ConstraintTable(DataTable t, XmlSchemaIdentityConstraint c)
		{
			this.table = t;
			this.constraint = c;
		}

		public DataTable table;

		public XmlSchemaIdentityConstraint constraint;
	}
}
