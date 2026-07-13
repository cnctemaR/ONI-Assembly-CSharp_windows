using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_SimpleReg : OpCodeHandler
	{
		public OpCodeHandler_SimpleReg(Code code, int index)
		{
			this.code = code;
			this.index = index;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			int operandSize = (int)decoder.state.operandSize;
			instruction.InternalSetCodeNoCheck(operandSize + this.code);
			instruction.Op0Register = operandSize * 16 + this.index + (int)decoder.state.zs.extraBaseRegisterBase + Register.AX;
		}

		private readonly Code code;

		private readonly int index;
	}
}
