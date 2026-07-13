using System;

public class FossilSculptureLightMonitor : GameStateMachine<FossilSculptureLightMonitor, FossilSculptureLightMonitor.Instance, IStateMachineTarget, FossilSculptureLightMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.noLit;
		this.noLit.TagTransition(GameTags.Operational, this.lit, false).EventHandler(GameHashes.WorkableCompleteWork, new StateMachine<FossilSculptureLightMonitor, FossilSculptureLightMonitor.Instance, IStateMachineTarget, FossilSculptureLightMonitor.Def>.State.Callback(FossilSculptureLightMonitor.HideLitEffect)).EventHandler(GameHashes.ArtableStateChanged, new StateMachine<FossilSculptureLightMonitor, FossilSculptureLightMonitor.Instance, IStateMachineTarget, FossilSculptureLightMonitor.Def>.State.Callback(FossilSculptureLightMonitor.HideLitEffect))
			.Enter(new StateMachine<FossilSculptureLightMonitor, FossilSculptureLightMonitor.Instance, IStateMachineTarget, FossilSculptureLightMonitor.Def>.State.Callback(FossilSculptureLightMonitor.HideLitEffect));
		this.lit.TagTransition(GameTags.Operational, this.noLit, true).EventHandler(GameHashes.WorkableCompleteWork, new StateMachine<FossilSculptureLightMonitor, FossilSculptureLightMonitor.Instance, IStateMachineTarget, FossilSculptureLightMonitor.Def>.State.Callback(FossilSculptureLightMonitor.ShowLitEffect)).EventHandler(GameHashes.ArtableStateChanged, new StateMachine<FossilSculptureLightMonitor, FossilSculptureLightMonitor.Instance, IStateMachineTarget, FossilSculptureLightMonitor.Def>.State.Callback(FossilSculptureLightMonitor.ShowLitEffect))
			.Enter(new StateMachine<FossilSculptureLightMonitor, FossilSculptureLightMonitor.Instance, IStateMachineTarget, FossilSculptureLightMonitor.Def>.State.Callback(FossilSculptureLightMonitor.ShowLitEffect));
	}

	public static void ShowLitEffect(FossilSculptureLightMonitor.Instance smi)
	{
		smi.SetAnimLitState(true);
	}

	public static void HideLitEffect(FossilSculptureLightMonitor.Instance smi)
	{
		smi.SetAnimLitState(false);
	}

	public const string LIT_LIGHT_BLOOM_SYMBOL_NAME = "statue_light_bloom";

	public const string LIT_SHADING_SYMBOL_NAME = "shading_with_light";

	public const string UNLIT_SHADING_SYMBOL_NAME = "shading_no_light";

	public GameStateMachine<FossilSculptureLightMonitor, FossilSculptureLightMonitor.Instance, IStateMachineTarget, FossilSculptureLightMonitor.Def>.State noLit;

	public GameStateMachine<FossilSculptureLightMonitor, FossilSculptureLightMonitor.Instance, IStateMachineTarget, FossilSculptureLightMonitor.Def>.State lit;

	public class Def : StateMachine.BaseDef
	{
		public bool usingBloom = true;
	}

	public new class Instance : GameStateMachine<FossilSculptureLightMonitor, FossilSculptureLightMonitor.Instance, IStateMachineTarget, FossilSculptureLightMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, FossilSculptureLightMonitor.Def def)
			: base(master, def)
		{
			this.SetAnimLitState(false);
		}

		public void SetAnimLitState(bool lit)
		{
			KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
			component.SetSymbolVisiblity("statue_light_bloom", base.def.usingBloom && lit);
			component.SetSymbolVisiblity("shading_with_light", lit);
			component.SetSymbolVisiblity("shading_no_light", !lit);
		}
	}
}
