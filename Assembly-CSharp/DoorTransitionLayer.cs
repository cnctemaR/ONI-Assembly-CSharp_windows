using System;
using System.Collections.Generic;
using UnityEngine;

public class DoorTransitionLayer : TransitionDriver.InterruptOverrideLayer
{
	public DoorTransitionLayer(Navigator navigator)
		: base(navigator)
	{
		KBoxCollider2D component = navigator.GetComponent<KBoxCollider2D>();
		this.checkCellAbove = component != null && component.size.y > 1f;
	}

	private bool AreAllDoorsOpen()
	{
		foreach (INavDoor navDoor in this.doors)
		{
			if (navDoor != null && !navDoor.IsOpen())
			{
				return false;
			}
		}
		return true;
	}

	protected override bool IsOverrideComplete()
	{
		return base.IsOverrideComplete() && this.AreAllDoorsOpen();
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		if (this.doors.Count > 0)
		{
			return;
		}
		int num = Grid.PosToCell(navigator);
		int num2 = Grid.OffsetCell(num, transition.x, transition.y);
		this.AddDoor(num2);
		if (navigator.CurrentNavType != NavType.Tube && this.checkCellAbove)
		{
			this.AddDoor(Grid.CellAbove(num2));
		}
		for (int i = 0; i < transition.navGridTransition.voidOffsets.Length; i++)
		{
			int num3 = Grid.OffsetCell(num, transition.navGridTransition.voidOffsets[i]);
			this.AddDoor(num3);
		}
		if (this.doors.Count == 0)
		{
			return;
		}
		if (!this.AreAllDoorsOpen())
		{
			base.BeginTransition(navigator, transition);
			transition.anim = navigator.NavGrid.GetIdleAnim(navigator.CurrentNavType);
			transition.start = this.originalTransition.start;
			transition.end = this.originalTransition.start;
		}
		foreach (INavDoor navDoor in this.doors)
		{
			navDoor.Open();
		}
	}

	public override void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.EndTransition(navigator, transition);
		if (this.doors.Count == 0)
		{
			return;
		}
		foreach (INavDoor navDoor in this.doors)
		{
			if (!navDoor.IsNullOrDestroyed())
			{
				navDoor.Close();
			}
		}
		this.doors.Clear();
	}

	private void AddDoor(int cell)
	{
		INavDoor door = this.GetDoor(cell);
		if (!door.IsNullOrDestroyed() && !this.doors.Contains(door))
		{
			this.doors.Add(door);
		}
	}

	private INavDoor GetDoor(int cell)
	{
		if (!Grid.HasDoor[cell])
		{
			return null;
		}
		GameObject gameObject = Grid.Objects[cell, 1];
		if (gameObject != null)
		{
			INavDoor navDoor = gameObject.GetComponent<INavDoor>();
			if (navDoor == null)
			{
				navDoor = gameObject.GetSMI<INavDoor>();
			}
			if (navDoor != null && navDoor.isSpawned)
			{
				return navDoor;
			}
		}
		return null;
	}

	private List<INavDoor> doors = new List<INavDoor>();

	private bool checkCellAbove;
}
