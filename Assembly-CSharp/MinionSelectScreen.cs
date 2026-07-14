using System;
using System.Collections;
using System.Runtime.CompilerServices;
using FMOD.Studio;
using Klei.AI;
using Klei.CustomSettings;
using ProcGen;
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
			App.LoadScene("frontend");
		};
		this.InitializeContainers();
		base.StartCoroutine(this.SetDefaultMinionsRoutine());
	}

	private IEnumerator SetDefaultMinionsRoutine()
	{
		yield return SequenceUtil.WaitForNextFrame;
		SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.ClusterLayout);
		ClusterLayout clusterData = SettingsCache.clusterLayouts.GetClusterData(currentQualitySetting.id);
		MinionSelectScreen.<>c__DisplayClass5_0 CS$<>8__locals1;
		CS$<>8__locals1.aquaticStart = MinionSelectScreen.IsAquaticStartWorld(clusterData);
		if (clusterData.startingMinions != null)
		{
			DebugUtil.Assert(clusterData.startingMinions.Length <= 3, "Cannot have more than 3 Minion presets");
			MinionSelectScreen.<SetDefaultMinionsRoutine>g__SetupMinion|5_0((CharacterContainer)this.containers[2], (clusterData.startingMinions.Length != 0) ? clusterData.startingMinions[0] : null, ref CS$<>8__locals1);
			MinionSelectScreen.<SetDefaultMinionsRoutine>g__SetupMinion|5_0((CharacterContainer)this.containers[1], (clusterData.startingMinions.Length > 1) ? clusterData.startingMinions[1] : null, ref CS$<>8__locals1);
			MinionSelectScreen.<SetDefaultMinionsRoutine>g__SetupMinion|5_0((CharacterContainer)this.containers[0], (clusterData.startingMinions.Length > 2) ? clusterData.startingMinions[2] : null, ref CS$<>8__locals1);
		}
		yield break;
	}

	private static bool IsAquaticStartWorld(ClusterLayout cluster)
	{
		if (cluster == null)
		{
			return false;
		}
		string startWorld = cluster.GetStartWorld();
		if (string.IsNullOrEmpty(startWorld))
		{
			return false;
		}
		global::ProcGen.World worldData = SettingsCache.worlds.GetWorldData(startWorld);
		return worldData != null && worldData.worldTags != null && worldData.worldTags.Contains("Aquatic");
	}

	private static void EnsureSwimmingSkill(CharacterContainer container)
	{
		if (container == null)
		{
			return;
		}
		MinionStartingStats stats = container.Stats;
		if (stats == null || stats.Traits == null)
		{
			return;
		}
		if (stats.personality != null && stats.personality.model == GameTags.Minions.Models.Bionic)
		{
			return;
		}
		foreach (Trait trait in stats.Traits)
		{
			if (trait != null && trait.Id == "GrantSkill_Swimming")
			{
				return;
			}
		}
		Trait trait2 = Db.Get().traits.TryGet("GrantSkill_Swimming");
		if (trait2 == null)
		{
			return;
		}
		int num = ((stats.Traits.Count > 0) ? 1 : 0);
		stats.Traits.Insert(num, trait2);
		container.SetMinion(stats);
	}

	public void SetProceedButtonActive(bool state, string tooltip = null)
	{
		if (state)
		{
			base.EnableProceedButton();
		}
		else
		{
			base.DisableProceedButton();
		}
		ToolTip component = this.proceedButton.GetComponent<ToolTip>();
		if (component != null)
		{
			if (tooltip != null)
			{
				component.toolTip = tooltip;
				return;
			}
			component.ClearMultiStringTooltip();
		}
	}

	protected override void OnSpawn()
	{
		this.OnDeliverableAdded();
		base.EnableProceedButton();
		this.proceedButton.GetComponentInChildren<LocText>().text = UI.IMMIGRANTSCREEN.EMBARK;
		this.containers.ForEach(delegate(ITelepadDeliverableContainer container)
		{
			CharacterContainer characterContainer = container as CharacterContainer;
			if (characterContainer != null)
			{
				characterContainer.DisableSelectButton();
			}
		});
	}

	protected override void OnProceed()
	{
		global::Util.KInstantiateUI(this.newBasePrefab.gameObject, GameScreenManager.Instance.ssOverlayCanvas, false);
		MusicManager.instance.StopSong("Music_FrontEnd", true, STOP_MODE.ALLOWFADEOUT);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().NewBaseSetupSnapshot);
		AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndWorldGenerationSnapshot, STOP_MODE.ALLOWFADEOUT);
		int num = 0;
		this.selectedDeliverables.Clear();
		foreach (ITelepadDeliverableContainer telepadDeliverableContainer in this.containers)
		{
			CharacterContainer characterContainer = (CharacterContainer)telepadDeliverableContainer;
			this.selectedDeliverables.Add(characterContainer.Stats);
			if (characterContainer.Stats.personality.model == BionicMinionConfig.MODEL)
			{
				num++;
			}
		}
		NewBaseScreen.Instance.Init(SaveLoader.Instance.Cluster, this.selectedDeliverables.ToArray());
		if (this.OnProceedEvent != null)
		{
			this.OnProceedEvent();
		}
		if (Game.IsDlcActiveForCurrentSave("DLC3_ID") && Components.RoleStations.Count > 0)
		{
			BuildingFacade component = Components.RoleStations[0].GetComponent<BuildingFacade>();
			bool flag = !component.IsOriginal;
			if (num == 3 || (!flag && num > 0))
			{
				component.ApplyBuildingFacade(Db.GetBuildingFacades().Get("permit_hqbase_cyberpunk"), false);
			}
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

	public override void OnPressBack()
	{
		foreach (ITelepadDeliverableContainer telepadDeliverableContainer in this.containers)
		{
			CharacterContainer characterContainer = telepadDeliverableContainer as CharacterContainer;
			if (characterContainer != null)
			{
				characterContainer.ForceStopEditingTitle();
			}
		}
	}

	[CompilerGenerated]
	internal static void <SetDefaultMinionsRoutine>g__SetupMinion|5_0(CharacterContainer container, string specificMinion, ref MinionSelectScreen.<>c__DisplayClass5_0 A_2)
	{
		if (specificMinion != null)
		{
			container.SetMinion(new MinionStartingStats(Db.Get().Personalities.Get(specificMinion.ToUpper()), null, null, false));
		}
		else
		{
			container.GenerateCharacter(true, null);
		}
		if (A_2.aquaticStart)
		{
			MinionSelectScreen.EnsureSwimmingSkill(container);
			container.OnReshuffled -= MinionSelectScreen.EnsureSwimmingSkill;
			container.OnReshuffled += MinionSelectScreen.EnsureSwimmingSkill;
		}
	}

	[SerializeField]
	private NewBaseScreen newBasePrefab;

	[SerializeField]
	private WattsonMessage wattsonMessagePrefab;

	public const string WattsonGameObjName = "WattsonMessage";

	public KButton backButton;
}
