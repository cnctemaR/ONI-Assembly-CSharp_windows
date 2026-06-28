using System;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class YamlMemberAttribute : Attribute
	{
		public Type SerializeAs { get; set; }

		public int Order { get; set; }

		public string Alias { get; set; }

		public ScalarStyle ScalarStyle { get; set; }

		public YamlMemberAttribute()
		{
			this.ScalarStyle = ScalarStyle.Any;
		}

		public YamlMemberAttribute(Type serializeAs)
			: this()
		{
			this.SerializeAs = serializeAs;
		}
	}
}
