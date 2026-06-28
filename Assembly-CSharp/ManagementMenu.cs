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
		this.overviewScreen.gameObject.SetActive(false);
		base.Subscribe(Game.Instance.gameObject, 288942073, new EventSystem.EventHandler(this.OnUIClear));
		this.overviewInfo = new KIconToggleMenu.ToggleInfo(UI.JOBS, "OverviewUI_jobs_icon", null, global::Action.ManagePeople, UI.TOOLTIPS.MANAGEMENTMENU_JOBS);
		this.vitalsInfo = new KIconToggleMenu.ToggleInfo(UI.VITALS, "OverviewUI_vitals_icon", null, global::Action.ManageVitals, UI.TOOLTIPS.MANAGEMENTMENU_VITALS);
		this.reportsInfo = new KIconToggleMenu.ToggleInfo(UI.REPORT, "OverviewUI_reports_icon", null, global::Action.ManageReport, UI.TOOLTIPS.MANAGEMENTMENU_DAILYREPORT);
		this.ResearchInfo = new KIconToggleMenu.ToggleInfo(UI.RESEARCH, "OverviewUI_research_nav_icon", null, global::Action.ManageResearch, UI.TOOLTIPS.MANAGEMENTMENU_RESEARCH);
		this.ScreenInfoMatch.Add(this.overviewInfo, new ManagementMenu.ScreenData
		{
			screen = this.overviewScreen,
			tabIdx = 0,
			toggleInfo = this.overviewInfo
		});
		this.ScreenInfoMatch.Add(this.vitalsInfo, new ManagementMenu.ScreenData
		{
			screen = this.overviewScreen,
			tabIdx = 1,
			toggleInfo = this.vitalsInfo
		});
		this.ScreenInfoMatch.Add(this.reportsInfo, new ManagementMenu.ScreenData
		{
			screen = this.reportsScreen,
			tabIdx = 0,
			toggleInfo = this.reportsInfo
		});
		base.Setup(new List<KIconToggleMenu.ToggleInfo> { this.overviewInfo, this.vitalsInfo, this.reportsInfo, this.ResearchInfo });
		base.onSelect += this.OnButtonClick;
		foreach (KeyValuePair<KIconToggleMenu.ToggleInfo, ManagementMenu.ScreenData> keyValuePair in this.ScreenInfoMatch)
		{
			keyValuePair.Value.screen.Show(false);
		}
		Components.Cmps<ResearchCenter> researchCenters = Components.ResearchCenters;
		researchCenters.OnAdd = (Action<ResearchCenter>)Delegate.Combine(researchCenters.OnAdd, new Action<ResearchCenter>(this.CheckResearch));
		Components.Cmps<ResearchCenter> researchCenters2 = Components.ResearchCenters;
		researchCenters2.OnRemove = (Action<ResearchCenter>)Delegate.Combine(researchCenters2.OnRemove, new Action<ResearchCenter>(this.CheckResearch));
		Game.Instance.Subscribe(-809948329, new EventSystem.EventHandler(this.CheckResearch));
		this.CheckResearch(null);
		foreach (KButton kbutton in this.CloseButtons)
		{
			kbutton.onClick += this.CloseAll;
		}
	}

	public void AddResearchScreen(ResearchScreen researchScreen)
	{
		if (this.ResearchScreen != null)
		{
			return;
		}
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

	public void CheckResearch(object o)
	{
		if (this.ResearchInfo.toggle == null)
		{
			return;
		}
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

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.activeScreen != null)
		{
			if (e.TryConsume(global::Action.MouseRight))
			{
				if (this.activeScreen != null)
				{
					this.ToggleScreen(this.activeScreen);
				}
			}
			else if (e.TryConsume(global::Action.Escape))
			{
				if (this.activeScreen != null)
				{
					this.ToggleScreen(this.activeScreen);
				}
			}
			else
			{
				base.OnKeyDown(e);
			}
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private bool ResearchAvailable()
	{
		return Components.ResearchCenters.Count > 0 || DebugHandler.InstantBuildMode;
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
		if (screenData != null && screenData.toggleInfo == this.ResearchInfo && !this.ResearchAvailable())
		{
			KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative", false));
			this.CloseActive();
			return;
		}
		if (screenData.toggleInfo.toggle.gameObject.GetComponentInChildren<ImageToggleState>().IsDisabled)
		{
			return;
		}
		if (this.activeScreen != null)
		{
			this.activeScreen.toggleInfo.toggle.gameObject.GetComponentInChildren<ImageToggleState>().SetInactive();
		}
		if (this.activeScreen != screenData)
		{
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
			OverlayScreen.Instance.DisableCurrentOverlay();
			KTabMenu componentInChildren = this.activeScreen.screen.GetComponentInChildren<KTabMenu>();
			if (componentInChildren != null)
			{
				componentInChildren.ActivateTab(screenData.tabIdx);
			}
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
			this.activeScreen.screen.Show(false);
			this.activeScreen = null;
		}
	}

	public void ToggleResearch()
	{
		if ((this.ResearchAvailable() || this.activeScreen == this.ScreenInfoMatch[ManagementMenu.Instance.ResearchInfo]) && this.ResearchInfo != null)
		{
			this.ToggleScreen(this.ScreenInfoMatch[ManagementMenu.Instance.ResearchInfo]);
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

	public KScreen overviewScreen;

	public KScreen scheduleScreen;

	public KScreen reportsScreen;

	public string colourSchemeDisabled;

	public InstantiateUIPrefabChild instantiator;

	private KIconToggleMenu.ToggleInfo overviewInfo;

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
