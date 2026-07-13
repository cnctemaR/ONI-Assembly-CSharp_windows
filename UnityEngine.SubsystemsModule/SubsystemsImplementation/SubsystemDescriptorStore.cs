using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.SubsystemsImplementation
{
	[NativeHeader("Modules/Subsystems/SubsystemManager.h")]
	public static class SubsystemDescriptorStore
	{
		[RequiredByNativeCode]
		internal static void InitializeManagedDescriptor(IntPtr ptr, IntegratedSubsystemDescriptor desc)
		{
			desc.m_Ptr = ptr;
			SubsystemDescriptorStore.s_IntegratedDescriptors.Add(desc);
		}

		[RequiredByNativeCode]
		internal static void ClearManagedDescriptors()
		{
			foreach (IntegratedSubsystemDescriptor integratedSubsystemDescriptor in SubsystemDescriptorStore.s_IntegratedDescriptors)
			{
				integratedSubsystemDescriptor.m_Ptr = IntPtr.Zero;
			}
			SubsystemDescriptorStore.s_IntegratedDescriptors.Clear();
		}

		private unsafe static void ReportSingleSubsystemAnalytics(string id)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(id, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = id.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				SubsystemDescriptorStore.ReportSingleSubsystemAnalytics_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public static void RegisterDescriptor(SubsystemDescriptorWithProvider descriptor)
		{
			descriptor.ThrowIfInvalid();
			SubsystemDescriptorStore.RegisterDescriptor<SubsystemDescriptorWithProvider, SubsystemDescriptorWithProvider>(descriptor, SubsystemDescriptorStore.s_StandaloneDescriptors);
		}

		internal static void GetAllSubsystemDescriptors(List<ISubsystemDescriptor> descriptors)
		{
			descriptors.Clear();
			int num = SubsystemDescriptorStore.s_IntegratedDescriptors.Count + SubsystemDescriptorStore.s_StandaloneDescriptors.Count + SubsystemDescriptorStore.s_DeprecatedDescriptors.Count;
			bool flag = descriptors.Capacity < num;
			if (flag)
			{
				descriptors.Capacity = num;
			}
			SubsystemDescriptorStore.AddDescriptorSubset<IntegratedSubsystemDescriptor>(SubsystemDescriptorStore.s_IntegratedDescriptors, descriptors);
			SubsystemDescriptorStore.AddDescriptorSubset<SubsystemDescriptorWithProvider>(SubsystemDescriptorStore.s_StandaloneDescriptors, descriptors);
			SubsystemDescriptorStore.AddDescriptorSubset<SubsystemDescriptor>(SubsystemDescriptorStore.s_DeprecatedDescriptors, descriptors);
		}

		private static void AddDescriptorSubset<TBaseTypeInList>(List<TBaseTypeInList> copyFrom, List<ISubsystemDescriptor> copyTo) where TBaseTypeInList : ISubsystemDescriptor
		{
			foreach (TBaseTypeInList tbaseTypeInList in copyFrom)
			{
				copyTo.Add(tbaseTypeInList);
			}
		}

		internal static void GetSubsystemDescriptors<T>(List<T> descriptors) where T : ISubsystemDescriptor
		{
			descriptors.Clear();
			SubsystemDescriptorStore.AddDescriptorSubset<IntegratedSubsystemDescriptor, T>(SubsystemDescriptorStore.s_IntegratedDescriptors, descriptors);
			SubsystemDescriptorStore.AddDescriptorSubset<SubsystemDescriptorWithProvider, T>(SubsystemDescriptorStore.s_StandaloneDescriptors, descriptors);
			SubsystemDescriptorStore.AddDescriptorSubset<SubsystemDescriptor, T>(SubsystemDescriptorStore.s_DeprecatedDescriptors, descriptors);
		}

		private static void AddDescriptorSubset<TBaseTypeInList, TQueryType>(List<TBaseTypeInList> copyFrom, List<TQueryType> copyTo) where TBaseTypeInList : ISubsystemDescriptor where TQueryType : ISubsystemDescriptor
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

		internal static void RegisterDescriptor<TDescriptor, TBaseTypeInList>(TDescriptor descriptor, List<TBaseTypeInList> storeInList) where TDescriptor : TBaseTypeInList where TBaseTypeInList : ISubsystemDescriptor
		{
			for (int i = 0; i < storeInList.Count; i++)
			{
				TBaseTypeInList tbaseTypeInList = storeInList[i];
				bool flag = tbaseTypeInList.id != descriptor.id;
				if (!flag)
				{
					Debug.LogWarning("Registering subsystem descriptor with duplicate ID '" + descriptor.id + "' - overwriting previous entry.");
					storeInList[i] = (TBaseTypeInList)((object)descriptor);
					return;
				}
			}
			SubsystemDescriptorStore.ReportSingleSubsystemAnalytics(descriptor.id);
			storeInList.Add((TBaseTypeInList)((object)descriptor));
		}

		internal static void RegisterDeprecatedDescriptor(SubsystemDescriptor descriptor)
		{
			SubsystemDescriptorStore.RegisterDescriptor<SubsystemDescriptor, SubsystemDescriptor>(descriptor, SubsystemDescriptorStore.s_DeprecatedDescriptors);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ReportSingleSubsystemAnalytics_Injected(ref ManagedSpanWrapper id);

		private static List<IntegratedSubsystemDescriptor> s_IntegratedDescriptors = new List<IntegratedSubsystemDescriptor>();

		private static List<SubsystemDescriptorWithProvider> s_StandaloneDescriptors = new List<SubsystemDescriptorWithProvider>();

		private static List<SubsystemDescriptor> s_DeprecatedDescriptors = new List<SubsystemDescriptor>();
	}
}
