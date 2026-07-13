using System;
using STRINGS;
using UnityEngine;

public class BeeSleepStates : GameStateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.findSleepLocation;
		GameStateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.State root = this.root;
		string text = CREATURES.STATUSITEMS.SLEEPING.NAME;
		string text2 = CREATURES.STATUSITEMS.SLEEPING.TOOLTIP;
		string text3 = "";
		StatusItem.IconType iconType = StatusItem.IconType.Info;
		NotificationType notificationType = NotificationType.Neutral;
		bool flag = false;
		StatusItemCategory main = Db.Get().StatusItemCategories.Main;
		root.ToggleStatusItem(text, text2, text3, iconType, notificationType, flag, default(HashedString), 129022, null, null, main);
		this.findSleepLocation.Enter(delegate(BeeSleepStates.Instance smi)
		{
			BeeSleepStates.FindSleepLocation(smi);
			if (smi.targetSleepCell != Grid.InvalidCell)
			{
				smi.GoTo(this.moveToSleepLocation);
				return;
			}
			smi.GoTo(this.behaviourcomplete);
		});
		this.moveToSleepLocation.MoveTo((BeeSleepStates.Instance smi) => smi.targetSleepCell, this.sleep.pre, this.behaviourcomplete, false);
		this.sleep.Enter("EnableGravity", delegate(BeeSleepStates.Instance smi)
		{
			GameComps.Gravities.Add(smi.gameObject, Vector2.zero, delegate(Transform transform)
			{
				if (GameComps.Gravities.Has(smi.gameObject))
				{
					GameComps.Gravities.Remove(smi.gameObject);
				}
			});
		}).TriggerOnEnter(GameHashes.SleepStarted, null).TriggerOnExit(GameHashes.SleepFinished, null)
			.Transition(this.sleep.pst, new StateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.Transition.ConditionCallback(BeeSleepStates.ShouldWakeUp), UpdateRate.SIM_1000ms);
		this.sleep.pre.QueueAnim("sleep_pre", false, null).OnAnimQueueComplete(this.sleep.loop);
		this.sleep.loop.Enter(delegate(BeeSleepStates.Instance smi)
		{
			smi.GetComponent<LoopingSounds>().PauseSound(GlobalAssets.GetSound("Bee_wings_LP", false), true);
		}).QueueAnim("sleep_loop", true, null).Exit(delegate(BeeSleepStates.Instance smi)
		{
			smi.GetComponent<LoopingSounds>().PauseSound(GlobalAssets.GetSound("Bee_wings_LP", false), false);
		});
		this.sleep.pst.QueueAnim("sleep_pst", false, null).OnAnimQueueComplete(this.behaviourcomplete);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.BeeWantsToSleep, false);
	}

	private static void FindSleepLocation(BeeSleepStates.Instance smi)
	{
		smi.targetSleepCell = Grid.InvalidCell;
		FloorCellQuery floorCellQuery = PathFinderQueries.floorCellQuery.Reset(1, 0);
		smi.GetComponent<Navigator>().RunQuery(floorCellQuery);
		if (floorCellQuery.result_cells.Count > 0)
		{
			smi.targetSleepCell = floorCellQuery.result_cells[global::UnityEngine.Random.Range(0, floorCellQuery.result_cells.Count)];
		}
	}

	public static bool ShouldWakeUp(BeeSleepStates.Instance smi)
	{
		return smi.GetSMI<BeeSleepMonitor.Instance>().CO2Exposure <= 0f;
	}

	public BeeSleepStates.SleepStates sleep;

	public GameStateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.State findSleepLocation;

	public GameStateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.State moveToSleepLocation;

	public GameStateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.GameInstance
	{
		public Instance(Chore<BeeSleepStates.Instance> chore, BeeSleepStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.BeeWantsToSleep);
		}

		public int targetSleepCell;

		public float co2Exposure;
	}

	public class SleepStates : GameStateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.State
	{
		public GameStateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.State pre;

		public GameStateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.State loop;

		public GameStateMachine<BeeSleepStates, BeeSleepStates.Instance, IStateMachineTarget, BeeSleepStates.Def>.State pst;
	}
}
