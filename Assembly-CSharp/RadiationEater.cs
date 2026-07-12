using System;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
public class RadiationEater : StateMachineComponent<RadiationEater.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	public class StatesInstance : GameStateMachine<RadiationEater.States, RadiationEater.StatesInstance, RadiationEater, object>.GameInstance
	{
		public StatesInstance(RadiationEater master)
			: base(master)
		{
			this.radiationEating = new AttributeModifier(Db.Get().Attributes.RadiationRecovery.Id, TRAITS.RADIATION_EATER_RECOVERY, DUPLICANTS.TRAITS.RADIATIONEATER.NAME, false, false, true);
		}

		public void OnEatRads(float radsEaten)
		{
			float num = Mathf.Abs(radsEaten) * TRAITS.RADS_TO_CALS;
			base.smi.master.gameObject.GetAmounts().Get(Db.Get().Amounts.Calories).ApplyDelta(num);
		}

		public AttributeModifier radiationEating;
	}

	public class States : GameStateMachine<RadiationEater.States, RadiationEater.StatesInstance, RadiationEater>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.root;
			this.root.ToggleAttributeModifier("Radiation Eating", (RadiationEater.StatesInstance smi) => smi.radiationEating, null).EventHandler(GameHashes.RadiationRecovery, delegate(RadiationEater.StatesInstance smi, object data)
			{
				float num = (float)data;
				smi.OnEatRads(num);
			});
		}
	}
}
