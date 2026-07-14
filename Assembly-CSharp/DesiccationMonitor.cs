using System;
using Klei.AI;
using UnityEngine;

public class DesiccationMonitor : GameStateMachine<DesiccationMonitor, DesiccationMonitor.Instance, IStateMachineTarget, DesiccationMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.wet;
		this.wet.Enter(delegate(DesiccationMonitor.Instance smi)
		{
			DesiccationMonitor.SetSpeedModifier(smi, 1f);
		}).UpdateTransition(this.dry, new Func<DesiccationMonitor.Instance, float, bool>(DesiccationMonitor.Dry), UpdateRate.SIM_200ms, false);
		this.dry.UpdateTransition(this.wet, new Func<DesiccationMonitor.Instance, float, bool>(DesiccationMonitor.NotDry), UpdateRate.SIM_1000ms, false).UpdateTransition(this.desiccating, new Func<DesiccationMonitor.Instance, float, bool>(DesiccationMonitor.IsCompletelyDry), UpdateRate.SIM_1000ms, false).ToggleTag(GameTags.Creatures.Dry)
			.Enter(delegate(DesiccationMonitor.Instance smi)
			{
				DesiccationMonitor.SetSpeedModifier(smi, 0.66f);
			});
		this.desiccating.Enter(delegate(DesiccationMonitor.Instance smi)
		{
			DesiccationMonitor.SetSpeedModifier(smi, 0.33f);
			smi.ApplySadLook();
		}).Exit(delegate(DesiccationMonitor.Instance smi)
		{
			smi.RemoveSadLook();
		}).UpdateTransition(this.wet, (DesiccationMonitor.Instance smi, float dt) => !DesiccationMonitor.IsCompletelyDry(smi, dt), UpdateRate.SIM_200ms, false)
			.ToggleStatusItem(Db.Get().CreatureStatusItems.Desiccation, (DesiccationMonitor.Instance smi) => smi)
			.ToggleTag(GameTags.Creatures.Dry)
			.Update(new Action<DesiccationMonitor.Instance, float>(DesiccationMonitor.CheckDying), UpdateRate.SIM_4000ms, false);
	}

	private static void CheckDying(DesiccationMonitor.Instance smi, float dt)
	{
		smi.health.Damage(smi.def.desiccationDamagePerSecond * dt);
		if (smi.health.IsDefeated())
		{
			smi.Trigger(1506153353, null);
		}
	}

	private static bool IsMoisturized(DesiccationMonitor.Instance smi, float moistureTreshold)
	{
		return smi.moisture.value > moistureTreshold;
	}

	private static bool NotDry(DesiccationMonitor.Instance smi, float _)
	{
		return DesiccationMonitor.IsMoisturized(smi, 30f);
	}

	private static bool Dry(DesiccationMonitor.Instance smi, float _)
	{
		return !DesiccationMonitor.IsMoisturized(smi, 30f);
	}

	private static bool IsCompletelyDry(DesiccationMonitor.Instance smi, float _)
	{
		return smi.moisture.value <= 0f;
	}

	private static void SetSpeedModifier(DesiccationMonitor.Instance smi, float amount)
	{
		smi.navigator.defaultSpeed = smi.originalSpeed * amount;
	}

	private GameStateMachine<DesiccationMonitor, DesiccationMonitor.Instance, IStateMachineTarget, DesiccationMonitor.Def>.State wet;

	private GameStateMachine<DesiccationMonitor, DesiccationMonitor.Instance, IStateMachineTarget, DesiccationMonitor.Def>.State dry;

	private GameStateMachine<DesiccationMonitor, DesiccationMonitor.Instance, IStateMachineTarget, DesiccationMonitor.Def>.State desiccating;

	public class Def : StateMachine.BaseDef
	{
		public float desiccationDamagePerSecond = 0.1f;
	}

	public new class Instance : GameStateMachine<DesiccationMonitor, DesiccationMonitor.Instance, IStateMachineTarget, DesiccationMonitor.Def>.GameInstance
	{
		public void ApplySadLook()
		{
			this.kbac.TintColour = new Color32
			{
				r = (byte)Mathf.Clamp((int)(this.kbac.TintColour.r - DesiccationMonitor.Instance.dryColorDiff.r), 0, 255),
				g = (byte)Mathf.Clamp((int)(this.kbac.TintColour.g - DesiccationMonitor.Instance.dryColorDiff.g), 0, 255),
				b = (byte)Mathf.Clamp((int)(this.kbac.TintColour.b - DesiccationMonitor.Instance.dryColorDiff.b), 0, 255),
				a = this.kbac.TintColour.a
			};
		}

		public void RemoveSadLook()
		{
			this.kbac.TintColour = new Color32
			{
				r = (byte)Mathf.Clamp((int)(this.kbac.TintColour.r + DesiccationMonitor.Instance.dryColorDiff.r), 0, 255),
				g = (byte)Mathf.Clamp((int)(this.kbac.TintColour.g + DesiccationMonitor.Instance.dryColorDiff.g), 0, 255),
				b = (byte)Mathf.Clamp((int)(this.kbac.TintColour.b + DesiccationMonitor.Instance.dryColorDiff.b), 0, 255),
				a = this.kbac.TintColour.a
			};
		}

		public float GetEstimatedTimeUntilDeath()
		{
			if (!base.smi.IsInsideState(base.smi.sm.desiccating))
			{
				return float.NaN;
			}
			return this.health.hitPoints / base.def.desiccationDamagePerSecond;
		}

		public bool IsDesiccating()
		{
			return base.smi.IsInsideState(base.smi.sm.desiccating);
		}

		public Instance(IStateMachineTarget master, DesiccationMonitor.Def def)
			: base(master, def)
		{
			this.moisture = Db.Get().Amounts.Moisture.Lookup(base.gameObject);
			this.health = master.GetComponent<Health>();
			this.navigator = base.smi.GetComponent<Navigator>();
			this.kbac = master.GetComponent<KBatchedAnimController>();
			this.originalSpeed = this.navigator.defaultSpeed;
		}

		public float originalSpeed;

		public Navigator navigator;

		public AmountInstance moisture;

		public Health health;

		private static readonly Color32 dryColorDiff = new Color32(45, 45, 45, 0);

		private KBatchedAnimController kbac;
	}
}
