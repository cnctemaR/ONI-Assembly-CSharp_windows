using System;
using System.Runtime.CompilerServices;

namespace System.Diagnostics.CodeAnalysis
{
	[NullableContext(1)]
	[Nullable(0)]
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class <1c2fb156-e9ba-45cc-af54-d5335bdb59af>MemberNotNullAttribute : Attribute
	{
		public string[] Members { get; }

		public <1c2fb156-e9ba-45cc-af54-d5335bdb59af>MemberNotNullAttribute(string member)
		{
			this.Members = new string[] { member };
		}

		public <1c2fb156-e9ba-45cc-af54-d5335bdb59af>MemberNotNullAttribute(params string[] members)
		{
			this.Members = members;
		}
	}
}
