using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MusicManager : KMonoBehaviour, ISerializationCallbackReceiver
{
	private void Log(string s)
	{
	}

	public Dictionary<string, MusicManager.SongInfo> SongMap
	{
		get
		{
			return this.songMap;
		}
	}

	public Dictionary<string, MusicManager.SongInfo> ActiveSongs
	{
		get
		{
			return this.activeSongs;
		}
	}

	public void PlaySong(string song_name, bool canWait = false)
	{
		this.Log("Play: " + song_name);
		if (!AudioDebug.Get().musicEnabled)
		{
			return;
		}
		MusicManager.SongInfo songInfo = null;
		if (!this.songMap.TryGetValue(song_name, out songInfo))
		{
			Output.LogError(new object[] { "Unknown song:", song_name });
			return;
		}
		if (this.activeSongs.ContainsKey(song_name))
		{
			Output.LogWarning(new object[] { "Trying to play duplicate song:", song_name });
			return;
		}
		if (this.activeSongs.Count == 0)
		{
			songInfo.ev = KFMOD.CreateInstance(songInfo.fmodEvent);
			if (songInfo.ev == null)
			{
				Output.LogWarning(new object[] { "Failed to find FMOD event [" + songInfo.fmodEvent + "]" });
			}
			songInfo.ev.start();
			this.activeSongs[song_name] = songInfo;
			if (songInfo.dynamic)
			{
				this.activeDynamicSong = songInfo;
			}
		}
		else
		{
			List<string> list = new List<string>(this.activeSongs.Keys);
			if (songInfo.stinger)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (!this.activeSongs[list[i]].stinger)
					{
						MusicManager.SongInfo songInfo2 = this.activeSongs[list[i]];
						songInfo2.ev.setParameterValue("interrupted_dimmed", 1f);
						this.Log("Dimming: " + Assets.GetSimpleSoundEventName(songInfo2.fmodEvent));
						songInfo.songsOnHold.Add(list[i]);
					}
				}
				songInfo.ev = KFMOD.CreateInstance(songInfo.fmodEvent);
				if (songInfo.ev == null)
				{
					Output.LogWarning(new object[] { "Failed to find FMOD event [" + songInfo.fmodEvent + "]" });
				}
				songInfo.ev.start();
				this.activeSongs[song_name] = songInfo;
			}
			else
			{
				int num = 0;
				foreach (string text in this.activeSongs.Keys)
				{
					MusicManager.SongInfo songInfo3 = this.activeSongs[text];
					if (!songInfo3.stinger && songInfo3.priority > num)
					{
						num = songInfo3.priority;
					}
				}
				if (songInfo.priority >= num)
				{
					for (int j = 0; j < list.Count; j++)
					{
						MusicManager.SongInfo songInfo4 = this.activeSongs[list[j]];
						FMOD.Studio.EventInstance ev = songInfo4.ev;
						if (!songInfo4.stinger)
						{
							ev.setParameterValue("interrupted_dimmed", 1f);
							ev.stop(STOP_MODE.ALLOWFADEOUT);
							this.activeSongs.Remove(list[j]);
							list.Remove(list[j]);
						}
					}
					songInfo.ev = KFMOD.CreateInstance(songInfo.fmodEvent);
					if (songInfo.ev == null)
					{
						Output.LogWarning(new object[] { "Failed to find FMOD event [" + songInfo.fmodEvent + "]" });
					}
					songInfo.ev.start();
					this.activeSongs[song_name] = songInfo;
				}
			}
		}
	}

	public void StopSong(string song_name, bool shouldLog = true, STOP_MODE stopMode = STOP_MODE.ALLOWFADEOUT)
	{
		if (shouldLog)
		{
			this.Log("Stop: " + song_name);
		}
		MusicManager.SongInfo songInfo = null;
		if (!this.songMap.TryGetValue(song_name, out songInfo))
		{
			Output.LogError(new object[] { "Unknown song:", song_name });
			return;
		}
		if (!this.activeSongs.ContainsKey(song_name))
		{
			Output.LogWarning(new object[] { "Trying to stop a song that isn't playing:", song_name });
			return;
		}
		FMOD.Studio.EventInstance ev = songInfo.ev;
		ev.stop(stopMode);
		ev.release();
		if (songInfo.dynamic)
		{
			this.activeDynamicSong = null;
		}
		if (songInfo.songsOnHold.Count > 0)
		{
			for (int i = 0; i < songInfo.songsOnHold.Count; i++)
			{
				MusicManager.SongInfo songInfo2;
				if (this.activeSongs.TryGetValue(songInfo.songsOnHold[i], out songInfo2) && songInfo2.ev != null)
				{
					FMOD.Studio.EventInstance ev2 = songInfo2.ev;
					this.Log("Undimming: " + Assets.GetSimpleSoundEventName(songInfo2.fmodEvent));
					ev2.setParameterValue("interrupted_dimmed", 0f);
					songInfo.songsOnHold.Remove(songInfo.songsOnHold[i]);
				}
				else
				{
					Output.LogWarning(new object[] { string.Concat(new string[]
					{
						"[",
						songInfo.fmodEvent,
						"] has a song on hold (",
						songInfo.songsOnHold[i],
						") which is not in activeSongs."
					}) });
				}
			}
		}
		this.activeSongs.Remove(song_name);
	}

	public void KillAllSongs(STOP_MODE stop_mode = STOP_MODE.IMMEDIATE)
	{
		this.Log("Kill All Songs");
		if (this.DynamicMusicIsActive())
		{
			this.StopDynamicMusic(true);
		}
		List<string> list = new List<string>(this.activeSongs.Keys);
		for (int i = 0; i < list.Count; i++)
		{
			MusicManager.SongInfo songInfo = this.activeSongs[list[i]];
			FMOD.Studio.EventInstance ev = songInfo.ev;
			ev.stop(stop_mode);
			ev.release();
			this.activeSongs[list[i]].songsOnHold.Clear();
			this.activeSongs.Remove(list[i]);
		}
	}

	public void SetSongParameter(string song_name, string parameter_name, float parameter_value, bool shouldLog = true)
	{
		if (shouldLog)
		{
			this.Log(string.Format("Set Param {0}: {1}, {2}", song_name, parameter_name, parameter_value));
		}
		MusicManager.SongInfo songInfo = null;
		if (!this.activeSongs.TryGetValue(song_name, out songInfo))
		{
			return;
		}
		FMOD.Studio.EventInstance ev = songInfo.ev;
		if (ev != null)
		{
			ev.setParameterValue(parameter_name, parameter_value);
		}
	}

	public bool SongIsPlaying(string song_name)
	{
		MusicManager.SongInfo songInfo = null;
		return this.activeSongs.TryGetValue(song_name, out songInfo) && songInfo.musicPlaybackState != PLAYBACK_STATE.STOPPED;
	}

	private void Update()
	{
		this.ClearFinishedSongs();
		this.SetDynamicMusicZoomLevel();
		this.SetDynamicMusicTimeSinceLastJob();
		if (GameClock.Instance != null && GameClock.Instance.GetCurrentDayAsPercentage() >= this.duskTime && this.DynamicMusicIsActive())
		{
			this.StopDynamicMusic(false);
		}
	}

	private void ClearFinishedSongs()
	{
		if (this.activeSongs.Count > 0)
		{
			List<string> list = new List<string>(this.activeSongs.Keys);
			for (int i = 0; i < list.Count; i++)
			{
				MusicManager.SongInfo songInfo = this.activeSongs[list[i]];
				FMOD.Studio.EventInstance ev = songInfo.ev;
				ev.getPlaybackState(out songInfo.musicPlaybackState);
				if (songInfo.musicPlaybackState == PLAYBACK_STATE.STOPPED || songInfo.musicPlaybackState == PLAYBACK_STATE.STOPPING)
				{
					this.activeSongs.Remove(list[i]);
					if (songInfo.songsOnHold.Count > 0)
					{
						for (int j = 0; j < songInfo.songsOnHold.Count; j++)
						{
							this.SetSongParameter(songInfo.songsOnHold[j], "interrupted_dimmed", 0f, true);
							songInfo.songsOnHold.Remove(songInfo.songsOnHold[j]);
						}
					}
				}
			}
		}
	}

	public void OnEscapeMenu(bool paused)
	{
		foreach (KeyValuePair<string, MusicManager.SongInfo> keyValuePair in this.activeSongs)
		{
			if (keyValuePair.Value != null)
			{
				this.StartFadeToPause(keyValuePair.Value.ev, paused, 0.25f);
			}
		}
	}

	public void StartFadeToPause(FMOD.Studio.EventInstance inst, bool paused, float fadeTime = 0.25f)
	{
		if (paused)
		{
			base.StartCoroutine(this.FadeToPause(inst, fadeTime));
		}
		else
		{
			base.StartCoroutine(this.FadeToUnpause(inst, fadeTime));
		}
	}

	private IEnumerator FadeToPause(FMOD.Studio.EventInstance inst, float fadeTime)
	{
		float startVolume;
		inst.getVolume(out startVolume);
		float targetVolume = 0f;
		float lerpTime = 0f;
		float lerpedVolume = 0f;
		while (lerpTime < 1f)
		{
			lerpTime += Time.unscaledDeltaTime / fadeTime;
			lerpedVolume = Mathf.Lerp(startVolume, targetVolume, lerpTime);
			inst.setVolume(lerpedVolume);
			yield return null;
		}
		inst.setPaused(true);
		yield break;
	}

	private IEnumerator FadeToUnpause(FMOD.Studio.EventInstance inst, float fadeTime)
	{
		float startVolume;
		inst.getVolume(out startVolume);
		float targetVolume = 1f;
		float lerpTime = 0f;
		float lerpedVolume = 0f;
		inst.setPaused(false);
		while (lerpTime < 1f)
		{
			lerpTime += Time.unscaledDeltaTime / fadeTime;
			lerpedVolume = Mathf.Lerp(startVolume, targetVolume, lerpTime);
			inst.setVolume(lerpedVolume);
			yield return null;
		}
		yield break;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (PlayerPrefs.HasKey(AudioOptionsScreen.AlwaysPlayMusicKey))
		{
			this.alwaysPlayMusic = PlayerPrefs.GetInt(AudioOptionsScreen.AlwaysPlayMusicKey) == 1;
		}
	}

	public void PlayDynamicMusic()
	{
		if (this.DynamicMusicIsActive())
		{
			this.Log("Trying to play DynamicMusic when it is already playing.");
			return;
		}
		this.daysSinceDynamicMusic = 0;
		string nextDynamicSong = this.GetNextDynamicSong();
		this.PlaySong(nextDynamicSong, false);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().DynamicMusicPlayingSnapshot);
		MusicManager.SongInfo songInfo;
		if (this.activeSongs.TryGetValue(nextDynamicSong, out songInfo))
		{
			this.activeDynamicSong = songInfo;
		}
		else
		{
			this.Log("DynamicMusic song " + nextDynamicSong + " did not start.");
		}
		if (SpeedControlScreen.Instance != null && SpeedControlScreen.Instance.IsPaused)
		{
			this.SetDynamicMusicPaused();
		}
		if (OverlayScreen.Instance != null && OverlayScreen.Instance.mode != SimViewMode.None)
		{
			this.SetDynamicMusicOverlayActive();
		}
		string text = "Volume_Music";
		if (PlayerPrefs.HasKey(text))
		{
			float @float = PlayerPrefs.GetFloat(text);
			AudioMixer.instance.SetSnapshotParameter(AudioMixerSnapshots.Get().DynamicMusicPlayingSnapshot, "userVolume_Music", @float, true);
		}
	}

	public void StopDynamicMusic(bool stopImmediate = false)
	{
		if (this.activeDynamicSong != null)
		{
			STOP_MODE stop_MODE = ((!stopImmediate) ? STOP_MODE.ALLOWFADEOUT : STOP_MODE.IMMEDIATE);
			this.Log("Stop DynamicMusic: " + Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent));
			this.StopSong(Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent), true, stop_MODE);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().DynamicMusicPlayingSnapshot, STOP_MODE.ALLOWFADEOUT);
		}
	}

	public string GetNextDynamicSong()
	{
		int num = 0;
		if (this.lastDynamicSongPlayed != -1)
		{
			num = ((this.lastDynamicSongPlayed + 1 < this.dynamicSongs.Count) ? (this.lastDynamicSongPlayed + 1) : 0);
		}
		this.lastDynamicSongPlayed = num;
		return this.dynamicSongs[num];
	}

	public bool DynamicMusicIsActive()
	{
		return this.activeDynamicSong != null;
	}

	public void SetDynamicMusicPaused()
	{
		if (this.DynamicMusicIsActive())
		{
			this.SetSongParameter(Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent), "Paused", 1f, true);
		}
	}

	public void SetDynamicMusicUnpaused()
	{
		if (this.DynamicMusicIsActive())
		{
			this.SetSongParameter(Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent), "Paused", 0f, true);
		}
	}

	public void SetDynamicMusicZoomLevel()
	{
		if (this.DynamicMusicIsActive() && CameraController.Instance != null)
		{
			float num = 100f - Camera.main.orthographicSize / 20f * 100f;
			this.SetSongParameter(Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent), "zoomPercentage", num, false);
		}
	}

	public void SetDynamicMusicTimeSinceLastJob()
	{
		if (this.DynamicMusicIsActive())
		{
			this.SetSongParameter(Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent), "secsSinceNewJob", Time.time - Game.Instance.LastTimeWorkStarted, false);
		}
	}

	public void SetDynamicMusicOverlayActive()
	{
		if (this.DynamicMusicIsActive())
		{
			this.SetSongParameter(Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent), "overlayActive", 1f, true);
		}
	}

	public void SetDynamicMusicOverlayInactive()
	{
		if (this.DynamicMusicIsActive())
		{
			this.SetSongParameter(Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent), "overlayActive", 0f, true);
		}
	}

	public bool ShouldPlayDynamicMusicStartOfDay()
	{
		return this.alwaysPlayMusic || this.daysSinceDynamicMusic - 1 >= this.daysBetweenDynamicMusic;
	}

	public bool ShouldPlayDynamicMusicLoadedGame()
	{
		return GameClock.Instance.GetCurrentDayAsPercentage() <= this.loadGameCutoffPoint;
	}

	public static MusicManager instance
	{
		get
		{
			return MusicManager._instance;
		}
	}

	protected override void OnPrefabInit()
	{
		MusicManager._instance = this;
		this.dynamicSongs = new List<string>(this.dynamicSongMap.Keys);
	}

	protected override void OnCleanUp()
	{
		MusicManager._instance = null;
	}

	[ContextMenu("Reload")]
	private void ReloadSongs()
	{
		this.songMap.Clear();
		foreach (MusicManager.SongInfo songInfo in this.songs)
		{
			string simpleSoundEventName = Assets.GetSimpleSoundEventName(songInfo.fmodEvent);
			this.songMap[simpleSoundEventName] = songInfo;
			if (songInfo.dynamic)
			{
				this.dynamicSongMap[simpleSoundEventName] = songInfo;
			}
		}
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		this.ReloadSongs();
	}

	[SerializeField]
	private MusicManager.SongInfo[] songs;

	private Dictionary<string, MusicManager.SongInfo> songMap = new Dictionary<string, MusicManager.SongInfo>();

	public Dictionary<string, MusicManager.SongInfo> activeSongs = new Dictionary<string, MusicManager.SongInfo>();

	public List<string> MusicDebugLog = new List<string>();

	private Dictionary<string, MusicManager.SongInfo> dynamicSongMap = new Dictionary<string, MusicManager.SongInfo>();

	private List<string> dynamicSongs;

	private int lastDynamicSongPlayed = -1;

	[NonSerialized]
	public MusicManager.SongInfo activeDynamicSong;

	public int daysSinceDynamicMusic;

	public int daysBetweenDynamicMusic;

	public bool alwaysPlayMusic;

	private float duskTime = 0.75f;

	private float loadGameCutoffPoint = 0.5f;

	private static MusicManager _instance;

	[DebuggerDisplay("{fmodEvent}")]
	[Serializable]
	public class SongInfo
	{
		[EventRef]
		public string fmodEvent;

		[SerializeField]
		public int priority;

		[SerializeField]
		public bool stinger;

		[SerializeField]
		public bool dynamic;

		[NonSerialized]
		public FMOD.Studio.EventInstance ev;

		[NonSerialized]
		public List<string> songsOnHold = new List<string>();

		[NonSerialized]
		public PLAYBACK_STATE musicPlaybackState;
	}
}
