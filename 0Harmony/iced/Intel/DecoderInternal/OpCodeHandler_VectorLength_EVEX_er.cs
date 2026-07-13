using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class OpCodeHandler_VectorLength_EVEX_er : OpCodeHandlerModRM
	{
		public OpCodeHandler_VectorLength_EVEX_er(OpCodeHandler handler128, OpCodeHandler handler256, OpCodeHandler handler512)
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
			int num3 = 2;
			if (handler512 == null)
			{
				throw new ArgumentNullException("handler512");
			}
			array[num3] = handler512;
			array[3] = OpCodeHandler_Invalid.Instance;
			this.handlers = array;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			int num = (int)decoder.state.vectorLength;
			if (decoder.state.mod == 3U && (decoder.state.zs.flags & StateFlags.b) != (StateFlags)0U)
			{
				num = 2;
			}
			this.handlers[num].Decode(decoder, ref instruction);
		}

		private readonly OpCodeHandler[] handlers;
	}
}
