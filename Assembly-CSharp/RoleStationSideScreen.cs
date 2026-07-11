using System;
using UnityEngine;

public class RoleStationSideScreen : SideScreenContent
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return false;
	}

	public GameObject content;

	private GameObject target;

	public LocText DescriptionText;
}
