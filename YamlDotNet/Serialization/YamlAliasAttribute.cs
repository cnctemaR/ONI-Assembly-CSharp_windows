using System;

namespace YamlDotNet.Serialization
{
	[Obsolete("Please use YamlMember instead")]
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class YamlAliasAttribute : Attribute
	{
		public string Alias { get; set; }

		public YamlAliasAttribute(string alias)
		{
			this.Alias = alias;
		}
	}
}
