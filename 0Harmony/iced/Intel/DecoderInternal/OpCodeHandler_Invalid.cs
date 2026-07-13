using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class OpCodeHandler_Invalid : OpCodeHandlerModRM
	{
		private OpCodeHandler_Invalid()
		{
		}

		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			decoder.SetInvalidInstruction();
		}

		public static readonly OpCodeHandler_Invalid Instance = new OpCodeHandler_Invalid();
	}
}
