using System;
using System.Collections.Generic;

public class FabricatorIngredientStatusManager : KMonoBehaviour, ISim1000ms
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.selectable = base.GetComponent<KSelectable>();
		this.fabricator = base.GetComponent<ComplexFabricator>();
		this.InitializeBalances();
	}

	private void InitializeBalances()
	{
		foreach (ComplexRecipe complexRecipe in this.fabricator.GetRecipes())
		{
			this.recipeRequiredResourceBalances.Add(complexRecipe, new Dictionary<Tag, float>());
			foreach (ComplexRecipe.RecipeElement recipeElement in complexRecipe.ingredients)
			{
				this.recipeRequiredResourceBalances[complexRecipe].Add(recipeElement.material, 0f);
			}
		}
	}

	public void Sim1000ms(float dt)
	{
		this.RefreshStatusItems();
	}

	private void RefreshStatusItems()
	{
		using (Dictionary<ComplexRecipe, Guid>.Enumerator enumerator = this.statusItems.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				KeyValuePair<ComplexRecipe, Guid> status = enumerator.Current;
				if (this.fabricator.GetUserOrders().Find((ComplexFabricator.UserOrder match) => match.recipe == status.Key) == null)
				{
					this.deadOrderKeys.Add(status.Key);
				}
			}
		}
		foreach (ComplexRecipe complexRecipe in this.deadOrderKeys)
		{
			this.recipeRequiredResourceBalances[complexRecipe].Clear();
			foreach (ComplexRecipe.RecipeElement recipeElement in complexRecipe.ingredients)
			{
				this.recipeRequiredResourceBalances[complexRecipe].Add(recipeElement.material, 0f);
			}
			this.selectable.RemoveStatusItem(this.statusItems[complexRecipe], false);
			this.statusItems.Remove(complexRecipe);
		}
		this.deadOrderKeys.Clear();
		foreach (ComplexFabricator.UserOrder userOrder in this.fabricator.GetUserOrders())
		{
			bool flag = false;
			foreach (ComplexRecipe.RecipeElement recipeElement2 in userOrder.recipe.ingredients)
			{
				float num = this.fabricator.inStorage.GetAmountAvailable(recipeElement2.material) + this.fabricator.buildStorage.GetAmountAvailable(recipeElement2.material) + WorldInventory.Instance.GetAmount(recipeElement2.material) - recipeElement2.amount;
				flag = flag || this.ChangeRecipeRequiredResourceBalance(userOrder.recipe, recipeElement2.material, num) || (this.statusItems.ContainsKey(userOrder.recipe) && this.fabricator.GetRecipeQueueCount(userOrder.recipe) == 0);
			}
			if (flag)
			{
				if (this.statusItems.ContainsKey(userOrder.recipe))
				{
					this.selectable.RemoveStatusItem(this.statusItems[userOrder.recipe], false);
					this.statusItems.Remove(userOrder.recipe);
				}
				if (this.fabricator.GetRecipeQueueCount(userOrder.recipe) > 0 || this.fabricator.GetRecipeQueueCount(userOrder.recipe) == ComplexFabricator.QUEUE_INFINITE)
				{
					foreach (float num2 in this.recipeRequiredResourceBalances[userOrder.recipe].Values)
					{
						if (num2 < 0f)
						{
							Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
							foreach (KeyValuePair<Tag, float> keyValuePair in this.recipeRequiredResourceBalances[userOrder.recipe])
							{
								if (keyValuePair.Value < 0f)
								{
									dictionary.Add(keyValuePair.Key, -keyValuePair.Value);
								}
							}
							Guid guid = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.MaterialsUnavailable, dictionary);
							this.statusItems.Add(userOrder.recipe, guid);
							break;
						}
					}
				}
			}
		}
	}

	private bool ChangeRecipeRequiredResourceBalance(ComplexRecipe recipe, Tag tag, float newBalance)
	{
		bool flag = false;
		if (this.recipeRequiredResourceBalances[recipe][tag] >= 0f != newBalance >= 0f)
		{
			flag = true;
		}
		this.recipeRequiredResourceBalances[recipe][tag] = newBalance;
		return flag;
	}

	private KSelectable selectable;

	private ComplexFabricator fabricator;

	private Dictionary<ComplexRecipe, Guid> statusItems = new Dictionary<ComplexRecipe, Guid>();

	private Dictionary<ComplexRecipe, Dictionary<Tag, float>> recipeRequiredResourceBalances = new Dictionary<ComplexRecipe, Dictionary<Tag, float>>();

	private List<ComplexRecipe> deadOrderKeys = new List<ComplexRecipe>();
}
