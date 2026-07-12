using System;

namespace Mono.Cecil
{
	internal abstract class MetadataTable<TRow> : MetadataTable where TRow : struct
	{
		public sealed override int Length
		{
			get
			{
				return this.length;
			}
		}

		public int AddRow(TRow row)
		{
			if (this.rows.Length == this.length)
			{
				this.Grow();
			}
			TRow[] array = this.rows;
			int num = this.length;
			this.length = num + 1;
			array[num] = row;
			return this.length;
		}

		private void Grow()
		{
			TRow[] array = new TRow[this.rows.Length * 2];
			Array.Copy(this.rows, array, this.rows.Length);
			this.rows = array;
		}

		public override void Sort()
		{
		}

		internal TRow[] rows = new TRow[2];

		internal int length;
	}
}
