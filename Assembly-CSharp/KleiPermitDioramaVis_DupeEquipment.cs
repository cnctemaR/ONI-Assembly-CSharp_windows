using System;
using Database;
using UnityEngine;

public class KleiPermitDioramaVis_DupeEquipment : KMonoBehaviour, IKleiPermitDioramaVisTarget
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
		ClothingItemResource clothingItemResource = permit as ClothingItemResource;
		if (clothingItemResource != null)
		{
			this.uiMannequin.SetOutfit(new ClothingItemResource[] { clothingItemResource });
			this.uiMannequin.ReactToClothingItemChange(clothingItemResource.Category);
		}
	}

	[SerializeField]
	private UIMannequin uiMannequin;
}
