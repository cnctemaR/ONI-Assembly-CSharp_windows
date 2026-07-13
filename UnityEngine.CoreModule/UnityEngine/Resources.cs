using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Assertions;
using UnityEngine.Bindings;
using UnityEngineInternal;

namespace UnityEngine
{
	[NativeHeader("Runtime/Misc/ResourceManagerUtility.h")]
	[NativeHeader("Runtime/Export/Resources/Resources.bindings.h")]
	public sealed class Resources
	{
		internal static T[] ConvertObjects<T>(Object[] rawObjects) where T : Object
		{
			bool flag = rawObjects == null;
			T[] array;
			if (flag)
			{
				array = null;
			}
			else
			{
				T[] array2 = new T[rawObjects.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					array2[i] = (T)((object)rawObjects[i]);
				}
				array = array2;
			}
			return array;
		}

		public static Object[] FindObjectsOfTypeAll(Type type)
		{
			return ResourcesAPI.ActiveAPI.FindObjectsOfTypeAll(type);
		}

		public static T[] FindObjectsOfTypeAll<T>() where T : Object
		{
			return Resources.ConvertObjects<T>(Resources.FindObjectsOfTypeAll(typeof(T)));
		}

		public static Object Load(string path)
		{
			return Resources.Load(path, typeof(Object));
		}

		public static T Load<T>(string path) where T : Object
		{
			return (T)((object)Resources.Load(path, typeof(T)));
		}

		public static Object Load(string path, Type systemTypeInstance)
		{
			return ResourcesAPI.ActiveAPI.Load(path, systemTypeInstance);
		}

		public static ResourceRequest LoadAsync(string path)
		{
			return Resources.LoadAsync(path, typeof(Object));
		}

		public static ResourceRequest LoadAsync<T>(string path) where T : Object
		{
			return Resources.LoadAsync(path, typeof(T));
		}

		public static ResourceRequest LoadAsync(string path, Type type)
		{
			return ResourcesAPI.ActiveAPI.LoadAsync(path, type);
		}

		public static Object[] LoadAll(string path, Type systemTypeInstance)
		{
			return ResourcesAPI.ActiveAPI.LoadAll(path, systemTypeInstance);
		}

		public static Object[] LoadAll(string path)
		{
			return Resources.LoadAll(path, typeof(Object));
		}

