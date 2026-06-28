using System;

namespace Unity.Bindings
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
	internal class NativeNameAttribute : Attribute
	{
		public NativeNameAttribute(string name)
		{
			this.Name = name;
		}

		public string Name { get; set; }
	}
}
