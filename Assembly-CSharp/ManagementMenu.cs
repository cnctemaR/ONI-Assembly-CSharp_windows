using System;
using System.Collections.Generic;
using FMOD.Studio;
using STRINGS;
using UnityEngine;

public class ManagementMenu : KIconToggleMenu
{
	public static void DestroyInstance()
	{
		ManagementMenu.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ManagementMenu.Instance = this;
		CodexCache.Init();
		this.instantiator.Instantiate();
		this.jobsScreen = this.instantiator.GetComponentInChildren<JobsTableScreen>(true);
		this.consumablesScreen = this.instantiator.GetComponentInChildren<ConsumablesTableScreen>(true);
		this.vitalsScreen = this.instantiator.GetComponentInChildren<VitalsTableScreen>(true);
		this.starmapScreen = Resources.FindObjectsOfTypeAll(typeof(StarmapScreen))[0] as StarmapScreen;
		this.codexScreen = this.instantiator.GetComponentInChildren<CodexScreen>(true);
		this.scheduleScreen = this.instantiator.GetComponentInChildren<ScheduleScreen>(true);
		this.rolesScreen = Resources.FindObjectsOfTypeAll(typeof(RolesScreen))[0] as RolesScreen;
		base.Subscribe(Game.Instance.gameObject, 288942073, new Action<object>(this.OnUIClear));
		this.consumablesInfo = new KIconToggleMenu.ToggleInfo(UI.CONSUMABLES, "OverviewUI_consumables_icon", null, global::Action.ManageConsumables, UI.TOOLTIPS.MANAGEMENTMENU_CONSUMABLES, string.Empty);
		this.vitalsInfo = new KIconToggleMenu.ToggleInfo(UI.VITALS, "OverviewUI_vitals_icon", null, global::Action.ManageVitals, UI.TOOLTIPS.MANAGEMENTMENU_VITALS, string.Empty);
		this.reportsInfo = new KIconToggleMenu.ToggleInfo(UI.REPORT, "OverviewUI_reports_icon", null, global::Action.ManageReport, UI.TOOLTIPS.MANAGEMENTMENU_DAILYREPORT, string.Empty);
		this.researchInfo = new KIconToggleMenu.ToggleInfo(UI.RESEARCH, "OverviewUI_research_nav_icon", null, global::Action.ManageResearch, UI.TOOLTIPS.MANAGEMENTMENU_RESEARCH, string.Empty);
		this.jobsInfo = new KIconToggleMenu.ToggleInfo(UI.JOBS, "OverviewUI_priority_icon", null, global::Action.ManagePeople, UI.TOOLTIPS.MANAGEMENTMENU_JOBS, string.Empty);
		this.rolesInfo = new KIconToggleMenu.ToggleInfo(UI.ROLES_SCREEN.MANAGEMENT_BUTTON, "OverviewUI_jobs_icon", null, global::Action.ManageRoles, UI.TOOLTIPS.MANAGEMENTMENU_ROLES, string.Empty);
		this.starmapInfo = new KIconToggleMenu.ToggleInfo(UI.STARMAP.MANAGEMENT_BUTTON, "ic_rocket", null, global::Action.ManageStarmap, UI.TOOLTIPS.MANAGEMENTMENU_STARMAP, string.Empty);
		this.codexInfo = new KIconToggleMenu.ToggleInfo(UI.CODEX.MANAGEMENT_BUTTON, "OverviewUI_database_icon", null, global::Action.ManageCodex, UI.TOOLTIPS.MANAGEMENTMENU_CODEX, string.Empty);
		this.codexInfo.prefabOverride = this.smallPrefab;
		this.scheduleInfo = new KIconToggleMenu.ToggleInfo(UI.SCHEDULE, null, null, global::Action.ManageSchedule, UI.TOOLTIPS.MANAGEMENTMENU_SCHEDULE, string.Empty);
		this.scheduleInfo.instanceOverride = global::DateTime.Instance.scheduleToggle;
		this.ScreenInfoMatch.Add(this.consumablesInfo, new ManagementMenu.ScreenData
		{
			screen = this.consumablesScreen,
			tabIdx = 3,
			toggleInfo = this.consumablesInfo
		});
		this.ScreenInfoMatch.Add(this.vitalsInfo, new ManagementMenu.ScreenData
		{
			screen = this.vitalsScreen,
			tabIdx = 2,
			toggleInfo = this.vitalsInfo
		});
		this.ScreenInfoMatch.Add(this.reportsInfo, new ManagementMenu.ScreenData
		{
			screen = this.reportsScreen,
			tabIdx = 4,
			toggleInfo = this.reportsInfo
		});
		this.ScreenInfoMatch.Add(this.jobsInfo, new ManagementMenu.ScreenData
		{
			screen = this.jobsScreen,
			tabIdx = 1,
			toggleInfo = this.jobsInfo
		});
		this.ScreenInfoMatch.Add(this.rolesInfo, new ManagementMenu.ScreenData
		{
			screen = this.rolesScreen,
			tabIdx = 0,
			toggleInfo = this.rolesInfo
		});
		this.ScreenInfoMatch.Add(this.codexInfo, new ManagementMenu.ScreenData
		{
			screen = this.codexScreen,
			tabIdx = 6,
			toggleInfo = this.codexInfo
		});
		this.ScreenInfoMatch.Add(this.scheduleInfo, new ManagementMenu.ScreenData
		{
			screen = this.scheduleScreen,
			tabIdx = 7,
			toggleInfo = this.scheduleInfo
		});
		this.ScreenInfoMatch.Add(this.starmapInfo, new ManagementMenu.ScreenData
		{
			screen = this.starmapScreen,
			tabIdx = 7,
			toggleInfo = this.starmapInfo
		});
		base.Setup(new List<KIconToggleMenu.ToggleInfo> { this.consumablesInfo, this.vitalsInfo, this.reportsInfo, this.researchInfo, this.jobsInfo, this.rolesInfo, this.starmapInfo, this.codexInfo, this.scheduleInfo });
		base.onSelect += this.OnButtonClick;
		Components.ResearchCenters.OnAdd += new Action<ResearchCenter>(this.CheckResearch);
		Components.ResearchCenters.OnRemove += new Action<ResearchCenter>(this.CheckResearch);
		Components.RoleStations.OnAdd += new Action<RoleStation>(this.CheckRoles);
		Components.RoleStations.OnRemove += new Action<RoleStation>(this.CheckRoles);
		Game.Instance.Subscribe(-809948329, new Action<object>(this.CheckResearch));
		Game.Instance.Subscribe(-809948329, new Action<object>(this.CheckRoles));
		Components.Telescopes.OnAdd += new Action<Telescope>(this.CheckStarmap);
		Components.Telescopes.OnRemove += new Action<Telescope>(this.CheckStarmap);
		this.rolesTooltipDisabled = UI.TOOLTIPS.MANAGEMENTMENU_REQUIRES_ROLES_STATION;
		this.rolesTooltip = UI.TOOLTIPS.MANAGEMENTMENU_ROLES + " " + GameUtil.GetHotkeyString(global::Action.ManageRoles);
		this.researchTooltipDisabled = UI.TOOLTIPS.MANAGEMENTMENU_REQUIRES_RESEARCH;
		this.researchTooltip = UI.TOOLTIPS.MANAGEMENTMENU_RESEARCH + " " + GameUtil.GetHotkeyString(global::Action.ManageResearch);
		this.starmapTooltipDisabled = UI.TOOLTIPS.MANAGEMENTMENU_REQUIRES_TELESCOPE;
		this.starmapTooltip = UI.TOOLTIPS.MANAGEMENTMENU_STARMAP + " " + GameUtil.GetHotkeyString(global::Action.ManageResearch);
		this.CheckResearch(null);
		this.CheckRoles(null);
		this.CheckStarmap(null);
		this.researchInfo.toggle.soundPlayer.AcceptClickCondition = () => this.ResearchAvailable() || this.activeScreen == this.ScreenInfoMatch[ManagementMenu.Instance.researchInfo];
		foreach (KButton kbutton in this.CloseButtons)
		{
			kbutton.onClick += this.CloseAll;
			kbutton.soundPlayer.Enabled = false;
		}
		foreach (KToggle ktoggle in this.toggles)
		{
			ktoggle.soundPlayer.toggle_widget_sound_events[0].PlaySound = false;
			ktoggle.soundPlayer.toggle_widget_sound_events[1].PlaySound = false;
		}
	}

