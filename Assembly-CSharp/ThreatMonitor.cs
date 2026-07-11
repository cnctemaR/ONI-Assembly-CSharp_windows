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
			smi.RefreshThreat(null);
		}).Update("safe", delegate(ThreatMonitor.Instance smi, float dt)
		{
			smi.RefreshThreat(null);
		}, UpdateRate.SIM_1000ms, true);
		this.threatened.duplicant.Transition(this.safe, (ThreatMonitor.Instance smi) => !smi.CheckForThreats(), UpdateRate.SIM_200ms);
		this.threatened.duplicant.ShouldFight.ToggleChore(new Func<ThreatMonitor.Instance, Chore>(this.CreateAttackChore), this.safe).Update("DupeUpdateTarget", new Action<ThreatMonitor.Instance, float>(ThreatMonitor.DupeUpdateTarget), UpdateRate.SIM_200ms, false);
		this.threatened.duplicant.ShoudFlee.ToggleChore(new Func<ThreatMonitor.Instance, Chore>(this.CreateFleeChore), this.safe);
		this.threatened.creature.ToggleBehaviour(GameTags.Creatures.Flee, (ThreatMonitor.Instance smi) => !smi.WillFight(), delegate(ThreatMonitor.Instance smi)
		{
			smi.GoTo(this.safe);
		}).ToggleBehaviour(GameTags.Creatures.Attack, (ThreatMonitor.Instance smi) => smi.WillFight(), delegate(ThreatMonitor.Instance smi)
		{
			smi.GoTo(this.safe);
		}).Update("Threatened", new Action<ThreatMonitor.Instance, float>(ThreatMonitor.CritterUpdateThreats), UpdateRate.SIM_200ms, false);
	}

	private static void DupeUpdateTarget(ThreatMonitor.Instance smi, float dt)
	{
		if (smi.MainThreat == null || !smi.MainThreat.GetComponent<FactionAlignment>().targeted)
		{
			smi.Trigger(2144432245, null);
		}
	}

	private static void CritterUpdateThreats(ThreatMonitor.Instance smi, float dt)
	{
		if (smi.isMasterNull)
		{
			return;
		}
		if (smi.revengeThreat.target != null && smi.revengeThreat.Calm(dt, smi.alignment))
		{
			smi.Trigger(-21431934, null);
			return;
		}
		if (!smi.CheckForThreats())
		{
			smi.GoTo(smi.sm.safe);
		}
	}

	private Chore CreateAttackChore(ThreatMonitor.Instance smi)
	{
		return new AttackChore(smi.master, smi.MainThreat);
	}

	private Chore CreateFleeChore(ThreatMonitor.Instance smi)
	{
		return new FleeChore(smi.master, smi.MainThreat);
	}

	private FactionAlignment alignment;

	private Navigator navigator;

	public GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State safe;

	public ThreatMonitor.ThreatenedStates threatened;

	public class Def : StateMachine.BaseDef
	{
		public Health.HealthState fleethresholdState = Health.HealthState.Injured;
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

		public FactionAlignment target;

		public float grudgeTime;
	}

	public new class Instance : GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, ThreatMonitor.Def def)
			: base(master, def)
		{
			this.alignment = master.GetComponent<FactionAlignment>();
			this.navigator = master.GetComponent<Navigator>();
			this.choreDriver = master.GetComponent<ChoreDriver>();
			this.health = master.GetComponent<Health>();
			this.choreConsumer = master.GetComponent<ChoreConsumer>();
		}

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
				this.mainThreat.Unsubscribe(1623392196, new Action<object>(this.RefreshThreat));
				this.mainThreat.Unsubscribe(1969584890, new Action<object>(this.RefreshThreat));
				if (threat == null)
				{
					base.Trigger(2144432245, null);
				}
			}
			if (this.mainThreat != null)
			{
				this.mainThreat.Unsubscribe(1623392196, new Action<object>(this.RefreshThreat));
				this.mainThreat.Unsubscribe(1969584890, new Action<object>(this.RefreshThreat));
			}
			this.mainThreat = threat;
			if (this.mainThreat != null)
			{
				this.mainThreat.Subscribe(1623392196, new Action<object>(this.RefreshThreat));
				this.mainThreat.Subscribe(1969584890, new Action<object>(this.RefreshThreat));
			}
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
			if (this.mainThreat == null)
			{
				this.SetMainThreat(factionAlignment.gameObject);
				this.GoToThreatened();
			}
			else if (!this.WillFight())
			{
				this.GoToThreatened();
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
			bool flag = this.health.State >= base.smi.def.fleethresholdState;
			return !flag;
		}

		private void GotoThreatResponse()
		{
			if (this.WillFight() && this.mainThreat.GetComponent<FactionAlignment>().targeted)
			{
				base.smi.GoTo(base.smi.sm.threatened.duplicant.ShouldFight);
			}
			else
			{
				base.smi.GoTo(base.smi.sm.threatened.duplicant.ShoudFlee);
			}
		}

		public void GoToThreatened()
		{
			if (this.IAmADuplicant)
			{
				this.GotoThreatResponse();
			}
			else
			{
				base.smi.GoTo(base.sm.threatened.creature);
			}
		}

		public void Cleanup(object data)
		{
			if (this.mainThreat)
			{
				this.mainThreat.Unsubscribe(1623392196, new Action<object>(this.RefreshThreat));
				this.mainThreat.Unsubscribe(1969584890, new Action<object>(this.RefreshThreat));
			}
		}

		public void RefreshThreat(object data)
		{
			if (!base.IsRunning())
			{
				return;
			}
			bool flag = base.smi.CheckForThreats();
			if (flag)
			{
				this.GoToThreatened();
			}
			else if (base.smi.GetCurrentState() != base.sm.safe)
			{
				base.Trigger(-21431934, null);
				base.smi.GoTo(base.sm.safe);
			}
		}

		public bool CheckForThreats()
		{
			GameObject gameObject;
			if (this.revengeThreat.target != null && this.revengeThreat.target.IsAlignmentActive() && !this.revengeThreat.target.health.IsDefeated() && (!this.IAmADuplicant || !this.revengeThreat.target.targeted))
			{
				gameObject = this.revengeThreat.target.gameObject;
			}
			else
			{
				gameObject = this.FindThreat();
			}
			this.SetMainThreat(gameObject);
			return gameObject != null;
		}

		public GameObject FindThreat()
		{
			this.threats.Clear();
			if (base.isMasterNull)
			{
				return null;
			}
			bool flag = this.WillFight();
			if (this.IAmADuplicant && flag)
			{
				for (int i = 0; i < 6; i++)
				{
					if (i != 0)
					{
						foreach (FactionAlignment factionAlignment in FactionManager.Instance.GetFaction((FactionManager.FactionID)i).Members)
						{
							if (factionAlignment.targeted && !factionAlignment.health.IsDefeated() && !this.threats.Contains(factionAlignment) && this.navigator.CanReach(factionAlignment.attackable))
							{
								this.threats.Add(factionAlignment);
							}
						}
					}
				}
			}
			if (this.threats.Count == 0)
			{
				return null;
			}
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

		public FactionAlignment alignment;

		private Navigator navigator;

		public ChoreDriver choreDriver;

		private Health health;

		private ChoreConsumer choreConsumer;

		public ThreatMonitor.Grudge revengeThreat;

		private GameObject mainThreat;

		private List<FactionAlignment> threats = new List<FactionAlignment>();
	}
}
