using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeType("Runtime/Animation/AnimationClip.h")]
	[NativeHeader("Runtime/Animation/ScriptBindings/AnimationClip.bindings.h")]
	public sealed class AnimationClip : Motion
	{
		public AnimationClip()
		{
			AnimationClip.Internal_CreateAnimationClip(this);
		}

		[FreeFunction("AnimationClipBindings::Internal_CreateAnimationClip")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CreateAnimationClip([Writable] AnimationClip self);

		public void SampleAnimation(GameObject go, float time)
		{
			AnimationClip.SampleAnimation(go, this, time, this.wrapMode);
		}

		[FreeFunction]
		[NativeHeader("Runtime/Animation/AnimationUtility.h")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SampleAnimation([NotNull] GameObject go, [NotNull] AnimationClip clip, float inTime, WrapMode wrapMode);

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

		[NativeProperty("SampleRate", false, TargetType.Function)]
		public extern float frameRate
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[FreeFunction("AnimationClipBindings::Internal_SetCurve", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetCurve([NotNull] string relativePath, [NotNull] Type type, [NotNull] string propertyName, AnimationCurve curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void EnsureQuaternionContinuity();

		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ClearCurves();

		[NativeProperty("WrapMode", false, TargetType.Function)]
		public extern WrapMode wrapMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

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

		public new extern bool legacy
		{
			[NativeMethod("IsLegacy")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetLegacy")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool humanMotion
		{
			[NativeMethod("IsHumanMotion")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool empty
		{
			[NativeMethod("IsEmpty")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool hasGenericRootTransform
		{
			[NativeMethod("HasGenericRootTransform")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool hasMotionFloatCurves
		{
			[NativeMethod("HasMotionFloatCurves")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool hasMotionCurves
		{
			[NativeMethod("HasMotionCurves")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern bool hasRootCurves
		{
			[NativeMethod("HasRootCurves")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal extern bool hasRootMotion
		{
			[FreeFunction(Name = "AnimationClipBindings::Internal_GetHasRootMotion", HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

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

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_localBounds_Injected(out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_localBounds_Injected(ref Bounds value);
	}
}
