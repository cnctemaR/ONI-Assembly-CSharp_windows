using System;
using Database;
using UnityEngine;

public class KleiPermitDioramaVis_BuildingOnBackground : KMonoBehaviour, IKleiPermitDioramaVisTarget
{
	public void ConfigureSetup()
	{
		this.buildingKAnimPrefab.gameObject.SetActive(false);
		this.buildingKAnimArray = new KBatchedAnimController[9];
		for (int i = 0; i < this.buildingKAnimArray.Length; i++)
		{
			this.buildingKAnimArray[i] = (KBatchedAnimController)global::UnityEngine.Object.Instantiate(this.buildingKAnimPrefab, this.buildingKAnimPrefab.transform.parent, false);
		}
		Vector2 anchoredPosition = this.buildingKAnimPrefab.rectTransform().anchoredPosition;
		Vector2 vector = 175f * Vector2.one;
		Vector2 vector2 = anchoredPosition + vector * new Vector2(-1f, 0f);
		int num = 0;
		for (int j = 0; j < 3; j++)
		{
			int k = 0;
			while (k < 3)
			{
				this.buildingKAnimArray[num].rectTransform().anchoredPosition = vector2 + vector * new Vector2((float)j, (float)k);
				this.buildingKAnimArray[num].gameObject.SetActive(true);
				k++;
				num++;
			}
		}
	}

	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	public void ConfigureWith(PermitResource permit, PermitPresentationInfo permitPresInfo)
	{
		BuildingFacadeResource buildingFacadeResource = (BuildingFacadeResource)permit;
		BuildingDef value = KleiPermitVisUtil.GetBuildingDef(permit).Value;
		DebugUtil.DevAssert(value.WidthInCells == 1, "assert failed", null);
		DebugUtil.DevAssert(value.HeightInCells == 1, "assert failed", null);
		KBatchedAnimController[] array = this.buildingKAnimArray;
		for (int i = 0; i < array.Length; i++)
		{
			KleiPermitVisUtil.ConfigureToRenderBuilding(array[i], buildingFacadeResource);
		}
	}

	[SerializeField]
	private KBatchedAnimController buildingKAnimPrefab;

	private KBatchedAnimController[] buildingKAnimArray;
}
