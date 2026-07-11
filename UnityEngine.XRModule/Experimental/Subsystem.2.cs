using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental
{
	[UsedByNativeCode("XRSubsystem_TXRSubsystemDescriptor")]
	public class Subsystem<TSubsystemDescriptor> : Subsystem where TSubsystemDescriptor : ISubsystemDescriptor
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
