using System;
using System.Collections.Generic;
using KSerialization;
using UnityEngine;

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

	public virtual IList<Tag> GetTagOptions()
	{
		List<Tag> list = new List<Tag>();
		list.Add(GameTags.Void);
		foreach (Element element in ElementLoader.elements)
		{
			if (!element.disabled)
			{
				bool flag = true;
				if (this.filterElementState != Filterable.ElementState.None)
				{
					switch (this.filterElementState)
					{
					case Filterable.ElementState.Solid:
						flag = element.IsSolid;
						break;
					case Filterable.ElementState.Liquid:
						flag = element.IsLiquid;
						break;
					case Filterable.ElementState.Gas:
						flag = element.IsGas;
						break;
					}
				}
				if (flag)
				{
					Tag tag = GameTagExtensions.Create(element.id);
					list.Add(tag);
				}
			}
		}
		return list;
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
