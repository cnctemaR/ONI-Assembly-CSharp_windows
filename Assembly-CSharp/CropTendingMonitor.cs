using System;
using UnityEngine;

public class CropTendingMonitor : GameStateMachine<CropTendingMonitor, CropTendingMonitor.Instance, IStateMachineTarget, CropTendingMonitor.Def>
{
	private bool InterestedInTendingCrops(CropTendingMonitor.Instance smi)
	{
		return !smi.HasTag(GameTags.Creatures.Hungry) || global::UnityEngine.Random.value <= smi.def.unsatisfiedTendChance;
	}

	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.cooldown;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.cooldown.ParamTransition<float>(this.cooldownTimer, this.lookingForCrop, (CropTendingMonitor.Instance smi, float p) => this.cooldownTimer.Get(smi) <= 0f && this.InterestedInTendingCrops(smi)).ParamTransition<float>(this.cooldownTimer, this.reset, (CropTendingMonitor.Instance smi, float p) => this.cooldownTimer.Get(smi) <= 0f && !this.InterestedInTendingCrops(smi)).Update(delegate(CropTendingMonitor.Instance smi, float dt)
		{
			this.cooldownTimer.Delta(-dt, smi);
		}, UpdateRate.SIM_1000ms, false);
		this.lookingForCrop.ToggleBehaviour(GameTags.Creatures.WantsToTendCrops, (CropTendingMonitor.Instance smi) => true, delegate(CropTendingMonitor.Instance smi)
		{
			smi.GoTo(this.reset);
		});
		this.reset.Exit(delegate(CropTendingMonitor.Instance smi)
		{
			this.cooldownTimer.Set(600f / smi.def.numCropsTendedPerCycle, smi, false);
		}).GoTo(this.cooldown);
	}

	private StateMachine<CropTendingMonitor, CropTendingMonitor.Instance, IStateMachineTarget, CropTendingMonitor.Def>.FloatParameter cooldownTimer;

	private GameStateMachine<CropTendingMonitor, CropTendingMonitor.Instance, IStateMachineTarget, CropTendingMonitor.Def>.State cooldown;

	private GameStateMachine<CropTendingMonitor, CropTendingMonitor.Instance, IStateMachineTarget, CropTendingMonitor.Def>.State lookingForCrop;

	private GameStateMachine<CropTendingMonitor, CropTendingMonitor.Instance, IStateMachineTarget, CropTendingMonitor.Def>.State reset;

	public class Def : StateMachine.BaseDef
	{
		public float numCropsTendedPerCycle = 8f;

		public float unsatisfiedTendChance = 0.5f;
	}

	public new class Instance : GameStateMachine<CropTendingMonitor, CropTendingMonitor.Instance, IStateMachineTarget, CropTendingMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, CropTendingMonitor.Def def)
			: base(master, def)
		{
		}
	}
}
