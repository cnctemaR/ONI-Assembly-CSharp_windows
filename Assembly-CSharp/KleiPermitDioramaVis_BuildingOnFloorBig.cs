using System;
using Database;
using UnityEngine;

public class KleiPermitDioramaVis_BuildingOnFloorBig : KMonoBehaviour, IKleiPermitDioramaVisTarget
{
	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	public void ConfigureSetup()
	{
		this.defaultAnchoredPosition = this.buildingKAnim.rectTransform().anchoredPosition;
	}

	public void ConfigureWith(PermitResource permit)
	{
		BuildingFacadeResource buildingFacadeResource = (BuildingFacadeResource)permit;
		this.buildingKAnim.SetSymbolVisiblity("booster", false);
		this.buildingKAnim.SetSymbolVisiblity("blue_light_bloom", false);
		this.buildingKAnim.rectTransform().anchoredPosition = this.defaultAnchoredPosition;
		this.buildingKAnim.rectTransform().localScale = Vector3.one * 0.825f;
		string text = "place";
		if (buildingFacadeResource.PrefabID == "SteamTurbine2")
		{
			this.buildingKAnim.rectTransform().anchoredPosition += new Vector2(0f, 140f);
			text = "place_alt";
		}
		KleiPermitVisUtil.ConfigureToRenderBuilding(this.buildingKAnim, buildingFacadeResource);
		KleiPermitVisUtil.AnimateIn(this.buildingKAnim, default(Updater), text);
	}

	[SerializeField]
	private KBatchedAnimController buildingKAnim;

	private Vector2 defaultAnchoredPosition;
}
