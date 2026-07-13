using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class OpCodeHandler_W : OpCodeHandlerModRM
	{
		public OpCodeHandler_W(OpCodeHandler handlerW0, OpCodeHandler handlerW1)
		{
			if (handlerW0 == null)
			{
				throw new ArgumentNullException("handlerW0");
			}
			this.handlerW0 = handlerW0;
			if (handlerW1 == null)
			{
				throw new ArgumentNullException("handlerW1");
			}
			this.handlerW1 = handlerW1;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			(((decoder.state.zs.flags & StateFlags.W) != (StateFlags)0U) ? this.handlerW1 : this.handlerW0).Decode(decoder, ref instruction);
		}

		private readonly OpCodeHandler handlerW0;

		private readonly OpCodeHandler handlerW1;
	}
}
