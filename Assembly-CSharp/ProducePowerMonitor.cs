using System;
using UnityEngine;

public class ProducePowerMonitor : GameStateMachine<ProducePowerMonitor, ProducePowerMonitor.Instance, IStateMachineTarget, ProducePowerMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.idle;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.idle.Enter(delegate(ProducePowerMonitor.Instance smi)
		{
			this.targetSleepCell.Set(Grid.InvalidCell, smi);
			smi.GetComponent<Staterpillar>().DestroyGenerator();
		}).EventTransition(GameHashes.Nighttime, (ProducePowerMonitor.Instance smi) => GameClock.Instance, this.searching.looking, (ProducePowerMonitor.Instance smi) => GameClock.Instance.IsNighttime());
		this.searching.Enter(new StateMachine<ProducePowerMonitor, ProducePowerMonitor.Instance, IStateMachineTarget, ProducePowerMonitor.Def>.State.Callback(this.TryRecoverSave)).EventTransition(GameHashes.NewDay, (ProducePowerMonitor.Instance smi) => GameClock.Instance, this.idle, (ProducePowerMonitor.Instance smi) => !GameClock.Instance.IsNighttime()).Exit(delegate(ProducePowerMonitor.Instance smi)
		{
			this.targetSleepCell.Set(Grid.InvalidCell, smi);
			smi.GetComponent<Staterpillar>().DestroyGenerator();
		});
		this.searching.looking.Update(delegate(ProducePowerMonitor.Instance smi, float dt)
		{
			this.FindSleepLocation(smi);
		}, UpdateRate.SIM_1000ms, false).ToggleStatusItem(Db.Get().CreatureStatusItems.NoSleepSpot, null).ParamTransition<int>(this.targetSleepCell, this.searching.found, (ProducePowerMonitor.Instance smi, int sleepCell) => sleepCell != Grid.InvalidCell);
		this.searching.found.Enter(delegate(ProducePowerMonitor.Instance smi)
		{
			smi.GetComponent<Staterpillar>().SpawnGenerator(this.targetSleepCell.Get(smi));
		}).ParamTransition<int>(this.targetSleepCell, this.searching.looking, (ProducePowerMonitor.Instance smi, int sleepCell) => sleepCell == Grid.InvalidCell).ToggleBehaviour(GameTags.Creatures.WantsToProducePower, (ProducePowerMonitor.Instance smi) => this.targetSleepCell.Get(smi) != Grid.InvalidCell, null);
	}

	private void TryRecoverSave(ProducePowerMonitor.Instance smi)
	{
		Staterpillar component = smi.GetComponent<Staterpillar>();
		if (this.targetSleepCell.Get(smi) == Grid.InvalidCell && component.GetGenerator() != null)
		{
			int num = Grid.PosToCell(component.GetGenerator());
			this.targetSleepCell.Set(num, smi);
		}
	}

	private void FindSleepLocation(ProducePowerMonitor.Instance smi)
	{
		StaterpillarCellQuery staterpillarCellQuery = PathFinderQueries.staterpillarCellQuery.Reset(10, smi.gameObject);
		smi.GetComponent<Navigator>().RunQuery(staterpillarCellQuery);
		if (staterpillarCellQuery.result_cells.Count > 0)
		{
			foreach (int num in staterpillarCellQuery.result_cells)
			{
				int cellInDirection = Grid.GetCellInDirection(num, Direction.Down);
				if (Grid.Objects[cellInDirection, 26] != null)
				{
					this.targetSleepCell.Set(num, smi);
					break;
				}
			}
			if (this.targetSleepCell.Get(smi) == Grid.InvalidCell)
			{
				this.targetSleepCell.Set(staterpillarCellQuery.result_cells[global::UnityEngine.Random.Range(0, staterpillarCellQuery.result_cells.Count)], smi);
			}
		}
	}

	private GameStateMachine<ProducePowerMonitor, ProducePowerMonitor.Instance, IStateMachineTarget, ProducePowerMonitor.Def>.State idle;

	private ProducePowerMonitor.SleepSearchStates searching;

	public StateMachine<ProducePowerMonitor, ProducePowerMonitor.Instance, IStateMachineTarget, ProducePowerMonitor.Def>.IntParameter targetSleepCell = new StateMachine<ProducePowerMonitor, ProducePowerMonitor.Instance, IStateMachineTarget, ProducePowerMonitor.Def>.IntParameter(Grid.InvalidCell);

	public class Def : StateMachine.BaseDef
	{
	}

	private class SleepSearchStates : GameStateMachine<ProducePowerMonitor, ProducePowerMonitor.Instance, IStateMachineTarget, ProducePowerMonitor.Def>.State
	{
		public GameStateMachine<ProducePowerMonitor, ProducePowerMonitor.Instance, IStateMachineTarget, ProducePowerMonitor.Def>.State looking;

		public GameStateMachine<ProducePowerMonitor, ProducePowerMonitor.Instance, IStateMachineTarget, ProducePowerMonitor.Def>.State found;
	}

	public new class Instance : GameStateMachine<ProducePowerMonitor, ProducePowerMonitor.Instance, IStateMachineTarget, ProducePowerMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, ProducePowerMonitor.Def def)
			: base(master, def)
		{
		}
	}
}
