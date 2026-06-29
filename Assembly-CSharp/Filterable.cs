using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using KSerialization;

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
		IEnumerator enumerator = Enum.GetValues(typeof(SimHashes)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object obj = enumerator.Current;
				SimHashes simHashes = (SimHashes)obj;
				bool flag = true;
				if (this.filterElementState != Filterable.ElementState.None)
				{
					Element element = ElementLoader.FindElementByHash(simHashes);
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
					Tag tag = GameTagExtensions.Create(simHashes);
					list.Add(tag);
				}
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

	[Serialize]
	public Filterable.ElementState filterElementState;

	[Serialize]
	private Tag selectedTag;

	private static Operational.Flag filterSelected = new Operational.Flag("filterSelected", Operational.Flag.Type.Requirement);

	public enum ElementState
	{
		None,
		Solid,
		Liquid,
		Gas
	}
}
