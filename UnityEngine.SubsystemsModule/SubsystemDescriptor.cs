using System;

namespace UnityEngine
{
	[Obsolete("Use SubsystemDescriptorWithProvider instead.", false)]
	public abstract class SubsystemDescriptor : ISubsystemDescriptor
	{
		public string id { get; set; }

		public Type subsystemImplementationType { get; set; }

		ISubsystem ISubsystemDescriptor.Create()
		{
			return this.CreateImpl();
		}

		internal abstract ISubsystem CreateImpl();
	}
}
