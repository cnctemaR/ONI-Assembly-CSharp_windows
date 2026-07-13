using System;

namespace Iced.Intel
{
	internal struct ConstantOffsets
	{
		public readonly bool HasDisplacement
		{
			get
			{
				return this.DisplacementSize > 0;
			}
		}

		public readonly bool HasImmediate
		{
			get
			{
				return this.ImmediateSize > 0;
			}
		}

		public readonly bool HasImmediate2
		{
			get
			{
				return this.ImmediateSize2 > 0;
			}
		}

		public byte DisplacementOffset;

		public byte DisplacementSize;

		public byte ImmediateOffset;

		public byte ImmediateSize;

		public byte ImmediateOffset2;

		public byte ImmediateSize2;

		private byte pad1;

		private byte pad2;
	}
}
