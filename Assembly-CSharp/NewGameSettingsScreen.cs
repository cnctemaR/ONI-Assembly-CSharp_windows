using System;
using System.Collections.Generic;
using System.IO;
using FMOD.Studio;
using Klei.CustomSettings;
using ProcGenGame;
using TMPro;
using UnityEngine;

public class NewGameSettingsScreen : KModalScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

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
			this.Deactivate();
		};
		this.button_close.onClick += delegate
		{
			this.Deactivate();
		};
		this.settings = global::UnityEngine.Object.FindObjectOfType<CustomGameSettings>();
		this.SetGameTypeToggle(false);
		foreach (KeyValuePair<string, SettingConfig> keyValuePair in this.settings.QualitySettings)
		{
			ListSettingConfig list_setting = keyValuePair.Value as ListSettingConfig;
			if (list_setting != null)
			{
				GameObject gameObject = global::Util.KInstantiateUI(this.prefab_cycle_setting, this.content.gameObject, true);
				HierarchyReferences refs2 = gameObject.GetComponent<HierarchyReferences>();
				refs2.GetReference<LocText>("Label").text = keyValuePair.Value.label;
				refs2.GetReference<LocText>("Label").GetComponent<ToolTip>().toolTip = keyValuePair.Value.tooltip;
				string key2 = keyValuePair.Key;
				refs2.GetReference<KButton>("CycleLeft").onClick += delegate
				{
					this.CycleSetting(list_setting, refs2, key2, -1);
				};
				refs2.GetReference<KButton>("CycleRight").onClick += delegate
				{
					this.CycleSetting(list_setting, refs2, key2, 1);
				};
				this.CycleSetting(list_setting, refs2, key2, 0);
			}
			else
			{
				ToggleSettingConfig toggle_setting = keyValuePair.Value as ToggleSettingConfig;
				if (toggle_setting != null)
				{
					GameObject gameObject2 = global::Util.KInstantiateUI(this.prefab_checkbox_setting, this.content.gameObject, true);
					HierarchyReferences refs3 = gameObject2.GetComponent<HierarchyReferences>();
					refs3.GetReference<LocText>("Label").text = keyValuePair.Value.label;
					refs3.GetReference<LocText>("Label").GetComponent<ToolTip>().toolTip = keyValuePair.Value.tooltip;
					string key3 = keyValuePair.Key;
					MultiToggle reference = refs3.GetReference<MultiToggle>("Toggle");
					reference.onClick = (global::System.Action)Delegate.Combine(reference.onClick, new global::System.Action(delegate
					{
						this.ToggleSetting(toggle_setting, refs3, key3, false);
					}));
					this.ToggleSetting(toggle_setting, refs3, key3, true);
				}
				else
				{
					SeedSettingConfig seed_setting = keyValuePair.Value as SeedSettingConfig;
					if (seed_setting != null)
					{
						GameObject gameObject3 = global::Util.KInstantiateUI(this.prefab_seed_input_setting, this.content.gameObject, true);
						HierarchyReferences refs = gameObject3.GetComponent<HierarchyReferences>();
						refs.GetReference<LocText>("Label").text = keyValuePair.Value.label;
						refs.GetReference<LocText>("Label").GetComponent<ToolTip>().toolTip = keyValuePair.Value.tooltip;
						string key = keyValuePair.Key;
						refs.GetReference<TMP_InputField>("Input").onEndEdit.AddListener(delegate(string s)
						{
							this.SetSeedSetting(seed_setting, refs, key, s);
						});
						refs.GetReference<KButton>("Randomize").onClick += delegate
						{
							this.GetNewRandomSeed(seed_setting, refs, key);
						};
						this.GetNewRandomSeed(seed_setting, refs, key);
					}
				}
			}
		}
	}

	private void CycleSetting(ListSettingConfig setting, HierarchyReferences refs, string key, int direction)
	{
		string text = setting.CycleSettingLevelID(this.settings.CurrentQualityLevelsBySetting[key], direction);
		this.settings.CurrentQualityLevelsBySetting[key] = text;
		SettingLevel level = setting.GetLevel(text);
		refs.GetReference<LocText>("ValueLabel").text = level.label;
		refs.GetReference<LocText>("ValueLabel").GetComponent<ToolTip>().toolTip = level.tooltip;
		refs.GetReference<KButton>("CycleLeft").isInteractable = !setting.IsFirstLevel(text);
		refs.GetReference<KButton>("CycleRight").isInteractable = !setting.IsLastLevel(text);
	}

	private void ToggleSetting(ToggleSettingConfig setting, HierarchyReferences refs, string key, bool just_update_widgets = false)
	{
		string text = this.settings.CurrentQualityLevelsBySetting[key];
		if (!just_update_widgets)
		{
			text = setting.ToggleSettingLevelID(text);
			this.settings.CurrentQualityLevelsBySetting[key] = text;
		}
		SettingLevel level = setting.GetLevel(text);
		refs.GetReference<MultiToggle>("Toggle").ChangeState((!setting.IsOnLevel(text)) ? 0 : 1);
		refs.GetReference<MultiToggle>("Toggle").GetComponent<ToolTip>().toolTip = level.tooltip;
	}

	private void SetSeedSetting(SeedSettingConfig setting, HierarchyReferences refs, string key, string input)
	{
		this.settings.CurrentQualityLevelsBySetting[key] = input;
		int num;
		try
		{
			num = Convert.ToInt32(input);
		}
		catch
		{
			num = 0;
		}
		OfflineWorldGen.SetSeed(num);
		Output.Log(new object[] { "Set worldgen seed to", input });
	}

	private void GetNewRandomSeed(SeedSettingConfig setting, HierarchyReferences refs, string key)
	{
		int num = global::UnityEngine.Random.Range(0, int.MaxValue);
		refs.GetReference<TMP_InputField>("Input").text = num.ToString();
		this.SetSeedSetting(setting, refs, key, num.ToString());
	}

	private void SetGameTypeToggle(bool custom_game)
	{
		this.settings.is_custom_game = custom_game;
		this.toggle_standard_game.ChangeState((!this.settings.is_custom_game) ? 1 : 0);
		this.toggle_custom_game.ChangeState((!this.settings.is_custom_game) ? 0 : 1);
		this.disable_custom_settings_shroud.SetActive(!this.settings.is_custom_game);
		KPlayerPrefs.SetInt(OfflineWorldGen.USE_WORLD_SEED_KEY, (!this.settings.is_custom_game) ? 0 : 1);
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
		this.Deactivate();
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

	[SerializeField]
	[Header("Static UI Refs")]
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
	[Header("Prefab UI Refs")]
	private GameObject prefab_cycle_setting;

	[SerializeField]
	private GameObject prefab_slider_setting;

	[SerializeField]
	private GameObject prefab_checkbox_setting;

	[SerializeField]
	private GameObject prefab_seed_input_setting;

	private CustomGameSettings settings;
}
