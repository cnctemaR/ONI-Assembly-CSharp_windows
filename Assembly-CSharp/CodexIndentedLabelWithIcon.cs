using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodexIndentedLabelWithIcon : CodexWidget<CodexIndentedLabelWithIcon>
{
	public CodexImage icon { get; set; }

	public CodexText label { get; set; }

	public string stringKey { get; set; } = "";

	public string batchedAnimPrefabSourceID { get; set; } = "";

	public string spriteName { get; set; } = "";

	public CodexIndentedLabelWithIcon()
	{
		this.icon = new CodexImage();
		this.label = new CodexText();
	}

	public CodexIndentedLabelWithIcon(string text, CodexTextStyle style, global::Tuple<Sprite, Color> coloredSprite)
	{
		this.icon = new CodexImage(coloredSprite);
		this.label = new CodexText(text, style, null);
	}

	public CodexIndentedLabelWithIcon(string text, CodexTextStyle style, global::Tuple<Sprite, Color> coloredSprite, int iconWidth, int iconHeight)
	{
		this.icon = new CodexImage(iconWidth, iconHeight, coloredSprite);
		this.label = new CodexText(text, style, null);
	}

	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		if (!string.IsNullOrEmpty(this.stringKey))
		{
			this.label.stringKey = this.stringKey;
		}
		if (!string.IsNullOrEmpty(this.batchedAnimPrefabSourceID))
		{
			GameObject gameObject = Assets.TryGetPrefab(this.batchedAnimPrefabSourceID);
			KBatchedAnimController kbatchedAnimController = ((gameObject != null) ? gameObject.GetComponent<KBatchedAnimController>() : null);
			KAnimFile kanimFile = ((kbatchedAnimController != null) ? kbatchedAnimController.AnimFiles[0] : null);
			this.icon.sprite = ((kanimFile != null) ? Def.GetUISpriteFromMultiObjectAnim(kanimFile, "ui", false, "") : null);
		}
		if (!string.IsNullOrEmpty(this.spriteName))
		{
			this.icon.sprite = Assets.GetSprite(this.spriteName);
		}
		Image componentInChildren = contentGameObject.GetComponentInChildren<Image>();
		this.icon.ConfigureImage(componentInChildren);
		this.label.ConfigureLabel(contentGameObject.GetComponentInChildren<LocText>(), textStyles);
		if (this.icon.preferredWidth != -1 && this.icon.preferredHeight != -1)
		{
			LayoutElement component = componentInChildren.GetComponent<LayoutElement>();
			component.minWidth = (float)this.icon.preferredHeight;
			component.minHeight = (float)this.icon.preferredWidth;
			component.preferredHeight = (float)this.icon.preferredHeight;
			component.preferredWidth = (float)this.icon.preferredWidth;
		}
	}
}
