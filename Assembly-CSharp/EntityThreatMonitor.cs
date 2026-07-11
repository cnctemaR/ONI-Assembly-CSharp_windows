using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityThreatMonitor : GameStateMachine<EntityThreatMonitor, EntityThreatMonitor.Instance, IStateMachineTarget, EntityThreatMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.safe;
		this.root.EventHandler(GameHashes.ObjectDestroyed, delegate(EntityThreatMonitor.Instance smi, object d)
		{
			smi.Cleanup(d);
		});
		this.safe.Enter(delegate(EntityThreatMonitor.Instance smi)
		{
			smi.RefreshThreat(null);
		}).Update("safe", delegate(EntityThreatMonitor.Instance smi, float dt)
		{
			smi.RefreshThreat(null);
		}, UpdateRate.SIM_1000ms, true);
		this.threatened.ToggleBehaviour(GameTags.Creatures.Defend, (EntityThreatMonitor.Instance smi) => smi.MainThreat != null, delegate(EntityThreatMonitor.Instance smi)
		{
			smi.GoTo(this.safe);
		}).Update("Threatened", new Action<EntityThreatMonitor.Instance, float>(EntityThreatMonitor.CritterUpdateThreats), UpdateRate.SIM_200ms, false);
	}

	private static void CritterUpdateThreats(EntityThreatMonitor.Instance smi, float dt)
	{
		if (smi.isMasterNull)
		{
			return;
		}
		if (!smi.CheckForThreats())
		{
			smi.GoTo(smi.sm.safe);
		}
	}

	public GameStateMachine<EntityThreatMonitor, EntityThreatMonitor.Instance, IStateMachineTarget, EntityThreatMonitor.Def>.State safe;

	public GameStateMachine<EntityThreatMonitor, EntityThreatMonitor.Instance, IStateMachineTarget, EntityThreatMonitor.Def>.State threatened;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<EntityThreatMonitor, EntityThreatMonitor.Instance, IStateMachineTarget, EntityThreatMonitor.Def>.GameInstance
	{
		public GameObject MainThreat
		{
			get
			{
				return this.mainThreat;
			}
		}

		public Instance(IStateMachineTarget master, EntityThreatMonitor.Def def)
			: base(master, def)
		{
			this.alignment = master.GetComponent<FactionAlignment>();
			this.navigator = master.GetComponent<Navigator>();
			this.choreDriver = master.GetComponent<ChoreDriver>();
			this.refreshThreatDelegate = new Action<object>(this.RefreshThreat);
		}

		public void SetMainThreat(GameObject threat)
		{
			if (threat == this.mainThreat)
			{
				return;
			}
			if (this.mainThreat != null)
			{
				this.mainThreat.Unsubscribe(1623392196, this.refreshThreatDelegate);
				this.mainThreat.Unsubscribe(1969584890, this.refreshThreatDelegate);
				if (threat == null)
				{
					base.Trigger(2144432245, null);
				}
			}
			if (this.mainThreat != null)
			{
				this.mainThreat.Unsubscribe(1623392196, this.refreshThreatDelegate);
				this.mainThreat.Unsubscribe(1969584890, this.refreshThreatDelegate);
			}
			this.mainThreat = threat;
			if (this.mainThreat != null)
			{
				this.mainThreat.Subscribe(1623392196, this.refreshThreatDelegate);
				this.mainThreat.Subscribe(1969584890, this.refreshThreatDelegate);
			}
		}

		public void Cleanup(object data)
		{
			if (this.mainThreat)
			{
				this.mainThreat.Unsubscribe(1623392196, this.refreshThreatDelegate);
				this.mainThreat.Unsubscribe(1969584890, this.refreshThreatDelegate);
			}
		}

		public void GoToThreatened()
		{
			base.smi.GoTo(base.sm.threatened);
		}

		public void RefreshThreat(object data)
		{
			if (!base.IsRunning() || this.entityToProtect == null)
			{
				return;
			}
			if (base.smi.CheckForThreats())
			{
				this.GoToThreatened();
				return;
			}
			if (base.smi.GetCurrentState() != base.sm.safe)
			{
				base.Trigger(-21431934, null);
				base.smi.GoTo(base.sm.safe);
			}
		}

		public bool CheckForThreats()
		{
			if (this.entityToProtect == null)
			{
				return false;
			}
			GameObject gameObject = this.FindThreat();
			this.SetMainThreat(gameObject);
			return gameObject != null;
		}

		public GameObject FindThreat()
		{
			this.threats.Clear();
			ListPool<ScenePartitionerEntry, ThreatMonitor>.PooledList pooledList = ListPool<ScenePartitionerEntry, ThreatMonitor>.Allocate();
			Extents extents = new Extents(Grid.PosToCell(this.entityToProtect), this.maxThreatDistance);
			GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.attackableEntitiesLayer, pooledList);
			for (int i = 0; i < pooledList.Count; i++)
			{
				FactionAlignment factionAlignment = pooledList[i].obj as FactionAlignment;
				if (!(factionAlignment.transform == null) && !(factionAlignment == this.alignment) && factionAlignment.IsAlignmentActive() && this.navigator.CanReach(factionAlignment.attackable) && (!(this.allyTag != null) || !factionAlignment.HasTag(this.allyTag)))
				{
					this.threats.Add(factionAlignment);
				}
			}
			pooledList.Recycle();
			return this.PickBestTarget(this.threats);
		}

		public GameObject PickBestTarget(List<FactionAlignment> threats)
		{
			float num = 1f;
			Vector2 vector = base.gameObject.transform.GetPosition();
			GameObject gameObject = null;
			float num2 = float.PositiveInfinity;
			for (int i = threats.Count - 1; i >= 0; i--)
			{
				FactionAlignment factionAlignment = threats[i];
				float num3 = Vector2.Distance(vector, factionAlignment.transform.GetPosition()) / num;
				if (num3 < num2)
				{
					num2 = num3;
					gameObject = factionAlignment.gameObject;
				}
			}
			return gameObject;
		}

		public GameObject entityToProtect;

		public FactionAlignment alignment;

		private Navigator navigator;

		public ChoreDriver choreDriver;

		public Tag allyTag;

		private GameObject mainThreat;

		private List<FactionAlignment> threats = new List<FactionAlignment>();

		private int maxThreatDistance = 6;

		private Action<object> refreshThreatDelegate;
	}
}
