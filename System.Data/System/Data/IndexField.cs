using System;

namespace System.Data
{
	internal readonly struct IndexField
	{
		internal IndexField(DataColumn column, bool isDescending)
		{
			this.Column = column;
			this.IsDescending = isDescending;
		}

		public static bool operator ==(IndexField if1, IndexField if2)
		{
			return if1.Column == if2.Column && if1.IsDescending == if2.IsDescending;
		}

		public static bool operator !=(IndexField if1, IndexField if2)
		{
			return !(if1 == if2);
		}

		public override bool Equals(object obj)
		{
			return obj is IndexField && this == (IndexField)obj;
		}

		public override int GetHashCode()
		{
			return this.Column.GetHashCode() ^ this.IsDescending.GetHashCode();
		}

		public readonly DataColumn Column;

		public readonly bool IsDescending;
	}
}
