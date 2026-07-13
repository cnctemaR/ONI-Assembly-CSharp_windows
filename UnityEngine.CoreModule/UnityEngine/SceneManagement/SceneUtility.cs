using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.SceneManagement
{
	[NativeHeader("Runtime/Export/SceneManager/SceneUtility.bindings.h")]
	public static class SceneUtility
	{
		[StaticAccessor("SceneUtilityBindings", StaticAccessorType.DoubleColon)]
		public static string GetScenePathByBuildIndex(int buildIndex)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				SceneUtility.GetScenePathByBuildIndex_Injected(buildIndex, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[StaticAccessor("SceneUtilityBindings", StaticAccessorType.DoubleColon)]
		public unsafe static int GetBuildIndexByScenePath(string scenePath)
		{
			int buildIndexByScenePath_Injected;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(scenePath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = scenePath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				buildIndexByScenePath_Injected = SceneUtility.GetBuildIndexByScenePath_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return buildIndexByScenePath_Injected;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetScenePathByBuildIndex_Injected(int buildIndex, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetBuildIndexByScenePath_Injected(ref ManagedSpanWrapper scenePath);
	}
}
