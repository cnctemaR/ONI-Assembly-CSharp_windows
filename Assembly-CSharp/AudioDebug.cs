using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AudioDebug")]
public class AudioDebug : KMonoBehaviour
{
	public static AudioDebug Get()
	{
		return AudioDebug.instance;
	}

	protected override void OnPrefabInit()
	{
		AudioDebug.instance = this;
	}

	public void ToggleMusic()
	{
		if (Game.Instance != null)
		{
			Game.Instance.SetMusicEnabled(this.musicEnabled);
		}
		this.musicEnabled = !this.musicEnabled;
	}

	private static AudioDebug instance;

	public bool musicEnabled;

	public bool debugSoundEvents;

	public bool debugFloorSounds;

	public bool debugGameEventSounds;

	public bool debugNotificationSounds;

	public bool debugVoiceSounds;
}
