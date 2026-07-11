using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodexImage : CodexWidget<CodexImage>
{
	public CodexImage()
	{
	}

	public CodexImage(int preferredWidth, int preferredHeight, Sprite sprite, Color color)
		: base(preferredWidth, preferredHeight)
	{
		this.sprite = sprite;
		this.color = color;
	}

	public CodexImage(int preferredWidth, int preferredHeight, Sprite sprite)
		: this(preferredWidth, preferredHeight, sprite, Color.white)
	{
	}

	public CodexImage(int preferredWidth, int preferredHeight, Tuple<Sprite, Color> coloredSprite)
		: this(preferredWidth, preferredHeight, coloredSprite.first, coloredSprite.second)
	{
	}

	public CodexImage(Tuple<Sprite, Color> coloredSprite)
		: this(-1, -1, coloredSprite)
	{
	}

	public Sprite sprite { get; set; }

	public Color color { get; set; }

	public string spriteName
	{
		get
		{
			return "--> " + ((!(this.sprite == null)) ? this.sprite.ToString() : "NULL");
		}
		set
		{
			this.sprite = Assets.GetSprite(value);
		}
	}

	public string batchedAnimPrefabSourceID
	{
		get
		{
			return "--> " + ((!(this.sprite == null)) ? this.sprite.ToString() : "NULL");
		}
		set
		{
			GameObject prefab = Assets.GetPrefab(value);
			KBatchedAnimController kbatchedAnimController = ((!(prefab != null)) ? null : prefab.GetComponent<KBatchedAnimController>());
			KAnimFile kanimFile = ((!(kbatchedAnimController != null)) ? null : kbatchedAnimController.AnimFiles[0]);
			this.sprite = ((!(kanimFile != null)) ? null : Def.GetUISpriteFromMultiObjectAnim(kanimFile, "ui", false, string.Empty));
		}
	}

	public void ConfigureImage(Image image)
	{
		image.sprite = this.sprite;
		image.color = this.color;
	}

	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		this.ConfigureImage(contentGameObject.GetComponent<Image>());
		base.ConfigurePreferredLayout(contentGameObject);
	}
}
