using System;

public class RefineryNeeds : KMonoBehaviour
{
	public static RefineryNeeds Instance { get; private set; }

	public static void DestroyInstance()
	{
		RefineryNeeds.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		RefineryNeeds.Instance = this;
		Components.Refineries.Register(new Action<Refinery>(this.OnAddRefinery), new Action<Refinery>(this.OnRemoveRefinery));
	}

	private void OnAddRefinery(Refinery refinery)
	{
		refinery.OnCreateOrder = (Action<Refinery.UserOrder>)Delegate.Combine(refinery.OnCreateOrder, new Action<Refinery.UserOrder>(this.OnCreateOrder));
		refinery.OnOrderCancelledOrComplete = (Action<Refinery.UserOrder>)Delegate.Combine(refinery.OnOrderCancelledOrComplete, new Action<Refinery.UserOrder>(this.OnFinishOrder));
	}

	private void OnRemoveRefinery(Refinery refinery)
	{
		refinery.OnCreateOrder = (Action<Refinery.UserOrder>)Delegate.Remove(refinery.OnCreateOrder, new Action<Refinery.UserOrder>(this.OnCreateOrder));
		refinery.OnOrderCancelledOrComplete = (Action<Refinery.UserOrder>)Delegate.Remove(refinery.OnOrderCancelledOrComplete, new Action<Refinery.UserOrder>(this.OnFinishOrder));
	}

	private void OnCreateOrder(Refinery.UserOrder order)
	{
		foreach (ComplexRecipe.RecipeElement recipeElement in order.recipe.ingredients)
		{
			MaterialNeeds.Instance.UpdateNeed(recipeElement.material, recipeElement.amount);
		}
	}

	private void OnFinishOrder(Refinery.UserOrder order)
	{
		foreach (ComplexRecipe.RecipeElement recipeElement in order.recipe.ingredients)
		{
			MaterialNeeds.Instance.UpdateNeed(recipeElement.material, -recipeElement.amount);
		}
	}
}
