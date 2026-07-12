using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodexIndentedLabelWithIcon : CodexWidget<CodexIndentedLabelWithIcon>
{
	public CodexImage icon { get; set; }

	public CodexText label { get; set; }

	public CodexIndentedLabelWithIcon()
	{
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
