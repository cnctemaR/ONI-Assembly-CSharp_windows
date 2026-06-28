using System;
using Klei.AI;
using UnityEngine;
using UnityEngine.UI;

public class StatusItem : Resource
{
	public StatusItem(string id, string prefix, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, SimViewMode overlay, SimViewMode second_overlay, bool showShowWorldIcon = true)
		: base(id, Strings.Get(string.Concat(new string[]
		{
			"STRINGS.",
			prefix,
			".STATUSITEMS.",
			id.ToUpper(),
			".NAME"
		})))
	{
		this.prefix = prefix;
		string text = Strings.Get(string.Concat(new string[]
		{
			"STRINGS.",
			prefix,
			".STATUSITEMS.",
			id.ToUpper(),
			".TOOLTIP"
		}));
		switch (icon_type)
		{
		case StatusItem.IconType.Info:
			icon = "dash";
			break;
		case StatusItem.IconType.Exclamation:
			icon = "status_item_exclamation";
			break;
		}
		this.iconName = icon;
		this.notificationType = notification_type;
		this.sprite = Assets.GetTintedSprite(icon);
		this.tooltipText = text;
		this.iconType = icon_type;
		this.allowMultiples = allow_multiples;
		this.overlay = overlay;
		this.showShowWorldIcon = showShowWorldIcon;
		if (this.sprite == null)
		{
			global::Debug.LogWarning("Status item '" + id + "' references a missing icon: " + icon, null);
		}
	}

	public StatusItem(string id, string name, string tooltip, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, SimViewMode overlay, SimViewMode second_overlay)
		: base(id, name)
	{
		switch (icon_type)
		{
		case StatusItem.IconType.Info:
			icon = "dash";
			break;
		case StatusItem.IconType.Exclamation:
			icon = "status_item_exclamation";
			break;
		}
		this.iconName = icon;
		this.notificationType = notification_type;
		this.sprite = Assets.GetTintedSprite(icon);
		this.tooltipText = tooltip;
		this.iconType = icon_type;
		this.allowMultiples = allow_multiples;
		this.overlay = overlay;
		if (this.sprite == null)
		{
			global::Debug.LogWarning("Status item '" + id + "' references a missing icon: " + icon, null);
		}
	}

	public void AddNotification(string sound_path = null, string notification_text = null, string notification_tooltip = null, float notification_delay = 0f)
	{
		this.shouldNotify = true;
		this.notificationDelay = notification_delay;
		if (sound_path == null)
		{
			NotificationType notificationType = this.notificationType;
			if (notificationType != NotificationType.Bad)
			{
				this.soundPath = "Notification";
			}
			else
			{
				this.soundPath = "Warning";
			}
		}
		else
		{
			this.soundPath = sound_path;
		}
		if (notification_text != null)
		{
			this.notificationText = notification_text;
		}
		else
		{
			DebugUtil.Assert(this.prefix != null, "When adding a notification, either set the status prefix or specify strings!");
			this.notificationText = Strings.Get(string.Concat(new string[]
			{
				"STRINGS.",
				this.prefix,
				".STATUSITEMS.",
				this.Id.ToUpper(),
				".NOTIFICATION_NAME"
			}));
		}
		if (notification_tooltip != null)
		{
			this.notificationTooltipText = notification_tooltip;
		}
		else
		{
			DebugUtil.Assert(this.prefix != null, "When adding a notification, either set the status prefix or specify strings!");
			this.notificationTooltipText = Strings.Get(string.Concat(new string[]
			{
				"STRINGS.",
				this.prefix,
				".STATUSITEMS.",
				this.Id.ToUpper(),
				".NOTIFICATION_TOOLTIP"
			}));
		}
	}

	public void AddEffect(string effect_name)
	{
		if (!string.IsNullOrEmpty(effect_name))
		{
			this.effect = Db.Get().effects.Get(effect_name);
		}
	}

	public void AddEffect(Effect effect)
	{
		this.effect = effect;
	}

	public virtual string GetName(object data)
	{
		return this.ResolveString(this.Name, data);
	}

	public string ResolveString(string str, object data)
	{
		if (this.resolveStringCallback != null && data != null)
		{
			return this.resolveStringCallback(str, data);
		}
		return str;
	}

	public bool ShouldShowIcon()
	{
		return this.iconType == StatusItem.IconType.Custom && this.showShowWorldIcon;
	}

	public virtual void ShowToolTip(ToolTip tooltip_widget, object data, TextStyleSetting property_style)
	{
		tooltip_widget.ClearMultiStringTooltip();
		string text = this.ResolveString(this.tooltipText, data);
		tooltip_widget.AddMultiStringTooltip(text, property_style);
	}

	public void SetIcon(Image image, object data)
	{
		if (this.sprite == null)
		{
			return;
		}
		image.color = this.sprite.color;
		image.sprite = this.sprite.sprite;
	}

	public bool UseConditionalCallback(SimViewMode overlay, Transform transform)
	{
		return overlay != SimViewMode.None && this.conditionalOverlayCallback != null && this.conditionalOverlayCallback(overlay, transform);
	}

	public StatusItem SetResolveStringCallback(Func<string, object, string> cb)
	{
		this.resolveStringCallback = cb;
		return this;
	}

	public string tooltipText;

	public string notificationText;

	public string notificationTooltipText;

	public float notificationDelay;

	public string soundPath;

	public string iconName;

	public TintedSprite sprite;

	public bool shouldNotify;

	public StatusItem.IconType iconType;

	public NotificationType notificationType;

	public Notification.ClickCallback notificationClickCallback;

	public Func<string, object, string> resolveStringCallback;

	public bool allowMultiples;

	public Effect effect;

	public Func<SimViewMode, object, bool> conditionalOverlayCallback;

	public SimViewMode overlay;

	private string prefix;

	private bool showShowWorldIcon = true;

	public enum IconType
	{
		Info,
		Exclamation,
		Custom
	}
}
