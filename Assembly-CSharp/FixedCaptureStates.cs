using System;
using STRINGS;

internal class FixedCaptureStates : GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.capture;
		this.root.Exit("AbandonedCapturePoint", delegate(FixedCaptureStates.Instance smi)
		{
			smi.AbandonedCapturePoint();
		});
		this.capture.EventTransition(GameHashes.CapturePointNoLongerAvailable, null, null).DefaultState(this.capture.cheer);
		GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State state = this.capture.cheer.DefaultState(this.capture.cheer.pre);
		string text = CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.NAME;
		string text2 = CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.TOOLTIP;
		StatusItemCategory statusItemCategory = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, statusItemCategory);
		this.capture.cheer.pre.ScheduleGoTo(0.9f, this.capture.cheer.cheer);
		this.capture.cheer.cheer.Enter("FaceRancher", delegate(FixedCaptureStates.Instance smi)
		{
			smi.GetComponent<Facing>().Face(smi.GetCapturePoint().transform.GetPosition());
		}).PlayAnim("excited_loop").OnAnimQueueComplete(this.capture.cheer.pst);
		this.capture.cheer.pst.ScheduleGoTo(0.2f, this.capture.move);
		GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State state2 = this.capture.move.DefaultState(this.capture.move.movetoranch);
		text2 = CREATURES.STATUSITEMS.GETTING_RANCHED.NAME;
		text = CREATURES.STATUSITEMS.GETTING_RANCHED.TOOLTIP;
		statusItemCategory = Db.Get().StatusItemCategories.Main;
		state2.ToggleStatusItem(text2, text, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, statusItemCategory);
		this.capture.move.movetoranch.Enter("Speedup", delegate(FixedCaptureStates.Instance smi)
		{
			smi.GetComponent<Navigator>().defaultSpeed = smi.originalSpeed * 1.25f;
		}).MoveTo(new Func<FixedCaptureStates.Instance, int>(FixedCaptureStates.GetTargetCaptureCell), this.capture.move.getontable, null, false).Exit("RestoreSpeed", delegate(FixedCaptureStates.Instance smi)
		{
			smi.GetComponent<Navigator>().defaultSpeed = smi.originalSpeed;
		});
		this.capture.move.getontable.Enter(new StateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State.Callback(FixedCaptureStates.PlayGroomingPreAnim)).Enter("FaceRight", delegate(FixedCaptureStates.Instance smi)
		{
			smi.GetComponent<Facing>().Face(smi.transform.GetPosition().x + 1f);
		}).OnAnimQueueComplete(this.capture.move.waitforranchertobeready);
		this.capture.move.waitforranchertobeready.Enter("SetCreatureAtRanchingStation", delegate(FixedCaptureStates.Instance smi)
		{
			smi.GetCapturePoint().Trigger(-1992722293, null);
		}).EventTransition(GameHashes.RancherReadyAtCapturePoint, this.capture.ranching, null);
		GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State state3 = this.capture.ranching.Enter(new StateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State.Callback(FixedCaptureStates.PlayGroomingLoopAnim)).EventTransition(GameHashes.FixedCaptureComplete, this.wavegoodbye, null);
		text = CREATURES.STATUSITEMS.GETTING_RANCHED.NAME;
		text2 = CREATURES.STATUSITEMS.GETTING_RANCHED.TOOLTIP;
		statusItemCategory = Db.Get().StatusItemCategories.Main;
		state3.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, statusItemCategory);
		GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State state4 = this.wavegoodbye.Enter(new StateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State.Callback(FixedCaptureStates.PlayGroomingPstAnim)).OnAnimQueueComplete(this.runaway);
		text2 = CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.NAME;
		text = CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.TOOLTIP;
		statusItemCategory = Db.Get().StatusItemCategories.Main;
		state4.ToggleStatusItem(text2, text, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, statusItemCategory);
		GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State state5 = this.runaway.MoveTo(new Func<FixedCaptureStates.Instance, int>(FixedCaptureStates.GetRunawayCell), this.behaviourcomplete, this.behaviourcomplete, false);
		text = CREATURES.STATUSITEMS.IDLE.NAME;
		text2 = CREATURES.STATUSITEMS.IDLE.TOOLTIP;
		statusItemCategory = Db.Get().StatusItemCategories.Main;
		state5.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, statusItemCategory);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToGetRanched, false);
	}

	private static FixedCapturePoint.Instance GetCapturePoint(FixedCaptureStates.Instance smi)
	{
		return smi.GetSMI<FixedCapturableMonitor.Instance>().targetCapturePoint;
	}

	private static void PlayGroomingPreAnim(FixedCaptureStates.Instance smi)
	{
		smi.Get<KBatchedAnimController>().Queue(FixedCaptureStates.GetCapturePoint(smi).def.ranchedPreAnim, KAnim.PlayMode.Once, 1f, 0f);
	}

	private static void PlayGroomingLoopAnim(FixedCaptureStates.Instance smi)
	{
		smi.Get<KBatchedAnimController>().Queue(FixedCaptureStates.GetCapturePoint(smi).def.ranchedLoopAnim, KAnim.PlayMode.Loop, 1f, 0f);
	}

	private static void PlayGroomingPstAnim(FixedCaptureStates.Instance smi)
	{
		smi.Get<KBatchedAnimController>().Queue(FixedCaptureStates.GetCapturePoint(smi).def.ranchedPstAnim, KAnim.PlayMode.Once, 1f, 0f);
	}

	private static int GetTargetCaptureCell(FixedCaptureStates.Instance smi)
	{
		FixedCapturePoint.Instance capturePoint = FixedCaptureStates.GetCapturePoint(smi);
		return capturePoint.def.getTargetCapturePoint(capturePoint);
	}

	private static int GetRunawayCell(FixedCaptureStates.Instance smi)
	{
		int num = Grid.PosToCell(smi.transform.GetPosition());
		int num2 = Grid.OffsetCell(num, 2, 0);
		if (Grid.Solid[num2])
		{
			num2 = Grid.OffsetCell(num, -2, 0);
		}
		return num2;
	}

	private FixedCaptureStates.CaptureStates capture;

	private GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State wavegoodbye;

	private GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State runaway;

	private GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.GameInstance
	{
		public Instance(Chore<FixedCaptureStates.Instance> chore, FixedCaptureStates.Def def)
			: base(chore, def)
		{
			this.originalSpeed = base.GetComponent<Navigator>().defaultSpeed;
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToGetCaptured);
		}

		public FixedCapturePoint.Instance GetCapturePoint()
		{
			FixedCapturableMonitor.Instance smi = this.GetSMI<FixedCapturableMonitor.Instance>();
			return (smi == null) ? null : smi.targetCapturePoint;
		}

		public void AbandonedCapturePoint()
		{
			if (this.GetCapturePoint() != null)
			{
				this.GetCapturePoint().Trigger(-1000356449, null);
			}
		}

		public float originalSpeed;
	}

	public class CaptureStates : GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State
	{
		public FixedCaptureStates.CaptureStates.CheerStates cheer;

		public FixedCaptureStates.CaptureStates.MoveStates move;

		public GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State ranching;

		public class CheerStates : GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State
		{
			public GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State pre;

			public GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State cheer;

			public GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State pst;
		}

		public class MoveStates : GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State
		{
			public GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State movetoranch;

			public GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State getontable;

			public GameStateMachine<FixedCaptureStates, FixedCaptureStates.Instance, IStateMachineTarget, FixedCaptureStates.Def>.State waitforranchertobeready;
		}
	}
}
