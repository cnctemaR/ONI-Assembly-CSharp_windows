using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.BlockEncoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class CallInstr : Instr
	{
		public CallInstr(BlockEncoder blockEncoder, Block block, in Instruction instruction)
			: base(block, instruction.IP)
		{
			this.bitness = (byte)blockEncoder.Bitness;
			this.instruction = instruction;
			Instruction instruction2 = instruction;
			instruction2.NearBranch64 = 0UL;
			this.origInstructionSize = (byte)blockEncoder.GetInstructionSize(in instruction2, 0UL);
			if (!blockEncoder.FixBranches)
			{
				this.Size = (uint)this.origInstructionSize;
				this.useOrigInstruction = true;
				return;
			}
			if (blockEncoder.Bitness == 64)
			{
				this.Size = Math.Max((uint)this.origInstructionSize, 6U);
				return;
			}
			this.Size = (uint)this.origInstructionSize;
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
			if (this.Done || this.useOrigInstruction)
			{
				this.Done = true;
				return false;
			}
			bool flag = this.bitness != 64 || this.targetInstr.IsInBlock(this.Block);
			if (!flag)
			{
				long address = (long)this.targetInstr.GetAddress();
				ulong num = this.IP + (ulong)this.origInstructionSize;
				long num2 = address - (long)num;
				num2 = Instr.CorrectDiff(this.targetInstr.IsInBlock(this.Block), num2, gained);
				flag = -2147483648L <= num2 && num2 <= 2147483647L;
			}
			if (flag)
			{
				if (this.pointerData != null)
				{
					this.pointerData.IsValid = false;
				}
				this.Size = (uint)this.origInstructionSize;
				this.useOrigInstruction = true;
				this.Done = true;
				return true;
			}
			if (this.pointerData == null)
			{
				this.pointerData = this.Block.AllocPointerLocation();
			}
			return false;
		}

		[return: Nullable(2)]
		public override string TryEncode(Encoder encoder, out ConstantOffsets constantOffsets, out bool isOriginalInstruction)
		{
			if (this.useOrigInstruction)
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
			else
			{
				isOriginalInstruction = false;
				constantOffsets = default(ConstantOffsets);
				this.pointerData.Data = this.targetInstr.GetAddress();
				uint num;
				string text2 = base.EncodeBranchToPointerData(encoder, true, this.IP, this.pointerData, out num, this.Size);
				if (text2 != null)
				{
					return Instr.CreateErrorMessage(text2, in this.instruction);
				}
				return null;
			}
		}

		private readonly byte bitness;

		private Instruction instruction;

		private TargetInstr targetInstr;

		private readonly byte origInstructionSize;

		private BlockData pointerData;

		private bool useOrigInstruction;
	}
}
