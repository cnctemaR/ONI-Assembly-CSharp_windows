using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Yb_Xb : OpCodeHandler
	{
		public OpCodeHandler_Yb_Xb(Code code)
		{
			this.code = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.code);
			if (decoder.state.addressSize == OpSize.Size64)
			{
				instruction.Op0Kind = OpKind.MemoryESRDI;
				instruction.Op1Kind = OpKind.MemorySegRSI;
				return;
			}
			if (decoder.state.addressSize == OpSize.Size32)
			{
				instruction.Op0Kind = OpKind.MemoryESEDI;
				instruction.Op1Kind = OpKind.MemorySegESI;
				return;
			}
			instruction.Op0Kind = OpKind.MemoryESDI;
			instruction.Op1Kind = OpKind.MemorySegSI;
		}

		private readonly Code code;
	}
}
