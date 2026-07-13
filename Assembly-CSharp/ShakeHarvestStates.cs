using System;
using STRINGS;
using UnityEngine;

public class ShakeHarvestStates : GameStateMachine<ShakeHarvestStates, ShakeHarvestStates.Instance, IStateMachineTarget, ShakeHarvestStates.Def>
{
	private static StatusItem GoingToHarvestStatus(ShakeHarvestStates.Instance smi)
	{
		return ShakeHarvestStates.MakeStatus(smi, CREATURES.STATUSITEMS.GOING_TO_HARVEST.NAME, CREATURES.STATUSITEMS.GOING_TO_HARVEST.TOOLTIP);
	}

	private static StatusItem HarvestingStatus(ShakeHarvestStates.Instance smi)
	{
		return ShakeHarvestStates.MakeStatus(smi, CREATURES.STATUSITEMS.HARVESTING.NAME, CREATURES.STATUSITEMS.HARVESTING.TOOLTIP);
	}

	private static StatusItem MakeStatus(ShakeHarvestStates.Instance smi, string name, string tooltip)
	{
		return new StatusItem(smi.GetCurrentState().longName, name, tooltip, "", StatusItem.IconType.Info, NotificationType.Neutral, false, default(HashedString), 129022, true, null);
	}

	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.Never;
		default_state = this.approach;
		this.root.Enter(delegate(ShakeHarvestStates.Instance smi)
		{
			ShakeHarvestMonitor.Instance smi2 = smi.GetSMI<ShakeHarvestMonitor.Instance>();
			this.plant.Set(smi2.sm.plant.Get(smi2), smi, false);
		});
		this.approach.InitializeStates(this.harvester, this.plant, delegate(ShakeHarvestStates.Instance smi)
		{
			ListPool<CellOffset, ShakeHarvestStates>.PooledList pooledList = ListPool<CellOffset, ShakeHarvestStates>.Allocate();
			ShakeHarvestMonitor.Def.GetApproachOffsets(this.plant.Get(smi), pooledList);
			CellOffset[] array = pooledList.ToArray();
			pooledList.Recycle();
			return array;
		}, this.harvest, this.failed, null).ToggleMainStatusItem(new Func<ShakeHarvestStates.Instance, StatusItem>(ShakeHarvestStates.GoingToHarvestStatus), null).OnTargetLost(this.plant, this.failed)
			.Target(this.plant)
			.EventTransition(GameHashes.Harvest, this.failed, null)
			.EventTransition(GameHashes.Uprooted, this.failed, null)
			.EventTransition(GameHashes.QueueDestroyObject, this.failed, null);
		this.harvest.PlayAnim("shake", KAnim.PlayMode.Once).ToggleMainStatusItem(new Func<ShakeHarvestStates.Instance, StatusItem>(ShakeHarvestStates.HarvestingStatus), null).OnAnimQueueComplete(this.complete)
			.OnTargetLost(this.plant, this.failed);
		this.complete.Enter(delegate(ShakeHarvestStates.Instance smi)
		{
			GameObject gameObject = this.plant.Get(smi);
			if (gameObject.IsNullOrDestroyed())
			{
				return;
			}
			Harvestable component = gameObject.GetComponent<Harvestable>();
			if (component != null && component.CanBeHarvested)
			{
				component.Trigger(2127324410, BoxedBools.True);
				component.Harvest();
			}
		}).BehaviourComplete(GameTags.Creatures.WantsToHarvest, false);
		this.failed.Enter(delegate(ShakeHarvestStates.Instance smi)
		{
			ShakeHarvestMonitor.Instance smi3 = smi.GetSMI<ShakeHarvestMonitor.Instance>();
			if (smi3 != null)
			{
				smi3.sm.failed.Trigger(smi3);
			}
		}).EnterGoTo(null);
	}

	private readonly GameStateMachine<ShakeHarvestStates, ShakeHarvestStates.Instance, IStateMachineTarget, ShakeHarvestStates.Def>.ApproachSubState<IApproachable> approach;

	private readonly GameStateMachine<ShakeHarvestStates, ShakeHarvestStates.Instance, IStateMachineTarget, ShakeHarvestStates.Def>.State harvest;

	private readonly GameStateMachine<ShakeHarvestStates, ShakeHarvestStates.Instance, IStateMachineTarget, ShakeHarvestStates.Def>.State complete;

	private readonly GameStateMachine<ShakeHarvestStates, ShakeHarvestStates.Instance, IStateMachineTarget, ShakeHarvestStates.Def>.State failed;

	private readonly StateMachine<ShakeHarvestStates, ShakeHarvestStates.Instance, IStateMachineTarget, ShakeHarvestStates.Def>.TargetParameter harvester;

	private readonly StateMachine<ShakeHarvestStates, ShakeHarvestStates.Instance, IStateMachineTarget, ShakeHarvestStates.Def>.TargetParameter plant;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<ShakeHarvestStates, ShakeHarvestStates.Instance, IStateMachineTarget, ShakeHarvestStates.Def>.GameInstance
	{
		public Instance(Chore<ShakeHarvestStates.Instance> chore, ShakeHarvestStates.Def def)
			: base(chore, def)
		{
			chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, GameTags.Creatures.WantsToHarvest);
			base.sm.harvester.Set(base.gameObject, this, false);
		}
	}
}
