using System;
using Database;
using UnityEngine;

public class KleiPermitDioramaVis_BuildingOnWall : KMonoBehaviour, IKleiPermitDioramaVisTarget
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
	}

	[SerializeField]
	private KBatchedAnimController buildingKAnim;
}
