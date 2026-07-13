using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class OpCodeHandler_VEX2 : OpCodeHandlerModRM
	{
		public OpCodeHandler_VEX2(OpCodeHandler handlerMem)
		{
			if (handlerMem == null)
			{
				throw new ArgumentNullException("handlerMem");
			}
			this.handlerMem = handlerMem;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (decoder.is64bMode)
			{
				decoder.VEX2(ref instruction);
				return;
			}
			if (decoder.state.mod == 3U)
			{
				decoder.VEX2(ref instruction);
				return;
			}
			this.handlerMem.Decode(decoder, ref instruction);
		}

		private readonly OpCodeHandler handlerMem;
	}
}
