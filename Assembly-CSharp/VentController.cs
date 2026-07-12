using System;
using UnityEngine;

public class VentController : GameStateMachine<VentController, VentController.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.off;
		this.root.EventHandler(GameHashes.VentAnimatingChanged, new GameStateMachine<VentController, VentController.Instance, IStateMachineTarget, object>.GameEvent.Callback(VentController.UpdateMeterColor)).EventTransition(GameHashes.VentClosed, this.closed, (VentController.Instance smi) => smi.GetComponent<Vent>().Closed()).EventTransition(GameHashes.VentOpen, this.off, (VentController.Instance smi) => !smi.GetComponent<Vent>().Closed());
		this.off.PlayAnim("off").EventTransition(GameHashes.VentAnimatingChanged, this.working_pre, new StateMachine<VentController, VentController.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(VentController.IsAnimating));
		this.working_pre.PlayAnim("working_pre").OnAnimQueueComplete(this.working_loop);
		this.working_loop.PlayAnim("working_loop", KAnim.PlayMode.Loop).Enter(new StateMachine<VentController, VentController.Instance, IStateMachineTarget, object>.State.Callback(VentController.PlayOutputMeterAnim)).EventTransition(GameHashes.VentAnimatingChanged, this.working_pst, GameStateMachine<VentController, VentController.Instance, IStateMachineTarget, object>.Not(new StateMachine<VentController, VentController.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(VentController.IsAnimating)));
		this.working_pst.PlayAnim("working_pst").OnAnimQueueComplete(this.off);
		this.closed.PlayAnim("closed").EventTransition(GameHashes.VentAnimatingChanged, this.working_pre, new StateMachine<VentController, VentController.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(VentController.IsAnimating));
	}

	public static void PlayOutputMeterAnim(VentController.Instance smi)
	{
		smi.PlayMeterAnim();
	}

	public static bool IsAnimating(VentController.Instance smi)
	{
		return smi.exhaust.IsAnimating();
	}

	public static void UpdateMeterColor(VentController.Instance smi, object data)
	{
		if (data != null)
		{
			Color32 color = (Color32)data;
			smi.SetMeterOutputColor(color);
		}
	}

	public GameStateMachine<VentController, VentController.Instance, IStateMachineTarget, object>.State off;

	public GameStateMachine<VentController, VentController.Instance, IStateMachineTarget, object>.State working_pre;

	public GameStateMachine<VentController, VentController.Instance, IStateMachineTarget, object>.State working_loop;

	public GameStateMachine<VentController, VentController.Instance, IStateMachineTarget, object>.State working_pst;

	public GameStateMachine<VentController, VentController.Instance, IStateMachineTarget, object>.State closed;

	public StateMachine<VentController, VentController.Instance, IStateMachineTarget, object>.BoolParameter isAnimating;

	public class Def : StateMachine.BaseDef
	{
		public bool usingDynamicColor;

		public string outputSubstanceAnimName;
	}

	public new class Instance : GameStateMachine<VentController, VentController.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master, VentController.Def def)
			: base(master, def)
		{
			if (def.usingDynamicColor)
			{
				this.outputSubstanceMeter = new MeterController(this.anim, "meter_target", def.outputSubstanceAnimName, Meter.Offset.NoChange, Grid.SceneLayer.Building, Array.Empty<string>());
			}
		}

		public void PlayMeterAnim()
		{
			if (this.outputSubstanceMeter != null)
			{
				this.outputSubstanceMeter.meterController.Play(this.outputSubstanceMeter.meterController.initialAnim, KAnim.PlayMode.Loop, 1f, 0f);
			}
		}

		public void SetMeterOutputColor(Color32 color)
		{
			if (this.outputSubstanceMeter != null)
			{
				this.outputSubstanceMeter.meterController.TintColour = color;
			}
		}

		[MyCmpGet]
		private KBatchedAnimController anim;

		[MyCmpGet]
		public Exhaust exhaust;

		private MeterController outputSubstanceMeter;
	}
}
