using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

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
		KInputManager.InputChange.AddListener(new UnityAction(this.RefreshText));
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

	private new void OnDestroy()
	{
		KInputManager.InputChange.RemoveListener(new UnityAction(this.RefreshText));
		base.OnDestroy();
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
		if (input != null)
		{
			string text = LocText.ParseText(input);
			if (text != input)
			{
				this.originalString = input;
			}
			else
			{
				this.originalString = string.Empty;
			}
			input = text;
		}
		if (this.AllowLinks)
		{
			return LocText.ModifyLinkStrings(input);
		}
		return input;
	}

	public static string ParseText(string input)
	{
		string text = "\\{Hotkey/(\\w+)\\}";
		string text2 = Regex.Replace(input, text, delegate(Match m)
		{
			string value = m.Groups[1].Value;
			global::Action action;
			if (LocText.ActionLookup.TryGetValue(value, out action))
			{
				return GameUtil.GetHotkeyString(action);
			}
			return m.Value;
		});
		if (text2.Contains('\\'))
		{
			string[] array = text2.Split(new char[] { '\\' });
			string text3 = string.Empty;
			if (array.Length >= 3)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (i % 2 == 0)
					{
						text3 += array[i];
					}
					else if (KInputManager.currentControllerIsGamepad)
					{
						if (Enum.TryParse<UI.ClickType>(array[i], out LocText.clickCache))
						{
							switch (LocText.clickCache)
							{
							case UI.ClickType.Click:
								text3 += UI.CONTROLS.PRESS;
								break;
							case UI.ClickType.Clicked:
								text3 += UI.CONTROLS.PRESSED;
								break;
							case UI.ClickType.Clicking:
								text3 += UI.CONTROLS.PRESSING;
								break;
							case UI.ClickType.Clickable:
								text3 += UI.CONTROLS.PRESSABLE;
								break;
							case UI.ClickType.Clicks:
								text3 += UI.CONTROLS.PRESSES;
								break;
							case UI.ClickType.click:
								text3 += UI.CONTROLS.PRESSLOWER;
								break;
							case UI.ClickType.clicked:
								text3 += UI.CONTROLS.PRESSEDLOWER;
								break;
							case UI.ClickType.clicking:
								text3 += UI.CONTROLS.PRESSINGLOWER;
								break;
							case UI.ClickType.clickable:
								text3 += UI.CONTROLS.PRESSABLELOWER;
								break;
							case UI.ClickType.clicks:
								text3 += UI.CONTROLS.PRESSESLOWER;
								break;
							case UI.ClickType.CLICK:
								text3 += UI.CONTROLS.PRESSUPPER;
								break;
							case UI.ClickType.CLICKED:
								text3 += UI.CONTROLS.PRESSEDUPPER;
								break;
							case UI.ClickType.CLICKING:
								text3 += UI.CONTROLS.PRESSINGUPPER;
								break;
							case UI.ClickType.CLICKABLE:
								text3 += UI.CONTROLS.PRESSABLEUPPER;
								break;
							case UI.ClickType.CLICKS:
								text3 += UI.CONTROLS.PRESSESUPPER;
								break;
							default:
								text3 += array[i];
								break;
							}
						}
						else
						{
							text3 += array[i];
						}
					}
					else if (Enum.TryParse<UI.ClickType>(array[i], out LocText.clickCache))
					{
						switch (LocText.clickCache)
						{
						case UI.ClickType.Click:
							text3 += UI.CONTROLS.CLICK;
							break;
						case UI.ClickType.Clicked:
							text3 += UI.CONTROLS.CLICKED;
							break;
						case UI.ClickType.Clicking:
							text3 += UI.CONTROLS.CLICKING;
							break;
						case UI.ClickType.Clickable:
							text3 += UI.CONTROLS.CLICKABLE;
							break;
						case UI.ClickType.Clicks:
							text3 += UI.CONTROLS.CLICKS;
							break;
						case UI.ClickType.click:
							text3 += UI.CONTROLS.CLICKLOWER;
							break;
						case UI.ClickType.clicked:
							text3 += UI.CONTROLS.CLICKEDLOWER;
							break;
						case UI.ClickType.clicking:
							text3 += UI.CONTROLS.CLICKINGLOWER;
							break;
						case UI.ClickType.clickable:
							text3 += UI.CONTROLS.CLICKABLELOWER;
							break;
						case UI.ClickType.clicks:
							text3 += UI.CONTROLS.CLICKSLOWER;
							break;
						case UI.ClickType.CLICK:
							text3 += UI.CONTROLS.CLICKUPPER;
							break;
						case UI.ClickType.CLICKED:
							text3 += UI.CONTROLS.CLICKEDUPPER;
							break;
						case UI.ClickType.CLICKING:
							text3 += UI.CONTROLS.CLICKINGUPPER;
							break;
						case UI.ClickType.CLICKABLE:
							text3 += UI.CONTROLS.CLICKABLEUPPER;
							break;
						case UI.ClickType.CLICKS:
							text3 += UI.CONTROLS.CLICKSUPPER;
							break;
						default:
							text3 += array[i];
							break;
						}
					}
					else
					{
						text3 += array[i];
					}
				}
				text2 = text3;
			}
		}
		return text2;
	}

	private void RefreshText()
	{
		if (this.originalString != string.Empty)
		{
			this.SetText(this.originalString);
		}
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
		if (input == null || input.IndexOf("<b><style=\"KLink\">") != -1)
		{
			return input;
		}
		StringBuilder stringBuilder = new StringBuilder(input);
		stringBuilder.Replace("<link=\"", LocText.combinedPrefix);
		stringBuilder.Replace("</link>", LocText.combinedSuffix);
		return stringBuilder.ToString();
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

	private string originalString = string.Empty;

	private static UI.ClickType clickCache = UI.ClickType.click;

	[SerializeField]
	private bool allowLinksInternal;

	private static readonly Dictionary<string, global::Action> ActionLookup = Enum.GetNames(typeof(global::Action)).ToDictionary<string, string, global::Action>((string x) => x, (string x) => (global::Action)Enum.Parse(typeof(global::Action), x), StringComparer.OrdinalIgnoreCase);

	private const string linkPrefix_open = "<link=\"";

	private const string linkSuffix = "</link>";

	private const string linkColorPrefix = "<b><style=\"KLink\">";

	private const string linkColorSuffix = "</style></b>";

	private static readonly string combinedPrefix = "<b><style=\"KLink\"><link=\"";

	private static readonly string combinedSuffix = "</style></b></link>";
}
