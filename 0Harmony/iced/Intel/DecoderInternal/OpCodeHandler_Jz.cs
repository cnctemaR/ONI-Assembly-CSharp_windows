using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.DecoderInternal
{
	internal sealed class OpCodeHandler_Jz : OpCodeHandler
	{
		public OpCodeHandler_Jz(Code code16, Code code32, Code code64)
		{
			this.code16 = code16;
			this.code32 = code32;
			this.code64 = code64;
		}

		[NullableContext(1)]
		public override void Decode(Decoder decoder, ref Instruction instruction)
		{
			if (decoder.is64bMode)
			{
				if ((decoder.options & DecoderOptions.AMD) == DecoderOptions.None || decoder.state.operandSize != OpSize.Size16)
				{
					instruction.InternalSetCodeNoCheck(this.code64);
					instruction.Op0Kind = OpKind.NearBranch64;
					instruction.NearBranch64 = (ulong)((long)decoder.ReadUInt32() + (long)decoder.GetCurrentInstructionPointer64());
					return;
				}
				instruction.InternalSetCodeNoCheck(this.code16);
				instruction.Op0Kind = OpKind.NearBranch16;
				instruction.InternalNearBranch16 = (uint)((ushort)(decoder.ReadUInt16() + decoder.GetCurrentInstructionPointer32()));
				return;
			}
			else
			{
				if (decoder.state.operandSize != OpSize.Size16)
				{
					instruction.InternalSetCodeNoCheck(this.code32);
					instruction.Op0Kind = OpKind.NearBranch32;
					instruction.NearBranch32 = decoder.ReadUInt32() + decoder.GetCurrentInstructionPointer32();
					return;
				}
				instruction.InternalSetCodeNoCheck(this.code16);
				instruction.Op0Kind = OpKind.NearBranch16;
				instruction.InternalNearBranch16 = (uint)((ushort)(decoder.ReadUInt16() + decoder.GetCurrentInstructionPointer32()));
				return;
			}
		}

		private readonly Code code16;

		private readonly Code code32;

		private readonly Code code64;
	}
}
