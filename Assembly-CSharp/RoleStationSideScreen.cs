using System;
using UnityEngine;

public class RoleStationSideScreen : SideScreenContent
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.openRolesScreenButton.onClick += delegate
		{
			ManagementMenu.Instance.ToggleRoles();
		};
	}

	public override bool IsValidForTarget(GameObject target)
	{
		return target.GetComponent<RoleStation>() != null;
	}

	public KButton openRolesScreenButton;

	public GameObject content;

	private GameObject target;

	public LocText DescriptionText;
}
