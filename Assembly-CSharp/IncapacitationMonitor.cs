using System;
using Klei.AI;
using UnityEngine;

public class IncapacitationMonitor : GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.healthy;
		base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
		this.healthy.Update(delegate(IncapacitationMonitor.Instance smi, float dt)
		{
			smi.RecoverBleedOutStamina(dt, smi);
		}, UpdateRate.SIM_200ms, false).EventTransition(GameHashes.BecameIncapacitated, this.incapacitated, null);
		this.incapacitated.EventTransition(GameHashes.IncapacitationRecovery, this.recovered, null).ToggleTag(GameTags.Incapacitated).ToggleRecurringChore((IncapacitationMonitor.Instance smi) => new BeIncapacitatedChore(smi.master), null)
			.ParamTransition<float>(this.bleedOutStamina, this.die, GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.IsLTEZero)
			.ToggleUrge(Db.Get().Urges.BeIncapacitated)
			.Update(delegate(IncapacitationMonitor.Instance smi, float dt)
			{
				smi.Bleed(dt, smi);
			}, UpdateRate.SIM_200ms, false);
		this.recovered.Enter(delegate(IncapacitationMonitor.Instance smi)
		{
			smi.ApplyRecoverEffect();
		}).GoTo(this.healthy);
		this.die.Enter(delegate(IncapacitationMonitor.Instance smi)
		{
			smi.master.gameObject.GetSMI<DeathMonitor.Instance>().Kill(smi.GetCauseOfIncapacitation());
		});
	}

	public GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.State healthy;

	public GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.State recovered;

	public GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.State incapacitated;

	public GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.State die;

	private StateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.FloatParameter bleedOutStamina = new StateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.FloatParameter(120f);

	private StateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.FloatParameter baseBleedOutSpeed = new StateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.FloatParameter(1f);

	private StateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.FloatParameter baseStaminaRecoverSpeed = new StateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.FloatParameter(1f);

	private StateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.FloatParameter maxBleedOutStamina = new StateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.FloatParameter(120f);

	public new class Instance : GameStateMachine<IncapacitationMonitor, IncapacitationMonitor.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			Health component = master.GetComponent<Health>();
			if (component)
			{
				component.canBeIncapacitated = true;
			}
		}

		public void Bleed(float dt, IncapacitationMonitor.Instance smi)
		{
			smi.sm.bleedOutStamina.Delta(dt * -smi.sm.baseBleedOutSpeed.Get(smi), smi);
		}

		public void RecoverBleedOutStamina(float dt, IncapacitationMonitor.Instance smi)
		{
			smi.sm.bleedOutStamina.Delta(Mathf.Min(dt * smi.sm.baseStaminaRecoverSpeed.Get(smi), smi.sm.maxBleedOutStamina.Get(smi) - smi.sm.bleedOutStamina.Get(smi)), smi);
		}

		public float GetBleedLifeTime(IncapacitationMonitor.Instance smi)
		{
			return Mathf.Floor(smi.sm.bleedOutStamina.Get(smi) / smi.sm.baseBleedOutSpeed.Get(smi));
		}

		public Death GetCauseOfIncapacitation()
		{
			Health component = base.GetComponent<Health>();
			if (component.CauseOfIncapacitation == GameTags.RadiationSicknessIncapacitation)
			{
				return Db.Get().Deaths.Radiation;
			}
			if (component.CauseOfIncapacitation == GameTags.HitPointsDepleted)
			{
				return Db.Get().Deaths.Slain;
			}
			return Db.Get().Deaths.Generic;
		}

		public void ApplyRecoverEffect()
		{
			base.smi.Get<Effects>().Add("NearDeathExperience", true);
		}
	}
}
