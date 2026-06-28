using System;
using UnityEngine;

public class WorkableReactable : Reactable
{
	public WorkableReactable(Workable workable, ChoreType chore_type, WorkableReactable.AllowedDirection allowed_direction = WorkableReactable.AllowedDirection.Any)
		: base(workable.gameObject, chore_type, 1, 1, false)
	{
		this.workable = workable;
		this.allowedDirection = allowed_direction;
	}

	public override bool InternalCanBegin(GameObject new_reactor, Navigator.ActiveTransition transition)
	{
		bool flag;
		if (this.workable == null)
		{
			flag = false;
		}
		else if (this.reactor != null)
		{
			flag = false;
		}
		else
		{
			Brain component = new_reactor.GetComponent<Brain>();
			if (component == null)
			{
				flag = false;
			}
			else if (!component.IsRunning())
			{
				flag = false;
			}
			else
			{
				Navigator component2 = new_reactor.GetComponent<Navigator>();
				if (component2 == null)
				{
					flag = false;
				}
				else if (!component2.IsMoving())
				{
					flag = false;
				}
				else if (this.allowedDirection == WorkableReactable.AllowedDirection.Any)
				{
					flag = true;
				}
				else
				{
					Facing component3 = new_reactor.GetComponent<Facing>();
					if (component3 == null)
					{
						flag = false;
					}
					else
					{
						bool facing = component3.GetFacing();
						flag = (!facing || this.allowedDirection != WorkableReactable.AllowedDirection.Right) && (facing || this.allowedDirection != WorkableReactable.AllowedDirection.Left);
					}
				}
			}
		}
		return flag;
	}

	protected override void InternalBegin()
	{
		this.worker = this.reactor.GetComponent<Worker>();
		this.worker.StartWork(new Worker.StartWorkInfo(this.workable));
	}

	public override void Update(float dt)
	{
		if (this.worker.Work())
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
