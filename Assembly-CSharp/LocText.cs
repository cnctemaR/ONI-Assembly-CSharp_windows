using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class LocText : TextMeshProUGUI
{
	protected override void OnEnable()
	{
		base.OnEnable();
	}

	public bool AllowLinks
	{
		get
		{
			return this.allowLinksInternal;
		}
		set
		{
			this.allowLinksInternal = value;
			this.RefreshLinkHandler();
			this.raycastTarget = this.raycastTarget || this.allowLinksInternal;
		}
	}

	[ContextMenu("Apply Settings")]
	public void ApplySettings()
	{
		if (this.key != "" && Application.isPlaying)
		{
			StringKey stringKey = new StringKey(this.key);
			this.text = Strings.Get(stringKey);
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
		if (this.key != "")
		{
			StringEntry stringEntry = Strings.Get(new StringKey(this.key));
			this.text = stringEntry.String;
		}
		this.text = Localization.Fixup(this.text);
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
		this.textLinkHandler = base.GetComponent<TextLinkHandler>();
	}

	private new void Start()
	{
		base.Start();
		this.RefreshLinkHandler();
	}

	public override void SetLayoutDirty()
	{
		if (this.staticLayout)
		{
			return;
		}
		base.SetLayoutDirty();
	}

	public override string text
	{
		get
		{
			return base.text;
		}
		set
		{
			base.text = this.FilterInput(value);
		}
	}

	public override void SetText(string text)
	{
		text = this.FilterInput(text);
		base.SetText(text);
	}

	private string FilterInput(string input)
	{
		if (this.AllowLinks)
		{
			return LocText.ModifyLinkStrings(input);
		}
		return input;
	}

	protected override void GenerateTextMesh()
	{
		base.GenerateTextMesh();
	}

	internal void SwapFont(TMP_FontAsset font, bool isRightToLeft)
	{
		base.font = font;
		if (this.key != "")
		{
			StringEntry stringEntry = Strings.Get(new StringKey(this.key));
			this.text = stringEntry.String;
		}
		this.text = Localization.Fixup(this.text);
		base.isRightToLeftText = isRightToLeft;
	}

	private static string ModifyLinkStrings(string input)
	{
		string text = "<link=\"";
		string text2 = "</link>";
		string text3 = "<b><style=\"KLink\">";
		string text4 = "</style></b>";
		string text5 = text3 + text;
		if (input == null || Regex.Split(input, text5).Length > 1)
		{
			return input;
		}
		LocText.splits = Regex.Split(input, text);
		if (LocText.splits.Length > 1)
		{
			for (int i = 1; i < LocText.splits.Length; i++)
			{
				if (!(LocText.splits[i] == ""))
				{
					int num = input.IndexOf(LocText.splits[i]);
					input = input.Insert(num - text.Length, text3);
				}
			}
		}
		LocText.splits = Regex.Split(input, text2);
		if (LocText.splits.Length > 1)
		{
			for (int j = 0; j < LocText.splits.Length; j++)
			{
				if (!(LocText.splits[j] == ""))
				{
					int num2 = input.IndexOf(LocText.splits[j]);
					if (num2 != 0)
					{
						input = input.Insert(num2, text4);
					}
				}
			}
		}
		return input;
	}

	private void RefreshLinkHandler()
	{
		if (this.textLinkHandler == null && this.allowLinksInternal)
		{
			this.textLinkHandler = base.GetComponent<TextLinkHandler>();
			if (this.textLinkHandler == null)
			{
				this.textLinkHandler = base.gameObject.AddComponent<TextLinkHandler>();
			}
		}
		else if (!this.allowLinksInternal && this.textLinkHandler != null)
		{
			global::UnityEngine.Object.Destroy(this.textLinkHandler);
			this.textLinkHandler = null;
		}
		if (this.textLinkHandler != null)
		{
			this.textLinkHandler.CheckMouseOver();
		}
	}

	public string key;

	public TextStyleSetting textStyleSetting;

	public bool allowOverride;

	public bool staticLayout;

	private TextLinkHandler textLinkHandler;

	[SerializeField]
	private bool allowLinksInternal;

	private static string[] splits;
}
