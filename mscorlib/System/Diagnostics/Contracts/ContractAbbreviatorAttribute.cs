using System;

namespace System.Diagnostics.Contracts
{
	[Conditional("CONTRACTS_FULL")]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public sealed class ContractAbbreviatorAttribute : Attribute
	{
	}
}
