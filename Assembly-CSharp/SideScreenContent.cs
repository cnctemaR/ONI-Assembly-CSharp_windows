using System;
using UnityEngine;

public class SideScreenContent : KScreen
{
	public virtual void SetTarget(GameObject target)
	{
	}

	public virtual void ClearTarget()
	{
	}

	public virtual string GetTitle()
	{
		return Strings.Get(this.titleKey);
	}

	[SerializeField]
	protected string titleKey;

	public GameObject ContentContainer;
}