	public void AddResearchScreen(ResearchScreen researchScreen)
	{
		if (this.researchScreen != null)
		{
			return;
		}
		this.researchScreen = researchScreen;
		this.researchScreen.gameObject.SetActive(false);
		this.ScreenInfoMatch.Add(this.researchInfo, new ManagementMenu.ScreenData
		{
			screen = this.researchScreen,
			tabIdx = 5,
			toggleInfo = this.researchInfo
		});
		this.researchScreen.Show(false);
	}

	public void CheckResearch(object o)
	{
		if (this.researchInfo.toggle == null)
		{
			return;
		}
		bool flag = Components.ResearchCenters.Count <= 0 && !DebugHandler.InstantBuildMode;
		bool flag2 = !flag && this.activeScreen != null && this.activeScreen.toggleInfo == this.researchInfo;
		string text = ((!flag) ? this.researchTooltip : this.researchTooltipDisabled);
		this.ConfigureToggle(this.researchInfo.toggle, flag, flag2, text, this.ToggleToolTipTextStyleSetting);
	}

	public void CheckRoles(object o = null)
	{
		if (this.rolesInfo.toggle == null)
		{
			return;
		}
		bool flag = Components.RoleStations.Count <= 0 && !DebugHandler.InstantBuildMode;
		bool flag2 = this.activeScreen != null && this.activeScreen.toggleInfo == this.rolesInfo;
		string text = ((!flag) ? this.rolesTooltip : this.rolesTooltipDisabled);
		this.ConfigureToggle(this.rolesInfo.toggle, flag, flag2, text, this.ToggleToolTipTextStyleSetting);
	}

