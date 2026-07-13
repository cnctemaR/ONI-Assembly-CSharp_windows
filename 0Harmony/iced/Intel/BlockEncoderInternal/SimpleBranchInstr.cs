using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.BlockEncoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class SimpleBranchInstr : Instr
	{
		public SimpleBranchInstr(BlockEncoder blockEncoder, Block block, in Instruction instruction)
			: base(block, instruction.IP)
		{
			this.bitness = (byte)blockEncoder.Bitness;
			this.instruction = instruction;
			this.instrKind = SimpleBranchInstr.InstrKind.Uninitialized;
			Instruction instruction2;
			if (!blockEncoder.FixBranches)
			{
				this.instrKind = SimpleBranchInstr.InstrKind.Unchanged;
				instruction2 = instruction;
				instruction2.NearBranch64 = 0UL;
				this.Size = blockEncoder.GetInstructionSize(in instruction2, 0UL);
				return;
			}
			instruction2 = instruction;
			instruction2.NearBranch64 = 0UL;
			this.shortInstructionSize = (byte)blockEncoder.GetInstructionSize(in instruction2, 0UL);
			this.nativeCode = SimpleBranchInstr.ToNativeBranchCode(instruction.Code, blockEncoder.Bitness);
			if (this.nativeCode == instruction.Code)
			{
				this.nativeInstructionSize = this.shortInstructionSize;
			}
			else
			{
				instruction2 = instruction;
				instruction2.InternalSetCodeNoCheck(this.nativeCode);
				instruction2.NearBranch64 = 0UL;
				this.nativeInstructionSize = (byte)blockEncoder.GetInstructionSize(in instruction2, 0UL);
			}
			int num = blockEncoder.Bitness;
			int num2;
			if (num != 16)
			{
				if (num != 32 && num != 64)
				{
					throw new InvalidOperationException();
				}
				num2 = (int)(this.nativeInstructionSize + 2 + 5);
			}
			else
			{
				num2 = (int)(this.nativeInstructionSize + 2 + 3);
			}
			this.nearInstructionSize = (byte)num2;
			if (blockEncoder.Bitness == 64)
			{
				this.longInstructionSize = (byte)((long)(this.nativeInstructionSize + 2) + 6L);
				this.Size = (uint)Math.Max(Math.Max(this.shortInstructionSize, this.nearInstructionSize), this.longInstructionSize);
				return;
			}
			this.Size = (uint)Math.Max(this.shortInstructionSize, this.nearInstructionSize);
		}

		private static Code ToNativeBranchCode(Code code, int bitness)
		{
			Code code2;
			Code code3;
			Code code4;
			switch (code)
			{
			case Code.Loopne_rel8_16_CX:
			case Code.Loopne_rel8_32_CX:
				code2 = Code.Loopne_rel8_16_CX;
				code3 = Code.Loopne_rel8_32_CX;
				code4 = Code.INVALID;
				break;
			case Code.Loopne_rel8_16_ECX:
			case Code.Loopne_rel8_32_ECX:
			case Code.Loopne_rel8_64_ECX:
				code2 = Code.Loopne_rel8_16_ECX;
				code3 = Code.Loopne_rel8_32_ECX;
				code4 = Code.Loopne_rel8_64_ECX;
				break;
			case Code.Loopne_rel8_16_RCX:
			case Code.Loopne_rel8_64_RCX:
				code2 = Code.Loopne_rel8_16_RCX;
				code3 = Code.INVALID;
				code4 = Code.Loopne_rel8_64_RCX;
				break;
			case Code.Loope_rel8_16_CX:
			case Code.Loope_rel8_32_CX:
				code2 = Code.Loope_rel8_16_CX;
				code3 = Code.Loope_rel8_32_CX;
				code4 = Code.INVALID;
				break;
			case Code.Loope_rel8_16_ECX:
			case Code.Loope_rel8_32_ECX:
			case Code.Loope_rel8_64_ECX:
				code2 = Code.Loope_rel8_16_ECX;
				code3 = Code.Loope_rel8_32_ECX;
				code4 = Code.Loope_rel8_64_ECX;
				break;
			case Code.Loope_rel8_16_RCX:
			case Code.Loope_rel8_64_RCX:
				code2 = Code.Loope_rel8_16_RCX;
				code3 = Code.INVALID;
				code4 = Code.Loope_rel8_64_RCX;
				break;
			case Code.Loop_rel8_16_CX:
			case Code.Loop_rel8_32_CX:
				code2 = Code.Loop_rel8_16_CX;
				code3 = Code.Loop_rel8_32_CX;
				code4 = Code.INVALID;
				break;
			case Code.Loop_rel8_16_ECX:
			case Code.Loop_rel8_32_ECX:
			case Code.Loop_rel8_64_ECX:
				code2 = Code.Loop_rel8_16_ECX;
				code3 = Code.Loop_rel8_32_ECX;
				code4 = Code.Loop_rel8_64_ECX;
				break;
			case Code.Loop_rel8_16_RCX:
			case Code.Loop_rel8_64_RCX:
				code2 = Code.Loop_rel8_16_RCX;
				code3 = Code.INVALID;
				code4 = Code.Loop_rel8_64_RCX;
				break;
			case Code.Jcxz_rel8_16:
			case Code.Jcxz_rel8_32:
				code2 = Code.Jcxz_rel8_16;
				code3 = Code.Jcxz_rel8_32;
				code4 = Code.INVALID;
				break;
			case Code.Jecxz_rel8_16:
			case Code.Jecxz_rel8_32:
			case Code.Jecxz_rel8_64:
				code2 = Code.Jecxz_rel8_16;
				code3 = Code.Jecxz_rel8_32;
				code4 = Code.Jecxz_rel8_64;
				break;
			case Code.Jrcxz_rel8_16:
			case Code.Jrcxz_rel8_64:
				code2 = Code.Jrcxz_rel8_16;
				code3 = Code.INVALID;
				code4 = Code.Jrcxz_rel8_64;
				break;
			default:
				throw new ArgumentOutOfRangeException("code");
			}
			Code code5;
			if (bitness != 16)
			{
				if (bitness != 32)
				{
					if (bitness != 64)
					{
						throw new ArgumentOutOfRangeException("bitness");
					}
					code5 = code4;
				}
				else
				{
					code5 = code3;
				}
			}
			else
			{
				code5 = code2;
			}
			return code5;
		}

		public override void Initialize(BlockEncoder blockEncoder)
		{
			this.targetInstr = blockEncoder.GetTarget(this.instruction.NearBranchTarget);
		}

		public override bool Optimize(ulong gained)
		{
			return this.TryOptimize(gained);
		}

		private bool TryOptimize(ulong gained)
		{
			if (this.instrKind == SimpleBranchInstr.InstrKind.Unchanged || this.instrKind == SimpleBranchInstr.InstrKind.Short)
			{
				this.Done = true;
				return false;
			}
			long address = (long)this.targetInstr.GetAddress();
			ulong num = this.IP + (ulong)this.shortInstructionSize;
			long num2 = address - (long)num;
			num2 = Instr.ConvertDiffToBitnessDiff((int)this.bitness, Instr.CorrectDiff(this.targetInstr.IsInBlock(this.Block), num2, gained));
			if (-128L <= num2 && num2 <= 127L)
			{
				if (this.pointerData != null)
				{
					this.pointerData.IsValid = false;
				}
				this.instrKind = SimpleBranchInstr.InstrKind.Short;
				this.Size = (uint)this.shortInstructionSize;
				this.Done = true;
				return true;
			}
			bool flag = this.bitness != 64 || this.targetInstr.IsInBlock(this.Block);
			if (!flag)
			{
				long address2 = (long)this.targetInstr.GetAddress();
				num = this.IP + (ulong)this.nearInstructionSize;
				num2 = address2 - (long)num;
				num2 = Instr.ConvertDiffToBitnessDiff((int)this.bitness, Instr.CorrectDiff(this.targetInstr.IsInBlock(this.Block), num2, gained));
				flag = -2147483648L <= num2 && num2 <= 2147483647L;
			}
			if (flag)
			{
				if (this.pointerData != null)
				{
					this.pointerData.IsValid = false;
				}
				if (num2 < -1920L || num2 > 1905L)
				{
					this.Done = true;
				}
				this.instrKind = SimpleBranchInstr.InstrKind.Near;
				this.Size = (uint)this.nearInstructionSize;
				return true;
			}
			if (this.pointerData == null)
			{
				this.pointerData = this.Block.AllocPointerLocation();
			}
			this.instrKind = SimpleBranchInstr.InstrKind.Long;
			return false;
		}

		[return: Nullable(2)]
		public override string TryEncode(Encoder encoder, out ConstantOffsets constantOffsets, out bool isOriginalInstruction)
		{
			switch (this.instrKind)
			{
			case SimpleBranchInstr.InstrKind.Unchanged:
			case SimpleBranchInstr.InstrKind.Short:
			{
				isOriginalInstruction = true;
				this.instruction.NearBranch64 = this.targetInstr.GetAddress();
				uint num;
				string text;
				if (!encoder.TryEncode(in this.instruction, this.IP, out num, out text))
				{
					constantOffsets = default(ConstantOffsets);
					return Instr.CreateErrorMessage(text, in this.instruction);
				}
				constantOffsets = encoder.GetConstantOffsets();
				return null;
			}
			case SimpleBranchInstr.InstrKind.Near:
			{
				isOriginalInstruction = false;
				constantOffsets = default(ConstantOffsets);
				Instruction instruction = this.instruction;
				instruction.InternalSetCodeNoCheck(this.nativeCode);
				instruction.NearBranch64 = this.IP + (ulong)this.nativeInstructionSize + 2UL;
				string text;
				uint num2;
				if (!encoder.TryEncode(in instruction, this.IP, out num2, out text))
				{
					return Instr.CreateErrorMessage(text, in this.instruction);
				}
				instruction = default(Instruction);
				instruction.NearBranch64 = this.IP + (ulong)this.nearInstructionSize;
				int num3 = encoder.Bitness;
				Code code;
				if (num3 != 16)
				{
					if (num3 != 32)
					{
						if (num3 != 64)
						{
							throw new InvalidOperationException();
						}
						instruction.InternalSetCodeNoCheck(Code.Jmp_rel8_64);
						code = Code.Jmp_rel32_64;
						instruction.Op0Kind = OpKind.NearBranch64;
					}
					else
					{
						instruction.InternalSetCodeNoCheck(Code.Jmp_rel8_32);
						code = Code.Jmp_rel32_32;
						instruction.Op0Kind = OpKind.NearBranch32;
					}
				}
				else
				{
					instruction.InternalSetCodeNoCheck(Code.Jmp_rel8_16);
					code = Code.Jmp_rel16;
					instruction.Op0Kind = OpKind.NearBranch16;
				}
				uint num4;
				if (!encoder.TryEncode(in instruction, this.IP + (ulong)num2, out num4, out text))
				{
					return Instr.CreateErrorMessage(text, in this.instruction);
				}
				num2 += num4;
				instruction.InternalSetCodeNoCheck(code);
				instruction.NearBranch64 = this.targetInstr.GetAddress();
				encoder.TryEncode(in instruction, this.IP + (ulong)num2, out num4, out text);
				if (text != null)
				{
					return Instr.CreateErrorMessage(text, in this.instruction);
				}
				return null;
			}
			case SimpleBranchInstr.InstrKind.Long:
			{
				isOriginalInstruction = false;
				constantOffsets = default(ConstantOffsets);
				this.pointerData.Data = this.targetInstr.GetAddress();
				Instruction instruction = this.instruction;
				instruction.InternalSetCodeNoCheck(this.nativeCode);
				instruction.NearBranch64 = this.IP + (ulong)this.nativeInstructionSize + 2UL;
				string text;
				uint num4;
				if (!encoder.TryEncode(in instruction, this.IP, out num4, out text))
				{
					return Instr.CreateErrorMessage(text, in this.instruction);
				}
				uint num2 = num4;
				instruction = default(Instruction);
				instruction.NearBranch64 = this.IP + (ulong)this.longInstructionSize;
				int num3 = encoder.Bitness;
				if (num3 != 16)
				{
					if (num3 != 32)
					{
						if (num3 != 64)
						{
							throw new InvalidOperationException();
						}
						instruction.InternalSetCodeNoCheck(Code.Jmp_rel8_64);
						instruction.Op0Kind = OpKind.NearBranch64;
					}
					else
					{
						instruction.InternalSetCodeNoCheck(Code.Jmp_rel8_32);
						instruction.Op0Kind = OpKind.NearBranch32;
					}
				}
				else
				{
					instruction.InternalSetCodeNoCheck(Code.Jmp_rel8_16);
					instruction.Op0Kind = OpKind.NearBranch16;
				}
				if (!encoder.TryEncode(in instruction, this.IP + (ulong)num2, out num4, out text))
				{
					return Instr.CreateErrorMessage(text, in this.instruction);
				}
				num2 += num4;
				uint num;
				text = base.EncodeBranchToPointerData(encoder, false, this.IP + (ulong)num2, this.pointerData, out num, this.Size - num2);
				if (text != null)
				{
					return Instr.CreateErrorMessage(text, in this.instruction);
				}
				return null;
			}
			}
			throw new InvalidOperationException();
		}

		private readonly byte bitness;

		private Instruction instruction;

		private TargetInstr targetInstr;

		private BlockData pointerData;

		private SimpleBranchInstr.InstrKind instrKind;

		private readonly byte shortInstructionSize;

		private readonly byte nearInstructionSize;

		private readonly byte longInstructionSize;

		private readonly byte nativeInstructionSize;

		private readonly Code nativeCode;

		private enum InstrKind : byte
		{
			Unchanged,
			Short,
			Near,
			Long,
			Uninitialized
		}
	}
}
