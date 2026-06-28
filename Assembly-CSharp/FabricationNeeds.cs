using System;

public class FabricationNeeds : KMonoBehaviour
{
	public static FabricationNeeds Instance { get; private set; }

	protected override void OnPrefabInit()
	{
		FabricationNeeds.Instance = this;
		Components.Fabricators.Register(new Action<Fabricator>(this.OnAddFabricator), new Action<Fabricator>(this.OnRemoveFabricator));
	}

	private void OnAddFabricator(Fabricator fabricator)
	{
		fabricator.OnCreateOrder = (Action<Fabricator.UserOrder>)Delegate.Combine(fabricator.OnCreateOrder, new Action<Fabricator.UserOrder>(this.OnCreateOrder));
		fabricator.OnOrderCancelledOrComplete = (Action<Fabricator.UserOrder>)Delegate.Combine(fabricator.OnOrderCancelledOrComplete, new Action<Fabricator.UserOrder>(this.OnFinishOrder));
	}

	private void OnRemoveFabricator(Fabricator fabricator)
	{
		fabricator.OnCreateOrder = (Action<Fabricator.UserOrder>)Delegate.Remove(fabricator.OnCreateOrder, new Action<Fabricator.UserOrder>(this.OnCreateOrder));
		fabricator.OnOrderCancelledOrComplete = (Action<Fabricator.UserOrder>)Delegate.Remove(fabricator.OnOrderCancelledOrComplete, new Action<Fabricator.UserOrder>(this.OnFinishOrder));
	}

	private void OnCreateOrder(Fabricator.UserOrder order)
	{
		foreach (Recipe.Ingredient ingredient in order.recipe.GetAllIngredients(order.orderTags))
		{
			MaterialNeeds.Instance.UpdateNeed(ingredient.tag, ingredient.amount);
		}
	}

	private void OnFinishOrder(Fabricator.UserOrder order)
	{
		foreach (Recipe.Ingredient ingredient in order.recipe.GetAllIngredients(order.orderTags))
		{
			MaterialNeeds.Instance.UpdateNeed(ingredient.tag, -ingredient.amount);
		}
	}
}
