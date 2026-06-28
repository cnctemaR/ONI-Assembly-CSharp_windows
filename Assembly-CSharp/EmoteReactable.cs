using System;
using System.Collections.Generic;
using UnityEngine;

public class EmoteReactable : Reactable
{
	public EmoteReactable(GameObject gameObject, ChoreType chore_type, HashedString animset, int range_width = 15, int range_height = 8)
		: base(gameObject, chore_type, range_width, range_height, true)
	{
		this.reactionSource = gameObject;
		this.animset = Assets.GetAnim(animset);
	}

	public EmoteReactable AddStep(EmoteReactable.EmoteStep step)
	{
		this.emoteSteps.Add(step);
		return this;
	}

	public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
	{
		bool flag;
		if (new_reactor == null)
		{
			flag = false;
		}
		else
		{
			Navigator component = new_reactor.GetComponent<Navigator>();
			flag = !(component == null) && component.IsMoving() && this.reactionSource != new_reactor;
		}
		return flag;
	}

	public override void Update(float dt)
	{
		if (this.currentStep >= 0 && this.emoteSteps[this.currentStep].timeout > 0f && this.emoteSteps[this.currentStep].timeout < this.elapsed)
		{
			this.NextStep(null);
		}
		else
		{
			this.elapsed += dt;
		}
	}

	protected override void InternalBegin()
	{
		this.kbac = this.reactor.GetComponent<KBatchedAnimController>();
		this.kbac.AddAnimOverrides(this.animset, 0f);
		this.NextStep(null);
	}

	protected override void InternalEnd()
	{
		if (this.currentStep >= 0 && this.currentStep < this.emoteSteps.Count && this.emoteSteps[this.currentStep].timeout <= 0f)
		{
			this.kbac.onAnimComplete -= this.NextStep;
		}
		this.kbac.RemoveAnimOverrides(this.animset);
		this.kbac = null;
		this.currentStep = -1;
	}

	protected override void InternalCleanup()
	{
	}

	private void NextStep(HashedString finishedAnim)
	{
		if (this.currentStep >= 0 && this.emoteSteps[this.currentStep].timeout <= 0f)
		{
			this.kbac.onAnimComplete -= this.NextStep;
			if (this.emoteSteps[this.currentStep].finishcb != null)
			{
				this.emoteSteps[this.currentStep].finishcb(this.reactor);
			}
		}
		this.currentStep++;
		if (this.currentStep >= this.emoteSteps.Count)
		{
			base.End();
		}
		else
		{
			if (this.emoteSteps[this.currentStep].anim != HashedString.Invalid)
			{
				this.kbac.Play(this.emoteSteps[this.currentStep].anim, this.emoteSteps[this.currentStep].mode, 1f, 0f);
			}
			if (this.emoteSteps[this.currentStep].timeout <= 0f)
			{
				this.kbac.onAnimComplete += this.NextStep;
			}
			else
			{
				this.elapsed = 0f;
			}
			if (this.emoteSteps[this.currentStep].startcb != null)
			{
				this.emoteSteps[this.currentStep].startcb(this.reactor);
			}
		}
	}

	protected GameObject reactionSource;

	private KBatchedAnimController kbac;

	public Expression expression = Db.Get().Expressions.Uncomfortable;

	public Thought thought = Db.Get().Thoughts.Unhappy;

	private KAnimFile animset;

	private List<EmoteReactable.EmoteStep> emoteSteps = new List<EmoteReactable.EmoteStep>();

	private int currentStep = -1;

	private float elapsed = 0f;

	public class EmoteStep
	{
		public HashedString anim = HashedString.Invalid;

		public KAnim.PlayMode mode = KAnim.PlayMode.Once;

		public float timeout = -1f;

		public Action<GameObject> startcb;

		public Action<GameObject> finishcb;
	}
}
