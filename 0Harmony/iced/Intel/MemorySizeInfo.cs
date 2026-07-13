using System;

namespace Iced.Intel
{
	internal readonly struct MemorySizeInfo
	{
		public MemorySize MemorySize
		{
			get
			{
				return (MemorySize)this.memorySize;
			}
		}

		public int Size
		{
			get
			{
				return (int)this.size;
			}
		}

		public int ElementSize
		{
			get
			{
				return (int)this.elementSize;
			}
		}

		public MemorySize ElementType
		{
			get
			{
				return (MemorySize)this.elementType;
			}
		}

		public bool IsSigned
		{
			get
			{
				return this.isSigned;
			}
		}

		public bool IsBroadcast
		{
			get
			{
				return this.isBroadcast;
			}
		}

		public bool IsPacked
		{
			get
			{
				return this.elementSize < this.size;
			}
		}

		public int ElementCount
		{
			get
			{
				if (this.elementSize != this.size)
				{
					return (int)(this.size / this.elementSize);
				}
				return 1;
			}
		}

		public MemorySizeInfo(MemorySize memorySize, int size, int elementSize, MemorySize elementType, bool isSigned, bool isBroadcast)
		{
			if (size < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_size();
			}
			if (elementSize < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_elementSize();
			}
			if (elementSize > size)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_elementSize();
			}
			this.memorySize = (byte)memorySize;
			this.size = (ushort)size;
			this.elementSize = (ushort)elementSize;
			this.elementType = (byte)elementType;
			this.isSigned = isSigned;
			this.isBroadcast = isBroadcast;
		}

		private readonly ushort size;

		private readonly ushort elementSize;

		private readonly byte memorySize;

		private readonly byte elementType;

		private readonly bool isSigned;

		private readonly bool isBroadcast;
	}
}
