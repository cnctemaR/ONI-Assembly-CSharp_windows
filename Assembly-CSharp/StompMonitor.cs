using System;
using System.Collections.Generic;
using UnityEngine;

public class StompMonitor : GameStateMachine<StompMonitor, StompMonitor.Instance, IStateMachineTarget, StompMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.cooldown;
		this.cooldown.ParamTransition<float>(this.TimeSinceLastStomp, this.stomp, new StateMachine<StompMonitor, StompMonitor.Instance, IStateMachineTarget, StompMonitor.Def>.Parameter<float>.Callback(StompMonitor.IsTimeToStomp)).Update(new Action<StompMonitor.Instance, float>(StompMonitor.CooldownTick), UpdateRate.SIM_200ms, false);
		this.stomp.ParamTransition<float>(this.TimeSinceLastStomp, this.cooldown, GameStateMachine<StompMonitor, StompMonitor.Instance, IStateMachineTarget, StompMonitor.Def>.IsLTEZero).DefaultState(this.stomp.lookingForTarget);
		this.stomp.lookingForTarget.ParamTransition<GameObject>(this.TargetPlant, this.stomp.stomping, GameStateMachine<StompMonitor, StompMonitor.Instance, IStateMachineTarget, StompMonitor.Def>.IsNotNull).PreBrainUpdate(new Action<StompMonitor.Instance>(StompMonitor.LookForTarget));
		this.stomp.stomping.Enter(new StateMachine<StompMonitor, StompMonitor.Instance, IStateMachineTarget, StompMonitor.Def>.State.Callback(StompMonitor.ReservePlant)).OnSignal(this.StompStateFailed, this.stomp.lookingForTarget).ToggleBehaviour(GameTags.Creatures.WantsToStomp, (StompMonitor.Instance smi) => smi.Target != null, new Action<StompMonitor.Instance>(StompMonitor.OnStompCompleted))
			.Exit(new StateMachine<StompMonitor, StompMonitor.Instance, IStateMachineTarget, StompMonitor.Def>.State.Callback(StompMonitor.UnreserveAndClearPlantTarget));
	}

	private static void ReservePlant(StompMonitor.Instance smi)
	{
		smi.Target.AddTag(StompMonitor.ReservedForStomp);
	}

	private static bool IsTimeToStomp(StompMonitor.Instance smi, float timeSinceLastStomp)
	{
		return timeSinceLastStomp > smi.def.Cooldown;
	}

	private static void CooldownTick(StompMonitor.Instance smi, float dt)
	{
		smi.sm.TimeSinceLastStomp.Set(smi.TimeSinceLastStomp + dt, smi, false);
	}

	private static void OnStompCompleted(StompMonitor.Instance smi)
	{
		smi.sm.TimeSinceLastStomp.Set(0f, smi, false);
	}

	private static void LookForTarget(StompMonitor.Instance smi)
	{
		smi.LookForTarget();
	}

	private static void UnreserveAndClearPlantTarget(StompMonitor.Instance smi)
	{
		if (smi.Target != null)
		{
			smi.Target.RemoveTag(StompMonitor.ReservedForStomp);
		}
		smi.sm.TargetPlant.Set(null, smi);
	}

	public static readonly Tag ReservedForStomp = GameTags.Creatures.ReservedByCreature;

	public GameStateMachine<StompMonitor, StompMonitor.Instance, IStateMachineTarget, StompMonitor.Def>.State cooldown;

	public StompMonitor.StompBehaviourStates stomp;

	public StateMachine<StompMonitor, StompMonitor.Instance, IStateMachineTarget, StompMonitor.Def>.FloatParameter TimeSinceLastStomp = new StateMachine<StompMonitor, StompMonitor.Instance, IStateMachineTarget, StompMonitor.Def>.FloatParameter(float.MaxValue);

	public StateMachine<StompMonitor, StompMonitor.Instance, IStateMachineTarget, StompMonitor.Def>.TargetParameter TargetPlant;

	public StateMachine<StompMonitor, StompMonitor.Instance, IStateMachineTarget, StompMonitor.Def>.Signal StompStateFailed;

	public class Def : StateMachine.BaseDef
	{
		public Navigator.Scanner<KPrefabID> PlantSeeker
		{
			get
			{
				if (this.plantSeeker == null)
				{
					this.plantSeeker = new Navigator.Scanner<KPrefabID>(this.radius, GameScenePartitioner.Instance.plants, new Func<KPrefabID, bool>(StompMonitor.Def.IsPlantTargetCandidate));
					this.plantSeeker.SetDynamicOffsetsFn(delegate(KPrefabID plant, List<CellOffset> offsets)
					{
						StompMonitor.Def.GetObjectCellsOffsetsWithExtraBottomPadding(plant.gameObject, offsets);
					});
				}
				return this.plantSeeker;
			}
		}

		private static bool IsPlantTargetCandidate(KPrefabID plant)
		{
			return !(plant == null) && !plant.pendingDestruction && !plant.HasTag(StompMonitor.ReservedForStomp) && plant.HasTag(GameTags.GrowingPlant) && plant.HasTag(GameTags.FullyGrown);
		}

		public static void GetObjectCellsOffsetsWithExtraBottomPadding(GameObject obj, List<CellOffset> offsets)
		{
			OccupyArea component = obj.GetComponent<OccupyArea>();
			int widthInCells = component.GetWidthInCells();
			int num = int.MaxValue;
			int num2 = int.MaxValue;
			for (int i = 0; i < component.OccupiedCellsOffsets.Length; i++)
			{
				CellOffset cellOffset = component.OccupiedCellsOffsets[i];
				offsets.Add(cellOffset);
				num = Mathf.Min(num, cellOffset.x);
				num2 = Mathf.Min(num2, cellOffset.y);
			}
			for (int j = 0; j < widthInCells; j++)
			{
				CellOffset cellOffset2 = new CellOffset(num + j, num2 - 1);
				offsets.Add(cellOffset2);
			}
		}

		public float Cooldown;

		public int radius = 10;

		private Navigator.Scanner<KPrefabID> plantSeeker;
	}

	public class StompBehaviourStates : GameStateMachine<StompMonitor, StompMonitor.Instance, IStateMachineTarget, StompMonitor.Def>.State
	{
		public GameStateMachine<StompMonitor, StompMonitor.Instance, IStateMachineTarget, StompMonitor.Def>.State lookingForTarget;

		public GameStateMachine<StompMonitor, StompMonitor.Instance, IStateMachineTarget, StompMonitor.Def>.State stomping;
	}

	public new class Instance : GameStateMachine<StompMonitor, StompMonitor.Instance, IStateMachineTarget, StompMonitor.Def>.GameInstance
	{
		public GameObject Target
		{
			get
			{
				return base.sm.TargetPlant.Get(this);
			}
		}

		public float TimeSinceLastStomp
		{
			get
			{
				return base.sm.TimeSinceLastStomp.Get(this);
			}
		}

		public Navigator Navigator { get; private set; }

		public Instance(IStateMachineTarget master, StompMonitor.Def def)
			: base(master, def)
		{
			this.Navigator = base.GetComponent<Navigator>();
		}

		public void LookForTarget()
		{
			KPrefabID kprefabID = base.def.PlantSeeker.Scan(Grid.PosToXY(base.transform.GetPosition()), this.Navigator);
			base.sm.TargetPlant.Set(kprefabID, this);
		}
	}
}
