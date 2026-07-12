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
				ColonyAchievementStatus colonyAchievementStatus = new ColonyAchievementStatus(colonyAchievement.Id);
				this.achievements.Add(colonyAchievement.Id, colonyAchievementStatus);
			}
		}
		this.forceCheckAchievementHandle = Game.Instance.Subscribe(395452326, new Action<object>(this.CheckAchievements));
		base.Subscribe<ColonyAchievementTracker>(631075836, ColonyAchievementTracker.OnNewDayDelegate);
		this.UpgradeTamedCritterAchievements();
	}

	private void UpgradeTamedCritterAchievements()
	{
		foreach (ColonyAchievementRequirement colonyAchievementRequirement in Db.Get().ColonyAchievements.TameAllBasicCritters.requirementChecklist)
		{
			CritterTypesWithTraits critterTypesWithTraits = colonyAchievementRequirement as CritterTypesWithTraits;
			if (critterTypesWithTraits != null)
			{
				critterTypesWithTraits.UpdateSavedState();
			}
		}
		foreach (ColonyAchievementRequirement colonyAchievementRequirement2 in Db.Get().ColonyAchievements.TameAGassyMoo.requirementChecklist)
		{
			CritterTypesWithTraits critterTypesWithTraits2 = colonyAchievementRequirement2 as CritterTypesWithTraits;
			if (critterTypesWithTraits2 != null)
			{
				critterTypesWithTraits2.UpdateSavedState();
			}
		}
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
		if (colonyAchievement != null && !string.IsNullOrEmpty(colonyAchievement.platformAchievementId))
		{
			if (SteamAchievementService.Instance)
			{
				SteamAchievementService.Instance.Unlock(colonyAchievement.platformAchievementId);
				return;
			}
			global::Debug.LogWarningFormat("Steam achievement [{0}] was achieved, but achievement service was null", new object[] { colonyAchievement.platformAchievementId });
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
			SpeedControlScreen.Instance.Pause(false, false);
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
				SpeedControlScreen.Instance.Pause(false, false);
			}
			CameraController.Instance.SetWorldInteractive(true);
			Db.Get().ColonyAchievements.Get(achievementID).victorySequence(this);
		}));
	}

	public bool IsAchievementUnlocked(ColonyAchievement achievement)
	{
		foreach (KeyValuePair<string, ColonyAchievementStatus> keyValuePair in this.achievements)
		{
			if (keyValuePair.Key == achievement.Id)
			{
				if (keyValuePair.Value.success)
				{
					return true;
				}
				keyValuePair.Value.UpdateAchievement();
				return keyValuePair.Value.success;
			}
		}
		return false;
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
			ColonyAchievementStatus colonyAchievementStatus = ColonyAchievementStatus.Deserialize(reader, text);
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

	public void LogCritterTamed(Tag prefabId)
	{
		this.tamedCritterTypes.Add(prefabId);
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
			if (equippable && equippable.GetComponent<KPrefabID>().IsAnyPrefabID(ColonyAchievementTracker.SuitTags))
			{
				flag = true;
				break;
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

	public void LogAnalyzedSeed(Tag seed)
	{
		this.analyzedSeeds.Add(seed);
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
		if (DlcManager.IsExpansion1Active())
		{
			SurviveARocketWithMinimumMorale surviveARocketWithMinimumMorale = Db.Get().ColonyAchievements.SurviveInARocket.requirementChecklist[0] as SurviveARocketWithMinimumMorale;
			if (surviveARocketWithMinimumMorale != null)
			{
				float minimumMorale = surviveARocketWithMinimumMorale.minimumMorale;
				int numberOfCycles = surviveARocketWithMinimumMorale.numberOfCycles;
				foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
				{
					if (worldContainer.IsModuleInterior)
					{
						if (!this.cyclesRocketDupeMoraleAboveRequirement.ContainsKey(worldContainer.id))
						{
							this.cyclesRocketDupeMoraleAboveRequirement.Add(worldContainer.id, 0);
						}
						if (worldContainer.GetComponent<Clustercraft>().Status != Clustercraft.CraftStatus.Grounded)
						{
							List<MinionIdentity> worldItems = Components.MinionIdentities.GetWorldItems(worldContainer.id, false);
							bool flag = worldItems.Count > 0;
							foreach (MinionIdentity minionIdentity in worldItems)
							{
								if (Db.Get().Attributes.QualityOfLife.Lookup(minionIdentity).GetTotalValue() < minimumMorale)
								{
									flag = false;
									break;
								}
							}
							this.cyclesRocketDupeMoraleAboveRequirement[worldContainer.id] = (flag ? (this.cyclesRocketDupeMoraleAboveRequirement[worldContainer.id] + 1) : 0);
						}
						else if (this.cyclesRocketDupeMoraleAboveRequirement[worldContainer.id] < numberOfCycles)
						{
							this.cyclesRocketDupeMoraleAboveRequirement[worldContainer.id] = 0;
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

	[Serialize]
	public HashSet<Tag> tamedCritterTypes = new HashSet<Tag>();

	[Serialize]
	public bool defrostedDuplicant;

	[Serialize]
	public HashSet<Tag> analyzedSeeds = new HashSet<Tag>();

	[Serialize]
	public float totalMaterialsHarvestFromPOI;

	[Serialize]
	public float radBoltTravelDistance;

	[Serialize]
	public bool harvestAHiveWithoutGettingStung;

	[Serialize]
	public Dictionary<int, int> cyclesRocketDupeMoraleAboveRequirement = new Dictionary<int, int>();

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

	private static readonly Tag[] SuitTags = new Tag[]
	{
		GameTags.AtmoSuit,
		GameTags.JetSuit,
		GameTags.LeadSuit
	};

	private static readonly EventSystem.IntraObjectHandler<ColonyAchievementTracker> OnNewDayDelegate = new EventSystem.IntraObjectHandler<ColonyAchievementTracker>(delegate(ColonyAchievementTracker component, object data)
	{
		component.OnNewDay(data);
	});
}
