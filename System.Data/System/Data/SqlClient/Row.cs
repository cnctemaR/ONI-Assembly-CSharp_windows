using System;

namespace System.Data.SqlClient
{
	internal sealed class Row
	{
		internal Row(int rowCount)
		{
			this._dataFields = new object[rowCount];
		}

		internal object[] DataFields
		{
			get
			{
				return this._dataFields;
			}
		}

		internal object this[int index]
		{
			get
			{
				return this._dataFields[index];
			}
		}

		private object[] _dataFields;
	}
}
