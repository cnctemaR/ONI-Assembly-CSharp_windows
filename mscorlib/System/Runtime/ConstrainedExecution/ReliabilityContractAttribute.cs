using System;

namespace System.Runtime.ConstrainedExecution
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Interface, Inherited = false)]
	public sealed class ReliabilityContractAttribute : Attribute
	{
		public ReliabilityContractAttribute(Consistency consistencyGuarantee, Cer cer)
		{
			this.consistency = consistencyGuarantee;
			this.cer = cer;
		}

		public Cer Cer
		{
			get
			{
				return this.cer;
			}
		}

		public Consistency ConsistencyGuarantee
		{
			get
			{
				return this.consistency;
			}
		}

		private Consistency consistency;

		private Cer cer;
	}
}
