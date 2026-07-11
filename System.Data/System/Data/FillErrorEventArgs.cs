using System;

namespace System.Data
{
	public class FillErrorEventArgs : EventArgs
	{
		public FillErrorEventArgs(DataTable dataTable, object[] values)
		{
			this._dataTable = dataTable;
			this._values = values;
			if (this._values == null)
			{
				this._values = Array.Empty<object>();
			}
		}

		public bool Continue
		{
			get
			{
				return this._continueFlag;
			}
			set
			{
				this._continueFlag = value;
			}
		}

		public DataTable DataTable
		{
			get
			{
				return this._dataTable;
			}
		}

		public Exception Errors
		{
			get
			{
				return this._errors;
			}
			set
			{
				this._errors = value;
			}
		}

		public object[] Values
		{
			get
			{
				object[] array = new object[this._values.Length];
				for (int i = 0; i < this._values.Length; i++)
				{
					array[i] = this._values[i];
				}
				return array;
			}
		}

		private bool _continueFlag;

		private DataTable _dataTable;

		private Exception _errors;

		private object[] _values;
	}
}
