using System;
using Klei.AI;
using UnityEngine;

public class HugMinionReactable : Reactable
{
	public HugMinionReactable(GameObject gameObject)
		: base(gameObject, "HugMinionReactable", Db.Get().ChoreTypes.Hug, 1, 1, true, 1f, 0f, float.PositiveInfinity, 0f, ObjectLayer.Minion)
	{
	}

	public override bool InternalCanBegin(GameObject newReactor, Navigator.ActiveTransition transition)
	{
		if (this.reactor != null)
		{
			return false;
		}
		Navigator component = newReactor.GetComponent<Navigator>();
		return !(component == null) && component.IsMoving();
	}

	public override void Update(float dt)
	{
		this.gameObject.GetComponent<Facing>().SetFacing(this.reactor.GetComponent<Facing>().GetFacing());
	}

	protected override void InternalBegin()
	{
		KAnimControllerBase component = this.reactor.GetComponent<KAnimControllerBase>();
		component.AddAnimOverrides(Assets.GetAnim("anim_react_pip_kanim"), 0f);
		component.Play("hug_dupe_pre", KAnim.PlayMode.Once, 1f, 0f);
		component.Queue("hug_dupe_loop", KAnim.PlayMode.Once, 1f, 0f);
		component.Queue("hug_dupe_pst", KAnim.PlayMode.Once, 1f, 0f);
		component.onAnimComplete += this.Finish;
		this.gameObject.GetSMI<AnimInterruptMonitor.Instance>().PlayAnimSequence(new HashedString[] { "hug_dupe_pre", "hug_dupe_loop", "hug_dupe_pst" });
	}

	private void Finish(HashedString anim)
	{
		if (anim == "hug_dupe_pst")
		{
			if (this.reactor != null)
			{
				this.reactor.GetComponent<KAnimControllerBase>().onAnimComplete -= this.Finish;
				this.ApplyEffects();
			}
			else
			{
				DebugUtil.LogWarningArgs(new object[] { "HugMinionReactable finishing without adding a Hugged effect." });
			}
			base.End();
		}
	}

	private void ApplyEffects()
	{
		this.reactor.GetComponent<Effects>().Add("Hugged", true);
		HugMonitor.Instance smi = this.gameObject.GetSMI<HugMonitor.Instance>();
		if (smi != null)
		{
			smi.EnterHuggingFrenzy();
		}
	}

	protected override void InternalEnd()
	{
	}

	protected override void InternalCleanup()
	{
	}
}
