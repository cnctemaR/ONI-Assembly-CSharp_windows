using System;
using STRINGS;

public static class MinionIdentitySort
{
	public static int CompareByName(MinionIdentity a, MinionIdentity b)
	{
		return a.GetProperName().CompareTo(b.GetProperName());
	}

	public static int CompareByRole(MinionIdentity a, MinionIdentity b)
	{
		ChoreConsumer component = a.GetComponent<ChoreConsumer>();
		ChoreConsumer component2 = b.GetComponent<ChoreConsumer>();
		return component.resume.CurrentRole.CompareTo(component2.resume.CurrentRole);
	}

	// Note: this type is marked as 'beforefieldinit'.
	static MinionIdentitySort()
	{
		MinionIdentitySort.SortInfo[] array = new MinionIdentitySort.SortInfo[2];
		int num = 0;
		MinionIdentitySort.SortInfo sortInfo = new MinionIdentitySort.SortInfo();
		sortInfo.name = UI.MINION_IDENTITY_SORT.NAME;
		sortInfo.compare = new Comparison<MinionIdentity>(MinionIdentitySort.CompareByName);
		array[num] = sortInfo;
		int num2 = 1;
		sortInfo = new MinionIdentitySort.SortInfo();
		sortInfo.name = UI.MINION_IDENTITY_SORT.ROLE;
		sortInfo.compare = new Comparison<MinionIdentity>(MinionIdentitySort.CompareByRole);
		array[num2] = sortInfo;
		MinionIdentitySort.SortInfos = array;
	}

	public static readonly MinionIdentitySort.SortInfo[] SortInfos;

	public class SortInfo : IListableOption
	{
		public string GetProperName()
		{
			return this.name;
		}

		public LocString name;

		public Comparison<MinionIdentity> compare;
	}
}
