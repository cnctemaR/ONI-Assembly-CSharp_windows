using System;

namespace UnityEngine.SubsystemsImplementation
{
	public abstract class SubsystemWithProvider<TSubsystem, TSubsystemDescriptor, TProvider> : SubsystemWithProvider where TSubsystem : SubsystemWithProvider, new() where TSubsystemDescriptor : SubsystemDescriptorWithProvider where TProvider : SubsystemProvider<TSubsystem>
	{
		public TSubsystemDescriptor subsystemDescriptor { get; private set; }

		protected internal TProvider provider { get; private set; }

		protected virtual void OnCreate()
		{
		}

		protected override void OnStart()
		{
			this.provider.Start();
		}

		protected override void OnStop()
		{
			this.provider.Stop();
		}

		protected override void OnDestroy()
		{
			this.provider.Destroy();
		}

		internal sealed override void Initialize(SubsystemDescriptorWithProvider descriptor, SubsystemProvider provider)
		{
			base.providerBase = provider;
			this.provider = (TProvider)((object)provider);
			this.subsystemDescriptor = (TSubsystemDescriptor)((object)descriptor);
			this.OnCreate();
		}

		internal sealed override SubsystemDescriptorWithProvider descriptor
		{
			get
			{
				return this.subsystemDescriptor;
			}
		}
	}
}
