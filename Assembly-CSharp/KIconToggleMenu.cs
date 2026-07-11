using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class KIconToggleMenu : KScreen
{
	[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
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
		Transform transform = ((!(this.toggleParent != null)) ? base.transform : this.toggleParent);
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
			Transform transform2 = ktoggle2.transform.Find("Text");
			if (transform2 != null)
			{
				LocText component = transform2.GetComponent<LocText>();
				if (component != null)
				{
					component.text = toggleInfo.text;
				}
			}
			ToolTip component2 = ktoggle2.GetComponent<ToolTip>();
			if (component2)
			{
				if (toggleInfo.tooltipHeader != string.Empty)
				{
					component2.AddMultiStringTooltip(toggleInfo.tooltipHeader, (!(this.ToggleToolTipHeaderTextStyleSetting != null)) ? this.ToggleToolTipTextStyleSetting : this.ToggleToolTipHeaderTextStyleSetting);
					if (this.ToggleToolTipHeaderTextStyleSetting == null)
					{
						global::Debug.Log("!");
					}
				}
				component2.AddMultiStringTooltip(GameUtil.ReplaceHotkeyString(toggleInfo.tooltip, toggleInfo.hotKey), this.ToggleToolTipTextStyleSetting);
			}
			if (toggleInfo.getSpriteCB != null)
			{
				ktoggle2.fgImage.sprite = toggleInfo.getSpriteCB();
			}
			else if (toggleInfo.icon != null)
			{
				ktoggle2.fgImage.sprite = Assets.GetSprite(toggleInfo.icon);
			}
			toggleInfo.toggle = ktoggle2;
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
					}
					break;
				}
			}
		}
	}

	public virtual void Close()
	{
		this.ClearSelection();
		base.Show(false);
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
		}

		public ToggleInfo(string text, object user_data, global::Action hotkey, Func<Sprite> get_sprite_cb)
		{
			this.text = text;
			this.userData = user_data;
			this.hotKey = hotkey;
			this.getSpriteCB = get_sprite_cb;
		}

		public string text;

		public object userData;

		public string icon;

		public string tooltip;

		public string tooltipHeader;

		public KToggle toggle;

		public global::Action hotKey;

		public Func<Sprite> getSpriteCB;

		public KToggle prefabOverride;

		public KToggle instanceOverride;
	}
}
