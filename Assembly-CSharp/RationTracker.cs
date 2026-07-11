using System;
using System.Collections.Generic;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class RationTracker : KMonoBehaviour, ISaveLoadable
{
	public static void DestroyInstance()
	{
		RationTracker.instance = null;
	}

	public static RationTracker Get()
	{
		return RationTracker.instance;
	}

	protected override void OnPrefabInit()
	{
		RationTracker.instance = this;
	}

	protected override void OnSpawn()
	{
		base.Subscribe<RationTracker>(631075836, RationTracker.OnNewDayDelegate);
	}

	private void OnNewDay(object data)
	{
		this.previousFrame = this.currentFrame;
		this.currentFrame = default(RationTracker.Frame);
	}

	public float CountRations(Dictionary<string, float> unitCountByFoodType, bool excludeUnreachable = true)
	{
		float num = 0f;
		List<Pickupable> pickupables = WorldInventory.Instance.GetPickupables(GameTags.Edible);
		if (pickupables != null)
		{
			foreach (Pickupable pickupable in pickupables)
			{
				if (!pickupable.KPrefabID.HasTag(GameTags.StoredPrivate))
				{
					Edible component = pickupable.GetComponent<Edible>();
					num += component.Calories;
					if (unitCountByFoodType != null)
					{
						if (!unitCountByFoodType.ContainsKey(component.FoodID))
						{
							unitCountByFoodType[component.FoodID] = 0f;
						}
						string foodID;
						unitCountByFoodType[foodID = component.FoodID] = unitCountByFoodType[foodID] + component.Units;
					}
				}
			}
		}
		return num;
	}

	public void RegisterCaloriesProduced(float calories)
	{
		this.currentFrame.caloriesProduced = this.currentFrame.caloriesProduced + calories;
	}

	public void RegisterRationsConsumed(float calories)
	{
		this.currentFrame.caloriesConsumed = this.currentFrame.caloriesConsumed + calories;
	}

	private static RationTracker instance;

	[Serialize]
	public RationTracker.Frame currentFrame = default(RationTracker.Frame);

	[Serialize]
	public RationTracker.Frame previousFrame = default(RationTracker.Frame);

	private static readonly EventSystem.IntraObjectHandler<RationTracker> OnNewDayDelegate = new EventSystem.IntraObjectHandler<RationTracker>(delegate(RationTracker component, object data)
	{
		component.OnNewDay(data);
	});

	public struct Frame
	{
		public float caloriesProduced;

		public float caloriesConsumed;
	}
}
