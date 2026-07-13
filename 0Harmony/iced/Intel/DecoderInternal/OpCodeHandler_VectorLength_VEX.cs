using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class OpCodeHandler_VectorLength_VEX : OpCodeHandlerModRM
	{
		public OpCodeHandler_VectorLength_VEX(OpCodeHandler handler128, OpCodeHandler handler256)
		{
			OpCodeHandler[] array = new OpCodeHandler[4];
			int num = 0;
			if (handler128 == null)
			{
				throw new ArgumentNullException("handler128");
			}
			array[num] = handler128;
			int num2 = 1;
			if (handler256 == null)
			{
				throw new ArgumentNullException("handler256");
			}
			array[num2] = handler256;
			array[2] = OpCodeHandler_Invalid.Instance;
			array[3] = OpCodeHandler_Invalid.Instance;
			this.handlers = array;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			this.handlers[(int)decoder.state.vectorLength].Decode(decoder, ref instruction);
		}

		private readonly OpCodeHandler[] handlers;
	}
}
