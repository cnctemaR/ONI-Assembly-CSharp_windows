using System;

namespace UnityEngine.SubsystemsImplementation
{
	public abstract class SubsystemDescriptorWithProvider : ISubsystemDescriptor
	{
		public string id { get; set; }

		protected internal Type providerType { get; set; }

		protected internal Type subsystemTypeOverride { get; set; }

		internal abstract ISubsystem CreateImpl();

		ISubsystem ISubsystemDescriptor.Create()
		{
			return this.CreateImpl();
		}

		internal abstract void ThrowIfInvalid();
	}
}
