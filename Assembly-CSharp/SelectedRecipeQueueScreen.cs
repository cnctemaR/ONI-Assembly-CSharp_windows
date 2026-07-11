using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.UI;

public class SelectedRecipeQueueScreen : KScreen
{
	public override float GetSortKey()
	{
		if (this.isEditing)
		{
			return 100f;
		}
		return base.GetSortKey();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.DecrementButton.onClick = delegate
		{
			this.target.DecrementRecipeQueueCount(this.selectedRecipe, false);
			this.RefreshQueueCountDisplay();
			this.ownerScreen.RefreshQueueCountDisplayForRecipe(this.selectedRecipe, this.target);
		};
		this.IncrementButton.onClick = delegate
		{
			this.target.IncrementRecipeQueueCount(this.selectedRecipe);
			this.RefreshQueueCountDisplay();
			this.ownerScreen.RefreshQueueCountDisplayForRecipe(this.selectedRecipe, this.target);
		};
		this.InfiniteButton.onClick += delegate
		{
			if (this.target.GetRecipeQueueCount(this.selectedRecipe) != ComplexFabricator.QUEUE_INFINITE)
			{
				this.target.SetRecipeQueueCount(this.selectedRecipe, ComplexFabricator.QUEUE_INFINITE);
			}
			else
			{
				this.target.SetRecipeQueueCount(this.selectedRecipe, 0);
			}
			this.RefreshQueueCountDisplay();
			this.ownerScreen.RefreshQueueCountDisplayForRecipe(this.selectedRecipe, this.target);
		};
		this.QueueCount.onEndEdit += delegate
		{
			this.isEditing = false;
			this.target.SetRecipeQueueCount(this.selectedRecipe, Mathf.RoundToInt(this.QueueCount.currentValue));
			this.RefreshQueueCountDisplay();
			this.ownerScreen.RefreshQueueCountDisplayForRecipe(this.selectedRecipe, this.target);
		};
		this.QueueCount.onStartEdit += delegate
		{
			this.isEditing = true;
			KScreenManager.Instance.RefreshStack();
		};
	}

	public void SetRecipe(ComplexFabricatorSideScreen owner, ComplexFabricator target, ComplexRecipe recipe)
	{
		this.ownerScreen = owner;
		this.target = target;
		this.selectedRecipe = recipe;
		this.recipeName.text = recipe.GetUIName(false);
		Tuple<Sprite, Color> uisprite = Def.GetUISprite((recipe.nameDisplay != ComplexRecipe.RecipeNameDisplay.Ingredient) ? recipe.results[0].material : recipe.ingredients[0].material, "ui", false);
		this.recipeIcon.sprite = uisprite.first;
		this.recipeIcon.color = uisprite.second;
		this.RefreshIngredientDescriptors();
		this.RefreshResultDescriptors();
		this.RefreshQueueCountDisplay();
	}

	private void RefreshQueueCountDisplay()
	{
		bool flag = this.target.GetRecipeQueueCount(this.selectedRecipe) == ComplexFabricator.QUEUE_INFINITE;
		if (!flag)
		{
			this.QueueCount.SetAmount((float)this.target.GetRecipeQueueCount(this.selectedRecipe));
		}
		else
		{
			this.QueueCount.SetDisplayValue(string.Empty);
		}
		this.InfiniteIcon.gameObject.SetActive(flag);
	}

