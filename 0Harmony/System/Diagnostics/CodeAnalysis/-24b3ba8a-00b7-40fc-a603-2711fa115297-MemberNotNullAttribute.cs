using System;
using System.Runtime.CompilerServices;

namespace System.Diagnostics.CodeAnalysis
{
	[NullableContext(1)]
	[Nullable(0)]
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
	[ExcludeFromCodeCoverage]
	[DebuggerNonUserCode]
	internal sealed class <24b3ba8a-00b7-40fc-a603-2711fa115297>MemberNotNullAttribute : Attribute
	{
		public string[] Members { get; }

		public <24b3ba8a-00b7-40fc-a603-2711fa115297>MemberNotNullAttribute(string member)
		{
			this.Members = new string[] { member };
		}

		public <24b3ba8a-00b7-40fc-a603-2711fa115297>MemberNotNullAttribute(params string[] members)
		{
			this.Members = members;
		}
	}
}