	public void CheckStarmap(object o = null)
	{
		if (this.starmapInfo.toggle == null)
		{
			return;
		}
		bool flag = Components.Telescopes.Count <= 0 && !DebugHandler.InstantBuildMode;
		bool flag2 = this.activeScreen != null && this.activeScreen.toggleInfo == this.starmapInfo;
		string text = ((!flag) ? this.starmapTooltip : this.starmapTooltipDisabled);
		this.ConfigureToggle(this.starmapInfo.toggle, flag, flag2, text, this.ToggleToolTipTextStyleSetting);
	}

	private void ConfigureToggle(KToggle toggle, bool disabled, bool active, string tooltip, TextStyleSetting tooltip_style)
	{
		toggle.interactable = active;
		toggle.GetComponent<KToggle>().interactable = !disabled;
		if (disabled)
		{
			toggle.GetComponentInChildren<ImageToggleState>().SetDisabled();
		}
		else
		{
			toggle.GetComponentInChildren<ImageToggleState>().SetActiveState(active);
		}
		ToolTip component = toggle.GetComponent<ToolTip>();
		component.ClearMultiStringTooltip();
		component.AddMultiStringTooltip(tooltip, tooltip_style);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.activeScreen != null && e.TryConsume(global::Action.Escape))
		{
			this.ToggleScreen(this.activeScreen);
		}
		if (!e.Consumed)
		{
			base.OnKeyDown(e);
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (this.activeScreen != null && PlayerController.Instance.ConsumeIfNotDragging(e, global::Action.MouseRight))
		{
			this.ToggleScreen(this.activeScreen);
		}
		if (!e.Consumed)
		{
			base.OnKeyUp(e);
		}
	}

	private bool ResearchAvailable()
	{
		return Components.ResearchCenters.Count > 0 || DebugHandler.InstantBuildMode;
	}

	private bool RolesAvailable()
	{
		return Components.RoleStations.Count > 0 || DebugHandler.InstantBuildMode;
	}

	private bool StarmapAvailable()
	{
		return Components.Telescopes.Count > 0 || DebugHandler.InstantBuildMode;
	}

	public void CloseAll()
	{
		if (this.activeScreen == null)
		{
			return;
		}
		if (this.activeScreen.toggleInfo != null)
		{
			this.ToggleScreen(this.activeScreen);
		}
		this.CloseActive();
		this.ClearSelection();
	}

	private void OnUIClear(object data)
	{
		this.CloseAll();
	}

