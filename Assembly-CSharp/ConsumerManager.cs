using System;
using System.Collections.Generic;
using System.Diagnostics;
using KSerialization;
using TUNING;

[SerializationConfig(MemberSerialization.OptIn)]
public class ConsumerManager : KMonoBehaviour, ISaveLoadable
{
	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Tag> OnDiscover;

	public List<Tag> DefaultForbiddenTagsList
	{
		get
		{
			return this.defaultForbiddenTagsList;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		ConsumerManager.instance = this;
		this.RefreshDiscovered(null);
		WorldInventory.Instance.OnDiscover += this.OnWorldInventoryDiscover;
		Game.Instance.Subscribe(-107300940, new Action<object>(this.RefreshDiscovered));
	}

	public bool isDiscovered(Tag id)
	{
		return !this.undiscoveredConsumableTags.Contains(id);
	}

	private void OnWorldInventoryDiscover(Tag t)
	{
		if (this.undiscoveredConsumableTags.Contains(t))
		{
			this.RefreshDiscovered(null);
		}
	}

	public void RefreshDiscovered(object data = null)
	{
		foreach (EdiblesManager.FoodInfo foodInfo in FOOD.FOOD_TYPES_LIST)
		{
			if (!this.ShouldBeDiscovered(foodInfo.Id.ToTag()) && !this.undiscoveredConsumableTags.Contains(foodInfo.Id.ToTag()))
			{
				this.undiscoveredConsumableTags.Add(foodInfo.Id.ToTag());
				if (this.OnDiscover != null)
				{
					this.OnDiscover("UndiscoveredSomething".ToTag());
				}
			}
			else if (this.undiscoveredConsumableTags.Contains(foodInfo.Id.ToTag()) && this.ShouldBeDiscovered(foodInfo.Id.ToTag()))
			{
				this.undiscoveredConsumableTags.Remove(foodInfo.Id.ToTag());
				if (this.OnDiscover != null)
				{
					this.OnDiscover(foodInfo.Id.ToTag());
				}
				if (!WorldInventory.Instance.IsDiscovered(foodInfo.Id.ToTag()))
				{
					if (foodInfo.CaloriesPerUnit == 0f)
					{
						WorldInventory.Instance.Discover(foodInfo.Id.ToTag(), GameTags.CookingIngredient);
					}
					else
					{
						WorldInventory.Instance.Discover(foodInfo.Id.ToTag(), GameTags.Edible);
					}
				}
			}
		}
	}

	private bool ShouldBeDiscovered(Tag food_id)
	{
		bool flag;
		if (DebugHandler.InstantBuildMode)
		{
			flag = true;
		}
		else if (WorldInventory.Instance.IsDiscovered(food_id))
		{
			flag = true;
		}
		else
		{
			foreach (Recipe recipe in RecipeManager.Get().recipes)
			{
				if (recipe.Result == food_id)
				{
					foreach (string text in recipe.fabricators)
					{
						if (Db.Get().TechItems.IsTechItemComplete(text))
						{
							return true;
						}
					}
				}
			}
			foreach (Crop crop in Components.Crops)
			{
				if (Grid.Visible[Grid.PosToCell(crop.gameObject)] > 0)
				{
					if (crop.cropId == food_id.Name)
					{
						return true;
					}
				}
			}
			flag = false;
		}
		return flag;
	}

	public static ConsumerManager instance;

	[Serialize]
	private List<Tag> undiscoveredConsumableTags = new List<Tag>();

	[Serialize]
	private List<Tag> defaultForbiddenTagsList = new List<Tag>();
}
