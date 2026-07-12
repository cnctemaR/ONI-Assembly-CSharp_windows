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
		this.storyTraitShuffleButton.gameObject.SetActive(Db.Get().Stories.Count > 2);
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
		this.newGameSettings.Init();
		this.newGameSettings.SetCloseAction(new global::System.Action(this.CustomizeClose));
		this.destinationMapPanel.Init();
		CustomGameSettings.Instance.OnQualitySettingChanged += this.QualitySettingChanged;
		CustomGameSettings.Instance.OnStorySettingChanged += this.QualitySettingChanged;
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
		this.storyContentPanel.SelectRandomStories(2, 2, true);
		this.storyContentPanel.SelectDefault();
		this.RefreshStoryLabel();
		this.RefreshRowsAndDescriptions();
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
	}

	protected override void OnCleanUp()
	{
		CustomGameSettings.Instance.OnQualitySettingChanged -= this.QualitySettingChanged;
		CustomGameSettings.Instance.OnStorySettingChanged -= this.QualitySettingChanged;
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
		this.newGameSettings.Cancel();
		base.NavigateBackward();
	}

	private void CustomizeClicked()
	{
		this.newGameSettings.Refresh();
		this.customSettings.SetActive(true);
	}

	private void CustomizeClose()
	{
		this.customSettings.SetActive(false);
	}

	private void LaunchClicked()
	{
		base.NavigateForward();
	}

	private void RefreshMenuTabs()
	{
		for (int i = 0; i < this.menuTabs.Length; i++)
		{
			this.menuTabs[i].ChangeState((i == this.selectedMenuTabIdx) ? 1 : 0);
			this.menuTabs[i].GetComponentInChildren<LocText>().color = ((i == this.selectedMenuTabIdx) ? Color.white : Color.grey);
		}
		this.destinationInfoPanel.gameObject.SetActive(this.selectedMenuTabIdx == 0);
		this.storyInfoPanel.gameObject.SetActive(this.selectedMenuTabIdx == 1);
		this.destinationDetailsHeader.SetParent((this.selectedMenuTabIdx == 0) ? this.destinationDetailsParent_Asteroid : this.destinationDetailsParent_Story);
		this.destinationDetailsHeader.SetAsFirstSibling();
	}

	private void ShuffleClicked()
	{
		int num = this.random.Next();
		this.newGameSettings.SetSetting(CustomGameSettingConfigs.WorldgenSeed, num.ToString());
	}

	private void StoryTraitShuffleClicked()
	{
		this.storyContentPanel.SelectRandomStories(2, 2, false);
	}

	private void CoordinateChanged(string text)
	{
		string[] array = CustomGameSettings.ParseSettingCoordinate(text);
		if (array.Length != 4 && array.Length != 5)
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
			this.newGameSettings.SetSetting(CustomGameSettingConfigs.ClusterLayout, clusterLayout.filePath);
		}
		this.newGameSettings.SetSetting(CustomGameSettingConfigs.WorldgenSeed, array[2]);
		this.newGameSettings.ConsumeSettingsCode(array[3]);
		string text3 = ((array.Length >= 5) ? array[4] : "0");
		this.newGameSettings.ConsumeStoryTraitsCode(text3);
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
		if (!this.isEditingCoordinate)
		{
			this.coordinate.text = CustomGameSettings.Instance.GetSettingsCoordinate();
		}
		this.RefreshRowsAndDescriptions();
	}

	public void RefreshRowsAndDescriptions()
	{
		string setting = this.newGameSettings.GetSetting(CustomGameSettingConfigs.ClusterLayout);
		string setting2 = this.newGameSettings.GetSetting(CustomGameSettingConfigs.WorldgenSeed);
		this.destinationMapPanel.UpdateDisplayedClusters();
		int num;
		int.TryParse(setting2, out num);
		ColonyDestinationAsteroidBeltData cluster;
		try
		{
			cluster = this.destinationMapPanel.SelectCluster(setting, num);
		}
		catch
		{
			string defaultAsteroid = this.destinationMapPanel.GetDefaultAsteroid();
			this.newGameSettings.SetSetting(CustomGameSettingConfigs.ClusterLayout, defaultAsteroid);
			cluster = this.destinationMapPanel.SelectCluster(defaultAsteroid, num);
		}
		if (DlcManager.IsContentActive("EXPANSION1_ID"))
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
		this.newGameSettings.SetSetting(CustomGameSettingConfigs.ClusterLayout, cluster.beltPath);
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

	[SerializeField]
	private MultiToggle[] menuTabs;

	private int selectedMenuTabIdx;

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

	[SerializeField]
	private HierarchyReferences locationIcons;

	[SerializeField]
	private RectTransform worldsScrollPanel;

	[SerializeField]
	private RectTransform storyScrollPanel;

	[SerializeField]
	private RectTransform destinationDetailsParent_Asteroid;

	[SerializeField]
	private RectTransform destinationDetailsParent_Story;

	[SerializeField]
	private LocText storyTraitsDestinationDetailsLabel;

	private const int DESTINATION_HEADER_BUTTON_HEIGHT_CLUSTER = 164;

	private const int DESTINATION_HEADER_BUTTON_HEIGHT_BASE = 76;

	private const int WORLDS_SCROLL_PANEL_HEIGHT_CLUSTER = 436;

	private const int WORLDS_SCROLL_PANEL_HEIGHT_BASE = 524;

	[SerializeField]
	private AsteroidDescriptorPanel destinationProperties;

	[SerializeField]
	private AsteroidDescriptorPanel selectedLocationProperties;

	[SerializeField]
	private KInputTextField coordinate;

	[SerializeField]
	private RectTransform destinationDetailsHeader;

	[SerializeField]
	private RectTransform destinationInfoPanel;

	[SerializeField]
	private RectTransform storyInfoPanel;

	[MyCmpReq]
	public NewGameSettingsPanel newGameSettings;

	[MyCmpReq]
	private DestinationSelectPanel destinationMapPanel;

	[SerializeField]
	private StoryContentPanel storyContentPanel;

	private KRandom random;

	private bool isEditingCoordinate;
}
