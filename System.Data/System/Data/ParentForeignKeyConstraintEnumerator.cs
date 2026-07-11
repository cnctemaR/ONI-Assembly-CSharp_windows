using System;

namespace System.Data
{
	internal sealed class ParentForeignKeyConstraintEnumerator : ForeignKeyConstraintEnumerator
	{
		public ParentForeignKeyConstraintEnumerator(DataSet dataSet, DataTable inTable)
			: base(dataSet)
		{
			this._table = inTable;
		}

		protected override bool IsValidCandidate(Constraint constraint)
		{
			return constraint is ForeignKeyConstraint && ((ForeignKeyConstraint)constraint).RelatedTable == this._table;
		}

		private readonly DataTable _table;
	}
}
