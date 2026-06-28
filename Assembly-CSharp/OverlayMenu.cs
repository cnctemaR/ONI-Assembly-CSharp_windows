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
		Game.Instance.Subscribe(-107300940, new Action<object>(this.OnResearchComplete));
		base.onSelect += this.OnToggleSelect;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.RefreshButtons();
	}

	protected override void RefreshButtons()
	{
		base.RefreshButtons();
		if (Research.Instance == null)
		{
			return;
		}
		foreach (KIconToggleMenu.ToggleInfo toggleInfo in this.overlay_toggle_infos)
		{
			OverlayMenu.OverlayToggleInfo overlayToggleInfo = (OverlayMenu.OverlayToggleInfo)toggleInfo;
			toggleInfo.toggle.gameObject.SetActive(overlayToggleInfo.IsUnlocked());
		}
	}

	private void OnResearchComplete(object data)
	{
		this.RefreshButtons();
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
			new OverlayMenu.OverlayToggleInfo(UI.OVERLAYS.OXYGEN.BUTTON, "overlay_oxygen", SimViewMode.OxygenMap, string.Empty, global::Action.Overlay1, UI.TOOLTIPS.OXYGENOVERLAYSTRING, UI.OVERLAYS.OXYGEN.BUTTON),
			new OverlayMenu.OverlayToggleInfo(UI.OVERLAYS.ELECTRICAL.BUTTON, "overlay_power", SimViewMode.PowerMap, string.Empty, global::Action.Overlay2, UI.TOOLTIPS.POWEROVERLAYSTRING, UI.OVERLAYS.ELECTRICAL.BUTTON),
			new OverlayMenu.OverlayToggleInfo(UI.OVERLAYS.TEMPERATURE.BUTTON, "overlay_temperature", SimViewMode.TemperatureMap, string.Empty, global::Action.Overlay3, UI.TOOLTIPS.TEMPERATUREOVERLAYSTRING, UI.OVERLAYS.TEMPERATURE.BUTTON),
			new OverlayMenu.OverlayToggleInfo(UI.OVERLAYS.HEATFLOW.BUTTON, "overlay_heatflow", SimViewMode.HeatFlow, string.Empty, global::Action.Overlay4, UI.TOOLTIPS.HEATFLOWOVERLAYSTRING, UI.OVERLAYS.HEATFLOW.BUTTON),
			new OverlayMenu.OverlayToggleInfo(UI.OVERLAYS.LIGHTING.BUTTON, "overlay_lights", SimViewMode.Light, string.Empty, global::Action.Overlay5, UI.TOOLTIPS.LIGHTSOVERLAYSTRING, UI.OVERLAYS.LIGHTING.BUTTON),
			new OverlayMenu.OverlayToggleInfo(UI.OVERLAYS.LIQUIDPLUMBING.BUTTON, "overlay_liquidvent", SimViewMode.LiquidVentMap, string.Empty, global::Action.Overlay6, UI.TOOLTIPS.LIQUIDVENTOVERLAYSTRING, UI.OVERLAYS.LIQUIDPLUMBING.BUTTON),
			new OverlayMenu.OverlayToggleInfo(UI.OVERLAYS.GASPLUMBING.BUTTON, "overlay_gasvent", SimViewMode.GasVentMap, string.Empty, global::Action.Overlay7, UI.TOOLTIPS.GASVENTOVERLAYSTRING, UI.OVERLAYS.GASPLUMBING.BUTTON),
			new OverlayMenu.OverlayToggleInfo(UI.OVERLAYS.DECOR.BUTTON, "overlay_decor", SimViewMode.Decor, string.Empty, global::Action.Overlay8, UI.TOOLTIPS.DECOROVERLAYSTRING, UI.OVERLAYS.DECOR.BUTTON),
			new OverlayMenu.OverlayToggleInfo(UI.OVERLAYS.DISEASE.BUTTON, "overlay_disease", SimViewMode.Disease, string.Empty, global::Action.Overlay9, UI.TOOLTIPS.DISEASEOVERLAYSTRING, UI.OVERLAYS.DISEASE.BUTTON),
			new OverlayMenu.OverlayToggleInfo(UI.OVERLAYS.CROPS.BUTTON, "overlay_farming", SimViewMode.Crop, string.Empty, global::Action.Overlay10, UI.TOOLTIPS.CROPS_OVERLAY_STRING, UI.OVERLAYS.CROPS.BUTTON),
			new OverlayMenu.OverlayToggleInfo(UI.OVERLAYS.ROOMS.BUTTON, "overlay_rooms", SimViewMode.Rooms, string.Empty, global::Action.Overlay11, UI.TOOLTIPS.ROOMSOVERLAYSTRING, UI.OVERLAYS.ROOMS.BUTTON),
			new OverlayMenu.OverlayToggleInfo(UI.OVERLAYS.SUIT.BUTTON, "overlay_suit", SimViewMode.SuitRequiredMap, "Suits", global::Action.Overlay12, UI.TOOLTIPS.ROOMSOVERLAYSTRING, UI.OVERLAYS.SUIT.BUTTON)
		};
	}

	private void OnToggleSelect(KIconToggleMenu.ToggleInfo toggle_info)
	{
		if (SimDebugView.Instance.GetMode() == ((OverlayMenu.OverlayToggleInfo)toggle_info).simView)
		{
			OverlayScreen.Instance.ToggleOverlay(SimViewMode.None);
		}
		else if (((OverlayMenu.OverlayToggleInfo)toggle_info).IsUnlocked())
		{
			OverlayScreen.Instance.ToggleOverlay(((OverlayMenu.OverlayToggleInfo)toggle_info).simView);
		}
	}

	private void OnOverlayChanged(object overlay_data)
	{
		SimViewMode simViewMode = (SimViewMode)((int)overlay_data);
		for (int i = 0; i < this.overlay_toggle_infos.Count; i++)
		{
			if (((OverlayMenu.OverlayToggleInfo)this.overlay_toggle_infos[i]).simView == simViewMode)
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

	private class OverlayToggleInfo : KIconToggleMenu.ToggleInfo
	{
		public OverlayToggleInfo(string text, string icon_name, SimViewMode sim_view, string required_tech = "", global::Action hotKey = global::Action.NumActions, string tooltip = "", string tooltip_header = "")
			: base(text, icon_name, null, hotKey, tooltip, tooltip_header)
		{
			this.simView = sim_view;
			this.requiredTech = required_tech;
		}

		public bool IsUnlocked()
		{
			if (string.IsNullOrEmpty(this.requiredTech))
			{
				return true;
			}
			Tech tech = Db.Get().Techs.Get(this.requiredTech);
			TechInstance techInstance = Research.Instance.Get(tech);
			return techInstance != null && techInstance.IsComplete();
		}

		public SimViewMode simView;

		public string requiredTech;
	}
}
