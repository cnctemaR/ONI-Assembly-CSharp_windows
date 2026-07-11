using System;

public class ComplexFabricatorNeeds : KMonoBehaviour
{
	public static ComplexFabricatorNeeds Instance { get; private set; }

	public static void DestroyInstance()
	{
		ComplexFabricatorNeeds.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		ComplexFabricatorNeeds.Instance = this;
		Components.ComplexFabricators.Register(new Action<ComplexFabricator>(this.OnAddRefinery), new Action<ComplexFabricator>(this.OnRemoveRefinery));
	}

	private void OnAddRefinery(ComplexFabricator refinery)
	{
		refinery.OnCreateMachineOrder = (Action<ComplexFabricator.MachineOrder>)Delegate.Combine(refinery.OnCreateMachineOrder, new Action<ComplexFabricator.MachineOrder>(this.OnCreateMachineOrder));
		refinery.OnMachineOrderCancelledOrComplete = (Action<ComplexFabricator.MachineOrder>)Delegate.Combine(refinery.OnMachineOrderCancelledOrComplete, new Action<ComplexFabricator.MachineOrder>(this.OnFinishMachineOrder));
	}

	private void OnRemoveRefinery(ComplexFabricator refinery)
	{
		refinery.OnCreateMachineOrder = (Action<ComplexFabricator.MachineOrder>)Delegate.Remove(refinery.OnCreateMachineOrder, new Action<ComplexFabricator.MachineOrder>(this.OnCreateMachineOrder));
		refinery.OnMachineOrderCancelledOrComplete = (Action<ComplexFabricator.MachineOrder>)Delegate.Remove(refinery.OnMachineOrderCancelledOrComplete, new Action<ComplexFabricator.MachineOrder>(this.OnFinishMachineOrder));
	}

	private void OnCreateMachineOrder(ComplexFabricator.MachineOrder order)
	{
		foreach (ComplexRecipe.RecipeElement recipeElement in order.parentOrder.recipe.ingredients)
		{
			MaterialNeeds.Instance.UpdateNeed(recipeElement.material, recipeElement.amount);
		}
	}

	private void OnFinishMachineOrder(ComplexFabricator.MachineOrder order)
	{
		foreach (ComplexRecipe.RecipeElement recipeElement in order.parentOrder.recipe.ingredients)
		{
			MaterialNeeds.Instance.UpdateNeed(recipeElement.material, -recipeElement.amount);
		}
	}
}
