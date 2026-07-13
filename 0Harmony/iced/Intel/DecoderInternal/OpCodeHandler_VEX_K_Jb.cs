using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_VEX_K_Jb : OpCodeHandlerModRM
	{
		public OpCodeHandler_VEX_K_Jb(Code code)
		{
			this.code = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			decoder.state.zs.flags = decoder.state.zs.flags | StateFlags.BranchImm8;
			if (decoder.invalidCheckMask != 0U && decoder.state.vvvv > 7U)
			{
				decoder.SetInvalidInstruction();
			}
			instruction.Op0Register = (int)(decoder.state.vvvv & 7U) + Register.K0;
			instruction.InternalSetCodeNoCheck(this.code);
			instruction.Op1Kind = OpKind.NearBranch64;
			instruction.NearBranch64 = (ulong)((long)((sbyte)decoder.state.modrm) + (long)decoder.GetCurrentInstructionPointer64());
		}

		private readonly Code code;
	}
}
