using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ThreatMonitor : GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.safe;
		this.root.EventHandler(GameHashes.SafeFromThreats, delegate(ThreatMonitor.Instance smi, object d)
		{
			smi.OnSafe(d);
		}).EventHandler(GameHashes.Attacked, delegate(ThreatMonitor.Instance smi, object d)
		{
			smi.OnAttacked(d);
		}).EventHandler(GameHashes.ObjectDestroyed, delegate(ThreatMonitor.Instance smi, object d)
		{
			smi.Cleanup(d);
		});
		this.safe.Enter(delegate(ThreatMonitor.Instance smi)
		{
			smi.revengeThreat.Clear();
		}).Enter(new StateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State.Callback(ThreatMonitor.SeekThreats)).EventHandler(GameHashes.FactionChanged, new StateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State.Callback(ThreatMonitor.SeekThreats));
		this.safe.passive.DoNothing();
		this.safe.seeking.PreBrainUpdate(delegate(ThreatMonitor.Instance smi)
		{
			smi.RefreshThreat(null);
		});
		this.threatened.duplicant.Transition(this.safe, GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.Not(new StateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.Transition.ConditionCallback(ThreatMonitor.DupeHasValidTarget)), UpdateRate.SIM_200ms);
		this.threatened.duplicant.ShouldFight.ToggleChore(new Func<ThreatMonitor.Instance, Chore>(this.CreateAttackChore), this.safe).Update("DupeUpdateTarget", new Action<ThreatMonitor.Instance, float>(ThreatMonitor.DupeUpdateTarget), UpdateRate.SIM_200ms, false);
		this.threatened.duplicant.ShoudFlee.ToggleChore(new Func<ThreatMonitor.Instance, Chore>(this.CreateFleeChore), this.safe);
		this.threatened.creature.ToggleBehaviour(GameTags.Creatures.Flee, (ThreatMonitor.Instance smi) => !smi.WillFight(), delegate(ThreatMonitor.Instance smi)
		{
			smi.GoTo(this.safe);
		}).ToggleBehaviour(GameTags.Creatures.Attack, (ThreatMonitor.Instance smi) => smi.WillFight(), delegate(ThreatMonitor.Instance smi)
		{
			smi.GoTo(this.safe);
		}).Update("CritterCalmUpdate", new Action<ThreatMonitor.Instance, float>(ThreatMonitor.CritterCalmUpdate), UpdateRate.SIM_200ms, false)
			.PreBrainUpdate(new Action<ThreatMonitor.Instance>(ThreatMonitor.CritterUpdateThreats));
	}

	private static void SeekThreats(ThreatMonitor.Instance smi)
	{
		Faction faction = FactionManager.Instance.GetFaction(smi.alignment.Alignment);
		if (smi.IAmADuplicant || faction.CanAttack)
		{
			smi.GoTo(smi.sm.safe.seeking);
			return;
		}
		smi.GoTo(smi.sm.safe.passive);
	}

	private static bool DupeHasValidTarget(ThreatMonitor.Instance smi)
	{
		bool flag = false;
		if (smi.MainThreat != null && smi.MainThreat.GetComponent<FactionAlignment>().IsPlayerTargeted())
		{
			IApproachable component = smi.MainThreat.GetComponent<RangedAttackable>();
			if (component != null)
			{
				flag = smi.navigator.GetNavigationCost(component) != -1;
			}
		}
		return flag;
	}

	private static void DupeUpdateTarget(ThreatMonitor.Instance smi, float dt)
	{
		if (!ThreatMonitor.DupeHasValidTarget(smi))
		{
			smi.Trigger(2144432245, null);
		}
	}

	private static void CritterCalmUpdate(ThreatMonitor.Instance smi, float dt)
	{
		if (smi.isMasterNull)
		{
			return;
		}
		if (smi.revengeThreat.target != null && smi.revengeThreat.Calm(dt, smi.alignment))
		{
			smi.Trigger(-21431934, null);
		}
	}

	private static void CritterUpdateThreats(ThreatMonitor.Instance smi)
	{
		if (smi.isMasterNull)
		{
			return;
		}
		if (!smi.CheckForThreats() && !ThreatMonitor.IsInSafeState(smi))
		{
			smi.GoTo(smi.sm.safe);
		}
	}

	private static bool IsInSafeState(ThreatMonitor.Instance smi)
	{
		return smi.GetCurrentState() == smi.sm.safe.passive || smi.GetCurrentState() == smi.sm.safe.seeking;
	}

	private Chore CreateAttackChore(ThreatMonitor.Instance smi)
	{
		return new AttackChore(smi.master, smi.MainThreat);
	}

	private Chore CreateFleeChore(ThreatMonitor.Instance smi)
	{
		return new FleeChore(smi.master, smi.MainThreat);
	}

	public ThreatMonitor.SafeStates safe;

	public ThreatMonitor.ThreatenedStates threatened;

	public class Def : StateMachine.BaseDef
	{
		public Health.HealthState fleethresholdState = Health.HealthState.Injured;

		public Tag[] friendlyCreatureTags;

		public int maxSearchEntities = 50;

		public int maxSearchDistance = 20;

		public CellOffset[] offsets = OffsetGroups.Use;
	}

	public class SafeStates : GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State
	{
		public GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State passive;

		public GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State seeking;
	}

	public class ThreatenedStates : GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State
	{
		public ThreatMonitor.ThreatenedDuplicantStates duplicant;

		public GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State creature;
	}

	public class ThreatenedDuplicantStates : GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State
	{
		public GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State ShoudFlee;

		public GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State ShouldFight;
	}

	public struct Grudge
	{
		public void Reset(FactionAlignment revengeTarget)
		{
			this.target = revengeTarget;
			float num = 10f;
			this.grudgeTime = num;
		}

		public bool Calm(float dt, FactionAlignment self)
		{
			if (this.grudgeTime <= 0f)
			{
				return true;
			}
			this.grudgeTime = Mathf.Max(0f, this.grudgeTime - dt);
			if (this.grudgeTime == 0f)
			{
				if (FactionManager.Instance.GetDisposition(self.Alignment, this.target.Alignment) != FactionManager.Disposition.Attack)
				{
					PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, UI.GAMEOBJECTEFFECTS.FORGAVEATTACKER, self.transform, 2f, true);
				}
				this.Clear();
				return true;
			}
			return false;
		}

		public void Clear()
		{
			this.grudgeTime = 0f;
			this.target = null;
		}

		public bool IsValidRevengeTarget(bool isDuplicant)
		{
			return this.target != null && this.target.IsAlignmentActive() && (this.target.health == null || !this.target.health.IsDefeated()) && (!isDuplicant || !this.target.IsPlayerTargeted());
		}

		public FactionAlignment target;

		public float grudgeTime;
	}

	public new class Instance : GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.GameInstance
	{
		public GameObject MainThreat
		{
			get
			{
				return this.mainThreat;
			}
		}

		public bool IAmADuplicant
		{
			get
			{
				return this.alignment.Alignment == FactionManager.FactionID.Duplicant;
			}
		}

		public Instance(IStateMachineTarget master, ThreatMonitor.Def def)
			: base(master, def)
		{
			this.alignment = master.GetComponent<FactionAlignment>();
			this.navigator = master.GetComponent<Navigator>();
			this.choreDriver = master.GetComponent<ChoreDriver>();
			this.health = master.GetComponent<Health>();
			this.choreConsumer = master.GetComponent<ChoreConsumer>();
			this.refreshThreatDelegate = new Action<object>(this.RefreshThreat);
		}

		public void ClearMainThreat()
		{
			this.SetMainThreat(null);
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
				this.mainThreatFaction = this.mainThreat.GetComponent<FactionAlignment>().Alignment;
				this.mainThreat.Subscribe(1623392196, this.refreshThreatDelegate);
				this.mainThreat.Subscribe(1969584890, this.refreshThreatDelegate);
			}
		}

		public bool HasThreat()
		{
			return this.MainThreat != null;
		}

		public void OnSafe(object data)
		{
			if (this.revengeThreat.target != null)
			{
				if (!this.revengeThreat.target.GetComponent<FactionAlignment>().IsAlignmentActive())
				{
					this.revengeThreat.Clear();
				}
				this.ClearMainThreat();
			}
		}

		public void OnAttacked(object data)
		{
			FactionAlignment factionAlignment = (FactionAlignment)data;
			this.revengeThreat.Reset(factionAlignment);
			Game.BrainScheduler.PrioritizeBrain(base.GetComponent<Brain>());
			if (this.mainThreat == null)
			{
				this.SetMainThreat(factionAlignment.gameObject);
				this.GoToThreatened();
			}
			else if (!this.WillFight())
			{
				this.GoToThreatened();
			}
			if (factionAlignment.GetComponent<Bee>())
			{
				Chore chore = ((this.choreDriver != null) ? this.choreDriver.GetCurrentChore() : null);
				if (chore != null && chore.gameObject.GetComponent<HiveWorkableEmpty>() != null)
				{
					chore.gameObject.GetComponent<HiveWorkableEmpty>().wasStung = true;
				}
			}
		}

		public bool WillFight()
		{
			if (this.choreConsumer != null)
			{
				if (!this.choreConsumer.IsPermittedByUser(Db.Get().ChoreGroups.Combat))
				{
					return false;
				}
				if (!this.choreConsumer.IsPermittedByTraits(Db.Get().ChoreGroups.Combat))
				{
					return false;
				}
			}
			return (this.IAmADuplicant || base.smi.mainThreatFaction != FactionManager.FactionID.Predator) && this.health.State < base.smi.def.fleethresholdState;
		}

		private void GotoThreatResponse()
		{
			Chore currentChore = base.smi.master.GetComponent<ChoreDriver>().GetCurrentChore();
			if (this.WillFight() && this.mainThreat.GetComponent<FactionAlignment>().IsPlayerTargeted())
			{
				base.smi.GoTo(base.smi.sm.threatened.duplicant.ShouldFight);
				return;
			}
			if (currentChore != null && currentChore.target != null && currentChore.target != base.master && currentChore.target.GetComponent<Pickupable>() != null)
			{
				return;
			}
			base.smi.GoTo(base.smi.sm.threatened.duplicant.ShoudFlee);
		}

		public void GoToThreatened()
		{
			if (this.IAmADuplicant)
			{
				this.GotoThreatResponse();
				return;
			}
			base.smi.GoTo(base.sm.threatened.creature);
		}

		public void Cleanup(object data)
		{
			if (this.mainThreat)
			{
				this.mainThreat.Unsubscribe(1623392196, this.refreshThreatDelegate);
				this.mainThreat.Unsubscribe(1969584890, this.refreshThreatDelegate);
			}
		}

		public void RefreshThreat(object data)
		{
			if (!base.IsRunning())
			{
				return;
			}
			if (base.smi.CheckForThreats())
			{
				this.GoToThreatened();
				return;
			}
			if (!ThreatMonitor.IsInSafeState(base.smi))
			{
				base.Trigger(-21431934, null);
				base.smi.GoTo(base.sm.safe);
			}
		}

		public bool CheckForThreats()
		{
			if (base.isMasterNull)
			{
				return false;
			}
			GameObject gameObject;
			if (this.revengeThreat.IsValidRevengeTarget(this.IAmADuplicant))
			{
				gameObject = this.revengeThreat.target.gameObject;
			}
			else if (this.IAmADuplicant)
			{
				gameObject = this.FindThreatDuplicant();
			}
			else
			{
				gameObject = this.FindThreatOther();
			}
			this.SetMainThreat(gameObject);
			return gameObject != null;
		}

		private GameObject FindThreatDuplicant()
		{
			this.threats.Clear();
			if (this.WillFight())
			{
				foreach (object obj in Components.PlayerTargeted)
				{
					FactionAlignment factionAlignment = (FactionAlignment)obj;
					if (!factionAlignment.IsNullOrDestroyed() && factionAlignment.IsPlayerTargeted() && !factionAlignment.health.IsDefeated() && this.navigator.CanReach(factionAlignment.attackable.GetCell(), base.smi.def.offsets))
					{
						this.threats.Add(factionAlignment);
					}
				}
			}
			return this.PickBestTarget(this.threats);
		}

		private GameObject FindThreatOther()
		{
			this.threats.Clear();
			this.GatherThreats();
			return this.PickBestTarget(this.threats);
		}

		private static Util.IterationInstruction collectFactionAlignments(object obj, List<FactionAlignment> alignments)
		{
			alignments.Add(obj as FactionAlignment);
			return Util.IterationInstruction.Continue;
		}

		private void GatherThreats()
		{
			ListPool<FactionAlignment, ThreatMonitor>.PooledList pooledList = ListPool<FactionAlignment, ThreatMonitor>.Allocate();
			Extents extents = new Extents(Grid.PosToCell(base.gameObject), base.def.maxSearchDistance);
			GameScenePartitioner.Instance.VisitEntries<ListPool<FactionAlignment, ThreatMonitor>.PooledList>(extents.x, extents.y, extents.width, extents.height, GameScenePartitioner.Instance.attackableEntitiesLayer, new Func<object, ListPool<FactionAlignment, ThreatMonitor>.PooledList, Util.IterationInstruction>(ThreatMonitor.Instance.collectFactionAlignments), pooledList);
			int count = pooledList.Count;
			int num = Mathf.Min(count, base.def.maxSearchEntities);
			for (int i = 0; i < num; i++)
			{
				if (this.currentUpdateIndex >= count)
				{
					this.currentUpdateIndex = 0;
				}
				FactionAlignment factionAlignment = pooledList[this.currentUpdateIndex];
				this.currentUpdateIndex++;
				if (!(factionAlignment.transform == null) && !(factionAlignment == this.alignment) && (base.def.friendlyCreatureTags == null || !factionAlignment.kprefabID.HasAnyTags(base.def.friendlyCreatureTags)) && factionAlignment.IsAlignmentActive() && FactionManager.Instance.GetDisposition(this.alignment.Alignment, factionAlignment.Alignment) == FactionManager.Disposition.Attack && this.navigator.CanReach(factionAlignment.attackable.GetCell(), base.smi.def.offsets))
				{
					this.threats.Add(factionAlignment);
				}
			}
			pooledList.Recycle();
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

		public FactionAlignment alignment;

		public Navigator navigator;

		public ChoreDriver choreDriver;

		private Health health;

		private ChoreConsumer choreConsumer;

		public ThreatMonitor.Grudge revengeThreat;

		public int currentUpdateIndex;

		private GameObject mainThreat;

		private FactionManager.FactionID mainThreatFaction;

		private List<FactionAlignment> threats = new List<FactionAlignment>();

		private Action<object> refreshThreatDelegate;
	}
}
