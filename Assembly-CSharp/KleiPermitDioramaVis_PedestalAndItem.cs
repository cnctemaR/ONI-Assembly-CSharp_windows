using System;
using Database;
using UnityEngine;
using UnityEngine.UI;

public class KleiPermitDioramaVis_PedestalAndItem : KMonoBehaviour, IKleiPermitDioramaVisTarget
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
		PermitPresentationInfo permitPresentationInfo = permit.GetPermitPresentationInfo();
		RectTransform rectTransform = this.pedestalKAnim.rectTransform();
		RectTransform rectTransform2 = this.itemSprite.rectTransform();
		rectTransform.pivot = new Vector2(0.5f, 0f);
		KleiPermitVisUtil.ConfigureToRenderBuilding(this.pedestalKAnim, Assets.GetBuildingDef("ItemPedestal"));
		rectTransform2.pivot = new Vector2(0.5f, 0f);
		rectTransform2.anchoredPosition = rectTransform.anchoredPosition + Vector2.up * 0.79f * 176f;
		rectTransform2.sizeDelta = Vector2.one * 176f;
		this.itemSprite.sprite = permitPresentationInfo.sprite;
	}

	[SerializeField]
	private KBatchedAnimController pedestalKAnim;

	[SerializeField]
	private Image itemSprite;

	private const float TILE_COUNT_TO_PEDESTAL_SLOT = 0.79f;
}
