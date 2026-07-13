using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[NativeHeader("Runtime/Math/AnimationCurve.bindings.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class AnimationCurve : IEquatable<AnimationCurve>
	{
		[FreeFunction("AnimationCurveBindings::Internal_Destroy", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Destroy(IntPtr ptr);

		[FreeFunction("AnimationCurveBindings::Internal_Create", IsThreadSafe = true)]
		private unsafe static IntPtr Internal_Create(Keyframe[] keys)
		{
			Span<Keyframe> span = new Span<Keyframe>(keys);
			IntPtr intPtr;
			fixed (Keyframe* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				intPtr = AnimationCurve.Internal_Create_Injected(ref managedSpanWrapper);
			}
			return intPtr;
		}

		[FreeFunction("AnimationCurveBindings::Internal_Equals", HasExplicitThis = true, IsThreadSafe = true)]
		private bool Internal_Equals(IntPtr other)
		{
			IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AnimationCurve.Internal_Equals_Injected(intPtr, other);
		}

		[FreeFunction("AnimationCurveBindings::Internal_CopyFrom", HasExplicitThis = true, IsThreadSafe = true)]
		private void Internal_CopyFrom(IntPtr other)
		{
			IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AnimationCurve.Internal_CopyFrom_Injected(intPtr, other);
		}

		protected override void Finalize()
		{
			try
			{
				bool requiresNativeCleanup = this.m_RequiresNativeCleanup;
				if (requiresNativeCleanup)
				{
					AnimationCurve.Internal_Destroy(this.m_Ptr);
				}
			}
			finally
			{
				base.Finalize();
			}
		}

		[ThreadSafe]
		public float Evaluate(float time)
		{
			IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AnimationCurve.Evaluate_Injected(intPtr, time);
		}

		public unsafe Keyframe[] keys
		{
			[FreeFunction("AnimationCurveBindings::GetKeysArray", HasExplicitThis = true, IsThreadSafe = true)]
			get
			{
				Keyframe[] array2;
				try
				{
					IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					AnimationCurve.get_keys_Injected(intPtr, out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					Keyframe[] array;
					blittableArrayWrapper.Unmarshal<Keyframe>(ref array);
					array2 = array;
				}
				return array2;
			}
			[FreeFunction("AnimationCurveBindings::SetKeysWithSpan", HasExplicitThis = true, IsThreadSafe = true)]
			set
			{
				IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Span<Keyframe> span = new Span<Keyframe>(value);
				fixed (Keyframe* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					AnimationCurve.set_keys_Injected(intPtr, ref managedSpanWrapper);
				}
			}
		}

		[FreeFunction("AnimationCurveBindings::AddKeySmoothTangents", HasExplicitThis = true, IsThreadSafe = true)]
		public int AddKey(float time, float value)
		{
			IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AnimationCurve.AddKey_Injected(intPtr, time, value);
		}

		public int AddKey(Keyframe key)
		{
			return this.AddKey_Internal(key);
		}

		[NativeMethod("AddKey", IsThreadSafe = true)]
		private int AddKey_Internal(Keyframe key)
		{
			IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AnimationCurve.AddKey_Internal_Injected(intPtr, ref key);
		}

		[NativeThrows]
		[FreeFunction("AnimationCurveBindings::MoveKey", HasExplicitThis = true, IsThreadSafe = true)]
		public int MoveKey(int index, Keyframe key)
		{
			IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AnimationCurve.MoveKey_Injected(intPtr, index, ref key);
		}

		[FreeFunction("AnimationCurveBindings::ClearKeys", HasExplicitThis = true, IsThreadSafe = true)]
		public void ClearKeys()
		{
			IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AnimationCurve.ClearKeys_Injected(intPtr);
		}

		[FreeFunction("AnimationCurveBindings::RemoveKey", HasExplicitThis = true, IsThreadSafe = true)]
		[NativeThrows]
		public void RemoveKey(int index)
		{
			IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AnimationCurve.RemoveKey_Injected(intPtr, index);
		}

		public Keyframe this[int index]
		{
			get
			{
				return this.GetKey(index);
			}
		}

		public int length
		{
			[NativeMethod("GetKeyCount", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationCurve.get_length_Injected(intPtr);
			}
		}

		[FreeFunction("AnimationCurveBindings::GetKey", HasExplicitThis = true, IsThreadSafe = true)]
		[NativeThrows]
		private Keyframe GetKey(int index)
		{
			IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Keyframe keyframe;
			AnimationCurve.GetKey_Injected(intPtr, index, out keyframe);
			return keyframe;
		}

		[FreeFunction("AnimationCurveBindings::GetKeysArray", HasExplicitThis = true, IsThreadSafe = true)]
		private Keyframe[] GetKeysArray()
		{
			Keyframe[] array2;
			try
			{
				IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				AnimationCurve.GetKeysArray_Injected(intPtr, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Keyframe[] array;
				blittableArrayWrapper.Unmarshal<Keyframe>(ref array);
				array2 = array;
			}
			return array2;
		}

		public void GetKeys(Span<Keyframe> keys)
		{
			int length = this.length;
			bool flag = length > keys.Length;
			if (flag)
			{
				throw new ArgumentException("Destination array must be large enough to store the keys", "keys");
			}
			this.GetKeysWithSpan(keys);
		}

		[SecurityCritical]
		[FreeFunction(Name = "AnimationCurveBindings::GetKeysWithSpan", HasExplicitThis = true, IsThreadSafe = true)]
		private unsafe void GetKeysWithSpan(Span<Keyframe> keys)
		{
			IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Keyframe> span = keys;
			fixed (Keyframe* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				AnimationCurve.GetKeysWithSpan_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		[FreeFunction("AnimationCurveBindings::SetKeysWithSpan", HasExplicitThis = true, IsThreadSafe = true)]
		public unsafe void SetKeys(ReadOnlySpan<Keyframe> keys)
		{
			IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			ReadOnlySpan<Keyframe> readOnlySpan = keys;
			fixed (Keyframe* pinnableReference = readOnlySpan.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, readOnlySpan.Length);
				AnimationCurve.SetKeys_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		[FreeFunction("AnimationCurveBindings::GetHashCode", HasExplicitThis = true, IsThreadSafe = true)]
		public override int GetHashCode()
		{
			IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			return AnimationCurve.GetHashCode_Injected(intPtr);
		}

		[FreeFunction("AnimationCurveBindings::SmoothTangents", HasExplicitThis = true, IsThreadSafe = true)]
		[NativeThrows]
		public void SmoothTangents(int index, float weight)
		{
			IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			AnimationCurve.SmoothTangents_Injected(intPtr, index, weight);
		}

		public static AnimationCurve Constant(float timeStart, float timeEnd, float value)
		{
			return AnimationCurve.Linear(timeStart, value, timeEnd, value);
		}

		public static AnimationCurve Linear(float timeStart, float valueStart, float timeEnd, float valueEnd)
		{
			bool flag = timeStart == timeEnd;
			AnimationCurve animationCurve;
			if (flag)
			{
				Keyframe keyframe = new Keyframe(timeStart, valueStart);
				animationCurve = new AnimationCurve(new Keyframe[] { keyframe });
			}
			else
			{
				float num = (valueEnd - valueStart) / (timeEnd - timeStart);
				Keyframe[] array = new Keyframe[]
				{
					new Keyframe(timeStart, valueStart, 0f, num),
					new Keyframe(timeEnd, valueEnd, num, 0f)
				};
				animationCurve = new AnimationCurve(array);
			}
			return animationCurve;
		}

		public static AnimationCurve EaseInOut(float timeStart, float valueStart, float timeEnd, float valueEnd)
		{
			bool flag = timeStart == timeEnd;
			AnimationCurve animationCurve;
			if (flag)
			{
				Keyframe keyframe = new Keyframe(timeStart, valueStart);
				animationCurve = new AnimationCurve(new Keyframe[] { keyframe });
			}
			else
			{
				Keyframe[] array = new Keyframe[]
				{
					new Keyframe(timeStart, valueStart, 0f, 0f),
					new Keyframe(timeEnd, valueEnd, 0f, 0f)
				};
				animationCurve = new AnimationCurve(array);
			}
			return animationCurve;
		}

		public WrapMode preWrapMode
		{
			[NativeMethod("GetPreInfinity", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationCurve.get_preWrapMode_Injected(intPtr);
			}
			[NativeMethod("SetPreInfinity", IsThreadSafe = true)]
			set
			{
				IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AnimationCurve.set_preWrapMode_Injected(intPtr, value);
			}
		}

		public WrapMode postWrapMode
		{
			[NativeMethod("GetPostInfinity", IsThreadSafe = true)]
			get
			{
				IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return AnimationCurve.get_postWrapMode_Injected(intPtr);
			}
			[NativeMethod("SetPostInfinity", IsThreadSafe = true)]
			set
			{
				IntPtr intPtr = AnimationCurve.BindingsMarshaller.ConvertToNative(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				AnimationCurve.set_postWrapMode_Injected(intPtr, value);
			}
		}

		public AnimationCurve(params Keyframe[] keys)
		{
			this.m_Ptr = AnimationCurve.Internal_Create(keys);
			this.m_RequiresNativeCleanup = true;
		}

		[RequiredByNativeCode]
		public AnimationCurve()
		{
			this.m_Ptr = AnimationCurve.Internal_Create(null);
			this.m_RequiresNativeCleanup = true;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.ParticleSystemModule" })]
		internal AnimationCurve(IntPtr ptr, bool ownMemory)
		{
			this.m_Ptr = ptr;
			this.m_RequiresNativeCleanup = ownMemory;
		}

		public override bool Equals(object o)
		{
			bool flag = o == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = this == o;
				flag2 = flag3 || (o.GetType() == base.GetType() && this.Equals((AnimationCurve)o));
			}
			return flag2;
		}

		public bool Equals(AnimationCurve other)
		{
			bool flag = other == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = this == other;
				flag2 = flag3 || this.m_Ptr.Equals(other.m_Ptr) || this.Internal_Equals(other.m_Ptr);
			}
			return flag2;
		}

		public void CopyFrom(AnimationCurve other)
		{
			this.Internal_CopyFrom(other.m_Ptr);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create_Injected(ref ManagedSpanWrapper keys);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_Equals_Injected(IntPtr _unity_self, IntPtr other);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_CopyFrom_Injected(IntPtr _unity_self, IntPtr other);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern float Evaluate_Injected(IntPtr _unity_self, float time);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_keys_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_keys_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int AddKey_Injected(IntPtr _unity_self, float time, float value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int AddKey_Internal_Injected(IntPtr _unity_self, [In] ref Keyframe key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int MoveKey_Injected(IntPtr _unity_self, int index, [In] ref Keyframe key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ClearKeys_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void RemoveKey_Injected(IntPtr _unity_self, int index);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_length_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetKey_Injected(IntPtr _unity_self, int index, out Keyframe ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetKeysArray_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetKeysWithSpan_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keys);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetKeys_Injected(IntPtr _unity_self, ref ManagedSpanWrapper keys);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetHashCode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SmoothTangents_Injected(IntPtr _unity_self, int index, float weight);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern WrapMode get_preWrapMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_preWrapMode_Injected(IntPtr _unity_self, WrapMode value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern WrapMode get_postWrapMode_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_postWrapMode_Injected(IntPtr _unity_self, WrapMode value);

		[VisibleToOtherModules(new string[] { "UnityEngine.ParticleSystemModule" })]
		internal IntPtr m_Ptr;

		private bool m_RequiresNativeCleanup;

		internal static class BindingsMarshaller
		{
			public static AnimationCurve ConvertToManaged(IntPtr ptr)
			{
				return new AnimationCurve(ptr, true);
			}

			public static IntPtr ConvertToNative(AnimationCurve animationCurve)
			{
				return animationCurve.m_Ptr;
			}
		}
	}
}
