using System;

namespace Mono.Cecil.Cil
{
	internal struct VariableIndex
	{
		public int Index
		{
			get
			{
				if (this.variable != null)
				{
					return this.variable.Index;
				}
				if (this.index != null)
				{
					return this.index.Value;
				}
				throw new NotSupportedException();
			}
		}

		internal bool IsResolved
		{
			get
			{
				return this.variable != null;
			}
		}

		internal VariableDefinition ResolvedVariable
		{
			get
			{
				return this.variable;
			}
		}

		public VariableIndex(VariableDefinition variable)
		{
			if (variable == null)
			{
				throw new ArgumentNullException("variable");
			}
			this.variable = variable;
			this.index = null;
		}

		public VariableIndex(int index)
		{
			this.variable = null;
			this.index = new int?(index);
		}

		private readonly VariableDefinition variable;

		private readonly int? index;
	}
}
