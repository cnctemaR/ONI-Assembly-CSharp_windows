using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine
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
				bool flag = ((IntegratedSubsystem)Internal_SubsystemInstances.s_IntegratedSubsystemInstances[i]).m_Ptr == ptr;
				if (flag)
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
				bool flag = integratedSubsystem.m_Ptr == ptr;
				if (flag)
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

		internal static Subsystem Internal_FindStandaloneSubsystemInstanceGivenDescriptor(SubsystemDescriptor descriptor)
		{
			foreach (ISubsystem subsystem in Internal_SubsystemInstances.s_StandaloneSubsystemInstances)
			{
				Subsystem subsystem2 = (Subsystem)subsystem;
				bool flag = subsystem2.m_subsystemDescriptor == descriptor;
				if (flag)
				{
					return subsystem2;
				}
			}
			return null;
		}

		internal static List<ISubsystem> s_IntegratedSubsystemInstances = new List<ISubsystem>();

		internal static List<ISubsystem> s_StandaloneSubsystemInstances = new List<ISubsystem>();
	}
}
