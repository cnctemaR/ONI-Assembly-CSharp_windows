using System;

namespace System.Runtime.Serialization
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module, Inherited = false, AllowMultiple = true)]
	public sealed class ContractNamespaceAttribute : Attribute
	{
		public ContractNamespaceAttribute(string ns)
		{
			this.contract_ns = ns;
		}

		public string ClrNamespace
		{
			get
			{
				return this.clr_ns;
			}
			set
			{
				this.clr_ns = value;
			}
		}

		public string ContractNamespace
		{
			get
			{
				return this.contract_ns;
			}
		}

		private string clr_ns;

		private string contract_ns;
	}
}
