using System;

namespace Mono.Cecil.Cil
{
	internal sealed class ConstantDebugInformation : DebugInformation
	{
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

		public TypeReference ConstantType
		{
			get
			{
				return this.constant_type;
			}
			set
			{
				this.constant_type = value;
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		public ConstantDebugInformation(string name, TypeReference constant_type, object value)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.name = name;
			this.constant_type = constant_type;
			this.value = value;
			this.token = new MetadataToken(TokenType.LocalConstant);
		}

		private string name;

		private TypeReference constant_type;

		private object value;
	}
}
