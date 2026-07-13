using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_PrefixFsGs : OpCodeHandler
	{
		public OpCodeHandler_PrefixFsGs(Register seg)
		{
			this.seg = seg;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.SegmentPrefix = this.seg;
			decoder.state.zs.segmentPrio = 1;
			decoder.ResetRexPrefixState();
			decoder.CallOpCodeHandlerXXTable(ref instruction);
		}

		private readonly Register seg;
	}
}
