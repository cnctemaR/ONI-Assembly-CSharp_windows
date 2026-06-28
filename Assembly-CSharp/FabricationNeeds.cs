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
	}

	private void OnRemoveFabricator(Fabricator fabricator)
	{
		fabricator.OnCreateOrder = (Action<Fabricator.UserOrder>)Delegate.Remove(fabricator.OnCreateOrder, new Action<Fabricator.UserOrder>(this.OnCreateOrder));
	}

	private void OnCreateOrder(Fabricator.UserOrder order)
	{
		order.OnComplete = (Action<Fabricator.UserOrder>)Delegate.Combine(order.OnComplete, new Action<Fabricator.UserOrder>(this.OnFinishOrder));
		order.OnCancel = (Action<Fabricator.UserOrder>)Delegate.Combine(order.OnCancel, new Action<Fabricator.UserOrder>(this.OnFinishOrder));
		foreach (Recipe.Ingredient ingredient in order.recipe.GetAllIngredients(order.orderTags))
		{
			MaterialNeeds.Instance.UpdateNeed(ingredient.tag, ingredient.amount);
		}
	}

	private void OnFinishOrder(Fabricator.UserOrder order)
	{
		order.OnComplete = (Action<Fabricator.UserOrder>)Delegate.Remove(order.OnComplete, new Action<Fabricator.UserOrder>(this.OnFinishOrder));
		order.OnCancel = (Action<Fabricator.UserOrder>)Delegate.Remove(order.OnCancel, new Action<Fabricator.UserOrder>(this.OnFinishOrder));
		foreach (Recipe.Ingredient ingredient in order.recipe.GetAllIngredients(order.orderTags))
		{
			MaterialNeeds.Instance.UpdateNeed(ingredient.tag, -ingredient.amount);
		}
	}
}
