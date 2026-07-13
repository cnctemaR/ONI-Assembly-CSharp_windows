using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_EVEX_WkV : OpCodeHandlerModRM
	{
		public OpCodeHandler_EVEX_WkV(Register baseReg, Code code, TupleType tupleType)
		{
			this.baseReg1 = baseReg;
			this.baseReg2 = baseReg;
			this.code = code;
			this.tupleType = tupleType;
			this.disallowZeroingMasking = 0U;
		}

		public OpCodeHandler_EVEX_WkV(Register baseReg, Code code, TupleType tupleType, bool allowZeroingMasking)
		{
			this.baseReg1 = baseReg;
			this.baseReg2 = baseReg;
			this.code = code;
			this.tupleType = tupleType;
			this.disallowZeroingMasking = (allowZeroingMasking ? 0U : uint.MaxValue);
		}

		public OpCodeHandler_EVEX_WkV(Register baseReg1, Register baseReg2, Code code, TupleType tupleType)
		{
			this.baseReg1 = baseReg1;
			this.baseReg2 = baseReg2;
			this.code = code;
			this.tupleType = tupleType;
			this.disallowZeroingMasking = 0U;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if ((((decoder.state.zs.flags & StateFlags.b) | (StateFlags)decoder.state.vvvv_invalidCheck) & (StateFlags)decoder.invalidCheckMask) != (StateFlags)0U)
			{
				decoder.SetInvalidInstruction();
			}
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.Op1Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase + decoder.state.extraRegisterBaseEVEX) + this.baseReg2;
			if ((decoder.state.zs.flags & StateFlags.z & (StateFlags)this.disallowZeroingMasking & (StateFlags)decoder.invalidCheckMask) != (StateFlags)0U)
			{
				decoder.SetInvalidInstruction();
			}
			if (decoder.state.mod == 3U)
			{
				instruction.Op0Register = (int)(decoder.state.rm + decoder.state.extraBaseRegisterBaseEVEX) + this.baseReg1;
				return;
			}
			instruction.Op0Kind = OpKind.Memory;
			if ((decoder.state.zs.flags & StateFlags.z & (StateFlags)decoder.invalidCheckMask) != (StateFlags)0U)
			{
				decoder.SetInvalidInstruction();
			}
			decoder.ReadOpMem(ref instruction, this.tupleType);
		}

		private readonly Register baseReg1;

		private readonly Register baseReg2;

		private readonly Code code;

		private readonly TupleType tupleType;

		private readonly uint disallowZeroingMasking;
	}
}
