using System;
using UnityEngine;

public class CreatureLightToggleController : GameStateMachine<CreatureLightToggleController, CreatureLightToggleController.Instance, IStateMachineTarget, CreatureLightToggleController.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.light_on;
		base.serializable = true;
		this.light_off.Enter(delegate(CreatureLightToggleController.Instance smi)
		{
			smi.SwitchLight(false);
		}).TagTransition(GameTags.Creatures.Overcrowded, this.turning_on, true);
		this.turning_off.Update(delegate(CreatureLightToggleController.Instance smi, float dt)
		{
			smi.Dim(dt);
		}, UpdateRate.SIM_200ms, false).Transition(this.light_off, (CreatureLightToggleController.Instance smi) => smi.IsOff(), UpdateRate.SIM_200ms);
		this.light_on.Enter(delegate(CreatureLightToggleController.Instance smi)
		{
			smi.SwitchLight(true);
		}).TagTransition(GameTags.Creatures.Overcrowded, this.turning_off, false);
		this.turning_on.Enter(delegate(CreatureLightToggleController.Instance smi)
		{
			smi.SwitchLight(true);
		}).Update(delegate(CreatureLightToggleController.Instance smi, float dt)
		{
			smi.Brighten(dt);
		}, UpdateRate.SIM_200ms, false).Transition(this.light_on, (CreatureLightToggleController.Instance smi) => smi.IsOn(), UpdateRate.SIM_200ms);
	}

	private GameStateMachine<CreatureLightToggleController, CreatureLightToggleController.Instance, IStateMachineTarget, CreatureLightToggleController.Def>.State light_off;

	private GameStateMachine<CreatureLightToggleController, CreatureLightToggleController.Instance, IStateMachineTarget, CreatureLightToggleController.Def>.State turning_off;

	private GameStateMachine<CreatureLightToggleController, CreatureLightToggleController.Instance, IStateMachineTarget, CreatureLightToggleController.Def>.State light_on;

	private GameStateMachine<CreatureLightToggleController, CreatureLightToggleController.Instance, IStateMachineTarget, CreatureLightToggleController.Def>.State turning_on;

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<CreatureLightToggleController, CreatureLightToggleController.Instance, IStateMachineTarget, CreatureLightToggleController.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, CreatureLightToggleController.Def def)
			: base(master, def)
		{
			this.light = master.GetComponent<Light2D>();
			this.originalLux = this.light.Lux;
			this.originalRange = this.light.Range;
		}

		public void SwitchLight(bool on)
		{
			this.light.enabled = on;
		}

		public void Dim(float dt)
		{
			float num = (float)this.originalLux / 25f;
			this.light.Lux = Mathf.FloorToInt(Mathf.Max(0f, (float)this.light.Lux - num * dt));
			this.light.Range = this.originalRange * (float)this.light.Lux / (float)this.originalLux;
			this.light.Refresh();
		}

		public void Brighten(float dt)
		{
			float num = (float)this.originalLux / 15f;
			this.light.Lux = Mathf.CeilToInt(Mathf.Min((float)this.originalLux, (float)this.light.Lux + num * dt));
			this.light.Range = this.originalRange * (float)this.light.Lux / (float)this.originalLux;
			this.light.Refresh();
		}

		public bool IsOff()
		{
			return this.light.Lux == 0;
		}

		public bool IsOn()
		{
			return this.light.Lux >= this.originalLux;
		}

		private const float DIM_TIME = 25f;

		private const float GLOW_TIME = 15f;

		private int originalLux;

		private float originalRange;

		private Light2D light;
	}
}
