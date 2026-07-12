using System;
using Database;
using UnityEngine;

public class KleiPermitDioramaVis_ArtableSticker : KMonoBehaviour, IKleiPermitDioramaVisTarget
{
	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	public void ConfigureSetup()
	{
		SymbolOverrideControllerUtil.AddToPrefab(this.buildingKAnim.gameObject);
	}

	public void ConfigureWith(PermitResource permit)
	{
		DbStickerBomb dbStickerBomb = (DbStickerBomb)permit;
		KleiPermitVisUtil.ConfigureToRenderBuilding(this.buildingKAnim, dbStickerBomb);
	}

	[SerializeField]
	private KBatchedAnimController buildingKAnim;
}
