using System;

namespace Unity.Jobs.LowLevel.Unsafe
{
	/// <summary>
	///   <para>All job interface types must be marked with the JobProducerType. This is used to compile the Execute method by the Burst ASM inspector.</para>
	/// </summary>
	[AttributeUsage(AttributeTargets.Interface)]
	public sealed class JobProducerTypeAttribute : Attribute
	{
		/// <summary>
		///   <para></para>
		/// </summary>
		/// <param name="producerType">The type containing a static method named "Execute" method which is the method invokes by the job system.</param>
		public JobProducerTypeAttribute(Type producerType)
		{
			this.ProducerType = producerType;
		}

		/// <summary>
		///   <para>ProducerType is the type containing a static method named "Execute" method which is the method invokes by the job system.</para>
		/// </summary>
		public Type ProducerType { get; }
	}
}
