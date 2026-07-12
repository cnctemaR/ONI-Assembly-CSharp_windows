using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class FossilHuntInitializer : StoryTraitStateMachine<FossilHuntInitializer, FossilHuntInitializer.Instance, FossilHuntInitializer.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.Inactive;
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		this.Inactive.ParamTransition<bool>(this.storyCompleted, this.Active.StoryComplete, GameStateMachine<FossilHuntInitializer, FossilHuntInitializer.Instance, StateMachineController, FossilHuntInitializer.Def>.IsTrue).ParamTransition<bool>(this.wasStoryStarted, this.Active.inProgress, GameStateMachine<FossilHuntInitializer, FossilHuntInitializer.Instance, StateMachineController, FossilHuntInitializer.Def>.IsTrue);
		this.Active.inProgress.ParamTransition<bool>(this.storyCompleted, this.Active.StoryComplete, GameStateMachine<FossilHuntInitializer, FossilHuntInitializer.Instance, StateMachineController, FossilHuntInitializer.Def>.IsTrue).OnSignal(this.CompleteStory, this.Active.StoryComplete);
		this.Active.StoryComplete.Enter(new StateMachine<FossilHuntInitializer, FossilHuntInitializer.Instance, StateMachineController, FossilHuntInitializer.Def>.State.Callback(FossilHuntInitializer.CompleteStoryTrait));
	}

	public static bool OnUI_Quest_ObjectiveRowClicked(string rowLinkID)
	{
		rowLinkID = rowLinkID.ToUpper();
		if (!rowLinkID.Contains("MOVECAMERATO"))
		{
			return true;
		}
		string text = rowLinkID.Replace("MOVECAMERATO", "");
		if (Components.MajorFossilDigSites.Count > 0 && CodexCache.FormatLinkID(Components.MajorFossilDigSites[0].gameObject.PrefabID().ToString()) == text)
		{
			FossilHuntInitializer.FocusCamera(Components.MajorFossilDigSites[0].transform, true);
			return false;
		}
		foreach (object obj in Components.MinorFossilDigSites)
		{
			MinorFossilDigSite.Instance instance = (MinorFossilDigSite.Instance)obj;
			if (CodexCache.FormatLinkID(instance.PrefabID().ToString()) == text)
			{
				CameraController.Instance.CameraGoTo(instance.transform.GetPosition(), 2f, true);
				SelectTool.Instance.Select(instance.gameObject.GetComponent<KSelectable>(), false);
				return false;
			}
		}
		return false;
	}

	public static void CompleteStoryTrait(FossilHuntInitializer.Instance smi)
	{
		StoryInstance storyInstance = StoryManager.Instance.GetStoryInstance(Db.Get().Stories.FossilHunt.HashId);
		if (storyInstance == null)
		{
			return;
		}
		smi.sm.storyCompleted.Set(true, smi, false);
		if (storyInstance.HasDisplayedPopup(EventInfoDataHelper.PopupType.COMPLETE))
		{
			return;
		}
		smi.CompleteEvent();
	}

	public static string ResolveStrings_QuestObjectivesRowTooltips(string originalText, object obj)
	{
		return originalText + CODEX.STORY_TRAITS.FOSSILHUNT.QUEST.LINKED_TOOLTIP;
	}

	public static string ResolveQuestTitle(string title, QuestInstance quest)
	{
		int discoveredDigsitesRequired = FossilDigSiteConfig.DiscoveredDigsitesRequired;
		string text = Mathf.RoundToInt(quest.CurrentProgress * (float)discoveredDigsitesRequired).ToString() + "/" + discoveredDigsitesRequired.ToString();
		return title + " - " + text;
	}

	public static ICheckboxListGroupControl.ListGroup[] GetFossilHuntQuestData()
	{
		QuestInstance quest = QuestManager.GetInstance(FossilDigSiteConfig.hashID, Db.Get().Quests.FossilHuntQuest);
		ICheckboxListGroupControl.CheckboxItem[] checkBoxData = quest.GetCheckBoxData(null);
		for (int i = 0; i < checkBoxData.Length; i++)
		{
			checkBoxData[i].overrideLinkActions = new Func<string, bool>(FossilHuntInitializer.OnUI_Quest_ObjectiveRowClicked);
			checkBoxData[i].resolveTooltipCallback = new Func<string, object, string>(FossilHuntInitializer.ResolveStrings_QuestObjectivesRowTooltips);
		}
		if (quest != null)
		{
			return new ICheckboxListGroupControl.ListGroup[]
			{
				new ICheckboxListGroupControl.ListGroup(Db.Get().Quests.FossilHuntQuest.Title, checkBoxData, (string title) => FossilHuntInitializer.ResolveQuestTitle(title, quest), null)
			};
		}
		return new ICheckboxListGroupControl.ListGroup[0];
	}

	public static void FocusCamera(Transform target, bool select = true)
	{
		CameraController.Instance.CameraGoTo(target.GetPosition(), 2f, true);
		if (select)
		{
			KSelectable component = target.GetComponent<KSelectable>();
			SelectTool.Instance.Select(component, false);
		}
	}

	private GameStateMachine<FossilHuntInitializer, FossilHuntInitializer.Instance, StateMachineController, FossilHuntInitializer.Def>.State Inactive;

	private FossilHuntInitializer.ActiveState Active;

	public StateMachine<FossilHuntInitializer, FossilHuntInitializer.Instance, StateMachineController, FossilHuntInitializer.Def>.BoolParameter storyCompleted;

	public StateMachine<FossilHuntInitializer, FossilHuntInitializer.Instance, StateMachineController, FossilHuntInitializer.Def>.BoolParameter wasStoryStarted;

	public StateMachine<FossilHuntInitializer, FossilHuntInitializer.Instance, StateMachineController, FossilHuntInitializer.Def>.Signal CompleteStory;

	public const string LINK_OVERRIDE_PREFIX = "MOVECAMERATO";

	public class Def : StoryTraitStateMachine<FossilHuntInitializer, FossilHuntInitializer.Instance, FossilHuntInitializer.Def>.TraitDef
	{
		public override void Configure(GameObject prefab)
		{
			this.Story = Db.Get().Stories.FossilHunt;
			this.CompletionData = new StoryCompleteData
			{
				KeepSakeSpawnOffset = new CellOffset(1, 2),
				CameraTargetOffset = new CellOffset(0, 3)
			};
			this.InitalLoreId = "story_trait_fossilhunt_initial";
			this.EventIntroInfo = new StoryManager.PopupInfo
			{
				Title = CODEX.STORY_TRAITS.FOSSILHUNT.BEGIN_POPUP.NAME,
				Description = CODEX.STORY_TRAITS.FOSSILHUNT.BEGIN_POPUP.DESCRIPTION,
				CloseButtonText = CODEX.STORY_TRAITS.FOSSILHUNT.BEGIN_POPUP.BUTTON,
				TextureName = "fossildigdiscovered_kanim",
				DisplayImmediate = true,
				PopupType = EventInfoDataHelper.PopupType.BEGIN
			};
			this.CompleteLoreId = "story_trait_fossilhunt_complete";
			this.EventCompleteInfo = new StoryManager.PopupInfo
			{
				Title = CODEX.STORY_TRAITS.FOSSILHUNT.END_POPUP.NAME,
				Description = CODEX.STORY_TRAITS.FOSSILHUNT.END_POPUP.DESCRIPTION,
				CloseButtonText = CODEX.STORY_TRAITS.FOSSILHUNT.END_POPUP.BUTTON,
				TextureName = "fossildigmining_kanim",
				PopupType = EventInfoDataHelper.PopupType.COMPLETE
			};
		}

		public const string LORE_UNLOCK_PREFIX = "story_trait_fossilhunt_";

		public bool IsMainDigsite;
	}

	public class ActiveState : GameStateMachine<FossilHuntInitializer, FossilHuntInitializer.Instance, StateMachineController, FossilHuntInitializer.Def>.State
	{
		public GameStateMachine<FossilHuntInitializer, FossilHuntInitializer.Instance, StateMachineController, FossilHuntInitializer.Def>.State inProgress;

		public GameStateMachine<FossilHuntInitializer, FossilHuntInitializer.Instance, StateMachineController, FossilHuntInitializer.Def>.State StoryComplete;
	}

	public new class Instance : StoryTraitStateMachine<FossilHuntInitializer, FossilHuntInitializer.Instance, FossilHuntInitializer.Def>.TraitInstance
	{
		public Instance(StateMachineController master, FossilHuntInitializer.Def def)
			: base(master, def)
		{
		}

		public string Title
		{
			get
			{
				return CODEX.STORY_TRAITS.FOSSILHUNT.NAME;
			}
		}

		public string Description
		{
			get
			{
				return CODEX.STORY_TRAITS.FOSSILHUNT.DESCRIPTION;
			}
		}

		public override void StartSM()
		{
			base.StartSM();
			base.gameObject.GetSMI<MajorFossilDigSite>();
			StoryInstance storyInstance = StoryManager.Instance.GetStoryInstance(Db.Get().Stories.FossilHunt.HashId);
			if (storyInstance == null)
			{
				return;
			}
			if (base.sm.wasStoryStarted.Get(this) || storyInstance.CurrentState >= StoryInstance.State.IN_PROGRESS)
			{
				StoryInstance.State currentState = storyInstance.CurrentState;
				if (currentState != StoryInstance.State.IN_PROGRESS)
				{
					if (currentState == StoryInstance.State.COMPLETE)
					{
						this.GoTo(base.sm.Active.StoryComplete);
					}
				}
				else
				{
					base.sm.wasStoryStarted.Set(true, this, false);
				}
			}
			StoryInstance storyInstance2 = storyInstance;
			storyInstance2.StoryStateChanged = (Action<StoryInstance.State>)Delegate.Combine(storyInstance2.StoryStateChanged, new Action<StoryInstance.State>(this.OnStoryStateChanged));
		}

		protected override void OnCleanUp()
		{
			StoryInstance storyInstance = StoryManager.Instance.GetStoryInstance(Db.Get().Stories.FossilHunt.HashId);
			if (storyInstance != null)
			{
				StoryInstance storyInstance2 = storyInstance;
				storyInstance2.StoryStateChanged = (Action<StoryInstance.State>)Delegate.Remove(storyInstance2.StoryStateChanged, new Action<StoryInstance.State>(this.OnStoryStateChanged));
			}
			base.OnCleanUp();
		}

		private void OnStoryStateChanged(StoryInstance.State state)
		{
			if (state == StoryInstance.State.IN_PROGRESS)
			{
				base.sm.wasStoryStarted.Set(true, this, false);
			}
		}

		protected override void OnObjectSelect(object clicked)
		{
			if (!StoryManager.Instance.HasDisplayedPopup(base.def.Story, EventInfoDataHelper.PopupType.BEGIN))
			{
				this.RevealMajorFossilDigSites();
				this.RevealMinorFossilDigSites();
			}
			if (!(bool)clicked)
			{
				return;
			}
			StoryInstance storyInstance = StoryManager.Instance.GetStoryInstance(base.def.Story.HashId);
			if (storyInstance != null && storyInstance.PendingType != EventInfoDataHelper.PopupType.NONE && (storyInstance.PendingType != EventInfoDataHelper.PopupType.COMPLETE || base.def.IsMainDigsite))
			{
				base.OnNotificationClicked(null);
				return;
			}
			if (!StoryManager.Instance.HasDisplayedPopup(base.def.Story, EventInfoDataHelper.PopupType.BEGIN))
			{
				base.DisplayPopup(base.def.EventIntroInfo);
			}
		}

		public override void OnPopupClosed()
		{
			if (!StoryManager.Instance.HasDisplayedPopup(base.def.Story, EventInfoDataHelper.PopupType.COMPLETE))
			{
				base.TriggerStoryEvent(StoryInstance.State.IN_PROGRESS);
			}
			base.OnPopupClosed();
		}

		protected override void OnBuildingActivated(object activated)
		{
			StoryInstance storyInstance = StoryManager.Instance.GetStoryInstance(Db.Get().Stories.MegaBrainTank.HashId);
			if (storyInstance == null || base.sm.wasStoryStarted.Get(this) || storyInstance.CurrentState >= StoryInstance.State.IN_PROGRESS)
			{
				return;
			}
			this.RevealMinorFossilDigSites();
			this.RevealMajorFossilDigSites();
			base.OnBuildingActivated(activated);
		}

		public void RevealMajorFossilDigSites()
		{
			this.RevealAll(8, new Tag[] { "FossilDig" });
		}

		public void RevealMinorFossilDigSites()
		{
			this.RevealAll(3, new Tag[] { "FossilResin", "FossilIce", "FossilRock" });
		}

		private void RevealAll(int radius, params Tag[] tags)
		{
			foreach (WorldGenSpawner.Spawnable spawnable in SaveGame.Instance.worldGenSpawner.GetSpawnablesWithTag(false, tags))
			{
				int num;
				int num2;
				Grid.CellToXY(spawnable.cell, out num, out num2);
				GridVisibility.Reveal(num, num2, radius, (float)radius);
			}
		}

		public override void OnCompleteStorySequence()
		{
			if (base.def.IsMainDigsite)
			{
				base.OnCompleteStorySequence();
			}
		}

		public void ShowLoreUnlockedPopup(int popupID)
		{
			InfoDialogScreen infoDialogScreen = LoreBearer.ShowPopupDialog().SetHeader(CODEX.STORY_TRAITS.FOSSILHUNT.UNLOCK_DNADATA_POPUP.NAME).AddDefaultOK(false);
			bool flag = CodexCache.GetEntryForLock(FossilDigSiteConfig.FOSSIL_HUNT_LORE_UNLOCK_ID.For(popupID)) != null;
			Option<string> option = FossilDigSiteConfig.GetBodyContentForFossil(popupID);
			if (flag && option.HasValue)
			{
				infoDialogScreen.AddPlainText(option.Value).AddOption(CODEX.STORY_TRAITS.FOSSILHUNT.UNLOCK_DNADATA_POPUP.VIEW_IN_CODEX, LoreBearerUtil.OpenCodexByEntryID("STORYTRAITFOSSILHUNT"), false);
				return;
			}
			infoDialogScreen.AddPlainText(GravitasCreatureManipulatorConfig.GetBodyContentForUnknownSpecies());
		}

		public void ShowObjectiveCompletedNotification()
		{
			FossilHuntInitializer.Instance.<>c__DisplayClass16_0 CS$<>8__locals1 = new FossilHuntInitializer.Instance.<>c__DisplayClass16_0();
			CS$<>8__locals1.<>4__this = this;
			QuestInstance instance = QuestManager.GetInstance(FossilDigSiteConfig.hashID, Db.Get().Quests.FossilHuntQuest);
			if (instance == null)
			{
				return;
			}
			CS$<>8__locals1.objectivesCompleted = Mathf.RoundToInt(instance.CurrentProgress * (float)instance.CriteriaCount);
			if (CS$<>8__locals1.objectivesCompleted == 0)
			{
				this.ShowFirstFossilExcavatedNotification();
				return;
			}
			string text = FossilDigSiteConfig.FOSSIL_HUNT_LORE_UNLOCK_ID.For(CS$<>8__locals1.objectivesCompleted);
			Game.Instance.unlocks.Unlock(text, false);
			CS$<>8__locals1.<ShowObjectiveCompletedNotification>g__ShowNotificationAndWaitForClick|1().Then(delegate
			{
				CS$<>8__locals1.<>4__this.ShowLoreUnlockedPopup(CS$<>8__locals1.objectivesCompleted);
			});
		}

		public void ShowFirstFossilExcavatedNotification()
		{
			this.<ShowFirstFossilExcavatedNotification>g__ShowNotificationAndWaitForClick|17_1().Then(delegate
			{
				this.ShowQuestUnlockedPopup();
			});
		}

		public void ShowQuestUnlockedPopup()
		{
			LoreBearer.ShowPopupDialog().SetHeader(CODEX.STORY_TRAITS.FOSSILHUNT.QUEST_AVAILABLE_POPUP.NAME).AddDefaultOK(false)
				.AddPlainText(CODEX.STORY_TRAITS.FOSSILHUNT.QUEST_AVAILABLE_POPUP.DESCRIPTION.text.Value)
				.AddOption(CODEX.STORY_TRAITS.FOSSILHUNT.QUEST_AVAILABLE_POPUP.CHECK_BUTTON, delegate(InfoDialogScreen dialog)
				{
					dialog.Deactivate();
					FossilHuntInitializer.FocusCamera(base.transform, true);
				}, false);
		}

		[CompilerGenerated]
		private Promise <ShowFirstFossilExcavatedNotification>g__ShowNotificationAndWaitForClick|17_1()
		{
			return new Promise(delegate(global::System.Action resolve)
			{
				Notification notification = new Notification(CODEX.STORY_TRAITS.FOSSILHUNT.QUEST_AVAILABLE_NOTIFICATION.NAME, NotificationType.Event, (List<Notification> notifications, object obj) => CODEX.STORY_TRAITS.FOSSILHUNT.QUEST_AVAILABLE_NOTIFICATION.TOOLTIP, null, false, 0f, delegate(object obj)
				{
					resolve();
				}, null, null, true, true, false);
				base.gameObject.AddOrGet<Notifier>().Add(notification, "");
			});
		}
	}
}
