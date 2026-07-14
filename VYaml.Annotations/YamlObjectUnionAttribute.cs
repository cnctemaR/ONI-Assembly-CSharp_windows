using System;
using System.Runtime.CompilerServices;

namespace VYaml.Annotations
{
	[NullableContext(1)]
	[Nullable(0)]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = true, Inherited = false)]
	public class YamlObjectUnionAttribute : Attribute
	{
		public string Tag { get; }

		public Type SubType { get; }

		public YamlObjectUnionAttribute(string tagString, Type subType)
		{
			this.Tag = tagString;
			this.SubType = subType;
		}
	}
}
