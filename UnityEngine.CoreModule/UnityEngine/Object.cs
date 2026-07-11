using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine
{
	[RequiredByNativeCode(GenerateProxy = true)]
	[NativeHeader("Runtime/Export/UnityEngineObject.bindings.h")]
	[NativeHeader("Runtime/GameCode/CloneObject.h")]
	[NativeHeader("Runtime/SceneManager/SceneManager.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class Object
	{
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

		private void EnsureRunningOnMainThread()
		{
			if (!Object.CurrentThreadIsMainThread())
			{
				throw new InvalidOperationException("EnsureRunningOnMainThread can only be called from the main thread");
			}
		}

		private static bool IsNativeObjectAlive(Object o)
		{
			return o.GetCachedPtr() != IntPtr.Zero;
		}

		private IntPtr GetCachedPtr()
		{
			return this.m_CachedPtr;
		}

		public string name
		{
			get
			{
				return Object.GetName(this);
			}
			set
			{
				Object.SetName(this, value);
			}
		}

		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object original, Vector3 position, Quaternion rotation)
		{
			Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
			if (original is ScriptableObject)
			{
				throw new ArgumentException("Cannot instantiate a ScriptableObject with a position and rotation");
			}
			Object @object = Object.Internal_InstantiateSingle(original, position, rotation);
			if (@object == null)
			{
				throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
			}
			return @object;
		}

		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent)
		{
			Object @object;
			if (parent == null)
			{
				@object = Object.Instantiate(original, position, rotation);
			}
			else
			{
				Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
				Object object2 = Object.Internal_InstantiateSingleWithParent(original, parent, position, rotation);
				if (object2 == null)
				{
					throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
				}
				@object = object2;
			}
			return @object;
		}

		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object original)
		{
			Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
			Object @object = Object.Internal_CloneSingle(original);
			if (@object == null)
			{
				throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
			}
			return @object;
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
				@object = Object.Instantiate(original);
			}
			else
			{
				Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
				Object object2 = Object.Internal_CloneSingleWithParent(original, parent, instantiateInWorldSpace);
				if (object2 == null)
				{
					throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
				}
				@object = object2;
			}
			return @object;
		}

		public static T Instantiate<T>(T original) where T : Object
		{
			Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
			T t = (T)((object)Object.Internal_CloneSingle(original));
			if (t == null)
			{
				throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
			}
			return t;
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

		[NativeMethod(Name = "Scripting::DestroyObjectFromScripting", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Destroy(Object obj, [DefaultValue("0.0F")] float t);

		[ExcludeFromDocs]
		public static void Destroy(Object obj)
		{
			float num = 0f;
			Object.Destroy(obj, num);
		}

		[NativeMethod(Name = "Scripting::DestroyObjectFromScriptingImmediate", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DestroyImmediate(Object obj, [DefaultValue("false")] bool allowDestroyingAssets);

		[ExcludeFromDocs]
		public static void DestroyImmediate(Object obj)
		{
			bool flag = false;
			Object.DestroyImmediate(obj, flag);
		}

		[FreeFunction("UnityEngineObjectBindings::FindObjectsOfType")]
		[TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Object[] FindObjectsOfType(Type type);

		[FreeFunction("GetSceneManager().DontDestroyOnLoad")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DontDestroyOnLoad(Object target);

		public extern HideFlags hideFlags
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("use Object.Destroy instead.")]
		public static void DestroyObject(Object obj, [DefaultValue("0.0F")] float t)
		{
			Object.Destroy(obj, t);
		}

		[Obsolete("use Object.Destroy instead.")]
		[ExcludeFromDocs]
		public static void DestroyObject(Object obj)
		{
			float num = 0f;
			Object.Destroy(obj, num);
		}

		[FreeFunction("UnityEngineObjectBindings::FindObjectsOfType")]
		[Obsolete("warning use Object.FindObjectsOfType instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Object[] FindSceneObjectsOfType(Type type);

		[FreeFunction("UnityEngineObjectBindings::FindObjectsOfTypeIncludingAssets")]
		[Obsolete("use Resources.FindObjectsOfTypeAll instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Object[] FindObjectsOfTypeIncludingAssets(Type type);

		public static T[] FindObjectsOfType<T>() where T : Object
		{
			return Resources.ConvertObjects<T>(Object.FindObjectsOfType(typeof(T)));
		}

		public static T FindObjectOfType<T>() where T : Object
		{
			return (T)((object)Object.FindObjectOfType(typeof(T)));
		}

		[Obsolete("Please use Resources.FindObjectsOfTypeAll instead")]
		public static Object[] FindObjectsOfTypeAll(Type type)
		{
			return Resources.FindObjectsOfTypeAll(type);
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

		public override string ToString()
		{
			return Object.ToString(this);
		}

		public static bool operator ==(Object x, Object y)
		{
			return Object.CompareBaseObjects(x, y);
		}

		public static bool operator !=(Object x, Object y)
		{
			return !Object.CompareBaseObjects(x, y);
		}

		[NativeMethod(Name = "Object::GetOffsetOfInstanceIdMember", IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetOffsetOfInstanceIDInCPlusPlusObject();

		[NativeMethod(Name = "CurrentThreadIsMainThread", IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CurrentThreadIsMainThread();

		[FreeFunction("CloneObject")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object Internal_CloneSingle(Object data);

		[FreeFunction("CloneObject")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object Internal_CloneSingleWithParent(Object data, Transform parent, bool worldPositionStays);

		[FreeFunction("InstantiateObject")]
		private static Object Internal_InstantiateSingle(Object data, Vector3 pos, Quaternion rot)
		{
			return Object.Internal_InstantiateSingle_Injected(data, ref pos, ref rot);
		}

		[FreeFunction("InstantiateObject")]
		private static Object Internal_InstantiateSingleWithParent(Object data, Transform parent, Vector3 pos, Quaternion rot)
		{
			return Object.Internal_InstantiateSingleWithParent_Injected(data, parent, ref pos, ref rot);
		}

		[FreeFunction("UnityEngineObjectBindings::ToString")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string ToString(Object obj);

		[FreeFunction("UnityEngineObjectBindings::GetName")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetName(Object obj);

		[FreeFunction("UnityEngineObjectBindings::SetName")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetName(Object obj, string name);

		[NativeMethod(Name = "UnityEngineObjectBindings::DoesObjectWithInstanceIDExist", IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool DoesObjectWithInstanceIDExist(int instanceID);

		[FreeFunction("UnityEngineObjectBindings::FindObjectFromInstanceID")]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Object FindObjectFromInstanceID(int instanceID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object Internal_InstantiateSingle_Injected(Object data, ref Vector3 pos, ref Quaternion rot);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object Internal_InstantiateSingleWithParent_Injected(Object data, Transform parent, ref Vector3 pos, ref Quaternion rot);

		private IntPtr m_CachedPtr;

		internal static int OffsetOfInstanceIDInCPlusPlusObject = -1;

		private const string objectIsNullMessage = "The Object you want to instantiate is null.";

		private const string cloneDestroyedMessage = "Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.";
	}
}
