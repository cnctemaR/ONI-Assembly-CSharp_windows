using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Jobs;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.Animations
{
	[NativeHeader("Modules/Animation/Director/AnimationSceneHandles.h")]
	[NativeHeader("Modules/Animation/Animator.h")]
	[StaticAccessor("AnimatorJobExtensionsBindings", StaticAccessorType.DoubleColon)]
	[NativeHeader("Modules/Animation/ScriptBindings/AnimatorJobExtensions.bindings.h")]
	[MovedFrom("UnityEngine.Experimental.Animations")]
	[NativeHeader("Modules/Animation/Director/AnimationStreamHandles.h")]
	[NativeHeader("Modules/Animation/Director/AnimationStream.h")]
	public static class AnimatorJobExtensions
	{
		public static void AddJobDependency(this Animator animator, JobHandle jobHandle)
		{
			AnimatorJobExtensions.InternalAddJobDependency(animator, jobHandle);
		}

		public static TransformStreamHandle BindStreamTransform(this Animator animator, Transform transform)
		{
			TransformStreamHandle transformStreamHandle = default(TransformStreamHandle);
			AnimatorJobExtensions.InternalBindStreamTransform(animator, transform, out transformStreamHandle);
			return transformStreamHandle;
		}

		public static PropertyStreamHandle BindStreamProperty(this Animator animator, Transform transform, Type type, string property)
		{
			return animator.BindStreamProperty(transform, type, property, false);
		}

		public static PropertyStreamHandle BindCustomStreamProperty(this Animator animator, string property, CustomStreamPropertyType type)
		{
			PropertyStreamHandle propertyStreamHandle = default(PropertyStreamHandle);
			AnimatorJobExtensions.InternalBindCustomStreamProperty(animator, property, type, out propertyStreamHandle);
			return propertyStreamHandle;
		}

		public static PropertyStreamHandle BindStreamProperty(this Animator animator, Transform transform, Type type, string property, [DefaultValue("false")] bool isObjectReference)
		{
			PropertyStreamHandle propertyStreamHandle = default(PropertyStreamHandle);
			AnimatorJobExtensions.InternalBindStreamProperty(animator, transform, type, property, isObjectReference, out propertyStreamHandle);
			return propertyStreamHandle;
		}

		public static TransformSceneHandle BindSceneTransform(this Animator animator, Transform transform)
		{
			TransformSceneHandle transformSceneHandle = default(TransformSceneHandle);
			AnimatorJobExtensions.InternalBindSceneTransform(animator, transform, out transformSceneHandle);
			return transformSceneHandle;
		}

		public static PropertySceneHandle BindSceneProperty(this Animator animator, Transform transform, Type type, string property)
		{
			return animator.BindSceneProperty(transform, type, property, false);
		}

		public static PropertySceneHandle BindSceneProperty(this Animator animator, Transform transform, Type type, string property, [DefaultValue("false")] bool isObjectReference)
		{
			PropertySceneHandle propertySceneHandle = default(PropertySceneHandle);
			AnimatorJobExtensions.InternalBindSceneProperty(animator, transform, type, property, isObjectReference, out propertySceneHandle);
			return propertySceneHandle;
		}

		public static bool OpenAnimationStream(this Animator animator, ref AnimationStream stream)
		{
			return AnimatorJobExtensions.InternalOpenAnimationStream(animator, ref stream);
		}

		public static void CloseAnimationStream(this Animator animator, ref AnimationStream stream)
		{
			AnimatorJobExtensions.InternalCloseAnimationStream(animator, ref stream);
		}

		public static void ResolveAllStreamHandles(this Animator animator)
		{
			AnimatorJobExtensions.InternalResolveAllStreamHandles(animator);
		}

		public static void ResolveAllSceneHandles(this Animator animator)
		{
			AnimatorJobExtensions.InternalResolveAllSceneHandles(animator);
		}

		internal static void UnbindAllHandles(this Animator animator)
		{
			AnimatorJobExtensions.InternalUnbindAllStreamHandles(animator);
			AnimatorJobExtensions.InternalUnbindAllSceneHandles(animator);
		}

		public static void UnbindAllStreamHandles(this Animator animator)
		{
			AnimatorJobExtensions.InternalUnbindAllStreamHandles(animator);
		}

		public static void UnbindAllSceneHandles(this Animator animator)
		{
			AnimatorJobExtensions.InternalUnbindAllSceneHandles(animator);
		}

		private static void InternalAddJobDependency([NotNull] Animator animator, JobHandle jobHandle)
		{
			if (animator == null)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(animator);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			AnimatorJobExtensions.InternalAddJobDependency_Injected(intPtr, ref jobHandle);
		}

		private static void InternalBindStreamTransform([NotNull] Animator animator, [NotNull] Transform transform, out TransformStreamHandle transformStreamHandle)
		{
			if (animator == null)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			if (transform == null)
			{
				ThrowHelper.ThrowArgumentNullException(transform, "transform");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(animator);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Transform>(transform);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(transform, "transform");
			}
			AnimatorJobExtensions.InternalBindStreamTransform_Injected(intPtr, intPtr2, out transformStreamHandle);
		}

		private unsafe static void InternalBindStreamProperty([NotNull] Animator animator, [NotNull] Transform transform, [NotNull] Type type, [NotNull] string property, bool isObjectReference, out PropertyStreamHandle propertyStreamHandle)
		{
			if (animator == null)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			if (transform == null)
			{
				ThrowHelper.ThrowArgumentNullException(transform, "transform");
			}
			if (type == null)
			{
				ThrowHelper.ThrowArgumentNullException(type, "type");
			}
			if (property == null)
			{
				ThrowHelper.ThrowArgumentNullException(property, "property");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(animator);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(animator, "animator");
				}
				IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Transform>(transform);
				if (intPtr2 == 0)
				{
					ThrowHelper.ThrowArgumentNullException(transform, "transform");
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
				AnimatorJobExtensions.InternalBindStreamProperty_Injected(intPtr, intPtr2, type, ref managedSpanWrapper, isObjectReference, out propertyStreamHandle);
			}
			finally
			{
				char* ptr = null;
			}
		}

		private unsafe static void InternalBindCustomStreamProperty([NotNull] Animator animator, [NotNull] string property, CustomStreamPropertyType propertyType, out PropertyStreamHandle propertyStreamHandle)
		{
			if (animator == null)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			if (property == null)
			{
				ThrowHelper.ThrowArgumentNullException(property, "property");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(animator);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(animator, "animator");
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
				AnimatorJobExtensions.InternalBindCustomStreamProperty_Injected(intPtr, ref managedSpanWrapper, propertyType, out propertyStreamHandle);
			}
			finally
			{
				char* ptr = null;
			}
		}

		private static void InternalBindSceneTransform([NotNull] Animator animator, [NotNull] Transform transform, out TransformSceneHandle transformSceneHandle)
		{
			if (animator == null)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			if (transform == null)
			{
				ThrowHelper.ThrowArgumentNullException(transform, "transform");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(animator);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Transform>(transform);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(transform, "transform");
			}
			AnimatorJobExtensions.InternalBindSceneTransform_Injected(intPtr, intPtr2, out transformSceneHandle);
		}

		private unsafe static void InternalBindSceneProperty([NotNull] Animator animator, [NotNull] Transform transform, [NotNull] Type type, [NotNull] string property, bool isObjectReference, out PropertySceneHandle propertySceneHandle)
		{
			if (animator == null)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			if (transform == null)
			{
				ThrowHelper.ThrowArgumentNullException(transform, "transform");
			}
			if (type == null)
			{
				ThrowHelper.ThrowArgumentNullException(type, "type");
			}
			if (property == null)
			{
				ThrowHelper.ThrowArgumentNullException(property, "property");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(animator);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowArgumentNullException(animator, "animator");
				}
				IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Transform>(transform);
				if (intPtr2 == 0)
				{
					ThrowHelper.ThrowArgumentNullException(transform, "transform");
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
				AnimatorJobExtensions.InternalBindSceneProperty_Injected(intPtr, intPtr2, type, ref managedSpanWrapper, isObjectReference, out propertySceneHandle);
			}
			finally
			{
				char* ptr = null;
			}
		}

		private static bool InternalOpenAnimationStream([NotNull] Animator animator, ref AnimationStream stream)
		{
			if (animator == null)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(animator);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			return AnimatorJobExtensions.InternalOpenAnimationStream_Injected(intPtr, ref stream);
		}

		private static void InternalCloseAnimationStream([NotNull] Animator animator, ref AnimationStream stream)
		{
			if (animator == null)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(animator);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			AnimatorJobExtensions.InternalCloseAnimationStream_Injected(intPtr, ref stream);
		}

		private static void InternalResolveAllStreamHandles([NotNull] Animator animator)
		{
			if (animator == null)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(animator);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			AnimatorJobExtensions.InternalResolveAllStreamHandles_Injected(intPtr);
		}

		private static void InternalResolveAllSceneHandles([NotNull] Animator animator)
		{
			if (animator == null)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(animator);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			AnimatorJobExtensions.InternalResolveAllSceneHandles_Injected(intPtr);
		}

		private static void InternalUnbindAllStreamHandles([NotNull] Animator animator)
		{
			if (animator == null)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(animator);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			AnimatorJobExtensions.InternalUnbindAllStreamHandles_Injected(intPtr);
		}

		private static void InternalUnbindAllSceneHandles([NotNull] Animator animator)
		{
			if (animator == null)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<Animator>(animator);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(animator, "animator");
			}
			AnimatorJobExtensions.InternalUnbindAllSceneHandles_Injected(intPtr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalAddJobDependency_Injected(IntPtr animator, [In] ref JobHandle jobHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalBindStreamTransform_Injected(IntPtr animator, IntPtr transform, out TransformStreamHandle transformStreamHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalBindStreamProperty_Injected(IntPtr animator, IntPtr transform, Type type, ref ManagedSpanWrapper property, bool isObjectReference, out PropertyStreamHandle propertyStreamHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalBindCustomStreamProperty_Injected(IntPtr animator, ref ManagedSpanWrapper property, CustomStreamPropertyType propertyType, out PropertyStreamHandle propertyStreamHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalBindSceneTransform_Injected(IntPtr animator, IntPtr transform, out TransformSceneHandle transformSceneHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalBindSceneProperty_Injected(IntPtr animator, IntPtr transform, Type type, ref ManagedSpanWrapper property, bool isObjectReference, out PropertySceneHandle propertySceneHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalOpenAnimationStream_Injected(IntPtr animator, ref AnimationStream stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalCloseAnimationStream_Injected(IntPtr animator, ref AnimationStream stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalResolveAllStreamHandles_Injected(IntPtr animator);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalResolveAllSceneHandles_Injected(IntPtr animator);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalUnbindAllStreamHandles_Injected(IntPtr animator);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalUnbindAllSceneHandles_Injected(IntPtr animator);
	}
}
