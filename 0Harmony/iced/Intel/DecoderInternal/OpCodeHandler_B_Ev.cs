using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_B_Ev : OpCodeHandlerModRM
	{
		public OpCodeHandler_B_Ev(Code code32, Code code64, bool supportsRipRel)
		{
			this.code32 = code32;
			this.code64 = code64;
			this.ripRelMask = (supportsRipRel ? 0U : uint.MaxValue);
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (decoder.state.reg > 3U || (decoder.state.zs.extraRegisterBase & decoder.invalidCheckMask) != 0U)
			{
				decoder.SetInvalidInstruction();
			}
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
			instruction.Op0Register = (int)decoder.state.reg + Register.BND0;
			if (decoder.state.mod == 3U)
			{
				instruction.Op1Register = (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + register;
				return;
			}
			instruction.Op1Kind = OpKind.Memory;
			decoder.ReadOpMem_MPX(ref instruction);
			if ((this.ripRelMask & decoder.invalidCheckMask) != 0U && instruction.MemoryBase == Register.RIP)
			{
				decoder.SetInvalidInstruction();
			}
		}

		private readonly Code code32;

		private readonly Code code64;

		private readonly uint ripRelMask;
	}
}
