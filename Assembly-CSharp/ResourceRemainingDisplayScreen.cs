using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceRemainingDisplayScreen : KScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Activate();
		ResourceRemainingDisplayScreen.instance = this;
		this.dispayPrefab.SetActive(false);
	}

	public void ActivateDisplay(GameObject target)
	{
		this.numberOfPendingConstructions = 0;
		this.dispayPrefab.SetActive(true);
	}

	public void DeactivateDisplay()
	{
		this.dispayPrefab.SetActive(false);
	}

	public void SetResources(IList<Element> _selected_elements, Recipe recipe)
	{
		this.selected_elements.Clear();
		foreach (Element element in _selected_elements)
		{
			this.selected_elements.Add(element);
		}
		this.currentRecipe = recipe;
		Debug.Assert(this.selected_elements.Count == recipe.Ingredients.Count);
	}

	public void SetNumberOfPendingConstructions(int number)
	{
		this.numberOfPendingConstructions = number;
	}

	public void Update()
	{
		if (!this.dispayPrefab.activeSelf)
		{
			return;
		}
		if (base.canvas != null)
		{
			if (this.rect == null)
			{
				this.rect = base.GetComponent<RectTransform>();
			}
			this.rect.anchoredPosition = base.WorldToScreen(PlayerController.GetCursorPos(Input.mousePosition));
		}
		if (this.displayedConstructionCostMultiplier == this.numberOfPendingConstructions)
		{
			this.label.text = string.Empty;
		}
		else
		{
			this.displayedConstructionCostMultiplier = this.numberOfPendingConstructions;
		}
	}

	public string GetString()
	{
		string text = string.Empty;
		if (this.selected_elements != null && this.currentRecipe != null)
		{
			for (int i = 0; i < this.currentRecipe.Ingredients.Count; i++)
			{
				Element element = this.selected_elements[i];
				Tag tag = TagManager.Create(element.id);
				float num = this.currentRecipe.Ingredients[i].amount * (float)this.numberOfPendingConstructions;
				float num2 = WorldInventory.Instance.GetTotalAmount(tag) - WorldInventory.Instance.GetAmount(tag);
				float num3 = WorldInventory.Instance.GetTotalAmount(tag) - (num2 + num);
				if (num3 < 0f)
				{
					num3 = 0f;
				}
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					tag.ProperName(),
					": ",
					GameUtil.GetFormattedMass(num3, GameUtil.TimeSlice.None, true, "F1"),
					" / ",
					GameUtil.GetFormattedMass(this.currentRecipe.Ingredients[i].amount, GameUtil.TimeSlice.None, true, "F1")
				});
				if (i < this.selected_elements.Count - 1)
				{
					text += "\n";
				}
			}
		}
		return text;
	}

	public static ResourceRemainingDisplayScreen instance;

	public GameObject dispayPrefab;

	public LocText label;

	private Recipe currentRecipe;

	private List<Element> selected_elements = new List<Element>();

	private int numberOfPendingConstructions;

	private int displayedConstructionCostMultiplier;

	private RectTransform rect;
}
