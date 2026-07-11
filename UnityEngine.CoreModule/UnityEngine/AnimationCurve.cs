using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[ThreadAndSerializationSafe]
	[RequiredByNativeCode]
	[NativeHeader("Runtime/Math/AnimationCurve.bindings.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class AnimationCurve : IEquatable<AnimationCurve>
	{
		public AnimationCurve(params Keyframe[] keys)
		{
			this.m_Ptr = AnimationCurve.Internal_Create(keys);
		}

		[RequiredByNativeCode]
		public AnimationCurve()
		{
			this.m_Ptr = AnimationCurve.Internal_Create(null);
		}

		[FreeFunction("AnimationCurveBindings::Internal_Destroy", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Destroy(IntPtr ptr);

		[FreeFunction("AnimationCurveBindings::Internal_Create", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create(Keyframe[] keys);

		[FreeFunction("AnimationCurveBindings::Internal_Equals", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool Internal_Equals(IntPtr other);

		~AnimationCurve()
		{
			AnimationCurve.Internal_Destroy(this.m_Ptr);
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float Evaluate(float time);

		public Keyframe[] keys
		{
			get
			{
				return this.GetKeys();
			}
			set
			{
				this.SetKeys(value);
			}
		}

		[FreeFunction("AnimationCurveBindings::AddKeySmoothTangents", HasExplicitThis = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int AddKey(float time, float value);

		public int AddKey(Keyframe key)
		{
			return this.AddKey_Internal(key);
		}

		[NativeMethod("AddKey", IsThreadSafe = true)]
		private int AddKey_Internal(Keyframe key)
		{
			return this.AddKey_Internal_Injected(ref key);
		}

		[FreeFunction("AnimationCurveBindings::MoveKey", HasExplicitThis = true, IsThreadSafe = true)]
		[NativeThrows]
		public int MoveKey(int index, Keyframe key)
		{
			return this.MoveKey_Injected(index, ref key);
		}

		[NativeThrows]
		[FreeFunction("AnimationCurveBindings::RemoveKey", HasExplicitThis = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RemoveKey(int index);

		public Keyframe this[int index]
		{
			get
			{
				return this.GetKey(index);
			}
		}

		public extern int length
		{
			[NativeMethod("GetKeyCount", IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction("AnimationCurveBindings::SetKeys", HasExplicitThis = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetKeys(Keyframe[] keys);

		[NativeThrows]
		[FreeFunction("AnimationCurveBindings::GetKey", HasExplicitThis = true, IsThreadSafe = true)]
		private Keyframe GetKey(int index)
		{
			Keyframe keyframe;
			this.GetKey_Injected(index, out keyframe);
			return keyframe;
		}

		[FreeFunction("AnimationCurveBindings::GetKeys", HasExplicitThis = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern Keyframe[] GetKeys();

		[NativeThrows]
		[FreeFunction("AnimationCurveBindings::SmoothTangents", HasExplicitThis = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SmoothTangents(int index, float weight);

		public static AnimationCurve Constant(float timeStart, float timeEnd, float value)
		{
			return AnimationCurve.Linear(timeStart, value, timeEnd, value);
		}

		public static AnimationCurve Linear(float timeStart, float valueStart, float timeEnd, float valueEnd)
		{
			AnimationCurve animationCurve;
			if (timeStart == timeEnd)
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
			AnimationCurve animationCurve;
			if (timeStart == timeEnd)
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

		public extern WrapMode preWrapMode
		{
			[NativeMethod("GetPreInfinity", IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetPreInfinity", IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern WrapMode postWrapMode
		{
			[NativeMethod("GetPostInfinity", IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetPostInfinity", IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public override bool Equals(object o)
		{
			return !object.ReferenceEquals(null, o) && (object.ReferenceEquals(this, o) || (o.GetType() == base.GetType() && this.Equals((AnimationCurve)o)));
		}

		public bool Equals(AnimationCurve other)
		{
			return !object.ReferenceEquals(null, other) && (object.ReferenceEquals(this, other) || this.m_Ptr.Equals(other.m_Ptr) || this.Internal_Equals(other.m_Ptr));
		}

		public override int GetHashCode()
		{
			return this.m_Ptr.GetHashCode();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int AddKey_Internal_Injected(ref Keyframe key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern int MoveKey_Injected(int index, ref Keyframe key);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetKey_Injected(int index, out Keyframe ret);

		internal IntPtr m_Ptr;
	}
}
