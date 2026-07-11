using System;
using System.Collections.Generic;
using System.IO;
using Database;
using KSerialization;
using STRINGS;
using UnityEngine;

public class ColonyAchievementTracker : KMonoBehaviour, ISaveLoadableDetails
{
	public List<string> achievementsToDisplay
	{
		get
		{
			return this.completedAchievementsToDisplay;
		}
	}

	public void ClearDisplayAchievements()
	{
		this.achievementsToDisplay.Clear();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		foreach (ColonyAchievement colonyAchievement in Db.Get().ColonyAchievements.resources)
		{
			if (!this.achievements.ContainsKey(colonyAchievement.Id))
			{
				ColonyAchievementStatus colonyAchievementStatus = new ColonyAchievementStatus();
				colonyAchievementStatus.SetRequirements(colonyAchievement.requirementChecklist);
				this.achievements.Add(colonyAchievement.Id, colonyAchievementStatus);
			}
		}
		this.forceCheckAchievementHandle = Game.Instance.Subscribe(395452326, new Action<object>(this.CheckAchievements));
		GameScheduler.Instance.Schedule("CheckColonyAchievements", 5f, new Action<object>(this.CheckAchievements), null, null);
	}

	private void CheckAchievements(object data = null)
	{
		foreach (KeyValuePair<string, ColonyAchievementStatus> keyValuePair in this.achievements)
		{
			if (!keyValuePair.Value.success && !keyValuePair.Value.failed)
			{
				keyValuePair.Value.UpdateAchievement();
				if (keyValuePair.Value.success && !keyValuePair.Value.failed)
				{
					this.newlyCompletedAchievements.Add(keyValuePair.Key);
				}
			}
		}
		if (this.newlyCompletedAchievements.Count > 0)
		{
			foreach (string text in this.newlyCompletedAchievements)
			{
				ColonyAchievementTracker.UnlockPlatformAchievement(text);
				this.completedAchievementsToDisplay.Add(text);
			}
			this.TriggerNewAchievementCompleted(null);
			RetireColonyUtility.SaveColonySummaryData();
		}
		this.newlyCompletedAchievements.Clear();
		this.checkAchievementsHandle = GameScheduler.Instance.Schedule("CheckColonyAchievements", 12f, new Action<object>(this.CheckAchievements), null, null);
	}

	private static void UnlockPlatformAchievement(string achievement_id)
	{
		if (DebugHandler.InstantBuildMode)
		{
			global::Debug.LogWarningFormat("UnlockPlatformAchievement {0} skipping: instant build mode", new object[] { achievement_id });
			return;
		}
		if (SaveGame.Instance.sandboxEnabled)
		{
			global::Debug.LogWarningFormat("UnlockPlatformAchievement {0} skipping: sandbox mode", new object[] { achievement_id });
			return;
		}
		if (Game.Instance.debugWasUsed)
		{
			global::Debug.LogWarningFormat("UnlockPlatformAchievement {0} skipping: debug was used.", new object[] { achievement_id });
			return;
		}
		ColonyAchievement colonyAchievement = Db.Get().ColonyAchievements.Get(achievement_id);
		if (colonyAchievement != null && !string.IsNullOrEmpty(colonyAchievement.steamAchievementId))
		{
			if (SteamAchievementService.Instance)
			{
				SteamAchievementService.Instance.Unlock(colonyAchievement.steamAchievementId);
			}
			else
			{
				global::Debug.LogWarningFormat("Steam achievement [{0}] was achieved, but achievement service was null", new object[] { colonyAchievement.steamAchievementId });
			}
		}
	}

	public void DebugTriggerAchievement(string id)
	{
		this.newlyCompletedAchievements.Add(id);
		this.achievements[id].failed = false;
		this.achievements[id].success = true;
	}

