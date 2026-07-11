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
			TSubsystem tsubsystem = Internal_SubsystemInstances.Internal_FindStandaloneSubsystemInstanceGivenDescriptor(this) as TSubsystem;
			bool flag = tsubsystem != null;
			TSubsystem tsubsystem2;
			if (flag)
			{
				tsubsystem2 = tsubsystem;
			}
			else
			{
				TSubsystem tsubsystem3 = Activator.CreateInstance(base.subsystemImplementationType) as TSubsystem;
				tsubsystem3.m_subsystemDescriptor = this;
				Internal_SubsystemInstances.Internal_AddStandaloneSubsystem(tsubsystem3);
				tsubsystem2 = tsubsystem3;
			}
			return tsubsystem2;
		}
	}
}
