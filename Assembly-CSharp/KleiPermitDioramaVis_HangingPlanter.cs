using System;
using Database;
using UnityEngine;

public class KleiPermitDioramaVis_HangingPlanter : KMonoBehaviour, IKleiPermitDioramaVisTarget
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
		KleiPermitVisUtil.ConfigureToRenderBuilding(this.planterKAnim, (BuildingFacadeResource)permit);
	}

	[SerializeField]
	private KBatchedAnimController planterKAnim;

	[SerializeField]
	private KBatchedAnimController hookKAnim;
}
