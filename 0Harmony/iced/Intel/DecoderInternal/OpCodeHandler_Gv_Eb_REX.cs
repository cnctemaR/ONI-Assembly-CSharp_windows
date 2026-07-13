using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Gv_Eb_REX : OpCodeHandlerModRM
	{
		public OpCodeHandler_Gv_Eb_REX(Code code32, Code code64)
		{
			this.code32 = code32;
			this.code64 = code64;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if ((decoder.state.zs.flags & StateFlags.W) != (StateFlags)0U)
			{
				instruction.InternalSetCodeNoCheck(this.code64);
				instruction.Op0Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase) + Register.RAX;
			}
			else
			{
				instruction.InternalSetCodeNoCheck(this.code32);
				instruction.Op0Register = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase) + Register.EAX;
			}
			if (decoder.state.mod == 3U)
			{
				uint num = decoder.state.rm + decoder.state.zs.extraBaseRegisterBase;
				if ((decoder.state.zs.flags & StateFlags.HasRex) != (StateFlags)0U && num >= 4U)
				{
					num += 4U;
				}
				instruction.Op1Register = (int)num + Register.AL;
				return;
			}
			instruction.Op1Kind = OpKind.Memory;
			decoder.ReadOpMem(ref instruction);
		}

		private readonly Code code32;

		private readonly Code code64;
	}
}
