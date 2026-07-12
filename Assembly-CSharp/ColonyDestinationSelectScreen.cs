using System;
using Klei.CustomSettings;
using ProcGen;
using STRINGS;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ColonyDestinationSelectScreen : NewGameFlowScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.backButton.onClick += this.BackClicked;
		this.customizeButton.onClick += this.CustomizeClicked;
		this.launchButton.onClick += this.LaunchClicked;
		this.shuffleButton.onClick += this.ShuffleClicked;
		this.destinationMapPanel.OnAsteroidClicked += this.OnAsteroidClicked;
		TMP_InputField tmp_InputField = this.coordinate;
		tmp_InputField.onFocus = (global::System.Action)Delegate.Combine(tmp_InputField.onFocus, new global::System.Action(this.CoordinateEditStarted));
		this.coordinate.onEndEdit.AddListener(new UnityAction<string>(this.CoordinateEditFinished));
		if (this.locationIcons != null)
		{
			bool cloudSavesAvailable = SaveLoader.GetCloudSavesAvailable();
			this.locationIcons.gameObject.SetActive(cloudSavesAvailable);
		}
		this.random = new global::System.Random();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.RefreshCloudSavePref();
		this.RefreshCloudLocalIcon();
		this.newGameSettings.Init();
		this.newGameSettings.SetCloseAction(new global::System.Action(this.CustomizeClose));
		this.destinationMapPanel.Init();
		CustomGameSettings.Instance.OnSettingChanged += this.SettingChanged;
		this.ShuffleClicked();
	}

	protected override void OnCleanUp()
	{
		CustomGameSettings.Instance.OnSettingChanged -= this.SettingChanged;
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

	private void ShuffleClicked()
	{
		int num = this.random.Next();
		this.newGameSettings.SetSetting(CustomGameSettingConfigs.WorldgenSeed, num.ToString());
	}

	private void CoordinateChanged(string text)
	{
		string[] array = CustomGameSettings.ParseSettingCoordinate(text);
		if (array.Length != 4)
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

	private void SettingChanged(SettingConfig config, SettingLevel level)
	{
		if (config == CustomGameSettingConfigs.SaveToCloud)
		{
			this.RefreshCloudLocalIcon();
		}
		if (!this.isEditingCoordinate)
		{
			this.coordinate.text = CustomGameSettings.Instance.GetSettingsCoordinate();
		}
		string setting = this.newGameSettings.GetSetting(CustomGameSettingConfigs.ClusterLayout);
		string setting2 = this.newGameSettings.GetSetting(CustomGameSettingConfigs.WorldgenSeed);
		this.destinationMapPanel.UpdateDisplayedClusters();
		int num;
		int.TryParse(setting2, out num);
		ColonyDestinationAsteroidBeltData colonyDestinationAsteroidBeltData;
		try
		{
			colonyDestinationAsteroidBeltData = this.destinationMapPanel.SelectAsteroid(setting, num);
		}
		catch
		{
			string defaultAsteroid = this.destinationMapPanel.GetDefaultAsteroid();
			this.newGameSettings.SetSetting(CustomGameSettingConfigs.ClusterLayout, defaultAsteroid);
			colonyDestinationAsteroidBeltData = this.destinationMapPanel.SelectAsteroid(defaultAsteroid, num);
		}
		this.destinationProperties.SetDescriptors(colonyDestinationAsteroidBeltData.GetParamDescriptors());
		this.startLocationProperties.SetDescriptors(colonyDestinationAsteroidBeltData.GetTraitDescriptors());
	}

	private void OnAsteroidClicked(ColonyDestinationAsteroidBeltData asteroid)
	{
		this.newGameSettings.SetSetting(CustomGameSettingConfigs.ClusterLayout, asteroid.beltPath);
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
		else if (this.customSettings.activeSelf && !e.Consumed && e.TryConsume(global::Action.Escape))
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
	private KButton backButton;

	[SerializeField]
	private KButton customizeButton;

	[SerializeField]
	private KButton launchButton;

	[SerializeField]
	private KButton shuffleButton;

	[SerializeField]
	private HierarchyReferences locationIcons;

	[SerializeField]
	private AsteroidDescriptorPanel destinationProperties;

	[SerializeField]
	private AsteroidDescriptorPanel startLocationProperties;

	[SerializeField]
	private TMP_InputField coordinate;

	[MyCmpReq]
	private NewGameSettingsPanel newGameSettings;

	[MyCmpReq]
	private DestinationSelectPanel destinationMapPanel;

	private global::System.Random random;

	private bool isEditingCoordinate;
}
