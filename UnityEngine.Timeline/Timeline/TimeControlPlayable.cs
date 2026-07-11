using System;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	public class TimeControlPlayable : PlayableBehaviour
	{
		public static ScriptPlayable<TimeControlPlayable> Create(PlayableGraph graph, ITimeControl timeControl)
		{
			ScriptPlayable<TimeControlPlayable> scriptPlayable;
			if (timeControl == null)
			{
				scriptPlayable = ScriptPlayable<TimeControlPlayable>.Null;
			}
			else
			{
				ScriptPlayable<TimeControlPlayable> scriptPlayable2 = ScriptPlayable<TimeControlPlayable>.Create(graph, 0);
				scriptPlayable2.GetBehaviour().Initialize(timeControl);
				scriptPlayable = scriptPlayable2;
			}
			return scriptPlayable;
		}

		public void Initialize(ITimeControl timeControl)
		{
			this.m_timeControl = timeControl;
		}

		public override void PrepareFrame(Playable playable, FrameData info)
		{
			if (this.m_timeControl != null)
			{
				this.m_timeControl.SetTime(playable.GetTime<Playable>());
			}
		}

		public override void OnBehaviourPlay(Playable playable, FrameData info)
		{
			if (this.m_timeControl != null)
			{
				if (!this.m_started)
				{
					this.m_timeControl.OnControlTimeStart();
					this.m_started = true;
				}
			}
		}

		public override void OnBehaviourPause(Playable playable, FrameData info)
		{
			if (this.m_timeControl != null)
			{
				if (this.m_started)
				{
					this.m_timeControl.OnControlTimeStop();
					this.m_started = false;
				}
			}
		}

		private ITimeControl m_timeControl;

		private bool m_started;
	}
}
