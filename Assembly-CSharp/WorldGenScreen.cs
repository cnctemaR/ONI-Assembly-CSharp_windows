using System;
using System.IO;
using FMOD.Studio;
using ProcGenGame;
using UnityEngine;

public class WorldGenScreen : NewGameFlowScreen
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		WorldGenScreen.Instance = this;
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.TriggerLoadingMusic();
		global::UnityEngine.Object.FindObjectOfType<FrontEndBackground>().gameObject.SetActive(false);
		SaveLoader.SetActiveSaveFilePath(null);
		try
		{
			File.Delete(WorldGen.SIM_SAVE_FILENAME);
		}
		catch (Exception ex)
		{
			DebugUtil.LogWarningArgs(new object[] { ex.ToString() });
		}
		this.offlineWorldGen.Generate();
	}

	private void TriggerLoadingMusic()
	{
		if (AudioDebug.Get().musicEnabled && !MusicManager.instance.SongIsPlaying("Music_FrontEnd"))
		{
			MusicManager.instance.StopSong("Music_TitleTheme", true, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndSnapshot, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
			AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndWorldGenerationSnapshot);
			MusicManager.instance.PlaySong("Music_FrontEnd", false);
			MusicManager.instance.SetSongParameter("Music_FrontEnd", "songSection", 1f, true);
		}
	}

	public override void OnKeyDown(KButtonEvent e)
	{
		if (!e.Consumed)
		{
			e.TryConsume(global::Action.Escape);
		}
		base.OnKeyDown(e);
	}

	[MyCmpReq]
	private OfflineWorldGen offlineWorldGen;

	public static WorldGenScreen Instance;
}
