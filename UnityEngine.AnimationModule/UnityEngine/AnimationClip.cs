using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Animation/ScriptBindings/AnimationClip.bindings.h")]
	[NativeType("Modules/Animation/AnimationClip.h")]
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
		[NativeHeader("Modules/Animation/AnimationUtility.h")]
		internal static void SampleAnimation([NotNull] GameObject go, [NotNull] AnimationClip clip, float inTime, WrapMode wrapMode)
		{
			if (go == null)
			{
				ThrowHelper.ThrowArgumentNullException(go, "go");
			}
			if (clip == null)
			{
				ThrowHelper.ThrowArgumentNullException(clip, "clip");
			}
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<GameObject>(go);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowArgumentNullException(go, "go");
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(clip);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(clip, "clip");
			}
			AnimationClip.SampleAnimation_Injected(intPtr, intPtr2, inTime, wrapMode);
		}

		[NativeProperty("Length", false, TargetType.Function)]
		public float length
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationClip.get_length_Injected(intPtr);
			}
		}

		[NativeProperty("StartTime", false, TargetType.Function)]
		internal float startTime
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationClip.get_startTime_Injected(intPtr);
			}
		}

		[NativeProperty("StopTime", false, TargetType.Function)]
		internal float stopTime
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationClip.get_stopTime_Injected(intPtr);
			}
		}

		[NativeProperty("SampleRate", false, TargetType.Function)]
		public float frameRate
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationClip.get_frameRate_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AnimationClip.set_frameRate_Injected(intPtr, value);
			}
		}

		[FreeFunction("AnimationClipBindings::Internal_SetCurve", HasExplicitThis = true)]
		public unsafe void SetCurve([NotNull] string relativePath, [NotNull] Type type, [NotNull] string propertyName, AnimationCurve curve)
		{
			if (relativePath == null)
			{
				ThrowHelper.ThrowArgumentNullException(relativePath, "relativePath");
			}
			if (type == null)
			{
				ThrowHelper.ThrowArgumentNullException(type, "type");
			}
			if (propertyName == null)
			{
				ThrowHelper.ThrowArgumentNullException(propertyName, "propertyName");
			}
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(relativePath, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = relativePath.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(propertyName, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = propertyName.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				AnimationClip.SetCurve_Injected(intPtr, ref managedSpanWrapper, type, ref managedSpanWrapper2, (curve == null) ? ((IntPtr)0) : AnimationCurve.BindingsMarshaller.ConvertToNative(curve));
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
		}

		public void EnsureQuaternionContinuity()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AnimationClip.EnsureQuaternionContinuity_Injected(intPtr);
		}

		public void ClearCurves()
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AnimationClip.ClearCurves_Injected(intPtr);
		}

		[NativeProperty("WrapMode", false, TargetType.Function)]
		public WrapMode wrapMode
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationClip.get_wrapMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AnimationClip.set_wrapMode_Injected(intPtr, value);
			}
		}

		[NativeProperty("Bounds", false, TargetType.Function)]
		public Bounds localBounds
		{
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Bounds bounds;
				AnimationClip.get_localBounds_Injected(intPtr, out bounds);
				return bounds;
			}
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AnimationClip.set_localBounds_Injected(intPtr, ref value);
			}
		}

		public new bool legacy
		{
			[NativeMethod("IsLegacy")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationClip.get_legacy_Injected(intPtr);
			}
			[NativeMethod("SetLegacy")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AnimationClip.set_legacy_Injected(intPtr, value);
			}
		}

		public bool humanMotion
		{
			[NativeMethod("IsHumanMotion")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationClip.get_humanMotion_Injected(intPtr);
			}
		}

		public bool empty
		{
			[NativeMethod("IsEmpty")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationClip.get_empty_Injected(intPtr);
			}
		}

		public bool hasGenericRootTransform
		{
			[NativeMethod("HasGenericRootTransform")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationClip.get_hasGenericRootTransform_Injected(intPtr);
			}
		}

		public bool hasMotionFloatCurves
		{
			[NativeMethod("HasMotionFloatCurves")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationClip.get_hasMotionFloatCurves_Injected(intPtr);
			}
		}

		public bool hasMotionCurves
		{
			[NativeMethod("HasMotionCurves")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationClip.get_hasMotionCurves_Injected(intPtr);
			}
		}

		public bool hasRootCurves
		{
			[NativeMethod("HasRootCurves")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationClip.get_hasRootCurves_Injected(intPtr);
			}
		}

		internal bool hasRootMotion
		{
			[FreeFunction(Name = "AnimationClipBindings::Internal_GetHasRootMotion", HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationClip.get_hasRootMotion_Injected(intPtr);
			}
		}

		public void AddEvent(AnimationEvent evt)
		{
			bool flag = evt == null;
			if (flag)
			{
				throw new ArgumentNullException("evt");
			}
			AnimationEventBlittable animationEventBlittable = AnimationEventBlittable.FromAnimationEvent(evt);
			this.AddEventInternal(animationEventBlittable);
			animationEventBlittable.Dispose();
		}

		[FreeFunction(Name = "AnimationClipBindings::AddEventInternal", HasExplicitThis = true)]
		private void AddEventInternal(object evt)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AnimationClip.AddEventInternal_Injected(intPtr, evt);
		}

		public unsafe AnimationEvent[] events
		{
			get
			{
				IntPtr intPtr;
				int num;
				this.GetEventsInternal(out intPtr, out num);
				AnimationEvent[] array = AnimationEventBlittable.PointerToAnimationEvents(intPtr, num);
				AnimationEventBlittable.DisposeEvents(intPtr, num);
				return array;
			}
			set
			{
				using (NativeArray<AnimationEventBlittable> nativeArray = new NativeArray<AnimationEventBlittable>(value.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory))
				{
					AnimationEventBlittable* ptr = (AnimationEventBlittable*)nativeArray.GetUnsafePtr<AnimationEventBlittable>();
					AnimationEventBlittable.FromAnimationEvents(value, ptr);
					this.SetEventsInternal((void*)ptr, nativeArray.Length);
					for (int i = 0; i < value.Length; i++)
					{
						ptr->Dispose();
						ptr++;
					}
				}
			}
		}

		[FreeFunction(Name = "AnimationClipBindings::SetEventsInternal", HasExplicitThis = true)]
		private unsafe void SetEventsInternal(void* data, int length)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AnimationClip.SetEventsInternal_Injected(intPtr, data, length);
		}

		[FreeFunction(Name = "AnimationClipBindings::GetEventsInternal", HasExplicitThis = true)]
		private void GetEventsInternal(out IntPtr values, out int size)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<AnimationClip>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AnimationClip.GetEventsInternal_Injected(intPtr, out values, out size);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SampleAnimation_Injected(IntPtr go, IntPtr clip, float inTime, WrapMode wrapMode);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_length_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_startTime_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_stopTime_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_frameRate_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_frameRate_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetCurve_Injected(IntPtr _unity_self, ref ManagedSpanWrapper relativePath, Type type, ref ManagedSpanWrapper propertyName, IntPtr curve);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void EnsureQuaternionContinuity_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearCurves_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern WrapMode get_wrapMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_wrapMode_Injected(IntPtr _unity_self, WrapMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_localBounds_Injected(IntPtr _unity_self, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_localBounds_Injected(IntPtr _unity_self, [In] ref Bounds value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_legacy_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_legacy_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_humanMotion_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_empty_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_hasGenericRootTransform_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_hasMotionFloatCurves_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_hasMotionCurves_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_hasRootCurves_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_hasRootMotion_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddEventInternal_Injected(IntPtr _unity_self, object evt);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void SetEventsInternal_Injected(IntPtr _unity_self, void* data, int length);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetEventsInternal_Injected(IntPtr _unity_self, out IntPtr values, out int size);
	}
}
