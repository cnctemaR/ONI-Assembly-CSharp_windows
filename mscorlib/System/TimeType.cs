using System;

namespace System
{
	internal class TimeType
	{
		public TimeType(int offset, bool is_dst, string abbrev)
		{
			this.Offset = offset;
			this.IsDst = is_dst;
			this.Name = abbrev;
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"offset: ",
				this.Offset,
				"s, is_dst: ",
				this.IsDst.ToString(),
				", zone name: ",
				this.Name
			});
		}

		public readonly int Offset;

		public readonly bool IsDst;

		public string Name;
	}
}
