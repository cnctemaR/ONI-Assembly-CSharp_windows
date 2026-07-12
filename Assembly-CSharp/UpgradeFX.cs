using System;
using UnityEngine;

public class UpgradeFX : GameStateMachine<UpgradeFX, UpgradeFX.Instance>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		base.Target(this.fx);
		this.root.PlayAnim("upgrade").OnAnimQueueComplete(null).ToggleReactable((UpgradeFX.Instance smi) => smi.CreateReactable())
			.Exit("DestroyFX", delegate(UpgradeFX.Instance smi)
			{
				smi.DestroyFX();
			});
	}

	public StateMachine<UpgradeFX, UpgradeFX.Instance, IStateMachineTarget, object>.TargetParameter fx;

	public new class Instance : GameStateMachine<UpgradeFX, UpgradeFX.Instance, IStateMachineTarget, object>.GameInstance
	{
		public Instance(IStateMachineTarget master, Vector3 offset)
			: base(master)
		{
			KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("upgrade_fx_kanim", master.gameObject.transform.GetPosition() + offset, master.gameObject.transform, true, Grid.SceneLayer.Front, false);
			base.sm.fx.Set(kbatchedAnimController.gameObject, base.smi, false);
		}

		public void DestroyFX()
		{
			Util.KDestroyGameObject(base.sm.fx.Get(base.smi));
		}

		public Reactable CreateReactable()
		{
			return new EmoteReactable(base.master.gameObject, "UpgradeFX", Db.Get().ChoreTypes.Emote, 15, 8, 0f, 20f, float.PositiveInfinity, 0f).SetEmote(Db.Get().Emotes.Minion.Cheer);
		}
	}
}