	private void RefreshResultDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		list.AddRange(this.GetResultDescriptions(this.selectedRecipe));
		list.AddRange(this.target.AdditionalEffectsForRecipe(this.selectedRecipe));
		if (list.Count > 0)
		{
			GameUtil.IndentListOfDescriptors(list, 1);
			list.Insert(0, new Descriptor(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RESULTEFFECTS, UI.UISIDESCREENS.FABRICATORSIDESCREEN.RESULTEFFECTS, Descriptor.DescriptorType.Effect, false));
			this.EffectsDescriptorPanel.gameObject.SetActive(true);
			this.EffectsDescriptorPanel.SetDescriptors(list);
		}
	}

	public List<Descriptor> GetResultDescriptions(ComplexRecipe recipe)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (ComplexRecipe.RecipeElement recipeElement in recipe.results)
		{
			GameObject prefab = Assets.GetPrefab(recipeElement.material);
			string formattedByTag = GameUtil.GetFormattedByTag(recipeElement.material, recipeElement.amount, GameUtil.TimeSlice.None);
			list.Add(new Descriptor(string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPEPRODUCT, prefab.GetProperName(), formattedByTag), string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.TOOLTIPS.RECIPEPRODUCT, prefab.GetProperName(), formattedByTag), Descriptor.DescriptorType.Requirement, false));
			Element element = ElementLoader.GetElement(recipeElement.material);
			if (element != null)
			{
				List<Descriptor> materialDescriptors = GameUtil.GetMaterialDescriptors(element);
				GameUtil.IndentListOfDescriptors(materialDescriptors, 1);
				list.AddRange(materialDescriptors);
			}
			else
			{
				List<Descriptor> effectDescriptors = GameUtil.GetEffectDescriptors(GameUtil.GetAllDescriptors(prefab, false));
				GameUtil.IndentListOfDescriptors(effectDescriptors, 1);
				list.AddRange(effectDescriptors);
			}
		}
		return list;
	}

	private void RefreshIngredientDescriptors()
	{
		List<Descriptor> list = new List<Descriptor>();
		list.Add(new Descriptor(UI.UISIDESCREENS.FABRICATORSIDESCREEN.COST, UI.UISIDESCREENS.FABRICATORSIDESCREEN.COST, Descriptor.DescriptorType.Requirement, false));
		List<Descriptor> ingredientDescriptions = this.GetIngredientDescriptions(this.selectedRecipe);
		GameUtil.IndentListOfDescriptors(ingredientDescriptions, 1);
		list.AddRange(ingredientDescriptions);
		this.IngredientsDescriptorPanel.gameObject.SetActive(true);
		this.IngredientsDescriptorPanel.SetDescriptors(list);
	}

	public List<Descriptor> GetIngredientDescriptions(ComplexRecipe recipe)
	{
		List<Descriptor> list = new List<Descriptor>();
		foreach (ComplexRecipe.RecipeElement recipeElement in recipe.ingredients)
		{
			GameObject prefab = Assets.GetPrefab(recipeElement.material);
			string formattedByTag = GameUtil.GetFormattedByTag(recipeElement.material, recipeElement.amount, GameUtil.TimeSlice.None);
			string formattedByTag2 = GameUtil.GetFormattedByTag(recipeElement.material, WorldInventory.Instance.GetAmount(recipeElement.material), GameUtil.TimeSlice.None);
			bool flag = WorldInventory.Instance.GetAmount(recipeElement.material) >= recipeElement.amount;
			string text = ((!flag) ? ("<color=#F44A47>" + string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPERQUIREMENT, prefab.GetProperName(), formattedByTag, formattedByTag2) + "</color>") : string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.RECIPERQUIREMENT, prefab.GetProperName(), formattedByTag, formattedByTag2));
			list.Add(new Descriptor(text, text, Descriptor.DescriptorType.Requirement, false));
		}
		return list;
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.isEditing)
		{
			e.Consumed = true;
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	public Image recipeIcon;

	public LocText recipeName;

	public DescriptorPanel IngredientsDescriptorPanel;

	public DescriptorPanel EffectsDescriptorPanel;

	public KNumberInputField QueueCount;

	public MultiToggle DecrementButton;

	public MultiToggle IncrementButton;

	public KButton InfiniteButton;

	public GameObject InfiniteIcon;

	private ComplexFabricator target;

	private ComplexFabricatorSideScreen ownerScreen;

	private ComplexRecipe selectedRecipe;

	private bool isEditing;
}
