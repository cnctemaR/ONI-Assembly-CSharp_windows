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

	public void SetLinkOverrideAction(Func<string, bool> action)
	{
		this.RefreshLinkHandler();
		if (this.textLinkHandler != null)
		{
			this.textLinkHandler.overrideLinkAction = action;
		}
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
		text = "\\(ClickType/(\\w+)\\)";
		return Regex.Replace(text2, text, delegate(Match m)
		{
			string value2 = m.Groups[1].Value;
			Pair<LocString, LocString> pair;
			if (!LocText.ClickLookup.TryGetValue(value2, out pair))
			{
				return m.Value;
			}
			if (KInputManager.currentControllerIsGamepad)
			{
				return pair.first.ToString();
			}
			return pair.second.ToString();
		});
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
		StringBuilder stringBuilder = GlobalStringBuilderPool.Alloc();
		stringBuilder.Append(input);
		stringBuilder.Replace("<link=\"", LocText.combinedPrefix);
		stringBuilder.Replace("</link>", LocText.combinedSuffix);
		return GlobalStringBuilderPool.ReturnAndFree(stringBuilder);
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

	[SerializeField]
	private bool allowLinksInternal;

	private static readonly Dictionary<string, global::Action> ActionLookup = Enum.GetNames(typeof(global::Action)).ToDictionary<string, string, global::Action>((string x) => x, (string x) => (global::Action)Enum.Parse(typeof(global::Action), x), StringComparer.OrdinalIgnoreCase);

	private static readonly Dictionary<string, Pair<LocString, LocString>> ClickLookup = new Dictionary<string, Pair<LocString, LocString>>
	{
		{
			UI.ClickType.Click.ToString(),
			new Pair<LocString, LocString>(UI.CONTROLS.PRESS, UI.CONTROLS.CLICK)
		},
		{
			UI.ClickType.Clickable.ToString(),
			new Pair<LocString, LocString>(UI.CONTROLS.PRESSABLE, UI.CONTROLS.CLICKABLE)
		},
		{
			UI.ClickType.Clicked.ToString(),
			new Pair<LocString, LocString>(UI.CONTROLS.PRESSED, UI.CONTROLS.CLICKED)
		},
		{
			UI.ClickType.Clicking.ToString(),
			new Pair<LocString, LocString>(UI.CONTROLS.PRESSING, UI.CONTROLS.CLICKING)
		},
		{
			UI.ClickType.Clicks.ToString(),
			new Pair<LocString, LocString>(UI.CONTROLS.PRESSES, UI.CONTROLS.CLICKS)
		},
		{
			UI.ClickType.click.ToString(),
			new Pair<LocString, LocString>(UI.CONTROLS.PRESSLOWER, UI.CONTROLS.CLICKLOWER)
		},
		{
			UI.ClickType.clickable.ToString(),
			new Pair<LocString, LocString>(UI.CONTROLS.PRESSABLELOWER, UI.CONTROLS.CLICKABLELOWER)
		},
		{
			UI.ClickType.clicked.ToString(),
			new Pair<LocString, LocString>(UI.CONTROLS.PRESSEDLOWER, UI.CONTROLS.CLICKEDLOWER)
		},
		{
			UI.ClickType.clicking.ToString(),
			new Pair<LocString, LocString>(UI.CONTROLS.PRESSINGLOWER, UI.CONTROLS.CLICKINGLOWER)
		},
		{
			UI.ClickType.clicks.ToString(),
			new Pair<LocString, LocString>(UI.CONTROLS.PRESSESLOWER, UI.CONTROLS.CLICKSLOWER)
		},
		{
			UI.ClickType.CLICK.ToString(),
			new Pair<LocString, LocString>(UI.CONTROLS.PRESSUPPER, UI.CONTROLS.CLICKUPPER)
		},
		{
			UI.ClickType.CLICKABLE.ToString(),
			new Pair<LocString, LocString>(UI.CONTROLS.PRESSABLEUPPER, UI.CONTROLS.CLICKABLEUPPER)
		},
		{
			UI.ClickType.CLICKED.ToString(),
			new Pair<LocString, LocString>(UI.CONTROLS.PRESSEDUPPER, UI.CONTROLS.CLICKEDUPPER)
		},
		{
			UI.ClickType.CLICKING.ToString(),
			new Pair<LocString, LocString>(UI.CONTROLS.PRESSINGUPPER, UI.CONTROLS.CLICKINGUPPER)
		},
		{
			UI.ClickType.CLICKS.ToString(),
			new Pair<LocString, LocString>(UI.CONTROLS.PRESSESUPPER, UI.CONTROLS.CLICKSUPPER)
		}
	};

	private const string linkPrefix_open = "<link=\"";

	private const string linkSuffix = "</link>";

	private const string linkColorPrefix = "<b><style=\"KLink\">";

	private const string linkColorSuffix = "</style></b>";

	private static readonly string combinedPrefix = "<b><style=\"KLink\"><link=\"";

	private static readonly string combinedSuffix = "</style></b></link>";
}
