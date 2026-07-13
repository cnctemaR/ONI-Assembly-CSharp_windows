using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.BlockEncoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class XbeginInstr : Instr
	{
		public XbeginInstr(BlockEncoder blockEncoder, Block block, in Instruction instruction)
			: base(block, instruction.IP)
		{
			this.instruction = instruction;
			this.instrKind = XbeginInstr.InstrKind.Uninitialized;
			Instruction instruction2;
			if (!blockEncoder.FixBranches)
			{
				this.instrKind = XbeginInstr.InstrKind.Unchanged;
				instruction2 = instruction;
				instruction2.NearBranch64 = 0UL;
				this.Size = blockEncoder.GetInstructionSize(in instruction2, 0UL);
				return;
			}
			instruction2 = instruction;
			instruction2.InternalSetCodeNoCheck(Code.Xbegin_rel16);
			instruction2.NearBranch64 = 0UL;
			this.shortInstructionSize = (byte)blockEncoder.GetInstructionSize(in instruction2, 0UL);
			instruction2 = instruction;
			instruction2.InternalSetCodeNoCheck(Code.Xbegin_rel32);
			instruction2.NearBranch64 = 0UL;
			this.nearInstructionSize = (byte)blockEncoder.GetInstructionSize(in instruction2, 0UL);
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
			if (this.instrKind == XbeginInstr.InstrKind.Unchanged || this.instrKind == XbeginInstr.InstrKind.Rel16)
			{
				this.Done = true;
				return false;
			}
			long address = (long)this.targetInstr.GetAddress();
			ulong num = this.IP + (ulong)this.shortInstructionSize;
			long num2 = address - (long)num;
			num2 = Instr.CorrectDiff(this.targetInstr.IsInBlock(this.Block), num2, gained);
			if (-32768L <= num2 && num2 <= 32767L)
			{
				this.instrKind = XbeginInstr.InstrKind.Rel16;
				this.Size = (uint)this.shortInstructionSize;
				return true;
			}
			this.instrKind = XbeginInstr.InstrKind.Rel32;
			this.Size = (uint)this.nearInstructionSize;
			return false;
		}

		[return: Nullable(2)]
		public override string TryEncode(Encoder encoder, out ConstantOffsets constantOffsets, out bool isOriginalInstruction)
		{
			XbeginInstr.InstrKind instrKind = this.instrKind;
			if (instrKind > XbeginInstr.InstrKind.Rel32)
			{
				if (instrKind != XbeginInstr.InstrKind.Uninitialized)
				{
				}
				throw new InvalidOperationException();
			}
			isOriginalInstruction = true;
			if (this.instrKind != XbeginInstr.InstrKind.Unchanged)
			{
				if (this.instrKind == XbeginInstr.InstrKind.Rel16)
				{
					this.instruction.InternalSetCodeNoCheck(Code.Xbegin_rel16);
				}
				else
				{
					this.instruction.InternalSetCodeNoCheck(Code.Xbegin_rel32);
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

		private Instruction instruction;

		private TargetInstr targetInstr;

		private XbeginInstr.InstrKind instrKind;

		private readonly byte shortInstructionSize;

		private readonly byte nearInstructionSize;

		private enum InstrKind : byte
		{
			Unchanged,
			Rel16,
			Rel32,
			Uninitialized
		}
	}
}
