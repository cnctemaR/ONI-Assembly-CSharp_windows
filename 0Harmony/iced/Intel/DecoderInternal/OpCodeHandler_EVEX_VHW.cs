using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_EVEX_VHW : OpCodeHandlerModRM
	{
		public OpCodeHandler_EVEX_VHW(Register baseReg, Code codeR, Code codeM, TupleType tupleType)
		{
			this.baseReg1 = baseReg;
			this.baseReg2 = baseReg;
			this.baseReg3 = baseReg;
			this.codeR = codeR;
			this.codeM = codeM;
			this.tupleType = tupleType;
		}

		public OpCodeHandler_EVEX_VHW(Register baseReg, Code code, TupleType tupleType)
		{
			this.baseReg1 = baseReg;
			this.baseReg2 = baseReg;
			this.baseReg3 = baseReg;
			this.codeR = code;
			this.codeM = code;
			this.tupleType = tupleType;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if ((((decoder.state.zs.flags & (StateFlags.b | StateFlags.z)) | (StateFlags)decoder.state.aaa) & (StateFlags)decoder.invalidCheckMask) != (StateFlags)0U)
			{
				decoder.SetInvalidInstruction();
			}
			instruction.Op0Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase + decoder.state.extraRegisterBaseEVEX) + this.baseReg1;
			instruction.Op1Register = (int)decoder.state.vvvv + this.baseReg2;
			if (decoder.state.mod == 3U)
			{
				instruction.InternalSetCodeNoCheck(this.codeR);
				instruction.Op2Register = (int)(decoder.state.rm + decoder.state.extraBaseRegisterBaseEVEX) + this.baseReg3;
				return;
			}
			instruction.InternalSetCodeNoCheck(this.codeM);
			instruction.Op2Kind = OpKind.Memory;
			decoder.ReadOpMem(ref instruction, this.tupleType);
		}

		private readonly Register baseReg1;

		private readonly Register baseReg2;

		private readonly Register baseReg3;

		private readonly Code codeR;

		private readonly Code codeM;

		private readonly TupleType tupleType;
	}
}
