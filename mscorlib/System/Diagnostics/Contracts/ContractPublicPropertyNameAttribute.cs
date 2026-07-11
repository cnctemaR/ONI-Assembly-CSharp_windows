using System;

namespace System.Diagnostics.Contracts
{
	[Conditional("CONTRACTS_FULL")]
	[AttributeUsage(AttributeTargets.Field)]
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
