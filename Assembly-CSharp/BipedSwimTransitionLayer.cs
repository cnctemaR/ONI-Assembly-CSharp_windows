using System;
using System.Collections.Generic;
using UnityEngine;

public class BipedSwimTransitionLayer : TransitionDriver.OverrideLayer
{
	public BipedSwimTransitionLayer(Navigator navigator)
		: base(navigator)
	{
		this.animcontroller = navigator.GetComponent<KBatchedAnimController>();
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
		this.lerpingOffset = false;
		int num = Grid.CellAbove(navigator.cachedCell);
		bool flag = Grid.IsWorldValidCell(num) && !Grid.IsLiquid(num);
		object obj = transition.start == NavType.Swim && transition.end == NavType.Swim;
		bool flag2 = transition.x != 0 && transition.y != 0;
		bool flag3 = transition.x != 0 && transition.y == 0;
		object obj2 = obj;
		Dictionary<HashedString, HashedString> dictionary;
		HashedString hashedString;
		if ((obj2 & flag3 & flag) != null)
		{
			transition.anim = "shallow_swim_1_0_loop";
			transition.isLooping = true;
			this.SetupOffsets(navigator, transition);
		}
		else if (this.animcontroller.currentAnim != transition.anim && SwimMonitor.transitionAnims.TryGetValue(this.animcontroller.currentAnim, out dictionary) && dictionary.TryGetValue(transition.anim, out hashedString))
		{
			transition.preAnim = hashedString;
		}
		if (obj2 != null && !transition.isLooping)
		{
			if (transition.speed > 0f)
			{
				transition.animSpeed = transition.speed;
			}
			if (flag2)
			{
				transition.animSpeed *= 0.9f;
			}
		}
	}

	public override void UpdateTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		if (transition.start == NavType.Swim && transition.end == NavType.Swim && this.lerpingOffset)
		{
			Vector3 position = navigator.transform.GetPosition();
			float num = Vector3.Distance(this.startPos, this.endPos);
			float num2 = Vector3.Distance(this.startPos, position);
			float num3 = ((num > 0f) ? Mathf.Clamp01(num2 / num) : 1f);
			this.offset.y = Mathf.Lerp(this.startOffsetY, this.targetOffsetY, num3);
			if (MathF.Abs(this.offset.y - this.animcontroller.Offset.y) > SwimMonitor.OffsetEpsilon)
			{
				this.animcontroller.Offset = this.offset;
			}
		}
		base.UpdateTransition(navigator, transition);
	}

	private void SetupOffsets(Navigator navigator, Navigator.ActiveTransition transition)
	{
		int cachedCell = navigator.cachedCell;
		int num = Grid.OffsetCell(cachedCell, transition.x, transition.y);
		this.startOffsetY = SwimMonitor.ComputeSwimOffsetY(cachedCell);
		this.targetOffsetY = (BipedSwimTransitionLayer.IsSurfaceSwimCell(num) ? SwimMonitor.ComputeSwimOffsetY(num) : 0f);
		this.startPos = navigator.transform.GetPosition();
		this.endPos = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
		this.lerpingOffset = true;
		this.offset.y = this.startOffsetY;
		this.animcontroller.Offset = this.offset;
	}

	private static bool IsSurfaceSwimCell(int cell)
	{
		if (!Grid.IsWorldValidCell(cell) || !Grid.IsLiquid(cell))
		{
			return false;
		}
		int num = Grid.CellAbove(cell);
		return Grid.IsWorldValidCell(num) && !Grid.IsLiquid(num);
	}

	public override void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		this.lerpingOffset = false;
		base.EndTransition(navigator, transition);
		if (MathF.Abs(this.animcontroller.Offset.y) > SwimMonitor.OffsetEpsilon)
		{
			this.offset = Vector3.zero;
			this.animcontroller.Offset = this.offset;
		}
	}

	private Vector3 offset;

	private KBatchedAnimController animcontroller;

	private bool lerpingOffset;

	private float startOffsetY;

	private float targetOffsetY;

	private Vector3 startPos;

	private Vector3 endPos;
}
