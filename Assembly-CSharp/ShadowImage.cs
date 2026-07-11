using System;
using UnityEngine;
using UnityEngine.UI;

public class ShadowImage : ShadowRect
{
	protected override void MatchRect()
	{
		base.MatchRect();
		if (this.RectMain == null || this.RectShadow == null)
		{
			return;
		}
		if (this.shadowImage == null)
		{
			this.shadowImage = this.RectShadow.GetComponent<Image>();
		}
		if (this.mainImage == null)
		{
			this.mainImage = this.RectMain.GetComponent<Image>();
		}
		if (this.mainImage == null)
		{
			if (this.shadowImage != null)
			{
				this.shadowImage.color = Color.clear;
			}
			return;
		}
		if (this.shadowImage == null)
		{
			return;
		}
		if (this.shadowImage.sprite != this.mainImage.sprite)
		{
			this.shadowImage.sprite = this.mainImage.sprite;
		}
		if (this.shadowImage.color != this.shadowColor)
		{
			if (this.shadowImage.sprite != null)
			{
				this.shadowImage.color = this.shadowColor;
				return;
			}
			this.shadowImage.color = Color.clear;
		}
	}

	private Image shadowImage;

	private Image mainImage;
}
