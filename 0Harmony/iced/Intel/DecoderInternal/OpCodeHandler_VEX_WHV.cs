using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_VEX_WHV : OpCodeHandlerModRM
	{
		public OpCodeHandler_VEX_WHV(Register baseReg, Code code)
		{
			this.baseReg = baseReg;
			this.codeR = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.codeR);
			instruction.Op0Register = (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + this.baseReg;
			instruction.Op1Register = (int)decoder.state.vvvv + this.baseReg;
			instruction.Op2Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase) + this.baseReg;
		}

		private readonly Register baseReg;

		private readonly Code codeR;
	}
}
