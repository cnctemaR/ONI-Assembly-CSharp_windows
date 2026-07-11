using System;
using System.Collections.Generic;
using UnityEngine;

public class CodexTextWithTooltip : CodexWidget<CodexTextWithTooltip>
{
	public string text { get; set; }

	public string tooltip { get; set; }

	public CodexTextStyle style { get; set; }

	public string stringKey
	{
		get
		{
			return "--> " + (this.text ?? "NULL");
		}
		set
		{
			this.text = Strings.Get(value);
		}
	}

	public CodexTextWithTooltip()
	{
		this.style = CodexTextStyle.Body;
	}

	public CodexTextWithTooltip(string text, string tooltip, CodexTextStyle style = CodexTextStyle.Body)
	{
		this.text = text;
		this.style = style;
		this.tooltip = tooltip;
	}

	public void ConfigureLabel(LocText label, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		label.gameObject.SetActive(true);
		label.AllowLinks = this.style == CodexTextStyle.Body;
		label.textStyleSetting = textStyles[this.style];
		label.text = this.text;
		label.ApplySettings();
	}

	public void ConfigureTooltip(ToolTip tooltip)
	{
		tooltip.SetSimpleTooltip(this.tooltip);
	}

	public override void Configure(GameObject contentGameObject, Transform displayPane, Dictionary<CodexTextStyle, TextStyleSetting> textStyles)
	{
		this.ConfigureLabel(contentGameObject.GetComponent<LocText>(), textStyles);
		this.ConfigureTooltip(contentGameObject.GetComponent<ToolTip>());
		base.ConfigurePreferredLayout(contentGameObject);
	}
}
