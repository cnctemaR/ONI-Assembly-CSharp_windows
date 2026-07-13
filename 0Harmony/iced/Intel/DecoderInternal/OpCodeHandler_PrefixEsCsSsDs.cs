using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_PrefixEsCsSsDs : OpCodeHandler
	{
		public OpCodeHandler_PrefixEsCsSsDs(Register seg)
		{
			this.seg = seg;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (!decoder.is64bMode || decoder.state.zs.segmentPrio <= 0)
			{
				instruction.SegmentPrefix = this.seg;
			}
			decoder.ResetRexPrefixState();
			decoder.CallOpCodeHandlerXXTable(ref instruction);
		}

		private readonly Register seg;
	}
}
