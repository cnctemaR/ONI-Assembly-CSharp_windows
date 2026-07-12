using System;

namespace Mono.Cecil.Metadata
{
	internal struct TableInformation
	{
		public bool IsLarge
		{
			get
			{
				return this.Length > 65535U;
			}
		}

		public uint Offset;

		public uint Length;

		public uint RowSize;
	}
}
