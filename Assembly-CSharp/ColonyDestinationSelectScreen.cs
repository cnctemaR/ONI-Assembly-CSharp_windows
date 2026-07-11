using System;
using Klei.CustomSettings;
using TMPro;
using UnityEngine;

public class ColonyDestinationSelectScreen : NewGameFlowScreen
{
	protected override void OnPrefabInit()
	{
		this.backButton.onClick += this.BackClicked;
		this.customizeButton.onClick += this.CustomizeClicked;
		this.launchButton.onClick += this.LaunchClicked;
		this.shuffleButton.onClick += this.ShuffleClicked;
		this.destinationMapPanel.OnAsteroidClicked += this.OnAsteroidClicked;
		this.random = new global::System.Random();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.newGameSettings.Init();
		this.newGameSettings.SetCloseAction(new global::System.Action(this.CustomizeClose));
		CustomGameSettings.Instance.OnSettingChanged += this.SettingChanged;
		this.ShuffleClicked();
	}

	protected override void OnCleanUp()
	{
		CustomGameSettings.Instance.OnSettingChanged -= this.SettingChanged;
		base.OnCleanUp();
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

	private void SettingChanged(SettingConfig config, SettingLevel level)
	{
		this.coordinate.text = CustomGameSettings.Instance.GetSettingsCoordinate();
		string setting = this.newGameSettings.GetSetting(CustomGameSettingConfigs.World);
		string setting2 = this.newGameSettings.GetSetting(CustomGameSettingConfigs.WorldgenSeed);
		int num;
		int.TryParse(setting2, out num);
		ColonyDestinationAsteroidData colonyDestinationAsteroidData = this.destinationMapPanel.SelectAsteroid(setting, num);
		DebugUtil.LogArgs(new object[] { "Selected asteroid", setting, num });
		this.destinationProperties.SetDescriptors(colonyDestinationAsteroidData.GetParamDescriptors());
		this.startLocationProperties.SetDescriptors(colonyDestinationAsteroidData.GetTraitDescriptors());
	}

	private void OnAsteroidClicked(ColonyDestinationAsteroidData asteroid)
	{
		this.newGameSettings.SetSetting(CustomGameSettingConfigs.World, asteroid.worldPath);
		this.ShuffleClicked();
	}

	public override void OnKeyDown(KButtonEvent e)
	{
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
}
