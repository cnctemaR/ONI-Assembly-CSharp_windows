using System;
using UnityEngine;

public class SideScreenContent : KScreen
{
	public virtual void SetTarget(GameObject target)
	{
	}

	public virtual string GetTitle()
	{
		return Strings.Get(this.titleKey);
	}

	[SerializeField]
	protected string titleKey;
}
