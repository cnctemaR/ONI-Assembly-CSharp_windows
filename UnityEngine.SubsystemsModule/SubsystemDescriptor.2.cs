using System;

namespace UnityEngine
{
	public class SubsystemDescriptor<TSubsystem> : SubsystemDescriptor where TSubsystem : Subsystem
	{
		internal override ISubsystem CreateImpl()
		{
			return this.Create();
		}

		public TSubsystem Create()
		{
			TSubsystem tsubsystem = SubsystemManager.FindDeprecatedSubsystemByDescriptor(this) as TSubsystem;
			bool flag = tsubsystem != null;
			TSubsystem tsubsystem2;
			if (flag)
			{
				tsubsystem2 = tsubsystem;
			}
			else
			{
				tsubsystem = Activator.CreateInstance(base.subsystemImplementationType) as TSubsystem;
				tsubsystem.m_SubsystemDescriptor = this;
				SubsystemManager.AddDeprecatedSubsystem(tsubsystem);
				tsubsystem2 = tsubsystem;
			}
			return tsubsystem2;
		}
	}
}
