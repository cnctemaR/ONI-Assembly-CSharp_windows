using System;

namespace System.Data
{
	public sealed class DataTableNewRowEventArgs : EventArgs
	{
		public DataTableNewRowEventArgs(DataRow row)
		{
			this._row = row;
		}

		public DataRow Row
		{
			get
			{
				return this._row;
			}
		}

		private readonly DataRow _row;
	}
}
