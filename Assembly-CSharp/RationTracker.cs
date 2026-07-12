using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[AddComponentMenu("KMonoBehaviour/scripts/RationTracker")]
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

	public float CountRations(Dictionary<string, float> unitCountByFoodType, WorldInventory inventory, bool excludeUnreachable = true)
	{
		float num = 0f;
		ICollection<Pickupable> pickupables = inventory.GetPickupables(GameTags.Edible, false);
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
						string foodID = component.FoodID;
						unitCountByFoodType[foodID] += component.Units;
					}
				}
			}
		}
		return num;
	}

	public float CountRationsByFoodType(string foodID, WorldInventory inventory, bool excludeUnreachable = true)
	{
		float num = 0f;
		ICollection<Pickupable> pickupables = inventory.GetPickupables(GameTags.Edible, false);
		if (pickupables != null)
		{
			foreach (Pickupable pickupable in pickupables)
			{
				if (!pickupable.KPrefabID.HasTag(GameTags.StoredPrivate))
				{
					Edible component = pickupable.GetComponent<Edible>();
					if (component.FoodID == foodID)
					{
						num += component.Calories;
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

	public void RegisterRationsConsumed(Edible edible)
	{
		this.currentFrame.caloriesConsumed = this.currentFrame.caloriesConsumed + edible.caloriesConsumed;
		if (!this.caloriesConsumedByFood.ContainsKey(edible.FoodInfo.Id))
		{
			this.caloriesConsumedByFood.Add(edible.FoodInfo.Id, edible.caloriesConsumed);
			return;
		}
		Dictionary<string, float> dictionary = this.caloriesConsumedByFood;
		string id = edible.FoodInfo.Id;
		dictionary[id] += edible.caloriesConsumed;
	}

	public float GetCaloiresConsumedByFood(List<string> foodTypes)
	{
		float num = 0f;
		foreach (string text in foodTypes)
		{
			if (this.caloriesConsumedByFood.ContainsKey(text))
			{
				num += this.caloriesConsumedByFood[text];
			}
		}
		return num;
	}

	public float GetCaloriesConsumed()
	{
		float num = 0f;
		foreach (KeyValuePair<string, float> keyValuePair in this.caloriesConsumedByFood)
		{
			num += keyValuePair.Value;
		}
		return num;
	}

	private static RationTracker instance;

	[Serialize]
	public RationTracker.Frame currentFrame;

	[Serialize]
	public RationTracker.Frame previousFrame;

	[Serialize]
	public Dictionary<string, float> caloriesConsumedByFood = new Dictionary<string, float>();

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
