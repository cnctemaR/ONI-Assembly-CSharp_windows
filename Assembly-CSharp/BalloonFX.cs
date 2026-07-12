using System;
using Database;
using UnityEngine;

public class BalloonFX : GameStateMachine<BalloonFX, BalloonFX.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		base.Target(this.fx);
		this.root.Exit("DestroyFX", delegate(BalloonFX.Instance smi)
		{
			smi.DestroyFX();
		});
	}

	public StateMachine<BalloonFX, BalloonFX.Instance, IStateMachineTarget, object>.TargetParameter fx;

	public KAnimFile defaultAnim = Assets.GetAnim("balloon_anim_kanim");

	private KAnimFile defaultBalloon = Assets.GetAnim("balloon_basic_red_kanim");

	private const string defaultAnimName = "balloon_anim_kanim";

	private const string balloonAnimName = "balloon_basic_red_kanim";

	private const string TARGET_SYMBOL_TO_OVERRIDE = "body";

	private const int TARGET_OVERRIDE_PRIORITY = 0;

	public new class Instance : GameStateMachine<BalloonFX, BalloonFX.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master)
			: base(master)
		{
			this.balloonAnimController = FXHelpers.CreateEffectOverride(new string[] { "balloon_anim_kanim", "balloon_basic_red_kanim" }, master.gameObject.transform.GetPosition() + new Vector3(0f, 0.3f, 1f), master.transform, true, Grid.SceneLayer.Creatures, false);
			base.sm.fx.Set(this.balloonAnimController.gameObject, base.smi, false);
			this.balloonAnimController.defaultAnim = "idle_default";
			master.GetComponent<KBatchedAnimController>().GetSynchronizer().Add(this.balloonAnimController.GetComponent<KBatchedAnimController>());
		}

		public void SetBalloonSymbolOverride(BalloonOverrideSymbol balloonOverride)
		{
			KAnimFile kanimFile = (balloonOverride.animFile.IsSome() ? balloonOverride.animFile.Unwrap() : base.smi.sm.defaultBalloon);
			this.balloonAnimController.SwapAnims(new KAnimFile[]
			{
				base.smi.sm.defaultAnim,
				kanimFile
			});
			SymbolOverrideController component = this.balloonAnimController.GetComponent<SymbolOverrideController>();
			if (this.currentBodyOverrideSymbol.IsSome())
			{
				component.RemoveSymbolOverride("body", 0);
			}
			if (balloonOverride.symbol.IsNone())
			{
				if (this.currentBodyOverrideSymbol.IsSome())
				{
					component.AddSymbolOverride("body", base.smi.sm.defaultAnim.GetData().build.GetSymbol("body"), 0);
				}
				this.balloonAnimController.SetBatchGroupOverride(HashedString.Invalid);
			}
			else
			{
				component.AddSymbolOverride("body", balloonOverride.symbol.Unwrap(), 0);
				this.balloonAnimController.SetBatchGroupOverride(kanimFile.batchTag);
			}
			this.currentBodyOverrideSymbol = balloonOverride;
		}

		public void DestroyFX()
		{
			Util.KDestroyGameObject(base.sm.fx.Get(base.smi));
		}

		private KBatchedAnimController balloonAnimController;

		private Option<BalloonOverrideSymbol> currentBodyOverrideSymbol;
	}
}
