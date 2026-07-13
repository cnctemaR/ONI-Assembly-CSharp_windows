using System;
using Klei.AI;
using UnityEngine;

public class PollinateMonitor : GameStateMachine<PollinateMonitor, PollinateMonitor.Instance, IStateMachineTarget, PollinateMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.lookingForPlant;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.lookingForPlant.PreBrainUpdate(new Action<PollinateMonitor.Instance>(PollinateMonitor.FindPollinateTarget)).ToggleBehaviour(GameTags.Creatures.WantsToPollinate, (PollinateMonitor.Instance smi) => smi.IsValidTarget(), delegate(PollinateMonitor.Instance smi)
		{
			smi.GoTo(this.satisfied);
		});
		this.satisfied.Enter(delegate(PollinateMonitor.Instance smi)
		{
			this.remainingSecondsForEffect.Set(ButterflyTuning.SEARCH_COOLDOWN, smi, false);
		}).ScheduleGoTo((PollinateMonitor.Instance smi) => this.remainingSecondsForEffect.Get(smi), this.lookingForPlant);
	}

	private static void FindPollinateTarget(PollinateMonitor.Instance smi)
	{
		if (smi.IsValidTarget())
		{
			return;
		}
		KPrefabID kprefabID = smi.def.PlantSeeker.Scan(Grid.PosToXY(smi.transform.GetPosition()), smi.navigator);
		GameObject gameObject = ((kprefabID != null) ? kprefabID.gameObject : null);
		if (gameObject != smi.target)
		{
			if (gameObject == null)
			{
				smi.target = null;
				smi.targetCell = -1;
			}
			else
			{
				smi.target = gameObject;
				smi.targetCell = Grid.PosToCell(smi.target);
			}
			smi.Trigger(-255880159, null);
		}
	}

	public static Tag ID = new Tag("PollinateMonitor");

	public GameStateMachine<PollinateMonitor, PollinateMonitor.Instance, IStateMachineTarget, PollinateMonitor.Def>.State lookingForPlant;

	public GameStateMachine<PollinateMonitor, PollinateMonitor.Instance, IStateMachineTarget, PollinateMonitor.Def>.State satisfied;

	private StateMachine<PollinateMonitor, PollinateMonitor.Instance, IStateMachineTarget, PollinateMonitor.Def>.FloatParameter remainingSecondsForEffect;

	public class Def : StateMachine.BaseDef
	{
		public Navigator.Scanner<KPrefabID> PlantSeeker
		{
			get
			{
				if (this.plantSeeker == null)
				{
					this.plantSeeker = new Navigator.Scanner<KPrefabID>(this.radius, GameScenePartitioner.Instance.plants, new Func<KPrefabID, bool>(PollinateMonitor.Def.IsHarvestablePlant));
					this.plantSeeker.SetEarlyOutThreshold(5);
				}
				return this.plantSeeker;
			}
		}

		private static bool IsHarvestablePlant(KPrefabID plant)
		{
			if (plant == null)
			{
				return false;
			}
			if (plant.HasTag(GameTags.Creatures.ReservedByCreature))
			{
				return false;
			}
			if (plant.HasTag("ButterflyPlant"))
			{
				return false;
			}
			if (!plant.HasTag(GameTags.GrowingPlant))
			{
				return false;
			}
			if (plant.HasTag(GameTags.FullyGrown))
			{
				return false;
			}
			Effects component = plant.GetComponent<Effects>();
			if (component == null)
			{
				return false;
			}
			for (int i = 0; i < PollinationMonitor.PollinationEffects.Length; i++)
			{
				HashedString hashedString = PollinationMonitor.PollinationEffects[i];
				if (component.HasEffect(hashedString))
				{
					return false;
				}
			}
			return true;
		}

		public int radius = 10;

		private Navigator.Scanner<KPrefabID> plantSeeker;
	}

	public new class Instance : GameStateMachine<PollinateMonitor, PollinateMonitor.Instance, IStateMachineTarget, PollinateMonitor.Def>.GameInstance, IApproachableBehaviour, ICreatureMonitor
	{
		public Instance(IStateMachineTarget master, PollinateMonitor.Def def)
			: base(master, def)
		{
			this.navigator = master.GetComponent<Navigator>();
		}

		public Tag Id
		{
			get
			{
				return PollinateMonitor.ID;
			}
		}

		public bool IsValidTarget()
		{
			return !this.target.IsNullOrDestroyed() && this.navigator.GetNavigationCost(this.targetCell) != -1;
		}

		public GameObject GetTarget()
		{
			return this.target;
		}

		public StatusItem GetApproachStatusItem()
		{
			return Db.Get().CreatureStatusItems.TravelingToPollinate;
		}

		public StatusItem GetBehaviourStatusItem()
		{
			return Db.Get().CreatureStatusItems.Pollinating;
		}

		public void OnSuccess()
		{
			Effects component = this.target.GetComponent<Effects>();
			if (component != null)
			{
				component.Add(Db.Get().effects.Get("ButterflyPollinated"), true);
			}
			this.target = null;
		}

		public GameObject target;

		public int targetCell;

		public Navigator navigator;
	}
}
