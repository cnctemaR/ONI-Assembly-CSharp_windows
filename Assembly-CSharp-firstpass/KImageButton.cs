using System;
using UnityEngine;
using UnityEngine.UI;

public class KImageButton : KButton
{
	public Sprite Sprite
	{
		get
		{
			return this.fgImage.sprite;
		}
		set
		{
			this.fgImage.enabled = value != null;
			this.fgImage.sprite = value;
		}
	}

	public Sprite BackgroundSprite
	{
		get
		{
			return this.bgImage.sprite;
		}
		set
		{
			this.bgImage.enabled = value != null;
			this.bgImage.sprite = value;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.fgImage.enabled = false;
	}

	public Text text;
}
