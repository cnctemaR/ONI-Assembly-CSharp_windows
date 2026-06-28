using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AudioOptionsScreen : KModalScreen
{
	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.closeButton.onClick += delegate
		{
			this.OnClose(base.gameObject);
		};
		this.doneButton.onClick += delegate
		{
			this.OnClose(base.gameObject);
		};
		this.sliderPool = new UIPool<SliderContainer>(this.sliderPrefab);
		Dictionary<string, AudioMixer.UserVolumeBus> userVolumeSettings = AudioMixer.instance.userVolumeSettings;
		foreach (KeyValuePair<string, AudioMixer.UserVolumeBus> keyValuePair in userVolumeSettings)
		{
			SliderContainer newSlider = this.sliderPool.GetFreeElement(this.sliderGroup, true);
			this.sliderBusMap.Add(newSlider.slider, keyValuePair.Key);
			newSlider.slider.value = keyValuePair.Value.busLevel;
			newSlider.nameLabel.text = keyValuePair.Value.labelString;
			newSlider.UpdateSliderLabel(keyValuePair.Value.busLevel);
			newSlider.slider.ClearReleaseHandleEvent();
			newSlider.slider.onValueChanged.AddListener(delegate(float value)
			{
				this.OnReleaseHandle(newSlider.slider);
			});
			if (keyValuePair.Key == "Master")
			{
				newSlider.transform.SetSiblingIndex(1);
				newSlider.slider.onValueChanged.AddListener(new UnityAction<float>(this.CheckMasterValue));
				this.CheckMasterValue(keyValuePair.Value.busLevel);
			}
		}
		GameObject gameObject = this.alwaysPlayMusicButton.transform.GetChild(0).gameObject;
		gameObject.GetComponent<ToolTip>().SetSimpleTooltip(UI.FRONTEND.AUDIO_OPTIONS_SCREEN.MUSIC_EVERY_CYCLE_TOOLTIP);
		gameObject.transform.GetChild(0).gameObject.SetActive(MusicManager.instance.alwaysPlayMusic);
		gameObject.GetComponent<KButton>().onClick += delegate
		{
			this.ToggleAlwaysPlayMusic();
		};
		LocText component = this.alwaysPlayMusicButton.transform.GetChild(1).GetComponent<LocText>();
		component.SetText(UI.FRONTEND.AUDIO_OPTIONS_SCREEN.MUSIC_EVERY_CYCLE);
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (e.TryConsume(global::Action.Escape) || e.TryConsume(global::Action.MouseRight))
		{
			this.Deactivate();
		}
		else
		{
			base.OnKeyDown(e);
		}
	}

	private void CheckMasterValue(float value)
	{
		this.jambell.enabled = value == 0f;
	}

	private void OnReleaseHandle(KSlider slider)
	{
		AudioMixer.instance.SetUserVolume(this.sliderBusMap[slider], slider.value);
	}

	private void ToggleAlwaysPlayMusic()
	{
		MusicManager.instance.alwaysPlayMusic = !MusicManager.instance.alwaysPlayMusic;
		this.alwaysPlayMusicButton.transform.GetChild(0).GetChild(0).gameObject.SetActive(MusicManager.instance.alwaysPlayMusic);
		PlayerPrefs.SetInt(AudioOptionsScreen.AlwaysPlayMusicKey, (!MusicManager.instance.alwaysPlayMusic) ? 0 : 1);
	}

	private void OnClose(GameObject go)
	{
		this.alwaysPlayMusicMetric["AlwaysPlayMusic"] = MusicManager.instance.alwaysPlayMusic;
		ThreadedHttps<KleiMetrics>.Instance.SendEvent(this.alwaysPlayMusicMetric);
		global::UnityEngine.Object.Destroy(go);
	}

	[SerializeField]
	private KButton closeButton;

	[SerializeField]
	private KButton doneButton;

	[SerializeField]
	private SliderContainer sliderPrefab;

	[SerializeField]
	private GameObject sliderGroup;

	[SerializeField]
	private Image jambell;

	[SerializeField]
	private GameObject alwaysPlayMusicButton;

	private UIPool<SliderContainer> sliderPool;

	private Dictionary<KSlider, string> sliderBusMap = new Dictionary<KSlider, string>();

	private Dictionary<string, object> alwaysPlayMusicMetric = new Dictionary<string, object> { { "AlwaysPlayMusic", null } };

	public static readonly string AlwaysPlayMusicKey = "AlwaysPlayMusic";
}
