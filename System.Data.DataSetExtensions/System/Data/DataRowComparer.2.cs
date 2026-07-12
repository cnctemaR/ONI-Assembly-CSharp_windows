using System;
using System.Collections.Generic;

namespace System.Data
{
	public sealed class DataRowComparer<TRow> : IEqualityComparer<TRow> where TRow : DataRow
	{
		private DataRowComparer()
		{
		}

		public static DataRowComparer<TRow> Default
		{
			get
			{
				return DataRowComparer<TRow>.s_instance;
			}
		}

		public bool Equals(TRow leftRow, TRow rightRow)
		{
			if (leftRow == rightRow)
			{
				return true;
			}
			if (leftRow == null || rightRow == null)
			{
				return false;
			}
			if (leftRow.RowState == DataRowState.Deleted || rightRow.RowState == DataRowState.Deleted)
			{
				throw DataSetUtil.InvalidOperation("The DataRowComparer does not work with DataRows that have been deleted since it only compares current values.");
			}
			int count = leftRow.Table.Columns.Count;
			if (count != rightRow.Table.Columns.Count)
			{
				return false;
			}
			for (int i = 0; i < count; i++)
			{
				if (!DataRowComparer.AreEqual(leftRow[i], rightRow[i]))
				{
					return false;
				}
			}
			return true;
		}

		public int GetHashCode(TRow row)
		{
			DataSetUtil.CheckArgumentNull<TRow>(row, "row");
			if (row.RowState == DataRowState.Deleted)
			{
				throw DataSetUtil.InvalidOperation("The DataRowComparer does not work with DataRows that have been deleted since it only compares current values.");
			}
			int num = 0;
			if (row.Table.Columns.Count > 0)
			{
				object obj = row[0];
				if (obj.GetType().IsArray)
				{
					Array array = obj as Array;
					if (array.Rank > 1)
					{
						num = obj.GetHashCode();
					}
					else if (array.Length > 0)
					{
						num = array.GetValue(array.GetLowerBound(0)).GetHashCode();
					}
				}
				else
				{
					ValueType valueType = obj as ValueType;
					if (valueType != null)
					{
						num = valueType.GetHashCode();
					}
					else
					{
						num = obj.GetHashCode();
					}
				}
			}
			return num;
		}

		private static DataRowComparer<TRow> s_instance = new DataRowComparer<TRow>();
	}
}
