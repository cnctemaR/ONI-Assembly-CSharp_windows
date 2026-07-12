using System;
using Database;
using UnityEngine;
using UnityEngine.UI;

public class KleiPermitDioramaVis_DupeEquipment : KMonoBehaviour, IKleiPermitDioramaVisTarget
{
	public GameObject GetGameObject()
	{
		return base.gameObject;
	}

	public void ConfigureSetup()
	{
		this.uiMannequin.shouldShowOutfitWithDefaultItems = false;
	}

	public void ConfigureWith(PermitResource permit)
	{
		ClothingItemResource clothingItemResource = permit as ClothingItemResource;
		if (clothingItemResource != null)
		{
			this.uiMannequin.SetOutfit(clothingItemResource.outfitType, new ClothingItemResource[] { clothingItemResource });
			this.uiMannequin.ReactToClothingItemChange(clothingItemResource.Category);
		}
		this.dioramaBGImage.sprite = KleiPermitDioramaVis.GetDioramaBackground(permit.Category);
	}

	[SerializeField]
	private UIMannequin uiMannequin;

	[Header("Diorama Backgrounds")]
	[SerializeField]
	private Image dioramaBGImage;

	[SerializeField]
	private Sprite clothingBG;

	[SerializeField]
	private Sprite atmosuitBG;
}
