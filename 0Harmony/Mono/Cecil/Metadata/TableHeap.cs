using System;

namespace Mono.Cecil.Metadata
{
	internal sealed class TableHeap : Heap
	{
		public TableInformation this[Table table]
		{
			get
			{
				return this.Tables[(int)table];
			}
		}

		public TableHeap(byte[] data)
			: base(data)
		{
		}

		public bool HasTable(Table table)
		{
			return (this.Valid & (1L << (int)table)) != 0L;
		}

		public long Valid;

		public long Sorted;

		public readonly TableInformation[] Tables = new TableInformation[58];
	}
}
