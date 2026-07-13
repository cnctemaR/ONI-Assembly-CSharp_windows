using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Iced.Intel
{
	[NullableContext(1)]
	[Nullable(0)]
	internal readonly struct BlockEncoderResult
	{
		[NullableContext(2)]
		internal BlockEncoderResult(ulong rip, List<RelocInfo> relocInfos, uint[] newInstructionOffsets, ConstantOffsets[] constantOffsets)
		{
			this.RIP = rip;
			this.RelocInfos = relocInfos;
			this.NewInstructionOffsets = newInstructionOffsets ?? Array2.Empty<uint>();
			this.ConstantOffsets = constantOffsets ?? Array2.Empty<ConstantOffsets>();
		}

		public readonly ulong RIP;

		[Nullable(2)]
		public readonly List<RelocInfo> RelocInfos;

		public readonly uint[] NewInstructionOffsets;

		public readonly ConstantOffsets[] ConstantOffsets;
	}
}
