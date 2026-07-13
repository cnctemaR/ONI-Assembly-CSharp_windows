using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Ob_Reg : OpCodeHandler
	{
		public OpCodeHandler_Ob_Reg(Code code, Register reg)
		{
			this.code = code;
			this.reg = reg;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			instruction.InternalSetCodeNoCheck(this.code);
			decoder.displIndex = decoder.state.zs.instructionLength;
			instruction.Op0Kind = OpKind.Memory;
			instruction.Op1Register = this.reg;
			if (decoder.state.addressSize == OpSize.Size64)
			{
				instruction.InternalSetMemoryDisplSize(4U);
				decoder.state.zs.flags = decoder.state.zs.flags | StateFlags.Addr64;
				instruction.MemoryDisplacement64 = decoder.ReadUInt64();
				return;
			}
			if (decoder.state.addressSize == OpSize.Size32)
			{
				instruction.InternalSetMemoryDisplSize(3U);
				instruction.MemoryDisplacement64 = (ulong)decoder.ReadUInt32();
				return;
			}
			instruction.InternalSetMemoryDisplSize(2U);
			instruction.MemoryDisplacement64 = (ulong)decoder.ReadUInt16();
		}

		private readonly Code code;

		private readonly Register reg;
	}
}
