using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_VEX_MHV : OpCodeHandlerModRM
	{
		public OpCodeHandler_VEX_MHV(Register baseReg, Code code)
		{
			this.baseReg = baseReg;
			this.code = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.Op1Register = (int)decoder.state.vvvv + this.baseReg;
			instruction.Op2Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase) + this.baseReg;
			if (decoder.state.mod == 3U)
			{
				decoder.SetInvalidInstruction();
				return;
			}
			instruction.Op0Kind = OpKind.Memory;
			decoder.ReadOpMem(ref instruction);
		}

		private readonly Register baseReg;

		private readonly Code code;
	}
}
