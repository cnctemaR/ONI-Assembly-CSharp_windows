using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_M : OpCodeHandlerModRM
	{
		public OpCodeHandler_M(Code codeW0, Code codeW1)
		{
			this.codeW0 = codeW0;
			this.codeW1 = codeW1;
		}

		public OpCodeHandler_M(Code codeW0)
		{
			this.codeW0 = codeW0;
			this.codeW1 = codeW0;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if ((decoder.state.zs.flags & StateFlags.W) != (StateFlags)0U)
			{
				instruction.InternalSetCodeNoCheck(this.codeW1);
			}
			else
			{
				instruction.InternalSetCodeNoCheck(this.codeW0);
			}
			if (decoder.state.mod == 3U)
			{
				decoder.SetInvalidInstruction();
				return;
			}
			instruction.Op0Kind = OpKind.Memory;
			decoder.ReadOpMem(ref instruction);
		}

		private readonly Code codeW0;

		private readonly Code codeW1;
	}
}
