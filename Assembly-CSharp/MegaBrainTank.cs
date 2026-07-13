using System;
using System.Collections.Generic;
using Klei.AI;
using KSerialization;
using STRINGS;
using UnityEngine;

public class MegaBrainTank : StateMachineComponent<MegaBrainTank.StatesInstance>
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		StoryManager.Instance.ForceCreateStory(Db.Get().Stories.MegaBrainTank, base.gameObject.GetMyWorldId());
		base.smi.StartSM();
		base.Subscribe(-1503271301, new Action<object>(this.OnBuildingSelect));
		base.GetComponent<Activatable>().SetWorkTime(5f);
		base.smi.JournalDelivery.refillMass = 25f;
		base.smi.JournalDelivery.FillToCapacity = true;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		base.Unsubscribe(-1503271301);
	}

	private void OnBuildingSelect(object obj)
	{
		if (!((Boxed<bool>)obj).value)
		{
			return;
		}
		if (!this.introDisplayed)
		{
			this.introDisplayed = true;
			EventInfoScreen.ShowPopup(EventInfoDataHelper.GenerateStoryTraitData(CODEX.STORY_TRAITS.MEGA_BRAIN_TANK.BEGIN_POPUP.NAME, CODEX.STORY_TRAITS.MEGA_BRAIN_TANK.BEGIN_POPUP.DESCRIPTION, CODEX.STORY_TRAITS.CLOSE_BUTTON, "braintankdiscovered_kanim", EventInfoDataHelper.PopupType.BEGIN, null, null, new global::System.Action(this.DoInitialUnlock)));
		}
		base.smi.ShowEventCompleteUI(null);
	}

	private void DoInitialUnlock()
	{
		Game.Instance.unlocks.Unlock("story_trait_mega_brain_tank_initial", true);
	}

	[Serialize]
	private bool introDisplayed;

	public class States : GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank>
	{
		public override void InitializeStates(out StateMachine.BaseState default_state)
		{
			base.serializable = StateMachine.SerializeType.ParamsOnly;
			default_state = this.root;
			this.root.Enter(delegate(MegaBrainTank.StatesInstance smi)
			{
				if (!StoryManager.Instance.CheckState(StoryInstance.State.COMPLETE, Db.Get().Stories.MegaBrainTank))
				{
					smi.GoTo(this.common.dormant);
					return;
				}
				if (smi.IsHungry)
				{
					smi.GoTo(this.common.idle);
					return;
				}
				smi.GoTo(this.common.active);
			});
			this.common.Update(delegate(MegaBrainTank.StatesInstance smi, float dt)
			{
				smi.IncrementMeter(dt);
				if (smi.UnitsFromLastStore != 0)
				{
					smi.ShelveJournals(dt);
				}
				bool flag = smi.ElementConverter.HasEnoughMass(GameTags.Oxygen, true);
				smi.Selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.MegaBrainNotEnoughOxygen, !flag, null);
			}, UpdateRate.SIM_33ms, false);
			this.common.dormant.Enter(delegate(MegaBrainTank.StatesInstance smi)
			{
				smi.SetBonusActive(false);
				smi.ElementConverter.SetAllConsumedActive(false);
				smi.ElementConverter.SetConsumedElementActive(DreamJournalConfig.ID, false);
				smi.Selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.MegaBrainTankDreamAnalysis, false);
				smi.master.GetComponent<Light2D>().enabled = false;
			}).Exit(delegate(MegaBrainTank.StatesInstance smi)
			{
				smi.ElementConverter.SetConsumedElementActive(DreamJournalConfig.ID, true);
				smi.ElementConverter.SetConsumedElementActive(GameTags.Oxygen, true);
				RequireInputs component = smi.GetComponent<RequireInputs>();
				component.requireConduitHasMass = true;
				component.visualizeRequirements = RequireInputs.Requirements.All;
			}).Update(delegate(MegaBrainTank.StatesInstance smi, float dt)
			{
				smi.ActivateBrains(dt);
			}, UpdateRate.SIM_33ms, false)
				.OnSignal(this.storyTraitCompleted, this.common.active);
			this.common.idle.Enter(delegate(MegaBrainTank.StatesInstance smi)
			{
				smi.CleanTank(false);
			}).UpdateTransition(this.common.active, (MegaBrainTank.StatesInstance smi, float _) => !smi.IsHungry && smi.gameObject.GetComponent<Operational>().enabled, UpdateRate.SIM_1000ms, false);
			this.common.active.Enter(delegate(MegaBrainTank.StatesInstance smi)
			{
				smi.CleanTank(true);
			}).Update(delegate(MegaBrainTank.StatesInstance smi, float dt)
			{
				smi.Digest();
			}, UpdateRate.SIM_33ms, false).UpdateTransition(this.common.idle, (MegaBrainTank.StatesInstance smi, float _) => smi.IsHungry || !smi.gameObject.GetComponent<Operational>().enabled, UpdateRate.SIM_1000ms, false);
			this.StatBonus = new Effect("MegaBrainTankBonus", DUPLICANTS.MODIFIERS.MEGABRAINTANKBONUS.NAME, DUPLICANTS.MODIFIERS.MEGABRAINTANKBONUS.TOOLTIP, 0f, true, true, false, null, -1f, 0f, null, "");
			object[,] stat_BONUSES = MegaBrainTankConfig.STAT_BONUSES;
			int length = stat_BONUSES.GetLength(0);
			for (int i = 0; i < length; i++)
			{
				string text = stat_BONUSES[i, 0] as string;
				float? num = (float?)stat_BONUSES[i, 1];
				Units? units = (Units?)stat_BONUSES[i, 2];
				this.StatBonus.Add(new AttributeModifier(text, ModifierSet.ConvertValue(num.Value, units.Value), DUPLICANTS.MODIFIERS.MEGABRAINTANKBONUS.NAME, false, false, true));
			}
		}

		public MegaBrainTank.States.CommonState common;

		public StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Signal storyTraitCompleted;

		public Effect StatBonus;

		public class CommonState : GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State
		{
			public GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State dormant;

			public GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State idle;

			public GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State active;
		}
	}

	public class StatesInstance : GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.GameInstance
	{
		public KBatchedAnimController BrainController
		{
			get
			{
				return this.controllers[0];
			}
		}

		public KBatchedAnimController ShelfController
		{
			get
			{
				return this.controllers[1];
			}
		}

		public Storage BrainStorage { get; private set; }

		public KSelectable Selectable { get; private set; }

		public Operational Operational { get; private set; }

		public ElementConverter ElementConverter { get; private set; }

		public ManualDeliveryKG JournalDelivery { get; private set; }

		public LoopingSounds BrainSounds { get; private set; }

		public bool IsHungry
		{
			get
			{
				return !this.ElementConverter.HasEnoughMassToStartConverting(true);
			}
		}

		public int JournalsStored
		{
			get
			{
				return (int)this.BrainStorage.GetUnitsAvailable(DreamJournalConfig.ID);
			}
		}

		private float DesiredMeterPosition
		{
			get
			{
				return (float)this.JournalsStored / 25f;
			}
		}

		public float DigestionTimeRemaining
		{
			get
			{
				return (float)this.JournalsStored * 60f;
			}
		}

		public HashedString CurrentActivationAnim
		{
			get
			{
				return MegaBrainTankConfig.ACTIVATION_ANIMS[(int)(this.nextActiveBrain - 1)];
			}
		}

		private HashedString currentActivationLoop
		{
			get
			{
				int num = (int)(this.nextActiveBrain - 1 + 5);
				return MegaBrainTankConfig.ACTIVATION_ANIMS[num];
			}
		}

		private float MeterPosition
		{
			get
			{
				return this.meterPositionValue;
			}
			set
			{
				this.meterPositionValue = value;
				this.meter.SetPositionPercent(value);
			}
		}

		public StatesInstance(MegaBrainTank master)
			: base(master)
		{
			this.BrainSounds = base.GetComponent<LoopingSounds>();
			this.BrainStorage = base.GetComponent<Storage>();
			this.ElementConverter = base.GetComponent<ElementConverter>();
			this.JournalDelivery = base.GetComponent<ManualDeliveryKG>();
			this.Operational = base.GetComponent<Operational>();
			this.Selectable = base.GetComponent<KSelectable>();
			this.notifier = base.GetComponent<Notifier>();
			this.controllers = base.gameObject.GetComponentsInChildren<KBatchedAnimController>();
			this.meter = new MeterController(this.BrainController, "meter_oxygen_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, MegaBrainTankConfig.METER_SYMBOLS);
			this.fxLink = new KAnimLink(this.BrainController, this.ShelfController);
		}

		public override void StartSM()
		{
			this.InitializeEffectsList();
			base.StartSM();
			this.BrainController.onAnimComplete += this.OnAnimComplete;
			this.ShelfController.onAnimComplete += this.OnAnimComplete;
			Storage brainStorage = this.BrainStorage;
			brainStorage.OnWorkableEventCB = (Action<Workable, Workable.WorkableEvent>)Delegate.Combine(brainStorage.OnWorkableEventCB, new Action<Workable, Workable.WorkableEvent>(this.OnJournalDeliveryStateChanged));
			this.brainHum = GlobalAssets.GetSound("MegaBrainTank_brain_wave_LP", false);
			StoryManager.Instance.DiscoverStoryEvent(Db.Get().Stories.MegaBrainTank);
			this.MeterPosition = this.DesiredMeterPosition;
			if (this.GetCurrentState() == base.sm.common.dormant)
			{
				short num = (short)(5f * this.MeterPosition);
				if (num > 0)
				{
					this.nextActiveBrain = num;
					this.BrainSounds.StartSound(this.brainHum);
					this.BrainSounds.SetParameter(this.brainHum, "BrainTankProgress", (float)num);
					this.CompleteBrainActivation();
					return;
				}
			}
			else
			{
				StoryManager.Instance.BeginStoryEvent(Db.Get().Stories.MegaBrainTank);
				this.nextActiveBrain = 5;
				this.CompleteBrainActivation();
			}
		}

		public override void StopSM(string reason)
		{
			this.BrainController.onAnimComplete -= this.OnAnimComplete;
			this.ShelfController.onAnimComplete -= this.OnAnimComplete;
			Storage brainStorage = this.BrainStorage;
			brainStorage.OnWorkableEventCB = (Action<Workable, Workable.WorkableEvent>)Delegate.Remove(brainStorage.OnWorkableEventCB, new Action<Workable, Workable.WorkableEvent>(this.OnJournalDeliveryStateChanged));
			base.StopSM(reason);
		}

		private void InitializeEffectsList()
		{
			Components.Cmps<MinionIdentity> liveMinionIdentities = Components.LiveMinionIdentities;
			liveMinionIdentities.OnAdd += this.OnLiveMinionIdAdded;
			liveMinionIdentities.OnRemove += this.OnLiveMinionIdRemoved;
			MegaBrainTank.StatesInstance.minionEffects = new List<Effects>((liveMinionIdentities.Count > 32) ? liveMinionIdentities.Count : 32);
			for (int i = 0; i < liveMinionIdentities.Count; i++)
			{
				this.OnLiveMinionIdAdded(liveMinionIdentities[i]);
			}
		}

		private void OnLiveMinionIdAdded(MinionIdentity id)
		{
			Effects component = id.GetComponent<Effects>();
			MegaBrainTank.StatesInstance.minionEffects.Add(component);
			if (this.GetCurrentState() == base.sm.common.active)
			{
				component.Add(base.sm.StatBonus, false);
			}
		}

		private void OnLiveMinionIdRemoved(MinionIdentity id)
		{
			Effects component = id.GetComponent<Effects>();
			MegaBrainTank.StatesInstance.minionEffects.Remove(component);
		}

		public void SetBonusActive(bool active)
		{
			for (int i = 0; i < MegaBrainTank.StatesInstance.minionEffects.Count; i++)
			{
				if (active)
				{
					MegaBrainTank.StatesInstance.minionEffects[i].Add(base.sm.StatBonus, false);
				}
				else
				{
					MegaBrainTank.StatesInstance.minionEffects[i].Remove(base.sm.StatBonus);
				}
			}
		}

		private void OnAnimComplete(HashedString anim)
		{
			if (anim == MegaBrainTankConfig.KACHUNK)
			{
				this.StoreJournals();
				return;
			}
			if ((anim == base.smi.CurrentActivationAnim || anim == MegaBrainTankConfig.ACTIVATE_ALL) && this.GetCurrentState() != base.sm.common.idle)
			{
				this.CompleteBrainActivation();
			}
		}

		private void OnJournalDeliveryStateChanged(Workable w, Workable.WorkableEvent state)
		{
			if (state == Workable.WorkableEvent.WorkStopped)
			{
				return;
			}
			if (state != Workable.WorkableEvent.WorkStarted)
			{
				this.ShelfController.Play(MegaBrainTankConfig.KACHUNK, KAnim.PlayMode.Once, 1f, 0f);
				return;
			}
			FetchAreaChore.StatesInstance smi = w.worker.GetSMI<FetchAreaChore.StatesInstance>();
			if (smi.IsNullOrStopped())
			{
				return;
			}
			GameObject gameObject = smi.sm.deliveryObject.Get(smi);
			if (gameObject == null)
			{
				return;
			}
			Pickupable component = gameObject.GetComponent<Pickupable>();
			this.UnitsFromLastStore = (short)component.PrimaryElement.Units;
			float num = Mathf.Clamp01(component.PrimaryElement.Units / 5f);
			this.BrainStorage.SetWorkTime(num * this.BrainStorage.storageWorkTime);
		}

		public void ShelveJournals(float dt)
		{
			float num = this.lastRemainingTime - this.BrainStorage.WorkTimeRemaining;
			if (num <= 0f)
			{
				num = this.BrainStorage.storageWorkTime - this.BrainStorage.WorkTimeRemaining;
			}
			this.lastRemainingTime = this.BrainStorage.WorkTimeRemaining;
			if (this.BrainStorage.storageWorkTime / 5f - this.journalActivationTimer > 0.001f)
			{
				this.journalActivationTimer += num;
				return;
			}
			int num2 = -1;
			this.journalActivationTimer = 0f;
			for (int i = 0; i < MegaBrainTankConfig.JOURNAL_SYMBOLS.Length; i++)
			{
				byte b = (byte)(1 << i);
				bool flag = (this.activatedJournals & b) == 0;
				if (flag && num2 == -1)
				{
					num2 = i;
				}
				if (flag & (global::UnityEngine.Random.Range(0f, 1f) >= 0.5f))
				{
					num2 = -1;
					this.activatedJournals |= b;
					this.ShelfController.SetSymbolVisiblity(MegaBrainTankConfig.JOURNAL_SYMBOLS[i], true);
					break;
				}
			}
			if (num2 != -1)
			{
				this.ShelfController.SetSymbolVisiblity(MegaBrainTankConfig.JOURNAL_SYMBOLS[num2], true);
			}
			this.UnitsFromLastStore -= 1;
		}

		public void StoreJournals()
		{
			this.lastRemainingTime = 0f;
			this.activatedJournals = 0;
			for (int i = 0; i < MegaBrainTankConfig.JOURNAL_SYMBOLS.Length; i++)
			{
				this.ShelfController.SetSymbolVisiblity(MegaBrainTankConfig.JOURNAL_SYMBOLS[i], false);
			}
			this.ShelfController.PlayMode = KAnim.PlayMode.Paused;
			this.ShelfController.SetPositionPercent(0f);
			this.meterTarget = this.DesiredMeterPosition;
		}

		public void ActivateBrains(float dt)
		{
			if (this.currentlyActivating)
			{
				return;
			}
			this.currentlyActivating = (float)this.nextActiveBrain / 5f - this.MeterPosition <= 0.001f;
			if (!this.currentlyActivating)
			{
				return;
			}
			this.BrainController.QueueAndSyncTransition(this.CurrentActivationAnim, KAnim.PlayMode.Once, 1f, 0f);
			if (this.nextActiveBrain > 0)
			{
				this.BrainSounds.StartSound(this.brainHum);
				this.BrainSounds.SetParameter(this.brainHum, "BrainTankProgress", (float)this.nextActiveBrain);
			}
		}

		public void CompleteBrainActivation()
		{
			this.BrainController.Play(this.currentActivationLoop, KAnim.PlayMode.Loop, 1f, 0f);
			this.nextActiveBrain += 1;
			this.currentlyActivating = false;
			if (this.nextActiveBrain > 5)
			{
				this.CompleteEvent();
			}
		}

		public void Digest()
		{
			if (this.meterTarget > this.MeterPosition)
			{
				return;
			}
			this.meterTarget = 0f;
			this.MeterPosition = this.DesiredMeterPosition;
		}

		public void CleanTank(bool active)
		{
			this.SetBonusActive(active);
			base.GetComponent<Light2D>().enabled = active;
			this.Selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.MegaBrainTankDreamAnalysis, active, this);
			this.ElementConverter.SetAllConsumedActive(active);
			this.BrainController.ClearQueue();
			if (active)
			{
				this.nextActiveBrain = 5;
				this.BrainController.QueueAndSyncTransition(MegaBrainTankConfig.ACTIVATE_ALL, KAnim.PlayMode.Once, 1f, 0f);
				this.BrainSounds.StartSound(this.brainHum);
				this.BrainSounds.SetParameter(this.brainHum, "BrainTankProgress", (float)this.nextActiveBrain);
				return;
			}
			if (this.BrainStorage.GetMassAvailable(DreamJournalConfig.ID) > 0f && !this.ElementConverter.HasEnoughMassToStartConverting(true))
			{
				this.BrainStorage.ConsumeAllIgnoringDisease(DreamJournalConfig.ID);
				this.MeterPosition = 0f;
			}
			this.BrainController.QueueAndSyncTransition(MegaBrainTankConfig.DEACTIVATE_ALL, KAnim.PlayMode.Once, 1f, 0f);
			this.BrainSounds.StopSound(this.brainHum);
		}

		public bool IncrementMeter(float dt)
		{
			if (this.meterTarget <= this.MeterPosition)
			{
				return false;
			}
			this.MeterPosition = Mathf.Min(this.MeterPosition + 0.04f * dt, this.meterTarget);
			return this.meterTarget > this.MeterPosition;
		}

		public void CompleteEvent()
		{
			this.Selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.MegaBrainTankActivationProgress, false);
			this.Selectable.AddStatusItem(Db.Get().BuildingStatusItems.MegaBrainTankComplete, base.smi);
			StoryInstance storyInstance = StoryManager.Instance.GetStoryInstance(Db.Get().Stories.MegaBrainTank.HashId);
			if (storyInstance == null || storyInstance.CurrentState == StoryInstance.State.COMPLETE)
			{
				return;
			}
			this.eventInfo = EventInfoDataHelper.GenerateStoryTraitData(CODEX.STORY_TRAITS.MEGA_BRAIN_TANK.END_POPUP.NAME, CODEX.STORY_TRAITS.MEGA_BRAIN_TANK.END_POPUP.DESCRIPTION, CODEX.STORY_TRAITS.MEGA_BRAIN_TANK.END_POPUP.BUTTON, "braintankcomplete_kanim", EventInfoDataHelper.PopupType.COMPLETE, null, null, null);
			base.smi.Selectable.AddStatusItem(Db.Get().MiscStatusItems.AttentionRequired, base.smi);
			this.eventComplete = EventInfoScreen.CreateNotification(this.eventInfo, new Notification.ClickCallback(this.ShowEventCompleteUI));
			this.notifier.Add(this.eventComplete, "");
		}

		public void ShowEventCompleteUI(object _ = null)
		{
			if (this.eventComplete == null)
			{
				return;
			}
			base.smi.Selectable.RemoveStatusItem(Db.Get().MiscStatusItems.AttentionRequired, false);
			this.notifier.Remove(this.eventComplete);
			this.eventComplete = null;
			Game.Instance.unlocks.Unlock("story_trait_mega_brain_tank_competed", true);
			Vector3 vector = Grid.CellToPosCCC(Grid.OffsetCell(Grid.PosToCell(base.master), new CellOffset(0, 3)), Grid.SceneLayer.Ore);
			StoryManager.Instance.CompleteStoryEvent(Db.Get().Stories.MegaBrainTank, base.master, new FocusTargetSequence.Data
			{
				WorldId = base.master.GetMyWorldId(),
				OrthographicSize = 6f,
				TargetSize = 6f,
				Target = vector,
				PopupData = this.eventInfo,
				CompleteCB = new global::System.Action(this.OnCompleteStorySequence),
				CanCompleteCB = null
			});
		}

		private void OnCompleteStorySequence()
		{
			Vector3 vector = Grid.CellToPosCCC(Grid.OffsetCell(Grid.PosToCell(base.master), new CellOffset(0, 2)), Grid.SceneLayer.Ore);
			StoryManager.Instance.CompleteStoryEvent(Db.Get().Stories.MegaBrainTank, vector);
			this.eventInfo = null;
			base.sm.storyTraitCompleted.Trigger(this);
		}

		private static List<Effects> minionEffects;

		public short UnitsFromLastStore;

		private float meterPositionValue;

		private float meterTarget;

		private float journalActivationTimer;

		private float lastRemainingTime;

		private byte activatedJournals;

		private bool currentlyActivating;

		private short nextActiveBrain = 1;

		private string brainHum;

		private KBatchedAnimController[] controllers;

		private KAnimLink fxLink;

		private MeterController meter;

		private EventInfoData eventInfo;

		private Notification eventComplete;

		private Notifier notifier;
	}
}
