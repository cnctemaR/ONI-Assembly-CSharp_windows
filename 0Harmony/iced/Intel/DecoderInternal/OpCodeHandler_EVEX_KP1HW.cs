using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_EVEX_KP1HW : OpCodeHandlerModRM
	{
		public OpCodeHandler_EVEX_KP1HW(Register baseReg, Code code, TupleType tupleType)
		{
			this.baseReg = baseReg;
			this.code = code;
			this.tupleType = tupleType;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.Op0Register = (int)decoder.state.reg + Register.K0;
			instruction.Op1Register = (int)decoder.state.vvvv + this.baseReg;
			if ((((decoder.state.zs.flags & StateFlags.z) | (StateFlags)decoder.state.aaa | (StateFlags)decoder.state.zs.extraRegisterBase | (StateFlags)decoder.state.extraRegisterBaseEVEX) & (StateFlags)decoder.invalidCheckMask) != (StateFlags)0U)
			{
				decoder.SetInvalidInstruction();
			}
			if (decoder.state.mod == 3U)
			{
				instruction.Op2Register = (int)(decoder.state.rm + decoder.state.extraBaseRegisterBaseEVEX) + this.baseReg;
				if ((decoder.state.zs.flags & StateFlags.b & (StateFlags)decoder.invalidCheckMask) != (StateFlags)0U)
				{
					decoder.SetInvalidInstruction();
					return;
				}
			}
			else
			{
				instruction.Op2Kind = OpKind.Memory;
				if ((decoder.state.zs.flags & StateFlags.b) != (StateFlags)0U)
				{
					instruction.InternalSetIsBroadcast();
				}
				decoder.ReadOpMem(ref instruction, this.tupleType);
			}
		}

		private readonly Register baseReg;

		private readonly Code code;

		private readonly TupleType tupleType;
	}
}
