using System;
using System.Runtime.Serialization;
using KSerialization;
using TUNING;

public class BalloonArtist : GameStateMachine<BalloonArtist, BalloonArtist.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.neutral;
		this.root.EventTransition(GameHashes.Died, null, null);
		this.neutral.TagTransition(GameTags.Overjoyed, this.overjoyed, false);
		this.overjoyed.TagTransition(GameTags.Overjoyed, this.neutral, true).DefaultState(this.overjoyed.idle).ParamTransition<int>(this.balloonsGivenOut, this.overjoyed.exitEarly, (BalloonArtist.Instance smi, int p) => p >= TRAITS.JOY_REACTIONS.BALLOON_ARTIST.NUM_BALLOONS_TO_GIVE)
			.Exit(delegate(BalloonArtist.Instance smi)
			{
				smi.numBalloonsGiven = 0;
				this.balloonsGivenOut.Set(0, smi);
			});
		this.overjoyed.idle.Enter(delegate(BalloonArtist.Instance smi)
		{
			if (smi.IsRecTime())
			{
				smi.GoTo(this.overjoyed.balloon_stand);
			}
		}).EventTransition(GameHashes.ScheduleBlocksChanged, this.overjoyed.balloon_stand, (BalloonArtist.Instance smi) => smi.IsRecTime());
		this.overjoyed.balloon_stand.EventTransition(GameHashes.ScheduleBlocksChanged, this.overjoyed.idle, (BalloonArtist.Instance smi) => !smi.IsRecTime()).ToggleChore((BalloonArtist.Instance smi) => new BalloonArtistChore(smi.master), null);
		this.overjoyed.exitEarly.Enter(delegate(BalloonArtist.Instance smi)
		{
			smi.ExitJoyReactionEarly();
		});
	}

	public StateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.IntParameter balloonsGivenOut;

	public GameStateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.State neutral;

	public BalloonArtist.OverjoyedStates overjoyed;

	public class OverjoyedStates : GameStateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.State idle;

		public GameStateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.State balloon_stand;

		public GameStateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.State exitEarly;
	}

	public new class Instance : GameStateMachine<BalloonArtist, BalloonArtist.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
		}

		[OnDeserialized]
		private void OnDeserialized()
		{
			base.smi.sm.balloonsGivenOut.Set(this.numBalloonsGiven, base.smi);
		}

		public bool IsRecTime()
		{
			Schedulable component = base.master.GetComponent<Schedulable>();
			return component.IsAllowed(Db.Get().ScheduleBlockTypes.Recreation);
		}

		public void GiveBalloon()
		{
			this.numBalloonsGiven++;
			base.smi.sm.balloonsGivenOut.Set(this.numBalloonsGiven, base.smi);
		}

		public void ExitJoyReactionEarly()
		{
			JoyBehaviourMonitor.Instance smi = base.master.gameObject.GetSMI<JoyBehaviourMonitor.Instance>();
			smi.sm.exitEarly.Trigger(smi);
		}

		[Serialize]
		public int numBalloonsGiven;
	}
}
