using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Xchg_Reg_rAX : OpCodeHandler
	{
		public OpCodeHandler_Xchg_Reg_rAX(int index)
		{
			this.index = index;
			this.codes = OpCodeHandler_Xchg_Reg_rAX.s_codes;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (this.index == 0 && decoder.state.zs.mandatoryPrefix == MandatoryPrefixByte.PF3 && (decoder.options & DecoderOptions.NoPause) == DecoderOptions.None)
			{
				decoder.ClearMandatoryPrefixF3(ref instruction);
				instruction.InternalSetCodeNoCheck(Code.Pause);
				return;
			}
			int operandSize = (int)decoder.state.operandSize;
			int num = this.index + (int)decoder.state.zs.extraBaseRegisterBase;
			instruction.InternalSetCodeNoCheck(this.codes[operandSize * 16 + num]);
			if (num != 0)
			{
				Register register = operandSize * 16 + num + Register.AX;
				instruction.Op0Register = register;
				instruction.Op1Register = operandSize * 16 + Register.AX;
			}
		}

		private readonly int index;

		private readonly Code[] codes;

		private static readonly Code[] s_codes = new Code[]
		{
			Code.Nopw,
			Code.Xchg_r16_AX,
			Code.Xchg_r16_AX,
			Code.Xchg_r16_AX,
			Code.Xchg_r16_AX,
			Code.Xchg_r16_AX,
			Code.Xchg_r16_AX,
			Code.Xchg_r16_AX,
			Code.Xchg_r16_AX,
			Code.Xchg_r16_AX,
			Code.Xchg_r16_AX,
			Code.Xchg_r16_AX,
			Code.Xchg_r16_AX,
			Code.Xchg_r16_AX,
			Code.Xchg_r16_AX,
			Code.Xchg_r16_AX,
			Code.Nopd,
			Code.Xchg_r32_EAX,
			Code.Xchg_r32_EAX,
			Code.Xchg_r32_EAX,
			Code.Xchg_r32_EAX,
			Code.Xchg_r32_EAX,
			Code.Xchg_r32_EAX,
			Code.Xchg_r32_EAX,
			Code.Xchg_r32_EAX,
			Code.Xchg_r32_EAX,
			Code.Xchg_r32_EAX,
			Code.Xchg_r32_EAX,
			Code.Xchg_r32_EAX,
			Code.Xchg_r32_EAX,
			Code.Xchg_r32_EAX,
			Code.Xchg_r32_EAX,
			Code.Nopq,
			Code.Xchg_r64_RAX,
			Code.Xchg_r64_RAX,
			Code.Xchg_r64_RAX,
			Code.Xchg_r64_RAX,
			Code.Xchg_r64_RAX,
			Code.Xchg_r64_RAX,
			Code.Xchg_r64_RAX,
			Code.Xchg_r64_RAX,
			Code.Xchg_r64_RAX,
			Code.Xchg_r64_RAX,
			Code.Xchg_r64_RAX,
			Code.Xchg_r64_RAX,
			Code.Xchg_r64_RAX,
			Code.Xchg_r64_RAX,
			Code.Xchg_r64_RAX
		};
	}
}
