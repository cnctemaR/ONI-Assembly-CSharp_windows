using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_MemBx : OpCodeHandler
	{
		public OpCodeHandler_MemBx(Code code)
		{
			this.code = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.InternalMemoryIndex = Register.AL;
			instruction.Op0Kind = OpKind.Memory;
			if (decoder.state.addressSize == OpSize.Size64)
			{
				instruction.InternalMemoryBase = Register.RBX;
				return;
			}
			if (decoder.state.addressSize == OpSize.Size32)
			{
				instruction.InternalMemoryBase = Register.EBX;
				return;
			}
			instruction.InternalMemoryBase = Register.BX;
		}

		private readonly Code code;
	}
}
