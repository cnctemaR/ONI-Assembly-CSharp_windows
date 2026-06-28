using System;
using System.Collections.Generic;
using FMOD.Studio;
using STRINGS;

public class ManagementMenu : KIconToggleMenu
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		ManagementMenu.Instance = this;
		this.instantiator.Instantiate();
		this.jobsScreen = this.instantiator.GetComponentInChildren<JobsTableScreen>();
		this.jobsScreen.gameObject.SetActive(false);
		this.consumablesScreen = this.instantiator.GetComponentInChildren<ConsumablesTableScreen>();
		this.consumablesScreen.gameObject.SetActive(false);
		this.vitalsScreen = this.instantiator.GetComponentInChildren<VitalsTableScreen>();
		this.vitalsScreen.gameObject.SetActive(false);
		base.Subscribe(Game.Instance.gameObject, 288942073, new Action<object>(this.OnUIClear));
		this.jobsInfo = new KIconToggleMenu.ToggleInfo(UI.JOBS, "OverviewUI_jobs_icon", null, global::Action.ManagePeople, UI.TOOLTIPS.MANAGEMENTMENU_JOBS, "");
		this.consumablesInfo = new KIconToggleMenu.ToggleInfo(UI.CONSUMABLES, "OverviewUI_consumables_icon", null, global::Action.ManageConsumables, UI.TOOLTIPS.MANAGEMENTMENU_CONSUMABLES, "");
		this.vitalsInfo = new KIconToggleMenu.ToggleInfo(UI.VITALS, "OverviewUI_vitals_icon", null, global::Action.ManageVitals, UI.TOOLTIPS.MANAGEMENTMENU_VITALS, "");
		this.reportsInfo = new KIconToggleMenu.ToggleInfo(UI.REPORT, "OverviewUI_reports_icon", null, global::Action.ManageReport, UI.TOOLTIPS.MANAGEMENTMENU_DAILYREPORT, "");
		this.ResearchInfo = new KIconToggleMenu.ToggleInfo(UI.RESEARCH, "OverviewUI_research_nav_icon", null, global::Action.ManageResearch, UI.TOOLTIPS.MANAGEMENTMENU_RESEARCH, "");
		this.ScreenInfoMatch.Add(this.jobsInfo, new ManagementMenu.ScreenData
		{
			screen = this.jobsScreen,
			tabIdx = 0,
			toggleInfo = this.jobsInfo
		});
		this.ScreenInfoMatch.Add(this.vitalsInfo, new ManagementMenu.ScreenData
		{
			screen = this.vitalsScreen,
			tabIdx = 1,
			toggleInfo = this.vitalsInfo
		});
		this.ScreenInfoMatch.Add(this.consumablesInfo, new ManagementMenu.ScreenData
		{
			screen = this.consumablesScreen,
			tabIdx = 2,
			toggleInfo = this.consumablesInfo
		});
		this.ScreenInfoMatch.Add(this.reportsInfo, new ManagementMenu.ScreenData
		{
			screen = this.reportsScreen,
			tabIdx = 3,
			toggleInfo = this.reportsInfo
		});
		base.Setup(new List<KIconToggleMenu.ToggleInfo> { this.jobsInfo, this.consumablesInfo, this.vitalsInfo, this.reportsInfo, this.ResearchInfo });
		base.onSelect += this.OnButtonClick;
		foreach (KeyValuePair<KIconToggleMenu.ToggleInfo, ManagementMenu.ScreenData> keyValuePair in this.ScreenInfoMatch)
		{
			keyValuePair.Value.screen.Show(false);
		}
		Components.Cmps<ResearchCenter> researchCenters = Components.ResearchCenters;
		researchCenters.OnAdd = (Action<ResearchCenter>)Delegate.Combine(researchCenters.OnAdd, new Action<ResearchCenter>(this.CheckResearch));
		Components.Cmps<ResearchCenter> researchCenters2 = Components.ResearchCenters;
		researchCenters2.OnRemove = (Action<ResearchCenter>)Delegate.Combine(researchCenters2.OnRemove, new Action<ResearchCenter>(this.CheckResearch));
		Game.Instance.Subscribe(-809948329, new Action<object>(this.CheckResearch));
		this.CheckResearch(null);
		this.ResearchInfo.toggle.soundPlayer.AcceptClickCondition = () => this.ResearchAvailable() || this.activeScreen == this.ScreenInfoMatch[ManagementMenu.Instance.ResearchInfo];
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
		if (!(this.ResearchScreen != null))
		{
			this.ResearchScreen = researchScreen;
			this.ResearchScreen.gameObject.SetActive(false);
			this.ScreenInfoMatch.Add(this.ResearchInfo, new ManagementMenu.ScreenData
			{
				screen = this.ResearchScreen,
				tabIdx = 0,
				toggleInfo = this.ResearchInfo
			});
			this.ResearchScreen.Show(false);
		}
	}

	public void CheckResearch(object o)
	{
		if (!(this.ResearchInfo.toggle == null))
		{
			if (Components.ResearchCenters.Count <= 0 && !DebugHandler.InstantBuildMode)
			{
				this.ResearchInfo.toggle.gameObject.GetComponentInChildren<ImageToggleState>().SetDisabled();
				ToolTip component = this.ResearchInfo.toggle.gameObject.GetComponent<ToolTip>();
				component.ClearMultiStringTooltip();
				component.AddMultiStringTooltip(UI.TOOLTIPS.MANAGEMENTMENU_REQUIRES_RESEARCH, this.ToggleToolTipTextStyleSetting);
			}
			else
			{
				if (this.activeScreen != null && this.activeScreen.toggleInfo == this.ResearchInfo)
				{
					this.ResearchInfo.toggle.gameObject.GetComponentInChildren<ImageToggleState>().SetActive();
				}
				else
				{
					this.ResearchInfo.toggle.gameObject.GetComponentInChildren<ImageToggleState>().SetInactive();
				}
				this.ResearchInfo.toggle.gameObject.GetComponent<ToolTip>().ClearMultiStringTooltip();
				this.ResearchInfo.toggle.gameObject.GetComponent<ToolTip>().AddMultiStringTooltip(UI.TOOLTIPS.MANAGEMENTMENU_RESEARCH + " " + GameUtil.GetHotkeyString(global::Action.ManageResearch), this.ToggleToolTipTextStyleSetting);
			}
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.activeScreen != null)
		{
			if (e.TryConsume(global::Action.Escape))
			{
				this.ToggleScreen(this.activeScreen);
			}
		}
		if (!e.Consumed)
		{
			base.OnKeyDown(e);
		}
	}

	public override void OnKeyUp(KButtonEvent e)
	{
		if (this.activeScreen != null)
		{
			if (PlayerController.Instance.ConsumeIfNotDragging(e, global::Action.MouseRight))
			{
				this.ToggleScreen(this.activeScreen);
			}
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

	public void CloseAll()
	{
		if (this.activeScreen != null)
		{
			if (this.activeScreen.toggleInfo != null)
			{
				this.ToggleScreen(this.activeScreen);
			}
			this.CloseActive();
			this.ClearSelection();
		}
	}

	private void OnUIClear(object data)
	{
		this.CloseAll();
	}

	public void ToggleScreen(ManagementMenu.ScreenData screenData)
	{
		if (screenData != null && screenData.toggleInfo == this.ResearchInfo && !this.ResearchAvailable())
		{
			this.CheckResearch(null);
			this.CloseActive();
		}
		else if (!screenData.toggleInfo.toggle.gameObject.GetComponentInChildren<ImageToggleState>().IsDisabled)
		{
			if (this.activeScreen != null)
			{
				this.activeScreen.toggleInfo.toggle.isOn = false;
				this.activeScreen.toggleInfo.toggle.gameObject.GetComponentInChildren<ImageToggleState>().SetInactive();
			}
			if (this.activeScreen != screenData)
			{
				OverlayScreen.Instance.ToggleOverlay(SimViewMode.None);
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
		if (this.ResearchAvailable() || this.activeScreen == this.ScreenInfoMatch[ManagementMenu.Instance.ResearchInfo])
		{
			if (this.ResearchInfo != null)
			{
				this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.ResearchInfo]);
			}
		}
	}

	public void OpenReports(int day)
	{
		if (this.activeScreen != this.ScreenInfoMatch[ManagementMenu.Instance.reportsInfo])
		{
			this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.reportsInfo]);
		}
		ReportScreen.Instance.ShowReport(day);
	}

	private ManagementMenu.ScreenData activeScreen;

	private KButton activeButton;

	public static ManagementMenu Instance;

	public KScreen ResearchScreen;

	private KScreen jobsScreen;

	public KScreen vitalsScreen;

	public KScreen scheduleScreen;

	public KScreen reportsScreen;

	private KScreen consumablesScreen;

	public string colourSchemeDisabled;

	public InstantiateUIPrefabChild instantiator;

	private KIconToggleMenu.ToggleInfo jobsInfo;

	private KIconToggleMenu.ToggleInfo consumablesInfo;

	private KIconToggleMenu.ToggleInfo scheduleInfo;

	private KIconToggleMenu.ToggleInfo vitalsInfo;

	private KIconToggleMenu.ToggleInfo reportsInfo;

	private KIconToggleMenu.ToggleInfo ResearchInfo;

	private Dictionary<KIconToggleMenu.ToggleInfo, ManagementMenu.ScreenData> ScreenInfoMatch = new Dictionary<KIconToggleMenu.ToggleInfo, ManagementMenu.ScreenData>();

	public KButton[] CloseButtons;

	public class ScreenData
	{
		public KScreen screen;

		public KIconToggleMenu.ToggleInfo toggleInfo;

		public int tabIdx;
	}
}
