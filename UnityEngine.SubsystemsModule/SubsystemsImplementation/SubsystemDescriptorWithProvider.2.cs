using System;

namespace UnityEngine.SubsystemsImplementation
{
	public class SubsystemDescriptorWithProvider<TSubsystem, TProvider> : SubsystemDescriptorWithProvider where TSubsystem : SubsystemWithProvider, new() where TProvider : SubsystemProvider<TSubsystem>
	{
		internal override ISubsystem CreateImpl()
		{
			return this.Create();
		}

		public TSubsystem Create()
		{
			TSubsystem tsubsystem = SubsystemManager.FindStandaloneSubsystemByDescriptor(this) as TSubsystem;
			bool flag = tsubsystem != null;
			TSubsystem tsubsystem2;
			if (flag)
			{
				tsubsystem2 = tsubsystem;
			}
			else
			{
				TProvider tprovider = this.CreateProvider();
				bool flag2 = tprovider == null;
				if (flag2)
				{
					tsubsystem2 = default(TSubsystem);
				}
				else
				{
					tsubsystem = ((base.subsystemTypeOverride != null) ? ((TSubsystem)((object)Activator.CreateInstance(base.subsystemTypeOverride))) : new TSubsystem());
					tsubsystem.Initialize(this, tprovider);
					SubsystemManager.AddStandaloneSubsystem(tsubsystem);
					tsubsystem2 = tsubsystem;
				}
			}
			return tsubsystem2;
		}

		internal sealed override void ThrowIfInvalid()
		{
			bool flag = base.providerType == null;
			if (flag)
			{
				throw new InvalidOperationException("Invalid descriptor - must supply a valid providerType field!");
			}
			bool flag2 = !base.providerType.IsSubclassOf(typeof(TProvider));
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("Can't create provider - providerType '{0}' is not a subclass of '{1}'!", base.providerType.ToString(), typeof(TProvider).ToString()));
			}
			bool flag3 = base.subsystemTypeOverride != null && !base.subsystemTypeOverride.IsSubclassOf(typeof(TSubsystem));
			if (flag3)
			{
				throw new InvalidOperationException(string.Format("Can't create provider - subsystemTypeOverride '{0}' is not a subclass of '{1}'!", base.subsystemTypeOverride.ToString(), typeof(TSubsystem).ToString()));
			}
		}

		internal TProvider CreateProvider()
		{
			TProvider tprovider = (TProvider)((object)Activator.CreateInstance(base.providerType));
			return tprovider.TryInitialize() ? tprovider : default(TProvider);
		}
	}
}
