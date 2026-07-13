using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.SceneManagement
{
	[StaticAccessor("SceneManagerBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("Runtime/Export/SceneManager/SceneManager.bindings.h")]
	[NativeHeader("Runtime/SceneManager/SceneManager.h")]
	internal static class SceneManagerAPIInternal
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetNumScenesInBuildSettings();

		[NativeThrows]
		public static Scene GetSceneByBuildIndex(int buildIndex)
		{
			Scene scene;
			SceneManagerAPIInternal.GetSceneByBuildIndex_Injected(buildIndex, out scene);
			return scene;
		}

		[NativeThrows]
		public unsafe static AsyncOperation LoadSceneAsyncNameIndexInternal(string sceneName, int sceneBuildIndex, LoadSceneParameters parameters, bool mustCompleteNextFrame)
		{
			AsyncOperation asyncOperation;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(sceneName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = sceneName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr = SceneManagerAPIInternal.LoadSceneAsyncNameIndexInternal_Injected(ref managedSpanWrapper, sceneBuildIndex, ref parameters, mustCompleteNextFrame);
			}
			finally
			{
				IntPtr intPtr;
				IntPtr intPtr2 = intPtr;
				asyncOperation = ((intPtr2 == 0) ? null : AsyncOperation.BindingsMarshaller.ConvertToManaged(intPtr2));
				char* ptr = null;
			}
			return asyncOperation;
		}

		[NativeThrows]
		public unsafe static AsyncOperation UnloadSceneNameIndexInternal(string sceneName, int sceneBuildIndex, bool immediately, UnloadSceneOptions options, out bool outSuccess)
		{
			AsyncOperation asyncOperation;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(sceneName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = sceneName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr = SceneManagerAPIInternal.UnloadSceneNameIndexInternal_Injected(ref managedSpanWrapper, sceneBuildIndex, immediately, options, out outSuccess);
			}
			finally
			{
				IntPtr intPtr;
				IntPtr intPtr2 = intPtr;
				asyncOperation = ((intPtr2 == 0) ? null : AsyncOperation.BindingsMarshaller.ConvertToManaged(intPtr2));
				char* ptr = null;
			}
			return asyncOperation;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSceneByBuildIndex_Injected(int buildIndex, out Scene ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr LoadSceneAsyncNameIndexInternal_Injected(ref ManagedSpanWrapper sceneName, int sceneBuildIndex, [In] ref LoadSceneParameters parameters, bool mustCompleteNextFrame);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr UnloadSceneNameIndexInternal_Injected(ref ManagedSpanWrapper sceneName, int sceneBuildIndex, bool immediately, UnloadSceneOptions options, out bool outSuccess);
	}
}
