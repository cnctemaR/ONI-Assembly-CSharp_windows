using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Content;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.SceneManagement;

namespace Unity.Loading
{
	[NativeHeader("Modules/ContentLoad/Public/ContentLoadFrontend.h")]
	[StaticAccessor("GetContentLoadFrontend()", StaticAccessorType.Dot)]
	public static class ContentLoadInterface
	{
		[NativeThrows]
		internal unsafe static ContentFile LoadContentFileAsync(ContentNamespace nameSpace, string filename, void* dependencies, int dependencyCount, JobHandle dependentFence, bool useUnsafe = false)
		{
			ContentFile contentFile;
			ContentLoadInterface.LoadContentFileAsync_Injected(ref nameSpace, filename, dependencies, dependencyCount, ref dependentFence, useUnsafe, out contentFile);
			return contentFile;
		}

		[NativeThrows]
		internal static ContentFileUnloadHandle ContentFile_UnloadAsync(ContentFile handle)
		{
			ContentFileUnloadHandle contentFileUnloadHandle;
			ContentLoadInterface.ContentFile_UnloadAsync_Injected(ref handle, out contentFileUnloadHandle);
			return contentFileUnloadHandle;
		}

		internal static Object ContentFile_GetObject(ContentFile handle, ulong localIdentifierInFile)
		{
			return ContentLoadInterface.ContentFile_GetObject_Injected(ref handle, localIdentifierInFile);
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

		internal static bool WaitForJobCompletion(JobHandle handle, int timeoutMs)
		{
			return ContentLoadInterface.WaitForJobCompletion_Injected(ref handle, timeoutMs);
		}

		[NativeThrows]
		internal unsafe static ContentSceneFile LoadSceneAsync(ContentNamespace nameSpace, string filename, string sceneName, ContentSceneParameters sceneParams, ContentFile* dependencies, int dependencyCount, JobHandle dependentFence)
		{
			ContentSceneFile contentSceneFile;
			ContentLoadInterface.LoadSceneAsync_Injected(ref nameSpace, filename, sceneName, ref sceneParams, dependencies, dependencyCount, ref dependentFence, out contentSceneFile);
			return contentSceneFile;
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
			return ContentLoadInterface.GetContentFiles_Injected(ref nameSpace);
		}

		public static ContentSceneFile[] GetSceneFiles(ContentNamespace nameSpace)
		{
			return ContentLoadInterface.GetSceneFiles_Injected(ref nameSpace);
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
		private unsafe static extern void LoadContentFileAsync_Injected(ref ContentNamespace nameSpace, string filename, void* dependencies, int dependencyCount, ref JobHandle dependentFence, bool useUnsafe = false, out ContentFile ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ContentFile_UnloadAsync_Injected(ref ContentFile handle, out ContentFileUnloadHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object ContentFile_GetObject_Injected(ref ContentFile handle, ulong localIdentifierInFile);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object[] ContentFile_GetObjects_Injected(ref ContentFile handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern LoadingStatus ContentFile_GetLoadingStatus_Injected(ref ContentFile handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ContentFile_IsHandleValid_Injected(ref ContentFile handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool WaitForLoadCompletion_Injected(ref ContentFile handle, int timeoutMs);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool WaitForJobCompletion_Injected(ref JobHandle handle, int timeoutMs);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void LoadSceneAsync_Injected(ref ContentNamespace nameSpace, string filename, string sceneName, ref ContentSceneParameters sceneParams, ContentFile* dependencies, int dependencyCount, ref JobHandle dependentFence, out ContentSceneFile ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ContentSceneFile_GetScene_Injected(ref ContentSceneFile handle, out Scene ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern SceneLoadingStatus ContentSceneFile_GetStatus_Injected(ref ContentSceneFile handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ContentSceneFile_IntegrateAtEndOfFrame_Injected(ref ContentSceneFile handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ContentSceneFile_UnloadAtEndOfFrame_Injected(ref ContentSceneFile handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ContentSceneFile_IsHandleValid_Injected(ref ContentSceneFile handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool ContentSceneFile_WaitForCompletion_Injected(ref ContentSceneFile handle, int timeoutMs);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ContentFile[] GetContentFiles_Injected(ref ContentNamespace nameSpace);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ContentSceneFile[] GetSceneFiles_Injected(ref ContentNamespace nameSpace);
	}
}
