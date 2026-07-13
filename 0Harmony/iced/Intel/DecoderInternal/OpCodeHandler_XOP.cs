using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class OpCodeHandler_XOP : OpCodeHandlerModRM
	{
		public OpCodeHandler_XOP(OpCodeHandler handler_reg0)
		{
			if (handler_reg0 == null)
			{
				throw new ArgumentNullException("handler_reg0");
			}
			this.handler_reg0 = handler_reg0;
		}

		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if ((decoder.state.modrm & 31U) < 8U)
			{
				this.handler_reg0.Decode(decoder, ref instruction);
				return;
			}
			decoder.XOP(ref instruction);
		}

		private readonly OpCodeHandler handler_reg0;
	}
}
