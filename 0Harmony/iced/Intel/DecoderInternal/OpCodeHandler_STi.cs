using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_STi : OpCodeHandlerModRM
	{
		public OpCodeHandler_STi(Code code)
		{
			this.code = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.Op0Register = Register.ST0 + (int)decoder.state.rm;
		}

		private readonly Code code;
	}
}
