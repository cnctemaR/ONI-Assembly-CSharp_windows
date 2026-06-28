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
		float num = 1f;
		bool flag = (transition.start == NavType.Pole || transition.end == NavType.Pole) && transition.y < 0 && transition.x == 0;
		bool flag2 = transition.start == NavType.Tube || transition.end == NavType.Tube;
		bool flag3 = !flag && !flag2;
		if (flag3)
		{
			if (this.isWalking)
			{
				return;
			}
			num = this.GetMovementSpeedMultiplier(navigator);
		}
		int num2 = Grid.PosToCell(navigator);
		if (transition.x == 0 && (transition.start == NavType.Ladder || transition.start == NavType.Pole) && transition.start == transition.end)
		{
			if (flag)
			{
				transition.speed = this.downPoleSpeed;
			}
			else
			{
				transition.speed = this.ladderSpeed * num;
				GameObject gameObject = Grid.Objects[num2, 1];
				if (gameObject != null)
				{
					Ladder component = gameObject.GetComponent<Ladder>();
					if (component != null)
					{
						float num3 = component.upwardsMovementSpeedMultiplier;
						if (transition.y < 0)
						{
							num3 = component.downwardsMovementSpeedMultiplier;
						}
						transition.speed *= num3;
						transition.animSpeed *= num3;
					}
				}
			}
		}
		else if (flag2)
		{
			transition.speed = this.tubeSpeed;
		}
		else
		{
			transition.speed = this.floorSpeed * num;
		}
		float num4 = num - 1f;
		transition.animSpeed += transition.animSpeed * num4 / 2f;
		if (transition.start == NavType.Floor && transition.end == NavType.Floor)
		{
			int num5 = Grid.CellBelow(num2);
			if (Grid.Foundation[num5])
			{
				GameObject gameObject2 = Grid.Objects[num5, 1];
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
		bool flag = (transition.start == NavType.Pole || transition.end == NavType.Pole) && transition.y < 0 && transition.x == 0;
		bool flag2 = transition.start == NavType.Tube || transition.end == NavType.Tube;
		if (!this.isWalking && !flag && !flag2)
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

	private float tubeSpeed = 18f;

	private float downPoleSpeed = 15f;

	private AttributeConverterInstance movementSpeed;
}
