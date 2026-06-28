using System;
using TMPro;
using UnityEngine;

public class LocText : TextMeshProUGUI
{
	[ContextMenu("Apply Settings")]
	public void ApplySettings()
	{
		if (this.key != string.Empty && Application.isPlaying)
		{
			StringKey stringKey = new StringKey(this.key);
			base.text = Strings.Get(stringKey);
		}
		if (this.textStyleSetting != null)
		{
			SetTextStyleSetting.ApplyStyle(this, this.textStyleSetting);
		}
	}

	private new void Awake()
	{
		base.Awake();
		if (!Application.isPlaying)
		{
			return;
		}
		if (this.key != string.Empty)
		{
			StringKey stringKey = new StringKey(this.key);
			StringEntry stringEntry = Strings.Get(stringKey);
			base.text = stringEntry.String;
		}
		base.text = Localization.Fixup(base.text);
		base.isRightToLeftText = Localization.IsRightToLeft;
		SetTextStyleSetting setTextStyleSetting = base.gameObject.GetComponent<SetTextStyleSetting>();
		if (setTextStyleSetting == null)
		{
			setTextStyleSetting = base.gameObject.AddComponent<SetTextStyleSetting>();
		}
		if (!this.allowOverride)
		{
			setTextStyleSetting.SetStyle(this.textStyleSetting);
		}
	}

	public override void SetLayoutDirty()
	{
		if (this.staticLayout)
		{
			return;
		}
		base.SetLayoutDirty();
	}

	internal void SwapFont(TMP_FontAsset font, bool isRightToLeft)
	{
		base.font = font;
		if (this.key != string.Empty)
		{
			StringKey stringKey = new StringKey(this.key);
			StringEntry stringEntry = Strings.Get(stringKey);
			base.text = stringEntry.String;
		}
		base.text = Localization.Fixup(base.text);
		base.isRightToLeftText = isRightToLeft;
	}

	public string key;

	public TextStyleSetting textStyleSetting;

	public bool allowOverride;

	public bool staticLayout;
}
