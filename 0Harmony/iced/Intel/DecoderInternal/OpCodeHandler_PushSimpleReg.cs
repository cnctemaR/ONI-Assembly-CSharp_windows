using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_PushSimpleReg : OpCodeHandler
	{
		public OpCodeHandler_PushSimpleReg(int index, Code code16, Code code32, Code code64)
		{
			this.index = index;
			this.code16 = code16;
			this.code32 = code32;
			this.code64 = code64;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (decoder.is64bMode)
			{
				if (decoder.state.operandSize != OpSize.Size16)
				{
					instruction.InternalSetCodeNoCheck(this.code64);
					instruction.Op0Register = this.index + (int)decoder.state.zs.extraBaseRegisterBase + Register.RAX;
					return;
				}
				instruction.InternalSetCodeNoCheck(this.code16);
				instruction.Op0Register = this.index + (int)decoder.state.zs.extraBaseRegisterBase + Register.AX;
				return;
			}
			else
			{
				if (decoder.state.operandSize == OpSize.Size32)
				{
					instruction.InternalSetCodeNoCheck(this.code32);
					instruction.Op0Register = this.index + (int)decoder.state.zs.extraBaseRegisterBase + Register.EAX;
					return;
				}
				instruction.InternalSetCodeNoCheck(this.code16);
				instruction.Op0Register = this.index + (int)decoder.state.zs.extraBaseRegisterBase + Register.AX;
				return;
			}
		}

		private readonly int index;

		private readonly Code code16;

		private readonly Code code32;

		private readonly Code code64;
	}
}
