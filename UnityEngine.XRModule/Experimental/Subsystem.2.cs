using System;

namespace UnityEngine.Experimental
{
	public abstract class Subsystem<TSubsystemDescriptor> : Subsystem where TSubsystemDescriptor : ISubsystemDescriptor
	{
		public TSubsystemDescriptor SubsystemDescriptor
		{
			get
			{
				return (TSubsystemDescriptor)((object)this.m_subsystemDescriptor);
			}
		}
	}
}
