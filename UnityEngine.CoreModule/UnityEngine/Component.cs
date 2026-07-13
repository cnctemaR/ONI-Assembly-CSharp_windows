using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Scripting/Component.bindings.h")]
	[RequiredByNativeCode]
	[NativeClass("Unity::Component")]
	public class Component : Object
	{
		public Transform transform
		{
			[FreeFunction("GetTransform", HasExplicitThis = true, ThrowsException = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Component>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<Transform>(Component.get_transform_Injected(intPtr));
			}
		}

		public TransformHandle transformHandle
		{
			[FreeFunction("GetTransformHandle", HasExplicitThis = true, ThrowsException = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Component>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				TransformHandle transformHandle;
				Component.get_transformHandle_Injected(intPtr, out transformHandle);
				return transformHandle;
			}
		}

		public GameObject gameObject
		{
			[FreeFunction("GetGameObject", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Component>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<GameObject>(Component.get_gameObject_Injected(intPtr));
			}
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public Component GetComponent(Type type)
		{
			return this.gameObject.GetComponent(type);
		}

		[FreeFunction(HasExplicitThis = true, ThrowsException = true)]
		internal void GetComponentFastPath(Type type, IntPtr oneFurtherThanResultValue)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Component>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Component.GetComponentFastPath_Injected(intPtr, type, oneFurtherThanResultValue);
		}

		[SecuritySafeCritical]
		public unsafe T GetComponent<T>()
		{
			CastHelper<T> castHelper = default(CastHelper<T>);
			this.GetComponentFastPath(typeof(T), new IntPtr((void*)(&castHelper.onePointerFurtherThanT)));
			return castHelper.t;
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public bool TryGetComponent(Type type, out Component component)
		{
			return this.gameObject.TryGetComponent(type, out component);
		}

		[SecuritySafeCritical]
		public bool TryGetComponent<T>(out T component)
		{
			return this.gameObject.TryGetComponent<T>(out component);
		}

		[FreeFunction(HasExplicitThis = true)]
		public unsafe Component GetComponent(string type)
		{
			Component component;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Component>(this);
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
				IntPtr component_Injected = Component.GetComponent_Injected(intPtr, ref managedSpanWrapper);
			}
			finally
			{
				IntPtr component_Injected;
				component = Unmarshal.UnmarshalUnityObject<Component>(component_Injected);
				char* ptr = null;
			}
			return component;
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public Component GetComponentInChildren(Type t, bool includeInactive)
		{
			return this.gameObject.GetComponentInChildren(t, includeInactive);
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public Component GetComponentInChildren(Type t)
		{
			return this.GetComponentInChildren(t, false);
		}

		public T GetComponentInChildren<T>([DefaultValue("false")] bool includeInactive)
		{
			return (T)((object)this.GetComponentInChildren(typeof(T), includeInactive));
		}

		[ExcludeFromDocs]
		public T GetComponentInChildren<T>()
		{
			return (T)((object)this.GetComponentInChildren(typeof(T), false));
		}

		public Component[] GetComponentsInChildren(Type t, bool includeInactive)
		{
			return this.gameObject.GetComponentsInChildren(t, includeInactive);
		}

		[ExcludeFromDocs]
		public Component[] GetComponentsInChildren(Type t)
		{
			return this.gameObject.GetComponentsInChildren(t, false);
		}

		public T[] GetComponentsInChildren<T>(bool includeInactive)
		{
			return this.gameObject.GetComponentsInChildren<T>(includeInactive);
		}

		public void GetComponentsInChildren<T>(bool includeInactive, List<T> result)
		{
			this.gameObject.GetComponentsInChildren<T>(includeInactive, result);
		}

		public T[] GetComponentsInChildren<T>()
		{
			return this.GetComponentsInChildren<T>(false);
		}

		public void GetComponentsInChildren<T>(List<T> results)
		{
			this.GetComponentsInChildren<T>(false, results);
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public Component GetComponentInParent(Type t, bool includeInactive)
		{
			return this.gameObject.GetComponentInParent(t, includeInactive);
		}

		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public Component GetComponentInParent(Type t)
		{
			return this.gameObject.GetComponentInParent(t, false);
		}

		public T GetComponentInParent<T>([DefaultValue("false")] bool includeInactive)
		{
			return (T)((object)this.GetComponentInParent(typeof(T), includeInactive));
		}

		public T GetComponentInParent<T>()
		{
			return (T)((object)this.GetComponentInParent(typeof(T), false));
		}

		public Component[] GetComponentsInParent(Type t, [DefaultValue("false")] bool includeInactive)
		{
			return this.gameObject.GetComponentsInParent(t, includeInactive);
		}

		[ExcludeFromDocs]
		public Component[] GetComponentsInParent(Type t)
		{
			return this.GetComponentsInParent(t, false);
		}

		public T[] GetComponentsInParent<T>(bool includeInactive)
		{
			return this.gameObject.GetComponentsInParent<T>(includeInactive);
		}

		public void GetComponentsInParent<T>(bool includeInactive, List<T> results)
		{
			this.gameObject.GetComponentsInParent<T>(includeInactive, results);
		}

		public T[] GetComponentsInParent<T>()
		{
			return this.GetComponentsInParent<T>(false);
		}

		public Component[] GetComponents(Type type)
		{
			return this.gameObject.GetComponents(type);
		}

		[FreeFunction(HasExplicitThis = true, ThrowsException = true)]
		private void GetComponentsForListInternal(Type searchType, object resultList)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Component>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Component.GetComponentsForListInternal_Injected(intPtr, searchType, resultList);
		}

		public void GetComponents(Type type, List<Component> results)
		{
			this.GetComponentsForListInternal(type, results);
		}

		public void GetComponents<T>(List<T> results)
		{
			this.GetComponentsForListInternal(typeof(T), results);
		}

		public string tag
		{
			get
			{
				return this.gameObject.tag;
			}
			set
			{
				this.gameObject.tag = value;
			}
		}

		public T[] GetComponents<T>()
		{
			return this.gameObject.GetComponents<T>();
		}

		public int GetComponentIndex()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Component>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return Component.GetComponentIndex_Injected(intPtr);
		}

		public bool CompareTag(string tag)
		{
			return this.gameObject.CompareTag(tag);
		}

		public bool CompareTag(TagHandle tag)
		{
			return this.gameObject.CompareTag(tag);
		}

		[FreeFunction(HasExplicitThis = true)]
		public unsafe void SendMessageUpwards(string methodName, [DefaultValue("null")] object value, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Component>(this);
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
				Component.SendMessageUpwards_Injected(intPtr, ref managedSpanWrapper, value, options);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[ExcludeFromDocs]
		public void SendMessageUpwards(string methodName, object value)
		{
			this.SendMessageUpwards(methodName, value, SendMessageOptions.RequireReceiver);
		}

		[ExcludeFromDocs]
		public void SendMessageUpwards(string methodName)
		{
			this.SendMessageUpwards(methodName, null, SendMessageOptions.RequireReceiver);
		}

		public void SendMessageUpwards(string methodName, SendMessageOptions options)
		{
			this.SendMessageUpwards(methodName, null, options);
		}

		public void SendMessage(string methodName, object value)
		{
			this.SendMessage(methodName, value, SendMessageOptions.RequireReceiver);
		}

		public void SendMessage(string methodName)
		{
			this.SendMessage(methodName, null, SendMessageOptions.RequireReceiver);
		}

		[FreeFunction("SendMessage", HasExplicitThis = true)]
		public unsafe void SendMessage(string methodName, object value, SendMessageOptions options)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Component>(this);
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
				Component.SendMessage_Injected(intPtr, ref managedSpanWrapper, value, options);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public void SendMessage(string methodName, SendMessageOptions options)
		{
			this.SendMessage(methodName, null, options);
		}

		[FreeFunction("BroadcastMessage", HasExplicitThis = true)]
		public unsafe void BroadcastMessage(string methodName, [DefaultValue("null")] object parameter, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options)
		{
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Component>(this);
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
				Component.BroadcastMessage_Injected(intPtr, ref managedSpanWrapper, parameter, options);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[ExcludeFromDocs]
		public void BroadcastMessage(string methodName, object parameter)
		{
			this.BroadcastMessage(methodName, parameter, SendMessageOptions.RequireReceiver);
		}

		[ExcludeFromDocs]
		public void BroadcastMessage(string methodName)
		{
			this.BroadcastMessage(methodName, null, SendMessageOptions.RequireReceiver);
		}

		public void BroadcastMessage(string methodName, SendMessageOptions options)
		{
			this.BroadcastMessage(methodName, null, options);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_transform_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_transformHandle_Injected(IntPtr _unity_self, out TransformHandle ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_gameObject_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetComponentFastPath_Injected(IntPtr _unity_self, Type type, IntPtr oneFurtherThanResultValue);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetComponent_Injected(IntPtr _unity_self, ref ManagedSpanWrapper type);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetComponentsForListInternal_Injected(IntPtr _unity_self, Type searchType, object resultList);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetComponentIndex_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SendMessageUpwards_Injected(IntPtr _unity_self, ref ManagedSpanWrapper methodName, [DefaultValue("null")] object value, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SendMessage_Injected(IntPtr _unity_self, ref ManagedSpanWrapper methodName, object value, SendMessageOptions options);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void BroadcastMessage_Injected(IntPtr _unity_self, ref ManagedSpanWrapper methodName, [DefaultValue("null")] object parameter, [DefaultValue("SendMessageOptions.RequireReceiver")] SendMessageOptions options);
	}
}
