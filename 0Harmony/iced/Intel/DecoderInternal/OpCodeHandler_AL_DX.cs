using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_AL_DX : OpCodeHandler
	{
		public OpCodeHandler_AL_DX(Code code)
		{
			this.code = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.Op0Register = Register.AL;
			instruction.Op1Register = Register.DX;
		}

		private readonly Code code;
	}
}
