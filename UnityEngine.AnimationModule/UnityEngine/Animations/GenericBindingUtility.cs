using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.Animations
{
	[NativeHeader("Modules/Animation/ScriptBindings/GenericBinding.bindings.h")]
	[StaticAccessor("UnityEngine::Animation::GenericBindingUtility", StaticAccessorType.DoubleColon)]
	public static class GenericBindingUtility
	{
		public static bool CreateGenericBinding(Object targetObject, string property, GameObject root, bool isObjectReference, out GenericBinding genericBinding)
		{
			bool flag = targetObject == null;
			if (flag)
			{
				throw new ArgumentNullException("targetObject");
			}
			bool flag2 = typeof(Transform).IsAssignableFrom(targetObject.GetType());
			if (flag2)
			{
				throw new ArgumentException("Unsupported type for targetObject. Cannot create a generic binding from a Transform component.");
			}
			Component component = targetObject as Component;
			bool flag3 = component != null;
			bool flag4;
			if (flag3)
			{
				flag4 = GenericBindingUtility.CreateGenericBindingForComponent(component, property, root, isObjectReference, out genericBinding);
			}
			else
			{
				GameObject gameObject = targetObject as GameObject;
				bool flag5 = gameObject != null;
				if (!flag5)
				{
					throw new ArgumentException(string.Format("Type {0} for {1} is unsupported. Expecting either a GameObject or a Component", targetObject.GetType(), "targetObject"));
				}
				flag4 = GenericBindingUtility.CreateGenericBindingForGameObject(gameObject, property, root, out genericBinding);
			}
			return flag4;
		}

		[NativeMethod(IsThreadSafe = false)]
		private unsafe static bool CreateGenericBindingForGameObject([NotNull] GameObject gameObject, string property, [NotNull] GameObject root, out GenericBinding genericBinding)
		{
			if (gameObject == null)
			{
				ThrowHelper.ThrowArgumentNullException(gameObject, "gameObject");
			}
			if (root == null)
			{
				ThrowHelper.ThrowArgumentNullException(root, "root");
			}
			bool flag;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(gameObject);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(gameObject, "gameObject");
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(property, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = property.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(root);
				if (intPtr2 == 0)
				{
					ThrowHelper.ThrowArgumentNullException(root, "root");
				}
				flag = GenericBindingUtility.CreateGenericBindingForGameObject_Injected(intPtr, ref managedSpanWrapper, intPtr2, out genericBinding);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[NativeMethod(IsThreadSafe = false)]
		private unsafe static bool CreateGenericBindingForComponent([NotNull] Component component, string property, [NotNull] GameObject root, bool isObjectReference, out GenericBinding genericBinding)
		{
			if (component == null)
			{
				ThrowHelper.ThrowArgumentNullException(component, "component");
			}
			if (root == null)
			{
				ThrowHelper.ThrowArgumentNullException(root, "root");
			}
			bool flag;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Component>(component);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(component, "component");
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(property, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = property.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(root);
				if (intPtr2 == 0)
				{
					ThrowHelper.ThrowArgumentNullException(root, "root");
				}
				flag = GenericBindingUtility.CreateGenericBindingForComponent_Injected(intPtr, ref managedSpanWrapper, intPtr2, isObjectReference, out genericBinding);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		[NativeMethod(IsThreadSafe = false)]
		public static GenericBinding[] GetAnimatableBindings([NotNull] GameObject targetObject, [NotNull] GameObject root)
		{
			if (targetObject == null)
			{
				ThrowHelper.ThrowArgumentNullException(targetObject, "targetObject");
			}
			if (root == null)
			{
				ThrowHelper.ThrowArgumentNullException(root, "root");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(targetObject);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(targetObject, "targetObject");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(root);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(root, "root");
			}
			return GenericBindingUtility.GetAnimatableBindings_Injected(intPtr, intPtr2);
		}

		[NativeMethod(IsThreadSafe = false)]
		public static GenericBinding[] GetCurveBindings([NotNull] AnimationClip clip)
		{
			if (clip == null)
			{
				ThrowHelper.ThrowArgumentNullException(clip, "clip");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(clip);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(clip, "clip");
			}
			return GenericBindingUtility.GetCurveBindings_Injected(intPtr);
		}

		[Obsolete("This version of BindProperties is deprecated. Use the overload which includes `out instanceIDProperties` instead.", false)]
		public static void BindProperties(GameObject rootGameObject, NativeArray<GenericBinding> genericBindings, out NativeArray<BoundProperty> floatProperties, out NativeArray<BoundProperty> discreteProperties, Allocator allocator)
		{
			NativeArray<BoundProperty> nativeArray;
			GenericBindingUtility.BindProperties(rootGameObject, genericBindings, out floatProperties, out discreteProperties, out nativeArray, allocator);
		}

		public unsafe static void BindProperties(GameObject rootGameObject, NativeArray<GenericBinding> genericBindings, out NativeArray<BoundProperty> floatProperties, out NativeArray<BoundProperty> discreteProperties, out NativeArray<BoundProperty> instanceIDProperties, Allocator allocator)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < genericBindings.Length; i++)
			{
				bool flag = genericBindings[i].typeID == 4;
				if (!flag)
				{
					bool isDiscrete = genericBindings[i].isDiscrete;
					if (isDiscrete)
					{
						num2++;
					}
					bool isObjectReference = genericBindings[i].isObjectReference;
					if (isObjectReference)
					{
						num3++;
					}
					else
					{
						num++;
					}
				}
			}
			floatProperties = new NativeArray<BoundProperty>(num, allocator, NativeArrayOptions.ClearMemory);
			discreteProperties = new NativeArray<BoundProperty>(num2, allocator, NativeArrayOptions.ClearMemory);
			instanceIDProperties = new NativeArray<BoundProperty>(num3, allocator, NativeArrayOptions.ClearMemory);
			void* unsafePtr = genericBindings.GetUnsafePtr<GenericBinding>();
			void* unsafePtr2 = floatProperties.GetUnsafePtr<BoundProperty>();
			void* unsafePtr3 = discreteProperties.GetUnsafePtr<BoundProperty>();
			void* unsafePtr4 = instanceIDProperties.GetUnsafePtr<BoundProperty>();
			GenericBindingUtility.Internal_BindProperties(rootGameObject, unsafePtr, genericBindings.Length, unsafePtr2, unsafePtr3, unsafePtr4);
		}

		[NativeMethod(IsThreadSafe = false)]
		internal unsafe static void Internal_BindProperties([NotNull] GameObject gameObject, void* genericBindings, int genericBindingsCount, void* floatProperties, void* discreteProperties, void* instanceIDProperties)
		{
			if (gameObject == null)
			{
				ThrowHelper.ThrowArgumentNullException(gameObject, "gameObject");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(gameObject);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(gameObject, "gameObject");
			}
			GenericBindingUtility.Internal_BindProperties_Injected(intPtr, genericBindings, genericBindingsCount, floatProperties, discreteProperties, instanceIDProperties);
		}

		public unsafe static void UnbindProperties(NativeArray<BoundProperty> boundProperties)
		{
			void* unsafePtr = boundProperties.GetUnsafePtr<BoundProperty>();
			GenericBindingUtility.Internal_UnbindProperties(unsafePtr, boundProperties.Length);
		}

		[NativeMethod(IsThreadSafe = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void Internal_UnbindProperties(void* boundProperties, int boundPropertiesCount);

		public unsafe static void SetValues(NativeArray<BoundProperty> boundProperties, NativeArray<float> values)
		{
			void* unsafePtr = boundProperties.GetUnsafePtr<BoundProperty>();
			void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<float>(values);
			GenericBindingUtility.SetFloatValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, values.Length);
		}

		public unsafe static void SetValues(NativeArray<BoundProperty> boundProperties, NativeArray<int> indices, NativeArray<float> values)
		{
			void* unsafePtr = boundProperties.GetUnsafePtr<BoundProperty>();
			void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<int>(indices);
			void* unsafeBufferPointerWithoutChecks2 = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<float>(values);
			GenericBindingUtility.SetScatterFloatValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, indices.Length, unsafeBufferPointerWithoutChecks2, values.Length);
		}

		public unsafe static void SetValues(NativeArray<BoundProperty> boundProperties, NativeArray<int> values)
		{
			void* unsafePtr = boundProperties.GetUnsafePtr<BoundProperty>();
			void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<int>(values);
			GenericBindingUtility.SetDiscreteValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, values.Length);
		}

		public unsafe static void SetValues(NativeArray<BoundProperty> boundProperties, NativeArray<int> indices, NativeArray<int> values)
		{
			void* unsafePtr = boundProperties.GetUnsafePtr<BoundProperty>();
			void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<int>(indices);
			void* unsafeBufferPointerWithoutChecks2 = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<int>(values);
			GenericBindingUtility.SetScatterDiscreteValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, indices.Length, unsafeBufferPointerWithoutChecks2, values.Length);
		}

		public unsafe static void SetValues(NativeArray<BoundProperty> boundProperties, NativeArray<EntityId> values)
		{
			void* unsafePtr = boundProperties.GetUnsafePtr<BoundProperty>();
			void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<EntityId>(values);
			GenericBindingUtility.SetEntityIdValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, values.Length);
		}

		public unsafe static void SetValues(NativeArray<BoundProperty> boundProperties, NativeArray<int> indices, NativeArray<EntityId> values)
		{
			void* unsafePtr = boundProperties.GetUnsafePtr<BoundProperty>();
			void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<int>(indices);
			void* unsafeBufferPointerWithoutChecks2 = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<EntityId>(values);
			GenericBindingUtility.SetScatterEntityIdValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, indices.Length, unsafeBufferPointerWithoutChecks2, values.Length);
		}

		public unsafe static void GetValues(NativeArray<BoundProperty> boundProperties, NativeArray<float> values)
		{
			void* unsafePtr = boundProperties.GetUnsafePtr<BoundProperty>();
			void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<float>(values);
			GenericBindingUtility.GetFloatValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, values.Length);
		}

		public unsafe static void GetValues(NativeArray<BoundProperty> boundProperties, NativeArray<int> indices, NativeArray<float> values)
		{
			void* unsafePtr = boundProperties.GetUnsafePtr<BoundProperty>();
			void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<int>(indices);
			void* unsafeBufferPointerWithoutChecks2 = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<float>(values);
			GenericBindingUtility.GetScatterFloatValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, indices.Length, unsafeBufferPointerWithoutChecks2, values.Length);
		}

		public unsafe static void GetValues(NativeArray<BoundProperty> boundProperties, NativeArray<int> values)
		{
			void* unsafePtr = boundProperties.GetUnsafePtr<BoundProperty>();
			void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<int>(values);
			GenericBindingUtility.GetDiscreteValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, values.Length);
		}

		public unsafe static void GetValues(NativeArray<BoundProperty> boundProperties, NativeArray<int> indices, NativeArray<int> values)
		{
			void* unsafePtr = boundProperties.GetUnsafePtr<BoundProperty>();
			void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<int>(indices);
			void* unsafeBufferPointerWithoutChecks2 = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<int>(values);
			GenericBindingUtility.GetScatterDiscreteValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, indices.Length, unsafeBufferPointerWithoutChecks2, values.Length);
		}

		public unsafe static void GetValues(NativeArray<BoundProperty> boundProperties, NativeArray<EntityId> values)
		{
			void* unsafePtr = boundProperties.GetUnsafePtr<BoundProperty>();
			void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<EntityId>(values);
			GenericBindingUtility.GetEntityIdValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, values.Length);
		}

		public unsafe static void GetValues(NativeArray<BoundProperty> boundProperties, NativeArray<int> indices, NativeArray<EntityId> values)
		{
			void* unsafePtr = boundProperties.GetUnsafePtr<BoundProperty>();
			void* unsafeBufferPointerWithoutChecks = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<int>(indices);
			void* unsafeBufferPointerWithoutChecks2 = NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<EntityId>(values);
			GenericBindingUtility.GetScatterEntityIdValues(unsafePtr, boundProperties.Length, unsafeBufferPointerWithoutChecks, indices.Length, unsafeBufferPointerWithoutChecks2, values.Length);
		}

		[NativeMethod(IsThreadSafe = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void SetFloatValues(void* boundProperties, int boundPropertiesCount, void* values, int valuesCount);

		[NativeMethod(IsThreadSafe = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void SetScatterFloatValues(void* boundProperties, int boundPropertiesCount, void* indices, int indicesCount, void* values, int valuesCount);

		[NativeMethod(IsThreadSafe = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void SetDiscreteValues(void* boundProperties, int boundPropertiesCount, void* values, int valuesCount);

		[NativeMethod(IsThreadSafe = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void SetScatterDiscreteValues(void* boundProperties, int boundPropertiesCount, void* indices, int indicesCount, void* values, int valuesCount);

		[NativeMethod(IsThreadSafe = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void SetEntityIdValues(void* boundProperties, int boundPropertiesCount, void* values, int valuesCount);

		[NativeMethod(IsThreadSafe = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void SetScatterEntityIdValues(void* boundProperties, int boundPropertiesCount, void* indices, int indicesCount, void* values, int valuesCount);

		[NativeMethod(IsThreadSafe = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void GetFloatValues(void* boundProperties, int boundPropertiesCount, void* values, int valuesCount);

		[NativeMethod(IsThreadSafe = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void GetScatterFloatValues(void* boundProperties, int boundPropertiesCount, void* indices, int indicesCount, void* values, int valuesCount);

		[NativeMethod(IsThreadSafe = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void GetDiscreteValues(void* boundProperties, int boundPropertiesCount, void* values, int valuesCount);

		[NativeMethod(IsThreadSafe = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void GetScatterDiscreteValues(void* boundProperties, int boundPropertiesCount, void* indices, int indicesCount, void* values, int valuesCount);

		[NativeMethod(IsThreadSafe = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void GetEntityIdValues(void* boundProperties, int boundPropertiesCount, void* values, int valuesCount);

		[NativeMethod(IsThreadSafe = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void GetScatterEntityIdValues(void* boundProperties, int boundPropertiesCount, void* indices, int indicesCount, void* values, int valuesCount);

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		internal static void ValidateIsCreated<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(NativeArray<T> array) where T : struct, ValueType
		{
			bool flag = !array.IsCreated;
			if (flag)
			{
				throw new ArgumentException("NativeArray of " + typeof(T).Name + " is not created.");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		internal static void ValidateIndicesAreInRange(NativeArray<int> indices, int maxValue)
		{
			for (int i = 0; i < indices.Length; i++)
			{
				bool flag = indices[i] < 0 || indices[i] >= maxValue;
				if (flag)
				{
					throw new IndexOutOfRangeException(string.Format("NativeArray of indices contain element out of range at index '{0}': value '{1}' is not in the range 0 to {2}.", i, indices[i], maxValue));
				}
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		internal static void ValidateLengthMatch<[global::System.Runtime.CompilerServices.IsUnmanaged] T1, [global::System.Runtime.CompilerServices.IsUnmanaged] T2>(NativeArray<T1> array1, NativeArray<T2> array2) where T1 : struct, ValueType where T2 : struct, ValueType
		{
			bool flag = array1.Length != array2.Length;
			if (flag)
			{
				throw new ArgumentException(string.Concat(new string[]
				{
					"Length must be equals for NativeArray<",
					typeof(T1).Name,
					"> and NativeArray<",
					typeof(T2).Name,
					">."
				}));
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateGenericBindingForGameObject_Injected(IntPtr gameObject, ref ManagedSpanWrapper property, IntPtr root, out GenericBinding genericBinding);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CreateGenericBindingForComponent_Injected(IntPtr component, ref ManagedSpanWrapper property, IntPtr root, bool isObjectReference, out GenericBinding genericBinding);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GenericBinding[] GetAnimatableBindings_Injected(IntPtr targetObject, IntPtr root);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GenericBinding[] GetCurveBindings_Injected(IntPtr clip);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void Internal_BindProperties_Injected(IntPtr gameObject, void* genericBindings, int genericBindingsCount, void* floatProperties, void* discreteProperties, void* instanceIDProperties);
	}
}
