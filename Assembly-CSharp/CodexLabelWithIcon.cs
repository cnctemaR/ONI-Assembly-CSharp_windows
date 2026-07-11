using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodexLabelWithIcon : CodexWidget<CodexLabelWithIcon>
{
	public CodexLabelWithIcon(string text, CodexTextStyle style, Tuple<Sprite, Color> coloredSprite)
	{
		this.icon = new CodexImage(coloredSprite);
		this.label = new CodexText(text, style);
	}

	public CodexImage icon { get; set; }

	public CodexText label { get; set; }

	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		this.icon.ConfigureImage(contentGameObject.GetComponentInChildren<Image>());
		this.label.ConfigureLabel(contentGameObject.GetComponentInChildren<LocText>(), textStyles);
	}
}
