using System;
using System.Collections.Generic;
using KSerialization;

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

	protected virtual IList<Tag> GetTagOptions()
	{
		List<Tag> list = new List<Tag>();
		foreach (object obj in Enum.GetValues(typeof(SimHashes)))
		{
			SimHashes simHashes = (SimHashes)((int)obj);
			Tag tag = TagManager.Create(simHashes);
			list.Add(tag);
		}
		return list;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.selectedTag = this.defaultValue;
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
		if (this.operational != null)
		{
			this.operational.SetFlag(Filterable.filterSelected, this.selectedTag != GameTags.Void);
		}
	}

	private Tag defaultValue = GameTags.Void;

	[Serialize]
	private Tag selectedTag;

	private static Operational.Flag filterSelected = new Operational.Flag("filterSelected", Operational.Flag.Type.Requirement);

	[MyCmpGet]
	private Operational operational;
}
