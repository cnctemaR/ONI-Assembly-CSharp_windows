using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Stores keyframe based animations.</para>
	/// </summary>
	[NativeHeader("Runtime/Animation/ScriptBindings/AnimationClip.bindings.h")]
	[NativeType("Runtime/Animation/AnimationClip.h")]
	public sealed class AnimationClip : Motion
	{
		/// <summary>
		///   <para>Creates a new animation clip.</para>
		/// </summary>
		public AnimationClip()
		{
			AnimationClip.Internal_CreateAnimationClip(this);
		}

		/// <summary>
		///   <para>Adds an animation event to the clip.</para>
		/// </summary>
		/// <param name="evt">AnimationEvent to add.</param>
		public void AddEvent(AnimationEvent evt)
		{
			if (evt == null)
			{
				throw new ArgumentNullException("evt");
			}
			this.AddEventInternal(evt);
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void AddEventInternal(object evt);

		/// <summary>
		///   <para>Animation Events for this animation clip.</para>
		/// </summary>
		public AnimationEvent[] events
		{
			get
			{
				return (AnimationEvent[])this.GetEventsInternal();
			}
			set
			{
				this.SetEventsInternal(value);
			}
		}

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetEventsInternal(Array value);

		[GeneratedByOldBindingsGenerator]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Array GetEventsInternal();

		[FreeFunction("AnimationClipBindings::Internal_CreateAnimationClip")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateAnimationClip([Writable] AnimationClip self);

		/// <summary>
		///   <para>Samples an animation at a given time for any animated properties.</para>
		/// </summary>
		/// <param name="go">The animated game object.</param>
		/// <param name="time">The time to sample an animation.</param>
		public void SampleAnimation(GameObject go, float time)
		{
			AnimationClip.SampleAnimation(go, this, time, this.wrapMode);
		}

		[NativeHeader("Runtime/Animation/AnimationUtility.h")]
		[FreeFunction]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SampleAnimation([NotNull] GameObject go, [NotNull] AnimationClip clip, float inTime, WrapMode wrapMode);

		/// <summary>
		///   <para>Animation length in seconds. (Read Only)</para>
		/// </summary>
		[NativeProperty("Length", false, TargetType.Function)]
		public extern float length
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeProperty("StartTime", false, TargetType.Function)]
		internal extern float startTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeProperty("StopTime", false, TargetType.Function)]
		internal extern float stopTime
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Frame rate at which keyframes are sampled. (Read Only)</para>
		/// </summary>
		[NativeProperty("SampleRate", false, TargetType.Function)]
		public extern float frameRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Assigns the curve to animate a specific property.</para>
		/// </summary>
		/// <param name="relativePath">Path to the game object this curve applies to. The relativePath
		///   is formatted similar to a pathname, e.g. "rootspineleftArm".  If relativePath
		///   is empty it refers to the game object the animation clip is attached to.</param>
		/// <param name="type">The class type of the component that is animated.</param>
		/// <param name="propertyName">The name or path to the property being animated.</param>
		/// <param name="curve">The animation curve.</param>
		[FreeFunction("AnimationClipBindings::Internal_SetCurve", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetCurve([NotNull] string relativePath, Type type, [NotNull] string propertyName, AnimationCurve curve);

		/// <summary>
		///   <para>Realigns quaternion keys to ensure shortest interpolation paths.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void EnsureQuaternionContinuity();

		/// <summary>
		///   <para>Clears all curves from the clip.</para>
		/// </summary>
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearCurves();

		/// <summary>
		///   <para>Sets the default wrap mode used in the animation state.</para>
		/// </summary>
		[NativeProperty("WrapMode", false, TargetType.Function)]
		public extern WrapMode wrapMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>AABB of this Animation Clip in local space of Animation component that it is attached too.</para>
		/// </summary>
		[NativeProperty("Bounds", false, TargetType.Function)]
		public Bounds localBounds
		{
			get
			{
				Bounds bounds;
				this.get_localBounds_Injected(out bounds);
				return bounds;
			}
			set
			{
				this.set_localBounds_Injected(ref value);
			}
		}

		/// <summary>
		///   <para>Set to true if the AnimationClip will be used with the Legacy Animation component ( instead of the Animator ).</para>
		/// </summary>
		public new extern bool legacy
		{
			[NativeMethod("IsLegacy")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetLegacy")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>Returns true if the animation contains curve that drives a humanoid rig.</para>
		/// </summary>
		public extern bool humanMotion
		{
			[NativeMethod("IsHumanMotion")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		/// <summary>
		///   <para>Returns true if the animation clip has no curves and no events.</para>
		/// </summary>
		public extern bool empty
		{
			[NativeMethod("IsEmpty")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool hasRootMotion
		{
			[FreeFunction(Name = "AnimationClipBindings::Internal_GetHasRootMotion", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_localBounds_Injected(out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_localBounds_Injected(ref Bounds value);
	}
}
