using System;

namespace System.Reflection
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface)]
	public sealed class DefaultMemberAttribute : Attribute
	{
		public DefaultMemberAttribute(string memberName)
		{
			this.MemberName = memberName;
		}

		public string MemberName { get; }
	}
}
