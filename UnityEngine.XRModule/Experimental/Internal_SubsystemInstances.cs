using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental
{
	internal static class Internal_SubsystemInstances
	{
		[RequiredByNativeCode]
		internal static void Internal_InitializeManagedInstance(IntPtr ptr, IntegratedSubsystem inst)
		{
			inst.m_Ptr = ptr;
			inst.SetHandle(inst);
			Internal_SubsystemInstances.s_IntegratedSubsystemInstances.Add(inst);
		}

		[RequiredByNativeCode]
		internal static void Internal_ClearManagedInstances()
		{
			foreach (ISubsystem subsystem in Internal_SubsystemInstances.s_IntegratedSubsystemInstances)
			{
				((IntegratedSubsystem)subsystem).m_Ptr = IntPtr.Zero;
			}
			Internal_SubsystemInstances.s_IntegratedSubsystemInstances.Clear();
			Internal_SubsystemInstances.s_StandaloneSubsystemInstances.Clear();
		}

		[RequiredByNativeCode]
		internal static void Internal_RemoveInstanceByPtr(IntPtr ptr)
		{
			for (int i = Internal_SubsystemInstances.s_IntegratedSubsystemInstances.Count - 1; i >= 0; i--)
			{
				if (((IntegratedSubsystem)Internal_SubsystemInstances.s_IntegratedSubsystemInstances[i]).m_Ptr == ptr)
				{
					((IntegratedSubsystem)Internal_SubsystemInstances.s_IntegratedSubsystemInstances[i]).m_Ptr = IntPtr.Zero;
					Internal_SubsystemInstances.s_IntegratedSubsystemInstances.RemoveAt(i);
				}
			}
		}

		internal static IntegratedSubsystem Internal_GetInstanceByPtr(IntPtr ptr)
		{
			foreach (ISubsystem subsystem in Internal_SubsystemInstances.s_IntegratedSubsystemInstances)
			{
				IntegratedSubsystem integratedSubsystem = (IntegratedSubsystem)subsystem;
				if (integratedSubsystem.m_Ptr == ptr)
				{
					return integratedSubsystem;
				}
			}
			return null;
		}

		internal static void Internal_AddStandaloneSubsystem(Subsystem inst)
		{
			Internal_SubsystemInstances.s_StandaloneSubsystemInstances.Add(inst);
		}

		internal static List<ISubsystem> s_IntegratedSubsystemInstances = new List<ISubsystem>();

		internal static List<ISubsystem> s_StandaloneSubsystemInstances = new List<ISubsystem>();
	}
}
