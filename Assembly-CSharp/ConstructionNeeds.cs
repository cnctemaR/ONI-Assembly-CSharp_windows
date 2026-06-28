using System;

public class ConstructionNeeds : KMonoBehaviour
{
	public static ConstructionNeeds Instance { get; private set; }

	protected override void OnPrefabInit()
	{
		ConstructionNeeds.Instance = this;
		Components.Constructables.Register(new Action<Constructable>(this.OnAddConstructable), new Action<Constructable>(this.OnRemoveConstructable));
	}

	private void OnAddConstructable(Constructable constructable)
	{
		foreach (Recipe.Ingredient ingredient in constructable.Recipe.GetAllIngredients(constructable.SelectedElements))
		{
			MaterialNeeds.Instance.UpdateNeed(ingredient.tag, ingredient.amount);
		}
	}

	private void OnRemoveConstructable(Constructable constructable)
	{
		foreach (Recipe.Ingredient ingredient in constructable.Recipe.GetAllIngredients(constructable.SelectedElements))
		{
			MaterialNeeds.Instance.UpdateNeed(ingredient.tag, -ingredient.amount);
		}
	}

	protected override void OnCleanUp()
	{
		Components.Constructables.Unregister(new Action<Constructable>(this.OnAddConstructable), new Action<Constructable>(this.OnRemoveConstructable));
	}
}
