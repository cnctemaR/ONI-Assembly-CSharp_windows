using System;
using System.Collections.Generic;
using System.Diagnostics;
using KSerialization;
using UnityEngine;

public class Filterable : KMonoBehaviour
{
	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
		foreach (Element element in ElementLoader.elements)
		{
			bool flag = true;
			if (this.filterElementState != Filterable.ElementState.None)
			{
				Filterable.ElementState elementState = this.filterElementState;
				if (elementState != Filterable.ElementState.Gas)
				{
					if (elementState != Filterable.ElementState.Liquid)
					{
						if (elementState == Filterable.ElementState.Solid)
						{
							flag = element.IsSolid;
						}
					}
					else
					{
						flag = element.IsLiquid;
					}
				}
				else
				{
					flag = element.IsGas;
				}
			}
			if (flag)
			{
				Tag tag = GameTagExtensions.Create(element.id);
				list.Add(tag);
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
		GameObject gameObject = (GameObject)data;
		Filterable component = gameObject.GetComponent<Filterable>();
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
	private Tag selectedTag;

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
