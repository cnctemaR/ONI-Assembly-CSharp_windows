using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Ov_Reg : OpCodeHandler
	{
		public OpCodeHandler_Ov_Reg(Code code16, Code code32, Code code64)
		{
			this.codes = new Code3(code16, code32, code64);
		}

		[NullableContext(1)]
		public unsafe override void Decode(Decoder decoder, ref Instruction instruction)
		{
			decoder.displIndex = decoder.state.zs.instructionLength;
			instruction.Op0Kind = OpKind.Memory;
			UIntPtr uintPtr = (UIntPtr)decoder.state.operandSize;
			instruction.InternalSetCodeNoCheck((Code)(*((ref this.codes.codes.FixedElementField) + (UIntPtr)((ulong)uintPtr * 2UL))));
			instruction.Op1Register = ((int)uintPtr << 4) + Register.AX;
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

		private readonly Code3 codes;
	}
}
