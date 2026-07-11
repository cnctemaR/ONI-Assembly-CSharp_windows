using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Store a collection of Keyframes that can be evaluated over time.</para>
	/// </summary>
	[NativeHeader("Runtime/Math/AnimationCurve.bindings.h")]
	[RequiredByNativeCode]
	[ThreadAndSerializationSafe]
	[StructLayout(LayoutKind.Sequential)]
	public class AnimationCurve
	{
		/// <summary>
		///   <para>Creates an animation curve from an arbitrary number of keyframes.</para>
		/// </summary>
		/// <param name="keys">An array of Keyframes used to define the curve.</param>
		public AnimationCurve(params Keyframe[] keys)
		{
			this.m_Ptr = AnimationCurve.Internal_Create(keys);
		}

		/// <summary>
		///   <para>Creates an empty animation curve.</para>
		/// </summary>
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

		~AnimationCurve()
		{
			AnimationCurve.Internal_Destroy(this.m_Ptr);
		}

		/// <summary>
		///   <para>Evaluate the curve at time.</para>
		/// </summary>
		/// <param name="time">The time within the curve you want to evaluate (the horizontal axis in the curve graph).</param>
		/// <returns>
		///   <para>The value of the curve, at the point in time specified.</para>
		/// </returns>
		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern float Evaluate(float time);

		/// <summary>
		///   <para>All keys defined in the animation curve.</para>
		/// </summary>
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

		/// <summary>
		///   <para>Add a new key to the curve.</para>
		/// </summary>
		/// <param name="time">The time at which to add the key (horizontal axis in the curve graph).</param>
		/// <param name="value">The value for the key (vertical axis in the curve graph).</param>
		/// <returns>
		///   <para>The index of the added key, or -1 if the key could not be added.</para>
		/// </returns>
		[FreeFunction("AnimationCurveBindings::AddKeySmoothTangents", HasExplicitThis = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern int AddKey(float time, float value);

		/// <summary>
		///   <para>Add a new key to the curve.</para>
		/// </summary>
		/// <param name="key">The key to add to the curve.</param>
		/// <returns>
		///   <para>The index of the added key, or -1 if the key could not be added.</para>
		/// </returns>
		public int AddKey(Keyframe key)
		{
			return this.AddKey_Internal(key);
		}

		[NativeMethod("AddKey", IsThreadSafe = true)]
		private int AddKey_Internal(Keyframe key)
		{
			return this.AddKey_Internal_Injected(ref key);
		}

		/// <summary>
		///   <para>Removes the keyframe at index and inserts key.</para>
		/// </summary>
		/// <param name="index">The index of the key to move.</param>
		/// <param name="key">The key (with its new time) to insert.</param>
		/// <returns>
		///   <para>The index of the keyframe after moving it.</para>
		/// </returns>
		[NativeThrows]
		[FreeFunction("AnimationCurveBindings::MoveKey", HasExplicitThis = true, IsThreadSafe = true)]
		public int MoveKey(int index, Keyframe key)
		{
			return this.MoveKey_Injected(index, ref key);
		}

		/// <summary>
		///   <para>Removes a key.</para>
		/// </summary>
		/// <param name="index">The index of the key to remove.</param>
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

		/// <summary>
		///   <para>The number of keys in the curve. (Read Only)</para>
		/// </summary>
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

		/// <summary>
		///   <para>Smooth the in and out tangents of the keyframe at index.</para>
		/// </summary>
		/// <param name="index">The index of the keyframe to be smoothed.</param>
		/// <param name="weight">The smoothing weight to apply to the keyframe's tangents.</param>
		[NativeThrows]
		[FreeFunction("AnimationCurveBindings::SmoothTangents", HasExplicitThis = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SmoothTangents(int index, float weight);

		/// <summary>
		///   <para>Creates a constant "curve" starting at timeStart, ending at timeEnd and with the value value.</para>
		/// </summary>
		/// <param name="timeStart">The start time for the constant curve.</param>
		/// <param name="timeEnd">The start time for the constant curve.</param>
		/// <param name="value">The value for the constant curve.</param>
		/// <returns>
		///   <para>The constant curve created from the specified values.</para>
		/// </returns>
		public static AnimationCurve Constant(float timeStart, float timeEnd, float value)
		{
			return AnimationCurve.Linear(timeStart, value, timeEnd, value);
		}

		/// <summary>
		///   <para>A straight Line starting at timeStart, valueStart and ending at timeEnd, valueEnd.</para>
		/// </summary>
		/// <param name="timeStart">The start time for the linear curve.</param>
		/// <param name="valueStart">The start value for the linear curve.</param>
		/// <param name="timeEnd">The end time for the linear curve.</param>
		/// <param name="valueEnd">The end value for the linear curve.</param>
		/// <returns>
		///   <para>The linear curve created from the specified values.</para>
		/// </returns>
		public static AnimationCurve Linear(float timeStart, float valueStart, float timeEnd, float valueEnd)
		{
			float num = (valueEnd - valueStart) / (timeEnd - timeStart);
			Keyframe[] array = new Keyframe[]
			{
				new Keyframe(timeStart, valueStart, 0f, num),
				new Keyframe(timeEnd, valueEnd, num, 0f)
			};
			return new AnimationCurve(array);
		}

		/// <summary>
		///   <para>Creates an ease-in and out curve starting at timeStart, valueStart and ending at timeEnd, valueEnd.</para>
		/// </summary>
		/// <param name="timeStart">The start time for the ease curve.</param>
		/// <param name="valueStart">The start value for the ease curve.</param>
		/// <param name="timeEnd">The end time for the ease curve.</param>
		/// <param name="valueEnd">The end value for the ease curve.</param>
		/// <returns>
		///   <para>The ease-in and out curve generated from the specified values.</para>
		/// </returns>
		public static AnimationCurve EaseInOut(float timeStart, float valueStart, float timeEnd, float valueEnd)
		{
			Keyframe[] array = new Keyframe[]
			{
				new Keyframe(timeStart, valueStart, 0f, 0f),
				new Keyframe(timeEnd, valueEnd, 0f, 0f)
			};
			return new AnimationCurve(array);
		}

		/// <summary>
		///   <para>The behaviour of the animation before the first keyframe.</para>
		/// </summary>
		public extern WrapMode preWrapMode
		{
			[NativeMethod("GetPreInfinity", IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetPreInfinity", IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		/// <summary>
		///   <para>The behaviour of the animation after the last keyframe.</para>
		/// </summary>
		public extern WrapMode postWrapMode
		{
			[NativeMethod("GetPostInfinity", IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeMethod("SetPostInfinity", IsThreadSafe = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
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
