using System;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/CopyBuildingSettings")]
public class CopyBuildingSettings : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe<CopyBuildingSettings>(493375141, CopyBuildingSettings.OnRefreshUserMenuDelegate);
	}

	private void OnRefreshUserMenu(object data)
	{
		Game.Instance.userMenu.AddButton(base.gameObject, new KIconButtonMenu.ButtonInfo("action_mirror", UI.USERMENUACTIONS.COPY_BUILDING_SETTINGS.NAME, new global::System.Action(this.ActivateCopyTool), global::Action.BuildingUtility1, null, null, null, UI.USERMENUACTIONS.COPY_BUILDING_SETTINGS.TOOLTIP, true), 1f);
	}

	private void ActivateCopyTool()
	{
		CopySettingsTool.Instance.SetSourceObject(base.gameObject);
		PlayerController.Instance.ActivateTool(CopySettingsTool.Instance);
	}

	public static ObjectLayer ResolveLayer(GameObject sourceGameObject)
	{
		ObjectLayer objectLayer = ObjectLayer.Building;
		MoverLayerOccupier moverLayerOccupier;
		if (sourceGameObject.TryGetComponent<MoverLayerOccupier>(out moverLayerOccupier))
		{
			objectLayer = ObjectLayer.Mover;
		}
		BuildingComplete buildingComplete;
		if (sourceGameObject.TryGetComponent<BuildingComplete>(out buildingComplete))
		{
			objectLayer = buildingComplete.Def.ObjectLayer;
		}
		return objectLayer;
	}

	public static KPrefabID ResolveTarget(ObjectLayer layer, int targetCell)
	{
		GameObject gameObject = Grid.Objects[targetCell, (int)layer];
		if (gameObject == null)
		{
			return null;
		}
		KPrefabID kprefabID;
		gameObject.TryGetComponent<KPrefabID>(out kprefabID);
		return kprefabID;
	}

	public static bool ApplyCopy(KPrefabID other_id, GameObject sourceGameObject, KPrefabID source_id, CopyBuildingSettings source_settings)
	{
		DebugUtil.DevAssert(other_id.gameObject != sourceGameObject, "source and target must not be equal", null);
		if (other_id.gameObject == sourceGameObject)
		{
			return false;
		}
		CopyBuildingSettings copyBuildingSettings;
		if (!other_id.gameObject.TryGetComponent<CopyBuildingSettings>(out copyBuildingSettings))
		{
			return false;
		}
		if (source_settings.copyGroupTag != Tag.Invalid)
		{
			if (source_settings.copyGroupTag != copyBuildingSettings.copyGroupTag)
			{
				return false;
			}
		}
		else if (other_id.PrefabID() != source_id.PrefabID())
		{
			return false;
		}
		other_id.Trigger(-905833192, sourceGameObject);
		PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, UI.COPIED_SETTINGS, other_id.gameObject.transform, new Vector3(0f, 0.5f, 0f), 1.5f, false, false);
		return true;
	}

	[MyCmpReq]
	private KPrefabID id;

	public Tag copyGroupTag;

	private static readonly EventSystem.IntraObjectHandler<CopyBuildingSettings> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<CopyBuildingSettings>(delegate(CopyBuildingSettings component, object data)
	{
		component.OnRefreshUserMenu(data);
	});
}
