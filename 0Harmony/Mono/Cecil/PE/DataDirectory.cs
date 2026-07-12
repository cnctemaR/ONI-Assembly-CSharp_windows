using System;

namespace Mono.Cecil.PE
{
	internal struct DataDirectory
	{
		public bool IsZero
		{
			get
			{
				return this.VirtualAddress == 0U && this.Size == 0U;
			}
		}

		public DataDirectory(uint rva, uint size)
		{
			this.VirtualAddress = rva;
			this.Size = size;
		}

		public readonly uint VirtualAddress;

		public readonly uint Size;
	}
}
