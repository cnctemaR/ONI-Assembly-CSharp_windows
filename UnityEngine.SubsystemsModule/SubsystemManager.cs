using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine
{
	[NativeHeader("Modules/Subsystems/SubsystemManager.h")]
	public static class SubsystemManager
	{
		[RequiredByNativeCode]
		private static void ReloadSubsystemsStarted()
		{
			bool flag = SubsystemManager.reloadSubsytemsStarted != null;
			if (flag)
			{
				SubsystemManager.reloadSubsytemsStarted();
			}
			bool flag2 = SubsystemManager.beforeReloadSubsystems != null;
			if (flag2)
			{
				SubsystemManager.beforeReloadSubsystems();
			}
		}

		[RequiredByNativeCode]
		private static void ReloadSubsystemsCompleted()
		{
			bool flag = SubsystemManager.reloadSubsytemsCompleted != null;
			if (flag)
			{
				SubsystemManager.reloadSubsytemsCompleted();
			}
			bool flag2 = SubsystemManager.afterReloadSubsystems != null;
			if (flag2)
			{
				SubsystemManager.afterReloadSubsystems();
			}
		}

		[RequiredByNativeCode]
		private static void InitializeIntegratedSubsystem(IntPtr ptr, IntegratedSubsystem subsystem)
		{
			subsystem.m_Ptr = ptr;
			subsystem.SetHandle(subsystem);
			SubsystemManager.s_IntegratedSubsystems.Add(subsystem);
		}

		[RequiredByNativeCode]
		private static void ClearSubsystems()
		{
			foreach (IntegratedSubsystem integratedSubsystem in SubsystemManager.s_IntegratedSubsystems)
			{
				integratedSubsystem.m_Ptr = IntPtr.Zero;
			}
			SubsystemManager.s_IntegratedSubsystems.Clear();
			SubsystemManager.s_StandaloneSubsystems.Clear();
			SubsystemManager.s_DeprecatedSubsystems.Clear();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void StaticConstructScriptingClassMap();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ReportSingleSubsystemAnalytics(string id);

		static SubsystemManager()
		{
			SubsystemManager.StaticConstructScriptingClassMap();
		}

		public static void GetAllSubsystemDescriptors(List<ISubsystemDescriptor> descriptors)
		{
			SubsystemDescriptorStore.GetAllSubsystemDescriptors(descriptors);
		}

		public static void GetSubsystemDescriptors<T>(List<T> descriptors) where T : ISubsystemDescriptor
		{
			SubsystemDescriptorStore.GetSubsystemDescriptors<T>(descriptors);
		}

		public static void GetSubsystems<T>(List<T> subsystems) where T : ISubsystem
		{
			subsystems.Clear();
			SubsystemManager.AddSubsystemSubset<IntegratedSubsystem, T>(SubsystemManager.s_IntegratedSubsystems, subsystems);
			SubsystemManager.AddSubsystemSubset<SubsystemWithProvider, T>(SubsystemManager.s_StandaloneSubsystems, subsystems);
			SubsystemManager.AddSubsystemSubset<Subsystem, T>(SubsystemManager.s_DeprecatedSubsystems, subsystems);
		}

		private static void AddSubsystemSubset<TBaseTypeInList, TQueryType>(List<TBaseTypeInList> copyFrom, List<TQueryType> copyTo) where TBaseTypeInList : ISubsystem where TQueryType : ISubsystem
		{
			foreach (TBaseTypeInList tbaseTypeInList in copyFrom)
			{
				TQueryType tqueryType;
				bool flag;
				if (tbaseTypeInList is TQueryType)
				{
					tqueryType = tbaseTypeInList as TQueryType;
					flag = true;
				}
				else
				{
					flag = false;
				}
				bool flag2 = flag;
				if (flag2)
				{
					copyTo.Add(tqueryType);
				}
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action beforeReloadSubsystems;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action afterReloadSubsystems;

		internal static IntegratedSubsystem GetIntegratedSubsystemByPtr(IntPtr ptr)
		{
			foreach (IntegratedSubsystem integratedSubsystem in SubsystemManager.s_IntegratedSubsystems)
			{
				bool flag = integratedSubsystem.m_Ptr == ptr;
				if (flag)
				{
					return integratedSubsystem;
				}
			}
			return null;
		}

		internal static void RemoveIntegratedSubsystemByPtr(IntPtr ptr)
		{
			for (int i = 0; i < SubsystemManager.s_IntegratedSubsystems.Count; i++)
			{
				bool flag = SubsystemManager.s_IntegratedSubsystems[i].m_Ptr != ptr;
				if (!flag)
				{
					SubsystemManager.s_IntegratedSubsystems[i].m_Ptr = IntPtr.Zero;
					SubsystemManager.s_IntegratedSubsystems.RemoveAt(i);
					break;
				}
			}
		}

		internal static void AddStandaloneSubsystem(SubsystemWithProvider subsystem)
		{
			SubsystemManager.s_StandaloneSubsystems.Add(subsystem);
		}

		internal static bool RemoveStandaloneSubsystem(SubsystemWithProvider subsystem)
		{
			return SubsystemManager.s_StandaloneSubsystems.Remove(subsystem);
		}

		internal static SubsystemWithProvider FindStandaloneSubsystemByDescriptor(SubsystemDescriptorWithProvider descriptor)
		{
			foreach (SubsystemWithProvider subsystemWithProvider in SubsystemManager.s_StandaloneSubsystems)
			{
				bool flag = subsystemWithProvider.descriptor == descriptor;
				if (flag)
				{
					return subsystemWithProvider;
				}
			}
			return null;
		}

		public static void GetInstances<T>(List<T> subsystems) where T : ISubsystem
		{
			SubsystemManager.GetSubsystems<T>(subsystems);
		}

		internal static void AddDeprecatedSubsystem(Subsystem subsystem)
		{
			SubsystemManager.s_DeprecatedSubsystems.Add(subsystem);
		}

		internal static bool RemoveDeprecatedSubsystem(Subsystem subsystem)
		{
			return SubsystemManager.s_DeprecatedSubsystems.Remove(subsystem);
		}

		internal static Subsystem FindDeprecatedSubsystemByDescriptor(SubsystemDescriptor descriptor)
		{
			foreach (Subsystem subsystem in SubsystemManager.s_DeprecatedSubsystems)
			{
				bool flag = subsystem.m_SubsystemDescriptor == descriptor;
				if (flag)
				{
					return subsystem;
				}
			}
			return null;
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action reloadSubsytemsStarted;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action reloadSubsytemsCompleted;

		private static List<IntegratedSubsystem> s_IntegratedSubsystems = new List<IntegratedSubsystem>();

		private static List<SubsystemWithProvider> s_StandaloneSubsystems = new List<SubsystemWithProvider>();

		private static List<Subsystem> s_DeprecatedSubsystems = new List<Subsystem>();
	}
}
