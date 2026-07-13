using System;

namespace System.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	internal sealed class MemberNotNullAttribute : Attribute
	{
		public MemberNotNullAttribute(string member)
		{
			this.Members = new string[] { member };
		}

		public MemberNotNullAttribute(params string[] members)
		{
			this.Members = members;
		}

		public string[] Members { get; }
	}
}
