using System;
using System.Collections;

namespace System.Data
{
	internal class ConstraintEnumerator
	{
		public ConstraintEnumerator(DataSet dataSet)
		{
			this._tables = ((dataSet != null) ? dataSet.Tables.GetEnumerator() : null);
			this._currentObject = null;
		}

		public bool GetNext()
		{
			this._currentObject = null;
			while (this._tables != null)
			{
				if (this._constraints == null)
				{
					if (!this._tables.MoveNext())
					{
						this._tables = null;
						return false;
					}
					this._constraints = ((DataTable)this._tables.Current).Constraints.GetEnumerator();
				}
				if (!this._constraints.MoveNext())
				{
					this._constraints = null;
				}
				else
				{
					Constraint constraint = (Constraint)this._constraints.Current;
					if (this.IsValidCandidate(constraint))
					{
						this._currentObject = constraint;
						return true;
					}
				}
			}
			return false;
		}

		public Constraint GetConstraint()
		{
			return this._currentObject;
		}

		protected virtual bool IsValidCandidate(Constraint constraint)
		{
			return true;
		}

		protected Constraint CurrentObject
		{
			get
			{
				return this._currentObject;
			}
		}

		private IEnumerator _tables;

		private IEnumerator _constraints;

		private Constraint _currentObject;
	}
}
