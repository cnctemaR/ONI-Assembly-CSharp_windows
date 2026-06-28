using System;
using System.Collections.Generic;
using KSerialization;

[SerializationConfig(MemberSerialization.OptIn)]
public class RationTracker : KMonoBehaviour, ISaveLoadableJson
{
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
		GameClock.Instance.Subscribe(631075836, new EventSystem.EventHandler(this.OnNewDay));
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
			for (int i = 0; i < pickupables.Count; i++)
			{
				Edible component = pickupables[i].GetComponent<Edible>();
				if (!(component == null))
				{
					num += component.rations;
					if (unitCountByFoodType != null)
					{
						if (!unitCountByFoodType.ContainsKey(component.FoodID))
						{
							unitCountByFoodType[component.FoodID] = 0f;
						}
						string foodID;
						string text = (foodID = component.FoodID);
						float num2 = unitCountByFoodType[foodID];
						unitCountByFoodType[text] = num2 + component.GetComponent<PrimaryElement>().Units;
					}
				}
			}
		}
		return num;
	}

	public void RegisterRationsProduced(int rations)
	{
		this.currentFrame.rationsProduced = this.currentFrame.rationsProduced + rations;
	}

	public void RegisterRationsConsumed(float rations)
	{
		this.currentFrame.rationsConsumed = this.currentFrame.rationsConsumed + rations;
	}

	private static RationTracker instance;

	[Serialize]
	public RationTracker.Frame currentFrame = default(RationTracker.Frame);

	[Serialize]
	public RationTracker.Frame previousFrame = default(RationTracker.Frame);

	public struct Frame
	{
		public int rationsProduced;

		public float rationsConsumed;
	}
}
