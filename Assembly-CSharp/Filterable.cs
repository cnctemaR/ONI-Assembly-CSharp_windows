using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using KSerialization;

public class Filterable : KMonoBehaviour
{
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

	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
	public event Action<Tag> onFilterChanged;

	protected virtual IList<Tag> GetTagOptions()
	{
		List<Tag> list = new List<Tag>();
		IEnumerator enumerator = Enum.GetValues(typeof(SimHashes)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				SimHashes simHashes = (SimHashes)obj;
				Tag tag = GameTagExtensions.Create(simHashes);
				list.Add(tag);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = enumerator as IDisposable) != null)
			{
				disposable.Dispose();
			}
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
