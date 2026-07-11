using System;

namespace System.Diagnostics.Contracts
{
	[AttributeUsage(AttributeTargets.Field)]
	[Conditional("CONTRACTS_FULL")]
	public sealed class ContractPublicPropertyNameAttribute : Attribute
	{
		public ContractPublicPropertyNameAttribute(string name)
		{
			this._publicName = name;
		}

		public string Name
		{
			get
			{
				return this._publicName;
			}
		}

		private string _publicName;
	}
}
