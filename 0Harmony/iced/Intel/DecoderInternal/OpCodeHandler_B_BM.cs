using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_B_BM : OpCodeHandlerModRM
	{
		public OpCodeHandler_B_BM(Code code32, Code code64)
		{
			this.code32 = code32;
			this.code64 = code64;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (decoder.state.reg > 3U || (decoder.state.zs.extraRegisterBase & decoder.invalidCheckMask) != 0U)
			{
				decoder.SetInvalidInstruction();
			}
			if (decoder.is64bMode)
			{
				instruction.InternalSetCodeNoCheck(this.code64);
			}
			else
			{
				instruction.InternalSetCodeNoCheck(this.code32);
			}
			instruction.Op0Register = (int)decoder.state.reg + Register.BND0;
			if (decoder.state.mod == 3U)
			{
				instruction.Op1Register = (int)decoder.state.rm + Register.BND0;
				if (decoder.state.rm > 3U || (decoder.state.zs.extraBaseRegisterBase & decoder.invalidCheckMask) != 0U)
				{
					decoder.SetInvalidInstruction();
					return;
				}
			}
			else
			{
				instruction.Op1Kind = OpKind.Memory;
				decoder.ReadOpMem_MPX(ref instruction);
			}
		}

		private readonly Code code32;

		private readonly Code code64;
	}
}
