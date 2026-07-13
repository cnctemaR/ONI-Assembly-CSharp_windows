using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/Animation/AnimationState.h")]
	[UsedByNativeCode]
	public sealed class AnimationState : TrackedReference
	{
		public bool enabled
		{
			get
			{
				IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationState.get_enabled_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AnimationState.set_enabled_Injected(intPtr, value);
			}
		}

		public float weight
		{
			get
			{
				IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationState.get_weight_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AnimationState.set_weight_Injected(intPtr, value);
			}
		}

		public WrapMode wrapMode
		{
			get
			{
				IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationState.get_wrapMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AnimationState.set_wrapMode_Injected(intPtr, value);
			}
		}

		public float time
		{
			get
			{
				IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationState.get_time_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AnimationState.set_time_Injected(intPtr, value);
			}
		}

		public float normalizedTime
		{
			get
			{
				IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationState.get_normalizedTime_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AnimationState.set_normalizedTime_Injected(intPtr, value);
			}
		}

		public float speed
		{
			get
			{
				IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationState.get_speed_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AnimationState.set_speed_Injected(intPtr, value);
			}
		}

		public float normalizedSpeed
		{
			get
			{
				IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationState.get_normalizedSpeed_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AnimationState.set_normalizedSpeed_Injected(intPtr, value);
			}
		}

		public float length
		{
			get
			{
				IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationState.get_length_Injected(intPtr);
			}
		}

		public int layer
		{
			get
			{
				IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationState.get_layer_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AnimationState.set_layer_Injected(intPtr, value);
			}
		}

		public AnimationClip clip
		{
			get
			{
				IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return Unmarshal.UnmarshalUnityObject<AnimationClip>(AnimationState.get_clip_Injected(intPtr));
			}
		}

		public unsafe string name
		{
			get
			{
				string stringAndDispose;
				try
				{
					IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					ManagedSpanWrapper managedSpanWrapper;
					AnimationState.get_name_Injected(intPtr, out managedSpanWrapper);
				}
				finally
				{
					ManagedSpanWrapper managedSpanWrapper;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
				}
				return stringAndDispose;
			}
			set
			{
				try
				{
					IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
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
					AnimationState.set_name_Injected(intPtr, ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
		}

		public AnimationBlendMode blendMode
		{
			get
			{
				IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationState.get_blendMode_Injected(intPtr);
			}
			set
			{
				IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AnimationState.set_blendMode_Injected(intPtr, value);
			}
		}

		[ExcludeFromDocs]
		public void AddMixingTransform(Transform mix)
		{
			this.AddMixingTransform(mix, true);
		}

		public void AddMixingTransform([NotNull] Transform mix, [DefaultValue("true")] bool recursive)
		{
			if (mix == null)
			{
				ThrowHelper.ThrowArgumentNullException(mix, "mix");
			}
			IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Transform>(mix);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(mix, "mix");
			}
			AnimationState.AddMixingTransform_Injected(intPtr, intPtr2, recursive);
		}

		public void RemoveMixingTransform([NotNull] Transform mix)
		{
			if (mix == null)
			{
				ThrowHelper.ThrowArgumentNullException(mix, "mix");
			}
			IntPtr intPtr = AnimationState.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			IntPtr intPtr2 = Object.MarshalledUnityObject.MarshalNotNull<Transform>(mix);
			if (intPtr2 == 0)
			{
				ThrowHelper.ThrowArgumentNullException(mix, "mix");
			}
			AnimationState.RemoveMixingTransform_Injected(intPtr, intPtr2);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool get_enabled_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_enabled_Injected(IntPtr _unity_self, bool value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_weight_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_weight_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern WrapMode get_wrapMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_wrapMode_Injected(IntPtr _unity_self, WrapMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_time_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_time_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_normalizedTime_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_normalizedTime_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_speed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_speed_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_normalizedSpeed_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_normalizedSpeed_Injected(IntPtr _unity_self, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float get_length_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_layer_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_layer_Injected(IntPtr _unity_self, int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr get_clip_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_name_Injected(IntPtr _unity_self, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_name_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AnimationBlendMode get_blendMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_blendMode_Injected(IntPtr _unity_self, AnimationBlendMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddMixingTransform_Injected(IntPtr _unity_self, IntPtr mix, [DefaultValue("true")] bool recursive);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveMixingTransform_Injected(IntPtr _unity_self, IntPtr mix);

		internal static class BindingsMarshaller
		{
			public static IntPtr ConvertToNative(AnimationState animationState)
			{
				return animationState.m_Ptr;
			}
		}
	}
}
