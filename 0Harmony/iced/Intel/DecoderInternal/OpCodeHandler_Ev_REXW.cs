using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Ev_REXW : OpCodeHandlerModRM
	{
		public OpCodeHandler_Ev_REXW(Code code32, Code code64, uint flags)
		{
			this.code32 = code32;
			this.code64 = code64;
			this.flags = flags;
			this.disallowReg = (((flags & 1U) != 0U) ? 0U : uint.MaxValue);
			this.disallowMem = (((flags & 2U) != 0U) ? 0U : uint.MaxValue);
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if ((decoder.state.zs.flags & StateFlags.W) != (StateFlags)0U)
			{
				instruction.InternalSetCodeNoCheck(this.code64);
			}
			else
			{
				instruction.InternalSetCodeNoCheck(this.code32);
			}
			if ((((this.flags & 4U) | (uint)(decoder.state.zs.flags & StateFlags.Has66)) & decoder.invalidCheckMask) == 32772U)
			{
				decoder.SetInvalidInstruction();
			}
			if (decoder.state.mod == 3U)
			{
				if ((decoder.state.zs.flags & StateFlags.W) != (StateFlags)0U)
				{
					instruction.Op0Register = (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + Register.RAX;
				}
				else
				{
					instruction.Op0Register = (int)(decoder.state.rm + decoder.state.zs.extraBaseRegisterBase) + Register.EAX;
				}
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
				instruction.Op0Kind = OpKind.Memory;
				decoder.ReadOpMem(ref instruction);
			}
		}

		private readonly Code code32;

		private readonly Code code64;

		private readonly uint flags;

		private readonly uint disallowReg;

		private readonly uint disallowMem;
	}
}
