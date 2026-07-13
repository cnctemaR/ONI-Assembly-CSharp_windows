using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Xb_Yb : OpCodeHandler
	{
		public OpCodeHandler_Xb_Yb(Code code)
		{
			this.code = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.code);
			if (decoder.state.addressSize == OpSize.Size64)
			{
				instruction.Op0Kind = OpKind.MemorySegRSI;
				instruction.Op1Kind = OpKind.MemoryESRDI;
				return;
			}
			if (decoder.state.addressSize == OpSize.Size32)
			{
				instruction.Op0Kind = OpKind.MemorySegESI;
				instruction.Op1Kind = OpKind.MemoryESEDI;
				return;
			}
			instruction.Op0Kind = OpKind.MemorySegSI;
			instruction.Op1Kind = OpKind.MemoryESDI;
		}

		private readonly Code code;
	}
}
