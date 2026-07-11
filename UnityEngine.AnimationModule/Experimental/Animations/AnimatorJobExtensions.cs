using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.Animations
{
	/// <summary>
	///   <para>Static class providing extension methods for Animator and the animation C# jobs.</para>
	/// </summary>
	[NativeHeader("Runtime/Animation/ScriptBindings/AnimatorJobExtensions.bindings.h")]
	[NativeHeader("Runtime/Animation/Director/AnimationStreamHandles.h")]
	[NativeHeader("Runtime/Animation/Animator.h")]
	[NativeHeader("Runtime/Animation/Director/AnimationStream.h")]
	[NativeHeader("Runtime/Animation/Director/AnimationSceneHandles.h")]
	[StaticAccessor("AnimatorJobExtensionsBindings", StaticAccessorType.DoubleColon)]
	public static class AnimatorJobExtensions
	{
		/// <summary>
		///   <para>Create a TransformStreamHandle representing the new binding between the Animator and a Transform already bound to the Animator.</para>
		/// </summary>
		/// <param name="animator">The Animator instance the method is called on.</param>
		/// <param name="transform">The Transform to bind.</param>
		/// <returns>
		///   <para>The TransformStreamHandle representing the new binding.</para>
		/// </returns>
		public static TransformStreamHandle BindStreamTransform(this Animator animator, Transform transform)
		{
			TransformStreamHandle transformStreamHandle = default(TransformStreamHandle);
			AnimatorJobExtensions.InternalBindStreamTransform(animator, transform, out transformStreamHandle);
			return transformStreamHandle;
		}

		/// <summary>
		///   <para>Create a PropertyStreamHandle representing the new binding on the Component property of a Transform already bound to the Animator.</para>
		/// </summary>
		/// <param name="animator">The Animator instance the method is called on.</param>
		/// <param name="transform">The Transform to target.</param>
		/// <param name="type">The Component type.</param>
		/// <param name="property">The property to bind.</param>
		/// <returns>
		///   <para>The PropertyStreamHandle representing the new binding.</para>
		/// </returns>
		public static PropertyStreamHandle BindStreamProperty(this Animator animator, Transform transform, Type type, string property)
		{
			PropertyStreamHandle propertyStreamHandle = default(PropertyStreamHandle);
			AnimatorJobExtensions.InternalBindStreamProperty(animator, transform, type, property, out propertyStreamHandle);
			return propertyStreamHandle;
		}

		/// <summary>
		///   <para>Create a TransformSceneHandle representing the new binding between the Animator and a Transform in the scene.</para>
		/// </summary>
		/// <param name="animator">The Animator instance the method is called on.</param>
		/// <param name="transform">The Transform to bind.</param>
		/// <returns>
		///   <para>The TransformSceneHandle representing the new binding.</para>
		/// </returns>
		public static TransformSceneHandle BindSceneTransform(this Animator animator, Transform transform)
		{
			TransformSceneHandle transformSceneHandle = default(TransformSceneHandle);
			AnimatorJobExtensions.InternalBindSceneTransform(animator, transform, out transformSceneHandle);
			return transformSceneHandle;
		}

		/// <summary>
		///   <para>Create a PropertySceneHandle representing the new binding on the Component property of a Transform in the scene.</para>
		/// </summary>
		/// <param name="animator">The Animator instance the method is called on.</param>
		/// <param name="transform">The Transform to target.</param>
		/// <param name="type">The Component type.</param>
		/// <param name="property">The property to bind.</param>
		/// <returns>
		///   <para>The PropertySceneHandle representing the new binding.</para>
		/// </returns>
		public static PropertySceneHandle BindSceneProperty(this Animator animator, Transform transform, Type type, string property)
		{
			PropertySceneHandle propertySceneHandle = default(PropertySceneHandle);
			AnimatorJobExtensions.InternalBindSceneProperty(animator, transform, type, property, out propertySceneHandle);
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

		/// <summary>
		///   <para>Newly created handles are always resolved lazily on the next access when the jobs are run. To avoid a cpu spike while evaluating the jobs you can manually resolve all handles from the main thread.</para>
		/// </summary>
		/// <param name="animator">The Animator instance the method is called on.</param>
		public static void ResolveAllStreamHandles(this Animator animator)
		{
			AnimatorJobExtensions.InternalResolveAllStreamHandles(animator);
		}

		/// <summary>
		///   <para>Newly created handles are always resolved lazily on the next access when the jobs are run. To avoid a cpu spike while evaluating the jobs you can manually resolve all handles from the main thread.</para>
		/// </summary>
		/// <param name="animator">The Animator instance the method is called on.</param>
		public static void ResolveAllSceneHandles(this Animator animator)
		{
			AnimatorJobExtensions.InternalResolveAllSceneHandles(animator);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalBindStreamTransform([NotNull] Animator animator, [NotNull] Transform transform, out TransformStreamHandle transformStreamHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalBindStreamProperty([NotNull] Animator animator, [NotNull] Transform transform, [NotNull] Type type, [NotNull] string property, out PropertyStreamHandle propertyStreamHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalBindSceneTransform([NotNull] Animator animator, [NotNull] Transform transform, out TransformSceneHandle transformSceneHandle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InternalBindSceneProperty([NotNull] Animator animator, [NotNull] Transform transform, [NotNull] Type type, [NotNull] string property, out PropertySceneHandle propertySceneHandle);

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
