using System;
using UnityEngine;
using UnityEngine.UI;

public class TitleBar : KMonoBehaviour
{
	public void SetTitle(string Name)
	{
		this.titleText.text = Name;
	}

	public void SetBackgroundState(bool warning)
	{
		if (this.backgroundImageToggle == null)
		{
			return;
		}
		if (warning)
		{
			this.backgroundImageToggle.SetInactive();
		}
		else
		{
			this.backgroundImageToggle.SetActive();
		}
	}

	public void SetSubText(string subtext, string tooltip = "")
	{
		this.subtextText.text = subtext;
		this.subtextText.GetComponent<ToolTip>().toolTip = tooltip;
	}

	public void SetWarningActve(bool state)
	{
		this.WarningNotification.SetActive(state);
	}

	public void SetWarning(Sprite icon, string label)
	{
		this.SetWarningActve(true);
		this.NotificationIcon.sprite = icon;
		this.NotificationText.text = label;
	}

	public void SetPortrait(GameObject target)
	{
		this.portrait.SetPortrait(target);
	}

	public LocText titleText;

	public LocText subtextText;

	public GameObject WarningNotification;

	public Text NotificationText;

	public Image NotificationIcon;

	public Sprite techIcon;

	public Sprite materialIcon;

	public TitleBarPortrait portrait;

	public ImageToggleState backgroundImageToggle;

	public bool userEditable;

	public bool setCameraControllerState = true;
}
