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
	/// <summary>
	///   <para>Base class for all objects Unity can reference.</para>
	/// </summary>
	[NativeHeader("Runtime/GameCode/CloneObject.h")]
	[NativeHeader("Runtime/SceneManager/SceneManager.h")]
	[RequiredByNativeCode(GenerateProxy = true)]
	[NativeHeader("Runtime/Export/UnityEngineObject.bindings.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class Object
	{
		/// <summary>
		///   <para>Returns the instance id of the object.</para>
		/// </summary>
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

		/// <summary>
		///   <para>The name of the object.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Clones the object original and returns the clone.</para>
		/// </summary>
		/// <param name="original">An existing object that you want to make a copy of.</param>
		/// <param name="position">Position for the new object.</param>
		/// <param name="rotation">Orientation of the new object.</param>
		/// <param name="parent">Parent that will be assigned to the new object.</param>
		/// <param name="instantiateInWorldSpace">Pass true when assigning a parent Object to maintain the world position of the Object, instead of setting its position relative to the new parent. Pass false to set the Object's position relative to its new parent.</param>
		/// <returns>
		///   <para>The instantiated clone.</para>
		/// </returns>
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

		/// <summary>
		///   <para>Clones the object original and returns the clone.</para>
		/// </summary>
		/// <param name="original">An existing object that you want to make a copy of.</param>
		/// <param name="position">Position for the new object.</param>
		/// <param name="rotation">Orientation of the new object.</param>
		/// <param name="parent">Parent that will be assigned to the new object.</param>
		/// <param name="instantiateInWorldSpace">Pass true when assigning a parent Object to maintain the world position of the Object, instead of setting its position relative to the new parent. Pass false to set the Object's position relative to its new parent.</param>
		/// <returns>
		///   <para>The instantiated clone.</para>
		/// </returns>
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

		/// <summary>
		///   <para>Clones the object original and returns the clone.</para>
		/// </summary>
		/// <param name="original">An existing object that you want to make a copy of.</param>
		/// <param name="position">Position for the new object.</param>
		/// <param name="rotation">Orientation of the new object.</param>
		/// <param name="parent">Parent that will be assigned to the new object.</param>
		/// <param name="instantiateInWorldSpace">Pass true when assigning a parent Object to maintain the world position of the Object, instead of setting its position relative to the new parent. Pass false to set the Object's position relative to its new parent.</param>
		/// <returns>
		///   <para>The instantiated clone.</para>
		/// </returns>
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

		/// <summary>
		///   <para>Clones the object original and returns the clone.</para>
		/// </summary>
		/// <param name="original">An existing object that you want to make a copy of.</param>
		/// <param name="position">Position for the new object.</param>
		/// <param name="rotation">Orientation of the new object.</param>
		/// <param name="parent">Parent that will be assigned to the new object.</param>
		/// <param name="instantiateInWorldSpace">Pass true when assigning a parent Object to maintain the world position of the Object, instead of setting its position relative to the new parent. Pass false to set the Object's position relative to its new parent.</param>
		/// <returns>
		///   <para>The instantiated clone.</para>
		/// </returns>
		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object original, Transform parent)
		{
			return Object.Instantiate(original, parent, false);
		}

		/// <summary>
		///   <para>Clones the object original and returns the clone.</para>
		/// </summary>
		/// <param name="original">An existing object that you want to make a copy of.</param>
		/// <param name="position">Position for the new object.</param>
		/// <param name="rotation">Orientation of the new object.</param>
		/// <param name="parent">Parent that will be assigned to the new object.</param>
		/// <param name="instantiateInWorldSpace">Pass true when assigning a parent Object to maintain the world position of the Object, instead of setting its position relative to the new parent. Pass false to set the Object's position relative to its new parent.</param>
		/// <returns>
		///   <para>The instantiated clone.</para>
		/// </returns>
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

		/// <summary>
		///   <para>Removes a gameobject, component or asset.</para>
		/// </summary>
		/// <param name="obj">The object to destroy.</param>
		/// <param name="t">The optional amount of time to delay before destroying the object.</param>
		[FreeFunction("Scripting::DestroyObjectFromScripting")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Destroy(Object obj, [DefaultValue("0.0F")] float t);

		/// <summary>
		///   <para>Removes a gameobject, component or asset.</para>
		/// </summary>
		/// <param name="obj">The object to destroy.</param>
		/// <param name="t">The optional amount of time to delay before destroying the object.</param>
		[ExcludeFromDocs]
		public static void Destroy(Object obj)
		{
			float num = 0f;
			Object.Destroy(obj, num);
		}

		/// <summary>
		///   <para>Destroys the object obj immediately. You are strongly recommended to use Destroy instead.</para>
		/// </summary>
		/// <param name="obj">Object to be destroyed.</param>
		/// <param name="allowDestroyingAssets">Set to true to allow assets to be destroyed.</param>
		[FreeFunction("Scripting::DestroyObjectFromScriptingImmediate")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DestroyImmediate(Object obj, [DefaultValue("false")] bool allowDestroyingAssets);

		/// <summary>
		///   <para>Destroys the object obj immediately. You are strongly recommended to use Destroy instead.</para>
		/// </summary>
		/// <param name="obj">Object to be destroyed.</param>
		/// <param name="allowDestroyingAssets">Set to true to allow assets to be destroyed.</param>
		[ExcludeFromDocs]
		public static void DestroyImmediate(Object obj)
		{
			bool flag = false;
			Object.DestroyImmediate(obj, flag);
		}

		/// <summary>
		///   <para>Returns a list of all active loaded objects of Type type.</para>
		/// </summary>
		/// <param name="type">The type of object to find.</param>
		/// <returns>
		///   <para>The array of objects found matching the type specified.</para>
		/// </returns>
		[TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
		[FreeFunction("UnityEngineObjectBindings::FindObjectsOfType")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Object[] FindObjectsOfType(Type type);

		/// <summary>
		///   <para>Do not destroy the target Object when loading a new Scene.</para>
		/// </summary>
		/// <param name="target">An Object not destroyed on Scene change.</param>
		[FreeFunction("GetSceneManager().DontDestroyOnLoad")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DontDestroyOnLoad(Object target);

		/// <summary>
		///   <para>Should the object be hidden, saved with the Scene or modifiable by the user?</para>
		/// </summary>
		public extern HideFlags hideFlags
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[Obsolete("use Object.Destroy instead.")]
		[FreeFunction("Scripting::DestroyObjectFromScripting")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DestroyObject(Object obj, [DefaultValue("0.0F")] float t);

		[Obsolete("use Object.Destroy instead.")]
		[ExcludeFromDocs]
		public static void DestroyObject(Object obj)
		{
			float num = 0f;
			Object.DestroyObject(obj, num);
		}

		[Obsolete("warning use Object.FindObjectsOfType instead.")]
		[FreeFunction("UnityEngineObjectBindings::FindObjectsOfType")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Object[] FindSceneObjectsOfType(Type type);

		/// <summary>
		///   <para>Returns a list of all active and inactive loaded objects of Type type, including assets.</para>
		/// </summary>
		/// <param name="type">The type of object or asset to find.</param>
		/// <returns>
		///   <para>The array of objects and assets found matching the type specified.</para>
		/// </returns>
		[Obsolete("use Resources.FindObjectsOfTypeAll instead.")]
		[FreeFunction("UnityEngineObjectBindings::FindObjectsOfTypeIncludingAssets")]
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

		/// <summary>
		///   <para>Returns a list of all active and inactive loaded objects of Type type.</para>
		/// </summary>
		/// <param name="type">The type of object to find.</param>
		/// <returns>
		///   <para>The array of objects found matching the type specified.</para>
		/// </returns>
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

		/// <summary>
		///   <para>Returns the first active loaded object of Type type.</para>
		/// </summary>
		/// <param name="type">The type of object to find.</param>
		/// <returns>
		///   <para>This returns the  Object that matches the specified type. It returns null if no Object matches the type.</para>
		/// </returns>
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

		/// <summary>
		///   <para>Returns the name of the GameObject.</para>
		/// </summary>
		/// <returns>
		///   <para>The name returned by ToString.</para>
		/// </returns>
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

		[VisibleToOtherModules]
		[FreeFunction("UnityEngineObjectBindings::FindObjectFromInstanceID")]
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
