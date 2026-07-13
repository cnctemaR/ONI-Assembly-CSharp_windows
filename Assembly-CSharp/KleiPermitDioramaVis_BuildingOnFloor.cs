using System;
using Database;
using UnityEngine;

public class KleiPermitDioramaVis_BuildingOnFloor : KMonoBehaviour, IKleiPermitDioramaVisTarget
{
	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	public void ConfigureSetup()
	{
		this.rectTransform = this.buildingKAnim.rectTransform();
		this.defaultScale = this.rectTransform.localScale;
	}

	public void ConfigureWith(PermitResource permit)
	{
		BuildingFacadeResource buildingFacadeResource = (BuildingFacadeResource)permit;
		string text = "place";
		this.buildingKAnim.SetSymbolVisiblity("sweep", false);
		if (buildingFacadeResource.PrefabID == "LiquidPumpingStation")
		{
			this.rectTransform.localScale = Vector3.one * 0.7f;
			this.buildingKAnim.SetSymbolVisiblity("pipe2", false);
			this.buildingKAnim.SetSymbolVisiblity("pipe3", false);
			this.buildingKAnim.SetSymbolVisiblity("pipe4", false);
			text = "place_alt";
		}
		else
		{
			this.rectTransform.localScale = this.defaultScale;
		}
		KleiPermitVisUtil.ConfigureToRenderBuilding(this.buildingKAnim, buildingFacadeResource);
		KleiPermitVisUtil.AnimateIn(this.buildingKAnim, default(Updater), text);
	}

	[SerializeField]
	private KBatchedAnimController buildingKAnim;

	private Vector2 defaultScale;

	private RectTransform rectTransform;
}
