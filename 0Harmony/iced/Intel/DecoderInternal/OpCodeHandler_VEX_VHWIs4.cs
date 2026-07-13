using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_VEX_VHWIs4 : OpCodeHandlerModRM
	{
		public OpCodeHandler_VEX_VHWIs4(Register baseReg, Code code)
		{
			this.baseReg = baseReg;
			this.code = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.Op0Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase) + this.baseReg;
			instruction.Op1Register = (int)decoder.state.vvvv + this.baseReg;
			if (decoder.state.mod == 3U)
			{
				instruction.Op2Register = (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + this.baseReg;
			}
			else
			{
				instruction.Op2Kind = OpKind.Memory;
				decoder.ReadOpMem(ref instruction);
			}
			instruction.Op3Register = (int)((decoder.ReadByte() >> 4) & decoder.reg15Mask) + this.baseReg;
		}

		private readonly Register baseReg;

		private readonly Code code;
	}
}
