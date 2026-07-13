using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_STi_ST : OpCodeHandlerModRM
	{
		public OpCodeHandler_STi_ST(Code code)
		{
			this.code = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.Op0Register = Register.ST0 + (int)decoder.state.rm;
			instruction.Op1Register = Register.ST0;
		}

		private readonly Code code;
	}
}
