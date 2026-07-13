using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Content;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.SceneManagement;

namespace Unity.Loading
{
	[StaticAccessor("GetContentLoadFrontend()", StaticAccessorType.Dot)]
	[NativeHeader("Modules/ContentLoad/Public/ContentLoadFrontend.h")]
	public static class ContentLoadInterface
	{
		[NativeThrows]
		internal unsafe static ContentFile LoadContentFileAsync(ContentNamespace nameSpace, string filename, void* dependencies, int dependencyCount, JobHandle dependentFence, bool useUnsafe = false)
		{
			ContentFile contentFile2;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(filename, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = filename.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ContentFile contentFile;
				ContentLoadInterface.LoadContentFileAsync_Injected(ref nameSpace, ref managedSpanWrapper, dependencies, dependencyCount, ref dependentFence, useUnsafe, out contentFile);
			}
			finally
			{
				char* ptr = null;
				ContentFile contentFile;
				contentFile2 = contentFile;
			}
			return contentFile2;
		}

		[NativeThrows]
		internal static void ContentFile_UnloadAsync(ContentFile handle)
		{
			ContentLoadInterface.ContentFile_UnloadAsync_Injected(ref handle);
		}

		internal static Object ContentFile_GetObject(ContentFile handle, ulong localIdentifierInFile)
		{
			return Unmarshal.UnmarshalUnityObject<Object>(ContentLoadInterface.ContentFile_GetObject_Injected(ref handle, localIdentifierInFile));
		}

		internal static Object[] ContentFile_GetObjects(ContentFile handle)
		{
			return ContentLoadInterface.ContentFile_GetObjects_Injected(ref handle);
		}

		internal static LoadingStatus ContentFile_GetLoadingStatus(ContentFile handle)
		{
			return ContentLoadInterface.ContentFile_GetLoadingStatus_Injected(ref handle);
		}

		internal static bool ContentFile_IsHandleValid(ContentFile handle)
		{
			return ContentLoadInterface.ContentFile_IsHandleValid_Injected(ref handle);
		}

		internal static extern float IntegrationTimeMS
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static bool WaitForLoadCompletion(ContentFile handle, int timeoutMs)
		{
			return ContentLoadInterface.WaitForLoadCompletion_Injected(ref handle, timeoutMs);
		}

		internal static bool WaitForUnloadCompletion(ContentFile handle, int timeoutMs)
		{
			return ContentLoadInterface.WaitForUnloadCompletion_Injected(ref handle, timeoutMs);
		}

		internal static bool ContentFile_IsUnloadComplete(ContentFile handle)
		{
			return ContentLoadInterface.ContentFile_IsUnloadComplete_Injected(ref handle);
		}

