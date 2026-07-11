using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.Experimental.Animations
{
	[NativeHeader("Runtime/Animation/ScriptBindings/AnimatorJobExtensions.bindings.h")]
	[NativeHeader("Runtime/Animation/Animator.h")]
	[NativeHeader("Runtime/Animation/Director/AnimationStreamHandles.h")]
	[NativeHeader("Runtime/Animation/Director/AnimationSceneHandles.h")]
	[NativeHeader("Runtime/Animation/Director/AnimationStream.h")]
	[StaticAccessor("AnimatorJobExtensionsBindings", StaticAccessorType.DoubleColon)]
	public static class AnimatorJobExtensions
	{
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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalBindStreamTransform([NotNull] Animator animator, [NotNull] Transform transform, out TransformStreamHandle transformStreamHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalBindStreamProperty([NotNull] Animator animator, [NotNull] Transform transform, [NotNull] Type type, [NotNull] string property, bool isObjectReference, out PropertyStreamHandle propertyStreamHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalBindSceneTransform([NotNull] Animator animator, [NotNull] Transform transform, out TransformSceneHandle transformSceneHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalBindSceneProperty([NotNull] Animator animator, [NotNull] Transform transform, [NotNull] Type type, [NotNull] string property, bool isObjectReference, out PropertySceneHandle propertySceneHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool InternalOpenAnimationStream([NotNull] Animator animator, ref AnimationStream stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalCloseAnimationStream([NotNull] Animator animator, ref AnimationStream stream);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalResolveAllStreamHandles([NotNull] Animator animator);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalResolveAllSceneHandles([NotNull] Animator animator);
	}
}
