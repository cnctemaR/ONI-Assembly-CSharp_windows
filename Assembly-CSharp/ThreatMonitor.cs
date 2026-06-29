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
		}, UpdateRate.SIM_200ms, true);
		this.threatned.duplicant.Transition(this.safe, (ThreatMonitor.Instance smi) => !smi.CheckForThreats(), UpdateRate.SIM_200ms);
		this.threatned.duplicant.ShouldFight.ToggleChore(new Func<ThreatMonitor.Instance, Chore>(this.CreateAttackChore), this.safe).Update("ShouldFight", delegate(ThreatMonitor.Instance smi, float dt)
		{
			if (this.GetMainThreat(smi) == null || !this.GetMainThreat(smi).GetComponent<FactionAlignment>().targeted)
			{
				smi.Trigger(2144432245, null);
			}
		}, UpdateRate.SIM_200ms, false);
		this.threatned.duplicant.ShoudFlee.ToggleChore(new Func<ThreatMonitor.Instance, Chore>(this.CreateFleeChore), this.safe);
		this.threatned.creature.ToggleBehaviour(GameTags.Creatures.Flee, (ThreatMonitor.Instance smi) => !CreatureHelpers.WillEngageNonEssentialTargets(smi.gameObject, smi.def.fleethresholdState), delegate(ThreatMonitor.Instance smi)
		{
			smi.GoTo(this.safe);
		}).ToggleBehaviour(GameTags.Creatures.Attack, (ThreatMonitor.Instance smi) => CreatureHelpers.WillEngageNonEssentialTargets(smi.gameObject, smi.def.fleethresholdState), delegate(ThreatMonitor.Instance smi)
		{
			smi.GoTo(this.safe);
		}).Enter(delegate(ThreatMonitor.Instance smi)
		{
			this.ReportThreat(smi);
		})
			.Update("Threatened", delegate(ThreatMonitor.Instance smi, float dt)
			{
				if (smi.isMasterNull)
				{
					return;
				}
				if (smi.revengeThreat.target != null && smi.revengeThreat.Calm(dt, smi.master.gameObject))
				{
					smi.Trigger(-21431934, null);
					return;
				}
				if (!smi.CheckForThreats())
				{
					smi.GoTo(this.safe);
				}
				else
				{
					this.ReportThreat(smi);
				}
			}, UpdateRate.SIM_200ms, false);
	}

	public GameObject GetMainThreat(ThreatMonitor.Instance smi)
	{
		return smi.GetMainThreat;
	}

	private void ReportThreat(ThreatMonitor.Instance smi)
	{
		smi.master.Trigger(229718515, smi.GetMainThreat);
	}

	private Chore CreateAttackChore(ThreatMonitor.Instance smi)
	{
		return new AttackChore(smi.master, smi.GetMainThreat);
	}

	private Chore CreateFleeChore(ThreatMonitor.Instance smi)
	{
		return new FleeChore(smi.master, smi.GetMainThreat);
	}

	private FactionAlignment alignment;

	private Navigator navigator;

	public GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State safe;

	public ThreatMonitor.ThreatnedStates threatned;

	public class Def : StateMachine.BaseDef
	{
		public Health.HealthState fleethresholdState = Health.HealthState.Injured;
	}

	public class ThreatnedStates : GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State
	{
		public ThreatMonitor.ThreatnedDuplicantStates duplicant;

		public GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State creature;
	}

	public class ThreatnedDuplicantStates : GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State
	{
		public GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State ShoudFlee;

		public GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget, ThreatMonitor.Def>.State ShouldFight;
	}

	public struct Grudge
	{
		public void reset(GameObject revengeTarget)
		{
			this.target = revengeTarget;
			float num = 10f;
			this.grudgeTime = num;
		}

		public bool Calm(float dt, GameObject self)
		{
			if (this.grudgeTime <= 0f)
			{
				return true;
			}
			this.grudgeTime = Mathf.Max(0f, this.grudgeTime - dt);
			if (this.grudgeTime == 0f)
			{
				if (FactionManager.Instance.GetDisposition(self.GetComponent<FactionAlignment>().Alignment, this.target.GetComponent<FactionAlignment>().Alignment) != FactionManager.Disposition.Attack)
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

		public GameObject target;

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

		public GameObject GetMainThreat
		{
			get
			{
				return this.mainThreat;
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
				if (!this.revengeThreat.target.GetComponent<FactionAlignment>().CheckAlignmentActive)
				{
					this.revengeThreat.Clear();
				}
				this.ClearMainThreat();
			}
		}

		public void OnAttacked(object data)
		{
			FactionAlignment factionAlignment = (FactionAlignment)data;
			this.revengeThreat.reset(factionAlignment.gameObject);
			if (this.mainThreat == null)
			{
				this.SetMainThreat(factionAlignment.gameObject);
				this.GoToThreatned();
			}
			else if (!this.WillEngageNonEssentialTargets())
			{
				this.GoToThreatned();
			}
		}

		public bool WillEngageNonEssentialTargets()
		{
			return (!(this.choreConsumer != null) || (this.choreConsumer.IsPermittedByUser(Db.Get().ChoreGroups.Combat) && this.choreConsumer.IsPermittedByTraits(Db.Get().ChoreGroups.Combat))) && this.health.State < base.smi.def.fleethresholdState;
		}

		public void OnOffended(FactionAlignment offender)
		{
			this.OnAttacked(offender);
		}

		private void GotoThreatResponse()
		{
			if (this.WillEngageNonEssentialTargets() && this.mainThreat.GetComponent<FactionAlignment>().targeted)
			{
				base.smi.GoTo(base.smi.sm.threatned.duplicant.ShouldFight);
			}
			else
			{
				base.smi.GoTo(base.smi.sm.threatned.duplicant.ShoudFlee);
			}
		}

		public void GoToThreatned()
		{
			if (base.GetComponent<MinionIdentity>() != null)
			{
				this.GotoThreatResponse();
			}
			else
			{
				base.smi.GoTo(base.sm.threatned.creature);
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
				this.GoToThreatned();
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
			if (this.revengeThreat.target != null && this.revengeThreat.target.GetComponent<FactionAlignment>().CheckAlignmentActive && !this.revengeThreat.target.GetComponent<Health>().IsDefeated() && (this.alignment.Alignment != FactionManager.FactionID.Duplicant || !this.revengeThreat.target.GetComponent<FactionAlignment>().targeted))
			{
				gameObject = this.revengeThreat.target;
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
			int num = Grid.OffsetCell(Grid.PosToCell(base.gameObject), new CellOffset(-this.maxThreatDistance / 2, -this.maxThreatDistance / 2));
			bool flag = this.WillEngageNonEssentialTargets();
			List<ScenePartitionerEntry> list = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(Grid.CellToXY(num).x, Grid.CellToXY(num).y, this.maxThreatDistance, this.maxThreatDistance, GameScenePartitioner.Instance.attackableEntitiesLayer, list);
			for (int i = 0; i < list.Count; i++)
			{
				ScenePartitionerEntry scenePartitionerEntry = list[i];
				FactionAlignment factionAlignment = scenePartitionerEntry.obj as FactionAlignment;
				if (!this.threats.Contains(factionAlignment))
				{
					if (!(factionAlignment.transform == null))
					{
						if (!(factionAlignment == this.alignment))
						{
							if (this.navigator.CanReach(factionAlignment.attackable))
							{
								if (factionAlignment.CheckAlignmentActive)
								{
									if (this.alignment.Alignment != FactionManager.FactionID.Duplicant || factionAlignment.targeted)
									{
										if (flag && this.alignment.Alignment == FactionManager.FactionID.Duplicant && factionAlignment.targeted)
										{
											this.threats.Add(factionAlignment);
										}
										else if (FactionManager.Instance.GetDisposition(this.alignment.Alignment, factionAlignment.Alignment) == FactionManager.Disposition.Attack)
										{
											this.threats.Add(factionAlignment);
										}
									}
								}
							}
						}
					}
				}
			}
			ListPool<ScenePartitionerEntry, GameScenePartitioner>.Free(list);
			if (this.alignment.Alignment == FactionManager.FactionID.Duplicant && flag)
			{
				for (int j = 0; j < 6; j++)
				{
					if (j != 0)
					{
						foreach (FactionAlignment factionAlignment2 in FactionManager.Instance.GetFaction((FactionManager.FactionID)j).Members)
						{
							if (factionAlignment2.targeted && !this.threats.Contains(factionAlignment2) && this.navigator.CanReach(factionAlignment2.attackable))
							{
								this.threats.Add(factionAlignment2);
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

		private int maxThreatDistance = 12;
	}
}
