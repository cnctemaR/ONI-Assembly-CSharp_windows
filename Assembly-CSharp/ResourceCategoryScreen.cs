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
		this.CreateTagSetHeaders(GameTags.MaterialCategories, ResourceCategoryHeader.MeasureUnit.mass);
		this.CreateTagSetHeaders(GameTags.CalorieCategories, ResourceCategoryHeader.MeasureUnit.kcal);
		this.CreateTagSetHeaders(GameTags.UnitCategories, ResourceCategoryHeader.MeasureUnit.quantity);
		if (!this.DisplayedCategories.ContainsKey(GameTags.Miscellaneous))
		{
			ResourceCategoryHeader resourceCategoryHeader = this.NewCategoryHeader(GameTags.Miscellaneous);
			this.DisplayedCategories.Add(GameTags.Miscellaneous, resourceCategoryHeader);
			resourceCategoryHeader.gameObject.SetActive(false);
		}
	}

	private void CreateTagSetHeaders(IEnumerable<Tag> set, ResourceCategoryHeader.MeasureUnit measure)
	{
		foreach (Tag tag in set)
		{
			if (!this.Filter(tag))
			{
				ResourceCategoryHeader resourceCategoryHeader = this.NewCategoryHeader(tag);
				resourceCategoryHeader.measure = measure;
				this.DisplayedCategories.Add(tag, resourceCategoryHeader);
				resourceCategoryHeader.gameObject.SetActive(false);
			}
		}
	}

	private void Update()
	{
		if (WorldInventory.Instance == null)
		{
			return;
		}
		for (int i = 0; i < 1; i++)
		{
			Tag tag = this.DisplayedCategories.Keys.ElementAt<Tag>(this.categoryUpdatePacer);
			if (WorldInventory.Instance.IsDiscovered(tag) && !this.DisplayedCategories[tag].gameObject.activeInHierarchy)
			{
				this.DisplayedCategories[tag].gameObject.SetActive(true);
			}
			this.DisplayedCategories[tag].UpdateContents();
			this.categoryUpdatePacer = (this.categoryUpdatePacer + 1) % this.DisplayedCategories.Keys.Count;
		}
		if (MeterScreen.Instance != null && !MeterScreen.Instance.StartValuesSet)
		{
			MeterScreen.Instance.InitializeValues();
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

	private int categoryUpdatePacer;
}
