using System;
using Database;
using UnityEngine;
using UnityEngine.UI;

public class KleiPermitDioramaVis_WiresAndAutomation : KMonoBehaviour, IKleiPermitDioramaVisTarget
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
		this.itemSprite.sprite = permitPresentationInfo.sprite;
		if (!this.itemSpriteDidInit)
		{
			this.itemSpriteDidInit = true;
			this.itemSpritePosStart = this.itemSprite.rectTransform.anchoredPosition + new Vector2(0f, 16f);
			this.itemSpritePosEnd = this.itemSprite.rectTransform.anchoredPosition;
		}
		this.itemSprite.StartCoroutine(Updater.Parallel(new Updater[]
		{
			Updater.Ease(delegate(float alpha)
			{
				this.itemSprite.color = new Color(1f, 1f, 1f, alpha);
			}, 0f, 1f, 0.2f, Easing.SmoothStep, 0.1f),
			Updater.Ease(delegate(Vector2 position)
			{
				this.itemSprite.rectTransform.anchoredPosition = position;
			}, this.itemSpritePosStart, this.itemSpritePosEnd, 0.2f, Easing.SmoothStep, 0.1f)
		}));
	}

	[SerializeField]
	private Image itemSprite;

	private bool itemSpriteDidInit;

	private Vector2 itemSpritePosStart;

	private Vector2 itemSpritePosEnd;
}
