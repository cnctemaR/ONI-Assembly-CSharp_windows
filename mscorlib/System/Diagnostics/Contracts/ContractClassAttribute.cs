using System;

namespace System.Diagnostics.Contracts
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
	[Conditional("DEBUG")]
	[Conditional("CONTRACTS_FULL")]
	public sealed class ContractClassAttribute : Attribute
	{
		public ContractClassAttribute(Type typeContainingContracts)
		{
			this._typeWithContracts = typeContainingContracts;
		}

		public Type TypeContainingContracts
		{
			get
			{
				return this._typeWithContracts;
			}
		}

		private Type _typeWithContracts;
	}
}
