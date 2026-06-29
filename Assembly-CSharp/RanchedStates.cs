using System;
using STRINGS;

internal class RanchedStates : GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.ranch;
		this.root.Exit("AbandonedRanchStation", delegate(RanchedStates.Instance smi)
		{
			smi.AbandonedRanchStation();
		});
		this.ranch.EventTransition(GameHashes.RanchStationNoLongerAvailable, null, null).DefaultState(this.ranch.cheer).Exit(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.ClearLayerOverride));
		GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State state = this.ranch.cheer.DefaultState(this.ranch.cheer.pre);
		string text = CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.NAME;
		string text2 = CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.TOOLTIP;
		StatusItemCategory statusItemCategory = Db.Get().StatusItemCategories.Main;
		state.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, statusItemCategory);
		this.ranch.cheer.pre.ScheduleGoTo(0.9f, this.ranch.cheer.cheer);
		this.ranch.cheer.cheer.Enter("FaceRancher", delegate(RanchedStates.Instance smi)
		{
			smi.GetComponent<Facing>().Face(smi.GetRanchStation().transform.GetPosition());
		}).PlayAnim("excited_loop").OnAnimQueueComplete(this.ranch.cheer.pst);
		this.ranch.cheer.pst.ScheduleGoTo(0.2f, this.ranch.move);
		GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State state2 = this.ranch.move.DefaultState(this.ranch.move.movetoranch);
		text2 = CREATURES.STATUSITEMS.GETTING_RANCHED.NAME;
		text = CREATURES.STATUSITEMS.GETTING_RANCHED.TOOLTIP;
		statusItemCategory = Db.Get().StatusItemCategories.Main;
		state2.ToggleStatusItem(text2, text, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, statusItemCategory);
		this.ranch.move.movetoranch.Enter("Speedup", delegate(RanchedStates.Instance smi)
		{
			smi.GetComponent<Navigator>().defaultSpeed = smi.originalSpeed * 1.25f;
		}).MoveTo(new Func<RanchedStates.Instance, int>(RanchedStates.GetTargetRanchCell), this.ranch.move.getontable, null, false).Exit("RestoreSpeed", delegate(RanchedStates.Instance smi)
		{
			smi.GetComponent<Navigator>().defaultSpeed = smi.originalSpeed;
		});
		this.ranch.move.getontable.Enter(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.PlayGroomingPreAnim)).Enter("FaceRight", delegate(RanchedStates.Instance smi)
		{
			smi.GetComponent<Facing>().Face(smi.transform.GetPosition().x + 1f);
		}).OnAnimQueueComplete(this.ranch.move.waitforranchertobeready);
		this.ranch.move.waitforranchertobeready.Enter("SetCreatureAtRanchingStation", delegate(RanchedStates.Instance smi)
		{
			smi.GetRanchStation().Trigger(-1357116271, null);
		}).EventTransition(GameHashes.RancherReadyAtRanchStation, this.ranch.ranching, null);
		GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State state3 = this.ranch.ranching.Enter(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.PlayGroomingLoopAnim)).EventTransition(GameHashes.RanchingComplete, this.wavegoodbye, null);
		text = CREATURES.STATUSITEMS.GETTING_RANCHED.NAME;
		text2 = CREATURES.STATUSITEMS.GETTING_RANCHED.TOOLTIP;
		statusItemCategory = Db.Get().StatusItemCategories.Main;
		state3.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, statusItemCategory);
		GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State state4 = this.wavegoodbye.Enter(new StateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State.Callback(RanchedStates.PlayGroomingPstAnim)).OnAnimQueueComplete(this.runaway);
		text2 = CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.NAME;
		text = CREATURES.STATUSITEMS.EXCITED_TO_BE_RANCHED.TOOLTIP;
		statusItemCategory = Db.Get().StatusItemCategories.Main;
		state4.ToggleStatusItem(text2, text, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, statusItemCategory);
		GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State state5 = this.runaway.MoveTo(new Func<RanchedStates.Instance, int>(RanchedStates.GetRunawayCell), this.behaviourcomplete, this.behaviourcomplete, false);
		text = CREATURES.STATUSITEMS.IDLE.NAME;
		text2 = CREATURES.STATUSITEMS.IDLE.TOOLTIP;
		statusItemCategory = Db.Get().StatusItemCategories.Main;
		state5.ToggleStatusItem(text, text2, string.Empty, StatusItem.IconType.Info, (NotificationType)0, false, SimViewMode.None, 0, null, null, statusItemCategory);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToGetRanched, false);
	}

	private static RanchStation.Instance GetRanchStation(RanchedStates.Instance smi)
	{
		return smi.GetSMI<RanchableMonitor.Instance>().targetRanchStation;
	}

	private static void ClearLayerOverride(RanchedStates.Instance smi)
	{
		smi.Get<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures);
	}

	private static void PlayGroomingPreAnim(RanchedStates.Instance smi)
	{
		smi.Get<KBatchedAnimController>().Queue(RanchedStates.GetRanchStation(smi).def.ranchedPreAnim, KAnim.PlayMode.Once, 1f, 0f);
	}

	private static void PlayGroomingLoopAnim(RanchedStates.Instance smi)
	{
		smi.Get<KBatchedAnimController>().Queue(RanchedStates.GetRanchStation(smi).def.ranchedLoopAnim, KAnim.PlayMode.Loop, 1f, 0f);
	}

	private static void PlayGroomingPstAnim(RanchedStates.Instance smi)
	{
		smi.Get<KBatchedAnimController>().Queue(RanchedStates.GetRanchStation(smi).def.ranchedPstAnim, KAnim.PlayMode.Once, 1f, 0f);
	}

	private static int GetTargetRanchCell(RanchedStates.Instance smi)
	{
		RanchStation.Instance ranchStation = RanchedStates.GetRanchStation(smi);
		return ranchStation.def.getTargetRanchCell(ranchStation);
	}

	private static int GetRunawayCell(RanchedStates.Instance smi)
	{
		int num = Grid.PosToCell(smi.transform.GetPosition());
		int num2 = Grid.OffsetCell(num, 2, 0);
		if (Grid.Solid[num2])
		{
			num2 = Grid.OffsetCell(num, -2, 0);
		}
		return num2;
	}

	private RanchedStates.RanchStates ranch;

	private GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State wavegoodbye;

	private GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State runaway;

	private GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.GameInstance
	{
		public Instance(Chore<RanchedStates.Instance> chore, RanchedStates.Def def)
			: base(chore, def)
		{
			this.originalSpeed = base.GetComponent<Navigator>().defaultSpeed;
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToGetRanched);
		}

		public RanchStation.Instance GetRanchStation()
		{
			return this.GetSMI<RanchableMonitor.Instance>().targetRanchStation;
		}

		public void AbandonedRanchStation()
		{
			if (this.GetRanchStation() != null)
			{
				this.GetRanchStation().Trigger(-364750427, null);
			}
		}

		public float originalSpeed;
	}

	public class RanchStates : GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State
	{
		public RanchedStates.RanchStates.CheerStates cheer;

		public RanchedStates.RanchStates.MoveStates move;

		public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State ranching;

		public class CheerStates : GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State
		{
			public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State pre;

			public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State cheer;

			public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State pst;
		}

		public class MoveStates : GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State
		{
			public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State movetoranch;

			public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State getontable;

			public GameStateMachine<RanchedStates, RanchedStates.Instance, IStateMachineTarget, RanchedStates.Def>.State waitforranchertobeready;
		}
	}
}
