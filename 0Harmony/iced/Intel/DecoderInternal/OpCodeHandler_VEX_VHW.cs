using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_VEX_VHW : OpCodeHandlerModRM
	{
		public OpCodeHandler_VEX_VHW(Register baseReg, Code codeR, Code codeM)
		{
			this.baseReg1 = baseReg;
			this.baseReg2 = baseReg;
			this.baseReg3 = baseReg;
			this.codeR = codeR;
			this.codeM = codeM;
		}

		public OpCodeHandler_VEX_VHW(Register baseReg, Code code)
		{
			this.baseReg1 = baseReg;
			this.baseReg2 = baseReg;
			this.baseReg3 = baseReg;
			this.codeR = code;
			this.codeM = code;
		}

		public OpCodeHandler_VEX_VHW(Register baseReg1, Register baseReg2, Register baseReg3, Code code)
		{
			this.baseReg1 = baseReg1;
			this.baseReg2 = baseReg2;
			this.baseReg3 = baseReg3;
			this.codeR = code;
			this.codeM = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.Op0Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase) + this.baseReg1;
			instruction.Op1Register = (int)decoder.state.vvvv + this.baseReg2;
			if (decoder.state.mod == 3U)
			{
				instruction.InternalSetCodeNoCheck(this.codeR);
				instruction.Op2Register = (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + this.baseReg3;
				return;
			}
			instruction.InternalSetCodeNoCheck(this.codeM);
			instruction.Op2Kind = OpKind.Memory;
			decoder.ReadOpMem(ref instruction);
		}

		private readonly Register baseReg1;

		private readonly Register baseReg2;

		private readonly Register baseReg3;

		private readonly Code codeR;

		private readonly Code codeM;
	}
}
