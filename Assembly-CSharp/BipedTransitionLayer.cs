using System;
using Klei.AI;
using TUNING;
using UnityEngine;

public class BipedTransitionLayer : TransitionDriver.OverrideLayer
{
	public BipedTransitionLayer(Navigator navigator, float floor_speed, float ladder_speed)
		: base(navigator)
	{
		navigator.Subscribe(1773898642, delegate(object data)
		{
			this.isWalking = true;
		});
		navigator.Subscribe(1597112836, delegate(object data)
		{
			this.isWalking = false;
		});
		this.floorSpeed = floor_speed;
		this.ladderSpeed = ladder_speed;
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
		if (this.isWalking)
		{
			return;
		}
		int num = Grid.PosToCell(navigator);
		float movementSpeedMultiplier = this.GetMovementSpeedMultiplier(navigator);
		if (transition.start == NavType.Ladder && transition.end == NavType.Ladder)
		{
			transition.speed = this.ladderSpeed * movementSpeedMultiplier;
		}
		else
		{
			transition.speed = this.floorSpeed * movementSpeedMultiplier;
		}
		float num2 = movementSpeedMultiplier - 1f;
		transition.animSpeed += transition.animSpeed * num2 / 2f;
		if (transition.start == NavType.Floor && transition.end == NavType.Floor && Grid.Foundation[Grid.CellBelow(num)])
		{
			transition.speed *= DUPLICANTSTATS.FOUNDATION_MOVEMENT_BOOST;
			transition.animSpeed *= DUPLICANTSTATS.FOUNDATION_MOVEMENT_BOOST;
		}
		this.startTime = Time.time;
	}

	public override void EndTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.EndTransition(navigator, transition);
		if (!this.isWalking)
		{
			AttributeLevels component = navigator.GetComponent<AttributeLevels>();
			if (component != null)
			{
				component.AddExperience(Db.Get().Attributes.Athletics.Id, Time.time - this.startTime);
			}
		}
	}

	public float GetMovementSpeedMultiplier(Navigator navigator)
	{
		float num = 1f;
		AttributeConverterInstance attributeConverterInstance = Db.Get().AttributeConverters.MovementSpeed.Lookup(navigator.gameObject);
		if (attributeConverterInstance != null)
		{
			num += attributeConverterInstance.Evaluate();
		}
		return Mathf.Max(0.1f, num);
	}

	private bool isWalking;

	private float floorSpeed;

	private float ladderSpeed;

	private float startTime;
}
