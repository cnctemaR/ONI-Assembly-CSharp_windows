using System;
using UnityEngine;
using UnityEngine.UI;

public class KTabMenuHeader : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.ActivateTabArtwork(0);
	}

	public void Add(string name, KTabMenuHeader.OnClick onClick, int id)
	{
		GameObject gameObject = Util.KInstantiateUI(this.prefab.gameObject, null, false);
		gameObject.SetActive(true);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.transform.SetParent(this.transform, false);
		component.name = name;
		Text componentInChildren = component.GetComponentInChildren<Text>();
		if (componentInChildren != null)
		{
			componentInChildren.text = name.ToUpper();
		}
		this.ActivateTabArtwork(id);
		KButton component2 = gameObject.GetComponent<KButton>();
		component2.onClick += delegate
		{
			onClick(id);
		};
	}

	public void Add(Sprite icon, string name, KTabMenuHeader.OnClick onClick, int id, string tooltip = "")
	{
		GameObject gameObject = Util.KInstantiateUI(this.prefab.gameObject, null, false);
		RectTransform component = gameObject.GetComponent<RectTransform>();
		component.transform.SetParent(this.transform, false);
		component.name = name;
		if (tooltip == string.Empty)
		{
			component.GetComponent<ToolTip>().toolTip = name;
		}
		else
		{
			component.GetComponent<ToolTip>().toolTip = tooltip;
		}
		this.ActivateTabArtwork(id);
		TabHeaderIcon componentInChildren = component.GetComponentInChildren<TabHeaderIcon>();
		if (componentInChildren)
		{
			componentInChildren.TitleText.text = name;
		}
		KToggle component2 = gameObject.GetComponent<KToggle>();
		if (component2 && component2.fgImage)
		{
			component2.fgImage.sprite = icon;
		}
		component2.group = base.GetComponent<ToggleGroup>();
		component2.onClick += delegate
		{
			onClick(id);
		};
	}

	public void Activate(int itemIdx, int previouslyActiveTabIdx)
	{
		int childCount = this.transform.childCount;
		if (itemIdx >= childCount)
		{
			return;
		}
		for (int i = 0; i < childCount; i++)
		{
			Transform child = this.transform.GetChild(i);
			if (child.gameObject.activeSelf)
			{
				KButton componentInChildren = child.GetComponentInChildren<KButton>();
				if (componentInChildren != null)
				{
					Text componentInChildren2 = componentInChildren.GetComponentInChildren<Text>();
					if (componentInChildren2 != null && i == itemIdx)
					{
						this.ActivateTabArtwork(itemIdx);
					}
				}
				KToggle component = child.GetComponent<KToggle>();
				if (component != null)
				{
					this.ActivateTabArtwork(itemIdx);
					if (i == itemIdx)
					{
						component.Select();
					}
					else
					{
						component.Deselect();
					}
				}
			}
		}
	}

	public void SetTabEnabled(int tabIdx, bool enabled)
	{
		if (tabIdx < this.transform.childCount)
		{
			this.transform.GetChild(tabIdx).gameObject.SetActive(enabled);
		}
	}

	public virtual void ActivateTabArtwork(int tabIdx)
	{
		if (tabIdx >= this.transform.childCount)
		{
			return;
		}
		for (int i = 0; i < this.transform.childCount; i++)
		{
			ImageToggleState component = this.transform.GetChild(i).GetComponent<ImageToggleState>();
			if (component != null)
			{
				if (i == tabIdx)
				{
					component.SetActive();
				}
				else
				{
					component.SetInactive();
				}
			}
			Canvas componentInChildren = this.transform.GetChild(i).GetComponentInChildren<Canvas>(true);
			if (componentInChildren != null)
			{
				componentInChildren.overrideSorting = tabIdx == i;
			}
			SetTextStyleSetting componentInChildren2 = this.transform.GetChild(i).GetComponentInChildren<SetTextStyleSetting>();
			if (componentInChildren2 != null && this.TextStyle_Active != null && this.TextStyle_Inactive != null)
			{
				if (i == tabIdx)
				{
					componentInChildren2.SetStyle(this.TextStyle_Active);
				}
				else
				{
					componentInChildren2.SetStyle(this.TextStyle_Inactive);
				}
			}
		}
	}

	[SerializeField]
	private RectTransform prefab;

	public TextStyleSetting TextStyle_Active;

	public TextStyleSetting TextStyle_Inactive;

	public delegate void OnClick(int id);
}
