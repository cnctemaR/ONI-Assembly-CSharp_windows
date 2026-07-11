using System;
using STRINGS;
using UnityEngine;

public class CopyBuildingSettings : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<CopyBuildingSettings>(493375141, CopyBuildingSettings.OnRefreshUserMenuDelegate);
	}

	private void OnRefreshUserMenu(object data)
	{
		UserMenu userMenu = Game.Instance.userMenu;
		GameObject gameObject = base.gameObject;
		string text = "action_mirror";
		string text2 = UI.USERMENUACTIONS.COPY_BUILDING_SETTINGS.NAME;
		global::System.Action action = new global::System.Action(this.ActivateCopyTool);
		global::Action action2 = global::Action.BuildingUtility1;
		string text3 = UI.USERMENUACTIONS.COPY_BUILDING_SETTINGS.TOOLTIP;
		userMenu.AddButton(gameObject, new KIconButtonMenu.ButtonInfo(text, text2, action, action2, null, null, null, text3, true), 1f);
	}

	private void ActivateCopyTool()
	{
		CopySettingsTool.Instance.SetSourceObject(base.gameObject);
		PlayerController.Instance.ActivateTool(CopySettingsTool.Instance);
	}

	public static bool ApplyCopy(int targetCell, GameObject sourceGameObject)
	{
		GameObject gameObject = Grid.Objects[targetCell, 1];
		if (gameObject == null)
		{
			return false;
		}
		KPrefabID component = sourceGameObject.GetComponent<KPrefabID>();
		if (component == null)
		{
			return false;
		}
		KPrefabID component2 = gameObject.GetComponent<KPrefabID>();
		if (component2 == null)
		{
			return false;
		}
		if (component2.PrefabID() != component.PrefabID())
		{
			return false;
		}
		component2.Trigger(-905833192, sourceGameObject);
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, UI.COPIED_SETTINGS, gameObject.transform, new Vector3(0f, 0.5f, 0f), 1.5f, false, false);
		return true;
	}

	[MyCmpReq]
	private KPrefabID id;

	private static readonly EventSystem.IntraObjectHandler<CopyBuildingSettings> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<CopyBuildingSettings>(delegate(CopyBuildingSettings component, object data)
	{
		component.OnRefreshUserMenu(data);
	});
}
