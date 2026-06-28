using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceCategoryScreen : KScreen
{
	public bool Filter(Tag tag)
	{
		return tag == new Tag("Liquifiable");
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		ResourceCategoryScreen.Instance = this;
		this.NonOreCategoryTags.Add(this.KCalTags);
		this.NonOreCategoryTags.Add(this.QuantityTags);
		foreach (Tag tag in Element.GetMaterialCategoryTags())
		{
			if (!this.Filter(tag))
			{
				ResourceCategoryHeader resourceCategoryHeader = this.NewCategoryHeader(tag);
				this.DisplayedCategories.Add(tag, resourceCategoryHeader);
				resourceCategoryHeader.gameObject.SetActive(false);
			}
		}
		if (!this.DisplayedCategories.ContainsKey(GameTags.Miscellaneous))
		{
			ResourceCategoryHeader resourceCategoryHeader2 = this.NewCategoryHeader(GameTags.Miscellaneous);
			this.DisplayedCategories.Add(GameTags.Miscellaneous, resourceCategoryHeader2);
			resourceCategoryHeader2.gameObject.SetActive(false);
		}
		this.CreateTagSetHeaders(this.KCalTags, ResourceCategoryHeader.MeasureUnit.kcal);
		this.CreateTagSetHeaders(this.QuantityTags, ResourceCategoryHeader.MeasureUnit.quantity);
	}

	private void CreateTagSetHeaders(Tag[] set, ResourceCategoryHeader.MeasureUnit measure)
	{
		foreach (Tag tag in set)
		{
			if (!this.Filter(tag))
			{
				ResourceCategoryHeader resourceCategoryHeader = this.NewCategoryHeader(tag);
				resourceCategoryHeader.isPickupableCategory = true;
				resourceCategoryHeader.measure = measure;
				this.DisplayedCategories.Add(tag, resourceCategoryHeader);
			}
		}
	}

	private void Update()
	{
		if (WorldInventory.Instance == null)
		{
			return;
		}
		this.RefreshCategories();
		for (int i = 0; i < 1; i++)
		{
			Tag tag = this.DisplayedCategories.Keys.ElementAt<Tag>(this.categoryUpdatePacer);
			ResourceCategoryHeader resourceCategoryHeader = this.DisplayedCategories[tag];
			resourceCategoryHeader.UpdateContents();
			this.categoryUpdatePacer = (this.categoryUpdatePacer + 1) % this.DisplayedCategories.Keys.Count;
		}
		foreach (Tag[] array in this.NonOreCategoryTags)
		{
			foreach (Tag tag2 in array)
			{
				this.DisplayedCategories[tag2].UpdateContents();
			}
		}
		if (MeterScreen.Instance != null && !MeterScreen.Instance.StartValuesSet)
		{
			MeterScreen.Instance.InitializeValues();
		}
	}

	public void RefreshCategories()
	{
		foreach (KeyValuePair<Tag, Tag> keyValuePair in WorldInventory.Instance.GetDiscoveredResourceTags())
		{
			if (!this.Filter(keyValuePair.Key))
			{
				if (!this.DisplayedCategories.ContainsKey(keyValuePair.Value))
				{
					this.DisplayedCategories.Add(keyValuePair.Value, this.NewCategoryHeader(keyValuePair.Value));
				}
				if (!this.DisplayedCategories[keyValuePair.Value].gameObject.activeInHierarchy)
				{
					this.DisplayedCategories[keyValuePair.Value].gameObject.SetActive(true);
				}
			}
		}
	}

	private ResourceCategoryHeader NewCategoryHeader(Tag categoryTag)
	{
		GameObject gameObject = Util.KInstantiateUI(this.Prefab_CategoryBar, this.CategoryContainer.gameObject, true);
		gameObject.name = "CategoryHeader_" + categoryTag.Name;
		ResourceCategoryHeader component = gameObject.GetComponent<ResourceCategoryHeader>();
		component.SetTag(categoryTag);
		return component;
	}

	public static ResourceCategoryScreen Instance;

	public GameObject Prefab_CategoryBar;

	public GameObject Prefab_MaterialDisplay;

	public Transform CategoryContainer;

	public Dictionary<Tag, ResourceCategoryHeader> DisplayedCategories = new Dictionary<Tag, ResourceCategoryHeader>();

	private List<Tag[]> NonOreCategoryTags = new List<Tag[]>();

	private Tag[] KCalTags = new Tag[] { GameTags.Edible };

	private Tag[] QuantityTags = new Tag[] { GameTags.Seed };

	private int categoryUpdatePacer;
}
