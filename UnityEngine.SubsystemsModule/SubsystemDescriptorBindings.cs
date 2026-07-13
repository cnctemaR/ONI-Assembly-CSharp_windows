using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	internal static class SubsystemDescriptorBindings
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern IntPtr Create(IntPtr descriptorPtr);

		public static string GetId(IntPtr descriptorPtr)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				SubsystemDescriptorBindings.GetId_Injected(descriptorPtr, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetId_Injected(IntPtr descriptorPtr, out ManagedSpanWrapper ret);
	}
}
