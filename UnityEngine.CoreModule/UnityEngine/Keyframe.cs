using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>A single keyframe that can be injected into an animation curve.</para>
	/// </summary>
	[RequiredByNativeCode]
	public struct Keyframe
	{
		/// <summary>
		///   <para>Create a keyframe.</para>
		/// </summary>
		/// <param name="time"></param>
		/// <param name="value"></param>
		public Keyframe(float time, float value)
		{
			this.m_Time = time;
			this.m_Value = value;
			this.m_InTangent = 0f;
			this.m_OutTangent = 0f;
			this.m_WeightedMode = 0;
			this.m_InWeight = 0f;
			this.m_OutWeight = 0f;
		}

		/// <summary>
		///   <para>Create a keyframe.</para>
		/// </summary>
		/// <param name="time"></param>
		/// <param name="value"></param>
		/// <param name="inTangent"></param>
		/// <param name="outTangent"></param>
		public Keyframe(float time, float value, float inTangent, float outTangent)
		{
			this.m_Time = time;
			this.m_Value = value;
			this.m_InTangent = inTangent;
			this.m_OutTangent = outTangent;
			this.m_WeightedMode = 0;
			this.m_InWeight = 0f;
			this.m_OutWeight = 0f;
		}

		/// <summary>
		///   <para>Create a keyframe.</para>
		/// </summary>
		/// <param name="time"></param>
		/// <param name="value"></param>
		/// <param name="inTangent"></param>
		/// <param name="outTangent"></param>
		/// <param name="inWeight"></param>
		/// <param name="outWeight"></param>
		public Keyframe(float time, float value, float inTangent, float outTangent, float inWeight, float outWeight)
		{
			this.m_Time = time;
			this.m_Value = value;
			this.m_InTangent = inTangent;
			this.m_OutTangent = outTangent;
			this.m_WeightedMode = 3;
			this.m_InWeight = inWeight;
			this.m_OutWeight = outWeight;
		}

		/// <summary>
		///   <para>The time of the keyframe.</para>
		/// </summary>
		public float time
		{
			get
			{
				return this.m_Time;
			}
			set
			{
				this.m_Time = value;
			}
		}

		/// <summary>
		///   <para>The value of the curve at keyframe.</para>
		/// </summary>
		public float value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.m_Value = value;
			}
		}

		/// <summary>
		///   <para>Sets the incoming tangent for this key. The incoming tangent affects the slope of the curve from the previous key to this key.</para>
		/// </summary>
		public float inTangent
		{
			get
			{
				return this.m_InTangent;
			}
			set
			{
				this.m_InTangent = value;
			}
		}

		/// <summary>
		///   <para>Sets the outgoing tangent for this key. The outgoing tangent affects the slope of the curve from this key to the next key.</para>
		/// </summary>
		public float outTangent
		{
			get
			{
				return this.m_OutTangent;
			}
			set
			{
				this.m_OutTangent = value;
			}
		}

		/// <summary>
		///   <para>Sets the incoming weight for this key. The incoming weight affects the slope of the curve from the previous key to this key.</para>
		/// </summary>
		public float inWeight
		{
			get
			{
				return this.m_InWeight;
			}
			set
			{
				this.m_InWeight = value;
			}
		}

		/// <summary>
		///   <para>Sets the outgoing weight for this key. The outgoing weight affects the slope of the curve from this key to the next key.</para>
		/// </summary>
		public float outWeight
		{
			get
			{
				return this.m_OutWeight;
			}
			set
			{
				this.m_OutWeight = value;
			}
		}

		/// <summary>
		///   <para>Weighted mode for the keyframe.</para>
		/// </summary>
		public WeightedMode weightedMode
		{
			get
			{
				return (WeightedMode)this.m_WeightedMode;
			}
			set
			{
				this.m_WeightedMode = (int)value;
			}
		}

		/// <summary>
		///   <para>TangentMode is deprecated.  Use AnimationUtility.SetKeyLeftTangentMode or AnimationUtility.SetKeyRightTangentMode instead.</para>
		/// </summary>
		[Obsolete("Use AnimationUtility.SetLeftTangentMode, AnimationUtility.SetRightTangentMode, AnimationUtility.GetLeftTangentMode or AnimationUtility.GetRightTangentMode instead.")]
		public int tangentMode
		{
			get
			{
				return this.tangentModeInternal;
			}
			set
			{
				this.tangentModeInternal = value;
			}
		}

		internal int tangentModeInternal
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		private float m_Time;

		private float m_Value;

		private float m_InTangent;

		private float m_OutTangent;

		private int m_WeightedMode;

		private float m_InWeight;

		private float m_OutWeight;
	}
}
