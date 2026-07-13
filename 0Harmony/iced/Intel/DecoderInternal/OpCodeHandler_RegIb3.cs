using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_RegIb3 : OpCodeHandler
	{
		public OpCodeHandler_RegIb3(int index)
		{
			this.index = index;
			this.withRexPrefix = OpCodeHandler_RegIb3.s_withRexPrefix;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			Register register;
			if ((decoder.state.zs.flags & StateFlags.HasRex) != (StateFlags)0U)
			{
				register = this.withRexPrefix[this.index + (int)decoder.state.zs.extraBaseRegisterBase];
			}
			else
			{
				register = this.index + Register.AL;
			}
			instruction.InternalSetCodeNoCheck(Code.Mov_r8_imm8);
			instruction.Op0Register = register;
			instruction.Op1Kind = OpKind.Immediate8;
			instruction.InternalImmediate8 = decoder.ReadByte();
		}

		private readonly int index;

		private readonly Register[] withRexPrefix;

		private static readonly Register[] s_withRexPrefix = new Register[]
		{
			Register.AL,
			Register.CL,
			Register.DL,
			Register.BL,
			Register.SPL,
			Register.BPL,
			Register.SIL,
			Register.DIL,
			Register.R8L,
			Register.R9L,
			Register.R10L,
			Register.R11L,
			Register.R12L,
			Register.R13L,
			Register.R14L,
			Register.R15L
		};
	}
}
