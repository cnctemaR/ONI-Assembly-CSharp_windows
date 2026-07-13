using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Reg_Xb : OpCodeHandler
	{
		public OpCodeHandler_Reg_Xb(Code code, Register reg)
		{
			this.code = code;
			this.reg = reg;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.Op0Register = this.reg;
			if (decoder.state.addressSize == OpSize.Size64)
			{
				instruction.Op1Kind = OpKind.MemorySegRSI;
				return;
			}
			if (decoder.state.addressSize == OpSize.Size32)
			{
				instruction.Op1Kind = OpKind.MemorySegESI;
				return;
			}
			instruction.Op1Kind = OpKind.MemorySegSI;
		}

		private readonly Code code;

		private readonly Register reg;
	}
}
