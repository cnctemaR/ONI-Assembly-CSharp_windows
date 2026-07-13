using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_PrefixF0 : OpCodeHandler
	{
		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetHasLockPrefix();
			decoder.state.zs.flags = decoder.state.zs.flags | StateFlags.Lock;
			decoder.ResetRexPrefixState();
			decoder.CallOpCodeHandlerXXTable(ref instruction);
		}
	}
}
