using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KIconToggleMenu : KScreen
{
	public event KIconToggleMenu.OnSelect onSelect;

	public void Setup(IList<KIconToggleMenu.ToggleInfo> toggleInfo)
	{
		this.toggleInfo = toggleInfo;
		this.RefreshButtons();
	}

	protected void Setup()
	{
		this.RefreshButtons();
	}

	protected virtual void RefreshButtons()
	{
		foreach (KToggle ktoggle in this.toggles)
		{
			if (ktoggle != null)
			{
				if (!this.dontDestroyToggles.Contains(ktoggle))
				{
					global::UnityEngine.Object.Destroy(ktoggle.gameObject);
				}
				else
				{
					ktoggle.ClearOnClick();
				}
			}
		}
		this.toggles.Clear();
		this.dontDestroyToggles.Clear();
		if (this.toggleInfo == null)
		{
			return;
		}
		Transform transform = ((this.toggleParent != null) ? this.toggleParent : base.transform);
		for (int i = 0; i < this.toggleInfo.Count; i++)
		{
			int idx = i;
			KIconToggleMenu.ToggleInfo toggleInfo = this.toggleInfo[i];
			KToggle ktoggle2;
			if (toggleInfo.instanceOverride != null)
			{
				ktoggle2 = toggleInfo.instanceOverride;
				this.dontDestroyToggles.Add(ktoggle2);
			}
			else if (toggleInfo.prefabOverride)
			{
				ktoggle2 = Util.KInstantiateUI<KToggle>(toggleInfo.prefabOverride.gameObject, transform.gameObject, true);
			}
			else
			{
				ktoggle2 = Util.KInstantiateUI<KToggle>(this.prefab.gameObject, transform.gameObject, true);
			}
			ktoggle2.Deselect();
			ktoggle2.gameObject.name = "Toggle:" + toggleInfo.text;
			ktoggle2.group = this.group;
			ktoggle2.onClick += delegate
			{
				this.OnClick(idx);
			};
			LocText componentInChildren = ktoggle2.transform.GetComponentInChildren<LocText>();
			if (componentInChildren != null)
			{
				componentInChildren.SetText(toggleInfo.text);
			}
			if (toggleInfo.getSpriteCB != null)
			{
				ktoggle2.fgImage.sprite = toggleInfo.getSpriteCB();
			}
			else if (toggleInfo.icon != null)
			{
				ktoggle2.fgImage.sprite = Assets.GetSprite(toggleInfo.icon);
			}
			toggleInfo.SetToggle(ktoggle2);
			this.toggles.Add(ktoggle2);
		}
	}

	public Sprite GetIcon(string name)
	{
		foreach (Sprite sprite in this.icons)
		{
			if (sprite.name == name)
			{
				return sprite;
			}
		}
		return null;
	}

	public virtual void ClearSelection()
	{
		if (this.toggles == null)
		{
			return;
		}
		foreach (KToggle ktoggle in this.toggles)
		{
			ktoggle.Deselect();
			ktoggle.ClearAnimState();
		}
		this.selected = -1;
	}

	private void OnClick(int i)
	{
		if (this.onSelect == null)
		{
			return;
		}
		this.selected = i;
		this.onSelect(this.toggleInfo[i]);
		if (!this.toggles[i].isOn)
		{
			this.selected = -1;
		}
		for (int j = 0; j < this.toggles.Count; j++)
		{
			if (j != this.selected)
			{
				this.toggles[j].isOn = false;
			}
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.toggles == null)
		{
			return;
		}
		if (this.toggleInfo == null)
		{
			return;
		}
		for (int i = 0; i < this.toggleInfo.Count; i++)
		{
			if (this.toggles[i].isActiveAndEnabled)
			{
				global::Action hotKey = this.toggleInfo[i].hotKey;
				if (hotKey != global::Action.NumActions && e.TryConsume(hotKey))
				{
					if (this.selected != i || this.repeatKeyDownToggles)
					{
						this.toggles[i].Click();
						if (this.selected == i)
						{
							this.toggles[i].Deselect();
						}
						this.selected = i;
						return;
					}
					break;
				}
			}
		}
	}

	public virtual void Close()
	{
		this.ClearSelection();
		this.Show(false);
	}

	[SerializeField]
	private Transform toggleParent;

	[SerializeField]
	private KToggle prefab;

	[SerializeField]
	private ToggleGroup group;

	[SerializeField]
	private Sprite[] icons;

	[SerializeField]
	public TextStyleSetting ToggleToolTipTextStyleSetting;

	[SerializeField]
	public TextStyleSetting ToggleToolTipHeaderTextStyleSetting;

	[SerializeField]
	protected bool repeatKeyDownToggles = true;

	protected KToggle currentlySelectedToggle;

	protected IList<KIconToggleMenu.ToggleInfo> toggleInfo;

	protected List<KToggle> toggles = new List<KToggle>();

	private List<KToggle> dontDestroyToggles = new List<KToggle>();

	protected int selected = -1;

	public delegate void OnSelect(KIconToggleMenu.ToggleInfo toggleInfo);

	public class ToggleInfo
	{
		public ToggleInfo(string text, string icon, object user_data = null, global::Action hotkey = global::Action.NumActions, string tooltip = "", string tooltip_header = "")
		{
			this.text = text;
			this.userData = user_data;
			this.icon = icon;
			this.hotKey = hotkey;
			this.tooltip = tooltip;
			this.tooltipHeader = tooltip_header;
			this.getTooltipText = new ToolTip.ComplexTooltipDelegate(this.DefaultGetTooltipText);
		}

		public ToggleInfo(string text, object user_data, global::Action hotkey, Func<Sprite> get_sprite_cb)
		{
			this.text = text;
			this.userData = user_data;
			this.hotKey = hotkey;
			this.getSpriteCB = get_sprite_cb;
		}

		public virtual void SetToggle(KToggle toggle)
		{
			this.toggle = toggle;
			toggle.GetComponent<ToolTip>().OnComplexToolTip = this.getTooltipText;
		}

		protected virtual List<global::Tuple<string, TextStyleSetting>> DefaultGetTooltipText()
		{
			List<global::Tuple<string, TextStyleSetting>> list = new List<global::Tuple<string, TextStyleSetting>>();
			if (this.tooltipHeader != null)
			{
				list.Add(new global::Tuple<string, TextStyleSetting>(this.tooltipHeader, ToolTipScreen.Instance.defaultTooltipHeaderStyle));
			}
			list.Add(new global::Tuple<string, TextStyleSetting>(this.tooltip, ToolTipScreen.Instance.defaultTooltipBodyStyle));
			return list;
		}

		public string text;

		public object userData;

		public string icon;

		public string tooltip;

		public string tooltipHeader;

		public KToggle toggle;

		public global::Action hotKey;

		public ToolTip.ComplexTooltipDelegate getTooltipText;

		public Func<Sprite> getSpriteCB;

		public KToggle prefabOverride;

		public KToggle instanceOverride;
	}
}
