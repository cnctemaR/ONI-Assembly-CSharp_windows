using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Database;
using KSerialization;
using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ColonyAchievementTracker")]
public class ColonyAchievementTracker : KMonoBehaviour, ISaveLoadableDetails, IRenderEveryTick
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
		base.Subscribe<ColonyAchievementTracker>(631075836, ColonyAchievementTracker.OnNewDayDelegate);
	}

	public void RenderEveryTick(float dt)
	{
		if (this.updatingAchievement >= this.achievements.Count)
		{
			this.updatingAchievement = 0;
		}
		KeyValuePair<string, ColonyAchievementStatus> keyValuePair = this.achievements.ElementAt<KeyValuePair<string, ColonyAchievementStatus>>(this.updatingAchievement);
		this.updatingAchievement++;
		if (!keyValuePair.Value.success && !keyValuePair.Value.failed)
		{
			keyValuePair.Value.UpdateAchievement();
			if (keyValuePair.Value.success && !keyValuePair.Value.failed)
			{
				ColonyAchievementTracker.UnlockPlatformAchievement(keyValuePair.Key);
				this.completedAchievementsToDisplay.Add(keyValuePair.Key);
				this.TriggerNewAchievementCompleted(keyValuePair.Key, null);
				RetireColonyUtility.SaveColonySummaryData();
			}
		}
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
					ColonyAchievementTracker.UnlockPlatformAchievement(keyValuePair.Key);
					this.completedAchievementsToDisplay.Add(keyValuePair.Key);
					this.TriggerNewAchievementCompleted(keyValuePair.Key, null);
				}
			}
		}
		RetireColonyUtility.SaveColonySummaryData();
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
				return;
			}
			global::Debug.LogWarningFormat("Steam achievement [{0}] was achieved, but achievement service was null", new object[] { colonyAchievement.steamAchievementId });
		}
	}

	public void DebugTriggerAchievement(string id)
	{
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
		component.OnClose = (global::System.Action)Delegate.Combine(component.OnClose, new global::System.Action(delegate
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

	private void TriggerNewAchievementCompleted(string achievement, GameObject cameraTarget = null)
	{
		this.unlockedAchievementMetric[ColonyAchievementTracker.UnlockedAchievementKey] = achievement;
		ThreadedHttps<KleiMetrics>.Instance.SendEvent(this.unlockedAchievementMetric, "TriggerNewAchievementCompleted");
		bool flag = false;
		if (Db.Get().ColonyAchievements.Get(achievement).isVictoryCondition)
		{
			flag = true;
			this.BeginVictorySequence(achievement);
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

	public void LogFetchChore(GameObject fetcher, ChoreType choreType)
	{
		if (choreType == Db.Get().ChoreTypes.StorageFetch || choreType == Db.Get().ChoreTypes.BuildFetch || choreType == Db.Get().ChoreTypes.RepairFetch || choreType == Db.Get().ChoreTypes.FoodFetch || choreType == Db.Get().ChoreTypes.Transport)
		{
			return;
		}
		Dictionary<int, int> dictionary = null;
		if (fetcher.GetComponent<SolidTransferArm>() != null)
		{
			dictionary = this.fetchAutomatedChoreDeliveries;
		}
		else if (fetcher.GetComponent<MinionIdentity>() != null)
		{
			dictionary = this.fetchDupeChoreDeliveries;
		}
		if (dictionary != null)
		{
			int cycle = GameClock.Instance.GetCycle();
			if (!dictionary.ContainsKey(cycle))
			{
				dictionary.Add(cycle, 0);
			}
			Dictionary<int, int> dictionary2 = dictionary;
			int num = cycle;
			int num2 = dictionary2[num];
			dictionary2[num] = num2 + 1;
		}
	}

	public void LogSuitChore(ChoreDriver driver)
	{
		if (driver == null || driver.GetComponent<MinionIdentity>() == null)
		{
			return;
		}
		bool flag = false;
		foreach (AssignableSlotInstance assignableSlotInstance in driver.GetComponent<MinionIdentity>().GetEquipment().Slots)
		{
			Equippable equippable = ((EquipmentSlotInstance)assignableSlotInstance).assignable as Equippable;
			if (equippable)
			{
				KPrefabID component = equippable.GetComponent<KPrefabID>();
				if (component.HasTag(GameTags.AtmoSuit) || component.HasTag(GameTags.JetSuit))
				{
					flag = true;
					break;
				}
			}
		}
		if (flag)
		{
			int cycle = GameClock.Instance.GetCycle();
			int instanceID = driver.GetComponent<KPrefabID>().InstanceID;
			if (!this.dupesCompleteChoresInSuits.ContainsKey(cycle))
			{
				this.dupesCompleteChoresInSuits.Add(cycle, new List<int> { instanceID });
				return;
			}
			if (!this.dupesCompleteChoresInSuits[cycle].Contains(instanceID))
			{
				this.dupesCompleteChoresInSuits[cycle].Add(instanceID);
			}
		}
	}

	public void OnNewDay(object data)
	{
		foreach (MinionStorage minionStorage in Components.MinionStorages.Items)
		{
			if (minionStorage.GetComponent<CommandModule>() != null)
			{
				List<MinionStorage.Info> storedMinionInfo = minionStorage.GetStoredMinionInfo();
				if (storedMinionInfo.Count > 0)
				{
					int cycle = GameClock.Instance.GetCycle();
					if (!this.dupesCompleteChoresInSuits.ContainsKey(cycle))
					{
						this.dupesCompleteChoresInSuits.Add(cycle, new List<int>());
					}
					for (int i = 0; i < storedMinionInfo.Count; i++)
					{
						KPrefabID kprefabID = storedMinionInfo[i].serializedMinion.Get();
						if (kprefabID != null)
						{
							this.dupesCompleteChoresInSuits[cycle].Add(kprefabID.InstanceID);
						}
					}
				}
			}
		}
	}

	public Dictionary<string, ColonyAchievementStatus> achievements = new Dictionary<string, ColonyAchievementStatus>();

	[Serialize]
	public Dictionary<int, int> fetchAutomatedChoreDeliveries = new Dictionary<int, int>();

	[Serialize]
	public Dictionary<int, int> fetchDupeChoreDeliveries = new Dictionary<int, int>();

	[Serialize]
	public Dictionary<int, List<int>> dupesCompleteChoresInSuits = new Dictionary<int, List<int>>();

	private SchedulerHandle checkAchievementsHandle;

	private int forceCheckAchievementHandle = -1;

	[Serialize]
	private int updatingAchievement;

	[Serialize]
	private List<string> completedAchievementsToDisplay = new List<string>();

	private SchedulerHandle victorySchedulerHandle;

	public static readonly string UnlockedAchievementKey = "UnlockedAchievement";

	private Dictionary<string, object> unlockedAchievementMetric = new Dictionary<string, object> { 
	{
		ColonyAchievementTracker.UnlockedAchievementKey,
		null
	} };

	private static readonly EventSystem.IntraObjectHandler<ColonyAchievementTracker> OnNewDayDelegate = new EventSystem.IntraObjectHandler<ColonyAchievementTracker>(delegate(ColonyAchievementTracker component, object data)
	{
		component.OnNewDay(data);
	});
}
