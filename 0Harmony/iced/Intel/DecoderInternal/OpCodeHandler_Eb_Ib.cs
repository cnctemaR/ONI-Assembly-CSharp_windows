using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Eb_Ib : OpCodeHandlerModRM
	{
		public OpCodeHandler_Eb_Ib(Code code)
		{
			this.code = code;
		}

		public OpCodeHandler_Eb_Ib(Code code, HandlerFlags flags)
		{
			this.code = code;
			this.flags = flags;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.code);
			if (decoder.state.mod < 3U)
			{
				decoder.state.zs.flags = decoder.state.zs.flags | (StateFlags)((this.flags & HandlerFlags.Lock) << 10);
				instruction.Op0Kind = OpKind.Memory;
				decoder.ReadOpMem(ref instruction);
			}
			else
			{
				uint num = decoder.state.rm + decoder.state.zs.extraBaseRegisterBase;
				if ((decoder.state.zs.flags & StateFlags.HasRex) != (StateFlags)0U && num >= 4U)
				{
					num += 4U;
				}
				instruction.Op0Register = (int)num + Register.AL;
			}
			instruction.Op1Kind = OpKind.Immediate8;
			instruction.InternalImmediate8 = decoder.ReadByte();
		}

		private readonly Code code;

		private readonly HandlerFlags flags;
	}
}
