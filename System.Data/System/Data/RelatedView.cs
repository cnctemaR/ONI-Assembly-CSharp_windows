using System;

namespace System.Data
{
	internal sealed class RelatedView : DataView, IFilter
	{
		public RelatedView(DataColumn[] columns, object[] values)
			: base(columns[0].Table, false)
		{
			if (values == null)
			{
				throw ExceptionBuilder.ArgumentNull("values");
			}
			this._parentRowView = null;
			this._parentKey = null;
			this._childKey = new DataKey(columns, true);
			this._filterValues = values;
			base.ResetRowViewCache();
		}

		public RelatedView(DataRowView parentRowView, DataKey parentKey, DataColumn[] childKeyColumns)
			: base(childKeyColumns[0].Table, false)
		{
			this._filterValues = null;
			this._parentRowView = parentRowView;
			this._parentKey = new DataKey?(parentKey);
			this._childKey = new DataKey(childKeyColumns, true);
			base.ResetRowViewCache();
		}

		private object[] GetParentValues()
		{
			if (this._filterValues != null)
			{
				return this._filterValues;
			}
			if (!this._parentRowView.HasRecord())
			{
				return null;
			}
			return this._parentKey.Value.GetKeyValues(this._parentRowView.GetRecord());
		}

		public bool Invoke(DataRow row, DataRowVersion version)
		{
			object[] parentValues = this.GetParentValues();
			if (parentValues == null)
			{
				return false;
			}
			object[] keyValues = row.GetKeyValues(this._childKey, version);
			bool flag = true;
			if (keyValues.Length != parentValues.Length)
			{
				flag = false;
			}
			else
			{
				for (int i = 0; i < keyValues.Length; i++)
				{
					if (!keyValues[i].Equals(parentValues[i]))
					{
						flag = false;
						break;
					}
				}
			}
			IFilter filter = base.GetFilter();
			if (filter != null)
			{
				flag &= filter.Invoke(row, version);
			}
			return flag;
		}

		internal override IFilter GetFilter()
		{
			return this;
		}

		public override DataRowView AddNew()
		{
			DataRowView dataRowView = base.AddNew();
			dataRowView.Row.SetKeyValues(this._childKey, this.GetParentValues());
			return dataRowView;
		}

		internal override void SetIndex(string newSort, DataViewRowState newRowStates, IFilter newRowFilter)
		{
			base.SetIndex2(newSort, newRowStates, newRowFilter, false);
			base.Reset();
		}

		public override bool Equals(DataView dv)
		{
			RelatedView relatedView = dv as RelatedView;
			if (relatedView == null)
			{
				return false;
			}
			if (!base.Equals(dv))
			{
				return false;
			}
			object[] array;
			if (this._filterValues != null)
			{
				array = this._childKey.ColumnsReference;
				object[] array2 = array;
				array = relatedView._childKey.ColumnsReference;
				return this.CompareArray(array2, array) && this.CompareArray(this._filterValues, relatedView._filterValues);
			}
			if (relatedView._filterValues != null)
			{
				return false;
			}
			array = this._childKey.ColumnsReference;
			object[] array3 = array;
			array = relatedView._childKey.ColumnsReference;
			if (this.CompareArray(array3, array))
			{
				array = this._parentKey.Value.ColumnsReference;
				object[] array4 = array;
				array = this._parentKey.Value.ColumnsReference;
				if (this.CompareArray(array4, array))
				{
					return this._parentRowView.Equals(relatedView._parentRowView);
				}
			}
			return false;
		}

		private bool CompareArray(object[] value1, object[] value2)
		{
			if (value1 == null || value2 == null)
			{
				return value1 == value2;
			}
			if (value1.Length != value2.Length)
			{
				return false;
			}
			for (int i = 0; i < value1.Length; i++)
			{
				if (value1[i] != value2[i])
				{
					return false;
				}
			}
			return true;
		}

		private readonly DataKey? _parentKey;

		private readonly DataKey _childKey;

		private readonly DataRowView _parentRowView;

		private readonly object[] _filterValues;
	}
}