		public static T[] LoadAll<T>(string path) where T : Object
		{
			return Resources.ConvertObjects<T>(Resources.LoadAll(path, typeof(T)));
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		[FreeFunction("GetScriptingBuiltinResource", ThrowsException = true)]
		public unsafe static Object GetBuiltinResource([NotNull] Type type, string path)
		{
			if (type == null)
			{
				ThrowHelper.ThrowArgumentNullException(type, "type");
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
				IntPtr builtinResource_Injected = Resources.GetBuiltinResource_Injected(type, ref managedSpanWrapper);
			}
			finally
			{
				IntPtr builtinResource_Injected;
				@object = Unmarshal.UnmarshalUnityObject<Object>(builtinResource_Injected);
				char* ptr = null;
			}
			return @object;
		}

		public static T GetBuiltinResource<T>(string path) where T : Object
		{
			return (T)((object)Resources.GetBuiltinResource(typeof(T), path));
		}

		public static void UnloadAsset(Object assetToUnload)
		{
			ResourcesAPI.ActiveAPI.UnloadAsset(assetToUnload);
		}

		[FreeFunction("Scripting::UnloadAssetFromScripting")]
		private static void UnloadAssetImplResourceManager(Object assetToUnload)
		{
			Resources.UnloadAssetImplResourceManager_Injected(Object.MarshalledUnityObject.Marshal<Object>(assetToUnload));
		}

		[FreeFunction("Resources_Bindings::UnloadUnusedAssets")]
		public static AsyncOperation UnloadUnusedAssets()
		{
			IntPtr intPtr = Resources.UnloadUnusedAssets_Injected();
			return (intPtr == 0) ? null : AsyncOperation.BindingsMarshaller.ConvertToManaged(intPtr);
		}

		[FreeFunction("Resources_Bindings::InstanceIDToObject")]
		public static Object EntityIdToObject(EntityId entityId)
		{
			return Unmarshal.UnmarshalUnityObject<Object>(Resources.EntityIdToObject_Injected(ref entityId));
		}

		[Obsolete("InstanceIDToObject is obsolete. Use EntityIdToObject instead.")]
		public static Object InstanceIDToObject(int instanceID)
		{
			return Resources.EntityIdToObject(instanceID);
		}

		[FreeFunction("Resources_Bindings::IsInstanceLoaded")]
		internal static bool IsObjectLoaded(EntityId entityId)
		{
			return Resources.IsObjectLoaded_Injected(ref entityId);
		}

		internal static bool IsInstanceLoaded(int instanceID)
		{
			return Resources.IsObjectLoaded(instanceID);
		}

		[FreeFunction("Resources_Bindings::InstanceIDToObjectList", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EntityIdsToObjectList(IntPtr entityIds, int instanceCount, List<Object> objects);

		public static void EntityIdsToObjectList(NativeArray<EntityId> entityIds, List<Object> objects)
		{
			bool flag = !entityIds.IsCreated;
			if (flag)
			{
				throw new ArgumentException("NativeArray is uninitialized", "entityIds");
			}
			bool flag2 = objects == null;
			if (flag2)
			{
				throw new ArgumentNullException("objects");
			}
			bool flag3 = entityIds.Length == 0;
			if (flag3)
			{
				objects.Clear();
			}
			else
			{
				Resources.EntityIdsToObjectList((IntPtr)entityIds.GetUnsafeReadOnlyPtr<EntityId>(), entityIds.Length, objects);
			}
		}

		[Obsolete("InstanceIDToObjectList is obsolete. Use EntityIdsToObjectList instead.")]
		public unsafe static void InstanceIDToObjectList(NativeArray<int> instanceIDs, List<Object> objects)
		{
			Debug.Assert(4 == sizeof(EntityId), "Update this path to 64bit when we support 64bit");
			bool flag = !instanceIDs.IsCreated;
			if (flag)
			{
				throw new ArgumentException("NativeArray is uninitialized", "instanceIDs");
			}
			bool flag2 = objects == null;
			if (flag2)
			{
				throw new ArgumentNullException("objects");
			}
			bool flag3 = instanceIDs.Length == 0;
			if (flag3)
			{
				objects.Clear();
			}
			else
			{
				Resources.EntityIdsToObjectList((IntPtr)instanceIDs.GetUnsafeReadOnlyPtr<int>(), instanceIDs.Length, objects);
			}
		}

		[FreeFunction("Resources_Bindings::InstanceIDsToValidArray", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InstanceIDsToValidArray_Internal(IntPtr instanceIDs, int instanceCount, IntPtr validArray, int validArrayCount);

		[FreeFunction("Resources_Bindings::DoesObjectWithInstanceIDExist", IsThreadSafe = true)]
		public static bool EntityIdIsValid(EntityId entityId)
		{
			return Resources.EntityIdIsValid_Injected(ref entityId);
		}

		[Obsolete("InstanceIDIsValid is obsolete. Use EntityIdIsValid instead.")]
		public static bool InstanceIDIsValid(int instanceId)
		{
			return Resources.EntityIdIsValid(instanceId);
		}

		[Obsolete("InstanceIDsToValidArray is obsolete. Use EntityIdsToValidArray instead.")]
		public unsafe static void InstanceIDsToValidArray(NativeArray<int> instanceIDs, NativeArray<bool> validArray)
		{
			bool flag = !instanceIDs.IsCreated;
			if (flag)
			{
				throw new ArgumentException("NativeArray is uninitialized", "instanceIDs");
			}
			bool flag2 = !validArray.IsCreated;
			if (flag2)
			{
				throw new ArgumentException("NativeArray is uninitialized", "validArray");
			}
			bool flag3 = instanceIDs.Length != validArray.Length;
			if (flag3)
			{
				throw new ArgumentException("Size mismatch! Both arrays must be the same length.");
			}
			bool flag4 = instanceIDs.Length == 0;
			if (!flag4)
			{
				Assert.AreEqual(4, sizeof(EntityId));
				Resources.InstanceIDsToValidArray_Internal((IntPtr)instanceIDs.GetUnsafeReadOnlyPtr<int>(), instanceIDs.Length, (IntPtr)validArray.GetUnsafePtr<bool>(), validArray.Length);
			}
		}

		public static void EntityIdsToValidArray(NativeArray<EntityId> entityIDs, NativeArray<bool> validArray)
		{
			bool flag = !entityIDs.IsCreated;
			if (flag)
			{
				throw new ArgumentException("NativeArray is uninitialized", "entityIDs");
			}
			bool flag2 = !validArray.IsCreated;
			if (flag2)
			{
				throw new ArgumentException("NativeArray is uninitialized", "validArray");
			}
			bool flag3 = entityIDs.Length != validArray.Length;
			if (flag3)
			{
				throw new ArgumentException("Size mismatch! Both arrays must be the same length.");
			}
			bool flag4 = entityIDs.Length == 0;
			if (!flag4)
			{
				Resources.InstanceIDsToValidArray_Internal((IntPtr)entityIDs.GetUnsafeReadOnlyPtr<EntityId>(), entityIDs.Length, (IntPtr)validArray.GetUnsafePtr<bool>(), validArray.Length);
			}
		}

		public unsafe static void InstanceIDsToValidArray(ReadOnlySpan<int> instanceIDs, Span<bool> validArray)
		{
			bool flag = instanceIDs.Length != validArray.Length;
			if (flag)
			{
				throw new ArgumentException("Size mismatch! Both arrays must be the same length.");
			}
			bool flag2 = instanceIDs.Length == 0;
			if (!flag2)
			{
				Assert.AreEqual(4, sizeof(EntityId));
				fixed (int* pinnableReference = instanceIDs.GetPinnableReference())
				{
					int* ptr = pinnableReference;
					fixed (bool* pinnableReference2 = validArray.GetPinnableReference())
					{
						bool* ptr2 = pinnableReference2;
						Resources.InstanceIDsToValidArray_Internal((IntPtr)((void*)ptr), instanceIDs.Length, (IntPtr)((void*)ptr2), validArray.Length);
					}
				}
			}
		}

		public unsafe static void EntityIdsToValidArray(ReadOnlySpan<EntityId> entityIds, Span<bool> validArray)
		{
			bool flag = entityIds.Length != validArray.Length;
			if (flag)
			{
				throw new ArgumentException("Size mismatch! Both arrays must be the same length.");
			}
			bool flag2 = entityIds.Length == 0;
			if (!flag2)
			{
				fixed (EntityId* pinnableReference = entityIds.GetPinnableReference())
				{
					EntityId* ptr = pinnableReference;
					fixed (bool* pinnableReference2 = validArray.GetPinnableReference())
					{
						bool* ptr2 = pinnableReference2;
						Resources.InstanceIDsToValidArray_Internal((IntPtr)((void*)ptr), entityIds.Length, (IntPtr)((void*)ptr2), validArray.Length);
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetBuiltinResource_Injected(Type type, ref ManagedSpanWrapper path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UnloadAssetImplResourceManager_Injected(IntPtr assetToUnload);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr UnloadUnusedAssets_Injected();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr EntityIdToObject_Injected([In] ref EntityId entityId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsObjectLoaded_Injected([In] ref EntityId entityId);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool EntityIdIsValid_Injected([In] ref EntityId entityId);
	}
}
