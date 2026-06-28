using System;
using System.Collections.Generic;
using System.Text;
using FMODUnity;
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
		HierarchyReferences component = this.alwaysPlayMusicButton.GetComponent<HierarchyReferences>();
		GameObject gameObject = component.GetReference("Button").gameObject;
		gameObject.GetComponent<ToolTip>().SetSimpleTooltip(UI.FRONTEND.AUDIO_OPTIONS_SCREEN.MUSIC_EVERY_CYCLE_TOOLTIP);
		component.GetReference("CheckMark").gameObject.SetActive(MusicManager.instance.alwaysPlayMusic);
		gameObject.GetComponent<KButton>().onClick += delegate
		{
			this.ToggleAlwaysPlayMusic();
		};
		LocText reference = component.GetReference<LocText>("Label");
		reference.SetText(UI.FRONTEND.AUDIO_OPTIONS_SCREEN.MUSIC_EVERY_CYCLE);
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
		this.alwaysPlayMusicButton.GetComponent<HierarchyReferences>().GetReference("CheckMark").gameObject.SetActive(MusicManager.instance.alwaysPlayMusic);
		KPlayerPrefs.SetInt(AudioOptionsScreen.AlwaysPlayMusicKey, (!MusicManager.instance.alwaysPlayMusic) ? 0 : 1);
	}

	private void BuildAudioDeviceList()
	{
		this.audioDevices.Clear();
		this.audioDeviceOptions.Clear();
		int num;
		RuntimeManager.LowlevelSystem.getNumDrivers(out num);
		for (int i = 0; i < num; i++)
		{
			KFMOD.AudioDevice audioDevice = default(KFMOD.AudioDevice);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Capacity = 64;
			RuntimeManager.LowlevelSystem.getDriverInfo(i, stringBuilder, stringBuilder.Capacity, out audioDevice.guid, out audioDevice.systemRate, out audioDevice.speakerMode, out audioDevice.speakerModeChannels);
			audioDevice.name = stringBuilder.ToString();
			audioDevice.fmod_id = i;
			this.audioDevices.Add(audioDevice);
			this.audioDeviceOptions.Add(new Dropdown.OptionData(audioDevice.name));
		}
	}

	private void OnAudioDeviceChanged(int idx)
	{
		RuntimeManager.LowlevelSystem.setDriver(idx);
		for (int i = 0; i < this.audioDevices.Count; i++)
		{
			if (idx == this.audioDevices[i].fmod_id)
			{
				KFMOD.currentDevice = this.audioDevices[i];
				KPlayerPrefs.SetString("AudioDeviceGuid", KFMOD.currentDevice.guid.ToString());
				break;
			}
		}
	}

	private void OnClose(GameObject go)
	{
		this.alwaysPlayMusicMetric[AudioOptionsScreen.AlwaysPlayMusicKey] = MusicManager.instance.alwaysPlayMusic;
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

	[SerializeField]
	private Dropdown deviceDropdown;

	private UIPool<SliderContainer> sliderPool;

	private Dictionary<KSlider, string> sliderBusMap = new Dictionary<KSlider, string>();

	public static readonly string AlwaysPlayMusicKey = "AlwaysPlayMusic";

	private Dictionary<string, object> alwaysPlayMusicMetric = new Dictionary<string, object> { 
	{
		AudioOptionsScreen.AlwaysPlayMusicKey,
		null
	} };

	private List<KFMOD.AudioDevice> audioDevices = new List<KFMOD.AudioDevice>();

	private List<Dropdown.OptionData> audioDeviceOptions = new List<Dropdown.OptionData>();
}
