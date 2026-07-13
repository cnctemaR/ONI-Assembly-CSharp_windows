using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Iced.Intel
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class InstructionListDebugView
	{
		public InstructionListDebugView(InstructionList list)
		{
			if (list == null)
			{
				throw new ArgumentNullException("list");
			}
			this.list = list;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public Instruction[] Items
		{
			get
			{
				return this.list.ToArray();
			}
		}

		private readonly InstructionList list;
	}
}
