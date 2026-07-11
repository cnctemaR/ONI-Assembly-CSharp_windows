using System;

namespace System.Runtime.Serialization
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module, Inherited = false, AllowMultiple = true)]
	public sealed class ContractNamespaceAttribute : Attribute
	{
		public ContractNamespaceAttribute(string contractNamespace)
		{
			this.contractNamespace = contractNamespace;
		}

		public string ClrNamespace
		{
			get
			{
				return this.clrNamespace;
			}
			set
			{
				this.clrNamespace = value;
			}
		}

		public string ContractNamespace
		{
			get
			{
				return this.contractNamespace;
			}
		}

		private string clrNamespace;

		private string contractNamespace;
	}
}
