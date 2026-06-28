using System;
using System.Collections;

namespace System.Data
{
	internal class Doublet
	{
		public Doublet(int count, string columnname)
		{
			this.count = count;
			this.columnNames.Add(columnname);
		}

		public int count;

		public ArrayList columnNames = new ArrayList();
	}
}
