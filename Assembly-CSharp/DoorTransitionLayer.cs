using System;
using System.Collections.Generic;
using UnityEngine;

public class DoorTransitionLayer : TransitionDriver.OverrideLayer
{
	public DoorTransitionLayer(Navigator navigator)
		: base(navigator)
	{
	}

	public override void Destroy()
	{
		base.Destroy();
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
		int num = Grid.PosToCell(navigator);
		int num2 = Grid.OffsetCell(num, transition.x, transition.y);
		this.targetDoor = this.GetDoor(num2);
		if (this.targetDoor != null && !this.targetDoor.IsOpen())
		{
			transition.anim = navigator.NavGrid.GetIdleAnim(navigator.CurrentNavType);
			transition.isLooping = false;
			transition.end = transition.start;
			transition.speed = 1f;
			transition.animSpeed = 1f;
			transition.x = 0;
			transition.y = 0;
			transition.isCompleteCB = () => this.targetDoor == null || this.targetDoor.IsOpen();
		}
		this.AddDoor(num);
		if (this.targetDoor != null)
		{
			this.doors.Add(this.targetDoor);
		}
		foreach (Door door in this.doors)
		{
			door.Open();
		}
	}

	public override void UpdateTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.UpdateTransition(navigator, transition);
	}

	public override void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.EndTransition(navigator, transition);
		foreach (Door door in this.doors)
		{
			if (door != null)
			{
				door.Close();
			}
		}
		this.doors.Clear();
	}

	private void AddDoor(int cell)
	{
		Door door = this.GetDoor(cell);
		if (door != null)
		{
			this.doors.Add(door);
		}
	}

	private Door GetDoor(int cell)
	{
		GameObject gameObject = Grid.Objects[cell, 3];
		if (gameObject != null)
		{
			Door component = gameObject.GetComponent<Door>();
			if (component != null && component.isSpawned)
			{
				return component;
			}
		}
		return null;
	}

	private List<Door> doors = new List<Door>();

	private Door targetDoor;
}
