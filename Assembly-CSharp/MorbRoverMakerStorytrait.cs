using System;
using Klei.AI;
using STRINGS;
using UnityEngine;

public class MorbRoverMakerStorytrait : StoryTraitStateMachine<MorbRoverMakerStorytrait, MorbRoverMakerStorytrait.Instance, MorbRoverMakerStorytrait.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.root;
	}

	public StateMachine<MorbRoverMakerStorytrait, MorbRoverMakerStorytrait.Instance, StateMachineController, MorbRoverMakerStorytrait.Def>.BoolParameter HasAnyBioBotBeenReleased;

	public class Def : StoryTraitStateMachine<MorbRoverMakerStorytrait, MorbRoverMakerStorytrait.Instance, MorbRoverMakerStorytrait.Def>.TraitDef
	{
		public override void Configure(GameObject prefab)
		{
			this.Story = Db.Get().Stories.MorbRoverMaker;
			this.CompletionData = new StoryCompleteData
			{
				KeepSakeSpawnOffset = new CellOffset(0, 2),
				CameraTargetOffset = new CellOffset(0, 3)
			};
			this.InitalLoreId = "story_trait_morbrover_initial";
			this.EventIntroInfo = new StoryManager.PopupInfo
			{
				Title = CODEX.STORY_TRAITS.MORB_ROVER_MAKER.POPUPS.BEGIN.NAME,
				Description = CODEX.STORY_TRAITS.MORB_ROVER_MAKER.POPUPS.BEGIN.DESCRIPTION,
				CloseButtonText = CODEX.STORY_TRAITS.MORB_ROVER_MAKER.POPUPS.BEGIN.BUTTON,
				TextureName = "biobotdiscovered_kanim",
				DisplayImmediate = true,
				PopupType = EventInfoDataHelper.PopupType.BEGIN
			};
			this.EventMachineRevealedInfo = new StoryManager.PopupInfo
			{
				Title = CODEX.STORY_TRAITS.MORB_ROVER_MAKER.POPUPS.REVEAL.NAME,
				Description = CODEX.STORY_TRAITS.MORB_ROVER_MAKER.POPUPS.REVEAL.DESCRIPTION,
				CloseButtonText = CODEX.STORY_TRAITS.MORB_ROVER_MAKER.POPUPS.REVEAL.BUTTON_CLOSE,
				extraButtons = new StoryManager.ExtraButtonInfo[]
				{
					new StoryManager.ExtraButtonInfo
					{
						ButtonText = CODEX.STORY_TRAITS.MORB_ROVER_MAKER.POPUPS.REVEAL.BUTTON_READLORE,
						OnButtonClick = delegate
						{
							global::System.Action normalPopupOpenCodexButtonPressed = this.NormalPopupOpenCodexButtonPressed;
							if (normalPopupOpenCodexButtonPressed != null)
							{
								normalPopupOpenCodexButtonPressed();
							}
							this.UnlockRevealEntries();
							string entryForLock = CodexCache.GetEntryForLock(this.MachineRevealedLoreId);
							if (entryForLock == null)
							{
								DebugUtil.DevLogError("Missing codex entry for lock: " + this.MachineRevealedLoreId);
								return;
							}
							ManagementMenu.Instance.OpenCodexToEntry(entryForLock, null);
						}
					}
				},
				TextureName = "BioBotCleanedUp_kanim",
				PopupType = EventInfoDataHelper.PopupType.NORMAL
			};
			this.CompleteLoreId = "story_trait_morbrover_complete";
			this.EventCompleteInfo = new StoryManager.PopupInfo
			{
				Title = CODEX.STORY_TRAITS.MORB_ROVER_MAKER.POPUPS.END.NAME,
				Description = CODEX.STORY_TRAITS.MORB_ROVER_MAKER.POPUPS.END.DESCRIPTION,
				CloseButtonText = CODEX.STORY_TRAITS.MORB_ROVER_MAKER.POPUPS.END.BUTTON,
				TextureName = "BioBotComplete_kanim",
				PopupType = EventInfoDataHelper.PopupType.COMPLETE
			};
		}

		public void UnlockRevealEntries()
		{
			Game.Instance.unlocks.Unlock(this.MachineRevealedLoreId, true);
			Game.Instance.unlocks.Unlock(this.MachineRevealedLoreId2, true);
		}

		public const string LORE_UNLOCK_PREFIX = "story_trait_morbrover_";

		public string MachineRevealedLoreId = "story_trait_morbrover_reveal";

		public string MachineRevealedLoreId2 = "story_trait_morbrover_reveal_lore";

		public string CompleteLoreId2 = "story_trait_morbrover_complete_lore";

		public string CompleteLoreId3 = "story_trait_morbrover_biobot";

		public global::System.Action NormalPopupOpenCodexButtonPressed;

		public StoryManager.PopupInfo EventMachineRevealedInfo;
	}

	public new class Instance : StoryTraitStateMachine<MorbRoverMakerStorytrait, MorbRoverMakerStorytrait.Instance, MorbRoverMakerStorytrait.Def>.TraitInstance
	{
		public Instance(StateMachineController master, MorbRoverMakerStorytrait.Def def)
			: base(master, def)
		{
			def.NormalPopupOpenCodexButtonPressed = (global::System.Action)Delegate.Combine(def.NormalPopupOpenCodexButtonPressed, new global::System.Action(this.OnNormalPopupOpenCodexButtonPressed));
		}

		public override void StartSM()
		{
			base.StartSM();
			this.machine = base.gameObject.GetSMI<MorbRoverMaker.Instance>();
			this.storyInstance = StoryManager.Instance.GetStoryInstance(Db.Get().Stories.MorbRoverMaker.HashId);
			if (this.storyInstance == null)
			{
				return;
			}
			if (this.machine != null)
			{
				MorbRoverMaker.Instance instance = this.machine;
				instance.OnUncovered = (global::System.Action)Delegate.Combine(instance.OnUncovered, new global::System.Action(this.OnMachineUncovered));
				MorbRoverMaker.Instance instance2 = this.machine;
				instance2.OnRoverSpawned = (Action<GameObject>)Delegate.Combine(instance2.OnRoverSpawned, new Action<GameObject>(this.OnRoverSpawned));
				if (this.machine.HasBeenRevealed && this.storyInstance.CurrentState != StoryInstance.State.COMPLETE && this.storyInstance.CurrentState != StoryInstance.State.IN_PROGRESS)
				{
					base.DisplayPopup(base.def.EventMachineRevealedInfo);
				}
				if (this.machine.HasBeenRevealed && base.sm.HasAnyBioBotBeenReleased.Get(this) && this.storyInstance.CurrentState != StoryInstance.State.COMPLETE)
				{
					this.CompleteEvent();
				}
			}
		}

		private void OnMachineUncovered()
		{
			if (this.storyInstance != null && !this.storyInstance.HasDisplayedPopup(EventInfoDataHelper.PopupType.NORMAL))
			{
				base.DisplayPopup(base.def.EventMachineRevealedInfo);
			}
		}

		protected override void ShowEventNormalUI()
		{
			base.ShowEventNormalUI();
			if (this.storyInstance != null && this.storyInstance.PendingType == EventInfoDataHelper.PopupType.NORMAL)
			{
				EventInfoScreen.ShowPopup(this.storyInstance.EventInfo);
			}
		}

		public override void OnPopupClosed()
		{
			base.OnPopupClosed();
			if (this.storyInstance.HasDisplayedPopup(EventInfoDataHelper.PopupType.COMPLETE))
			{
				Game.Instance.unlocks.Unlock(base.def.CompleteLoreId2, true);
				Game.Instance.unlocks.Unlock(base.def.CompleteLoreId3, true);
				return;
			}
			if (this.storyInstance != null && this.storyInstance.HasDisplayedPopup(EventInfoDataHelper.PopupType.NORMAL))
			{
				base.TriggerStoryEvent(StoryInstance.State.IN_PROGRESS);
				base.def.UnlockRevealEntries();
				return;
			}
		}

		private void OnNormalPopupOpenCodexButtonPressed()
		{
			base.TriggerStoryEvent(StoryInstance.State.IN_PROGRESS);
		}

		private void OnRoverSpawned(GameObject rover)
		{
			base.smi.sm.HasAnyBioBotBeenReleased.Set(true, base.smi, false);
			if (!this.storyInstance.HasDisplayedPopup(EventInfoDataHelper.PopupType.COMPLETE))
			{
				this.CompleteEvent();
			}
		}

		protected override void OnCleanUp()
		{
			if (this.machine != null)
			{
				MorbRoverMaker.Instance instance = this.machine;
				instance.OnUncovered = (global::System.Action)Delegate.Remove(instance.OnUncovered, new global::System.Action(this.OnMachineUncovered));
				MorbRoverMaker.Instance instance2 = this.machine;
				instance2.OnRoverSpawned = (Action<GameObject>)Delegate.Remove(instance2.OnRoverSpawned, new Action<GameObject>(this.OnRoverSpawned));
			}
			base.OnCleanUp();
		}

		private MorbRoverMaker.Instance machine;

		private StoryInstance storyInstance;
	}
}
