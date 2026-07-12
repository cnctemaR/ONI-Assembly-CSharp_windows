using System;
using System.Collections.Generic;
using Database;
using KSerialization;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class StoryManager : KMonoBehaviour
{
	public static StoryManager Instance { get; private set; }

	public static IReadOnlyList<StoryManager.StoryTelemetry> GetTelemetry()
	{
		return StoryManager.storyTelemetry;
	}

	protected override void OnPrefabInit()
	{
		StoryManager.Instance = this;
		GameClock.Instance.Subscribe(631075836, new Action<object>(this.OnNewDayStarted));
		Game instance = Game.Instance;
		instance.OnLoad = (Action<Game.GameSaveData>)Delegate.Combine(instance.OnLoad, new Action<Game.GameSaveData>(this.OnGameLoaded));
	}

	protected override void OnCleanUp()
	{
		GameClock.Instance.Unsubscribe(631075836, new Action<object>(this.OnNewDayStarted));
		Game instance = Game.Instance;
		instance.OnLoad = (Action<Game.GameSaveData>)Delegate.Remove(instance.OnLoad, new Action<Game.GameSaveData>(this.OnGameLoaded));
	}

	public void InitialSaveSetup()
	{
		this.highestStoryCoordinateWhenGenerated = Db.Get().Stories.GetHighestCoordinateOffset();
		foreach (WorldContainer worldContainer in ClusterManager.Instance.WorldContainers)
		{
			foreach (string text in worldContainer.StoryTraitIds)
			{
				Story storyFromStoryTrait = Db.Get().Stories.GetStoryFromStoryTrait(text);
				this.CreateStory(storyFromStoryTrait, worldContainer.id);
			}
		}
		this.LogInitialSaveSetup();
	}

	public StoryInstance CreateStory(string id, int worldId)
	{
		Story story = Db.Get().Stories.Get(id);
		return this.CreateStory(story, worldId);
	}

	public StoryInstance CreateStory(Story story, int worldId)
	{
		StoryInstance storyInstance = new StoryInstance(story, worldId);
		this._stories.Add(story.HashId, storyInstance);
		StoryManager.InitTelemetry(storyInstance);
		if (story.autoStart)
		{
			this.BeginStoryEvent(story);
		}
		return storyInstance;
	}

	public StoryInstance GetStoryInstance(int hash)
	{
		StoryInstance storyInstance;
		this._stories.TryGetValue(hash, out storyInstance);
		return storyInstance;
	}

	public Dictionary<int, StoryInstance> GetStoryInstances()
	{
		return this._stories;
	}

	public int GetHighestCoordinate()
	{
		return this.highestStoryCoordinateWhenGenerated;
	}

	private string GetCompleteUnlockId(string id)
	{
		return id + "_STORY_COMPLETE";
	}

	public void ForceCreateStory(Story story, int worldId)
	{
		if (this.GetStoryInstance(story.HashId) == null)
		{
			this.CreateStory(story, worldId);
		}
	}

	public void DiscoverStoryEvent(Story story)
	{
		StoryInstance storyInstance = this.GetStoryInstance(story.HashId);
		if (storyInstance == null || this.CheckState(StoryInstance.State.DISCOVERED, story))
		{
			return;
		}
		storyInstance.CurrentState = StoryInstance.State.DISCOVERED;
	}

	public void BeginStoryEvent(Story story)
	{
		StoryInstance storyInstance = this.GetStoryInstance(story.HashId);
		if (storyInstance == null || this.CheckState(StoryInstance.State.IN_PROGRESS, story))
		{
			return;
		}
		storyInstance.CurrentState = StoryInstance.State.IN_PROGRESS;
	}

	public void CompleteStoryEvent(Story story, MonoBehaviour keepsakeSpawnTarget, FocusTargetSequence.Data sequenceData)
	{
		if (this.GetStoryInstance(story.HashId) == null || this.CheckState(StoryInstance.State.COMPLETE, story))
		{
			return;
		}
		FocusTargetSequence.Start(keepsakeSpawnTarget, sequenceData);
	}

	public void CompleteStoryEvent(Story story, Vector3 keepsakeSpawnPosition)
	{
		StoryInstance storyInstance = this.GetStoryInstance(story.HashId);
		if (storyInstance == null)
		{
			return;
		}
		GameObject prefab = Assets.GetPrefab(storyInstance.GetStory().keepsakePrefabId);
		if (prefab != null)
		{
			keepsakeSpawnPosition.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
			GameObject gameObject = Util.KInstantiate(prefab, keepsakeSpawnPosition);
			gameObject.SetActive(true);
			new UpgradeFX.Instance(gameObject.GetComponent<KMonoBehaviour>(), new Vector3(0f, -0.5f, -0.1f)).StartSM();
		}
		storyInstance.CurrentState = StoryInstance.State.COMPLETE;
		Game.Instance.unlocks.Unlock(this.GetCompleteUnlockId(story.Id), true);
	}

	public bool CheckState(StoryInstance.State state, Story story)
	{
		StoryInstance storyInstance = this.GetStoryInstance(story.HashId);
		return storyInstance != null && storyInstance.CurrentState >= state;
	}

	public bool IsStoryComplete(Story story)
	{
		return this.CheckState(StoryInstance.State.COMPLETE, story);
	}

	public bool IsStoryCompleteGlobal(Story story)
	{
		return Game.Instance.unlocks.IsUnlocked(this.GetCompleteUnlockId(story.Id));
	}

	public StoryInstance DisplayPopup(Story story, StoryManager.PopupInfo info, global::System.Action popupCB = null, Notification.ClickCallback notificationCB = null)
	{
		StoryInstance storyInstance = this.GetStoryInstance(story.HashId);
		if (storyInstance == null || storyInstance.HasDisplayedPopup(info.PopupType))
		{
			return null;
		}
		EventInfoData eventInfoData = EventInfoDataHelper.GenerateStoryTraitData(info.Title, info.Description, info.CloseButtonText, info.TextureName, info.PopupType, info.CloseButtonToolTip, info.Minions, popupCB);
		Notification notification = null;
		if (!info.DisplayImmediate)
		{
			notification = EventInfoScreen.CreateNotification(eventInfoData, notificationCB);
		}
		storyInstance.SetPopupData(info, eventInfoData, notification);
		return storyInstance;
	}

	public bool HasDisplayedPopup(Story story, EventInfoDataHelper.PopupType type)
	{
		StoryInstance storyInstance = this.GetStoryInstance(story.HashId);
		return storyInstance != null && storyInstance.HasDisplayedPopup(type);
	}

	private void LogInitialSaveSetup()
	{
		int num = 0;
		StoryManager.StoryCreationTelemetry[] array = new StoryManager.StoryCreationTelemetry[CustomGameSettings.Instance.CurrentStoryLevelsBySetting.Count];
		foreach (KeyValuePair<string, string> keyValuePair in CustomGameSettings.Instance.CurrentStoryLevelsBySetting)
		{
			array[num] = new StoryManager.StoryCreationTelemetry
			{
				StoryId = keyValuePair.Key,
				Enabled = CustomGameSettings.Instance.IsStoryActive(keyValuePair.Key, keyValuePair.Value)
			};
			num++;
		}
		OniMetrics.LogEvent(OniMetrics.Event.NewSave, "StoryTraitsCreation", array);
	}

	private void OnNewDayStarted(object _)
	{
		OniMetrics.LogEvent(OniMetrics.Event.EndOfCycle, "SavedHighestStoryCoordinate", this.highestStoryCoordinateWhenGenerated);
		OniMetrics.LogEvent(OniMetrics.Event.EndOfCycle, "StoryTraits", StoryManager.storyTelemetry);
	}

	private static void InitTelemetry(StoryInstance story)
	{
		WorldContainer world = ClusterManager.Instance.GetWorld(story.worldId);
		if (world == null)
		{
			return;
		}
		story.Telemetry.StoryId = story.storyId;
		story.Telemetry.WorldId = world.worldName;
		StoryManager.storyTelemetry.Add(story.Telemetry);
	}

	private void OnGameLoaded(object _)
	{
		StoryManager.storyTelemetry.Clear();
		foreach (KeyValuePair<int, StoryInstance> keyValuePair in this._stories)
		{
			StoryManager.InitTelemetry(keyValuePair.Value);
		}
	}

	public static void DestroyInstance()
	{
		StoryManager.storyTelemetry.Clear();
		StoryManager.Instance = null;
	}

	public const int BEFORE_STORIES = -2;

	private static List<StoryManager.StoryTelemetry> storyTelemetry = new List<StoryManager.StoryTelemetry>();

	[Serialize]
	private Dictionary<int, StoryInstance> _stories = new Dictionary<int, StoryInstance>();

	[Serialize]
	private int highestStoryCoordinateWhenGenerated = -2;

	private const string STORY_TRAIT_KEY = "StoryTraits";

	private const string STORY_CREATION_KEY = "StoryTraitsCreation";

	private const string STORY_COORDINATE_KEY = "SavedHighestStoryCoordinate";

	public struct PopupInfo
	{
		public string Title;

		public string Description;

		public string CloseButtonText;

		public string CloseButtonToolTip;

		public string TextureName;

		public GameObject[] Minions;

		public bool DisplayImmediate;

		public EventInfoDataHelper.PopupType PopupType;
	}

	[SerializationConfig(MemberSerialization.OptIn)]
	public class StoryTelemetry : ISaveLoadable
	{
		public void LogStateChange(StoryInstance.State state, float time)
		{
			switch (state)
			{
			case StoryInstance.State.RETROFITTED:
				this.Retrofitted = ((this.Retrofitted >= 0f) ? this.Retrofitted : time);
				return;
			case StoryInstance.State.NOT_STARTED:
				break;
			case StoryInstance.State.DISCOVERED:
				this.Discovered = ((this.Discovered >= 0f) ? this.Discovered : time);
				return;
			case StoryInstance.State.IN_PROGRESS:
				this.InProgress = ((this.InProgress >= 0f) ? this.InProgress : time);
				return;
			case StoryInstance.State.COMPLETE:
				this.Completed = ((this.Completed >= 0f) ? this.Completed : time);
				break;
			default:
				return;
			}
		}

		public string StoryId;

		public string WorldId;

		[Serialize]
		public float Retrofitted = -1f;

		[Serialize]
		public float Discovered = -1f;

		[Serialize]
		public float InProgress = -1f;

		[Serialize]
		public float Completed = -1f;
	}

	public class StoryCreationTelemetry
	{
		public string StoryId;

		public bool Enabled;
	}
}
