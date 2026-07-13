using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine
{
	[RequiredByNativeCode(GenerateProxy = true)]
	[NativeHeader("Runtime/Export/Scripting/UnityEngineObject.bindings.h")]
	[NativeHeader("Runtime/SceneManager/SceneManager.h")]
	[NativeHeader("Runtime/GameCode/CloneObject.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class Object
	{
		[SecuritySafeCritical]
		public unsafe EntityId GetEntityId()
		{
			bool flag = this.m_CachedPtr == IntPtr.Zero;
			EntityId entityId;
			if (flag)
			{
				entityId = EntityId.None;
			}
			else
			{
				entityId = *(int*)((byte*)(void*)this.m_CachedPtr + Object.OffsetOfInstanceIDInCPlusPlusObject);
			}
			return entityId;
		}

		[SecuritySafeCritical]
		public unsafe int GetInstanceID()
		{
			bool flag = this.m_CachedPtr == IntPtr.Zero;
			int num;
			if (flag)
			{
				num = 0;
			}
			else
			{
				num = *(int*)((byte*)(void*)this.m_CachedPtr + Object.OffsetOfInstanceIDInCPlusPlusObject);
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
			bool flag = @object == null && other != null && !(other is Object);
			return !flag && Object.CompareBaseObjects(this, @object);
		}

		public static implicit operator bool([MaybeNullWhen(false)] [NotNullWhen(true)] Object exists)
		{
			return !Object.CompareBaseObjects(exists, null);
		}

		private static bool CompareBaseObjects(Object lhs, Object rhs)
		{
			bool flag = lhs == null;
			bool flag2 = rhs == null;
			bool flag3 = flag2 && flag;
			bool flag4;
			if (flag3)
			{
				flag4 = true;
			}
			else
			{
				bool flag5 = flag2;
				if (flag5)
				{
					flag4 = !Object.IsNativeObjectAlive(lhs);
				}
				else
				{
					bool flag6 = flag;
					if (flag6)
					{
						flag4 = !Object.IsNativeObjectAlive(rhs);
					}
					else
					{
						flag4 = lhs == rhs;
					}
				}
			}
			return flag4;
		}

		private void EnsureRunningOnMainThread()
		{
			bool flag = !Object.CurrentThreadIsMainThread();
			if (flag)
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
				return this.GetName();
			}
			set
			{
				this.SetName(value);
			}
		}

		public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original) where T : Object
		{
			return Object.InstantiateAsync<T>(original, new InstantiateParameters
			{
				worldSpace = true
			}, default(CancellationToken));
		}

		public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, Transform parent) where T : Object
		{
			return Object.InstantiateAsync<T>(original, new InstantiateParameters
			{
				worldSpace = true,
				parent = parent
			}, default(CancellationToken));
		}

		public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, Vector3 position, Quaternion rotation) where T : Object
		{
			return Object.InstantiateAsync<T>(original, position, rotation, new InstantiateParameters
			{
				worldSpace = true
			}, default(CancellationToken));
		}

		public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, Transform parent, Vector3 position, Quaternion rotation) where T : Object
		{
			return Object.InstantiateAsync<T>(original, position, rotation, new InstantiateParameters
			{
				worldSpace = true,
				parent = parent
			}, default(CancellationToken));
		}

		public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count) where T : Object
		{
			return Object.InstantiateAsync<T>(original, count, new InstantiateParameters
			{
				worldSpace = true
			}, default(CancellationToken));
		}

		public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent) where T : Object
		{
			return Object.InstantiateAsync<T>(original, count, new InstantiateParameters
			{
				worldSpace = true,
				parent = parent
			}, default(CancellationToken));
		}

		public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Vector3 position, Quaternion rotation) where T : Object
		{
			return Object.InstantiateAsync<T>(original, count, position, rotation, new InstantiateParameters
			{
				worldSpace = true
			}, default(CancellationToken));
		}

		public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Quaternion> rotations) where T : Object
		{
			return Object.InstantiateAsync<T>(original, count, positions, rotations, new InstantiateParameters
			{
				worldSpace = true
			}, default(CancellationToken));
		}

		public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent, Vector3 position, Quaternion rotation) where T : Object
		{
			return Object.InstantiateAsync<T>(original, count, position, rotation, new InstantiateParameters
			{
				worldSpace = true,
				parent = parent
			}, default(CancellationToken));
		}

		public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent, Vector3 position, Quaternion rotation, CancellationToken cancellationToken) where T : Object
		{
			return Object.InstantiateAsync<T>(original, count, position, rotation, new InstantiateParameters
			{
				worldSpace = true,
				parent = parent
			}, cancellationToken);
		}

		public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Quaternion> rotations) where T : Object
		{
			return Object.InstantiateAsync<T>(original, count, positions, rotations, new InstantiateParameters
			{
				worldSpace = true,
				parent = parent
			}, default(CancellationToken));
		}

		public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Quaternion> rotations, CancellationToken cancellationToken) where T : Object
		{
			return Object.InstantiateAsync<T>(original, count, positions, rotations, new InstantiateParameters
			{
				worldSpace = true,
				parent = parent
			}, cancellationToken);
		}

		public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, InstantiateParameters parameters, CancellationToken cancellationToken = default(CancellationToken)) where T : Object
		{
			return Object.InstantiateAsync<T>(original, 1, parameters, cancellationToken);
		}

		public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, InstantiateParameters parameters, CancellationToken cancellationToken = default(CancellationToken)) where T : Object
		{
			return Object.InstantiateAsync<T>(original, count, ReadOnlySpan<Vector3>.Empty, ReadOnlySpan<Quaternion>.Empty, parameters, cancellationToken);
		}

		public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, Vector3 position, Quaternion rotation, InstantiateParameters parameters, CancellationToken cancellationToken = default(CancellationToken)) where T : Object
		{
			return Object.InstantiateAsync<T>(original, 1, position, rotation, parameters, cancellationToken);
		}

		public unsafe static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Vector3 position, Quaternion rotation, InstantiateParameters parameters, CancellationToken cancellationToken = default(CancellationToken)) where T : Object
		{
			return Object.InstantiateAsync<T>(original, count, new ReadOnlySpan<Vector3>((void*)(&position), 1), new ReadOnlySpan<Quaternion>((void*)(&rotation), 1), parameters, cancellationToken);
		}

		[MethodImpl((MethodImplOptions)768)]
		public unsafe static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Quaternion> rotations, InstantiateParameters parameters, CancellationToken cancellationToken = default(CancellationToken)) where T : Object
		{
			Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
			bool flag = count <= 0;
			if (flag)
			{
				throw new ArgumentException("Cannot call instantiate multiple with count less or equal to zero");
			}
			fixed (Vector3* pinnableReference = positions.GetPinnableReference())
			{
				Vector3* ptr = pinnableReference;
				fixed (Quaternion* pinnableReference2 = rotations.GetPinnableReference())
				{
					Quaternion* ptr2 = pinnableReference2;
					return new AsyncInstantiateOperation<T>(Object.Internal_InstantiateAsyncWithParams(original, count, parameters, (IntPtr)((void*)ptr), positions.Length, (IntPtr)((void*)ptr2), rotations.Length), cancellationToken);
				}
			}
		}

		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object original, Vector3 position, Quaternion rotation)
		{
			Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
			bool flag = original is ScriptableObject;
			if (flag)
			{
				throw new ArgumentException("Cannot instantiate a ScriptableObject with a position and rotation");
			}
			Object @object = Object.Internal_InstantiateSingle(original, position, rotation);
			bool flag2 = @object == null;
			if (flag2)
			{
				throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
			}
			return @object;
		}

		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent)
		{
			bool flag = parent == null;
			Object @object;
			if (flag)
			{
				@object = Object.Instantiate(original, position, rotation);
			}
			else
			{
				Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
				Object object2 = Object.Internal_InstantiateSingleWithParent(original, parent, position, rotation);
				bool flag2 = object2 == null;
				if (flag2)
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
			bool flag = @object == null;
			if (flag)
			{
				throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
			}
			return @object;
		}

		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object original, Scene scene)
		{
			Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
			Object @object = Object.Internal_CloneSingleWithScene(original, scene);
			bool flag = @object == null;
			if (flag)
			{
				throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
			}
			return @object;
		}

		public static T Instantiate<T>(T original, InstantiateParameters parameters) where T : Object
		{
			Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
			T t = (T)((object)Object.Internal_CloneSingleWithParams(original, parameters));
			bool flag = t == null;
			if (flag)
			{
				throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
			}
			return t;
		}

		public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation, InstantiateParameters parameters) where T : Object
		{
			Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
			T t = (T)((object)Object.Internal_InstantiateSingleWithParams(original, position, rotation, parameters));
			bool flag = t == null;
			if (flag)
			{
				throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
			}
			return t;
		}

		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object original, Transform parent)
		{
			return Object.Instantiate(original, parent, false);
		}

		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object original, Transform parent, bool instantiateInWorldSpace)
		{
			bool flag = parent == null;
			Object @object;
			if (flag)
			{
				@object = Object.Instantiate(original);
			}
			else
			{
				Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
				Object object2 = Object.Internal_CloneSingleWithParent(original, parent, instantiateInWorldSpace);
				bool flag2 = object2 == null;
				if (flag2)
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
			bool flag = t == null;
			if (flag)
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
		public static void Destroy(Object obj, [DefaultValue("0.0F")] float t)
		{
			Object.Destroy_Injected(Object.MarshalledUnityObject.Marshal<Object>(obj), t);
		}

		[ExcludeFromDocs]
		public static void Destroy(Object obj)
		{
			float num = 0f;
			Object.Destroy(obj, num);
		}

		[NativeMethod(Name = "Scripting::DestroyObjectFromScriptingImmediate", IsFreeFunction = true, ThrowsException = true)]
		public static void DestroyImmediate(Object obj, [DefaultValue("false")] bool allowDestroyingAssets)
		{
			Object.DestroyImmediate_Injected(Object.MarshalledUnityObject.Marshal<Object>(obj), allowDestroyingAssets);
		}

		[ExcludeFromDocs]
		public static void DestroyImmediate(Object obj)
		{
			bool flag = false;
			Object.DestroyImmediate(obj, flag);
		}

		[Obsolete("Object.FindObjectsOfType has been deprecated. Use Object.FindObjectsByType instead which lets you decide whether you need the results sorted or not.  FindObjectsOfType sorts the results by InstanceID, but if you do not need this using FindObjectSortMode.None is considerably faster.", false)]
		public static Object[] FindObjectsOfType(Type type)
		{
			return Object.FindObjectsOfType(type, false);
		}

		[Obsolete("Object.FindObjectsOfType has been deprecated. Use Object.FindObjectsByType instead which lets you decide whether you need the results sorted or not.  FindObjectsOfType sorts the results by InstanceID but if you do not need this using FindObjectSortMode.None is considerably faster.", false)]
		[TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
		[FreeFunction("UnityEngineObjectBindings::FindObjectsOfType")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public static extern Object[] FindObjectsOfType(Type type, bool includeInactive);

		public static Object[] FindObjectsByType(Type type, FindObjectsSortMode sortMode)
		{
			return Object.FindObjectsByType(type, FindObjectsInactive.Exclude, sortMode);
		}

		[TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
		[FreeFunction("UnityEngineObjectBindings::FindObjectsByType")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public static extern Object[] FindObjectsByType(Type type, FindObjectsInactive findObjectsInactive, FindObjectsSortMode sortMode);

		[FreeFunction("GetSceneManager().DontDestroyOnLoad", ThrowsException = true)]
		public static void DontDestroyOnLoad([NotNull] Object target)
		{
			if (target == null)
			{
				ThrowHelper.ThrowArgumentNullException(target, "target");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(target);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(target, "target");
			}
			Object.DontDestroyOnLoad_Injected(intPtr);
		}

		public HideFlags hideFlags
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Object.get_hideFlags_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Object.set_hideFlags_Injected(intPtr, value);
			}
		}

		[Obsolete("use Object.Destroy instead.")]
		public static void DestroyObject(Object obj, [DefaultValue("0.0F")] float t)
		{
			Object.Destroy(obj, t);
		}

		[ExcludeFromDocs]
		[Obsolete("use Object.Destroy instead.")]
		public static void DestroyObject(Object obj)
		{
			float num = 0f;
			Object.Destroy(obj, num);
		}

		[Obsolete("Object.FindSceneObjectsOfType has been deprecated, Use Object.FindObjectsByType instead which lets you decide whether you need the results sorted or not.  FindSceneObjectsOfType sorts the results by InstanceID but if you do not need this using FindObjectSortMode.None is considerably faster.", false)]
		public static Object[] FindSceneObjectsOfType(Type type)
		{
			return Object.FindObjectsOfType(type);
		}

		[FreeFunction("UnityEngineObjectBindings::FindObjectsOfTypeIncludingAssets")]
		[Obsolete("use Resources.FindObjectsOfTypeAll instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: UnityMarshalAs(NativeType.ScriptingObjectPtr)]
		public static extern Object[] FindObjectsOfTypeIncludingAssets(Type type);

		[Obsolete("Object.FindObjectsOfType has been deprecated. Use Object.FindObjectsByType instead which lets you decide whether you need the results sorted or not.  FindObjectsOfType sorts the results by InstanceID but if you do not need this using FindObjectSortMode.None is considerably faster.", false)]
		public static T[] FindObjectsOfType<T>() where T : Object
		{
			return Resources.ConvertObjects<T>(Object.FindObjectsOfType(typeof(T), false));
		}

		public static T[] FindObjectsByType<T>(FindObjectsSortMode sortMode) where T : Object
		{
			return Resources.ConvertObjects<T>(Object.FindObjectsByType(typeof(T), FindObjectsInactive.Exclude, sortMode));
		}

		[Obsolete("Object.FindObjectsOfType has been deprecated. Use Object.FindObjectsByType instead which lets you decide whether you need the results sorted or not.  FindObjectsOfType sorts the results by InstanceID but if you do not need this using FindObjectSortMode.None is considerably faster.", false)]
		public static T[] FindObjectsOfType<T>(bool includeInactive) where T : Object
		{
			return Resources.ConvertObjects<T>(Object.FindObjectsOfType(typeof(T), includeInactive));
		}

		public static T[] FindObjectsByType<T>(FindObjectsInactive findObjectsInactive, FindObjectsSortMode sortMode) where T : Object
		{
			return Resources.ConvertObjects<T>(Object.FindObjectsByType(typeof(T), findObjectsInactive, sortMode));
		}

		[Obsolete("Object.FindObjectOfType has been deprecated. Use Object.FindFirstObjectByType instead or if finding any instance is acceptable the faster Object.FindAnyObjectByType", false)]
		public static T FindObjectOfType<T>() where T : Object
		{
			return (T)((object)Object.FindObjectOfType(typeof(T), false));
		}

		[Obsolete("Object.FindObjectOfType has been deprecated. Use Object.FindFirstObjectByType instead or if finding any instance is acceptable the faster Object.FindAnyObjectByType", false)]
		public static T FindObjectOfType<T>(bool includeInactive) where T : Object
		{
			return (T)((object)Object.FindObjectOfType(typeof(T), includeInactive));
		}

		public static T FindFirstObjectByType<T>() where T : Object
		{
			return (T)((object)Object.FindFirstObjectByType(typeof(T), FindObjectsInactive.Exclude));
		}

		public static T FindAnyObjectByType<T>() where T : Object
		{
			return (T)((object)Object.FindAnyObjectByType(typeof(T), FindObjectsInactive.Exclude));
		}

		public static T FindFirstObjectByType<T>(FindObjectsInactive findObjectsInactive) where T : Object
		{
			return (T)((object)Object.FindFirstObjectByType(typeof(T), findObjectsInactive));
		}

		public static T FindAnyObjectByType<T>(FindObjectsInactive findObjectsInactive) where T : Object
		{
			return (T)((object)Object.FindAnyObjectByType(typeof(T), findObjectsInactive));
		}

		[Obsolete("Please use Resources.FindObjectsOfTypeAll instead")]
		public static Object[] FindObjectsOfTypeAll(Type type)
		{
			return Resources.FindObjectsOfTypeAll(type);
		}

		private static void CheckNullArgument(object arg, string message)
		{
			bool flag = arg == null;
			if (flag)
			{
				throw new ArgumentException(message);
			}
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		[Obsolete("Object.FindObjectOfType has been deprecated. Use Object.FindFirstObjectByType instead or if finding any instance is acceptable the faster Object.FindAnyObjectByType", false)]
		public static Object FindObjectOfType(Type type)
		{
			Object[] array = Object.FindObjectsOfType(type, false);
			bool flag = array.Length != 0;
			Object @object;
			if (flag)
			{
				@object = array[0];
			}
			else
			{
				@object = null;
			}
			return @object;
		}

		public static Object FindFirstObjectByType(Type type)
		{
			Object[] array = Object.FindObjectsByType(type, FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
			return (array.Length != 0) ? array[0] : null;
		}

		public static Object FindAnyObjectByType(Type type)
		{
			Object[] array = Object.FindObjectsByType(type, FindObjectsInactive.Exclude, FindObjectsSortMode.None);
			return (array.Length != 0) ? array[0] : null;
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		[Obsolete("Object.FindObjectOfType has been deprecated. Use Object.FindFirstObjectByType instead or if finding any instance is acceptable the faster Object.FindAnyObjectByType", false)]
		public static Object FindObjectOfType(Type type, bool includeInactive)
		{
			Object[] array = Object.FindObjectsOfType(type, includeInactive);
			bool flag = array.Length != 0;
			Object @object;
			if (flag)
			{
				@object = array[0];
			}
			else
			{
				@object = null;
			}
			return @object;
		}

		public static Object FindFirstObjectByType(Type type, FindObjectsInactive findObjectsInactive)
		{
			Object[] array = Object.FindObjectsByType(type, findObjectsInactive, FindObjectsSortMode.InstanceID);
			return (array.Length != 0) ? array[0] : null;
		}

		public static Object FindAnyObjectByType(Type type, FindObjectsInactive findObjectsInactive)
		{
			Object[] array = Object.FindObjectsByType(type, findObjectsInactive, FindObjectsSortMode.None);
			return (array.Length != 0) ? array[0] : null;
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

		[NativeMethod(Name = "CloneObject", IsFreeFunction = true, ThrowsException = true)]
		private static Object Internal_CloneSingle([NotNull] Object data)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException(data, "data");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(data);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(data, "data");
			}
			return Unmarshal.UnmarshalUnityObject<Object>(Object.Internal_CloneSingle_Injected(intPtr));
		}

		[FreeFunction("CloneObjectToScene")]
		private static Object Internal_CloneSingleWithScene([NotNull] Object data, Scene scene)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException(data, "data");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(data);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(data, "data");
			}
			return Unmarshal.UnmarshalUnityObject<Object>(Object.Internal_CloneSingleWithScene_Injected(intPtr, ref scene));
		}

		[FreeFunction("CloneObjectWithParams")]
		private static Object Internal_CloneSingleWithParams([NotNull] Object data, InstantiateParameters parameters)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException(data, "data");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(data);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(data, "data");
			}
			return Unmarshal.UnmarshalUnityObject<Object>(Object.Internal_CloneSingleWithParams_Injected(intPtr, ref parameters));
		}

		[FreeFunction("InstantiateObjectWithParams")]
		private static Object Internal_InstantiateSingleWithParams([NotNull] Object data, Vector3 position, Quaternion rotation, InstantiateParameters parameters)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException(data, "data");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(data);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(data, "data");
			}
			return Unmarshal.UnmarshalUnityObject<Object>(Object.Internal_InstantiateSingleWithParams_Injected(intPtr, ref position, ref rotation, ref parameters));
		}

		[FreeFunction("CloneObject")]
		private static Object Internal_CloneSingleWithParent([NotNull] Object data, [NotNull] Transform parent, bool worldPositionStays)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException(data, "data");
			}
			if (parent == null)
			{
				ThrowHelper.ThrowArgumentNullException(parent, "parent");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(data);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(data, "data");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Transform>(parent);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(parent, "parent");
			}
			return Unmarshal.UnmarshalUnityObject<Object>(Object.Internal_CloneSingleWithParent_Injected(intPtr, intPtr2, worldPositionStays));
		}

		[FreeFunction("InstantiateAsyncObjects")]
		private static IntPtr Internal_InstantiateAsyncWithParams([NotNull] Object original, int count, InstantiateParameters parameters, IntPtr positions, int positionsCount, IntPtr rotations, int rotationsCount)
		{
			if (original == null)
			{
				ThrowHelper.ThrowArgumentNullException(original, "original");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(original);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(original, "original");
			}
			return Object.Internal_InstantiateAsyncWithParams_Injected(intPtr, count, ref parameters, positions, positionsCount, rotations, rotationsCount);
		}

		[FreeFunction("InstantiateObject")]
		private static Object Internal_InstantiateSingle([NotNull] Object data, Vector3 pos, Quaternion rot)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException(data, "data");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(data);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(data, "data");
			}
			return Unmarshal.UnmarshalUnityObject<Object>(Object.Internal_InstantiateSingle_Injected(intPtr, ref pos, ref rot));
		}

		[FreeFunction("InstantiateObject")]
		private static Object Internal_InstantiateSingleWithParent([NotNull] Object data, [NotNull] Transform parent, Vector3 pos, Quaternion rot)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException(data, "data");
			}
			if (parent == null)
			{
				ThrowHelper.ThrowArgumentNullException(parent, "parent");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(data);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(data, "data");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Transform>(parent);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(parent, "parent");
			}
			return Unmarshal.UnmarshalUnityObject<Object>(Object.Internal_InstantiateSingleWithParent_Injected(intPtr, intPtr2, ref pos, ref rot));
		}

		[FreeFunction("UnityEngineObjectBindings::ToString")]
		private static string ToString(Object obj)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				Object.ToString_Injected(Object.MarshalledUnityObject.Marshal<Object>(obj), out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[FreeFunction("UnityEngineObjectBindings::GetName", HasExplicitThis = true)]
		private string GetName()
		{
			string stringAndDispose;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				Object.GetName_Injected(intPtr, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		[FreeFunction("UnityEngineObjectBindings::IsPersistent")]
		internal static bool IsPersistent([NotNull] Object obj)
		{
			if (obj == null)
			{
				ThrowHelper.ThrowArgumentNullException(obj, "obj");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(obj);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(obj, "obj");
			}
			return Object.IsPersistent_Injected(intPtr);
		}

		[FreeFunction("UnityEngineObjectBindings::SetName", HasExplicitThis = true)]
		private unsafe void SetName(string name)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(name, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = name.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				Object.SetName_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[NativeMethod(Name = "UnityEngineObjectBindings::DoesObjectWithInstanceIDExist", IsFreeFunction = true, IsThreadSafe = true)]
		internal static bool DoesObjectWithInstanceIDExist(EntityId instanceID)
		{
			return Object.DoesObjectWithInstanceIDExist_Injected(ref instanceID);
		}

		[VisibleToOtherModules]
		[FreeFunction("UnityEngineObjectBindings::FindObjectFromInstanceID")]
		internal static Object FindObjectFromInstanceID(EntityId instanceID)
		{
			return Unmarshal.UnmarshalUnityObject<Object>(Object.FindObjectFromInstanceID_Injected(ref instanceID));
		}

		[FreeFunction("UnityEngineObjectBindings::GetPtrFromInstanceID")]
		private static IntPtr GetPtrFromInstanceID(EntityId instanceID, Type objectType, out bool isMonoBehaviour)
		{
			return Object.GetPtrFromInstanceID_Injected(ref instanceID, objectType, out isMonoBehaviour);
		}

		[VisibleToOtherModules]
		[FreeFunction("UnityEngineObjectBindings::ForceLoadFromInstanceID")]
		internal static Object ForceLoadFromInstanceID(EntityId instanceID)
		{
			return Unmarshal.UnmarshalUnityObject<Object>(Object.ForceLoadFromInstanceID_Injected(ref instanceID));
		}

		[FreeFunction("UnityEngineObjectBindings::MarkObjectDirty", HasExplicitThis = true)]
		internal void MarkDirty()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Object>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Object.MarkDirty_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Destroy_Injected(IntPtr obj, [DefaultValue("0.0F")] float t);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DestroyImmediate_Injected(IntPtr obj, [DefaultValue("false")] bool allowDestroyingAssets);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void DontDestroyOnLoad_Injected(IntPtr target);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern HideFlags get_hideFlags_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_hideFlags_Injected(IntPtr _unity_self, HideFlags value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_CloneSingle_Injected(IntPtr data);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_CloneSingleWithScene_Injected(IntPtr data, [In] ref Scene scene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_CloneSingleWithParams_Injected(IntPtr data, [In] ref InstantiateParameters parameters);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_InstantiateSingleWithParams_Injected(IntPtr data, [In] ref Vector3 position, [In] ref Quaternion rotation, [In] ref InstantiateParameters parameters);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_CloneSingleWithParent_Injected(IntPtr data, IntPtr parent, bool worldPositionStays);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_InstantiateAsyncWithParams_Injected(IntPtr original, int count, [In] ref InstantiateParameters parameters, IntPtr positions, int positionsCount, IntPtr rotations, int rotationsCount);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_InstantiateSingle_Injected(IntPtr data, [In] ref Vector3 pos, [In] ref Quaternion rot);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_InstantiateSingleWithParent_Injected(IntPtr data, IntPtr parent, [In] ref Vector3 pos, [In] ref Quaternion rot);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ToString_Injected(IntPtr obj, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetName_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsPersistent_Injected(IntPtr obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool DoesObjectWithInstanceIDExist_Injected([In] ref EntityId instanceID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr FindObjectFromInstanceID_Injected([In] ref EntityId instanceID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetPtrFromInstanceID_Injected([In] ref EntityId instanceID, Type objectType, out bool isMonoBehaviour);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr ForceLoadFromInstanceID_Injected([In] ref EntityId instanceID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void MarkDirty_Injected(IntPtr _unity_self);

		private const int kInstanceID_None = 0;

		private IntPtr m_CachedPtr;

		internal static readonly int OffsetOfInstanceIDInCPlusPlusObject = Object.GetOffsetOfInstanceIDInCPlusPlusObject();

		private const string objectIsNullMessage = "The Object you want to instantiate is null.";

		private const string cloneDestroyedMessage = "Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.";

		[VisibleToOtherModules]
		internal static class MarshalledUnityObject
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static IntPtr Marshal<T>(T obj) where T : Object
			{
				bool flag = obj == null;
				IntPtr intPtr;
				if (flag)
				{
					intPtr = IntPtr.Zero;
				}
				else
				{
					intPtr = Object.MarshalledUnityObject.MarshalNotNull<T>(obj);
				}
				return intPtr;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static IntPtr MarshalNotNull<T>(T obj) where T : Object
			{
				return obj.m_CachedPtr;
			}

			public static void TryThrowEditorNullExceptionObject(Object unityObj, string paramterName)
			{
			}
		}
	}
}
