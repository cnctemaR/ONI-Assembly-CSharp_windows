using System;

namespace Mono.Cecil
{
	internal struct CustomAttributeArgument
	{
		public TypeReference Type
		{
			get
			{
				return this.type;
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		public CustomAttributeArgument(TypeReference type, object value)
		{
			Mixin.CheckType(type);
			this.type = type;
			this.value = value;
		}

		private readonly TypeReference type;

		private readonly object value;
	}
}
