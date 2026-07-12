using System;
using Klei.AI;
using STRINGS;
using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
public class GlowStick : StateMachineComponent<GlowStick.StatesInstance>
{
	protected override void OnSpawn()
	{
		base.smi.StartSM();
	}

	public class StatesInstance : GameStateMachine<GlowStick.States, GlowStick.StatesInstance, GlowStick, object>.GameInstance
	{
		public StatesInstance(GlowStick master)
			: base(master)
		{
			this._light2D.Color = Color.green;
			this._light2D.Range = 2f;
			this._light2D.Angle = 0f;
			this._light2D.Direction = LIGHT2D.DEFAULT_DIRECTION;
			this._light2D.Offset = new Vector2(0.05f, 0.5f);
			this._light2D.shape = global::LightShape.Circle;
			this._light2D.Lux = 500;
			this._radiationEmitter.emitRads = 120f;
			this._radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
			this._radiationEmitter.emitRate = 0.5f;
			this._radiationEmitter.emitRadiusX = 3;
			this._radiationEmitter.emitRadiusY = 3;
			this.radiationResistance = new AttributeModifier(Db.Get().Attributes.RadiationResistance.Id, TRAITS.GLOWSTICK_RADIATION_RESISTANCE, DUPLICANTS.TRAITS.GLOWSTICK.NAME, false, false, true);
		}

		[MyCmpAdd]
		private RadiationEmitter _radiationEmitter;

		[MyCmpAdd]
		private Light2D _light2D;

		public AttributeModifier radiationResistance;
	}

	public class States : GameStateMachine<GlowStick.States, GlowStick.StatesInstance, GlowStick>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			default_state = this.root;
			this.root.ToggleComponent<RadiationEmitter>(false).ToggleComponent<Light2D>(false).ToggleAttributeModifier("Radiation Resistance", (GlowStick.StatesInstance smi) => smi.radiationResistance, null);
		}
	}
}
