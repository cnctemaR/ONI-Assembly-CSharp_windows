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

	public KButton openRolesScreenButton;

	public GameObject content;

	private GameObject target;

	public LocText DescriptionText;
}
