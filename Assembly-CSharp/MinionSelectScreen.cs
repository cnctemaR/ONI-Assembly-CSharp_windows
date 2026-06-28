using System;
using FMOD.Studio;
using Klei;
using STRINGS;
using UnityEngine;

public class MinionSelectScreen : CharacterSelectionController
{
	protected override void OnPrefabInit()
	{
		base.IsStarterMinion = true;
		base.OnPrefabInit();
		if (MusicManager.instance.SongIsPlaying("Music_FrontEnd"))
		{
			MusicManager.instance.SetSongParameter("Music_FrontEnd", "songSection", 2f, true);
		}
		GameObject gameObject = GameObject.Find("ScreenSpaceOverlayCanvas");
		GameObject gameObject2 = global::Util.KInstantiateUI(this.wattsonMessagePrefab.gameObject, gameObject, false);
		gameObject2.name = "WattsonMessage";
		gameObject2.SetActive(false);
		Game.Instance.Subscribe(-1992507039, new Action<object>(this.OnBaseAlreadyCreated));
		this.backButton.onClick += delegate
		{
			LoadScreen.ForceStopGame();
			WorldGen.Reset();
			App.LoadScene("frontend");
		};
		this.InitializeContainers();
	}

	protected override void OnSpawn()
	{
		this.OnCharacterAdded();
		base.EnableProceedButton();
		this.proceedButton.GetComponentInChildren<LocText>().text = UI.IMMIGRANTSCREEN.EMBARK;
		this.containers.ForEach(delegate(CharacterContainer container)
		{
			container.DisableSelectButton();
		});
	}

	protected override void OnProceed()
	{
		global::Util.KInstantiateUI(this.newBasePrefab.gameObject, GameScreenManager.Instance.ssOverlayCanvas, false);
		MusicManager.instance.StopSong("Music_FrontEnd", true, STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().NewBaseSetupSnapshot);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndWorldGenerationSnapshot, STOP_MODE.ALLOWFADEOUT);
		this.startingStats.Clear();
		foreach (CharacterContainer characterContainer in this.containers)
		{
			this.startingStats.Add(characterContainer.Stats);
		}
		NewBaseScreen.Instance.SetStartingMinionStats(this.startingStats.ToArray());
		if (this.OnProceedEvent != null)
		{
			this.OnProceedEvent();
		}
		Game.Instance.Trigger(-838649377, null);
		BuildWatermark.Instance.gameObject.SetActive(false);
		this.Deactivate();
	}

	private void OnBaseAlreadyCreated(object data)
	{
		Game.Instance.StopFE();
		Game.Instance.StartBE();
		Game.Instance.SetGameStarted();
		this.Deactivate();
	}

	private void ReshuffleAll()
	{
		if (this.OnReshuffleEvent != null)
		{
			this.OnReshuffleEvent(base.IsStarterMinion);
		}
	}

	public const string WattsonGameObjName = "WattsonMessage";

	[SerializeField]
	private NewBaseScreen newBasePrefab;

	[SerializeField]
	private WattsonMessage wattsonMessagePrefab;

	public KButton backButton;
}
