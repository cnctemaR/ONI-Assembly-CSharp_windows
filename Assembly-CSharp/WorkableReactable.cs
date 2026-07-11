using System;
using UnityEngine;

public class WorkableReactable : Reactable
{
	public WorkableReactable(Workable workable, HashedString id, ChoreType chore_type, WorkableReactable.AllowedDirection allowed_direction = WorkableReactable.AllowedDirection.Any)
		: base(workable.gameObject, id, chore_type, 1, 1, false, 0f, 0f, float.PositiveInfinity)
	{
		this.workable = workable;
		this.allowedDirection = allowed_direction;
	}

	public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
	{
		if (this.workable == null)
		{
			return false;
		}
		if (this.reactor != null)
		{
			return false;
		}
		Brain component = new_reactor.GetComponent<Brain>();
		if (component == null)
		{
			return false;
		}
		if (!component.IsRunning())
		{
			return false;
		}
		Navigator component2 = new_reactor.GetComponent<Navigator>();
		if (component2 == null)
		{
			return false;
		}
		if (!component2.IsMoving())
		{
			return false;
		}
		if (this.allowedDirection == WorkableReactable.AllowedDirection.Any)
		{
			return true;
		}
		Facing component3 = new_reactor.GetComponent<Facing>();
		if (component3 == null)
		{
			return false;
		}
		bool facing = component3.GetFacing();
		return (!facing || this.allowedDirection != WorkableReactable.AllowedDirection.Right) && (facing || this.allowedDirection != WorkableReactable.AllowedDirection.Left);
	}

	protected override void InternalBegin()
	{
		this.worker = this.reactor.GetComponent<Worker>();
		this.worker.StartWork(new Worker.StartWorkInfo(this.workable));
	}

	public override void Update(float dt)
	{
		if (this.worker.Work(dt))
		{
			this.worker.CompleteWork();
			base.End();
		}
	}

	protected override void InternalEnd()
	{
		if (this.worker != null)
		{
			this.worker.StopWork();
		}
	}

	protected override void InternalCleanup()
	{
	}

	protected Workable workable;

	private Worker worker;

	public WorkableReactable.AllowedDirection allowedDirection;

	public enum AllowedDirection
	{
		Any,
		Left,
		Right
	}
}
