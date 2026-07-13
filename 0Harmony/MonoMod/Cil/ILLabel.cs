using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Mono.Cecil.Cil;

namespace MonoMod.Cil
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class ILLabel
	{
		[Nullable(2)]
		public Instruction Target
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		public IEnumerable<Instruction> Branches
		{
			get
			{
				return this.Context.Instrs.Where<Instruction>((Instruction i) => i.Operand == this);
			}
		}

		internal ILLabel(ILContext context)
		{
			this.Context = context;
			this.Context._Labels.Add(this);
		}

		internal ILLabel(ILContext context, [Nullable(2)] Instruction target)
			: this(context)
		{
			this.Target = target;
		}

		private readonly ILContext Context;
	}
}
