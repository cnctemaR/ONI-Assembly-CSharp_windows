using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_VEX_VX_VSIB_HX : OpCodeHandlerModRM
	{
		public OpCodeHandler_VEX_VX_VSIB_HX(Register baseReg1, Register vsibIndex, Register baseReg3, Code code)
		{
			this.baseReg1 = baseReg1;
			this.vsibIndex = vsibIndex;
			this.baseReg3 = baseReg3;
			this.code = code;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.code);
			int num = (int)(decoder.state.reg + decoder.state.zs.extraRegisterBase);
			instruction.Op0Register = num + this.baseReg1;
			instruction.Op2Register = (int)decoder.state.vvvv + this.baseReg3;
			if (decoder.state.mod == 3U)
			{
				decoder.SetInvalidInstruction();
				return;
			}
			instruction.Op1Kind = OpKind.Memory;
			decoder.ReadOpMem_VSIB(ref instruction, this.vsibIndex, TupleType.N1);
			if (decoder.invalidCheckMask != 0U)
			{
				uint num2 = (uint)((instruction.MemoryIndex - Register.XMM0) % 32);
				if (num == (int)num2 || decoder.state.vvvv == num2 || num == (int)decoder.state.vvvv)
				{
					decoder.SetInvalidInstruction();
				}
			}
		}

		private readonly Register baseReg1;

		private readonly Register vsibIndex;

		private readonly Register baseReg3;

		private readonly Code code;
	}
}
