using System;
using Mono.Data.SqlExpressions;

namespace System.Data
{
	internal class RelatedDataView : DataView, IExpression
	{
		internal RelatedDataView(DataColumn[] relatedColumns, object[] keyValues)
		{
			this.dataTable = relatedColumns[0].Table;
			this.rowState = DataViewRowState.CurrentRows;
			this._columns = relatedColumns;
			this._keyValues = keyValues;
			base.Open();
		}

		void IExpression.ResetExpression()
		{
		}

		internal override IExpression FilterExpression
		{
			get
			{
				return this;
			}
		}

		public override bool Equals(object obj)
		{
			if (!(obj is RelatedDataView))
			{
				return base.FilterExpression != null && base.FilterExpression.Equals(obj);
			}
			RelatedDataView relatedDataView = (RelatedDataView)obj;
			if (this._columns.Length != relatedDataView._columns.Length)
			{
				return false;
			}
			for (int i = 0; i < this._columns.Length; i++)
			{
				if (!this._columns[i].Equals(relatedDataView._columns[i]) || !this._keyValues[i].Equals(relatedDataView._keyValues[i]))
				{
					return false;
				}
			}
			return relatedDataView.FilterExpression.Equals(base.FilterExpression);
		}

		public override int GetHashCode()
		{
			int num = 0;
			for (int i = 0; i < this._columns.Length; i++)
			{
				num ^= this._columns[i].GetHashCode();
				num ^= this._keyValues[i].GetHashCode();
			}
			if (base.FilterExpression != null)
			{
				num ^= base.FilterExpression.GetHashCode();
			}
			return num;
		}

		public object Eval(DataRow row)
		{
			return this.EvalBoolean(row);
		}

		public bool EvalBoolean(DataRow row)
		{
			for (int i = 0; i < this._columns.Length; i++)
			{
				if (!row[this._columns[i]].Equals(this._keyValues[i]))
				{
					return false;
				}
			}
			IExpression filterExpression = base.FilterExpression;
			return filterExpression == null || filterExpression.EvalBoolean(row);
		}

		public bool DependsOn(DataColumn other)
		{
			for (int i = 0; i < this._columns.Length; i++)
			{
				if (this._columns[i] == other)
				{
					return true;
				}
			}
			IExpression filterExpression = base.FilterExpression;
			return filterExpression != null && filterExpression.DependsOn(other);
		}

		private object[] _keyValues;

		private DataColumn[] _columns;
	}
}
