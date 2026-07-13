using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Prefix66 : OpCodeHandler
	{
		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			decoder.state.zs.flags = decoder.state.zs.flags | StateFlags.Has66;
			decoder.state.operandSize = decoder.defaultInvertedOperandSize;
			if (decoder.state.zs.mandatoryPrefix == MandatoryPrefixByte.None)
			{
				decoder.state.zs.mandatoryPrefix = MandatoryPrefixByte.P66;
			}
			decoder.ResetRexPrefixState();
			decoder.CallOpCodeHandlerXXTable(ref instruction);
		}
	}
}
