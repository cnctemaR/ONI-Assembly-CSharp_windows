using System;
using System.Collections.Generic;
using UnityEngine;

public class EmoteReactable : Reactable
{
	public EmoteReactable(GameObject gameObject, HashedString id, ChoreType chore_type, HashedString animset, int range_width = 15, int range_height = 8, float min_reactable_time = 0f, float min_reactor_time = 20f, float max_trigger_time = float.PositiveInfinity)
		: base(gameObject, id, chore_type, range_width, range_height, true, min_reactable_time, min_reactor_time, max_trigger_time)
	{
		this.animset = Assets.GetAnim(animset);
	}

	public EmoteReactable AddStep(EmoteReactable.EmoteStep step)
	{
		this.emoteSteps.Add(step);
		return this;
	}

	public EmoteReactable AddExpression(Expression expression)
	{
		this.expression = expression;
		return this;
	}

	public EmoteReactable AddThought(Thought thought)
	{
		this.thought = thought;
		return this;
	}

	public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
	{
		if (this.reactor != null)
		{
			return false;
		}
		if (new_reactor == null)
		{
			return false;
		}
		Navigator component = new_reactor.GetComponent<Navigator>();
		return !(component == null) && component.IsMoving() && component.CurrentNavType != NavType.Tube && component.CurrentNavType != NavType.Ladder && component.CurrentNavType != NavType.Pole && this.gameObject != new_reactor;
	}

	public override void Update(float dt)
	{
		if (this.gameObject != null && this.reactor != null)
		{
			Facing component = this.reactor.GetComponent<Facing>();
			if (component != null)
			{
				component.Face(this.gameObject.transform.GetPosition());
			}
		}
		if (this.currentStep >= 0 && this.emoteSteps[this.currentStep].timeout > 0f && this.emoteSteps[this.currentStep].timeout < this.elapsed)
		{
			this.NextStep(null);
			return;
		}
		this.elapsed += dt;
	}

	protected override void InternalBegin()
	{
		this.kbac = this.reactor.GetComponent<KBatchedAnimController>();
		this.kbac.AddAnimOverrides(this.animset, 0f);
		if (this.expression != null)
		{
			this.reactor.GetComponent<FaceGraph>().AddExpression(this.expression);
		}
		if (this.thought != null)
		{
			this.reactor.GetSMI<ThoughtGraph.Instance>().AddThought(this.thought);
		}
		this.NextStep(null);
	}

	protected override void InternalEnd()
	{
		if (this.kbac != null)
		{
			if (this.currentStep >= 0 && this.currentStep < this.emoteSteps.Count && this.emoteSteps[this.currentStep].timeout <= 0f)
			{
				this.kbac.onAnimComplete -= this.NextStep;
			}
			this.kbac.RemoveAnimOverrides(this.animset);
			this.kbac = null;
		}
		if (this.reactor != null)
		{
			if (this.expression != null)
			{
				this.reactor.GetComponent<FaceGraph>().RemoveExpression(this.expression);
			}
			if (this.thought != null)
			{
				this.reactor.GetSMI<ThoughtGraph.Instance>().RemoveThought(this.thought);
			}
		}
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
		if (this.currentStep >= this.emoteSteps.Count || this.kbac == null)
		{
			base.End();
			return;
		}
		if (this.emoteSteps[this.currentStep].anim != HashedString.Invalid)
		{
			this.kbac.Play(this.emoteSteps[this.currentStep].anim, this.emoteSteps[this.currentStep].mode, 1f, 0f);
			if (this.kbac.IsStopped())
			{
				DebugUtil.DevAssertArgs(false, new object[]
				{
					"Emote is missing anim:",
					this.emoteSteps[this.currentStep].anim
				});
				this.emoteSteps[this.currentStep].timeout = 0.25f;
			}
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

	private KBatchedAnimController kbac;

	public Expression expression;

	public Thought thought;

	private KAnimFile animset;

	private List<EmoteReactable.EmoteStep> emoteSteps = new List<EmoteReactable.EmoteStep>();

	private int currentStep = -1;

	private float elapsed;

	public class EmoteStep
	{
		public HashedString anim = HashedString.Invalid;

		public KAnim.PlayMode mode = KAnim.PlayMode.Once;

		public float timeout = -1f;

		public Action<GameObject> startcb;

		public Action<GameObject> finishcb;
	}
}
