using System;
using System.Collections.Generic;
using STRINGS;

public class OverlayMenu : KIconToggleMenu
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		OverlayMenu.Instance = this;
		this.overlay_toggle_infos = this.InitializeToggles();
		base.Setup(this.overlay_toggle_infos);
		Game.Instance.Subscribe(1798162660, new Action<object>(this.OnOverlayChanged));
		base.onSelect += this.OnToggleSelect;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Game.Instance.Unsubscribe(1798162660, new Action<object>(this.OnOverlayChanged));
	}

	private List<KIconToggleMenu.ToggleInfo> InitializeToggles()
	{
		return new List<KIconToggleMenu.ToggleInfo>
		{
			new KIconToggleMenu.ToggleInfo(UI.OVERLAYS.OXYGEN.BUTTON, "overlay_oxygen", SimViewMode.OxygenMap, global::Action.Overlay1, UI.TOOLTIPS.OXYGENOVERLAYSTRING),
			new KIconToggleMenu.ToggleInfo(UI.OVERLAYS.ELECTRICAL.BUTTON, "overlay_power", SimViewMode.PowerMap, global::Action.Overlay2, UI.TOOLTIPS.POWEROVERLAYSTRING),
			new KIconToggleMenu.ToggleInfo(UI.OVERLAYS.TEMPERATURE.BUTTON, "overlay_temperature", SimViewMode.TemperatureMap, global::Action.Overlay3, UI.TOOLTIPS.TEMPERATUREOVERLAYSTRING),
			new KIconToggleMenu.ToggleInfo(UI.OVERLAYS.HEATFLOW.BUTTON, "overlay_heatflow", SimViewMode.HeatFlow, global::Action.Overlay4, UI.TOOLTIPS.HEATFLOWOVERLAYSTRING),
			new KIconToggleMenu.ToggleInfo(UI.OVERLAYS.LIGHTING.BUTTON, "overlay_lights", SimViewMode.Light, global::Action.Overlay5, UI.TOOLTIPS.LIGHTSOVERLAYSTRING),
			new KIconToggleMenu.ToggleInfo(UI.OVERLAYS.LIQUIDPLUMBING.BUTTON, "overlay_liquidvent", SimViewMode.LiquidVentMap, global::Action.Overlay6, UI.TOOLTIPS.LIQUIDVENTOVERLAYSTRING),
			new KIconToggleMenu.ToggleInfo(UI.OVERLAYS.GASPLUMBING.BUTTON, "overlay_gasvent", SimViewMode.GasVentMap, global::Action.Overlay7, UI.TOOLTIPS.GASVENTOVERLAYSTRING),
			new KIconToggleMenu.ToggleInfo(UI.OVERLAYS.DECOR.BUTTON, "overlay_decor", SimViewMode.Decor, global::Action.Overlay8, UI.TOOLTIPS.DECOROVERLAYSTRING),
			new KIconToggleMenu.ToggleInfo(UI.OVERLAYS.PRIORITIES.BUTTON, "overlay_priority", SimViewMode.Priorities, global::Action.Overlay9, UI.TOOLTIPS.PRIORITIESOVERLAYSTRING)
		};
	}

	private void OnToggleSelect(KIconToggleMenu.ToggleInfo toggle_info)
	{
		if (SimDebugView.Instance.GetMode() == (SimViewMode)((int)toggle_info.userData))
		{
			OverlayScreen.Instance.ToggleOverlay(SimViewMode.None);
		}
		else
		{
			OverlayScreen.Instance.ToggleOverlay((SimViewMode)((int)toggle_info.userData));
		}
	}

	private void OnOverlayChanged(object overlay_data)
	{
		SimViewMode simViewMode = (SimViewMode)((int)overlay_data);
		for (int i = 0; i < this.overlay_toggle_infos.Count; i++)
		{
			if ((int)this.overlay_toggle_infos[i].userData == (int)simViewMode)
			{
				this.overlay_toggle_infos[i].toggle.isOn = true;
			}
			else
			{
				this.overlay_toggle_infos[i].toggle.isOn = false;
			}
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.Consumed)
		{
			return;
		}
		if (OverlayScreen.Instance.GetMode() != SimViewMode.None && e.TryConsume(global::Action.Escape))
		{
			OverlayScreen.Instance.ToggleOverlay(SimViewMode.None);
		}
		if (!e.Consumed)
		{
			base.OnKeyDown(e);
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (e.Consumed)
		{
			return;
		}
		if (OverlayScreen.Instance.GetMode() != SimViewMode.None && PlayerController.Instance.ConsumeIfNotDragging(e, global::Action.MouseRight))
		{
			OverlayScreen.Instance.ToggleOverlay(SimViewMode.None);
		}
		if (!e.Consumed)
		{
			base.OnKeyUp(e);
		}
	}

	public static OverlayMenu Instance;

	private List<KIconToggleMenu.ToggleInfo> overlay_toggle_infos;
}
