using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental
{
	internal static class Internal_SubsystemInstances
	{
		[RequiredByNativeCode]
		internal static void Internal_InitializeManagedInstance(IntPtr ptr, Subsystem inst)
		{
			inst.m_Ptr = ptr;
			inst.SetHandle(inst);
			Internal_SubsystemInstances.s_SubsystemInstances.Add(inst);
		}

		[RequiredByNativeCode]
		internal static void Internal_ClearManagedInstances()
		{
			foreach (Subsystem subsystem in Internal_SubsystemInstances.s_SubsystemInstances)
			{
				subsystem.m_Ptr = IntPtr.Zero;
			}
			Internal_SubsystemInstances.s_SubsystemInstances.Clear();
		}

		[RequiredByNativeCode]
		internal static void Internal_RemoveInstanceByPtr(IntPtr ptr)
		{
			for (int i = Internal_SubsystemInstances.s_SubsystemInstances.Count - 1; i >= 0; i--)
			{
				if (Internal_SubsystemInstances.s_SubsystemInstances[i].m_Ptr == ptr)
				{
					Internal_SubsystemInstances.s_SubsystemInstances[i].m_Ptr = IntPtr.Zero;
					Internal_SubsystemInstances.s_SubsystemInstances.RemoveAt(i);
				}
			}
		}

		internal static Subsystem Internal_GetInstanceByPtr(IntPtr ptr)
		{
			foreach (Subsystem subsystem in Internal_SubsystemInstances.s_SubsystemInstances)
			{
				if (subsystem.m_Ptr == ptr)
				{
					return subsystem;
				}
			}
			return null;
		}

		internal static List<Subsystem> s_SubsystemInstances = new List<Subsystem>();
	}
}
