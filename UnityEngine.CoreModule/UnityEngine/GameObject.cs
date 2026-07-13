using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Scripting/GameObject.bindings.h")]
	[ExcludeFromPreset]
	[UsedByNativeCode]
	public sealed class GameObject : Object
	{
		[FreeFunction("GameObjectBindings::CreatePrimitive")]
		public static GameObject CreatePrimitive(PrimitiveType type)
		{
			return Unmarshal.UnmarshalUnityObject<GameObject>(GameObject.CreatePrimitive_Injected(type));
		}

		[SecuritySafeCritical]
		public unsafe T GetComponent<T>()
		{
			CastHelper<T> castHelper = default(CastHelper<T>);
			this.GetComponentFastPath(typeof(T), new IntPtr((void*)(&castHelper.onePointerFurtherThanT)));
			return castHelper.t;
		}

		[FreeFunction(Name = "GameObjectBindings::GetComponentFromType", HasExplicitThis = true, ThrowsException = true)]
		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public Component GetComponent(Type type)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Component>(GameObject.GetComponent_Injected(intPtr, type));
		}

		[FreeFunction(Name = "GameObjectBindings::GetComponentFastPath", HasExplicitThis = true, ThrowsException = true)]
		internal void GetComponentFastPath(Type type, IntPtr oneFurtherThanResultValue)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GameObject.GetComponentFastPath_Injected(intPtr, type, oneFurtherThanResultValue);
		}

		[FreeFunction(Name = "Scripting::GetScriptingWrapperOfComponentOfGameObject", HasExplicitThis = true)]
		internal unsafe Component GetComponentByName(string type)
		{
			Component component;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(type, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = type.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr componentByName_Injected = GameObject.GetComponentByName_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				IntPtr componentByName_Injected;
				component = Unmarshal.UnmarshalUnityObject<Component>(componentByName_Injected);
				char* ptr = null;
			}
			return component;
		}

		[FreeFunction(Name = "Scripting::GetScriptingWrapperOfComponentOfGameObjectWithCase", HasExplicitThis = true)]
		internal unsafe Component GetComponentByNameWithCase(string type, bool caseSensitive)
		{
			Component component;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(type, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = type.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr componentByNameWithCase_Injected = GameObject.GetComponentByNameWithCase_Injected(intPtr, ref managedSpanWrapper, caseSensitive);
			}
			finally
			{
				IntPtr componentByNameWithCase_Injected;
				component = Unmarshal.UnmarshalUnityObject<Component>(componentByNameWithCase_Injected);
				char* ptr = null;
			}
			return component;
		}

		public Component GetComponent(string type)
		{
			return this.GetComponentByName(type);
		}

		[FreeFunction(Name = "GameObjectBindings::GetComponentInChildren", HasExplicitThis = true, ThrowsException = true)]
		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public Component GetComponentInChildren(Type type, bool includeInactive)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Component>(GameObject.GetComponentInChildren_Injected(intPtr, type, includeInactive));
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public Component GetComponentInChildren(Type type)
		{
			return this.GetComponentInChildren(type, false);
		}

		[ExcludeFromDocs]
		public T GetComponentInChildren<T>()
		{
			bool flag = false;
			return this.GetComponentInChildren<T>(flag);
		}

		public T GetComponentInChildren<T>([DefaultValue("false")] bool includeInactive)
		{
			return (T)((object)this.GetComponentInChildren(typeof(T), includeInactive));
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		[FreeFunction(Name = "GameObjectBindings::GetComponentInParent", HasExplicitThis = true, ThrowsException = true)]
		public Component GetComponentInParent(Type type, bool includeInactive)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Component>(GameObject.GetComponentInParent_Injected(intPtr, type, includeInactive));
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public Component GetComponentInParent(Type type)
		{
			return this.GetComponentInParent(type, false);
		}

		[ExcludeFromDocs]
		public T GetComponentInParent<T>()
		{
			bool flag = false;
			return this.GetComponentInParent<T>(flag);
		}

		public T GetComponentInParent<T>([DefaultValue("false")] bool includeInactive)
		{
			return (T)((object)this.GetComponentInParent(typeof(T), includeInactive));
		}

		[FreeFunction(Name = "GameObjectBindings::GetComponentsInternal", HasExplicitThis = true, ThrowsException = true)]
		private Array GetComponentsInternal(Type type, bool useSearchTypeAsArrayReturnType, bool recursive, bool includeInactive, bool reverse, object resultList)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GameObject.GetComponentsInternal_Injected(intPtr, type, useSearchTypeAsArrayReturnType, recursive, includeInactive, reverse, resultList);
		}

		public Component[] GetComponents(Type type)
		{
			return (Component[])this.GetComponentsInternal(type, false, false, true, false, null);
		}

		public T[] GetComponents<T>()
		{
			return (T[])this.GetComponentsInternal(typeof(T), true, false, true, false, null);
		}

		public void GetComponents(Type type, List<Component> results)
		{
			this.GetComponentsInternal(type, false, false, true, false, results);
		}

		public void GetComponents<T>(List<T> results)
		{
			this.GetComponentsInternal(typeof(T), true, false, true, false, results);
		}

		[ExcludeFromDocs]
		public Component[] GetComponentsInChildren(Type type)
		{
			bool flag = false;
			return this.GetComponentsInChildren(type, flag);
		}

		public Component[] GetComponentsInChildren(Type type, [DefaultValue("false")] bool includeInactive)
		{
			return (Component[])this.GetComponentsInternal(type, false, true, includeInactive, false, null);
		}

		public T[] GetComponentsInChildren<T>(bool includeInactive)
		{
			return (T[])this.GetComponentsInternal(typeof(T), true, true, includeInactive, false, null);
		}

		public void GetComponentsInChildren<T>(bool includeInactive, List<T> results)
		{
			this.GetComponentsInternal(typeof(T), true, true, includeInactive, false, results);
		}

		public T[] GetComponentsInChildren<T>()
		{
			return this.GetComponentsInChildren<T>(false);
		}

		public void GetComponentsInChildren<T>(List<T> results)
		{
			this.GetComponentsInChildren<T>(false, results);
		}

		[ExcludeFromDocs]
		public Component[] GetComponentsInParent(Type type)
		{
			bool flag = false;
			return this.GetComponentsInParent(type, flag);
		}

		public Component[] GetComponentsInParent(Type type, [DefaultValue("false")] bool includeInactive)
		{
			return (Component[])this.GetComponentsInternal(type, false, true, includeInactive, true, null);
		}

		public void GetComponentsInParent<T>(bool includeInactive, List<T> results)
		{
			this.GetComponentsInternal(typeof(T), true, true, includeInactive, true, results);
		}

		public T[] GetComponentsInParent<T>(bool includeInactive)
		{
			return (T[])this.GetComponentsInternal(typeof(T), true, true, includeInactive, true, null);
		}

		public T[] GetComponentsInParent<T>()
		{
			return this.GetComponentsInParent<T>(false);
		}

		[SecuritySafeCritical]
		public unsafe bool TryGetComponent<T>(out T component)
		{
			CastHelper<T> castHelper = default(CastHelper<T>);
			this.TryGetComponentFastPath(typeof(T), new IntPtr((void*)(&castHelper.onePointerFurtherThanT)));
			component = castHelper.t;
			return castHelper.t != null;
		}

		public bool TryGetComponent(Type type, out Component component)
		{
			component = this.TryGetComponentInternal(type);
			return component != null;
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		[FreeFunction(Name = "GameObjectBindings::TryGetComponentFromType", HasExplicitThis = true, ThrowsException = true)]
		internal Component TryGetComponentInternal(Type type)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Component>(GameObject.TryGetComponentInternal_Injected(intPtr, type));
		}

		[FreeFunction(Name = "GameObjectBindings::TryGetComponentFastPath", HasExplicitThis = true, ThrowsException = true)]
		internal void TryGetComponentFastPath(Type type, IntPtr oneFurtherThanResultValue)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GameObject.TryGetComponentFastPath_Injected(intPtr, type, oneFurtherThanResultValue);
		}

		public static GameObject FindWithTag(string tag)
		{
			return GameObject.FindGameObjectWithTag(tag);
		}

		[FreeFunction(Name = "GameObjectBindings::FindGameObjectsWithTagForListInternal", ThrowsException = true)]
		private unsafe static void FindGameObjectsWithTagForListInternal(string tag, object results)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(tag, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = tag.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				GameObject.FindGameObjectsWithTagForListInternal_Injected(ref managedSpanWrapper, results);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public static void FindGameObjectsWithTag(string tag, List<GameObject> results)
		{
			GameObject.FindGameObjectsWithTagForListInternal(tag, results);
		}

		public void SendMessageUpwards(string methodName, SendMessageOptions options)
		{
			this.SendMessageUpwards(methodName, null, options);
		}

		public void SendMessage(string methodName, SendMessageOptions options)
		{
			this.SendMessage(methodName, null, options);
		}

		public void BroadcastMessage(string methodName, SendMessageOptions options)
		{
			this.BroadcastMessage(methodName, null, options);
		}

		[FreeFunction(Name = "MonoAddComponent", HasExplicitThis = true)]
		internal unsafe Component AddComponentInternal(string className)
		{
			Component component;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(className, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = className.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr2 = GameObject.AddComponentInternal_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				IntPtr intPtr2;
				component = Unmarshal.UnmarshalUnityObject<Component>(intPtr2);
				char* ptr = null;
			}
			return component;
		}

		[FreeFunction(Name = "MonoAddComponentWithType", HasExplicitThis = true)]
		private Component Internal_AddComponentWithType(Type componentType)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Component>(GameObject.Internal_AddComponentWithType_Injected(intPtr, componentType));
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public Component AddComponent(Type componentType)
		{
			return this.Internal_AddComponentWithType(componentType);
		}

		public T AddComponent<T>() where T : Component
		{
			return this.AddComponent(typeof(T)) as T;
		}

		public int GetComponentCount()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GameObject.GetComponentCount_Injected(intPtr);
		}

		[NativeName("QueryComponentAtIndex<Unity::Component>")]
		internal Component QueryComponentAtIndex(int index)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Unmarshal.UnmarshalUnityObject<Component>(GameObject.QueryComponentAtIndex_Injected(intPtr, index));
		}

		public Component GetComponentAtIndex(int index)
		{
			bool flag = index < 0 || index >= this.GetComponentCount();
			if (flag)
			{
				throw new ArgumentOutOfRangeException("index", "Valid range is 0 to GetComponentCount() - 1.");
			}
			return this.QueryComponentAtIndex(index);
		}

		public T GetComponentAtIndex<T>(int index) where T : Component
		{
			T t = (T)((object)this.GetComponentAtIndex(index));
			bool flag = t == null;
			if (flag)
			{
				throw new InvalidCastException();
			}
			return t;
		}

		public int GetComponentIndex(Component component)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GameObject.GetComponentIndex_Injected(intPtr, Object.MarshalledUnityObject.Marshal<Component>(component));
		}

		public Transform transform
		{
			[FreeFunction("GameObjectBindings::GetTransform", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Transform>(GameObject.get_transform_Injected(intPtr));
			}
		}

		public TransformHandle transformHandle
		{
			[FreeFunction("GameObjectBindings::GetTransformHandle", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TransformHandle transformHandle;
				GameObject.get_transformHandle_Injected(intPtr, out transformHandle);
				return transformHandle;
			}
		}

		public int layer
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GameObject.get_layer_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GameObject.set_layer_Injected(intPtr, value);
			}
		}

		[Obsolete("GameObject.active is obsolete. Use GameObject.SetActive(), GameObject.activeSelf or GameObject.activeInHierarchy.")]
		public bool active
		{
			[NativeMethod(Name = "IsActive")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GameObject.get_active_Injected(intPtr);
			}
			[NativeMethod(Name = "SetSelfActive")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GameObject.set_active_Injected(intPtr, value);
			}
		}

		[NativeMethod(Name = "SetSelfActive")]
		public void SetActive(bool value)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GameObject.SetActive_Injected(intPtr, value);
		}

		public bool activeSelf
		{
			[NativeMethod(Name = "IsSelfActive")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GameObject.get_activeSelf_Injected(intPtr);
			}
		}

		public bool activeInHierarchy
		{
			[NativeMethod(Name = "IsActive")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GameObject.get_activeInHierarchy_Injected(intPtr);
			}
		}

		[Obsolete("gameObject.SetActiveRecursively() is obsolete. Use GameObject.SetActive(), which is now inherited by children.")]
		[NativeMethod(Name = "SetActiveRecursivelyDeprecated")]
		public void SetActiveRecursively(bool state)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			GameObject.SetActiveRecursively_Injected(intPtr, state);
		}

		public bool isStatic
		{
			[NativeMethod(Name = "GetIsStaticDeprecated")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GameObject.get_isStatic_Injected(intPtr);
			}
			[NativeMethod(Name = "SetIsStaticDeprecated")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				GameObject.set_isStatic_Injected(intPtr, value);
			}
		}

		internal bool isStaticBatchable
		{
			[NativeMethod(Name = "IsStaticBatchable")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GameObject.get_isStaticBatchable_Injected(intPtr);
			}
		}

		public unsafe string tag
		{
			[FreeFunction("GameObjectBindings::GetTag", HasExplicitThis = true)]
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					GameObject.get_tag_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
			[FreeFunction("GameObjectBindings::SetTag", HasExplicitThis = true)]
			set
			{
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = value.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					GameObject.set_tag_Injected(intPtr, ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
		}

		public bool CompareTag(string tag)
		{
			return this.CompareTag_Internal(tag);
		}

		public bool CompareTag(TagHandle tag)
		{
			return this.CompareTagHandle_Internal(tag);
		}

		[FreeFunction(Name = "GameObjectBindings::CompareTag", HasExplicitThis = true)]
		private unsafe bool CompareTag_Internal(string tag)
		{
			bool flag;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(tag, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = tag.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = GameObject.CompareTag_Internal_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[FreeFunction(Name = "GameObjectBindings::CompareTagHandle", HasExplicitThis = true)]
		private bool CompareTagHandle_Internal(TagHandle tag)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return GameObject.CompareTagHandle_Internal_Injected(intPtr, ref tag);
		}

		[FreeFunction(Name = "GameObjectBindings::FindGameObjectWithTag", ThrowsException = true)]
		public unsafe static GameObject FindGameObjectWithTag(string tag)
		{
			GameObject gameObject;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(tag, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = tag.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr = GameObject.FindGameObjectWithTag_Injected(ref managedSpanWrapper);
			}
			finally
			{
				IntPtr intPtr;
				gameObject = Unmarshal.UnmarshalUnityObject<GameObject>(intPtr);
				char* ptr = null;
			}
			return gameObject;
		}

		[FreeFunction(Name = "GameObjectBindings::FindGameObjectsWithTag", ThrowsException = true)]
		public unsafe static GameObject[] FindGameObjectsWithTag(string tag)
		{
			GameObject[] array;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(tag, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = tag.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				array = GameObject.FindGameObjectsWithTag_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return array;
		}

		[FreeFunction(Name = "Scripting::SendScriptingMessageUpwards", HasExplicitThis = true)]
		public unsafe void SendMessageUpwards(string methodName, [DefaultValue("null")] object value, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(methodName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = methodName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				GameObject.SendMessageUpwards_Injected(intPtr, ref managedSpanWrapper, value, options);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[ExcludeFromDocs]
		public void SendMessageUpwards(string methodName, object value)
		{
			SendMessageOptions sendMessageOptions = SendMessageOptions.RequireReceiver;
			this.SendMessageUpwards(methodName, value, sendMessageOptions);
		}

		[ExcludeFromDocs]
		public void SendMessageUpwards(string methodName)
		{
			SendMessageOptions sendMessageOptions = SendMessageOptions.RequireReceiver;
			object obj = null;
			this.SendMessageUpwards(methodName, obj, sendMessageOptions);
		}

		[FreeFunction(Name = "Scripting::SendScriptingMessage", HasExplicitThis = true)]
		public unsafe void SendMessage(string methodName, [DefaultValue("null")] object value, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(methodName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = methodName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				GameObject.SendMessage_Injected(intPtr, ref managedSpanWrapper, value, options);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[ExcludeFromDocs]
		public void SendMessage(string methodName, object value)
		{
			SendMessageOptions sendMessageOptions = SendMessageOptions.RequireReceiver;
			this.SendMessage(methodName, value, sendMessageOptions);
		}

		[ExcludeFromDocs]
		public void SendMessage(string methodName)
		{
			SendMessageOptions sendMessageOptions = SendMessageOptions.RequireReceiver;
			object obj = null;
			this.SendMessage(methodName, obj, sendMessageOptions);
		}

		[FreeFunction(Name = "Scripting::BroadcastScriptingMessage", HasExplicitThis = true)]
		public unsafe void BroadcastMessage(string methodName, [DefaultValue("null")] object parameter, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(methodName, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = methodName.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				GameObject.BroadcastMessage_Injected(intPtr, ref managedSpanWrapper, parameter, options);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[ExcludeFromDocs]
		public void BroadcastMessage(string methodName, object parameter)
		{
			SendMessageOptions sendMessageOptions = SendMessageOptions.RequireReceiver;
			this.BroadcastMessage(methodName, parameter, sendMessageOptions);
		}

		[ExcludeFromDocs]
		public void BroadcastMessage(string methodName)
		{
			SendMessageOptions sendMessageOptions = SendMessageOptions.RequireReceiver;
			object obj = null;
			this.BroadcastMessage(methodName, obj, sendMessageOptions);
		}

		public GameObject(string name)
		{
			GameObject.Internal_CreateGameObject(this, name);
		}

		public GameObject()
		{
			GameObject.Internal_CreateGameObject(this, null);
		}

		public GameObject(string name, params Type[] components)
		{
			GameObject.Internal_CreateGameObject(this, name);
			foreach (Type type in components)
			{
				this.AddComponent(type);
			}
		}

		[FreeFunction(Name = "GameObjectBindings::Internal_CreateGameObject")]
		private unsafe static void Internal_CreateGameObject([Writable] GameObject self, string name)
		{
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
				GameObject.Internal_CreateGameObject_Injected(self, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[FreeFunction(Name = "GameObjectBindings::Find")]
		public unsafe static GameObject Find(string name)
		{
			GameObject gameObject;
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
				IntPtr intPtr = GameObject.Find_Injected(ref managedSpanWrapper);
			}
			finally
			{
				IntPtr intPtr;
				gameObject = Unmarshal.UnmarshalUnityObject<GameObject>(intPtr);
				char* ptr = null;
			}
			return gameObject;
		}

		[FreeFunction(Name = "GameObjectBindings::SetGameObjectsActiveByInstanceID")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetGameObjectsActive(IntPtr instanceIds, int instanceCount, bool active);

		[Obsolete("Obsolete. Please use GameObject.SetGameObjectsActive(NativeArray<EntityId>, bool) instead.")]
		public static void SetGameObjectsActive(NativeArray<int> instanceIDs, bool active)
		{
			bool flag = !instanceIDs.IsCreated;
			if (flag)
			{
				throw new ArgumentException("NativeArray is uninitialized", "instanceIDs");
			}
			bool flag2 = instanceIDs.Length == 0;
			if (!flag2)
			{
				GameObject.SetGameObjectsActive((IntPtr)instanceIDs.GetUnsafeReadOnlyPtr<int>(), instanceIDs.Length, active);
			}
		}

		public unsafe static void SetGameObjectsActive(NativeArray<EntityId> entityIds, bool active)
		{
			Debug.Assert(sizeof(EntityId) == 4, "EntityId size mismatch. Please check the definition of EntityId.");
			bool flag = !entityIds.IsCreated;
			if (flag)
			{
				throw new ArgumentException("NativeArray is uninitialized", "entityIds");
			}
			bool flag2 = entityIds.Length == 0;
			if (!flag2)
			{
				GameObject.SetGameObjectsActive((IntPtr)entityIds.GetUnsafeReadOnlyPtr<EntityId>(), entityIds.Length, active);
			}
		}

		[Obsolete("Obsolete. Please use GameObject.SetGameObjectsActive(ReadOnlySpan<EntityId>, bool) instead.")]
		public unsafe static void SetGameObjectsActive(ReadOnlySpan<int> instanceIDs, bool active)
		{
			bool flag = instanceIDs.Length == 0;
			if (!flag)
			{
				fixed (int* pinnableReference = instanceIDs.GetPinnableReference())
				{
					int* ptr = pinnableReference;
					GameObject.SetGameObjectsActive((IntPtr)((void*)ptr), instanceIDs.Length, active);
				}
			}
		}

		public unsafe static void SetGameObjectsActive(ReadOnlySpan<EntityId> entityIds, bool active)
		{
			Debug.Assert(sizeof(EntityId) == 4, "EntityId size mismatch. Please check the definition of EntityId.");
			bool flag = entityIds.Length == 0;
			if (!flag)
			{
				fixed (EntityId* pinnableReference = entityIds.GetPinnableReference())
				{
					EntityId* ptr = pinnableReference;
					GameObject.SetGameObjectsActive((IntPtr)((void*)ptr), entityIds.Length, active);
				}
			}
		}

		[FreeFunction("GameObjectBindings::InstantiateGameObjectsByInstanceID")]
		private static void InstantiateGameObjects(EntityId sourceInstanceID, IntPtr newInstanceIDs, IntPtr newTransformInstanceIDs, int count, Scene destinationScene)
		{
			GameObject.InstantiateGameObjects_Injected(ref sourceInstanceID, newInstanceIDs, newTransformInstanceIDs, count, ref destinationScene);
		}

		[Obsolete("Obsolete. Please use GameObject.InstantiateGameObjects(EntityId, int, NativeArray<EntityId>, NativeArray<EntityId>, Scene) instead.")]
		public static void InstantiateGameObjects(int sourceInstanceID, int count, NativeArray<int> newInstanceIDs, NativeArray<int> newTransformInstanceIDs, Scene destinationScene = default(Scene))
		{
			bool flag = !newInstanceIDs.IsCreated;
			if (flag)
			{
				throw new ArgumentException("NativeArray is uninitialized", "newInstanceIDs");
			}
			bool flag2 = !newTransformInstanceIDs.IsCreated;
			if (flag2)
			{
				throw new ArgumentException("NativeArray is uninitialized", "newTransformInstanceIDs");
			}
			bool flag3 = count == 0;
			if (!flag3)
			{
				bool flag4 = count != newInstanceIDs.Length || count != newTransformInstanceIDs.Length;
				if (flag4)
				{
					throw new ArgumentException("Size mismatch! Both arrays must already be the size of count.");
				}
				GameObject.InstantiateGameObjects(sourceInstanceID, (IntPtr)newInstanceIDs.GetUnsafeReadOnlyPtr<int>(), (IntPtr)newTransformInstanceIDs.GetUnsafeReadOnlyPtr<int>(), newInstanceIDs.Length, destinationScene);
			}
		}

		public unsafe static void InstantiateGameObjects(EntityId sourceEntityId, int count, NativeArray<EntityId> newEntityIds, NativeArray<EntityId> newTransformEntityIds, Scene destinationScene = default(Scene))
		{
			Debug.Assert(sizeof(EntityId) == 4, "EntityId size mismatch. Please check the definition of EntityId.");
			bool flag = !newEntityIds.IsCreated;
			if (flag)
			{
				throw new ArgumentException("NativeArray is uninitialized", "newEntityIds");
			}
			bool flag2 = !newTransformEntityIds.IsCreated;
			if (flag2)
			{
				throw new ArgumentException("NativeArray is uninitialized", "newTransformEntityIds");
			}
			bool flag3 = count == 0;
			if (!flag3)
			{
				bool flag4 = count != newEntityIds.Length || count != newTransformEntityIds.Length;
				if (flag4)
				{
					throw new ArgumentException("Size mismatch! Both arrays must already be the size of count.");
				}
				GameObject.InstantiateGameObjects(sourceEntityId, (IntPtr)newEntityIds.GetUnsafeReadOnlyPtr<EntityId>(), (IntPtr)newTransformEntityIds.GetUnsafeReadOnlyPtr<EntityId>(), newEntityIds.Length, destinationScene);
			}
		}

		[Obsolete("Obsolete. Please use GameObject.GetScene(EntityId entityId) instead.")]
		public static Scene GetScene(int instanceID)
		{
			return GameObject.GetSceneInternal(instanceID);
		}

		[FreeFunction(Name = "GameObjectBindings::GetSceneByEntityId")]
		private static Scene GetSceneInternal(EntityId entityId)
		{
			Scene scene;
			GameObject.GetSceneInternal_Injected(ref entityId, out scene);
			return scene;
		}

		public static Scene GetScene(EntityId entityId)
		{
			return GameObject.GetSceneInternal(entityId);
		}

		public Scene scene
		{
			[FreeFunction("GameObjectBindings::GetScene", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Scene scene;
				GameObject.get_scene_Injected(intPtr, out scene);
				return scene;
			}
		}

		public ulong sceneCullingMask
		{
			[FreeFunction(Name = "GameObjectBindings::GetSceneCullingMask", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return GameObject.get_sceneCullingMask_Injected(intPtr);
			}
		}

		public GameObject gameObject
		{
			get
			{
				return this;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreatePrimitive_Injected(PrimitiveType type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetComponent_Injected(IntPtr _unity_self, Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetComponentFastPath_Injected(IntPtr _unity_self, Type type, IntPtr oneFurtherThanResultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetComponentByName_Injected(IntPtr _unity_self, ref ManagedSpanWrapper type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetComponentByNameWithCase_Injected(IntPtr _unity_self, ref ManagedSpanWrapper type, bool caseSensitive);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetComponentInChildren_Injected(IntPtr _unity_self, Type type, bool includeInactive);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetComponentInParent_Injected(IntPtr _unity_self, Type type, bool includeInactive);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Array GetComponentsInternal_Injected(IntPtr _unity_self, Type type, bool useSearchTypeAsArrayReturnType, bool recursive, bool includeInactive, bool reverse, object resultList);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr TryGetComponentInternal_Injected(IntPtr _unity_self, Type type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void TryGetComponentFastPath_Injected(IntPtr _unity_self, Type type, IntPtr oneFurtherThanResultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FindGameObjectsWithTagForListInternal_Injected(ref ManagedSpanWrapper tag, object results);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr AddComponentInternal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper className);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_AddComponentWithType_Injected(IntPtr _unity_self, Type componentType);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetComponentCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr QueryComponentAtIndex_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetComponentIndex_Injected(IntPtr _unity_self, IntPtr component);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_transform_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_transformHandle_Injected(IntPtr _unity_self, out TransformHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_layer_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_layer_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_active_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_active_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetActive_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_activeSelf_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_activeInHierarchy_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetActiveRecursively_Injected(IntPtr _unity_self, bool state);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isStatic_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_isStatic_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_isStaticBatchable_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_tag_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_tag_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CompareTag_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper tag);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CompareTagHandle_Internal_Injected(IntPtr _unity_self, [In] ref TagHandle tag);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr FindGameObjectWithTag_Injected(ref ManagedSpanWrapper tag);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GameObject[] FindGameObjectsWithTag_Injected(ref ManagedSpanWrapper tag);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SendMessageUpwards_Injected(IntPtr _unity_self, ref ManagedSpanWrapper methodName, [DefaultValue("null")] object value, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SendMessage_Injected(IntPtr _unity_self, ref ManagedSpanWrapper methodName, [DefaultValue("null")] object value, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BroadcastMessage_Injected(IntPtr _unity_self, ref ManagedSpanWrapper methodName, [DefaultValue("null")] object parameter, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateGameObject_Injected([Writable] GameObject self, ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Find_Injected(ref ManagedSpanWrapper name);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InstantiateGameObjects_Injected([In] ref EntityId sourceInstanceID, IntPtr newInstanceIDs, IntPtr newTransformInstanceIDs, int count, [In] ref Scene destinationScene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetSceneInternal_Injected([In] ref EntityId entityId, out Scene ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_scene_Injected(IntPtr _unity_self, out Scene ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern ulong get_sceneCullingMask_Injected(IntPtr _unity_self);
	}
}
