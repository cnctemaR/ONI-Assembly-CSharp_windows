using System;

namespace System.Data
{
	internal sealed class DataError
	{
		internal DataError()
		{
		}

		internal DataError(string rowError)
		{
			this.SetText(rowError);
		}

		internal string Text
		{
			get
			{
				return this._rowError;
			}
			set
			{
				this.SetText(value);
			}
		}

		internal bool HasErrors
		{
			get
			{
				return this._rowError.Length != 0 || this._count != 0;
			}
		}

		internal void SetColumnError(DataColumn column, string error)
		{
			if (error == null || error.Length == 0)
			{
				this.Clear(column);
				return;
			}
			if (this._errorList == null)
			{
				this._errorList = new DataError.ColumnError[1];
			}
			int num = this.IndexOf(column);
			this._errorList[num]._column = column;
			this._errorList[num]._error = error;
			column._errors++;
			if (num == this._count)
			{
				this._count++;
			}
		}

		internal string GetColumnError(DataColumn column)
		{
			for (int i = 0; i < this._count; i++)
			{
				if (this._errorList[i]._column == column)
				{
					return this._errorList[i]._error;
				}
			}
			return string.Empty;
		}

		internal void Clear(DataColumn column)
		{
			if (this._count == 0)
			{
				return;
			}
			for (int i = 0; i < this._count; i++)
			{
				if (this._errorList[i]._column == column)
				{
					Array.Copy(this._errorList, i + 1, this._errorList, i, this._count - i - 1);
					this._count--;
					column._errors--;
				}
			}
		}

		internal void Clear()
		{
			for (int i = 0; i < this._count; i++)
			{
				this._errorList[i]._column._errors--;
			}
			this._count = 0;
			this._rowError = string.Empty;
		}

		internal DataColumn[] GetColumnsInError()
		{
			DataColumn[] array = new DataColumn[this._count];
			for (int i = 0; i < this._count; i++)
			{
				array[i] = this._errorList[i]._column;
			}
			return array;
		}

		private void SetText(string errorText)
		{
			if (errorText == null)
			{
				errorText = string.Empty;
			}
			this._rowError = errorText;
		}

		internal int IndexOf(DataColumn column)
		{
			for (int i = 0; i < this._count; i++)
			{
				if (this._errorList[i]._column == column)
				{
					return i;
				}
			}
			if (this._count >= this._errorList.Length)
			{
				DataError.ColumnError[] array = new DataError.ColumnError[Math.Min(this._count * 2, column.Table.Columns.Count)];
				Array.Copy(this._errorList, 0, array, 0, this._count);
				this._errorList = array;
			}
			return this._count;
		}

		private string _rowError = string.Empty;

		private int _count;

		private DataError.ColumnError[] _errorList;

		internal const int initialCapacity = 1;

		internal struct ColumnError
		{
			internal DataColumn _column;

			internal string _error;
		}
	}
}
