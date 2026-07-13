using System;

namespace Mono.Cecil
{
	internal struct CustomAttributeNamedArgument
	{
		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public CustomAttributeArgument Argument
		{
			get
			{
				return this.argument;
			}
		}

		public CustomAttributeNamedArgument(string name, CustomAttributeArgument argument)
		{
			Mixin.CheckName(name);
			this.name = name;
			this.argument = argument;
		}

		private readonly string name;

		private readonly CustomAttributeArgument argument;
	}
}
