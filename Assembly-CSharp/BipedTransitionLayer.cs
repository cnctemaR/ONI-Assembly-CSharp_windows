using System;
using Klei.AI;
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
		this.movementSpeed = Db.Get().AttributeConverters.MovementSpeed.Lookup(navigator.gameObject);
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
		if (transition.x == 0 && transition.start == transition.end && (transition.start == NavType.Ladder || transition.start == NavType.Pole))
		{
			transition.speed = this.ladderSpeed * movementSpeedMultiplier;
			GameObject gameObject = Grid.Objects[num, 1];
			if (gameObject != null)
			{
				Ladder component = gameObject.GetComponent<Ladder>();
				if (component != null)
				{
					float num2 = component.upwardsMovementSpeedMultiplier;
					if (transition.y < 0)
					{
						num2 = component.downwardsMovementSpeedMultiplier;
					}
					transition.speed *= num2;
					transition.animSpeed *= num2;
				}
			}
		}
		else
		{
			transition.speed = this.floorSpeed * movementSpeedMultiplier;
		}
		float num3 = movementSpeedMultiplier - 1f;
		transition.animSpeed += transition.animSpeed * num3 / 2f;
		if (transition.start == NavType.Floor && transition.end == NavType.Floor)
		{
			int num4 = Grid.CellBelow(num);
			if (Grid.Foundation[num4])
			{
				GameObject gameObject2 = Grid.Objects[num4, 1];
				if (gameObject2 != null)
				{
					SimCellOccupier component2 = gameObject2.GetComponent<SimCellOccupier>();
					if (component2 != null)
					{
						transition.speed *= component2.movementSpeedMultiplier;
						transition.animSpeed *= component2.movementSpeedMultiplier;
					}
				}
			}
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
		if (this.movementSpeed != null)
		{
			num += this.movementSpeed.Evaluate();
		}
		return Mathf.Max(0.1f, num);
	}

	private bool isWalking;

	private float floorSpeed;

	private float ladderSpeed;

	private float startTime;

	private AttributeConverterInstance movementSpeed;
}
