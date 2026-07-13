using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_IbReg : OpCodeHandler
	{
		public OpCodeHandler_IbReg(Code code, Register reg)
		{
			this.code = code;
			this.reg = reg;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.Op1Register = this.reg;
			instruction.Op0Kind = OpKind.Immediate8;
			instruction.InternalImmediate8 = decoder.ReadByte();
		}

		private readonly Code code;

		private readonly Register reg;
	}
}
