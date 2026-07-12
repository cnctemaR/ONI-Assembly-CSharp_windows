using System;
using UnityEngine;

public abstract class SideScreenContent : KScreen
{
	public virtual void SetTarget(GameObject target)
	{
	}

	public virtual void ClearTarget()
	{
	}

	public abstract bool IsValidForTarget(GameObject target);

	public virtual int GetSideScreenSortOrder()
	{
		return 0;
	}

	public virtual string GetTitle()
	{
		return Strings.Get(this.titleKey);
	}

	[SerializeField]
	protected string titleKey;

	public GameObject ContentContainer;
}
