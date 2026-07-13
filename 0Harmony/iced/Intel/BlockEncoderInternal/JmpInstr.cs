using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.BlockEncoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class JmpInstr : Instr
	{
		public JmpInstr(BlockEncoder blockEncoder, Block block, in Instruction instruction)
			: base(block, instruction.IP)
		{
			this.bitness = (byte)blockEncoder.Bitness;
			this.instruction = instruction;
			this.instrKind = JmpInstr.InstrKind.Uninitialized;
			Instruction instruction2;
			if (!blockEncoder.FixBranches)
			{
				this.instrKind = JmpInstr.InstrKind.Unchanged;
				instruction2 = instruction;
				instruction2.NearBranch64 = 0UL;
				this.Size = blockEncoder.GetInstructionSize(in instruction2, 0UL);
				return;
			}
			instruction2 = instruction;
			instruction2.InternalSetCodeNoCheck(instruction.Code.ToShortBranch());
			instruction2.NearBranch64 = 0UL;
			this.shortInstructionSize = (byte)blockEncoder.GetInstructionSize(in instruction2, 0UL);
			instruction2 = instruction;
			instruction2.InternalSetCodeNoCheck(instruction.Code.ToNearBranch());
			instruction2.NearBranch64 = 0UL;
			this.nearInstructionSize = (byte)blockEncoder.GetInstructionSize(in instruction2, 0UL);
			if (blockEncoder.Bitness == 64)
			{
				this.Size = Math.Max((uint)this.nearInstructionSize, 6U);
				return;
			}
			this.Size = (uint)this.nearInstructionSize;
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
			if (this.instrKind == JmpInstr.InstrKind.Unchanged || this.instrKind == JmpInstr.InstrKind.Short)
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
				this.instrKind = JmpInstr.InstrKind.Short;
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
				this.instrKind = JmpInstr.InstrKind.Near;
				this.Size = (uint)this.nearInstructionSize;
				return true;
			}
			if (this.pointerData == null)
			{
				this.pointerData = this.Block.AllocPointerLocation();
			}
			this.instrKind = JmpInstr.InstrKind.Long;
			return false;
		}

		[return: Nullable(2)]
		public override string TryEncode(Encoder encoder, out ConstantOffsets constantOffsets, out bool isOriginalInstruction)
		{
			switch (this.instrKind)
			{
			case JmpInstr.InstrKind.Unchanged:
			case JmpInstr.InstrKind.Short:
			case JmpInstr.InstrKind.Near:
			{
				isOriginalInstruction = true;
				if (this.instrKind != JmpInstr.InstrKind.Unchanged)
				{
					if (this.instrKind == JmpInstr.InstrKind.Short)
					{
						this.instruction.InternalSetCodeNoCheck(this.instruction.Code.ToShortBranch());
					}
					else
					{
						this.instruction.InternalSetCodeNoCheck(this.instruction.Code.ToNearBranch());
					}
				}
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
			case JmpInstr.InstrKind.Long:
			{
				isOriginalInstruction = false;
				constantOffsets = default(ConstantOffsets);
				this.pointerData.Data = this.targetInstr.GetAddress();
				uint num;
				string text = base.EncodeBranchToPointerData(encoder, false, this.IP, this.pointerData, out num, this.Size);
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

		private JmpInstr.InstrKind instrKind;

		private readonly byte shortInstructionSize;

		private readonly byte nearInstructionSize;

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
