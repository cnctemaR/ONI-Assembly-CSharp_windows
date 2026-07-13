using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_EVEX_Gv_W_er : OpCodeHandlerModRM
	{
		public OpCodeHandler_EVEX_Gv_W_er(Register baseReg, Code codeW0, Code codeW1, TupleType tupleType, bool onlySAE)
		{
			this.baseReg = baseReg;
			this.codeW0 = codeW0;
			this.codeW1 = codeW1;
			this.tupleType = tupleType;
			this.onlySAE = onlySAE;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if ((((decoder.state.zs.flags & StateFlags.z) | (StateFlags)decoder.state.vvvv_invalidCheck | (StateFlags)decoder.state.aaa | (StateFlags)decoder.state.extraRegisterBaseEVEX) & (StateFlags)decoder.invalidCheckMask) != (StateFlags)0U)
			{
				decoder.SetInvalidInstruction();
			}
			if ((decoder.state.zs.flags & (StateFlags)decoder.is64bMode_and_W) != (StateFlags)0U)
			{
				instruction.InternalSetCodeNoCheck(this.codeW1);
				instruction.Op0Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase) + Register.RAX;
			}
			else
			{
				instruction.InternalSetCodeNoCheck(this.codeW0);
				instruction.Op0Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase) + Register.EAX;
			}
			if (decoder.state.mod == 3U)
			{
				instruction.Op1Register = (int)(decoder.state.rm + decoder.state.extraBaseRegisterBaseEVEX) + this.baseReg;
				if ((decoder.state.zs.flags & StateFlags.b) != (StateFlags)0U)
				{
					if (this.onlySAE)
					{
						instruction.InternalSetSuppressAllExceptions();
						return;
					}
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
				instruction.Op1Kind = OpKind.Memory;
				decoder.ReadOpMem(ref instruction, this.tupleType);
			}
		}

		private readonly Register baseReg;

		private readonly Code codeW0;

		private readonly Code codeW1;

		private readonly TupleType tupleType;

		private readonly bool onlySAE;
	}
}
