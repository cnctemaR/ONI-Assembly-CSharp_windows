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

	protected void RefreshButtons()
	{
		foreach (KToggle ktoggle in this.toggles)
		{
			if (ktoggle != null)
			{
				global::UnityEngine.Object.Destroy(ktoggle.gameObject);
			}
		}
		this.toggles.Clear();
		if (this.toggleInfo == null)
		{
			return;
		}
		Transform transform = ((!(this.toggleParent != null)) ? this.transform : this.toggleParent);
		for (int i = 0; i < this.toggleInfo.Count; i++)
		{
			int idx = i;
			KIconToggleMenu.ToggleInfo toggleInfo = this.toggleInfo[i];
			KToggle ktoggle2 = global::UnityEngine.Object.Instantiate(this.prefab, Vector3.zero, Quaternion.identity) as KToggle;
			ktoggle2.Deselect();
			ktoggle2.gameObject.name = "Toggle:" + toggleInfo.text;
			ktoggle2.transform.SetParent(transform, false);
			ktoggle2.group = this.group;
			ktoggle2.onClick += delegate
			{
				this.OnClick(idx);
			};
			Transform transform2 = ktoggle2.transform.FindChild("Text");
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
				string hotkeyString = GameUtil.GetHotkeyString(toggleInfo.hotKey);
				if (toggleInfo.tooltipHeader != string.Empty)
				{
					component2.AddMultiStringTooltip(toggleInfo.tooltipHeader, (!(this.ToggleToolTipHeaderTextStyleSetting != null)) ? this.ToggleToolTipTextStyleSetting : this.ToggleToolTipHeaderTextStyleSetting);
					if (this.ToggleToolTipHeaderTextStyleSetting == null)
					{
						global::Debug.Log("!", null);
					}
				}
				component2.AddMultiStringTooltip(toggleInfo.tooltip + " " + hotkeyString, this.ToggleToolTipTextStyleSetting);
			}
			foreach (Sprite sprite in this.icons)
			{
				if (sprite != null && sprite.name == toggleInfo.icon)
				{
					Image fgImage = ktoggle2.fgImage;
					fgImage.sprite = sprite;
					break;
				}
			}
			toggleInfo.toggle = ktoggle2;
			this.toggles.Add(ktoggle2);
		}
	}

	protected void SelectToggle(KToggle newlySelectedToggle)
	{
		if (this.currentlySelectedToggle == newlySelectedToggle)
		{
			this.currentlySelectedToggle = null;
			this.selected = -1;
		}
		else
		{
			this.currentlySelectedToggle = newlySelectedToggle;
		}
		foreach (KToggle ktoggle in this.toggles)
		{
			if (ktoggle != null)
			{
				ImageToggleState component = ktoggle.GetComponent<ImageToggleState>();
				if (ktoggle == this.currentlySelectedToggle)
				{
					ktoggle.Select();
					ktoggle.isOn = true;
					ktoggle.ActivateFlourish(true);
					if (component && component.GetIsActive())
					{
						component.SetInactive();
					}
					else if (component && !component.GetIsActive())
					{
						component.SetActive();
					}
				}
				else
				{
					ktoggle.Deselect();
					ktoggle.isOn = false;
					ktoggle.ActivateFlourish(false);
					if (component && component.GetIsActive())
					{
						component.SetInactive();
					}
				}
			}
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
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.toggles == null)
		{
			return;
		}
		for (int i = 0; i < this.toggleInfo.Count; i++)
		{
			global::Action hotKey = this.toggleInfo[i].hotKey;
			if (hotKey != global::Action.NumActions && e.TryConsume(hotKey))
			{
				if (this.selected != i || this.repeatKeyDownToggles)
				{
					this.toggles[i].Click();
					this.selected = i;
				}
				break;
			}
		}
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

	protected int selected = -1;

	public class ToggleInfo
	{
		public ToggleInfo(string text, string icon_name, object user_data = null, global::Action hotKey = global::Action.NumActions, string tooltip = "", string tooltip_header = "")
		{
			this.text = text;
			this.userData = user_data;
			this.icon = icon_name;
			this.hotKey = hotKey;
			this.tooltip = tooltip;
			this.tooltipHeader = tooltip_header;
		}

		public string text;

		public object userData;

		public string icon;

		public string tooltip;

		public string tooltipHeader;

		public KToggle toggle;

		public global::Action hotKey;
	}

	public delegate void OnSelect(KIconToggleMenu.ToggleInfo toggleInfo);
}
