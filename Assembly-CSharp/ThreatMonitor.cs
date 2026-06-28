using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class ThreatMonitor : GameStateMachine<ThreatMonitor, ThreatMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.safe;
		this.safe.Enter(delegate(ThreatMonitor.Instance smi)
		{
			smi.revengeThreat.Clear();
			smi.RefreshThreat(null);
		}).Update(delegate(ThreatMonitor.Instance smi)
		{
			smi.RefreshThreat(null);
		});
		this.threatned.duplicant.Update(delegate(ThreatMonitor.Instance smi)
		{
			if (!smi.CheckForThreats())
			{
				smi.GoTo(this.safe);
			}
		});
		this.threatned.duplicant.ShouldFight.ToggleChore(new Func<ThreatMonitor.Instance, Chore>(this.CreateAttackChore), this.safe, false);
		this.threatned.duplicant.ShoudFlee.ToggleChore(new Func<ThreatMonitor.Instance, Chore>(this.CreateFleeChore), this.safe, false);
		this.threatned.creature.Enter(delegate(ThreatMonitor.Instance smi)
		{
			this.ReportThreat(smi);
		}).Update(delegate(ThreatMonitor.Instance smi)
		{
			if (smi.revengeThreat.target != null && smi.master.gameObject != null && smi.revengeThreat.Calm(smi.dt, smi.master.gameObject))
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
		});
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

	[MyCmpReq]
	private FactionAlignment alignment;

	[MyCmpReq]
	private Navigator navigator;

	public Health.HealthState FleeThresholdState = Health.HealthState.Injured;

	public GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget>.State safe;

	public ThreatMonitor.ThreatnedStates threatned;

	public class ThreatnedStates : GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget>.State
	{
		public ThreatMonitor.ThreatnedDuplicantStates duplicant;

		public GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget>.State creature;
	}

	public class ThreatnedDuplicantStates : GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget>.State
	{
		public GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget>.State ShoudFlee;

		public GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget>.State ShouldFight;
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

	public new class Instance : GameStateMachine<ThreatMonitor, ThreatMonitor.Instance, IStateMachineTarget>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.alignment = master.GetComponent<FactionAlignment>();
			this.navigator = master.GetComponent<Navigator>();
			this.choreDriver = master.GetComponent<ChoreDriver>();
			base.Subscribe(-21431934, new EventSystem.EventHandler(this.OnSafe));
			base.Subscribe(-787691065, new EventSystem.EventHandler(this.OnAttacked));
			base.Subscribe(1969584890, new EventSystem.EventHandler(this.Cleanup));
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
				this.mainThreat.Unsubscribe(1623392196, new EventSystem.EventHandler(this.RefreshThreat));
				this.mainThreat.Unsubscribe(1969584890, new EventSystem.EventHandler(this.RefreshThreat));
				if (threat == null)
				{
					base.Trigger(2144432245, null);
				}
			}
			if (this.mainThreat != null)
			{
				this.mainThreat.Unsubscribe(1623392196, new EventSystem.EventHandler(this.RefreshThreat));
				this.mainThreat.Unsubscribe(1969584890, new EventSystem.EventHandler(this.RefreshThreat));
			}
			this.mainThreat = threat;
			if (this.mainThreat != null)
			{
				this.mainThreat.Subscribe(1623392196, new EventSystem.EventHandler(this.RefreshThreat));
				this.mainThreat.Subscribe(1969584890, new EventSystem.EventHandler(this.RefreshThreat));
			}
		}

		private void OnSafe(object data)
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

		private void OnAttacked(object data)
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
			Health component = base.smi.master.GetComponent<Health>();
			ChoreConsumer component2 = base.smi.master.GetComponent<ChoreConsumer>();
			return (!(component2 != null) || (component2.IsPermitted(Db.Get().ChoreGroups.Combat) && component2.IsEnabled(Db.Get().ChoreGroups.Combat))) && component.State < base.smi.sm.FleeThresholdState;
		}

		public void OnOffended(FactionAlignment offender)
		{
			this.OnAttacked(offender);
		}

		private void GotoThreatResponse()
		{
			if (this.WillEngageNonEssentialTargets())
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
			if (this.choreDriver != null)
			{
				this.GotoThreatResponse();
			}
			else
			{
				base.smi.GoTo(base.sm.threatned.creature);
			}
		}

		private void Cleanup(object data)
		{
			if (this.mainThreat)
			{
				this.mainThreat.Unsubscribe(1623392196, new EventSystem.EventHandler(this.RefreshThreat));
				this.mainThreat.Unsubscribe(1969584890, new EventSystem.EventHandler(this.RefreshThreat));
			}
		}

		public void RefreshThreat(object data)
		{
			if (!base.smi.CheckForThreats())
			{
				base.Trigger(-21431934, null);
				base.smi.GoTo(base.sm.safe);
			}
			else
			{
				this.GoToThreatned();
			}
		}

		public bool CheckForThreats()
		{
			GameObject gameObject;
			if (this.revengeThreat.target != null && this.revengeThreat.target.GetComponent<FactionAlignment>().CheckAlignmentActive && !this.revengeThreat.target.GetComponent<Health>().IsDefeated())
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

		public bool isValidThreat(FactionAlignment potentialThreat)
		{
			if (potentialThreat == null)
			{
				return false;
			}
			if (Vector2.Distance(base.transform.position, potentialThreat.transform.position) > (float)this.maxThreatDistance)
			{
				return false;
			}
			if (potentialThreat.GetHealth.IsDefeated())
			{
				return false;
			}
			AttackableBase component = potentialThreat.gameObject.GetComponent<AttackableBase>();
			return !(component == null) && this.navigator.CanReach(component);
		}

		public GameObject FindThreat()
		{
			this.threats.Clear();
			int mask = GameScenePartitioner.Instance.factionedEntities.mask;
			int num = Grid.OffsetCell(Grid.PosToCell(base.gameObject), new CellOffset(-this.maxThreatDistance / 2, -this.maxThreatDistance / 2));
			bool flag = this.WillEngageNonEssentialTargets();
			foreach (ScenePartitionerEntry scenePartitionerEntry in GameScenePartitioner.Instance.GatherEntries(Grid.CellToXY(num).x, Grid.CellToXY(num).y, this.maxThreatDistance, this.maxThreatDistance, mask))
			{
				GameObject gameObject = scenePartitionerEntry.obj as GameObject;
				if (!(gameObject == null))
				{
					FactionAlignment component = gameObject.GetComponent<FactionAlignment>();
					if (component.CheckAlignmentActive)
					{
						if (!(component == null) && !(component.gameObject == null) && !object.Equals(component.gameObject, null))
						{
							if (!this.threats.Contains(component))
							{
								if (!component.GetComponent<Health>().IsDefeated())
								{
									if (flag && this.alignment.Alignment == FactionManager.FactionID.Duplicant && component.targeted)
									{
										this.threats.Add(component);
									}
									else if (FactionManager.Instance.GetDisposition(this.alignment.Alignment, component.Alignment) == FactionManager.Disposition.Attack)
									{
										if (Vector3.Distance(base.transform.position, component.transform.position) <= (float)this.maxThreatDistance)
										{
											if (!(component == this.alignment))
											{
												if (this.isValidThreat(component))
												{
													this.threats.Add(component);
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			if (this.alignment.Alignment == FactionManager.FactionID.Duplicant && flag)
			{
				for (int i = 0; i < 6; i++)
				{
					if (i != 0)
					{
						foreach (FactionAlignment factionAlignment in FactionManager.Instance.GetFaction((FactionManager.FactionID)i).Members)
						{
							if (factionAlignment.targeted && !this.threats.Contains(factionAlignment))
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
			Vector2 vector = base.gameObject.transform.position;
			GameObject gameObject = null;
			float num2 = float.PositiveInfinity;
			for (int i = threats.Count - 1; i >= 0; i--)
			{
				FactionAlignment factionAlignment = threats[i];
				if (!this.navigator.CanReach(factionAlignment.GetComponent<IApproachable>()))
				{
					threats.Remove(factionAlignment);
				}
				else if (!factionAlignment.CheckAlignmentActive)
				{
					threats.Remove(factionAlignment);
				}
				else
				{
					float num3 = Vector2.Distance(vector, factionAlignment.transform.position) / num;
					if (num3 < num2)
					{
						num2 = num3;
						gameObject = factionAlignment.gameObject;
					}
				}
			}
			return gameObject;
		}

		public FactionAlignment alignment;

		private Navigator navigator;

		public ChoreDriver choreDriver;

		public ThreatMonitor.Grudge revengeThreat;

		private GameObject mainThreat;

		private List<FactionAlignment> threats = new List<FactionAlignment>();

		private int maxThreatDistance = 12;
	}
}
