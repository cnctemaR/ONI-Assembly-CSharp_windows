using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_EVEX_V_H_Ev_er : OpCodeHandlerModRM
	{
		public OpCodeHandler_EVEX_V_H_Ev_er(Register baseReg, Code codeW0, Code codeW1, TupleType tupleTypeW0, TupleType tupleTypeW1)
		{
			this.baseReg = baseReg;
			this.codeW0 = codeW0;
			this.codeW1 = codeW1;
			this.tupleTypeW0 = tupleTypeW0;
			this.tupleTypeW1 = tupleTypeW1;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if ((((decoder.state.zs.flags & StateFlags.z) | (StateFlags)decoder.state.aaa) & (StateFlags)decoder.invalidCheckMask) != (StateFlags)0U)
			{
				decoder.SetInvalidInstruction();
			}
			TupleType tupleType;
			Register register;
			if ((decoder.state.zs.flags & (StateFlags)decoder.is64bMode_and_W) != (StateFlags)0U)
			{
				instruction.InternalSetCodeNoCheck(this.codeW1);
				tupleType = this.tupleTypeW1;
				register = Register.RAX;
			}
			else
			{
				instruction.InternalSetCodeNoCheck(this.codeW0);
				tupleType = this.tupleTypeW0;
				register = Register.EAX;
			}
			instruction.Op0Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase + decoder.state.extraRegisterBaseEVEX) + this.baseReg;
			instruction.Op1Register = (int)decoder.state.vvvv + this.baseReg;
			if (decoder.state.mod == 3U)
			{
				instruction.Op2Register = (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + register;
				if ((decoder.state.zs.flags & StateFlags.b) != (StateFlags)0U)
				{
					instruction.InternalRoundingControl = decoder.state.vectorLength + 1U;
					return;
				}
			}
			else
			{
				if ((decoder.state.zs.flags & StateFlags.b & (StateFlags)decoder.invalidCheckMask) != (StateFlags)0U)
				{
					decoder.SetInvalidInstruction();
				}
				instruction.Op2Kind = OpKind.Memory;
				decoder.ReadOpMem(ref instruction, tupleType);
			}
		}

		private readonly Register baseReg;

		private readonly Code codeW0;

		private readonly Code codeW1;

		private readonly TupleType tupleTypeW0;

		private readonly TupleType tupleTypeW1;
	}
}
