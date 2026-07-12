using System;
using System.Collections.Generic;
using Database;
using UnityEngine;
using UnityEngine.UI;

public class KleiPermitDioramaVis_AutomationGates : KMonoBehaviour, IKleiPermitDioramaVisTarget
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
		this.itemSprite.gameObject.SetActive(false);
		BuildingFacadeResource buildingFacadeResource = (BuildingFacadeResource)permit;
		KleiPermitVisUtil.ConfigureToRenderBuilding(this.buildingKAnim, buildingFacadeResource);
		BuildingDef buildingDef = KleiPermitVisUtil.GetBuildingDef(permit);
		Dictionary<int, float> dictionary = new Dictionary<int, float>
		{
			{ 3, 0.7f },
			{ 2, 0.9f },
			{ 1, 0.85f }
		};
		Dictionary<int, float> dictionary2 = new Dictionary<int, float>
		{
			{ 4, 32f },
			{ 3, 32f },
			{ 2, 32f },
			{ 1, 96f }
		};
		this.buildingKAnimPosition.SetOn(this.buildingKAnim);
		this.buildingKAnim.rectTransform().localScale = Vector3.one * dictionary[buildingDef.WidthInCells];
		this.buildingKAnim.rectTransform().anchoredPosition += new Vector2(0f, dictionary2[buildingDef.HeightInCells]);
		KleiPermitVisUtil.AnimateIn(this.buildingKAnim, default(Updater));
	}

	[SerializeField]
	private Image itemSprite;

	[SerializeField]
	private KBatchedAnimController buildingKAnim;

	private PrefabDefinedUIPosition buildingKAnimPosition = new PrefabDefinedUIPosition();
}
