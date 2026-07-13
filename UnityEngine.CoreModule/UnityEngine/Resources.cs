using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
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
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Object GetBuiltinResource([NotNull("ArgumentNullException")] Type type, string path);

		public static T GetBuiltinResource<T>(string path) where T : Object
		{
			return (T)((object)Resources.GetBuiltinResource(typeof(T), path));
		}

		public static void UnloadAsset(Object assetToUnload)
		{
			ResourcesAPI.ActiveAPI.UnloadAsset(assetToUnload);
		}

		[FreeFunction("Scripting::UnloadAssetFromScripting")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void UnloadAssetImplResourceManager(Object assetToUnload);

		[FreeFunction("Resources_Bindings::UnloadUnusedAssets")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern AsyncOperation UnloadUnusedAssets();

		[FreeFunction("Resources_Bindings::InstanceIDToObject")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Object InstanceIDToObject(int instanceID);

		[FreeFunction("Resources_Bindings::InstanceIDToObjectList", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InstanceIDToObjectList(IntPtr instanceIDs, int instanceCount, List<Object> objects);

		public static void InstanceIDToObjectList(NativeArray<int> instanceIDs, List<Object> objects)
		{
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
				Resources.InstanceIDToObjectList((IntPtr)instanceIDs.GetUnsafeReadOnlyPtr<int>(), instanceIDs.Length, objects);
			}
		}

		[FreeFunction("Resources_Bindings::InstanceIDsToValidArray", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InstanceIDsToValidArray_Internal(IntPtr instanceIDs, int instanceCount, IntPtr validArray, int validArrayCount);

		[FreeFunction("Resources_Bindings::DoesObjectWithInstanceIDExist", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool InstanceIDIsValid(int instanceId);

		public static void InstanceIDsToValidArray(NativeArray<int> instanceIDs, NativeArray<bool> validArray)
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
				Resources.InstanceIDsToValidArray_Internal((IntPtr)instanceIDs.GetUnsafeReadOnlyPtr<int>(), instanceIDs.Length, (IntPtr)validArray.GetUnsafePtr<bool>(), validArray.Length);
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
	}
}
