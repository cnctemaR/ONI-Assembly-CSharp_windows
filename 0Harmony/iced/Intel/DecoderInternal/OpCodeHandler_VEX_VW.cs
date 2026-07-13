using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_VEX_VW : OpCodeHandlerModRM
	{
		public OpCodeHandler_VEX_VW(Register baseReg, Code code)
		{
			this.baseReg1 = baseReg;
			this.baseReg2 = baseReg;
			this.code = code;
		}

		public OpCodeHandler_VEX_VW(Register baseReg1, Register baseReg2, Code code)
		{
			this.baseReg1 = baseReg1;
			this.baseReg2 = baseReg2;
			this.code = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if ((decoder.state.vvvv_invalidCheck & decoder.invalidCheckMask) != 0U)
			{
				decoder.SetInvalidInstruction();
			}
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.Op0Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase) + this.baseReg1;
			if (decoder.state.mod == 3U)
			{
				instruction.Op1Register = (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + this.baseReg2;
				return;
			}
			instruction.Op1Kind = OpKind.Memory;
			decoder.ReadOpMem(ref instruction);
		}

		private readonly Register baseReg1;

		private readonly Register baseReg2;

		private readonly Code code;
	}
}
