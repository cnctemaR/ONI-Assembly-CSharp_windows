using System;

namespace Iced.Intel
{
	internal readonly struct MemoryOperand
	{
		public MemoryOperand(Register @base, Register index, int scale, long displacement, int displSize, bool isBroadcast, Register segmentPrefix)
		{
			this.SegmentPrefix = segmentPrefix;
			this.Base = @base;
			this.Index = index;
			this.Scale = scale;
			this.Displacement = displacement;
			this.DisplSize = displSize;
			this.IsBroadcast = isBroadcast;
		}

		public MemoryOperand(Register @base, Register index, int scale, bool isBroadcast, Register segmentPrefix)
		{
			this.SegmentPrefix = segmentPrefix;
			this.Base = @base;
			this.Index = index;
			this.Scale = scale;
			this.Displacement = 0L;
			this.DisplSize = 0;
			this.IsBroadcast = isBroadcast;
		}

		public MemoryOperand(Register @base, long displacement, int displSize, bool isBroadcast, Register segmentPrefix)
		{
			this.SegmentPrefix = segmentPrefix;
			this.Base = @base;
			this.Index = Register.None;
			this.Scale = 1;
			this.Displacement = displacement;
			this.DisplSize = displSize;
			this.IsBroadcast = isBroadcast;
		}

		public MemoryOperand(Register index, int scale, long displacement, int displSize, bool isBroadcast, Register segmentPrefix)
		{
			this.SegmentPrefix = segmentPrefix;
			this.Base = Register.None;
			this.Index = index;
			this.Scale = scale;
			this.Displacement = displacement;
			this.DisplSize = displSize;
			this.IsBroadcast = isBroadcast;
		}

		public MemoryOperand(Register @base, long displacement, bool isBroadcast, Register segmentPrefix)
		{
			this.SegmentPrefix = segmentPrefix;
			this.Base = @base;
			this.Index = Register.None;
			this.Scale = 1;
			this.Displacement = displacement;
			this.DisplSize = 1;
			this.IsBroadcast = isBroadcast;
		}

		public MemoryOperand(Register @base, Register index, int scale, long displacement, int displSize)
		{
			this.SegmentPrefix = Register.None;
			this.Base = @base;
			this.Index = index;
			this.Scale = scale;
			this.Displacement = displacement;
			this.DisplSize = displSize;
			this.IsBroadcast = false;
		}

		public MemoryOperand(Register @base, Register index, int scale)
		{
			this.SegmentPrefix = Register.None;
			this.Base = @base;
			this.Index = index;
			this.Scale = scale;
			this.Displacement = 0L;
			this.DisplSize = 0;
			this.IsBroadcast = false;
		}

		public MemoryOperand(Register @base, Register index)
		{
			this.SegmentPrefix = Register.None;
			this.Base = @base;
			this.Index = index;
			this.Scale = 1;
			this.Displacement = 0L;
			this.DisplSize = 0;
			this.IsBroadcast = false;
		}

		public MemoryOperand(Register @base, long displacement, int displSize)
		{
			this.SegmentPrefix = Register.None;
			this.Base = @base;
			this.Index = Register.None;
			this.Scale = 1;
			this.Displacement = displacement;
			this.DisplSize = displSize;
			this.IsBroadcast = false;
		}

		public MemoryOperand(Register index, int scale, long displacement, int displSize)
		{
			this.SegmentPrefix = Register.None;
			this.Base = Register.None;
			this.Index = index;
			this.Scale = scale;
			this.Displacement = displacement;
			this.DisplSize = displSize;
			this.IsBroadcast = false;
		}

		public MemoryOperand(Register @base, long displacement)
		{
			this.SegmentPrefix = Register.None;
			this.Base = @base;
			this.Index = Register.None;
			this.Scale = 1;
			this.Displacement = displacement;
			this.DisplSize = 1;
			this.IsBroadcast = false;
		}

		public MemoryOperand(Register @base)
		{
			this.SegmentPrefix = Register.None;
			this.Base = @base;
			this.Index = Register.None;
			this.Scale = 1;
			this.Displacement = 0L;
			this.DisplSize = 0;
			this.IsBroadcast = false;
		}

		public MemoryOperand(ulong displacement, int displSize)
		{
			this.SegmentPrefix = Register.None;
			this.Base = Register.None;
			this.Index = Register.None;
			this.Scale = 1;
			this.Displacement = (long)displacement;
			this.DisplSize = displSize;
			this.IsBroadcast = false;
		}

		public readonly Register SegmentPrefix;

		public readonly Register Base;

		public readonly Register Index;

		public readonly int Scale;

		public readonly long Displacement;

		public readonly int DisplSize;

		public readonly bool IsBroadcast;
	}
}
