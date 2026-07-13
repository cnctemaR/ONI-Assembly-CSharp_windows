using System;
using Database;
using UnityEngine;

public class KleiPermitDioramaVis_ArtablePainting : KMonoBehaviour, IKleiPermitDioramaVisTarget
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
		ArtableStage artableStage = (ArtableStage)permit;
		KleiPermitVisUtil.ConfigureToRenderBuilding(this.buildingKAnim, artableStage);
		BuildingDef buildingDef = KleiPermitVisUtil.GetBuildingDef(permit);
		this.buildingKAnimPosition.SetOn(this.buildingKAnim);
		this.buildingKAnim.rectTransform().anchoredPosition += new Vector2(0f, -176f * (float)buildingDef.HeightInCells / 2f + 176f);
		this.buildingKAnim.rectTransform().localScale = Vector3.one * 0.9f;
		KleiPermitVisUtil.AnimateIn(this.buildingKAnim, default(Updater), "place");
	}

	[SerializeField]
	private KBatchedAnimController buildingKAnim;

	private PrefabDefinedUIPosition buildingKAnimPosition = new PrefabDefinedUIPosition();
}
