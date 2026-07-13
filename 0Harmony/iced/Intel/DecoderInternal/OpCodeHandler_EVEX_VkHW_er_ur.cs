using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_EVEX_VkHW_er_ur : OpCodeHandlerModRM
	{
		public OpCodeHandler_EVEX_VkHW_er_ur(Register baseReg, Code code, TupleType tupleType, bool canBroadcast)
		{
			this.baseReg = baseReg;
			this.code = code;
			this.tupleType = tupleType;
			this.canBroadcast = canBroadcast;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.code);
			int num = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase + decoder.state.extraRegisterBaseEVEX);
			instruction.Op0Register = num + this.baseReg;
			instruction.Op1Register = (int)decoder.state.vvvv + this.baseReg;
			if (decoder.state.mod == 3U)
			{
				int num2 = (int)(decoder.state.rm + decoder.state.extraBaseRegisterBaseEVEX);
				instruction.Op2Register = num2 + this.baseReg;
				if (decoder.invalidCheckMask != 0U && (num == (int)decoder.state.vvvv || num == num2))
				{
					decoder.SetInvalidInstruction();
				}
				if ((decoder.state.zs.flags & StateFlags.b) != (StateFlags)0U)
				{
					instruction.InternalRoundingControl = decoder.state.vectorLength + 1U;
					return;
				}
			}
			else
			{
				if (decoder.invalidCheckMask != 0U && num == (int)decoder.state.vvvv)
				{
					decoder.SetInvalidInstruction();
				}
				instruction.Op2Kind = OpKind.Memory;
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

		private readonly Register baseReg;

		private readonly Code code;

		private readonly TupleType tupleType;

		private readonly bool canBroadcast;
	}
}
