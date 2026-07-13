using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel.BlockEncoderInternal
{
	[NullableContext(1)]
	[Nullable(0)]
	internal readonly struct TargetInstr
	{
		public TargetInstr(Instr instruction)
		{
			this.instruction = instruction;
			this.address = 0UL;
		}

		public TargetInstr(ulong address)
		{
			this.instruction = null;
			this.address = address;
		}

		public bool IsInBlock(Block block)
		{
			Instr instr = this.instruction;
			return ((instr != null) ? instr.Block : null) == block;
		}

		public ulong GetAddress()
		{
			Instr instr = this.instruction;
			if (instr == null)
			{
				return this.address;
			}
			return instr.IP;
		}

		private readonly Instr instruction;

		private readonly ulong address;
	}
}
