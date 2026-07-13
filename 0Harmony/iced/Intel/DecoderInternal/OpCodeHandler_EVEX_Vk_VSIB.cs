using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_EVEX_Vk_VSIB : OpCodeHandlerModRM
	{
		public OpCodeHandler_EVEX_Vk_VSIB(Register baseReg, Register vsibBase, Code code, TupleType tupleType)
		{
			this.baseReg = baseReg;
			this.vsibBase = vsibBase;
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
			int num = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase + decoder.state.extraRegisterBaseEVEX);
			instruction.Op0Register = num + this.baseReg;
			if (decoder.state.mod == 3U)
			{
				decoder.SetInvalidInstruction();
				return;
			}
			instruction.Op1Kind = OpKind.Memory;
			decoder.ReadOpMem_VSIB(ref instruction, this.vsibBase, this.tupleType);
			if (decoder.invalidCheckMask != 0U && num == (instruction.MemoryIndex - Register.XMM0) % 32)
			{
				decoder.SetInvalidInstruction();
			}
		}

		private readonly Register baseReg;

		private readonly Register vsibBase;

		private readonly Code code;

		private readonly TupleType tupleType;
	}
}
