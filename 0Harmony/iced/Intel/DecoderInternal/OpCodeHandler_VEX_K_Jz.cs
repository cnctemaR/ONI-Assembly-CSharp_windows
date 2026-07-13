using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_VEX_K_Jz : OpCodeHandlerModRM
	{
		public OpCodeHandler_VEX_K_Jz(Code code)
		{
			this.code = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (decoder.invalidCheckMask != 0U && decoder.state.vvvv > 7U)
			{
				decoder.SetInvalidInstruction();
			}
			instruction.Op0Register = (int)(decoder.state.vvvv & 7U) + Register.K0;
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.Op1Kind = OpKind.NearBranch64;
			uint num = decoder.state.modrm | (decoder.ReadByte() << 8) | (decoder.ReadByte() << 16) | (decoder.ReadByte() << 24);
			instruction.NearBranch64 = (ulong)((long)num + (long)decoder.GetCurrentInstructionPointer64());
		}

		private readonly Code code;
	}
}
