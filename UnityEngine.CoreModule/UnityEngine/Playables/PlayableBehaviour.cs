using System;
using UnityEngine.Scripting;

namespace UnityEngine.Playables
{
	/// <summary>
	///   <para>PlayableBehaviour is the base class from which every custom playable script derives.</para>
	/// </summary>
	[RequiredByNativeCode]
	[Serializable]
	public abstract class PlayableBehaviour : IPlayableBehaviour, ICloneable
	{
		public PlayableBehaviour()
		{
		}

		/// <summary>
		///   <para>This function is called when the PlayableGraph that owns this PlayableBehaviour starts.</para>
		/// </summary>
		/// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
		public virtual void OnGraphStart(Playable playable)
		{
		}

		/// <summary>
		///   <para>This function is called when the PlayableGraph that owns this PlayableBehaviour stops.</para>
		/// </summary>
		/// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
		public virtual void OnGraphStop(Playable playable)
		{
		}

		/// <summary>
		///   <para>This function is called when the Playable that owns the PlayableBehaviour is created.</para>
		/// </summary>
		/// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
		public virtual void OnPlayableCreate(Playable playable)
		{
		}

		/// <summary>
		///   <para>This function is called when the Playable that owns the PlayableBehaviour is destroyed.</para>
		/// </summary>
		/// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
		public virtual void OnPlayableDestroy(Playable playable)
		{
		}

		/// <summary>
		///   <para>This function is called when the Playable play state is changed to Playables.PlayState.Delayed.</para>
		/// </summary>
		/// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
		/// <param name="info">A FrameData structure that contains information about the current frame context.</param>
		public virtual void OnBehaviourDelay(Playable playable, FrameData info)
		{
		}

		/// <summary>
		///   <para>This function is called when the Playable play state is changed to Playables.PlayState.Playing.</para>
		/// </summary>
		/// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
		/// <param name="info">A FrameData structure that contains information about the current frame context.</param>
		public virtual void OnBehaviourPlay(Playable playable, FrameData info)
		{
		}

		/// <summary>
		///   <para>This function is called when the Playable play state is changed to Playables.PlayState.Paused.</para>
		/// </summary>
		/// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
		/// <param name="info">A FrameData structure that contains information about the current frame context.</param>
		public virtual void OnBehaviourPause(Playable playable, FrameData info)
		{
		}

		/// <summary>
		///   <para>This function is called during the PrepareData phase of the PlayableGraph.</para>
		/// </summary>
		/// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
		/// <param name="info">A FrameData structure that contains information about the current frame context.</param>
		public virtual void PrepareData(Playable playable, FrameData info)
		{
		}

		/// <summary>
		///   <para>This function is called during the PrepareFrame phase of the PlayableGraph.</para>
		/// </summary>
		/// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
		/// <param name="info">A FrameData structure that contains information about the current frame context.</param>
		public virtual void PrepareFrame(Playable playable, FrameData info)
		{
		}

		/// <summary>
		///   <para>This function is called during the ProcessFrame phase of the PlayableGraph.</para>
		/// </summary>
		/// <param name="playable">The Playable that owns the current PlayableBehaviour.</param>
		/// <param name="info">A FrameData structure that contains information about the current frame context.</param>
		/// <param name="playerData"></param>
		public virtual void ProcessFrame(Playable playable, FrameData info, object playerData)
		{
		}

		public virtual object Clone()
		{
			return base.MemberwiseClone();
		}
	}
}
