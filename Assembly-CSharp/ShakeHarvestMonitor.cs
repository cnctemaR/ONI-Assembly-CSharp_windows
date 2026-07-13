using System;
using System.Collections.Generic;
using UnityEngine;

public class ShakeHarvestMonitor : GameStateMachine<ShakeHarvestMonitor, ShakeHarvestMonitor.Instance, IStateMachineTarget, ShakeHarvestMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.cooldown;
		this.cooldown.Update(delegate(ShakeHarvestMonitor.Instance smi, float dt)
		{
			this.elapsedTime.Set(this.elapsedTime.Get(smi) + dt, smi, false);
		}, UpdateRate.SIM_200ms, false).ParamTransition<float>(this.elapsedTime, this.harvest, (ShakeHarvestMonitor.Instance smi, float elapsedTime) => elapsedTime > smi.def.cooldownDuration);
		this.harvest.DefaultState(this.harvest.seek).ParamTransition<float>(this.elapsedTime, this.cooldown, GameStateMachine<ShakeHarvestMonitor, ShakeHarvestMonitor.Instance, IStateMachineTarget, ShakeHarvestMonitor.Def>.IsLTEZero);
		this.harvest.seek.PreBrainUpdate(delegate(ShakeHarvestMonitor.Instance smi)
		{
			this.plant.Set(smi.Seek(), smi);
		}).ParamTransition<GameObject>(this.plant, this.harvest.execute, GameStateMachine<ShakeHarvestMonitor, ShakeHarvestMonitor.Instance, IStateMachineTarget, ShakeHarvestMonitor.Def>.IsNotNull);
		this.harvest.execute.Enter(delegate(ShakeHarvestMonitor.Instance smi)
		{
			this.plant.Get(smi).AddTag(ShakeHarvestMonitor.Reserved);
		}).OnSignal(this.failed, this.harvest.seek).ToggleBehaviour(GameTags.Creatures.WantsToHarvest, (ShakeHarvestMonitor.Instance smi) => this.plant.Get(smi) != null, delegate(ShakeHarvestMonitor.Instance smi)
		{
			this.elapsedTime.Set(0f, smi, false);
		})
			.Exit(delegate(ShakeHarvestMonitor.Instance smi)
			{
				GameObject gameObject = this.plant.Get(smi);
				if (gameObject != null)
				{
					gameObject.RemoveTag(ShakeHarvestMonitor.Reserved);
					this.plant.Set(null, smi);
				}
			});
	}

	public static readonly Tag Reserved = GameTags.Creatures.ReservedByCreature;

	public GameStateMachine<ShakeHarvestMonitor, ShakeHarvestMonitor.Instance, IStateMachineTarget, ShakeHarvestMonitor.Def>.State cooldown;

	public ShakeHarvestMonitor.HarvestStates harvest;

	public StateMachine<ShakeHarvestMonitor, ShakeHarvestMonitor.Instance, IStateMachineTarget, ShakeHarvestMonitor.Def>.FloatParameter elapsedTime = new StateMachine<ShakeHarvestMonitor, ShakeHarvestMonitor.Instance, IStateMachineTarget, ShakeHarvestMonitor.Def>.FloatParameter(float.MaxValue);

	public StateMachine<ShakeHarvestMonitor, ShakeHarvestMonitor.Instance, IStateMachineTarget, ShakeHarvestMonitor.Def>.TargetParameter plant;

	public StateMachine<ShakeHarvestMonitor, ShakeHarvestMonitor.Instance, IStateMachineTarget, ShakeHarvestMonitor.Def>.Signal failed;

	public class Def : StateMachine.BaseDef
	{
		public Navigator.Scanner<KPrefabID> PlantSeeker
		{
			get
			{
				if (this.plantSeeker == null)
				{
					this.plantSeeker = new Navigator.Scanner<KPrefabID>(this.radius, GameScenePartitioner.Instance.plants, new Func<KPrefabID, bool>(this.IsHarvestablePlant));
					this.plantSeeker.SetDynamicOffsetsFn(delegate(KPrefabID plant, List<CellOffset> offsets)
					{
						ShakeHarvestMonitor.Def.GetApproachOffsets(plant.gameObject, offsets);
					});
				}
				return this.plantSeeker;
			}
		}

		private bool IsHarvestablePlant(KPrefabID plant)
		{
			if (plant == null)
			{
				return false;
			}
			if (plant.pendingDestruction)
			{
				return false;
			}
			if (plant.HasTag(ShakeHarvestMonitor.Reserved))
			{
				return false;
			}
			if (!this.harvestablePlants.Contains(plant.PrefabID()))
			{
				return false;
			}
			Harvestable component = plant.GetComponent<Harvestable>();
			return !(component == null) && component.CanBeHarvested;
		}

		public static void GetApproachOffsets(GameObject plant, List<CellOffset> offsets)
		{
			Extents extents = plant.GetComponent<OccupyArea>().GetExtents();
			int num = -1;
			int width = extents.width;
			for (int num2 = 0; num2 != extents.height; num2++)
			{
				int num3 = num2;
				offsets.Add(new CellOffset(num, num3));
				offsets.Add(new CellOffset(width, num3));
			}
		}

		public float cooldownDuration;

		public HashSet<Tag> harvestablePlants = new HashSet<Tag>();

		public int radius = 10;

		private Navigator.Scanner<KPrefabID> plantSeeker;
	}

	public class HarvestStates : GameStateMachine<ShakeHarvestMonitor, ShakeHarvestMonitor.Instance, IStateMachineTarget, ShakeHarvestMonitor.Def>.State
	{
		public GameStateMachine<ShakeHarvestMonitor, ShakeHarvestMonitor.Instance, IStateMachineTarget, ShakeHarvestMonitor.Def>.State seek;

		public GameStateMachine<ShakeHarvestMonitor, ShakeHarvestMonitor.Instance, IStateMachineTarget, ShakeHarvestMonitor.Def>.State execute;
	}

	public new class Instance : GameStateMachine<ShakeHarvestMonitor, ShakeHarvestMonitor.Instance, IStateMachineTarget, ShakeHarvestMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, ShakeHarvestMonitor.Def def)
			: base(master, def)
		{
			this.navigator = base.GetComponent<Navigator>();
		}

		public KPrefabID Seek()
		{
			return base.def.PlantSeeker.Scan(Grid.PosToXY(base.smi.transform.GetPosition()), this.navigator);
		}

		private readonly Navigator navigator;
	}
}
