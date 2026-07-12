using System;
using System.Collections.Generic;

public class NotificationAlertBar : KMonoBehaviour
{
	public void Init(ManagementMenuNotification notification)
	{
		this.notification = notification;
		this.thisButton.onClick += this.OnThisButtonClicked;
		this.background.colorStyleSetting = this.alertColorStyle[(int)notification.valence];
		this.background.ApplyColorStyleSetting();
		this.text.text = notification.titleText;
		this.tooltip.SetSimpleTooltip(notification.ToolTip(null, notification.tooltipData));
		this.muteButton.onClick += this.OnMuteButtonClicked;
	}

	private void OnThisButtonClicked()
	{
		NotificationHighlightController componentInParent = base.GetComponentInParent<NotificationHighlightController>();
		if (componentInParent != null)
		{
			componentInParent.SetActiveTarget(this.notification);
			return;
		}
		this.notification.View();
	}

	private void OnMuteButtonClicked()
	{
	}

	public ManagementMenuNotification notification;

	public KButton thisButton;

	public KImage background;

	public LocText text;

	public ToolTip tooltip;

	public KButton muteButton;

	public List<ColorStyleSetting> alertColorStyle;
}
