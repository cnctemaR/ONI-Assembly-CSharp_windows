using System;

namespace UnityEngine.Experimental
{
	public class SubsystemDescriptor<TSubsystem> : SubsystemDescriptor where TSubsystem : Subsystem
	{
		public TSubsystem Create()
		{
			TSubsystem tsubsystem = Activator.CreateInstance(base.subsystemImplementationType) as TSubsystem;
			tsubsystem.m_subsystemDescriptor = this;
			Internal_SubsystemInstances.Internal_AddStandaloneSubsystem(tsubsystem);
			return tsubsystem;
		}
	}
}
