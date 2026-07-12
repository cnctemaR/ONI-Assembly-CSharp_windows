using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using FMOD.Studio;
using FMODUnity;
using ProcGen;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MusicManager")]
public class MusicManager : KMonoBehaviour, ISerializationCallbackReceiver
{
	public Dictionary<string, MusicManager.SongInfo> SongMap
	{
		get
		{
			return this.songMap;
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
			DebugUtil.LogErrorArgs(new object[] { "Unknown song:", song_name });
			return;
		}
		if (this.activeSongs.ContainsKey(song_name))
		{
			DebugUtil.LogWarningArgs(new object[] { "Trying to play duplicate song:", song_name });
			return;
		}
		if (this.activeSongs.Count == 0)
		{
			songInfo.ev = KFMOD.CreateInstance(songInfo.fmodEvent);
			if (!songInfo.ev.isValid())
			{
				object[] array = new object[1];
				int num = 0;
				string text = "Failed to find FMOD event [";
				EventReference eventReference = songInfo.fmodEvent;
				array[num] = text + eventReference.ToString() + "]";
				DebugUtil.LogWarningArgs(array);
			}
			int num2 = ((songInfo.numberOfVariations > 0) ? global::UnityEngine.Random.Range(1, songInfo.numberOfVariations + 1) : (-1));
			if (num2 != -1)
			{
				songInfo.ev.setParameterByName("variation", (float)num2, false);
			}
			if (songInfo.dynamic)
			{
				songInfo.ev.setProperty(EVENT_PROPERTY.SCHEDULE_DELAY, 16000f);
				songInfo.ev.setProperty(EVENT_PROPERTY.SCHEDULE_LOOKAHEAD, 48000f);
				this.activeDynamicSong = songInfo;
			}
			songInfo.ev.start();
			this.activeSongs[song_name] = songInfo;
			return;
		}
		List<string> list = new List<string>(this.activeSongs.Keys);
		if (songInfo.interruptsActiveMusic)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (!this.activeSongs[list[i]].interruptsActiveMusic)
				{
					MusicManager.SongInfo songInfo2 = this.activeSongs[list[i]];
					songInfo2.ev.setParameterByName("interrupted_dimmed", 1f, false);
					this.Log("Dimming: " + Assets.GetSimpleSoundEventName(songInfo2.fmodEvent));
					songInfo.songsOnHold.Add(list[i]);
				}
			}
			songInfo.ev = KFMOD.CreateInstance(songInfo.fmodEvent);
			if (!songInfo.ev.isValid())
			{
				object[] array2 = new object[1];
				int num3 = 0;
				string text2 = "Failed to find FMOD event [";
				EventReference eventReference = songInfo.fmodEvent;
				array2[num3] = text2 + eventReference.ToString() + "]";
				DebugUtil.LogWarningArgs(array2);
			}
			songInfo.ev.start();
			songInfo.ev.release();
			this.activeSongs[song_name] = songInfo;
			return;
		}
		int num4 = 0;
		foreach (string text3 in this.activeSongs.Keys)
		{
			MusicManager.SongInfo songInfo3 = this.activeSongs[text3];
			if (!songInfo3.interruptsActiveMusic && songInfo3.priority > num4)
			{
				num4 = songInfo3.priority;
			}
		}
		if (songInfo.priority >= num4)
		{
			for (int j = 0; j < list.Count; j++)
			{
				MusicManager.SongInfo songInfo4 = this.activeSongs[list[j]];
				FMOD.Studio.EventInstance ev = songInfo4.ev;
				if (!songInfo4.interruptsActiveMusic)
				{
					ev.setParameterByName("interrupted_dimmed", 1f, false);
					ev.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
					this.activeSongs.Remove(list[j]);
					list.Remove(list[j]);
				}
			}
			songInfo.ev = KFMOD.CreateInstance(songInfo.fmodEvent);
			if (!songInfo.ev.isValid())
			{
				object[] array3 = new object[1];
				int num5 = 0;
				string text4 = "Failed to find FMOD event [";
				EventReference eventReference = songInfo.fmodEvent;
				array3[num5] = text4 + eventReference.ToString() + "]";
				DebugUtil.LogWarningArgs(array3);
			}
			int num6 = ((songInfo.numberOfVariations > 0) ? global::UnityEngine.Random.Range(1, songInfo.numberOfVariations + 1) : (-1));
			if (num6 != -1)
			{
				songInfo.ev.setParameterByName("variation", (float)num6, false);
			}
			songInfo.ev.start();
			this.activeSongs[song_name] = songInfo;
		}
	}

	public void StopSong(string song_name, bool shouldLog = true, FMOD.Studio.STOP_MODE stopMode = FMOD.Studio.STOP_MODE.ALLOWFADEOUT)
	{
		if (shouldLog)
		{
			this.Log("Stop: " + song_name);
		}
		MusicManager.SongInfo songInfo = null;
		if (!this.songMap.TryGetValue(song_name, out songInfo))
		{
			DebugUtil.LogErrorArgs(new object[] { "Unknown song:", song_name });
			return;
		}
		if (!this.activeSongs.ContainsKey(song_name))
		{
			DebugUtil.LogWarningArgs(new object[] { "Trying to stop a song that isn't playing:", song_name });
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
				if (this.activeSongs.TryGetValue(songInfo.songsOnHold[i], out songInfo2) && songInfo2.ev.isValid())
				{
					FMOD.Studio.EventInstance ev2 = songInfo2.ev;
					this.Log("Undimming: " + Assets.GetSimpleSoundEventName(songInfo2.fmodEvent));
					ev2.setParameterByName("interrupted_dimmed", 0f, false);
					songInfo.songsOnHold.Remove(songInfo.songsOnHold[i]);
				}
				else
				{
					songInfo.songsOnHold.Remove(songInfo.songsOnHold[i]);
				}
			}
		}
		this.activeSongs.Remove(song_name);
	}

	public void KillAllSongs(FMOD.Studio.STOP_MODE stop_mode = FMOD.Studio.STOP_MODE.IMMEDIATE)
	{
		this.Log("Kill All Songs");
		if (this.DynamicMusicIsActive())
		{
			this.StopDynamicMusic(true);
		}
		List<string> list = new List<string>(this.activeSongs.Keys);
		for (int i = 0; i < list.Count; i++)
		{
			this.StopSong(list[i], true, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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
		if (ev.isValid())
		{
			ev.setParameterByName(parameter_name, parameter_value, false);
		}
	}

	public void SetSongParameter(string song_name, string parameter_name, string parameter_lable, bool shouldLog = true)
	{
		if (shouldLog)
		{
			this.Log(string.Format("Set Param {0}: {1}, {2}", song_name, parameter_name, parameter_lable));
		}
		MusicManager.SongInfo songInfo = null;
		if (!this.activeSongs.TryGetValue(song_name, out songInfo))
		{
			return;
		}
		FMOD.Studio.EventInstance ev = songInfo.ev;
		if (ev.isValid())
		{
			ev.setParameterByNameWithLabel(parameter_name, parameter_lable, false);
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
		if (this.DynamicMusicIsActive())
		{
			this.SetDynamicMusicZoomLevel();
			this.SetDynamicMusicTimeSinceLastJob();
			if (this.activeDynamicSong.useTimeOfDay)
			{
				this.SetDynamicMusicTimeOfDay();
			}
			if (GameClock.Instance != null && GameClock.Instance.GetCurrentCycleAsPercentage() >= this.duskTimePercentage / 100f)
			{
				this.StopDynamicMusic(false);
			}
		}
	}

	private void ClearFinishedSongs()
	{
		if (this.activeSongs.Count > 0)
		{
			ListPool<string, MusicManager>.PooledList pooledList = ListPool<string, MusicManager>.Allocate();
			foreach (KeyValuePair<string, MusicManager.SongInfo> keyValuePair in this.activeSongs)
			{
				MusicManager.SongInfo value = keyValuePair.Value;
				FMOD.Studio.EventInstance ev = value.ev;
				ev.getPlaybackState(out value.musicPlaybackState);
				if (value.musicPlaybackState == PLAYBACK_STATE.STOPPED || value.musicPlaybackState == PLAYBACK_STATE.STOPPING)
				{
					pooledList.Add(keyValuePair.Key);
					foreach (string text in value.songsOnHold)
					{
						this.SetSongParameter(text, "interrupted_dimmed", 0f, true);
					}
					value.songsOnHold.Clear();
				}
			}
			foreach (string text2 in pooledList)
			{
				this.activeSongs.Remove(text2);
			}
			pooledList.Recycle();
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

	public void OnSupplyClosetMenu(bool paused, float fadeTime)
	{
		bool flag = !paused;
		if (!PauseScreen.Instance.IsNullOrDestroyed() && PauseScreen.Instance.IsActive() && flag && MusicManager.instance.SongIsPlaying("Music_ESC_Menu"))
		{
			MusicManager.SongInfo songInfo = this.songMap["Music_ESC_Menu"];
			foreach (KeyValuePair<string, MusicManager.SongInfo> keyValuePair in this.activeSongs)
			{
				if (keyValuePair.Value != null && keyValuePair.Value != songInfo)
				{
					this.StartFadeToPause(keyValuePair.Value.ev, paused, 0.25f);
				}
			}
			this.StartFadeToPause(songInfo.ev, false, 0.25f);
			return;
		}
		foreach (KeyValuePair<string, MusicManager.SongInfo> keyValuePair2 in this.activeSongs)
		{
			if (keyValuePair2.Value != null)
			{
				this.StartFadeToPause(keyValuePair2.Value.ev, paused, fadeTime);
			}
		}
	}

	public void StartFadeToPause(FMOD.Studio.EventInstance inst, bool paused, float fadeTime = 0.25f)
	{
		if (paused)
		{
			base.StartCoroutine(this.FadeToPause(inst, fadeTime));
			return;
		}
		base.StartCoroutine(this.FadeToUnpause(inst, fadeTime));
	}

	private IEnumerator FadeToPause(FMOD.Studio.EventInstance inst, float fadeTime)
	{
		float startVolume;
		float targetVolume;
		inst.getVolume(out startVolume, out targetVolume);
		targetVolume = 0f;
		float lerpTime = 0f;
		while (lerpTime < 1f)
		{
			lerpTime += Time.unscaledDeltaTime / fadeTime;
			float num = Mathf.Lerp(startVolume, targetVolume, lerpTime);
			inst.setVolume(num);
			yield return null;
		}
		inst.setPaused(true);
		yield break;
	}

	private IEnumerator FadeToUnpause(FMOD.Studio.EventInstance inst, float fadeTime)
	{
		float startVolume;
		float targetVolume;
		inst.getVolume(out startVolume, out targetVolume);
		targetVolume = 1f;
		float lerpTime = 0f;
		inst.setPaused(false);
		while (lerpTime < 1f)
		{
			lerpTime += Time.unscaledDeltaTime / fadeTime;
			float num = Mathf.Lerp(startVolume, targetVolume, lerpTime);
			inst.setVolume(num);
			yield return null;
		}
		yield break;
	}

	public void WattsonStartDynamicMusic()
	{
		ClusterLayout currentClusterLayout = CustomGameSettings.Instance.GetCurrentClusterLayout();
		if (currentClusterLayout != null && currentClusterLayout.clusterAudio != null && !string.IsNullOrWhiteSpace(currentClusterLayout.clusterAudio.musicFirst))
		{
			DebugUtil.Assert(this.fullSongPlaylist.songMap.ContainsKey(currentClusterLayout.clusterAudio.musicFirst), "Attempting to play dlc music that isn't in the fullSongPlaylist");
			this.activePlaylist = this.fullSongPlaylist;
			this.PlayDynamicMusic(currentClusterLayout.clusterAudio.musicFirst);
			return;
		}
		this.PlayDynamicMusic();
	}

	public void PlayDynamicMusic()
	{
		if (this.DynamicMusicIsActive())
		{
			this.Log("Trying to play DynamicMusic when it is already playing.");
			return;
		}
		string nextDynamicSong = this.GetNextDynamicSong();
		this.PlayDynamicMusic(nextDynamicSong);
	}

	private void PlayDynamicMusic(string song_name)
	{
		if (song_name == "NONE")
		{
			return;
		}
		this.PlaySong(song_name, false);
		MusicManager.SongInfo songInfo;
		if (this.activeSongs.TryGetValue(song_name, out songInfo))
		{
			this.activeDynamicSong = songInfo;
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().DynamicMusicPlayingSnapshot);
			if (SpeedControlScreen.Instance != null && SpeedControlScreen.Instance.IsPaused)
			{
				this.SetDynamicMusicPaused();
			}
			if (OverlayScreen.Instance != null && OverlayScreen.Instance.mode != OverlayModes.None.ID)
			{
				this.SetDynamicMusicOverlayActive();
			}
			this.SetDynamicMusicPlayHook();
			this.SetDynamicMusicKeySigniture();
			string text = "Volume_Music";
			if (KPlayerPrefs.HasKey(text))
			{
				float @float = KPlayerPrefs.GetFloat(text);
				AudioMixer.instance.SetSnapshotParameter(AudioMixerSnapshots.Get().DynamicMusicPlayingSnapshot, "userVolume_Music", @float, true);
			}
			AudioMixer.instance.SetSnapshotParameter(AudioMixerSnapshots.Get().DynamicMusicPlayingSnapshot, "intensity", songInfo.sfxAttenuationPercentage / 100f, true);
			return;
		}
		this.Log("DynamicMusic song " + song_name + " did not start.");
		string text2 = "";
		foreach (KeyValuePair<string, MusicManager.SongInfo> keyValuePair in this.activeSongs)
		{
			text2 = text2 + keyValuePair.Key + ", ";
			global::Debug.Log(text2);
		}
		DebugUtil.DevAssert(false, "Song failed to play: " + song_name, null);
	}

	public void StopDynamicMusic(bool stopImmediate = false)
	{
		if (this.activeDynamicSong != null)
		{
			FMOD.Studio.STOP_MODE stop_MODE = (stopImmediate ? FMOD.Studio.STOP_MODE.IMMEDIATE : FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			this.Log("Stop DynamicMusic: " + Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent));
			this.StopSong(Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent), true, stop_MODE);
			this.activeDynamicSong = null;
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().DynamicMusicPlayingSnapshot, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
	}

	public string GetNextDynamicSong()
	{
		string text = "";
		if (this.alwaysPlayMusic && this.nextMusicType == MusicManager.TypeOfMusic.None)
		{
			while (this.nextMusicType == MusicManager.TypeOfMusic.None)
			{
				this.CycleToNextMusicType();
			}
		}
		switch (this.nextMusicType)
		{
		case MusicManager.TypeOfMusic.DynamicSong:
			text = this.fullSongPlaylist.GetNextSong();
			this.activePlaylist = this.fullSongPlaylist;
			break;
		case MusicManager.TypeOfMusic.MiniSong:
			text = this.miniSongPlaylist.GetNextSong();
			this.activePlaylist = this.miniSongPlaylist;
			break;
		case MusicManager.TypeOfMusic.None:
			text = "NONE";
			this.activePlaylist = null;
			break;
		}
		this.CycleToNextMusicType();
		return text;
	}

	private void CycleToNextMusicType()
	{
		int num = this.musicTypeIterator + 1;
		this.musicTypeIterator = num;
		this.musicTypeIterator = num % this.musicStyleOrder.Length;
		this.nextMusicType = this.musicStyleOrder[this.musicTypeIterator];
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
		if (CameraController.Instance != null)
		{
			float num = 100f - Camera.main.orthographicSize / 20f * 100f;
			this.SetSongParameter(Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent), "zoomPercentage", num, false);
		}
	}

	public void SetDynamicMusicTimeSinceLastJob()
	{
		this.SetSongParameter(Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent), "secsSinceNewJob", Time.time - Game.Instance.LastTimeWorkStarted, false);
	}

	public void SetDynamicMusicTimeOfDay()
	{
		if (this.time >= this.timeOfDayUpdateRate)
		{
			this.SetSongParameter(Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent), "timeOfDay", GameClock.Instance.GetCurrentCycleAsPercentage(), false);
			this.time = 0f;
		}
		this.time += Time.deltaTime;
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

	public void SetDynamicMusicPlayHook()
	{
		if (this.DynamicMusicIsActive())
		{
			string simpleSoundEventName = Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent);
			this.SetSongParameter(simpleSoundEventName, "playHook", this.activeDynamicSong.playHook ? 1f : 0f, true);
			this.activePlaylist.songMap[simpleSoundEventName].playHook = !this.activePlaylist.songMap[simpleSoundEventName].playHook;
		}
	}

	public bool ShouldPlayDynamicMusicLoadedGame()
	{
		return GameClock.Instance.GetCurrentCycleAsPercentage() <= this.loadGameCutoffPercentage / 100f;
	}

	public void SetDynamicMusicKeySigniture()
	{
		if (this.DynamicMusicIsActive())
		{
			string simpleSoundEventName = Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent);
			string musicKeySigniture = this.activePlaylist.songMap[simpleSoundEventName].musicKeySigniture;
			float num;
			if (musicKeySigniture != null)
			{
				if (musicKeySigniture == "Ab")
				{
					num = 0f;
					goto IL_0092;
				}
				if (musicKeySigniture == "Bb")
				{
					num = 1f;
					goto IL_0092;
				}
				if (musicKeySigniture == "C")
				{
					num = 2f;
					goto IL_0092;
				}
				if (musicKeySigniture == "D")
				{
					num = 3f;
					goto IL_0092;
				}
			}
			num = 2f;
			IL_0092:
			RuntimeManager.StudioSystem.setParameterByName("MusicInKey", num, false);
		}
	}

	public static MusicManager instance
	{
		get
		{
			return MusicManager._instance;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		if (!RuntimeManager.IsInitialized)
		{
			base.enabled = false;
			return;
		}
		if (KPlayerPrefs.HasKey(AudioOptionsScreen.AlwaysPlayMusicKey))
		{
			this.alwaysPlayMusic = KPlayerPrefs.GetInt(AudioOptionsScreen.AlwaysPlayMusicKey) == 1;
		}
	}

	protected override void OnPrefabInit()
	{
		MusicManager._instance = this;
		this.ConfigureSongs();
		this.nextMusicType = this.musicStyleOrder[this.musicTypeIterator];
	}

	protected override void OnCleanUp()
	{
		MusicManager._instance = null;
	}

	private static bool IsValidForDLCContext(string dlcid)
	{
		if (SaveLoader.Instance != null)
		{
			return SaveLoader.Instance.IsDLCActiveForCurrentSave(dlcid);
		}
		return DlcManager.IsContentSubscribed(dlcid);
	}

	[ContextMenu("Reload")]
	public void ConfigureSongs()
	{
		this.songMap.Clear();
		this.fullSongPlaylist.Clear();
		this.miniSongPlaylist.Clear();
		foreach (MusicManager.DynamicSong dynamicSong in this.fullSongs)
		{
			if (MusicManager.IsValidForDLCContext(dynamicSong.requiredDlcId))
			{
				string simpleSoundEventName = Assets.GetSimpleSoundEventName(dynamicSong.fmodEvent);
				MusicManager.SongInfo songInfo = new MusicManager.SongInfo();
				songInfo.fmodEvent = dynamicSong.fmodEvent;
				songInfo.requiredDlcId = dynamicSong.requiredDlcId;
				songInfo.priority = 100;
				songInfo.interruptsActiveMusic = false;
				songInfo.dynamic = true;
				songInfo.useTimeOfDay = dynamicSong.useTimeOfDay;
				songInfo.numberOfVariations = dynamicSong.numberOfVariations;
				songInfo.musicKeySigniture = dynamicSong.musicKeySigniture;
				songInfo.sfxAttenuationPercentage = this.dynamicMusicSFXAttenuationPercentage;
				this.songMap[simpleSoundEventName] = songInfo;
				this.fullSongPlaylist.songMap[simpleSoundEventName] = songInfo;
			}
		}
		foreach (MusicManager.Minisong minisong in this.miniSongs)
		{
			if (MusicManager.IsValidForDLCContext(minisong.requiredDlcId))
			{
				string simpleSoundEventName2 = Assets.GetSimpleSoundEventName(minisong.fmodEvent);
				MusicManager.SongInfo songInfo2 = new MusicManager.SongInfo();
				songInfo2.fmodEvent = minisong.fmodEvent;
				songInfo2.requiredDlcId = minisong.requiredDlcId;
				songInfo2.priority = 100;
				songInfo2.interruptsActiveMusic = false;
				songInfo2.dynamic = true;
				songInfo2.useTimeOfDay = false;
				songInfo2.numberOfVariations = 5;
				songInfo2.musicKeySigniture = minisong.musicKeySigniture;
				songInfo2.sfxAttenuationPercentage = this.miniSongSFXAttenuationPercentage;
				this.songMap[simpleSoundEventName2] = songInfo2;
				this.miniSongPlaylist.songMap[simpleSoundEventName2] = songInfo2;
			}
		}
		foreach (MusicManager.Stinger stinger in this.stingers)
		{
			if (MusicManager.IsValidForDLCContext(stinger.requiredDlcId))
			{
				string simpleSoundEventName3 = Assets.GetSimpleSoundEventName(stinger.fmodEvent);
				MusicManager.SongInfo songInfo3 = new MusicManager.SongInfo();
				songInfo3.fmodEvent = stinger.fmodEvent;
				songInfo3.priority = 100;
				songInfo3.interruptsActiveMusic = true;
				songInfo3.dynamic = false;
				songInfo3.useTimeOfDay = false;
				songInfo3.numberOfVariations = 0;
				songInfo3.requiredDlcId = stinger.requiredDlcId;
				this.songMap[simpleSoundEventName3] = songInfo3;
			}
		}
		foreach (MusicManager.MenuSong menuSong in this.menuSongs)
		{
			if (MusicManager.IsValidForDLCContext(menuSong.requiredDlcId))
			{
				string simpleSoundEventName4 = Assets.GetSimpleSoundEventName(menuSong.fmodEvent);
				MusicManager.SongInfo songInfo4 = new MusicManager.SongInfo();
				songInfo4.fmodEvent = menuSong.fmodEvent;
				songInfo4.priority = 100;
				songInfo4.interruptsActiveMusic = true;
				songInfo4.dynamic = false;
				songInfo4.useTimeOfDay = false;
				songInfo4.numberOfVariations = 0;
				songInfo4.requiredDlcId = menuSong.requiredDlcId;
				this.songMap[simpleSoundEventName4] = songInfo4;
			}
		}
		this.fullSongPlaylist.ResetUnplayedSongs();
		this.miniSongPlaylist.ResetUnplayedSongs();
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
	}

	private void Log(string s)
	{
	}

	private const string VARIATION_ID = "variation";

	private const string INTERRUPTED_DIMMED_ID = "interrupted_dimmed";

	private const string MUSIC_KEY = "MusicInKey";

	private const float DYNAMIC_MUSIC_SCHEDULE_DELAY = 16000f;

	private const float DYNAMIC_MUSIC_SCHEDULE_LOOKAHEAD = 48000f;

	[Header("Song Lists")]
	[Tooltip("Play during the daytime. The mix of the song is affected by the player's input, like pausing the sim, activating an overlay, or zooming in and out.")]
	[SerializeField]
	private MusicManager.DynamicSong[] fullSongs;

	[Tooltip("Simple dynamic songs which are more ambient in nature, which play quietly during \"non-music\" days. These are affected by Pause and OverlayActive.")]
	[SerializeField]
	private MusicManager.Minisong[] miniSongs;

	[Tooltip("Triggered by in-game events, such as completing research or night-time falling. They will temporarily interrupt a dynamicSong, fading the dynamicSong back in after the stinger is complete.")]
	[SerializeField]
	private MusicManager.Stinger[] stingers;

	[Tooltip("Generally songs that don't play during gameplay, while a menu is open. For example, the ESC menu or the Starmap.")]
	[SerializeField]
	private MusicManager.MenuSong[] menuSongs;

	private Dictionary<string, MusicManager.SongInfo> songMap = new Dictionary<string, MusicManager.SongInfo>();

	public Dictionary<string, MusicManager.SongInfo> activeSongs = new Dictionary<string, MusicManager.SongInfo>();

	[Space]
	[Header("Tuning Values")]
	[Tooltip("Just before night-time (88%), dynamic music fades out. At which point of the day should the music fade?")]
	[SerializeField]
	private float duskTimePercentage = 85f;

	[Tooltip("If we load into a save and the day is almost over, we shouldn't play music because it will stop soon anyway. At what point of the day should we not play music?")]
	[SerializeField]
	private float loadGameCutoffPercentage = 50f;

	[Tooltip("When dynamic music is active, we play a snapshot which attenuates the ambience and SFX. What intensity should that snapshot be applied?")]
	[SerializeField]
	private float dynamicMusicSFXAttenuationPercentage = 65f;

	[Tooltip("When mini songs are active, we play a snapshot which attenuates the ambience and SFX. What intensity should that snapshot be applied?")]
	[SerializeField]
	private float miniSongSFXAttenuationPercentage;

	[SerializeField]
	private MusicManager.TypeOfMusic[] musicStyleOrder;

	[NonSerialized]
	public bool alwaysPlayMusic;

	private MusicManager.DynamicSongPlaylist fullSongPlaylist = new MusicManager.DynamicSongPlaylist();

	private MusicManager.DynamicSongPlaylist miniSongPlaylist = new MusicManager.DynamicSongPlaylist();

	[NonSerialized]
	public MusicManager.SongInfo activeDynamicSong;

	[NonSerialized]
	public MusicManager.DynamicSongPlaylist activePlaylist;

	private MusicManager.TypeOfMusic nextMusicType;

	private int musicTypeIterator;

	private float time;

	private float timeOfDayUpdateRate = 2f;

	private static MusicManager _instance;

	[NonSerialized]
	public List<string> MusicDebugLog = new List<string>();

	[DebuggerDisplay("{fmodEvent}")]
	[Serializable]
	public class SongInfo
	{
		public EventReference fmodEvent;

		[NonSerialized]
		public int priority;

		[NonSerialized]
		public bool interruptsActiveMusic;

		[NonSerialized]
		public bool dynamic;

		[NonSerialized]
		public string requiredDlcId = "";

		[NonSerialized]
		public bool useTimeOfDay;

		[NonSerialized]
		public int numberOfVariations;

		[NonSerialized]
		public string musicKeySigniture = "C";

		[NonSerialized]
		public FMOD.Studio.EventInstance ev;

		[NonSerialized]
		public List<string> songsOnHold = new List<string>();

		[NonSerialized]
		public PLAYBACK_STATE musicPlaybackState;

		[NonSerialized]
		public bool playHook = true;

		[NonSerialized]
		public float sfxAttenuationPercentage = 65f;
	}

	[DebuggerDisplay("{fmodEvent}")]
	[Serializable]
	public class DynamicSong
	{
		public EventReference fmodEvent;

		[Tooltip("Some songs are set up to have Morning, Daytime, Hook, and Intro sections. Toggle this ON if this song has those sections.")]
		[SerializeField]
		public bool useTimeOfDay;

		[Tooltip("Some songs have different possible start locations. Enter how many start locations this song is set up to support.")]
		[SerializeField]
		public int numberOfVariations;

		[Tooltip("Some songs have different key signitures. Enter the key this music is in.")]
		[SerializeField]
		public string musicKeySigniture = "";

		[Tooltip("Should playback of this song be limited to an active DLC?")]
		[SerializeField]
		public string requiredDlcId = "";
	}

	[DebuggerDisplay("{fmodEvent}")]
	[Serializable]
	public class Stinger
	{
		public EventReference fmodEvent;

		[Tooltip("Should playback of this song be limited to an active DLC?")]
		[SerializeField]
		public string requiredDlcId = "";
	}

	[DebuggerDisplay("{fmodEvent}")]
	[Serializable]
	public class MenuSong
	{
		public EventReference fmodEvent;

		[Tooltip("Should playback of this song be limited to an active DLC?")]
		[SerializeField]
		public string requiredDlcId = "";
	}

	[DebuggerDisplay("{fmodEvent}")]
	[Serializable]
	public class Minisong
	{
		public EventReference fmodEvent;

		[Tooltip("Some songs have different key signitures. Enter the key this music is in.")]
		[SerializeField]
		public string musicKeySigniture = "";

		[Tooltip("Should playback of this song be limited to an active DLC?")]
		[SerializeField]
		public string requiredDlcId = "";
	}

	public enum TypeOfMusic
	{
		DynamicSong,
		MiniSong,
		None
	}

	public class DynamicSongPlaylist
	{
		public void Clear()
		{
			this.songMap.Clear();
			this.unplayedSongs.Clear();
			this.lastSongPlayed = "";
		}

		public string GetNextSong()
		{
			string text;
			if (this.unplayedSongs.Count > 0)
			{
				int num = global::UnityEngine.Random.Range(0, this.unplayedSongs.Count);
				text = this.unplayedSongs[num];
				this.unplayedSongs.RemoveAt(num);
			}
			else
			{
				this.ResetUnplayedSongs();
				bool flag = this.unplayedSongs.Count > 1;
				if (flag)
				{
					for (int i = 0; i < this.unplayedSongs.Count; i++)
					{
						if (this.unplayedSongs[i] == this.lastSongPlayed)
						{
							this.unplayedSongs.Remove(this.unplayedSongs[i]);
							break;
						}
					}
				}
				int num2 = global::UnityEngine.Random.Range(0, this.unplayedSongs.Count);
				text = this.unplayedSongs[num2];
				this.unplayedSongs.RemoveAt(num2);
				if (flag)
				{
					this.unplayedSongs.Add(this.lastSongPlayed);
				}
			}
			this.lastSongPlayed = text;
			global::Debug.Assert(this.songMap.ContainsKey(text), "Missing song " + text);
			return Assets.GetSimpleSoundEventName(this.songMap[text].fmodEvent);
		}

		public void ResetUnplayedSongs()
		{
			this.unplayedSongs.Clear();
			foreach (KeyValuePair<string, MusicManager.SongInfo> keyValuePair in this.songMap)
			{
				if (MusicManager.IsValidForDLCContext(keyValuePair.Value.requiredDlcId))
				{
					this.unplayedSongs.Add(keyValuePair.Key);
				}
			}
		}

		public Dictionary<string, MusicManager.SongInfo> songMap = new Dictionary<string, MusicManager.SongInfo>();

		public List<string> unplayedSongs = new List<string>();

		private string lastSongPlayed = "";
	}
}
