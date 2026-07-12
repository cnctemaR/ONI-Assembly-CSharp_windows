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
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		base.Unsubscribe(-1503271301);
	}

	private void OnBuildingSelect(object obj)
	{
		if (!(bool)obj)
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
			default_state = this.brain;
			this.brain.Initialize(this);
			this.brain.DefaultState(this.brain.inactive);
			this.brain.active.ParamTransition<bool>(this.activeParam, this.brain.inactive, new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Parameter<bool>.Callback(this.brain.IsInactive));
			this.brain.active.EventTransition(GameHashes.OperationalChanged, this.brain.inactive, new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Transition.ConditionCallback(this.brain.IsInactive));
			this.brain.inactive.ParamTransition<bool>(this.activeParam, this.brain.active.dormant, new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Parameter<bool>.Callback(this.brain.IsDormant));
			this.brain.inactive.EventTransition(GameHashes.OperationalChanged, this.brain.active.dormant, new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Transition.ConditionCallback(this.brain.IsDormant));
			this.brain.active.conscious.ParamTransition<bool>(this.dormantParam, this.brain.active.dormant, new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Parameter<bool>.Callback(this.brain.IsDormant));
			this.brain.inactive.ParamTransition<bool>(this.activeParam, this.brain.active.conscious, new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Parameter<bool>.Callback(this.brain.IsConscious));
			this.brain.inactive.EventTransition(GameHashes.OperationalChanged, this.brain.active.conscious, new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Transition.ConditionCallback(this.brain.IsConscious));
			this.brain.active.dormant.ParamTransition<bool>(this.dormantParam, this.brain.active.conscious, new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.Parameter<bool>.Callback(this.brain.IsConscious));
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

		public Operational.Flag activationCost = new Operational.Flag("brains restored", Operational.Flag.Type.Requirement);

		public MegaBrainTank.States.BrainState brain;

		public StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.BoolParameter dormantParam;

		public StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.BoolParameter activeParam;

		public Effect StatBonus;

		public class BrainState : BrainTankState
		{
			public bool IsInactive(MegaBrainTank.StatesInstance smi, bool _)
			{
				return !smi.IsActive;
			}

			public bool IsDormant(MegaBrainTank.StatesInstance smi, bool _)
			{
				return smi.IsActive && smi.IsHungry;
			}

			public bool IsConscious(MegaBrainTank.StatesInstance smi, bool _)
			{
				return smi.IsActive && !smi.IsHungry;
			}

			public bool IsInactive(MegaBrainTank.StatesInstance smi)
			{
				return this.IsInactive(smi, false);
			}

			public bool IsDormant(MegaBrainTank.StatesInstance smi)
			{
				return this.IsDormant(smi, false);
			}

			public bool IsConscious(MegaBrainTank.StatesInstance smi)
			{
				return this.IsConscious(smi, false);
			}

			public override void Initialize(MegaBrainTank.States sm)
			{
				base.EventHandler(GameHashes.BuildingActivated, new GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.GameEvent.Callback(this.OnActivatableChanged));
				base.EventHandler(GameHashes.OperationalChanged, new GameStateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.GameEvent.Callback(this.OnBuildingOperationalChanged));
				base.Update(new Action<MegaBrainTank.StatesInstance, float>(this.OnUpdate), UpdateRate.SIM_33ms, false);
				this.inactive.Initialize(sm);
				this.active.Initialize(sm);
			}

			public override void OnUpdate(MegaBrainTank.StatesInstance smi, float dt)
			{
				smi.IncrementMeter(dt);
				if (smi.UnitsFromLastStore == 0)
				{
					return;
				}
				smi.ShelveJournals(dt);
			}

			public override void OnAnimComplete(MegaBrainTank.StatesInstance smi, HashedString completedAnim)
			{
				if (completedAnim != MegaBrainTankConfig.KACHUNK)
				{
					return;
				}
				smi.StoreJournals();
			}

			private void OnBuildingOperationalChanged(MegaBrainTank.StatesInstance smi, object _)
			{
				if (!smi.IsActive)
				{
					return;
				}
				smi.Operational.SetActive(true, false);
			}

			private void OnActivatableChanged(MegaBrainTank.StatesInstance smi, object data)
			{
				if (!(bool)data)
				{
					return;
				}
				if (!this.sm.activeParam.Get(smi))
				{
					StoryManager.Instance.BeginStoryEvent(Db.Get().Stories.MegaBrainTank);
					smi.Selectable.AddStatusItem(Db.Get().BuildingStatusItems.MegaBrainTankActivationProgress, smi);
					return;
				}
				smi.Selectable.AddStatusItem(Db.Get().BuildingStatusItems.MegaBrainTankComplete, smi);
			}

			public MegaBrainTank.States.InactiveState inactive;

			public MegaBrainTank.States.ActiveState active;
		}

		public class InactiveState : BrainTankState
		{
			public override void Initialize(MegaBrainTank.States sm)
			{
				base.Enter(new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State.Callback(this.OnEnter));
				base.Update(new Action<MegaBrainTank.StatesInstance, float>(this.OnUpdate), UpdateRate.SIM_33ms, false);
				base.Exit(new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State.Callback(this.OnExit));
			}

			public override void OnEnter(MegaBrainTank.StatesInstance smi)
			{
				smi.SetBonusActive(false);
				smi.ElementConverter.SetAllConsumedActive(false);
				smi.Selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.MegaBrainTankDreamAnalysis, false);
				smi.ElementConverter.SetConsumedElementActive(DreamJournalConfig.ID, false);
				smi.master.GetComponent<Light2D>().enabled = false;
			}

			public override void OnExit(MegaBrainTank.StatesInstance smi)
			{
				smi.ElementConverter.SetConsumedElementActive(DreamJournalConfig.ID, true);
				RequireInputs component = smi.GetComponent<RequireInputs>();
				component.requireConduitHasMass = true;
				component.visualizeRequirements = RequireInputs.Requirements.All;
			}

			public override void OnUpdate(MegaBrainTank.StatesInstance smi, float dt)
			{
				smi.ActivateBrains(dt);
			}

			public override void OnAnimComplete(MegaBrainTank.StatesInstance smi, HashedString completedAnim)
			{
				if (completedAnim != smi.CurrentActivationAnim)
				{
					return;
				}
				smi.CompleteBrainActivation();
			}
		}

		public class ActiveState : BrainTankState
		{
			public override void Initialize(MegaBrainTank.States sm)
			{
				base.Enter(new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State.Callback(this.OnEnter));
				base.Exit(new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State.Callback(this.OnExit));
				this.dormant.Initialize(sm);
				this.conscious.Initialize(sm);
			}

			public override void OnEnter(MegaBrainTank.StatesInstance smi)
			{
				smi.master.GetComponent<Light2D>().enabled = false;
			}

			public override void OnExit(MegaBrainTank.StatesInstance smi)
			{
				smi.ElementConverter.SetConsumedElementActive(DreamJournalConfig.ID, false);
			}

			public MegaBrainTank.States.DormantState dormant;

			public MegaBrainTank.States.ConsciousState conscious;
		}

		public class DormantState : BrainTankState
		{
			public override void Initialize(MegaBrainTank.States sm)
			{
				base.EventHandler(GameHashes.OnStorageChange, new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State.Callback(this.OnStorageChanged));
				base.Enter(new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State.Callback(this.OnEnter));
			}

			public override void OnEnter(MegaBrainTank.StatesInstance smi)
			{
				smi.CleanTank();
				bool flag = smi.ElementConverter.HasEnoughMass(GameTags.Oxygen, true);
				smi.Selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.MegaBrainNotEnoughOxygen, !flag, null);
				smi.master.GetComponent<Light2D>().enabled = false;
			}

			private void OnStorageChanged(MegaBrainTank.StatesInstance smi)
			{
				float massAvailable = smi.BrainStorage.GetMassAvailable(GameTags.Oxygen);
				float massAvailable2 = smi.BrainStorage.GetMassAvailable(DreamJournalConfig.ID);
				bool flag = massAvailable >= 1f;
				smi.sm.dormantParam.Set(massAvailable2 <= 0f || !flag, smi, false);
				smi.Selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.MegaBrainNotEnoughOxygen, !flag, null);
			}
		}

		public class ConsciousState : BrainTankState
		{
			public override void Initialize(MegaBrainTank.States sm)
			{
				base.Enter(new StateMachine<MegaBrainTank.States, MegaBrainTank.StatesInstance, MegaBrainTank, object>.State.Callback(this.OnEnter));
				base.Update(new Action<MegaBrainTank.StatesInstance, float>(this.OnUpdate), UpdateRate.SIM_33ms, false);
			}

			public override void OnEnter(MegaBrainTank.StatesInstance smi)
			{
				smi.CleanTank();
				smi.Selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.MegaBrainNotEnoughOxygen, false);
				smi.master.GetComponent<Light2D>().enabled = true;
			}

			public override void OnUpdate(MegaBrainTank.StatesInstance smi, float dt)
			{
				smi.Digest(dt);
			}

			public override void OnAnimComplete(MegaBrainTank.StatesInstance smi, HashedString completedAnim)
			{
				if (completedAnim != MegaBrainTankConfig.ACTIVATE_ALL)
				{
					return;
				}
				smi.CompleteBrainActivation();
			}
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

		public bool IsActive
		{
			get
			{
				return this.Operational.IsOperational && base.sm.activeParam.Get(this);
			}
		}

		public bool IsHungry
		{
			get
			{
				return !this.ElementConverter.HasEnoughMassToStartConverting(true);
			}
		}

		public int TimeTilDigested
		{
			get
			{
				return (int)this.timeTilDigested;
			}
		}

		public int ActivationProgress
		{
			get
			{
				return (int)(25f * this.meterFill);
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
			bool flag = base.sm.activeParam.Get(this);
			this.Operational.SetFlag(base.sm.activationCost, flag);
			float unitsAvailable = this.BrainStorage.GetUnitsAvailable(DreamJournalConfig.ID);
			if (!flag)
			{
				this.meterFill = (this.targetProgress = unitsAvailable / 25f);
				this.meter.SetPositionPercent(this.meterFill);
				short num = (short)(5f * this.meterFill);
				if (num > 0)
				{
					this.nextActiveBrain = num;
					this.BrainSounds.StartSound(this.brainHum);
					this.BrainSounds.SetParameter(this.brainHum, "BrainTankProgress", (float)num);
					this.CompleteBrainActivation();
				}
				return;
			}
			this.timeTilDigested = unitsAvailable * 60f;
			this.meterFill = this.timeTilDigested - this.timeTilDigested % 0.04f;
			this.meterFill /= 1500f;
			this.meter.SetPositionPercent(this.meterFill);
			StoryManager.Instance.BeginStoryEvent(Db.Get().Stories.MegaBrainTank);
			this.nextActiveBrain = 5;
			this.CompleteBrainActivation();
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
			if (!base.sm.activeParam.Get(this) || !base.sm.dormantParam.Get(this) || !this.IsActive)
			{
				return;
			}
			component.Add(base.sm.StatBonus, false);
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
			BrainTankState brainTankState = this.GetCurrentState() as BrainTankState;
			if (brainTankState == null)
			{
				return;
			}
			while (brainTankState != null)
			{
				brainTankState.OnAnimComplete(this, anim);
				brainTankState = brainTankState.parent as BrainTankState;
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
			Pickupable component = smi.sm.deliveryObject.Get(smi).GetComponent<Pickupable>();
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
			float unitsAvailable = this.BrainStorage.GetUnitsAvailable(DreamJournalConfig.ID);
			this.targetProgress = Mathf.Clamp01(unitsAvailable / 25f);
		}

		public void ActivateBrains(float dt)
		{
			if (this.currentlyActivating)
			{
				return;
			}
			this.currentlyActivating = (float)this.nextActiveBrain / 5f - this.meterFill <= 0.001f;
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
				float unitsAvailable = this.BrainStorage.GetUnitsAvailable(DreamJournalConfig.ID);
				this.timeTilDigested = unitsAvailable * 60f;
				this.Operational.SetFlag(base.sm.activationCost, true);
				this.CompleteEvent();
			}
		}

		public void Digest(float dt)
		{
			float unitsAvailable = this.BrainStorage.GetUnitsAvailable(DreamJournalConfig.ID);
			this.timeTilDigested = unitsAvailable * 60f;
			base.sm.dormantParam.Set(this.IsHungry, this, false);
			if (this.targetProgress - this.meterFill > Mathf.Epsilon)
			{
				return;
			}
			this.targetProgress = 0f;
			float num = this.meterFill - this.timeTilDigested / 1500f;
			if (num >= 0.04f)
			{
				this.meterFill -= num - num % 0.04f;
				this.meter.SetPositionPercent(this.meterFill);
			}
		}

		public void CleanTank()
		{
			bool flag = !base.sm.dormantParam.Get(this);
			this.SetBonusActive(flag);
			this.Selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.MegaBrainTankDreamAnalysis, flag, this);
			this.ElementConverter.SetAllConsumedActive(flag);
			if (flag)
			{
				this.nextActiveBrain = 5;
				this.BrainController.QueueAndSyncTransition(MegaBrainTankConfig.ACTIVATE_ALL, KAnim.PlayMode.Once, 1f, 0f);
				this.BrainSounds.StartSound(this.brainHum);
				this.BrainSounds.SetParameter(this.brainHum, "BrainTankProgress", (float)this.nextActiveBrain);
				return;
			}
			if (this.timeTilDigested < 0.016666668f)
			{
				this.BrainStorage.ConsumeAllIgnoringDisease(DreamJournalConfig.ID);
				this.timeTilDigested = 0f;
				this.meterFill = 0f;
				this.meter.SetPositionPercent(this.meterFill);
			}
			this.BrainController.QueueAndSyncTransition(MegaBrainTankConfig.DEACTIVATE_ALL, KAnim.PlayMode.Once, 1f, 0f);
			this.BrainSounds.StopSound(this.brainHum);
		}

		public bool IncrementMeter(float dt)
		{
			if (this.targetProgress - this.meterFill <= Mathf.Epsilon)
			{
				return false;
			}
			this.meterFill += Mathf.Lerp(0f, 1f, 0.04f * dt);
			if (1f - this.meterFill <= 0.001f)
			{
				this.meterFill = 1f;
			}
			this.meter.SetPositionPercent(this.meterFill);
			return this.targetProgress - this.meterFill > 0.001f;
		}

		public void CompleteEvent()
		{
			this.Selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.MegaBrainTankActivationProgress, false);
			this.Selectable.AddStatusItem(Db.Get().BuildingStatusItems.MegaBrainTankComplete, base.smi);
			StoryInstance storyInstance = StoryManager.Instance.GetStoryInstance(Db.Get().Stories.MegaBrainTank.HashId);
			if (storyInstance == null || (base.sm.activeParam.Get(this) && storyInstance.CurrentState == StoryInstance.State.COMPLETE))
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
			base.sm.dormantParam.Set(base.smi.IsHungry, base.smi, false);
			base.sm.activeParam.Set(true, this, false);
		}

		private static List<Effects> minionEffects;

		public short UnitsFromLastStore;

		private float meterFill = 0.04f;

		private float targetProgress;

		private float timeTilDigested;

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
