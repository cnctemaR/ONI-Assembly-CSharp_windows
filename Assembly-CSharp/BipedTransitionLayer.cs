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
		this.jetPackSpeed = 7f;
		this.movementSpeed = Db.Get().AttributeConverters.MovementSpeed.Lookup(navigator.gameObject);
		this.attributeLevels = navigator.GetComponent<AttributeLevels>();
		this.attributes = navigator.gameObject.GetAttributes();
	}

	public override void BeginTransition(Navigator navigator, Navigator.ActiveTransition transition)
	{
		base.BeginTransition(navigator, transition);
		float num = 1f;
		bool flag = (transition.start == NavType.Pole || transition.end == NavType.Pole) && transition.y < 0 && transition.x == 0;
		bool flag2 = transition.start == NavType.Tube || transition.end == NavType.Tube;
		bool flag3 = transition.start == NavType.Hover || transition.end == NavType.Hover;
		bool flag4 = transition.start == NavType.Swim || transition.end == NavType.Swim;
		bool flag5 = !flag && !flag2 && !flag3;
		int num2 = Grid.PosToCell(navigator);
		this.isInLiquid = navigator.CurrentNavType == NavType.Swim || Grid.IsSubstantialLiquid(num2, 0.35f);
		if (flag5)
		{
			if (this.isWalking)
			{
				return;
			}
			num = this.GetMovementSpeedMultiplier();
		}
		float num3 = 1f;
		bool flag6 = (navigator.flags & PathFinder.PotentialPath.Flags.HasAtmoSuit) > PathFinder.PotentialPath.Flags.None;
		bool flag7 = (navigator.flags & PathFinder.PotentialPath.Flags.HasJetPack) > PathFinder.PotentialPath.Flags.None;
		bool flag8 = (navigator.flags & PathFinder.PotentialPath.Flags.HasLeadSuit) > PathFinder.PotentialPath.Flags.None;
		bool flag9 = flag7 || flag6 || flag8;
		if (!flag9 && !flag4 && Grid.IsSubstantialLiquid(num2, 0.35f))
		{
			num3 = 0.5f;
		}
		else if (flag9 && flag4)
		{
			num3 = 0.3f;
			transition.animSpeed = BipedTransitionLayer.GetSwimmingInSuitAnimSpeed(transition);
		}
		num *= num3;
		if (transition.x == 0 && (transition.start == NavType.Ladder || transition.start == NavType.Pole) && transition.start == transition.end)
		{
			if (flag)
			{
				transition.speed = 15f * num;
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
						float num4 = component.upwardsMovementSpeedMultiplier;
						if (transition.y < 0)
						{
							num4 = component.downwardsMovementSpeedMultiplier;
						}
						transition.speed *= num4;
						transition.animSpeed *= num4;
					}
				}
			}
		}
		else if (flag2)
		{
			transition.speed = this.GetTubeTravellingSpeedMultiplier(navigator);
		}
		else if (flag3)
		{
			transition.speed = this.jetPackSpeed;
			if (transition.x == 0 && transition.y == -1)
			{
				transition.speed *= 0.75f;
			}
			transition.animSpeed = transition.speed;
		}
		else
		{
			transition.speed = this.floorSpeed * num;
		}
		float num5 = num - 1f;
		transition.animSpeed += transition.animSpeed * num5 / 2f;
		if (transition.start == NavType.Floor && transition.end == NavType.Floor)
		{
			int num6 = Grid.CellBelow(num2);
			if (Grid.Foundation[num6])
			{
				GameObject gameObject2 = Grid.Objects[num6, 1];
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
		if (!this.isWalking && !flag && !flag2 && this.attributeLevels != null)
		{
			this.attributeLevels.AddExperience(Db.Get().Attributes.Athletics.Id, Time.time - this.startTime, DUPLICANTSTATS.ATTRIBUTE_LEVELING.ALL_DAY_EXPERIENCE);
		}
		int num = Grid.OffsetCell(navigator.cachedCell, transition.x, transition.y);
		if (transition.end != NavType.Swim)
		{
			Grid.IsSubstantialLiquid(num, 0.35f);
		}
		bool flag3 = this.isInLiquid;
	}

	public float GetTubeTravellingSpeedMultiplier(Navigator navigator)
	{
		AttributeInstance attributeInstance = Db.Get().Attributes.TransitTubeTravelSpeed.Lookup(navigator.gameObject);
		if (attributeInstance != null)
		{
			return attributeInstance.GetTotalValue();
		}
		return DUPLICANTSTATS.STANDARD.BaseStats.TRANSIT_TUBE_TRAVEL_SPEED;
	}

	public static float GetMovementSpeedMultiplier(AttributeConverterInstance movementSpeed)
	{
		float num = 1f;
		if (movementSpeed != null)
		{
			num += movementSpeed.Evaluate();
		}
		return Mathf.Max(0.1f, num);
	}

	public static float GetSwimmingInSuitAnimSpeed(Navigator.ActiveTransition transition)
	{
		if (!transition.isLooping && transition.x != 0 && transition.y != 0 && transition.start == NavType.Swim && transition.end == NavType.Swim)
		{
			return 0.3f;
		}
		return transition.animSpeed;
	}

	public float GetMovementSpeedMultiplier()
	{
		return BipedTransitionLayer.GetMovementSpeedMultiplier(this.movementSpeed);
	}

	private bool isWalking;

	private float floorSpeed;

	private float ladderSpeed;

	private float startTime;

	private bool isInLiquid;

	private float jetPackSpeed;

	private const float downPoleSpeed = 15f;

	private const float WATER_SPEED_PENALTY = 0.5f;

	private const float SUIT_SWIM_SPEED_PENALTY = 0.3f;

	private const float SUIT_SWIM_ANIM_PENALTY = 0.3f;

	private AttributeConverterInstance movementSpeed;

	private AttributeLevels attributeLevels;

	private Attributes attributes;
}
