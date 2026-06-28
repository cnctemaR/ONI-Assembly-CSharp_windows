using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using UnityEngine.Internal;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public class Object
	{
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object Internal_CloneSingle(Object data);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object Internal_CloneSingleWithParent(Object data, Transform parent, bool worldPositionStays);

		[ThreadAndSerializationSafe]
		private static Object Internal_InstantiateSingle(Object data, Vector3 pos, Quaternion rot)
		{
			return Object.INTERNAL_CALL_Internal_InstantiateSingle(data, ref pos, ref rot);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object INTERNAL_CALL_Internal_InstantiateSingle(Object data, ref Vector3 pos, ref Quaternion rot);

		private static Object Internal_InstantiateSingleWithParent(Object data, Transform parent, Vector3 pos, Quaternion rot)
		{
			return Object.INTERNAL_CALL_Internal_InstantiateSingleWithParent(data, parent, ref pos, ref rot);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object INTERNAL_CALL_Internal_InstantiateSingleWithParent(Object data, Transform parent, ref Vector3 pos, ref Quaternion rot);

		[ThreadAndSerializationSafe]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetOffsetOfInstanceIDInCPlusPlusObject();

		[ThreadAndSerializationSafe]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void EnsureRunningOnMainThread();

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Destroy(Object obj, [DefaultValue("0.0F")] float t);

		[ExcludeFromDocs]
		public static void Destroy(Object obj)
		{
			float num = 0f;
			Object.Destroy(obj, num);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DestroyImmediate(Object obj, [DefaultValue("false")] bool allowDestroyingAssets);

		[ExcludeFromDocs]
		public static void DestroyImmediate(Object obj)
		{
			bool flag = false;
			Object.DestroyImmediate(obj, flag);
		}

		[TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Object[] FindObjectsOfType(Type type);

		public extern string name
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DontDestroyOnLoad(Object target);

		public extern HideFlags hideFlags
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DestroyObject(Object obj, [DefaultValue("0.0F")] float t);

		[ExcludeFromDocs]
		public static void DestroyObject(Object obj)
		{
			float num = 0f;
			Object.DestroyObject(obj, num);
		}

		[Obsolete("use Object.FindObjectsOfType instead.")]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Object[] FindSceneObjectsOfType(Type type);

		[Obsolete("use Resources.FindObjectsOfTypeAll instead.")]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Object[] FindObjectsOfTypeIncludingAssets(Type type);

		[Obsolete("Please use Resources.FindObjectsOfTypeAll instead")]
		public static Object[] FindObjectsOfTypeAll(Type type)
		{
			return Resources.FindObjectsOfTypeAll(type);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public override extern string ToString();

		[ThreadAndSerializationSafe]
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool DoesObjectWithInstanceIDExist(int instanceID);

		[SecuritySafeCritical]
		public unsafe int GetInstanceID()
		{
			int num;
			if (this.m_CachedPtr == IntPtr.Zero)
			{
				num = 0;
			}
			else
			{
				if (Object.OffsetOfInstanceIDInCPlusPlusObject == -1)
				{
					Object.OffsetOfInstanceIDInCPlusPlusObject = Object.GetOffsetOfInstanceIDInCPlusPlusObject();
				}
				num = *(int*)(void*)new IntPtr(this.m_CachedPtr.ToInt64() + (long)Object.OffsetOfInstanceIDInCPlusPlusObject);
			}
			return num;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object other)
		{
			Object @object = other as Object;
			return (!(@object == null) || other == null || other is Object) && Object.CompareBaseObjects(this, @object);
		}

		public static implicit operator bool(Object exists)
		{
			return !Object.CompareBaseObjects(exists, null);
		}

		private static bool CompareBaseObjects(Object lhs, Object rhs)
		{
			bool flag = lhs == null;
			bool flag2 = rhs == null;
			bool flag3;
			if (flag2 && flag)
			{
				flag3 = true;
			}
			else if (flag2)
			{
				flag3 = !Object.IsNativeObjectAlive(lhs);
			}
			else if (flag)
			{
				flag3 = !Object.IsNativeObjectAlive(rhs);
			}
			else
			{
				flag3 = object.ReferenceEquals(lhs, rhs);
			}
			return flag3;
		}

		private static bool IsNativeObjectAlive(Object o)
		{
			return o.GetCachedPtr() != IntPtr.Zero;
		}

		private IntPtr GetCachedPtr()
		{
			return this.m_CachedPtr;
		}

		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object original, Vector3 position, Quaternion rotation)
		{
			Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
			if (original is ScriptableObject)
			{
				throw new ArgumentException("Cannot instantiate a ScriptableObject with a position and rotation");
			}
			return Object.Internal_InstantiateSingle(original, position, rotation);
		}

		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent)
		{
			Object @object;
			if (parent == null)
			{
				@object = Object.Internal_InstantiateSingle(original, position, rotation);
			}
			else
			{
				Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
				@object = Object.Internal_InstantiateSingleWithParent(original, parent, position, rotation);
			}
			return @object;
		}

		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object original)
		{
			Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
			return Object.Internal_CloneSingle(original);
		}

		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object original, Transform parent)
		{
			return Object.Instantiate(original, parent, false);
		}

		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object original, Transform parent, bool instantiateInWorldSpace)
		{
			Object @object;
			if (parent == null)
			{
				@object = Object.Internal_CloneSingle(original);
			}
			else
			{
				Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
				@object = Object.Internal_CloneSingleWithParent(original, parent, instantiateInWorldSpace);
			}
			return @object;
		}

		public static T Instantiate<T>(T original) where T : Object
		{
			Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
			return (T)((object)Object.Internal_CloneSingle(original));
		}

		public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object
		{
			return (T)((object)Object.Instantiate(original, position, rotation));
		}

		public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object
		{
			return (T)((object)Object.Instantiate(original, position, rotation, parent));
		}

		public static T Instantiate<T>(T original, Transform parent) where T : Object
		{
			return Object.Instantiate<T>(original, parent, false);
		}

		public static T Instantiate<T>(T original, Transform parent, bool worldPositionStays) where T : Object
		{
			return (T)((object)Object.Instantiate(original, parent, worldPositionStays));
		}

		public static T[] FindObjectsOfType<T>() where T : Object
		{
			return Resources.ConvertObjects<T>(Object.FindObjectsOfType(typeof(T)));
		}

		public static T FindObjectOfType<T>() where T : Object
		{
			return (T)((object)Object.FindObjectOfType(typeof(T)));
		}

		private static void CheckNullArgument(object arg, string message)
		{
			if (arg == null)
			{
				throw new ArgumentException(message);
			}
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public static Object FindObjectOfType(Type type)
		{
			Object[] array = Object.FindObjectsOfType(type);
			Object @object;
			if (array.Length > 0)
			{
				@object = array[0];
			}
			else
			{
				@object = null;
			}
			return @object;
		}

		public static bool operator ==(Object x, Object y)
		{
			return Object.CompareBaseObjects(x, y);
		}

		public static bool operator !=(Object x, Object y)
		{
			return !Object.CompareBaseObjects(x, y);
		}

		private IntPtr m_CachedPtr;

		internal static int OffsetOfInstanceIDInCPlusPlusObject = -1;
	}
}
