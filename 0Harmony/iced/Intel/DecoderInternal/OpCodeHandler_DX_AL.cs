using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_DX_AL : OpCodeHandler
	{
		public OpCodeHandler_DX_AL(Code code)
		{
			this.code = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.Op0Register = Register.DX;
			instruction.Op1Register = Register.AL;
		}

		private readonly Code code;
	}
}
