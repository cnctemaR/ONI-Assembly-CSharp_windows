using System;
using Database;
using UnityEngine;

public class KleiPermitDioramaVis_BuildingRocket : KMonoBehaviour, IKleiPermitDioramaVisTarget
{
	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	public void ConfigureSetup()
	{
	}

	public void ConfigureWith(PermitResource permit)
	{
		BuildingFacadeResource buildingFacadeResource = (BuildingFacadeResource)permit;
		KleiPermitVisUtil.ConfigureToRenderBuilding(this.buildingKAnim, buildingFacadeResource);
		KleiPermitVisUtil.AnimateIn(this.buildingKAnim, default(Updater), "place");
	}

	[SerializeField]
	private KBatchedAnimController buildingKAnim;
}
