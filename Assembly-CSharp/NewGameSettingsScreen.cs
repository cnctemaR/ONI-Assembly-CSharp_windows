using System;
using System.Collections.Generic;
using System.IO;
using FMOD.Studio;
using ProcGenGame;
using UnityEngine;

public class NewGameSettingsScreen : KScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		MultiToggle multiToggle = this.toggle_standard_game;
		multiToggle.onClick = (global::System.Action)Delegate.Combine(multiToggle.onClick, new global::System.Action(delegate
		{
			this.SetGameTypeToggle(false);
		}));
		MultiToggle multiToggle2 = this.toggle_custom_game;
		multiToggle2.onClick = (global::System.Action)Delegate.Combine(multiToggle2.onClick, new global::System.Action(delegate
		{
			this.SetGameTypeToggle(true);
		}));
		this.button_start.onClick += delegate
		{
			this.NewGame();
		};
		this.button_cancel.onClick += delegate
		{
			base.Show(false);
		};
		this.button_close.onClick += delegate
		{
			base.Show(false);
		};
		this.settings = global::UnityEngine.Object.FindObjectOfType<CustomGameSettings>();
		this.SetGameTypeToggle(false);
		foreach (KeyValuePair<string, CustomGameSettings.SettingConfig> keyValuePair in this.settings.QualitySettings)
		{
			GameObject gameObject = global::Util.KInstantiateUI(this.prefab_cycle_setting, this.content.gameObject, true);
			HierarchyReferences refs = gameObject.GetComponent<HierarchyReferences>();
			refs.GetReference<LocText>("Label").text = keyValuePair.Value.label;
			refs.GetReference<LocText>("Label").GetComponent<ToolTip>().toolTip = keyValuePair.Value.tooltip;
			refs.GetReference<LocText>("ValueLabel").text = this.settings.GetSettingLevelLabel(keyValuePair.Value.id, this.settings.CurrentQualityLevelsBySetting[keyValuePair.Value.id]);
			refs.GetReference<LocText>("ValueLabel").GetComponent<ToolTip>().toolTip = this.settings.GetSettingLevelTooltip(keyValuePair.Value.id, this.settings.CurrentQualityLevelsBySetting[keyValuePair.Value.id]);
			string key = keyValuePair.Key;
			refs.GetReference<KButton>("CycleLeft").onClick += delegate
			{
				this.settings.CycleSettingLevel(this.settings.QualitySettings[key].id, -1);
				refs.GetReference<LocText>("ValueLabel").text = this.settings.GetSettingLevelLabel(this.settings.QualitySettings[key].id, this.settings.CurrentQualityLevelsBySetting[this.settings.QualitySettings[key].id]);
				refs.GetReference<LocText>("ValueLabel").GetComponent<ToolTip>().toolTip = this.settings.GetSettingLevelTooltip(this.settings.QualitySettings[key].id, this.settings.CurrentQualityLevelsBySetting[this.settings.QualitySettings[key].id]);
			};
			refs.GetReference<KButton>("CycleRight").onClick += delegate
			{
				this.settings.CycleSettingLevel(this.settings.QualitySettings[key].id, 1);
				refs.GetReference<LocText>("ValueLabel").text = this.settings.GetSettingLevelLabel(this.settings.QualitySettings[key].id, this.settings.CurrentQualityLevelsBySetting[this.settings.QualitySettings[key].id]);
				refs.GetReference<LocText>("ValueLabel").GetComponent<ToolTip>().toolTip = this.settings.GetSettingLevelTooltip(this.settings.QualitySettings[key].id, this.settings.CurrentQualityLevelsBySetting[this.settings.QualitySettings[key].id]);
			};
		}
	}

	private void SetGameTypeToggle(bool custom_game)
	{
		this.settings.is_custom_game = custom_game;
		this.toggle_standard_game.ChangeState((!this.settings.is_custom_game) ? 1 : 0);
		this.toggle_custom_game.ChangeState((!this.settings.is_custom_game) ? 0 : 1);
		this.disable_custom_settings_shroud.SetActive(!this.settings.is_custom_game);
	}

	private void NewGame()
	{
		this.TriggerLoadingMusic();
		WorldGen.Reset();
		SaveLoader.SetActiveSaveFilePath(null);
		try
		{
			File.Delete(WorldGen.SIM_SAVE_FILENAME);
		}
		catch (Exception ex)
		{
			Output.LogWarning(new object[] { ex.ToString() });
		}
		global::Util.KInstantiateUI(ScreenPrefabs.Instance.WorldGenScreen.gameObject, this.transform.parent.gameObject, true);
		global::UnityEngine.Object.FindObjectOfType<FrontEndBackground>().gameObject.SetActive(false);
		base.Show(false);
	}

	private void TriggerLoadingMusic()
	{
		if (AudioDebug.Get().musicEnabled && !MusicManager.instance.SongIsPlaying("Music_FrontEnd"))
		{
			MusicManager.instance.StopSong("Music_TitleTheme", true, STOP_MODE.ALLOWFADEOUT);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndSnapshot, STOP_MODE.ALLOWFADEOUT);
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndWorldGenerationSnapshot);
			MusicManager.instance.PlaySong("Music_FrontEnd", false);
			MusicManager.instance.SetSongParameter("Music_FrontEnd", "songSection", 1f, true);
		}
	}

	[Header("Static UI Refs")]
	[SerializeField]
	private MultiToggle toggle_standard_game;

	[SerializeField]
	private MultiToggle toggle_custom_game;

	[SerializeField]
	private KButton button_cancel;

	[SerializeField]
	private KButton button_start;

	[SerializeField]
	private KButton button_close;

	[SerializeField]
	private GameObject disable_custom_settings_shroud;

	[SerializeField]
	private Transform content;

	[SerializeField]
	private Transform checkbox_grid;

	[SerializeField]
	[Header("Prefab UI Refs")]
	private GameObject prefab_cycle_setting;

	[SerializeField]
	private GameObject prefab_slider_setting;

	[SerializeField]
	private GameObject prefab_checkbox_setting;

	private CustomGameSettings settings;
}
