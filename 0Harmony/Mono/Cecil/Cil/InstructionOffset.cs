using System;

namespace Mono.Cecil.Cil
{
	internal struct InstructionOffset
	{
		public int Offset
		{
			get
			{
				if (this.instruction != null)
				{
					return this.instruction.Offset;
				}
				if (this.offset != null)
				{
					return this.offset.Value;
				}
				throw new NotSupportedException();
			}
		}

		public bool IsEndOfMethod
		{
			get
			{
				return this.instruction == null && this.offset == null;
			}
		}

		internal bool IsResolved
		{
			get
			{
				return this.instruction != null || this.offset == null;
			}
		}

		internal Instruction ResolvedInstruction
		{
			get
			{
				return this.instruction;
			}
		}

		public InstructionOffset(Instruction instruction)
		{
			if (instruction == null)
			{
				throw new ArgumentNullException("instruction");
			}
			this.instruction = instruction;
			this.offset = null;
		}

		public InstructionOffset(int offset)
		{
			this.instruction = null;
			this.offset = new int?(offset);
		}

		private readonly Instruction instruction;

		private readonly int? offset;
	}
}
