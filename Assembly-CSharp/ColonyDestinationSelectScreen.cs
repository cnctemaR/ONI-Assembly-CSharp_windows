using System;
using Klei.CustomSettings;
using ProcGen;
using STRINGS;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ColonyDestinationSelectScreen : NewGameFlowScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.backButton.onClick += this.BackClicked;
		this.customizeButton.onClick += this.CustomizeClicked;
		this.launchButton.onClick += this.LaunchClicked;
		this.shuffleButton.onClick += this.ShuffleClicked;
		this.storyTraitShuffleButton.onClick += this.StoryTraitShuffleClicked;
		this.storyTraitShuffleButton.gameObject.SetActive(Db.Get().Stories.Count > 5);
		this.destinationMapPanel.OnAsteroidClicked += this.OnAsteroidClicked;
		KInputTextField kinputTextField = this.coordinate;
		kinputTextField.onFocus = (global::System.Action)Delegate.Combine(kinputTextField.onFocus, new global::System.Action(this.CoordinateEditStarted));
		this.coordinate.onEndEdit.AddListener(new UnityAction<string>(this.CoordinateEditFinished));
		if (this.locationIcons != null)
		{
			bool cloudSavesAvailable = SaveLoader.GetCloudSavesAvailable();
			this.locationIcons.gameObject.SetActive(cloudSavesAvailable);
		}
		this.random = new KRandom();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.RefreshCloudSavePref();
		this.RefreshCloudLocalIcon();
		this.newGameSettingsPanel.Init();
		this.newGameSettingsPanel.SetCloseAction(new global::System.Action(this.CustomizeClose));
		this.destinationMapPanel.Init();
		this.mixingPanel.Init();
		this.ShuffleClicked();
		this.RefreshMenuTabs();
		for (int i = 0; i < this.menuTabs.Length; i++)
		{
			int target = i;
			this.menuTabs[i].onClick = delegate
			{
				this.selectedMenuTabIdx = target;
				this.RefreshMenuTabs();
			};
		}
		this.ResizeLayout();
		this.storyContentPanel.Init();
		this.storyContentPanel.SelectRandomStories(5, 5, true);
		this.storyContentPanel.SelectDefault();
		this.RefreshStoryLabel();
		this.RefreshRowsAndDescriptions();
		CustomGameSettings.Instance.OnQualitySettingChanged += this.QualitySettingChanged;
		CustomGameSettings.Instance.OnStorySettingChanged += this.QualitySettingChanged;
		CustomGameSettings.Instance.OnMixingSettingChanged += this.QualitySettingChanged;
	}

	private void ResizeLayout()
	{
		Vector2 sizeDelta = this.destinationProperties.clusterDetailsButton.rectTransform().sizeDelta;
		this.destinationProperties.clusterDetailsButton.rectTransform().sizeDelta = new Vector2(sizeDelta.x, (float)(DlcManager.FeatureClusterSpaceEnabled() ? 164 : 76));
		Vector2 sizeDelta2 = this.worldsScrollPanel.rectTransform().sizeDelta;
		Vector2 anchoredPosition = this.worldsScrollPanel.rectTransform().anchoredPosition;
		if (!DlcManager.FeatureClusterSpaceEnabled())
		{
			this.worldsScrollPanel.rectTransform().anchoredPosition = new Vector2(anchoredPosition.x, anchoredPosition.y + 88f);
		}
		float num = (float)(DlcManager.FeatureClusterSpaceEnabled() ? 436 : 524);
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.gameObject.rectTransform());
		num = Mathf.Min(num, this.destinationInfoPanel.sizeDelta.y - (float)(DlcManager.FeatureClusterSpaceEnabled() ? 164 : 76) - 22f);
		this.worldsScrollPanel.rectTransform().sizeDelta = new Vector2(sizeDelta2.x, num);
		this.storyScrollPanel.rectTransform().sizeDelta = new Vector2(sizeDelta2.x, num);
		this.mixingScrollPanel.rectTransform().sizeDelta = new Vector2(sizeDelta2.x, num);
		this.gameSettingsScrollPanel.rectTransform().sizeDelta = new Vector2(sizeDelta2.x, num);
	}

	protected override void OnCleanUp()
	{
		CustomGameSettings.Instance.OnQualitySettingChanged -= this.QualitySettingChanged;
		CustomGameSettings.Instance.OnStorySettingChanged -= this.QualitySettingChanged;
		this.newGameSettingsPanel.Uninit();
		this.destinationMapPanel.Uninit();
		this.mixingPanel.Uninit();
		this.storyContentPanel.Cleanup();
		base.OnCleanUp();
	}

	private void RefreshCloudLocalIcon()
	{
		if (this.locationIcons == null)
		{
			return;
		}
		if (!SaveLoader.GetCloudSavesAvailable())
		{
			return;
		}
		HierarchyReferences component = this.locationIcons.GetComponent<HierarchyReferences>();
		LocText component2 = component.GetReference<RectTransform>("LocationText").GetComponent<LocText>();
		KButton component3 = component.GetReference<RectTransform>("CloudButton").GetComponent<KButton>();
		KButton component4 = component.GetReference<RectTransform>("LocalButton").GetComponent<KButton>();
		ToolTip component5 = component3.GetComponent<ToolTip>();
		ToolTip component6 = component4.GetComponent<ToolTip>();
		component5.toolTip = string.Format("{0}\n{1}", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SAVETOCLOUD.TOOLTIP, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SAVETOCLOUD.TOOLTIP_EXTRA);
		component6.toolTip = string.Format("{0}\n{1}", UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SAVETOCLOUD.TOOLTIP_LOCAL, UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.SAVETOCLOUD.TOOLTIP_EXTRA);
		bool flag = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.SaveToCloud).id == "Enabled";
		component2.text = (flag ? UI.FRONTEND.LOADSCREEN.CLOUD_SAVE : UI.FRONTEND.LOADSCREEN.LOCAL_SAVE);
		component3.gameObject.SetActive(flag);
		component3.ClearOnClick();
		if (flag)
		{
			component3.onClick += delegate
			{
				CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.SaveToCloud, "Disabled");
				this.RefreshCloudLocalIcon();
			};
		}
		component4.gameObject.SetActive(!flag);
		component4.ClearOnClick();
		if (!flag)
		{
			component4.onClick += delegate
			{
				CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.SaveToCloud, "Enabled");
				this.RefreshCloudLocalIcon();
			};
		}
	}

	private void RefreshCloudSavePref()
	{
		if (!SaveLoader.GetCloudSavesAvailable())
		{
			return;
		}
		string cloudSavesDefaultPref = SaveLoader.GetCloudSavesDefaultPref();
		CustomGameSettings.Instance.SetQualitySetting(CustomGameSettingConfigs.SaveToCloud, cloudSavesDefaultPref);
	}

	private void BackClicked()
	{
		this.newGameSettingsPanel.Cancel();
		base.NavigateBackward();
	}

	private void CustomizeClicked()
	{
		this.newGameSettingsPanel.Refresh();
		this.customSettings.SetActive(true);
	}

	private void CustomizeClose()
	{
		this.customSettings.SetActive(false);
	}

	private void LaunchClicked()
	{
		CustomGameSettings.Instance.RemoveInvalidMixingSettings();
		base.NavigateForward();
	}

	private void RefreshMenuTabs()
	{
		for (int i = 0; i < this.menuTabs.Length; i++)
		{
			this.menuTabs[i].ChangeState((i == this.selectedMenuTabIdx) ? 1 : 0);
			LocText componentInChildren = this.menuTabs[i].GetComponentInChildren<LocText>();
			HierarchyReferences component = this.menuTabs[i].GetComponent<HierarchyReferences>();
			if (componentInChildren != null)
			{
				componentInChildren.color = ((i == this.selectedMenuTabIdx) ? Color.white : Color.grey);
			}
			if (component != null)
			{
				Image reference = component.GetReference<Image>("Icon");
				if (reference != null)
				{
					reference.color = ((i == this.selectedMenuTabIdx) ? Color.white : Color.grey);
				}
			}
		}
		this.destinationInfoPanel.gameObject.SetActive(this.selectedMenuTabIdx == 1);
		this.storyInfoPanel.gameObject.SetActive(this.selectedMenuTabIdx == 2);
		this.mixingSettingsPanel.gameObject.SetActive(this.selectedMenuTabIdx == 3);
		this.gameSettingsPanel.gameObject.SetActive(this.selectedMenuTabIdx == 4);
		int num = this.selectedMenuTabIdx;
		if (num != 1)
		{
			if (num == 2)
			{
				this.destinationDetailsHeader.SetParent(this.destinationDetailsParent_Story);
			}
		}
		else
		{
			this.destinationDetailsHeader.SetParent(this.destinationDetailsParent_Asteroid);
		}
		this.destinationDetailsHeader.SetAsFirstSibling();
	}

	private void ShuffleClicked()
	{
		ClusterLayout currentClusterLayout = CustomGameSettings.Instance.GetCurrentClusterLayout();
		int num = this.random.Next();
		if (currentClusterLayout != null && currentClusterLayout.fixedCoordinate != -1)
		{
			num = currentClusterLayout.fixedCoordinate;
		}
		this.newGameSettingsPanel.SetSetting(CustomGameSettingConfigs.WorldgenSeed, num.ToString(), true);
	}

	private void StoryTraitShuffleClicked()
	{
		this.storyContentPanel.SelectRandomStories(5, 5, false);
	}

	private void CoordinateChanged(string text)
	{
		string[] array = CustomGameSettings.ParseSettingCoordinate(text);
		if (array.Length < 4 || array.Length > 6)
		{
			return;
		}
		int num;
		if (!int.TryParse(array[2], out num))
		{
			return;
		}
		ClusterLayout clusterLayout = null;
		foreach (string text2 in SettingsCache.GetClusterNames())
		{
			ClusterLayout clusterData = SettingsCache.clusterLayouts.GetClusterData(text2);
			if (clusterData.coordinatePrefix == array[1])
			{
				clusterLayout = clusterData;
			}
		}
		if (clusterLayout != null)
		{
			this.newGameSettingsPanel.SetSetting(CustomGameSettingConfigs.ClusterLayout, clusterLayout.filePath, true);
		}
		this.newGameSettingsPanel.SetSetting(CustomGameSettingConfigs.WorldgenSeed, array[2], true);
		this.newGameSettingsPanel.ConsumeSettingsCode(array[3]);
		string text3 = ((array.Length >= 5) ? array[4] : "0");
		this.newGameSettingsPanel.ConsumeStoryTraitsCode(text3);
		string text4 = ((array.Length >= 6) ? array[5] : "0");
		this.newGameSettingsPanel.ConsumeMixingSettingsCode(text4);
	}

	private void CoordinateEditStarted()
	{
		this.isEditingCoordinate = true;
	}

	private void CoordinateEditFinished(string text)
	{
		this.CoordinateChanged(text);
		this.isEditingCoordinate = false;
		this.coordinate.text = CustomGameSettings.Instance.GetSettingsCoordinate();
	}

	private void QualitySettingChanged(SettingConfig config, SettingLevel level)
	{
		if (config == CustomGameSettingConfigs.SaveToCloud)
		{
			this.RefreshCloudLocalIcon();
		}
		if (!this.destinationDetailsHeader.IsNullOrDestroyed())
		{
			if (!this.isEditingCoordinate && !this.coordinate.IsNullOrDestroyed())
			{
				this.coordinate.text = CustomGameSettings.Instance.GetSettingsCoordinate();
			}
			this.RefreshRowsAndDescriptions();
		}
	}

	public void RefreshRowsAndDescriptions()
	{
		string setting = this.newGameSettingsPanel.GetSetting(CustomGameSettingConfigs.ClusterLayout);
		int num;
		int.TryParse(this.newGameSettingsPanel.GetSetting(CustomGameSettingConfigs.WorldgenSeed), out num);
		int fixedCoordinate = CustomGameSettings.Instance.GetCurrentClusterLayout().fixedCoordinate;
		if (fixedCoordinate != -1)
		{
			this.newGameSettingsPanel.SetSetting(CustomGameSettingConfigs.WorldgenSeed, fixedCoordinate.ToString(), false);
			num = fixedCoordinate;
			this.shuffleButton.isInteractable = false;
			this.shuffleButton.GetComponent<ToolTip>().SetSimpleTooltip(UI.FRONTEND.COLONYDESTINATIONSCREEN.SHUFFLETOOLTIP_DISABLED);
		}
		else
		{
			this.coordinate.interactable = true;
			this.shuffleButton.isInteractable = true;
			this.shuffleButton.GetComponent<ToolTip>().SetSimpleTooltip(UI.FRONTEND.COLONYDESTINATIONSCREEN.SHUFFLETOOLTIP);
		}
		ColonyDestinationAsteroidBeltData cluster;
		try
		{
			cluster = this.destinationMapPanel.SelectCluster(setting, num);
		}
		catch
		{
			string defaultAsteroid = this.destinationMapPanel.GetDefaultAsteroid();
			this.newGameSettingsPanel.SetSetting(CustomGameSettingConfigs.ClusterLayout, defaultAsteroid, true);
			cluster = this.destinationMapPanel.SelectCluster(defaultAsteroid, num);
		}
		if (DlcManager.IsContentSubscribed("EXPANSION1_ID"))
		{
			this.destinationProperties.EnableClusterLocationLabels(true);
			this.destinationProperties.RefreshAsteroidLines(cluster, this.selectedLocationProperties, this.storyContentPanel.GetActiveStories());
			this.destinationProperties.EnableClusterDetails(true);
			this.destinationProperties.SetClusterDetailLabels(cluster);
			this.selectedLocationProperties.headerLabel.SetText(UI.FRONTEND.COLONYDESTINATIONSCREEN.SELECTED_CLUSTER_TRAITS_HEADER);
			this.destinationProperties.clusterDetailsButton.onClick = delegate
			{
				this.destinationProperties.SelectWholeClusterDetails(cluster, this.selectedLocationProperties, this.storyContentPanel.GetActiveStories());
			};
		}
		else
		{
			this.destinationProperties.EnableClusterDetails(false);
			this.destinationProperties.EnableClusterLocationLabels(false);
			this.destinationProperties.SetParameterDescriptors(cluster.GetParamDescriptors());
			this.selectedLocationProperties.SetTraitDescriptors(cluster.GetTraitDescriptors(), this.storyContentPanel.GetActiveStories(), true);
		}
		this.RefreshStoryLabel();
	}

	public void RefreshStoryLabel()
	{
		this.storyTraitsDestinationDetailsLabel.SetText(this.storyContentPanel.GetTraitsString(false));
		this.storyTraitsDestinationDetailsLabel.GetComponent<ToolTip>().SetSimpleTooltip(this.storyContentPanel.GetTraitsString(true));
	}

	private void OnAsteroidClicked(ColonyDestinationAsteroidBeltData cluster)
	{
		this.newGameSettingsPanel.SetSetting(CustomGameSettingConfigs.ClusterLayout, cluster.beltPath, true);
		this.ShuffleClicked();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (this.isEditingCoordinate)
		{
			return;
		}
		if (!e.Consumed && e.TryConsume(global::Action.PanLeft))
		{
			this.destinationMapPanel.ScrollLeft();
		}
		else if (!e.Consumed && e.TryConsume(global::Action.PanRight))
		{
			this.destinationMapPanel.ScrollRight();
		}
		else if (this.customSettings.activeSelf && !e.Consumed && (e.TryConsume(global::Action.Escape) || e.TryConsume(global::Action.MouseRight)))
		{
			this.CustomizeClose();
		}
		base.OnKeyDown(e);
	}

	[SerializeField]
	private GameObject destinationMap;

	[SerializeField]
	private GameObject customSettings;

	[Header("Menu")]
	[SerializeField]
	private MultiToggle[] menuTabs;

	private int selectedMenuTabIdx = 1;

	[Header("Buttons")]
	[SerializeField]
	private KButton backButton;

	[SerializeField]
	private KButton customizeButton;

	[SerializeField]
	private KButton launchButton;

	[SerializeField]
	private KButton shuffleButton;

	[SerializeField]
	private KButton storyTraitShuffleButton;

	[Header("Scroll Panels")]
	[SerializeField]
	private RectTransform worldsScrollPanel;

	[SerializeField]
	private RectTransform storyScrollPanel;

	[SerializeField]
	private RectTransform mixingScrollPanel;

	[SerializeField]
	private RectTransform gameSettingsScrollPanel;

	[Header("Panels")]
	[SerializeField]
	private RectTransform destinationDetailsHeader;

	[SerializeField]
	private RectTransform destinationInfoPanel;

	[SerializeField]
	private RectTransform storyInfoPanel;

	[SerializeField]
	private RectTransform mixingSettingsPanel;

	[SerializeField]
	private RectTransform gameSettingsPanel;

	[Header("References")]
	[SerializeField]
	private RectTransform destinationDetailsParent_Asteroid;

	[SerializeField]
	private RectTransform destinationDetailsParent_Story;

	[SerializeField]
	private LocText storyTraitsDestinationDetailsLabel;

	[SerializeField]
	private HierarchyReferences locationIcons;

	[SerializeField]
	private KInputTextField coordinate;

	[SerializeField]
	private StoryContentPanel storyContentPanel;

	[SerializeField]
	private AsteroidDescriptorPanel destinationProperties;

	[SerializeField]
	private AsteroidDescriptorPanel selectedLocationProperties;

	private const int DESTINATION_HEADER_BUTTON_HEIGHT_CLUSTER = 164;

	private const int DESTINATION_HEADER_BUTTON_HEIGHT_BASE = 76;

	private const int WORLDS_SCROLL_PANEL_HEIGHT_CLUSTER = 436;

	private const int WORLDS_SCROLL_PANEL_HEIGHT_BASE = 524;

	[SerializeField]
	private NewGameSettingsPanel newGameSettingsPanel;

	[MyCmpReq]
	private DestinationSelectPanel destinationMapPanel;

	[SerializeField]
	private MixingContentPanel mixingPanel;

	private KRandom random;

	private bool isEditingCoordinate;
}
