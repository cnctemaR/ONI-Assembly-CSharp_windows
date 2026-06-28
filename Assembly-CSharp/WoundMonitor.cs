using System;

public class WoundMonitor : GameStateMachine<WoundMonitor, WoundMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.healthy;
		this.root.ToggleAnims("anim_hits_kanim", 0f);
		this.healthy.Update(delegate(WoundMonitor.Instance smi)
		{
			if (smi.health.State != Health.HealthState.Perfect)
			{
				smi.GoTo(this.wounded);
			}
		});
		this.wounded.ToggleUrge(Db.Get().Urges.Heal).Enter(delegate(WoundMonitor.Instance smi)
		{
			switch (smi.health.State)
			{
			case Health.HealthState.Scuffed:
				smi.GoTo(this.wounded.light);
				break;
			case Health.HealthState.Injured:
				smi.GoTo(this.wounded.medium);
				break;
			case Health.HealthState.Critical:
				smi.GoTo(this.wounded.heavy);
				break;
			}
		}).EventHandler(GameHashes.Healed, delegate(WoundMonitor.Instance smi)
		{
			smi.GoToProperHeathState();
		})
			.EventHandler(GameHashes.TookDamage, delegate(WoundMonitor.Instance smi)
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
			base.smi.master.gameObject.Subscribe(-2121334874, new Action<object>(base.smi.OnTookDamage));
		}

		public void OnTookDamage(object data)
		{
			if (this.health.hitPoints != 0f)
			{
				this.PlayHitAnimation();
			}
		}

		private void PlayHitAnimation()
		{
			string name = this.animController.CurrentAnim.name;
			KAnim.PlayMode playMode = this.animController.PlayMode;
			if (name != null)
			{
				if (name.Contains("hit"))
				{
					return;
				}
				if (name.Contains("2_0"))
				{
					return;
				}
				if (name.Contains("2_1"))
				{
					return;
				}
				if (name.Contains("2_-1"))
				{
					return;
				}
				if (name.Contains("2_-2"))
				{
					return;
				}
				if (name.Contains("1_-1"))
				{
					return;
				}
				if (name.Contains("1_-2"))
				{
					return;
				}
				if (name.Contains("1_1"))
				{
					return;
				}
				if (name.Contains("1_2"))
				{
					return;
				}
				if (name.Contains("breathe_"))
				{
					return;
				}
			}
			string text = "hit";
			AttackChore.StatesInstance smi = base.gameObject.GetSMI<AttackChore.StatesInstance>();
			if (smi != null && smi.GetCurrentState() == smi.sm.attack)
			{
				text = smi.master.GetHitAnim();
			}
			if (this.worker.GetComponent<Navigator>().CurrentNavType == NavType.Ladder)
			{
				text = "hit_ladder";
			}
			this.animController.Play(text, KAnim.PlayMode.Once, 1f, 0f);
			if (name != null)
			{
				this.animController.Queue(name, playMode, 1f, 0f);
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
