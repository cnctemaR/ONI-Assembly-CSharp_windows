using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Reg_Yb : OpCodeHandler
	{
		public OpCodeHandler_Reg_Yb(Code code, Register reg)
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
				instruction.Op1Kind = OpKind.MemoryESRDI;
				return;
			}
			if (decoder.state.addressSize == OpSize.Size32)
			{
				instruction.Op1Kind = OpKind.MemoryESEDI;
				return;
			}
			instruction.Op1Kind = OpKind.MemoryESDI;
		}

		private readonly Code code;

		private readonly Register reg;
	}
}
