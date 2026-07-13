using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_VEX_VWIb : OpCodeHandlerModRM
	{
		public OpCodeHandler_VEX_VWIb(Register baseReg, Code code)
		{
			this.baseReg1 = baseReg;
			this.baseReg2 = baseReg;
			this.codeW0 = code;
			this.codeW1 = code;
		}

		public OpCodeHandler_VEX_VWIb(Register baseReg, Code codeW0, Code codeW1)
		{
			this.baseReg1 = baseReg;
			this.baseReg2 = baseReg;
			this.codeW0 = codeW0;
			this.codeW1 = codeW1;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if ((decoder.state.vvvv_invalidCheck & decoder.invalidCheckMask) != 0U)
			{
				decoder.SetInvalidInstruction();
			}
			if ((decoder.state.zs.flags & (StateFlags)decoder.is64bMode_and_W) != (StateFlags)0U)
			{
				instruction.InternalSetCodeNoCheck(this.codeW1);
			}
			else
			{
				instruction.InternalSetCodeNoCheck(this.codeW0);
			}
			instruction.Op0Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase) + this.baseReg1;
			if (decoder.state.mod == 3U)
			{
				instruction.Op1Register = (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + this.baseReg2;
			}
			else
			{
				instruction.Op1Kind = OpKind.Memory;
				decoder.ReadOpMem(ref instruction);
			}
			instruction.Op2Kind = OpKind.Immediate8;
			instruction.InternalImmediate8 = decoder.ReadByte();
		}

		private readonly Register baseReg1;

		private readonly Register baseReg2;

		private readonly Code codeW0;

		private readonly Code codeW1;
	}
}
