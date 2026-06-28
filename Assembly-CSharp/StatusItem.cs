using System;
using UnityEngine;
using UnityEngine.UI;

public class StatusItem : Resource
{
	public StatusItem(string id, string prefix, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, SimViewMode render_overlay, bool showWorldIcon = true, int status_overlays = 63486)
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
		if (icon_type != StatusItem.IconType.Info)
		{
			if (icon_type != StatusItem.IconType.Exclamation)
			{
				if (icon_type != StatusItem.IconType.Custom)
				{
				}
			}
			else
			{
				icon = "status_item_exclamation";
			}
		}
		else
		{
			icon = "dash";
		}
		this.iconName = icon;
		this.notificationType = notification_type;
		this.sprite = Assets.GetTintedSprite(icon);
		this.tooltipText = text;
		this.iconType = icon_type;
		this.allowMultiples = allow_multiples;
		this.render_overlay = render_overlay;
		this.showShowWorldIcon = showWorldIcon;
		this.status_overlays = status_overlays;
		if (this.sprite == null)
		{
			global::Debug.LogWarning("Status item '" + id + "' references a missing icon: " + icon, null);
		}
	}

	public StatusItem(string id, string name, string tooltip, string icon, StatusItem.IconType icon_type, NotificationType notification_type, bool allow_multiples, SimViewMode render_overlay, int status_overlays = 63486)
		: base(id, name)
	{
		if (icon_type != StatusItem.IconType.Info)
		{
			if (icon_type != StatusItem.IconType.Exclamation)
			{
				if (icon_type != StatusItem.IconType.Custom)
				{
				}
			}
			else
			{
				icon = "status_item_exclamation";
			}
		}
		else
		{
			icon = "dash";
		}
		this.iconName = icon;
		this.notificationType = notification_type;
		this.sprite = Assets.GetTintedSprite(icon);
		this.tooltipText = tooltip;
		this.iconType = icon_type;
		this.allowMultiples = allow_multiples;
		this.render_overlay = render_overlay;
		this.status_overlays = status_overlays;
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

	public virtual string GetName(object data)
	{
		return this.ResolveString(this.Name, data);
	}

	public virtual string GetTooltip(object data)
	{
		return this.ResolveTooltip(this.tooltipText, data);
	}

	private string ResolveString(string str, object data)
	{
		if (this.resolveStringCallback != null && data != null)
		{
			return this.resolveStringCallback(str, data);
		}
		return str;
	}

	private string ResolveTooltip(string str, object data)
	{
		if (data != null)
		{
			if (this.resolveTooltipCallback != null)
			{
				return this.resolveTooltipCallback(str, data);
			}
			if (this.resolveStringCallback != null)
			{
				return this.resolveStringCallback(str, data);
			}
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
		string tooltip = this.GetTooltip(data);
		tooltip_widget.AddMultiStringTooltip(tooltip, property_style);
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

	public static StatusItem.StatusItemOverlays GetStatusItemOverlayBySimViewMode(SimViewMode mode)
	{
		StatusItem.StatusItemOverlays statusItemOverlays = StatusItem.StatusItemOverlays.None;
		if (mode != SimViewMode.Decor)
		{
			if (mode != SimViewMode.OxygenMap)
			{
				if (mode != SimViewMode.Crop)
				{
					if (mode != SimViewMode.LiquidVentMap)
					{
						if (mode != SimViewMode.PowerMap)
						{
							if (mode != SimViewMode.GasVentMap)
							{
								if (mode != SimViewMode.Rooms)
								{
									if (mode != SimViewMode.HeatFlow)
									{
										if (mode == SimViewMode.SuitRequiredMap)
										{
											return StatusItem.StatusItemOverlays.Suits;
										}
										if (mode == SimViewMode.SolidConveyorMap)
										{
											return StatusItem.StatusItemOverlays.Conveyor;
										}
										if (mode != SimViewMode.ThermalConductivity)
										{
											if (mode == SimViewMode.TemperatureMap)
											{
												return StatusItem.StatusItemOverlays.Temperature;
											}
											if (mode == SimViewMode.Disease)
											{
												return StatusItem.StatusItemOverlays.Pathogens;
											}
											if (mode == SimViewMode.Light)
											{
												return StatusItem.StatusItemOverlays.Light;
											}
											if (mode == SimViewMode.None)
											{
												return StatusItem.StatusItemOverlays.None;
											}
											if (mode != SimViewMode.Logic)
											{
												global::Debug.LogWarning("ViewMode " + mode + " has no StatusItemOverlay value", null);
												return statusItemOverlays;
											}
											return StatusItem.StatusItemOverlays.Logic;
										}
									}
									statusItemOverlays = StatusItem.StatusItemOverlays.ThermalComfort;
								}
								else
								{
									statusItemOverlays = StatusItem.StatusItemOverlays.Rooms;
								}
							}
							else
							{
								statusItemOverlays = StatusItem.StatusItemOverlays.GasPlunbing;
							}
						}
						else
						{
							statusItemOverlays = StatusItem.StatusItemOverlays.PowerMap;
						}
					}
					else
					{
						statusItemOverlays = StatusItem.StatusItemOverlays.LiquidPlumbing;
					}
				}
				else
				{
					statusItemOverlays = StatusItem.StatusItemOverlays.Farming;
				}
			}
			else
			{
				statusItemOverlays = StatusItem.StatusItemOverlays.None;
			}
		}
		else
		{
			statusItemOverlays = StatusItem.StatusItemOverlays.Decor;
		}
		return statusItemOverlays;
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

	public Func<string, object, string> resolveTooltipCallback;

	public bool allowMultiples;

	public Func<SimViewMode, object, bool> conditionalOverlayCallback;

	public SimViewMode render_overlay;

	public int status_overlays;

	private string prefix;

	private bool showShowWorldIcon = true;

	public const int ALL_OVERLAYS = 63486;

	public enum IconType
	{
		Info,
		Exclamation,
		Custom
	}

	[Flags]
	public enum StatusItemOverlays
	{
		None = 2,
		PowerMap = 4,
		Temperature = 8,
		ThermalComfort = 16,
		Light = 32,
		LiquidPlumbing = 64,
		GasPlunbing = 128,
		Decor = 256,
		Pathogens = 512,
		Farming = 1024,
		Rooms = 4096,
		Suits = 8192,
		Logic = 16384,
		Conveyor = 32768
	}
}
