using System;
using KSerialization;
using UnityEngine;

public class FoodStorage : KMonoBehaviour
{
	public FilteredStorage FilteredStorage { get; set; }

	public bool SpicedFoodOnly
	{
		get
		{
			return this.onlyStoreSpicedFood;
		}
		set
		{
			this.onlyStoreSpicedFood = value;
			base.Trigger(1163645216, this.onlyStoreSpicedFood);
			if (this.onlyStoreSpicedFood)
			{
				this.FilteredStorage.AddForbiddenTag(GameTags.UnspicedFood);
				this.storage.DropHasTags(new Tag[]
				{
					GameTags.Edible,
					GameTags.UnspicedFood
				});
				return;
			}
			this.FilteredStorage.RemoveForbiddenTag(GameTags.UnspicedFood);
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<FoodStorage>(-905833192, FoodStorage.OnCopySettingsDelegate);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
	}

	private void OnCopySettings(object data)
	{
		FoodStorage component = ((GameObject)data).GetComponent<FoodStorage>();
		if (component != null)
		{
			this.SpicedFoodOnly = component.SpicedFoodOnly;
		}
	}

	[Serialize]
	private bool onlyStoreSpicedFood;

	[MyCmpReq]
	public Storage storage;

	private static readonly EventSystem.IntraObjectHandler<FoodStorage> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<FoodStorage>(delegate(FoodStorage component, object data)
	{
		component.OnCopySettings(data);
	});
}
