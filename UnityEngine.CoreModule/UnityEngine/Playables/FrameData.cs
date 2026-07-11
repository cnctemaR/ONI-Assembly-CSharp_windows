using System;

namespace UnityEngine.Playables
{
	/// <summary>
	///   <para>This structure contains the frame information a Playable receives in Playable.PrepareFrame.</para>
	/// </summary>
	public struct FrameData
	{
		/// <summary>
		///   <para>The current frame identifier.</para>
		/// </summary>
		public ulong frameId
		{
			get
			{
				return this.m_FrameID;
			}
		}

		/// <summary>
		///   <para>Time difference between this frame and the preceding frame.</para>
		/// </summary>
		public float deltaTime
		{
			get
			{
				return (float)this.m_DeltaTime;
			}
		}

		/// <summary>
		///   <para>The weight of the current Playable.</para>
		/// </summary>
		public float weight
		{
			get
			{
				return this.m_Weight;
			}
		}

		/// <summary>
		///   <para>The accumulated weight of the Playable during the PlayableGraph traversal.</para>
		/// </summary>
		public float effectiveWeight
		{
			get
			{
				return this.m_EffectiveWeight;
			}
		}

		/// <summary>
		///   <para>The accumulated delay of the parent Playable during the PlayableGraph traversal.</para>
		/// </summary>
		public double effectiveParentDelay
		{
			get
			{
				return this.m_EffectiveParentDelay;
			}
		}

		/// <summary>
		///   <para>The accumulated speed of the parent Playable during the PlayableGraph traversal.</para>
		/// </summary>
		public float effectiveParentSpeed
		{
			get
			{
				return this.m_EffectiveParentSpeed;
			}
		}

		/// <summary>
		///   <para>The accumulated speed of the Playable during the PlayableGraph traversal.</para>
		/// </summary>
		public float effectiveSpeed
		{
			get
			{
				return this.m_EffectiveSpeed;
			}
		}

		/// <summary>
		///   <para>Indicates the type of evaluation that caused PlayableGraph.PrepareFrame to be called.</para>
		/// </summary>
		public FrameData.EvaluationType evaluationType
		{
			get
			{
				return ((this.m_Flags & FrameData.Flags.Evaluate) == (FrameData.Flags)0) ? FrameData.EvaluationType.Playback : FrameData.EvaluationType.Evaluate;
			}
		}

		/// <summary>
		///   <para>Indicates that the local time was explicitly set.</para>
		/// </summary>
		public bool seekOccurred
		{
			get
			{
				return (this.m_Flags & FrameData.Flags.SeekOccured) != (FrameData.Flags)0;
			}
		}

		/// <summary>
		///   <para>Indicates the local time wrapped because it has reached the duration and the extrapolation mode is set to Loop.</para>
		/// </summary>
		public bool timeLooped
		{
			get
			{
				return (this.m_Flags & FrameData.Flags.Loop) != (FrameData.Flags)0;
			}
		}

		/// <summary>
		///   <para>Indicates the local time did not advance because it has reached the duration and the extrapolation mode is set to Hold.</para>
		/// </summary>
		public bool timeHeld
		{
			get
			{
				return (this.m_Flags & FrameData.Flags.Hold) != (FrameData.Flags)0;
			}
		}

		internal ulong m_FrameID;

		internal double m_DeltaTime;

		internal float m_Weight;

		internal float m_EffectiveWeight;

		internal double m_EffectiveParentDelay;

		internal float m_EffectiveParentSpeed;

		internal float m_EffectiveSpeed;

		internal FrameData.Flags m_Flags;

		[Flags]
		internal enum Flags
		{
			Evaluate = 1,
			SeekOccured = 2,
			Loop = 4,
			Hold = 8
		}

		/// <summary>
		///   <para>Describes the cause for the evaluation of a PlayableGraph.</para>
		/// </summary>
		public enum EvaluationType
		{
			/// <summary>
			///   <para>Indicates the graph was updated due to a call to PlayableGraph.Evaluate.</para>
			/// </summary>
			Evaluate,
			/// <summary>
			///   <para>Indicates the graph was called by the runtime during normal playback due to PlayableGraph.Play being called.</para>
			/// </summary>
			Playback
		}
	}
}
