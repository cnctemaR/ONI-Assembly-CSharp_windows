using System;

namespace System.Data
{
	public sealed class DataTableNewRowEventArgs : EventArgs
	{
		public DataTableNewRowEventArgs(DataRow dataRow)
		{
		}

		public DataRow Row
		{
			get
			{
				throw null;
			}
		}
	}
}
