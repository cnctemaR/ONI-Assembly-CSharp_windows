using System;

namespace Mono.Cecil.Cil
{
	internal abstract class VariableReference
	{
		public TypeReference VariableType
		{
			get
			{
				return this.variable_type;
			}
			set
			{
				this.variable_type = value;
			}
		}

		public int Index
		{
			get
			{
				return this.index;
			}
		}

		internal VariableReference(TypeReference variable_type)
		{
			this.variable_type = variable_type;
		}

		public abstract VariableDefinition Resolve();

		public override string ToString()
		{
			if (this.index >= 0)
			{
				return "V_" + this.index.ToString();
			}
			return string.Empty;
		}

		internal int index = -1;

		protected TypeReference variable_type;
	}
}
