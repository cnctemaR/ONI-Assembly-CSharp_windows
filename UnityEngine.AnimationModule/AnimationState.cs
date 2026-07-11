using System;
using System.Runtime.CompilerServices;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>The AnimationState gives full control over animation blending.</para>
	/// </summary>
	[UsedByNativeCode]
	public sealed class AnimationState : TrackedReference
	{
		/// <summary>
		///   <para>Enables / disables the animation.</para>
		/// </summary>
		public extern bool enabled
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The weight of animation.</para>
		/// </summary>
		public extern float weight
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Wrapping mode of the animation.</para>
		/// </summary>
		public extern WrapMode wrapMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The current time of the animation.</para>
		/// </summary>
		public extern float time
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The normalized time of the animation.</para>
		/// </summary>
		public extern float normalizedTime
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The playback speed of the animation. 1 is normal playback speed.</para>
		/// </summary>
		public extern float speed
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The normalized playback speed.</para>
		/// </summary>
		public extern float normalizedSpeed
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The length of the animation clip in seconds.</para>
		/// </summary>
		public extern float length
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int layer
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The clip that is being played by this animation state.</para>
		/// </summary>
		public extern AnimationClip clip
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Adds a transform which should be animated. This allows you to reduce the number of animations you have to create.</para>
		/// </summary>
		/// <param name="mix">The transform to animate.</param>
		/// <param name="recursive">Whether to also animate all children of the specified transform.</param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void AddMixingTransform(Transform mix, [DefaultValue("true")] bool recursive);

		/// <summary>
		///   <para>Adds a transform which should be animated. This allows you to reduce the number of animations you have to create.</para>
		/// </summary>
		/// <param name="mix">The transform to animate.</param>
		/// <param name="recursive">Whether to also animate all children of the specified transform.</param>
		[ExcludeFromDocs]
		public void AddMixingTransform(Transform mix)
		{
			bool flag = true;
			this.AddMixingTransform(mix, flag);
		}

		/// <summary>
		///   <para>Removes a transform which should be animated.</para>
		/// </summary>
		/// <param name="mix"></param>
		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RemoveMixingTransform(Transform mix);

		/// <summary>
		///   <para>The name of the animation.</para>
		/// </summary>
		public extern string name
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Which blend mode should be used?</para>
		/// </summary>
		public extern AnimationBlendMode blendMode
		{
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[GeneratedByOldBindingsGenerator]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
