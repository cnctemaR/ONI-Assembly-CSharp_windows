using System;
using System.Runtime.CompilerServices;

namespace VYaml.Annotations
{
	[NullableContext(2)]
	[Nullable(0)]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public sealed class YamlMemberAttribute : Attribute
	{
		public string Name { get; }

		public int Order { get; set; }

		public YamlMemberAttribute(string name = null)
		{
			this.Name = name;
		}
	}
}
