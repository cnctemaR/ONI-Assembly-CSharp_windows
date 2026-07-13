using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_EVEX_VkW_er : OpCodeHandlerModRM
	{
		public OpCodeHandler_EVEX_VkW_er(Register baseReg, Code code, TupleType tupleType, bool onlySAE)
		{
			this.baseReg1 = baseReg;
			this.baseReg2 = baseReg;
			this.code = code;
			this.tupleType = tupleType;
			this.onlySAE = onlySAE;
			this.canBroadcast = true;
		}

		public OpCodeHandler_EVEX_VkW_er(Register baseReg1, Register baseReg2, Code code, TupleType tupleType, bool onlySAE)
		{
			this.baseReg1 = baseReg1;
			this.baseReg2 = baseReg2;
			this.code = code;
			this.tupleType = tupleType;
			this.onlySAE = onlySAE;
			this.canBroadcast = true;
		}

		public OpCodeHandler_EVEX_VkW_er(Register baseReg1, Register baseReg2, Code code, TupleType tupleType, bool onlySAE, bool canBroadcast)
		{
			this.baseReg1 = baseReg1;
			this.baseReg2 = baseReg2;
			this.code = code;
			this.tupleType = tupleType;
			this.onlySAE = onlySAE;
			this.canBroadcast = canBroadcast;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if ((decoder.state.vvvv_invalidCheck & decoder.invalidCheckMask) != 0U)
			{
				decoder.SetInvalidInstruction();
			}
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.Op0Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase + decoder.state.extraRegisterBaseEVEX) + this.baseReg1;
			if (decoder.state.mod == 3U)
			{
				instruction.Op1Register = (int)(decoder.state.rm + decoder.state.extraBaseRegisterBaseEVEX) + this.baseReg2;
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
				instruction.Op1Kind = OpKind.Memory;
				if ((decoder.state.zs.flags & StateFlags.b) != (StateFlags)0U)
				{
					if (this.canBroadcast)
					{
						instruction.InternalSetIsBroadcast();
					}
					else if (decoder.invalidCheckMask != 0U)
					{
						decoder.SetInvalidInstruction();
					}
				}
				decoder.ReadOpMem(ref instruction, this.tupleType);
			}
		}

		private readonly Register baseReg1;

		private readonly Register baseReg2;

		private readonly Code code;

		private readonly TupleType tupleType;

		private readonly bool onlySAE;

		private readonly bool canBroadcast;
	}
}
