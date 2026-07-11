using System;

namespace System.Diagnostics.Contracts
{
	[Conditional("CONTRACTS_FULL")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property)]
	public sealed class ContractVerificationAttribute : Attribute
	{
		public ContractVerificationAttribute(bool value)
		{
			this._value = value;
		}

		public bool Value
		{
			get
			{
				return this._value;
			}
		}

		private bool _value;
	}
}
