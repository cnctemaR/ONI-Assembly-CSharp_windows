using System;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Assembly, Inherited = false)]
	public sealed class AssemblyDescriptionAttribute : Attribute
	{
		public AssemblyDescriptionAttribute(string description)
		{
			this.Description = description;
		}

		public string Description { get; }
	}
}
