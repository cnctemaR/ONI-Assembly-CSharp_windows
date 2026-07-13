using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.BlockEncoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class IpRelMemOpInstr : Instr
	{
		public IpRelMemOpInstr(BlockEncoder blockEncoder, Block block, in Instruction instruction)
			: base(block, instruction.IP)
		{
			this.instruction = instruction;
			this.instrKind = IpRelMemOpInstr.InstrKind.Uninitialized;
			Instruction instruction2 = instruction;
			instruction2.MemoryBase = Register.RIP;
			instruction2.MemoryDisplacement64 = 0UL;
			this.ripInstructionSize = (byte)blockEncoder.GetInstructionSize(in instruction2, instruction2.IPRelativeMemoryAddress);
			instruction2.MemoryBase = Register.EIP;
			this.eipInstructionSize = (byte)blockEncoder.GetInstructionSize(in instruction2, instruction2.IPRelativeMemoryAddress);
			this.Size = (uint)this.eipInstructionSize;
		}

		public override void Initialize(BlockEncoder blockEncoder)
		{
			this.targetInstr = blockEncoder.GetTarget(this.instruction.IPRelativeMemoryAddress);
		}

		public override bool Optimize(ulong gained)
		{
			return this.TryOptimize(gained);
		}

		private bool TryOptimize(ulong gained)
		{
			if (this.instrKind == IpRelMemOpInstr.InstrKind.Unchanged || this.instrKind == IpRelMemOpInstr.InstrKind.Rip || this.instrKind == IpRelMemOpInstr.InstrKind.Eip)
			{
				this.Done = true;
				return false;
			}
			bool flag = this.targetInstr.IsInBlock(this.Block);
			ulong address = this.targetInstr.GetAddress();
			if (!flag)
			{
				ulong num = this.IP + (ulong)this.ripInstructionSize;
				long num2 = (long)(address - num);
				num2 = Instr.CorrectDiff(this.targetInstr.IsInBlock(this.Block), num2, gained);
				flag = -2147483648L <= num2 && num2 <= 2147483647L;
			}
			if (flag)
			{
				this.Size = (uint)this.ripInstructionSize;
				this.instrKind = IpRelMemOpInstr.InstrKind.Rip;
				this.Done = true;
				return true;
			}
			if (address <= (ulong)(-1))
			{
				this.Size = (uint)this.eipInstructionSize;
				this.instrKind = IpRelMemOpInstr.InstrKind.Eip;
				this.Done = true;
				return true;
			}
			this.instrKind = IpRelMemOpInstr.InstrKind.Long;
			return false;
		}

		[return: Nullable(2)]
		public override string TryEncode(Encoder encoder, out ConstantOffsets constantOffsets, out bool isOriginalInstruction)
		{
			switch (this.instrKind)
			{
			case IpRelMemOpInstr.InstrKind.Unchanged:
			case IpRelMemOpInstr.InstrKind.Rip:
			case IpRelMemOpInstr.InstrKind.Eip:
			{
				isOriginalInstruction = true;
				if (this.instrKind == IpRelMemOpInstr.InstrKind.Rip)
				{
					this.instruction.MemoryBase = Register.RIP;
				}
				else if (this.instrKind == IpRelMemOpInstr.InstrKind.Eip)
				{
					this.instruction.MemoryBase = Register.EIP;
				}
				ulong address = this.targetInstr.GetAddress();
				this.instruction.MemoryDisplacement64 = address;
				uint num;
				string text;
				encoder.TryEncode(in this.instruction, this.IP, out num, out text);
				if (this.instruction.IPRelativeMemoryAddress != ((this.instruction.MemoryBase == Register.EIP) ? ((ulong)((uint)address)) : address))
				{
					text = "Invalid IP relative address";
				}
				if (text != null)
				{
					constantOffsets = default(ConstantOffsets);
					return Instr.CreateErrorMessage(text, in this.instruction);
				}
				constantOffsets = encoder.GetConstantOffsets();
				return null;
			}
			case IpRelMemOpInstr.InstrKind.Long:
				isOriginalInstruction = false;
				constantOffsets = default(ConstantOffsets);
				return "IP relative memory operand is too far away and isn't currently supported. Try to allocate memory close to the original instruction (+/-2GB).";
			}
			throw new InvalidOperationException();
		}

		private Instruction instruction;

		private IpRelMemOpInstr.InstrKind instrKind;

		private readonly byte eipInstructionSize;

		private readonly byte ripInstructionSize;

		private TargetInstr targetInstr;

		private enum InstrKind : byte
		{
			Unchanged,
			Rip,
			Eip,
			Long,
			Uninitialized
		}
	}
}
