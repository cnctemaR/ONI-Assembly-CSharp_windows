using System;
using System.Collections.Generic;
using UnityEngine;

public class TransitionDriver
{
	public TransitionDriver(Navigator navigator)
	{
		this.log = new LoggerFS("TransitionDriver");
	}

	public void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		foreach (TransitionDriver.OverrideLayer overrideLayer in this.overrideLayers)
		{
			overrideLayer.BeginTransition(navigator, transition);
		}
		this.navigator = navigator;
		this.transition = transition;
		this.isComplete = false;
		Grid.SceneLayer sceneLayer = navigator.sceneLayer;
		if (transition.navGridTransition.start == NavType.Tube || transition.navGridTransition.end == NavType.Tube)
		{
			sceneLayer = Grid.SceneLayer.BuildingUse;
		}
		int num = Grid.PosToCell(navigator);
		int num2 = Grid.OffsetCell(num, transition.x, transition.y);
		this.targetPos = Grid.CellToPosCBC(num2, sceneLayer);
		if (transition.isLooping)
		{
			KAnimControllerBase component = navigator.GetComponent<KAnimControllerBase>();
			if (component.CurrentAnim == null || (component.CurrentAnim.name != transition.anim && component.CurrentAnim.name != transition.preAnim))
			{
				component.PlaySpeedMultiplier = transition.animSpeed;
				if (transition.preAnim != string.Empty)
				{
					component.Play(transition.preAnim, KAnim.PlayMode.Once, 1f, 0f);
					component.Queue(transition.anim, KAnim.PlayMode.Loop, 1f, 0f);
				}
				else
				{
					component.Play(transition.anim, KAnim.PlayMode.Loop, 1f, 0f);
				}
			}
		}
		else if (transition.anim != null)
		{
			KAnimControllerBase component2 = navigator.GetComponent<KAnimControllerBase>();
			component2.PlaySpeedMultiplier = transition.animSpeed;
			component2.Play(transition.anim, KAnim.PlayMode.Once, 1f, 0f);
			navigator.Subscribe(-1061186183, new Action<object>(this.OnAnimComplete));
		}
		navigator.GetComponent<Facing>().Face(this.targetPos.x);
		this.brain = navigator.GetComponent<Brain>();
	}

	public void UpdateTransition(float dt)
	{
		if (this.navigator == null)
		{
			return;
		}
		foreach (TransitionDriver.OverrideLayer overrideLayer in this.overrideLayers)
		{
			overrideLayer.UpdateTransition(this.navigator, this.transition);
		}
		if (!this.isComplete && this.transition.isCompleteCB != null)
		{
			this.isComplete = this.transition.isCompleteCB();
		}
		if (!(this.brain != null) || this.isComplete)
		{
		}
		if (this.transition.isLooping)
		{
			float speed = this.transition.speed;
			Vector3 position = this.navigator.transform.GetPosition();
			if (this.transition.x > 0)
			{
				position.x += dt * speed;
				if (position.x > this.targetPos.x)
				{
					this.isComplete = true;
				}
			}
			else if (this.transition.x < 0)
			{
				position.x -= dt * speed;
				if (position.x < this.targetPos.x)
				{
					this.isComplete = true;
				}
			}
			else
			{
				position.x = this.targetPos.x;
			}
			if (this.transition.y > 0)
			{
				position.y += dt * speed;
				if (position.y > this.targetPos.y)
				{
					this.isComplete = true;
				}
			}
			else if (this.transition.y < 0)
			{
				position.y -= dt * speed;
				if (position.y < this.targetPos.y)
				{
					this.isComplete = true;
				}
			}
			else
			{
				position.y = this.targetPos.y;
			}
			this.navigator.transform.SetPosition(position);
		}
		if (this.isComplete)
		{
			this.isComplete = false;
			Navigator navigator = this.navigator;
			navigator.SetCurrentNavType(this.transition.end);
			navigator.transform.SetPosition(this.targetPos);
			this.EndTransition();
			navigator.AdvancePath(true);
		}
	}

	private void OnAnimComplete(object data)
	{
		if (this.navigator != null)
		{
			this.navigator.Unsubscribe(-1061186183, new Action<object>(this.OnAnimComplete));
		}
		this.isComplete = true;
	}

	public void EndTransition()
	{
		if (this.navigator != null)
		{
			Navigator navigator = this.navigator;
			foreach (TransitionDriver.OverrideLayer overrideLayer in this.overrideLayers)
			{
				overrideLayer.EndTransition(this.navigator, this.transition);
			}
			this.navigator = null;
			navigator.GetComponent<KAnimControllerBase>().PlaySpeedMultiplier = 1f;
			navigator.Unsubscribe(-1061186183, new Action<object>(this.OnAnimComplete));
			Brain component = navigator.GetComponent<Brain>();
			if (component != null)
			{
				component.Resume("move_handler");
			}
		}
	}

	private Navigator.ActiveTransition transition;

	private Navigator navigator;

	private Vector3 targetPos;

	private bool isComplete;

	private Brain brain;

	public List<TransitionDriver.OverrideLayer> overrideLayers = new List<TransitionDriver.OverrideLayer>();

	private LoggerFS log;

	public class OverrideLayer
	{
		public OverrideLayer(Navigator navigator)
		{
		}

		public virtual void Destroy()
		{
		}

		public virtual void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
		{
		}

		public virtual void UpdateTransition(Navigator navigator, Navigator.ActiveTransition transition)
		{
		}

		public virtual void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
		{
		}
	}
}