		[NativeThrows]
		internal unsafe static ContentSceneFile LoadSceneAsync(ContentNamespace nameSpace, string filename, string sceneName, ContentSceneParameters sceneParams, ContentFile* dependencies, int dependencyCount, JobHandle dependentFence)
		{
			ContentSceneFile contentSceneFile2;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(filename, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = filename.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(sceneName, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = sceneName.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				ContentSceneFile contentSceneFile;
				ContentLoadInterface.LoadSceneAsync_Injected(ref nameSpace, ref managedSpanWrapper, ref managedSpanWrapper2, ref sceneParams, dependencies, dependencyCount, ref dependentFence, out contentSceneFile);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
				ContentSceneFile contentSceneFile;
				contentSceneFile2 = contentSceneFile;
			}
			return contentSceneFile2;
		}

		internal static Scene ContentSceneFile_GetScene(ContentSceneFile handle)
		{
			Scene scene;
			ContentLoadInterface.ContentSceneFile_GetScene_Injected(ref handle, out scene);
			return scene;
		}

		internal static SceneLoadingStatus ContentSceneFile_GetStatus(ContentSceneFile handle)
		{
			return ContentLoadInterface.ContentSceneFile_GetStatus_Injected(ref handle);
		}

		[NativeThrows]
		internal static void ContentSceneFile_IntegrateAtEndOfFrame(ContentSceneFile handle)
		{
			ContentLoadInterface.ContentSceneFile_IntegrateAtEndOfFrame_Injected(ref handle);
		}

		internal static bool ContentSceneFile_UnloadAtEndOfFrame(ContentSceneFile handle)
		{
			return ContentLoadInterface.ContentSceneFile_UnloadAtEndOfFrame_Injected(ref handle);
		}

		internal static bool ContentSceneFile_IsHandleValid(ContentSceneFile handle)
		{
			return ContentLoadInterface.ContentSceneFile_IsHandleValid_Injected(ref handle);
		}

		internal static bool ContentSceneFile_WaitForCompletion(ContentSceneFile handle, int timeoutMs)
		{
			return ContentLoadInterface.ContentSceneFile_WaitForCompletion_Injected(ref handle, timeoutMs);
		}

		public unsafe static ContentSceneFile LoadSceneAsync(ContentNamespace nameSpace, string filename, string sceneName, ContentSceneParameters sceneParams, NativeArray<ContentFile> dependencies, JobHandle dependentFence = default(JobHandle))
		{
			return ContentLoadInterface.LoadSceneAsync(nameSpace, filename, sceneName, sceneParams, (ContentFile*)dependencies.m_Buffer, dependencies.Length, dependentFence);
		}

		public static ContentFile LoadContentFileAsync(ContentNamespace nameSpace, string filename, NativeArray<ContentFile> dependencies, JobHandle dependentFence = default(JobHandle))
		{
			return ContentLoadInterface.LoadContentFileAsync(nameSpace, filename, dependencies.m_Buffer, dependencies.Length, dependentFence, false);
		}

		public static ContentFile[] GetContentFiles(ContentNamespace nameSpace)
		{
			ContentFile[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ContentLoadInterface.GetContentFiles_Injected(ref nameSpace, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ContentFile[] array;
				blittableArrayWrapper.Unmarshal<ContentFile>(ref array);
				array2 = array;
			}
			return array2;
		}

		public static ContentSceneFile[] GetSceneFiles(ContentNamespace nameSpace)
		{
			ContentSceneFile[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ContentLoadInterface.GetSceneFiles_Injected(ref nameSpace, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ContentSceneFile[] array;
				blittableArrayWrapper.Unmarshal<ContentSceneFile>(ref array);
				array2 = array;
			}
			return array2;
		}

		public static float GetIntegrationTimeMS()
		{
			return ContentLoadInterface.IntegrationTimeMS;
		}

		public static void SetIntegrationTimeMS(float integrationTimeMS)
		{
			bool flag = integrationTimeMS <= 0f;
			if (flag)
			{
				throw new ArgumentOutOfRangeException("integrationTimeMS", "integrationTimeMS was out of range. Must be greater than zero.");
			}
			ContentLoadInterface.IntegrationTimeMS = integrationTimeMS;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void LoadContentFileAsync_Injected([In] ref ContentNamespace nameSpace, ref ManagedSpanWrapper filename, void* dependencies, int dependencyCount, [In] ref JobHandle dependentFence, bool useUnsafe, out ContentFile ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ContentFile_UnloadAsync_Injected([In] ref ContentFile handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr ContentFile_GetObject_Injected([In] ref ContentFile handle, ulong localIdentifierInFile);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object[] ContentFile_GetObjects_Injected([In] ref ContentFile handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LoadingStatus ContentFile_GetLoadingStatus_Injected([In] ref ContentFile handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ContentFile_IsHandleValid_Injected([In] ref ContentFile handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool WaitForLoadCompletion_Injected([In] ref ContentFile handle, int timeoutMs);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool WaitForUnloadCompletion_Injected([In] ref ContentFile handle, int timeoutMs);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ContentFile_IsUnloadComplete_Injected([In] ref ContentFile handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void LoadSceneAsync_Injected([In] ref ContentNamespace nameSpace, ref ManagedSpanWrapper filename, ref ManagedSpanWrapper sceneName, [In] ref ContentSceneParameters sceneParams, ContentFile* dependencies, int dependencyCount, [In] ref JobHandle dependentFence, out ContentSceneFile ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ContentSceneFile_GetScene_Injected([In] ref ContentSceneFile handle, out Scene ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SceneLoadingStatus ContentSceneFile_GetStatus_Injected([In] ref ContentSceneFile handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ContentSceneFile_IntegrateAtEndOfFrame_Injected([In] ref ContentSceneFile handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ContentSceneFile_UnloadAtEndOfFrame_Injected([In] ref ContentSceneFile handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ContentSceneFile_IsHandleValid_Injected([In] ref ContentSceneFile handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ContentSceneFile_WaitForCompletion_Injected([In] ref ContentSceneFile handle, int timeoutMs);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetContentFiles_Injected([In] ref ContentNamespace nameSpace, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSceneFiles_Injected([In] ref ContentNamespace nameSpace, out BlittableArrayWrapper ret);
	}
}