	public void ToggleScreen(ManagementMenu.ScreenData screenData)
	{
		if (screenData == null)
		{
			return;
		}
		if (screenData.toggleInfo == this.researchInfo && !this.ResearchAvailable())
		{
			this.CheckResearch(null);
			this.CloseActive();
			return;
		}
		if (screenData.toggleInfo == this.rolesInfo && !this.RolesAvailable())
		{
			this.CheckRoles(null);
			this.CloseActive();
			return;
		}
		if (screenData.toggleInfo == this.starmapInfo && !this.StarmapAvailable())
		{
			this.CheckStarmap(null);
			this.CloseActive();
			return;
		}
		if (screenData.toggleInfo.toggle.gameObject.GetComponentInChildren<ImageToggleState>().IsDisabled)
		{
			return;
		}
		if (this.activeScreen != null)
		{
			this.activeScreen.toggleInfo.toggle.isOn = false;
			this.activeScreen.toggleInfo.toggle.gameObject.GetComponentInChildren<ImageToggleState>().SetInactive();
		}
		if (this.activeScreen != screenData)
		{
			OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID);
			if (this.activeScreen != null)
			{
				this.activeScreen.toggleInfo.toggle.ActivateFlourish(false);
			}
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open", false));
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().MenuOpenMigrated);
			screenData.toggleInfo.toggle.ActivateFlourish(true);
			screenData.toggleInfo.toggle.gameObject.GetComponentInChildren<ImageToggleState>().SetActive();
			this.CloseActive();
			this.activeScreen = screenData;
			this.activeScreen.screen.Show(true);
		}
		else
		{
			this.activeScreen.screen.Show(false);
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Close", false));
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MenuOpenMigrated, STOP_MODE.ALLOWFADEOUT);
			this.activeScreen.toggleInfo.toggle.ActivateFlourish(false);
			this.activeScreen = null;
			screenData.toggleInfo.toggle.gameObject.GetComponentInChildren<ImageToggleState>().SetInactive();
		}
	}

	public void OnButtonClick(KIconToggleMenu.ToggleInfo toggle_info)
	{
		this.ToggleScreen(this.ScreenInfoMatch[toggle_info]);
	}

	private void CloseActive()
	{
		if (this.activeScreen != null)
		{
			this.activeScreen.toggleInfo.toggle.isOn = false;
			this.activeScreen.screen.Show(false);
			this.activeScreen = null;
		}
	}

	public void ToggleResearch()
	{
		if (this.researchScreen == null)
		{
			this.AddResearchScreen(global::UnityEngine.Object.FindObjectOfType<ResearchScreen>());
		}
		if ((this.ResearchAvailable() || this.activeScreen == this.ScreenInfoMatch[ManagementMenu.Instance.researchInfo]) && this.researchInfo != null)
		{
			this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.researchInfo]);
		}
	}

	public void ToggleCodex()
	{
		this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.codexInfo]);
	}

	public void OpenCodexToEntry(string id)
	{
		if (!this.codexScreen.gameObject.activeInHierarchy)
		{
			this.ToggleCodex();
		}
		this.codexScreen.ChangeArticle(id, false);
	}

	public void ToggleRoles()
	{
		if ((this.RolesAvailable() || this.activeScreen == this.ScreenInfoMatch[ManagementMenu.Instance.rolesInfo]) && this.rolesInfo != null)
		{
			this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.rolesInfo]);
		}
	}

	public void ToggleStarmap()
	{
		if (this.starmapInfo != null)
		{
			this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.starmapInfo]);
		}
	}

	public void TogglePriorities()
	{
		this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.jobsInfo]);
	}

	public void OpenReports(int day)
	{
		if (this.activeScreen != this.ScreenInfoMatch[ManagementMenu.Instance.reportsInfo])
		{
			this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.reportsInfo]);
		}
		ReportScreen.Instance.ShowReport(day);
	}

	[SerializeField]
	private KToggle smallPrefab;

	private ManagementMenu.ScreenData activeScreen;

	private KButton activeButton;

	public static ManagementMenu Instance;

	public KScreen researchScreen;

	private JobsTableScreen jobsScreen;

	public VitalsTableScreen vitalsScreen;

	public ScheduleScreen scheduleScreen;

	public KScreen reportsScreen;

	private ConsumablesTableScreen consumablesScreen;

	public CodexScreen codexScreen;

	private RolesScreen rolesScreen;

	private StarmapScreen starmapScreen;

	public string colourSchemeDisabled;

	public InstantiateUIPrefabChild instantiator;

	private KIconToggleMenu.ToggleInfo jobsInfo;

	private KIconToggleMenu.ToggleInfo consumablesInfo;

	private KIconToggleMenu.ToggleInfo scheduleInfo;

	private KIconToggleMenu.ToggleInfo vitalsInfo;

	private KIconToggleMenu.ToggleInfo reportsInfo;

	private KIconToggleMenu.ToggleInfo researchInfo;

	private KIconToggleMenu.ToggleInfo codexInfo;

	private KIconToggleMenu.ToggleInfo rolesInfo;

	private KIconToggleMenu.ToggleInfo starmapInfo;

	private Dictionary<KIconToggleMenu.ToggleInfo, ManagementMenu.ScreenData> ScreenInfoMatch = new Dictionary<KIconToggleMenu.ToggleInfo, ManagementMenu.ScreenData>();

	public KButton[] CloseButtons;

	private string rolesTooltip;

	private string rolesTooltipDisabled;

	private string researchTooltip;

	private string researchTooltipDisabled;

	private string starmapTooltip;

	private string starmapTooltipDisabled;

	public class ScreenData
	{
		public KScreen screen;

		public KIconToggleMenu.ToggleInfo toggleInfo;

		public int tabIdx;
	}
}
