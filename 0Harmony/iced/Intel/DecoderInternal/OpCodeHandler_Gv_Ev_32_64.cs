using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Gv_Ev_32_64 : OpCodeHandlerModRM
	{
		public OpCodeHandler_Gv_Ev_32_64(Code code32, Code code64, bool allowReg, bool allowMem)
		{
			this.code32 = code32;
			this.code64 = code64;
			this.disallowMem = (allowMem ? 0U : uint.MaxValue);
			this.disallowReg = (allowReg ? 0U : uint.MaxValue);
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			Register register;
			if (decoder.is64bMode)
			{
				instruction.InternalSetCodeNoCheck(this.code64);
				register = Register.RAX;
			}
			else
			{
				instruction.InternalSetCodeNoCheck(this.code32);
				register = Register.EAX;
			}
			instruction.Op0Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase) + register;
			if (decoder.state.mod == 3U)
			{
				instruction.Op1Register = (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + register;
				if ((this.disallowReg & decoder.invalidCheckMask) != 0U)
				{
					decoder.SetInvalidInstruction();
					return;
				}
			}
			else
			{
				if ((this.disallowMem & decoder.invalidCheckMask) != 0U)
				{
					decoder.SetInvalidInstruction();
				}
				instruction.Op1Kind = OpKind.Memory;
				decoder.ReadOpMem(ref instruction);
			}
		}

		private readonly Code code32;

		private readonly Code code64;

		private readonly uint disallowReg;

		private readonly uint disallowMem;
	}
}