	private void BeginVictorySequence(string achievementID)
	{
		RootMenu.Instance.canTogglePauseScreen = false;
		CameraController.Instance.DisableUserCameraControl = true;
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			SpeedControlScreen.Instance.Pause(false);
		}
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().VictoryMessageSnapshot);
		AudioMixer.instance.Start(AudioMixerSnapshots.Get().MuteDynamicMusicSnapshot);
		this.ToggleVictoryUI(true);
		StoryMessageScreen component = GameScreenManager.Instance.StartScreen(ScreenPrefabs.Instance.StoryMessageScreen.gameObject, null, GameScreenManager.UIRenderTarget.ScreenSpaceOverlay).GetComponent<StoryMessageScreen>();
		component.restoreInterfaceOnClose = false;
		component.title = COLONY_ACHIEVEMENTS.PRE_VICTORY_MESSAGE_HEADER;
		component.body = string.Format(COLONY_ACHIEVEMENTS.PRE_VICTORY_MESSAGE_BODY, "<b>" + Db.Get().ColonyAchievements.Get(achievementID).Name + "</b>\n" + Db.Get().ColonyAchievements.Get(achievementID).description);
		component.Show(true);
		CameraController.Instance.SetWorldInteractive(false);
		StoryMessageScreen storyMessageScreen = component;
		storyMessageScreen.OnClose = (global::System.Action)Delegate.Combine(storyMessageScreen.OnClose, new global::System.Action(delegate
		{
			SpeedControlScreen.Instance.SetSpeed(1);
			if (!SpeedControlScreen.Instance.IsPaused)
			{
				SpeedControlScreen.Instance.Pause(false);
			}
			CameraController.Instance.SetWorldInteractive(true);
			Db.Get().ColonyAchievements.Get(achievementID).victorySequence(this);
		}));
	}

	protected override void OnCleanUp()
	{
		this.victorySchedulerHandle.ClearScheduler();
		Game.Instance.Unsubscribe(this.forceCheckAchievementHandle);
		this.checkAchievementsHandle.ClearScheduler();
		base.OnCleanUp();
	}

	private void TriggerNewAchievementCompleted(GameObject cameraTarget = null)
	{
		bool flag = false;
		for (int i = 0; i < this.newlyCompletedAchievements.Count; i++)
		{
			if (Db.Get().ColonyAchievements.Get(this.newlyCompletedAchievements[i]).isVictoryCondition)
			{
				flag = true;
				this.BeginVictorySequence(this.newlyCompletedAchievements[i]);
				break;
			}
		}
		if (!flag)
		{
			AchievementEarnedMessage achievementEarnedMessage = new AchievementEarnedMessage();
			Messenger.Instance.QueueMessage(achievementEarnedMessage);
		}
	}

	private void ToggleVictoryUI(bool victoryUIActive)
	{
		List<KScreen> list = new List<KScreen>();
		list.Add(NotificationScreen.Instance);
		list.Add(OverlayMenu.Instance);
		if (PlanScreen.Instance != null)
		{
			list.Add(PlanScreen.Instance);
		}
		if (BuildMenu.Instance != null)
		{
			list.Add(BuildMenu.Instance);
		}
		list.Add(ManagementMenu.Instance);
		list.Add(ToolMenu.Instance);
		list.Add(ToolMenu.Instance.PriorityScreen);
		list.Add(ResourceCategoryScreen.Instance);
		list.Add(TopLeftControlScreen.Instance);
		list.Add(global::DateTime.Instance);
		list.Add(BuildWatermark.Instance);
		list.Add(HoverTextScreen.Instance);
		list.Add(DetailsScreen.Instance);
		list.Add(DebugPaintElementScreen.Instance);
		list.Add(DebugBaseTemplateButton.Instance);
		list.Add(StarmapScreen.Instance);
		foreach (KScreen kscreen in list)
		{
			if (kscreen != null)
			{
				kscreen.Show(!victoryUIActive);
			}
		}
	}

	public void Serialize(BinaryWriter writer)
	{
		writer.Write(this.achievements.Count);
		foreach (KeyValuePair<string, ColonyAchievementStatus> keyValuePair in this.achievements)
		{
			writer.WriteKleiString(keyValuePair.Key);
			keyValuePair.Value.Serialize(writer);
		}
	}

	public void Deserialize(IReader reader)
	{
		if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 10))
		{
			return;
		}
		int num = reader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			string text = reader.ReadKleiString();
			ColonyAchievementStatus colonyAchievementStatus = new ColonyAchievementStatus();
			colonyAchievementStatus.Deserialize(reader);
			if (Db.Get().ColonyAchievements.Exists(text))
			{
				this.achievements.Add(text, colonyAchievementStatus);
			}
		}
	}

	public Dictionary<string, ColonyAchievementStatus> achievements = new Dictionary<string, ColonyAchievementStatus>();

	private SchedulerHandle checkAchievementsHandle;

	private int forceCheckAchievementHandle = -1;

	[Serialize]
	private List<string> completedAchievementsToDisplay = new List<string>();

	private List<string> newlyCompletedAchievements = new List<string>();

	private SchedulerHandle victorySchedulerHandle;
}
