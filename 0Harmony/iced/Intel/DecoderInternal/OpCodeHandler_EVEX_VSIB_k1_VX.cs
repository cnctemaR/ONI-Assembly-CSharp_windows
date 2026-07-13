using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_EVEX_VSIB_k1_VX : OpCodeHandlerModRM
	{
		public OpCodeHandler_EVEX_VSIB_k1_VX(Register vsibIndex, Register baseReg, Code code, TupleType tupleType)
		{
			this.vsibIndex = vsibIndex;
			this.baseReg = baseReg;
			this.code = code;
			this.tupleType = tupleType;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (decoder.invalidCheckMask != 0U && (((decoder.state.zs.flags & (StateFlags.b | StateFlags.z)) | (StateFlags)(decoder.state.vvvv_invalidCheck & 15U)) != (StateFlags)0U || decoder.state.aaa == 0U))
			{
				decoder.SetInvalidInstruction();
			}
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.Op1Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase + decoder.state.extraRegisterBaseEVEX) + this.baseReg;
			if (decoder.state.mod == 3U)
			{
				decoder.SetInvalidInstruction();
				return;
			}
			instruction.Op0Kind = OpKind.Memory;
			decoder.ReadOpMem_VSIB(ref instruction, this.vsibIndex, this.tupleType);
		}

		private readonly Register vsibIndex;

		private readonly Register baseReg;

		private readonly Code code;

		private readonly TupleType tupleType;
	}
}
