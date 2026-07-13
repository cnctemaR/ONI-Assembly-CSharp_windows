using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Resources/Resources.bindings.h")]
	[NativeHeader("Runtime/Misc/ResourceManagerUtility.h")]
	internal static class ResourcesAPIInternal
	{
		[TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
		[FreeFunction("Resources_Bindings::FindObjectsOfTypeAll")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public static extern Object[] FindObjectsOfTypeAll(Type type);

		[FreeFunction("GetShaderNameRegistry().FindShader")]
		public unsafe static Shader FindShaderByName(string name)
		{
			Shader shader;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr = ResourcesAPIInternal.FindShaderByName_Injected(ref managedSpanWrapper);
			}
			finally
			{
				IntPtr intPtr;
				shader = Unmarshal.UnmarshalUnityObject<Shader>(intPtr);
				char* ptr = null;
			}
			return shader;
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedBySecondArgument)]
		[FreeFunction("Resources_Bindings::Load")]
		[NativeThrows]
		public unsafe static Object Load(string path, [NotNull] Type systemTypeInstance)
		{
			if (systemTypeInstance == null)
			{
				ThrowHelper.ThrowArgumentNullException(systemTypeInstance, "systemTypeInstance");
			}
			Object @object;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = path.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr = ResourcesAPIInternal.Load_Injected(ref managedSpanWrapper, systemTypeInstance);
			}
			finally
			{
				IntPtr intPtr;
				@object = Unmarshal.UnmarshalUnityObject<Object>(intPtr);
				char* ptr = null;
			}
			return @object;
		}

		[FreeFunction("Resources_Bindings::LoadAll")]
		[NativeThrows]
		public unsafe static Object[] LoadAll([NotNull] string path, [NotNull] Type systemTypeInstance)
		{
			if (path == null)
			{
				ThrowHelper.ThrowArgumentNullException(path, "path");
			}
			if (systemTypeInstance == null)
			{
				ThrowHelper.ThrowArgumentNullException(systemTypeInstance, "systemTypeInstance");
			}
			Object[] array;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = path.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				array = ResourcesAPIInternal.LoadAll_Injected(ref managedSpanWrapper, systemTypeInstance);
			}
			finally
			{
				char* ptr = null;
			}
			return array;
		}

		[FreeFunction("Resources_Bindings::LoadAsyncInternal")]
		internal unsafe static ResourceRequest LoadAsyncInternal(string path, Type type)
		{
			ResourceRequest resourceRequest;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = path.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr = ResourcesAPIInternal.LoadAsyncInternal_Injected(ref managedSpanWrapper, type);
			}
			finally
			{
				IntPtr intPtr;
				IntPtr intPtr2 = intPtr;
				resourceRequest = ((intPtr2 == 0) ? null : ResourceRequest.BindingsMarshaller.ConvertToManaged(intPtr2));
				char* ptr = null;
			}
			return resourceRequest;
		}

		[FreeFunction("Scripting::UnloadAssetFromScripting")]
		public static void UnloadAsset(Object assetToUnload)
		{
			ResourcesAPIInternal.UnloadAsset_Injected(Object.MarshalledUnityObject.Marshal<Object>(assetToUnload));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr FindShaderByName_Injected(ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Load_Injected(ref ManagedSpanWrapper path, Type systemTypeInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object[] LoadAll_Injected(ref ManagedSpanWrapper path, Type systemTypeInstance);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr LoadAsyncInternal_Injected(ref ManagedSpanWrapper path, Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UnloadAsset_Injected(IntPtr assetToUnload);

		internal static class EntitiesAssetGC
		{
			[FreeFunction("Resources_Bindings::MarkInstanceIDsAsRoot")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void MarkInstanceIDsAsRoot(IntPtr instanceIDs, int count, IntPtr state);

			[FreeFunction("Resources_Bindings::EnableEntitiesAssetGCCallback")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			internal static extern void EnableEntitiesAssetGCCallback();

			internal static void RegisterAdditionalRootsHandler(ResourcesAPIInternal.EntitiesAssetGC.AdditionalRootsHandlerDelegate newAdditionalRootsHandler)
			{
				bool flag = ResourcesAPIInternal.EntitiesAssetGC.AdditionalRootsHandler == null;
				if (flag)
				{
					ResourcesAPIInternal.EntitiesAssetGC.EnableEntitiesAssetGCCallback();
					ResourcesAPIInternal.EntitiesAssetGC.AdditionalRootsHandler = newAdditionalRootsHandler;
				}
				else
				{
					Debug.LogWarning("Attempting to register more than one AdditionalRootsHandlerDelegate! Only one may be registered at a time.");
				}
			}

			[UsedByNativeCode]
			private static void GetAdditionalRoots(IntPtr state)
			{
				bool flag = ResourcesAPIInternal.EntitiesAssetGC.AdditionalRootsHandler != null;
				if (flag)
				{
					ResourcesAPIInternal.EntitiesAssetGC.AdditionalRootsHandler(state);
				}
			}

			internal static ResourcesAPIInternal.EntitiesAssetGC.AdditionalRootsHandlerDelegate AdditionalRootsHandler;

			internal delegate void AdditionalRootsHandlerDelegate(IntPtr state);
		}
	}
}
