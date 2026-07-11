using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Filterable")]
public class Filterable : KMonoBehaviour
{
	public event Action<Tag> onFilterChanged;

	public Tag SelectedTag
	{
		get
		{
			return this.selectedTag;
		}
		set
		{
			this.selectedTag = value;
			this.OnFilterChanged();
		}
	}

	public Dictionary<Tag, HashSet<Tag>> GetTagOptions()
	{
		Dictionary<Tag, HashSet<Tag>> dictionary = new Dictionary<Tag, HashSet<Tag>>();
		if (this.filterElementState == Filterable.ElementState.Solid)
		{
			dictionary = WorldInventory.Instance.GetDiscoveredResourcesFromTagSet(Filterable.filterableCategories);
		}
		else
		{
			foreach (Element element in ElementLoader.elements)
			{
				if (!element.disabled && ((element.IsGas && this.filterElementState == Filterable.ElementState.Gas) || (element.IsLiquid && this.filterElementState == Filterable.ElementState.Liquid)))
				{
					Tag materialCategoryTag = element.GetMaterialCategoryTag();
					if (!dictionary.ContainsKey(materialCategoryTag))
					{
						dictionary[materialCategoryTag] = new HashSet<Tag>();
					}
					Tag tag = GameTagExtensions.Create(element.id);
					dictionary[materialCategoryTag].Add(tag);
				}
			}
		}
		dictionary.Add(GameTags.Void, new HashSet<Tag> { GameTags.Void });
		return dictionary;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<Filterable>(-905833192, Filterable.OnCopySettingsDelegate);
	}

	private void OnCopySettings(object data)
	{
		Filterable component = ((GameObject)data).GetComponent<Filterable>();
		if (component != null)
		{
			this.SelectedTag = component.SelectedTag;
		}
	}

	protected override void OnSpawn()
	{
		this.OnFilterChanged();
	}

	private void OnFilterChanged()
	{
		if (this.onFilterChanged != null)
		{
			this.onFilterChanged(this.selectedTag);
		}
		Operational component = base.GetComponent<Operational>();
		if (component != null)
		{
			component.SetFlag(Filterable.filterSelected, this.selectedTag.IsValid);
		}
	}

	[MyCmpAdd]
	private CopyBuildingSettings copyBuildingSettings;

	[Serialize]
	public Filterable.ElementState filterElementState;

	[Serialize]
	private Tag selectedTag = GameTags.Void;

	private static TagSet filterableCategories = new TagSet(new TagSet[]
	{
		GameTags.CalorieCategories,
		GameTags.UnitCategories,
		GameTags.MaterialCategories,
		GameTags.MaterialBuildingElements
	});

	private static readonly Operational.Flag filterSelected = new Operational.Flag("filterSelected", Operational.Flag.Type.Requirement);

	private static readonly EventSystem.IntraObjectHandler<Filterable> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Filterable>(delegate(Filterable component, object data)
	{
		component.OnCopySettings(data);
	});

	public enum ElementState
	{
		None,
		Solid,
		Liquid,
		Gas
	}
}
