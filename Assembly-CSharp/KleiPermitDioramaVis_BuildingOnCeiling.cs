using System;
using Database;
using UnityEngine;

public class KleiPermitDioramaVis_BuildingOnCeiling : KMonoBehaviour, IKleiPermitDioramaVisTarget
{
	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	public void ConfigureSetup()
	{
	}

	public void ConfigureWith(PermitResource permit, PermitPresentationInfo permitPresInfo)
	{
		BuildingFacadeResource buildingFacadeResource = (BuildingFacadeResource)permit;
		KleiPermitVisUtil.ConfigureToRenderBuilding(this.buildingKAnim, buildingFacadeResource);
		BuildingDef value = KleiPermitVisUtil.GetBuildingDef(permit).Value;
		this.buildingKAnimPosition.SetOn(this.buildingKAnim);
		this.buildingKAnim.rectTransform().anchoredPosition += new Vector2(0f, -176f * (float)value.HeightInCells + 176f);
	}

	[SerializeField]
	private KBatchedAnimController buildingKAnim;

	private PrefabDefinedUIPosition buildingKAnimPosition = new PrefabDefinedUIPosition();
}
