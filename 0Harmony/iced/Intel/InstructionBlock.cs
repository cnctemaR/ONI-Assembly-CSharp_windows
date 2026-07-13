using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Iced.Intel
{
	[NullableContext(1)]
	[Nullable(0)]
	internal readonly struct InstructionBlock
	{
		public InstructionBlock(CodeWriter codeWriter, IList<Instruction> instructions, ulong rip)
		{
			if (codeWriter == null)
			{
				throw new ArgumentNullException("codeWriter");
			}
			this.CodeWriter = codeWriter;
			if (instructions == null)
			{
				throw new ArgumentNullException("instructions");
			}
			this.Instructions = instructions;
			this.RIP = rip;
		}

		public readonly CodeWriter CodeWriter;

		public readonly IList<Instruction> Instructions;

		public readonly ulong RIP;
	}
}
