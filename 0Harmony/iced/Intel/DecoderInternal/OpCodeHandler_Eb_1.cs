using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Eb_1 : OpCodeHandlerModRM
	{
		public OpCodeHandler_Eb_1(Code code)
		{
			this.code = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.Op1Kind = OpKind.Immediate8;
			instruction.InternalImmediate8 = 1U;
			decoder.state.zs.flags = decoder.state.zs.flags | StateFlags.NoImm;
			if (decoder.state.mod == 3U)
			{
				uint num = decoder.state.rm + decoder.state.zs.extraBaseRegisterBase;
				if ((decoder.state.zs.flags & StateFlags.HasRex) != (StateFlags)0U && num >= 4U)
				{
					num += 4U;
				}
				instruction.Op0Register = (int)num + Register.AL;
				return;
			}
			instruction.Op0Kind = OpKind.Memory;
			decoder.ReadOpMem(ref instruction);
		}

		private readonly Code code;
	}
}
