using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[UsedByNativeCode("Subsystem_TSubsystemDescriptor")]
	public class IntegratedSubsystem<TSubsystemDescriptor> : IntegratedSubsystem where TSubsystemDescriptor : ISubsystemDescriptor
	{
		public TSubsystemDescriptor subsystemDescriptor
		{
			get
			{
				return (TSubsystemDescriptor)((object)this.m_SubsystemDescriptor);
			}
		}

		[Obsolete("The property 'SubsystemDescriptor' is deprecated. Use `subsystemDescriptor` instead. UnityUpgradeable -> subsystemDescriptor", false)]
		public TSubsystemDescriptor SubsystemDescriptor
		{
			get
			{
				return this.subsystemDescriptor;
			}
		}
	}
}
