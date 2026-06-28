using System;
using Klei.AI;
using UnityEngine;

public class RanchStation : GameStateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.operational;
		this.unoperational.TagTransition(GameTags.Operational, this.operational, false);
		this.operational.TagTransition(GameTags.Operational, this.unoperational, true).ToggleChore((RanchStation.Instance smi) => smi.CreateChore(), this.unoperational, this.unoperational).Update("FindRanachable", delegate(RanchStation.Instance smi, float dt)
		{
			smi.FindRanchable();
		}, UpdateRate.SIM_1000ms, false);
	}

	public GameStateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>.State unoperational;

	public RanchStation.OperationalState operational;

	public class Def : StateMachine.BaseDef
	{
		public Effect effect;
	}

	public class OperationalState : GameStateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>.State
	{
	}

	public new class Instance : GameStateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, RanchStation.Def def)
			: base(master, def)
		{
		}

		public RanchableMonitor.Instance targetRanchable { get; private set; }

		public bool shouldCreatureGoGetRanched { get; private set; }

		public Chore CreateChore()
		{
			return new RancherChore(base.GetComponent<KPrefabID>());
		}

		public bool IsCreatureAvailableForRanching()
		{
			if (this.targetRanchable != null)
			{
				int num = Grid.PosToCell(base.transform.GetPosition());
				CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
				return RanchStation.Instance.CanRanchableBeRanchedAtRanchStation(this.targetRanchable, this, cavityForCell, num);
			}
			return false;
		}

		public void SetRancherIsAvailableForRanching()
		{
			this.shouldCreatureGoGetRanched = true;
		}

		public void ClearRancherIsAvailableForRanching()
		{
			this.shouldCreatureGoGetRanched = false;
		}

		private static bool CanRanchableBeRanchedAtRanchStation(RanchableMonitor.Instance ranchable, RanchStation.Instance ranch_station, CavityInfo ranch_cavity_info, int ranch_cell)
		{
			if (!ranchable.IsRunning())
			{
				return false;
			}
			if (ranchable.targetRanchStation != ranch_station && ranchable.targetRanchStation != null && ranchable.targetRanchStation.IsRunning())
			{
				return false;
			}
			if (ranchable.GetComponent<Effects>().HasEffect(ranch_station.def.effect))
			{
				return false;
			}
			if (!ranchable.GetComponent<ChoreConsumer>().IsChoreEqualOrAboveCurrentChorePriority<RanchedStates>())
			{
				return false;
			}
			int num = Grid.PosToCell(ranchable.transform.GetPosition());
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
			if (cavityForCell == null || cavityForCell != ranch_cavity_info)
			{
				return false;
			}
			int navigationCost = ranchable.GetComponent<Navigator>().GetNavigationCost(ranch_cell);
			return navigationCost != PathProber.InvalidCost;
		}

		public void FindRanchable()
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
			if (cavityForCell == null)
			{
				this.TriggerRanchStationNoLongerAvailable();
				return;
			}
			if (this.targetRanchable != null && !RanchStation.Instance.CanRanchableBeRanchedAtRanchStation(this.targetRanchable, this, cavityForCell, num))
			{
				this.TriggerRanchStationNoLongerAvailable();
			}
			if (this.targetRanchable == null)
			{
				RanchStation.Instance.RanchableIterator ranchableIterator = new RanchStation.Instance.RanchableIterator(this, cavityForCell, num);
				GameScenePartitioner.Instance.Iterate<RanchStation.Instance.RanchableIterator>(cavityForCell.minX, cavityForCell.minY, cavityForCell.maxX - cavityForCell.minX + 1, cavityForCell.maxY - cavityForCell.minY + 1, GameScenePartitioner.Instance.collisionLayer, ref ranchableIterator);
				ranchableIterator.Cleanup();
				this.targetRanchable = ranchableIterator.result;
				if (this.targetRanchable != null)
				{
					this.targetRanchable.targetRanchStation = this;
				}
			}
		}

		public void TriggerRanchStationNoLongerAvailable()
		{
			if (this.targetRanchable != null && this.targetRanchable.IsRunning())
			{
				this.targetRanchable.targetRanchStation = null;
				this.targetRanchable.Trigger(1689625967, null);
				this.targetRanchable = null;
			}
		}

		public void RanchCreature()
		{
			if (this.targetRanchable != null && this.targetRanchable.IsRunning())
			{
				this.targetRanchable.gameObject.GetComponent<Effects>().Add(base.def.effect, true);
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, base.def.effect.Name, this.targetRanchable.transform, new Vector3(0f, 0.5f, 0f), 1.5f, false, false);
				this.targetRanchable.Trigger(1827504087, null);
			}
		}

		private struct RanchableIterator : GameScenePartitioner.Iterator
		{
			public RanchableIterator(RanchStation.Instance ranch_station, CavityInfo ranch_cavity_info, int ranch_cell)
			{
				this.ranchStation = ranch_station;
				this.ranchCavityInfo = ranch_cavity_info;
				this.ranchCell = ranch_cell;
				this.result = null;
			}

			public RanchableMonitor.Instance result { get; private set; }

			public void Iterate(object target_obj)
			{
				KMonoBehaviour kmonoBehaviour = target_obj as KMonoBehaviour;
				if (kmonoBehaviour == null)
				{
					return;
				}
				RanchableMonitor.Instance smi = kmonoBehaviour.GetSMI<RanchableMonitor.Instance>();
				if (smi == null)
				{
					return;
				}
				if (RanchStation.Instance.CanRanchableBeRanchedAtRanchStation(smi, this.ranchStation, this.ranchCavityInfo, this.ranchCell))
				{
					this.result = smi;
				}
			}

			public void Cleanup()
			{
			}

			private CavityInfo ranchCavityInfo;

			private int ranchCell;

			private RanchStation.Instance ranchStation;
		}
	}
}
