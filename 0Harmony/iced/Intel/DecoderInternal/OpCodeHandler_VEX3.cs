using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class OpCodeHandler_VEX3 : OpCodeHandlerModRM
	{
		public OpCodeHandler_VEX3(OpCodeHandler handlerMem)
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
				decoder.VEX3(ref instruction);
				return;
			}
			if (decoder.state.mod == 3U)
			{
				decoder.VEX3(ref instruction);
				return;
			}
			this.handlerMem.Decode(decoder, ref instruction);
		}

		private readonly OpCodeHandler handlerMem;
	}
}
