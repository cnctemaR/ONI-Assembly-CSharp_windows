using System;

namespace System.Diagnostics.Contracts
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	[Conditional("CONTRACTS_FULL")]
	public sealed class ContractClassForAttribute : Attribute
	{
		public ContractClassForAttribute(Type typeContractsAreFor)
		{
			this._typeIAmAContractFor = typeContractsAreFor;
		}

		public Type TypeContractsAreFor
		{
			get
			{
				return this._typeIAmAContractFor;
			}
		}

		private Type _typeIAmAContractFor;
	}
}
