using System;
using STRINGS;
using TUNING;
using UnityEngine;

public class BalloonArtistChore : Chore<BalloonArtistChore.StatesInstance>, IWorkerPrioritizable
{
	public BalloonArtistChore(IStateMachineTarget target)
	{
		Chore.Precondition precondition = default(Chore.Precondition);
		precondition.id = "HasBalloonStallCell";
		precondition.description = DUPLICANTS.CHORES.PRECONDITIONS.HAS_BALLOON_STALL_CELL;
		precondition.fn = delegate(ref Chore.Precondition.Context context, object data)
		{
			return ((BalloonArtistChore)data).smi.HasBalloonStallCell();
		};
		this.HasBalloonStallCell = precondition;
		base..ctor(Db.Get().ChoreTypes.JoyReaction, target, target.GetComponent<ChoreProvider>(), false, null, null, null, PriorityScreen.PriorityClass.high, 5, false, true, 0, false, ReportManager.ReportType.PersonalTime);
		this.showAvailabilityInHoverText = false;
		base.smi = new BalloonArtistChore.StatesInstance(this, target.gameObject);
		base.AddPrecondition(this.HasBalloonStallCell, this);
		base.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert, null);
		base.AddPrecondition(ChorePreconditions.instance.IsScheduledTime, Db.Get().ScheduleBlockTypes.Recreation);
		base.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, this);
	}

	public bool GetWorkerPriority(Worker worker, out int priority)
	{
		priority = this.basePriority;
		return true;
	}

	private int basePriority = RELAXATION.PRIORITY.TIER1;

	private Chore.Precondition HasBalloonStallCell;

	public class States : GameStateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.goToStand;
			base.Target(this.balloonArtist);
			this.root.EventTransition(GameHashes.ScheduleBlocksChanged, this.idle, (BalloonArtistChore.StatesInstance smi) => !smi.IsRecTime());
			this.idle.DoNothing();
			this.goToStand.Transition(null, (BalloonArtistChore.StatesInstance smi) => !smi.HasBalloonStallCell(), UpdateRate.SIM_200ms).MoveTo((BalloonArtistChore.StatesInstance smi) => smi.GetBalloonStallCell(), this.balloonStand, null, false);
			this.balloonStand.ToggleAnims("anim_interacts_balloon_artist_kanim", 0f, "").Enter(delegate(BalloonArtistChore.StatesInstance smi)
			{
				smi.SpawnBalloonStand();
			}).Exit(delegate(BalloonArtistChore.StatesInstance smi)
			{
				smi.DestroyBalloonStand();
			})
				.DefaultState(this.balloonStand.idle);
			this.balloonStand.idle.PlayAnim("working_pre").QueueAnim("working_loop", true, null).OnSignal(this.giveBalloonOut, this.balloonStand.giveBalloon);
			this.balloonStand.giveBalloon.PlayAnim("working_pst").OnAnimQueueComplete(this.balloonStand.idle);
		}

		public StateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.TargetParameter balloonArtist;

		public StateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.IntParameter balloonsGivenOut = new StateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.IntParameter(0);

		public StateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.Signal giveBalloonOut;

		public GameStateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.State idle;

		public GameStateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.State goToStand;

		public BalloonArtistChore.States.BalloonStandStates balloonStand;

		public class BalloonStandStates : GameStateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.State
		{
			public GameStateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.State idle;

			public GameStateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.State giveBalloon;
		}
	}

	public class StatesInstance : GameStateMachine<BalloonArtistChore.States, BalloonArtistChore.StatesInstance, BalloonArtistChore, object>.GameInstance
	{
		public StatesInstance(BalloonArtistChore master, GameObject balloonArtist)
			: base(master)
		{
			this.balloonArtist = balloonArtist;
			base.sm.balloonArtist.Set(balloonArtist, base.smi, false);
		}

		public bool IsRecTime()
		{
			return base.master.GetComponent<Schedulable>().IsAllowed(Db.Get().ScheduleBlockTypes.Recreation);
		}

		public int GetBalloonStallCell()
		{
			return this.balloonArtistCellSensor.GetCell();
		}

		public int GetBalloonStallTargetCell()
		{
			return this.balloonArtistCellSensor.GetStandCell();
		}

		public bool HasBalloonStallCell()
		{
			if (this.balloonArtistCellSensor == null)
			{
				this.balloonArtistCellSensor = base.GetComponent<Sensors>().GetSensor<BalloonStandCellSensor>();
			}
			return this.balloonArtistCellSensor.GetCell() != Grid.InvalidCell;
		}

		public bool IsSameRoom()
		{
			int num = Grid.PosToCell(this.balloonArtist);
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
			CavityInfo cavityForCell2 = Game.Instance.roomProber.GetCavityForCell(this.GetBalloonStallCell());
			return cavityForCell != null && cavityForCell2 != null && cavityForCell.handle == cavityForCell2.handle;
		}

		public void SpawnBalloonStand()
		{
			Vector3 vector = Grid.CellToPos(this.GetBalloonStallTargetCell());
			this.balloonArtist.GetComponent<Facing>().Face(vector);
			this.balloonStand = Util.KInstantiate(Assets.GetPrefab("BalloonStand"), vector, Quaternion.identity, null, null, true, 0);
			this.balloonStand.SetActive(true);
			this.balloonStand.GetComponent<GetBalloonWorkable>().SetBalloonArtist(base.smi);
		}

		public void DestroyBalloonStand()
		{
			this.balloonStand.DeleteObject();
		}

		public void GiveBalloon()
		{
			this.balloonArtist.GetSMI<BalloonArtist.Instance>().GiveBalloon();
			base.smi.sm.giveBalloonOut.Trigger(base.smi);
		}

		private BalloonStandCellSensor balloonArtistCellSensor;

		private GameObject balloonArtist;

		private GameObject balloonStand;
	}
}
