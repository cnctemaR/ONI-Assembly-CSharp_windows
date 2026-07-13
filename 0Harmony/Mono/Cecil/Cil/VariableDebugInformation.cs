using System;

namespace Mono.Cecil.Cil
{
	internal sealed class VariableDebugInformation : DebugInformation
	{
		public int Index
		{
			get
			{
				return this.index.Index;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public VariableAttributes Attributes
		{
			get
			{
				return (VariableAttributes)this.attributes;
			}
			set
			{
				this.attributes = (ushort)value;
			}
		}

		public bool IsDebuggerHidden
		{
			get
			{
				return this.attributes.GetAttributes(1);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(1, value);
			}
		}

		internal VariableDebugInformation(int index, string name)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.index = new VariableIndex(index);
			this.name = name;
		}

		public VariableDebugInformation(VariableDefinition variable, string name)
		{
			if (variable == null)
			{
				throw new ArgumentNullException("variable");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.index = new VariableIndex(variable);
			this.name = name;
			this.token = new MetadataToken(TokenType.LocalVariable);
		}

		private string name;

		private ushort attributes;

		internal VariableIndex index;
	}
}
