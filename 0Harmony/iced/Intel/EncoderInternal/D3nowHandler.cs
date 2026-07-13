using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.EncoderInternal
{
	internal sealed class D3nowHandler : OpCodeHandler
	{
		public D3nowHandler(EncFlags2 encFlags2, EncFlags3 encFlags3)
			: base((EncFlags2)(((ulong)encFlags2 & 18446744073709486080UL) | 15UL), encFlags3, false, null, D3nowHandler.operands)
		{
			this.immediate = OpCodeHandler.GetOpCode(encFlags2);
		}

		[NullableContext(1)]
		public override void Encode(Encoder encoder, in Instruction instruction)
		{
			encoder.WritePrefixes(in instruction, true);
			encoder.WriteByteInternal(15U);
			encoder.ImmSize = ImmSize.Size1OpCode;
			encoder.Immediate = this.immediate;
		}

		private static readonly Op[] operands = new Op[]
		{
			new OpModRM_reg(Register.MM0, Register.MM7),
			new OpModRM_rm(Register.MM0, Register.MM7)
		};

		private readonly uint immediate;
	}
}
