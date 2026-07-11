using System;

namespace System.Data
{
	internal sealed class ChildForeignKeyConstraintEnumerator : ForeignKeyConstraintEnumerator
	{
		public ChildForeignKeyConstraintEnumerator(DataSet dataSet, DataTable inTable)
			: base(dataSet)
		{
			this._table = inTable;
		}

		protected override bool IsValidCandidate(Constraint constraint)
		{
			return constraint is ForeignKeyConstraint && ((ForeignKeyConstraint)constraint).Table == this._table;
		}

		private readonly DataTable _table;
	}
}
