using System;

public class WoundMonitor : GameStateMachine<WoundMonitor, WoundMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.healthy;
		this.root.ToggleAnims("anim_hits_kanim", 0f).EventHandler(GameHashes.HealthChanged, delegate(WoundMonitor.Instance smi, object data)
		{
			smi.OnHealthChanged(data);
		});
		this.healthy.EventTransition(GameHashes.HealthChanged, this.wounded, (WoundMonitor.Instance smi) => smi.health.State != Health.HealthState.Perfect);
		this.wounded.ToggleUrge(Db.Get().Urges.Heal).Enter(delegate(WoundMonitor.Instance smi)
		{
			Health.HealthState state = smi.health.State;
			if (state != Health.HealthState.Critical)
			{
				if (state != Health.HealthState.Injured)
				{
					if (state == Health.HealthState.Scuffed)
					{
						smi.GoTo(this.wounded.light);
					}
				}
				else
				{
					smi.GoTo(this.wounded.medium);
				}
			}
			else
			{
				smi.GoTo(this.wounded.heavy);
			}
		}).EventHandler(GameHashes.HealthChanged, delegate(WoundMonitor.Instance smi)
		{
			smi.GoToProperHeathState();
		});
		this.wounded.medium.ToggleAnims("anim_loco_wounded_kanim", 1f);
		this.wounded.heavy.ToggleAnims("anim_loco_wounded_kanim", 3f);
	}

	public GameStateMachine<WoundMonitor, WoundMonitor.Instance, IStateMachineTarget, object>.State healthy;

	public WoundMonitor.Wounded wounded;

	public class Wounded : GameStateMachine<WoundMonitor, WoundMonitor.Instance, IStateMachineTarget, object>.State
	{
		public GameStateMachine<WoundMonitor, WoundMonitor.Instance, IStateMachineTarget, object>.State light;

		public GameStateMachine<WoundMonitor, WoundMonitor.Instance, IStateMachineTarget, object>.State medium;

		public GameStateMachine<WoundMonitor, WoundMonitor.Instance, IStateMachineTarget, object>.State heavy;
	}

	public new class Instance : GameStateMachine<WoundMonitor, WoundMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.health = master.GetComponent<Health>();
			this.worker = master.GetComponent<Worker>();
		}

		public void OnHealthChanged(object data)
		{
			float num = (float)data;
			if (this.health.hitPoints != 0f && num < 0f)
			{
				this.PlayHitAnimation();
			}
		}

		private void PlayHitAnimation()
		{
			string text = null;
			if (this.animController.CurrentAnim != null)
			{
				text = this.animController.CurrentAnim.name;
			}
			KAnim.PlayMode playMode = this.animController.PlayMode;
			if (text != null)
			{
				if (text.Contains("hit"))
				{
					return;
				}
				if (text.Contains("2_0"))
				{
					return;
				}
				if (text.Contains("2_1"))
				{
					return;
				}
				if (text.Contains("2_-1"))
				{
					return;
				}
				if (text.Contains("2_-2"))
				{
					return;
				}
				if (text.Contains("1_-1"))
				{
					return;
				}
				if (text.Contains("1_-2"))
				{
					return;
				}
				if (text.Contains("1_1"))
				{
					return;
				}
				if (text.Contains("1_2"))
				{
					return;
				}
				if (text.Contains("breathe_"))
				{
					return;
				}
				if (text.Contains("death_"))
				{
					return;
				}
			}
			string text2 = "hit";
			AttackChore.StatesInstance smi = base.gameObject.GetSMI<AttackChore.StatesInstance>();
			if (smi != null && smi.GetCurrentState() == smi.sm.attack)
			{
				text2 = smi.master.GetHitAnim();
			}
			if (this.worker.GetComponent<Navigator>().CurrentNavType == NavType.Ladder)
			{
				text2 = "hit_ladder";
			}
			else if (this.worker.GetComponent<Navigator>().CurrentNavType == NavType.Pole)
			{
				text2 = "hit_ladder";
			}
			this.animController.Play(text2, KAnim.PlayMode.Once, 1f, 0f);
			if (text != null)
			{
				this.animController.Queue(text, playMode, 1f, 0f);
			}
		}

		public void GoToProperHeathState()
		{
			switch (base.smi.health.State)
			{
			case Health.HealthState.Perfect:
				base.smi.GoTo(base.sm.healthy);
				break;
			case Health.HealthState.Scuffed:
				base.smi.GoTo(base.sm.wounded.light);
				break;
			case Health.HealthState.Injured:
				base.smi.GoTo(base.sm.wounded.medium);
				break;
			case Health.HealthState.Critical:
				base.smi.GoTo(base.sm.wounded.heavy);
				break;
			}
		}

		public bool ShouldExitInfirmary()
		{
			return this.health.State == Health.HealthState.Perfect;
		}

		public Health health;

		private Worker worker;
	}
}
