using System;
using STRINGS;

public class ProducePowerStates : GameStateMachine<ProducePowerStates, ProducePowerStates.Instance, IStateMachineTarget, ProducePowerStates.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.generator.moveToSleepLocation;
		this.root.ToggleStatusItem(CREATURES.STATUSITEMS.SLEEPING.NAME, CREATURES.STATUSITEMS.SLEEPING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, null, null, Db.Get().StatusItemCategories.Main);
		this.generator.moveToSleepLocation.MoveTo(delegate(ProducePowerStates.Instance smi)
		{
			ProducePowerMonitor.Instance smi2 = smi.GetSMI<ProducePowerMonitor.Instance>();
			return smi2.sm.targetSleepCell.Get(smi2);
		}, this.generator.sleep, this.behaviourcomplete, false);
		this.generator.sleep.Enter(delegate(ProducePowerStates.Instance smi)
		{
			if (smi.GetComponent<Staterpillar>().GetGenerator() == null)
			{
				smi.GoTo(this.behaviourcomplete);
				return;
			}
			smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Ceiling);
			smi.GetComponent<Staterpillar>().EnableGenerator();
			if (smi.GetComponent<Staterpillar>().IsConnected())
			{
				smi.GoTo(this.generator.sleep.connected);
				return;
			}
			smi.GoTo(this.generator.sleep.noConnection);
		}).Exit(delegate(ProducePowerStates.Instance smi)
		{
			ProducePowerMonitor.Instance smi3 = smi.GetSMI<ProducePowerMonitor.Instance>();
			if (smi3 != null)
			{
				smi3.sm.targetSleepCell.Set(Grid.InvalidCell, smi3);
			}
			smi.GetComponent<Staterpillar>().DestroyGenerator();
		});
		this.generator.sleep.connected.Enter(delegate(ProducePowerStates.Instance smi)
		{
			smi.Get<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.SolidConduitBridges);
		}).EventTransition(GameHashes.NewDay, (ProducePowerStates.Instance smi) => GameClock.Instance, this.generator.connectedWake, null).Transition(this.generator.sleep.noConnection, (ProducePowerStates.Instance smi) => smi.GetComponent<Staterpillar>().IsNotConnected(), UpdateRate.SIM_200ms)
			.PlayAnim("sleep_charging_pre")
			.QueueAnim("sleep_charging_loop", true, null)
			.Exit(delegate(ProducePowerStates.Instance smi)
			{
				smi.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Creatures);
			});
		this.generator.sleep.noConnection.PlayAnim("sleep_pre").QueueAnim("sleep_loop", true, null).ToggleStatusItem(Db.Get().BuildingStatusItems.NoWireConnected, null)
			.EventTransition(GameHashes.NewDay, (ProducePowerStates.Instance smi) => GameClock.Instance, this.generator.noConnectionWake, null)
			.Transition(this.generator.sleep.connected, (ProducePowerStates.Instance smi) => smi.GetComponent<Staterpillar>().IsConnected(), UpdateRate.SIM_200ms);
		this.generator.connectedWake.QueueAnim("sleep_charging_pst", false, null).OnAnimQueueComplete(this.behaviourcomplete);
		this.generator.noConnectionWake.QueueAnim("sleep_pst", false, null).OnAnimQueueComplete(this.behaviourcomplete);
		this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsToProducePower, false);
	}

	public static bool ShouldStop(ProducePowerStates.Instance smi)
	{
		return !GameClock.Instance.IsNighttime();
	}

	public ProducePowerStates.HasGeneratorStates generator;

	public GameStateMachine<ProducePowerStates, ProducePowerStates.Instance, IStateMachineTarget, ProducePowerStates.Def>.State behaviourcomplete;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<ProducePowerStates, ProducePowerStates.Instance, IStateMachineTarget, ProducePowerStates.Def>.GameInstance
	{
		public Instance(Chore<ProducePowerStates.Instance> chore, ProducePowerStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToProducePower);
		}
	}

	public class SleepStates : GameStateMachine<ProducePowerStates, ProducePowerStates.Instance, IStateMachineTarget, ProducePowerStates.Def>.State
	{
		public GameStateMachine<ProducePowerStates, ProducePowerStates.Instance, IStateMachineTarget, ProducePowerStates.Def>.State connected;

		public GameStateMachine<ProducePowerStates, ProducePowerStates.Instance, IStateMachineTarget, ProducePowerStates.Def>.State noConnection;
	}

	public class HasGeneratorStates : GameStateMachine<ProducePowerStates, ProducePowerStates.Instance, IStateMachineTarget, ProducePowerStates.Def>.State
	{
		public GameStateMachine<ProducePowerStates, ProducePowerStates.Instance, IStateMachineTarget, ProducePowerStates.Def>.State moveToSleepLocation;

		public ProducePowerStates.SleepStates sleep;

		public GameStateMachine<ProducePowerStates, ProducePowerStates.Instance, IStateMachineTarget, ProducePowerStates.Def>.State noConnectionWake;

		public GameStateMachine<ProducePowerStates, ProducePowerStates.Instance, IStateMachineTarget, ProducePowerStates.Def>.State connectedWake;
	}
}
