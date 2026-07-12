using System;
using Klei;
using STRINGS;
using UnityEngine;

public class MorbRoverMaker : GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.no_operational;
		this.root.Update(new Action<MorbRoverMaker.Instance, float>(MorbRoverMaker.GermsRequiredFeedbackUpdate), UpdateRate.SIM_1000ms, false);
		this.no_operational.TagTransition(GameTags.Operational, this.operational, false);
		this.operational.TagTransition(GameTags.Operational, this.no_operational, true).DefaultState(this.operational.covered);
		this.operational.covered.ToggleStatusItem(Db.Get().BuildingStatusItems.MorbRoverMakerDusty, null).ParamTransition<bool>(this.WasUncoverByDuplicant, this.operational.idle, GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.IsTrue).Enter(delegate(MorbRoverMaker.Instance smi)
		{
			MorbRoverMaker.DisableManualDelivery(smi, "Machine can't ask for materials if it has not been investigated by a dupe");
		})
			.DefaultState(this.operational.covered.idle);
		this.operational.covered.idle.PlayAnim("dusty").ParamTransition<bool>(this.UncoverOrderRequested, this.operational.covered.careOrderGiven, GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.IsTrue);
		this.operational.covered.careOrderGiven.PlayAnim("dusty").Enter(new StateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State.Callback(MorbRoverMaker.StartWorkChore_RevealMachine)).Exit(new StateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State.Callback(MorbRoverMaker.CancelWorkChore_RevealMachine))
			.WorkableCompleteTransition((MorbRoverMaker.Instance smi) => smi.GetWorkable_RevealMachine(), this.operational.covered.complete)
			.ParamTransition<bool>(this.UncoverOrderRequested, this.operational.covered.idle, GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.IsFalse);
		this.operational.covered.complete.Enter(new StateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State.Callback(MorbRoverMaker.SetUncovered));
		this.operational.idle.Enter(delegate(MorbRoverMaker.Instance smi)
		{
			MorbRoverMaker.EnableManualDelivery(smi, "Operational and discovered");
		}).EnterTransition(this.operational.crafting, new StateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.Transition.ConditionCallback(MorbRoverMaker.ShouldBeCrafting)).EnterTransition(this.operational.waitingForMorb, new StateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.Transition.ConditionCallback(MorbRoverMaker.IsCraftingCompleted))
			.EventTransition(GameHashes.OnStorageChange, this.operational.crafting, new StateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.Transition.ConditionCallback(MorbRoverMaker.ShouldBeCrafting))
			.PlayAnim("idle")
			.ToggleStatusItem(Db.Get().BuildingStatusItems.MorbRoverMakerGermCollectionProgress, null);
		this.operational.crafting.DefaultState(this.operational.crafting.pre).ToggleStatusItem(Db.Get().BuildingStatusItems.MorbRoverMakerGermCollectionProgress, null).ToggleStatusItem(Db.Get().BuildingStatusItems.MorbRoverMakerCraftingBody, null);
		this.operational.crafting.pre.PlayAnim("crafting_pre").OnAnimQueueComplete(this.operational.crafting.loop);
		this.operational.crafting.loop.Update(new Action<MorbRoverMaker.Instance, float>(MorbRoverMaker.CraftingUpdate), UpdateRate.SIM_200ms, false).PlayAnim("crafting_loop", KAnim.PlayMode.Loop).ParamTransition<float>(this.CraftProgress, this.operational.crafting.pst, GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.IsOne);
		this.operational.crafting.pst.PlayAnim("crafting_pst").OnAnimQueueComplete(this.operational.waitingForMorb);
		this.operational.waitingForMorb.PlayAnim("crafting_complete").ParamTransition<long>(this.Germs, this.operational.doctor, new StateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.Parameter<long>.Callback(MorbRoverMaker.HasEnoughGerms)).ToggleStatusItem(Db.Get().BuildingStatusItems.MorbRoverMakerGermCollectionProgress, null);
		this.operational.doctor.Enter(new StateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State.Callback(MorbRoverMaker.StartWorkChore_ReleaseRover)).Exit(new StateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State.Callback(MorbRoverMaker.CancelWorkChore_ReleaseRover)).WorkableCompleteTransition((MorbRoverMaker.Instance smi) => smi.GetWorkable_ReleaseRover(), this.operational.finish)
			.DefaultState(this.operational.doctor.needed);
		this.operational.doctor.needed.PlayAnim("waiting").WorkableStartTransition((MorbRoverMaker.Instance smi) => smi.GetWorkable_ReleaseRover(), this.operational.doctor.working).ToggleStatusItem(Db.Get().BuildingStatusItems.MorbRoverMakerReadyForDoctor, null);
		this.operational.doctor.working.WorkableStopTransition((MorbRoverMaker.Instance smi) => smi.GetWorkable_ReleaseRover(), this.operational.doctor.needed);
		this.operational.finish.Enter(new StateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State.Callback(MorbRoverMaker.SpawnRover)).GoTo(this.operational.idle);
	}

	public static bool ShouldBeCrafting(MorbRoverMaker.Instance smi)
	{
		return smi.HasMaterialsForRover && smi.RoverDevelopment_Progress < 1f;
	}

	public static bool IsCraftingCompleted(MorbRoverMaker.Instance smi)
	{
		return smi.RoverDevelopment_Progress == 1f;
	}

	public static bool HasEnoughGerms(MorbRoverMaker.Instance smi, long germCount)
	{
		return germCount >= smi.def.GERMS_PER_ROVER;
	}

	public static void StartWorkChore_ReleaseRover(MorbRoverMaker.Instance smi)
	{
		smi.CreateWorkChore_ReleaseRover();
	}

	public static void CancelWorkChore_ReleaseRover(MorbRoverMaker.Instance smi)
	{
		smi.CancelWorkChore_ReleaseRover();
	}

	public static void StartWorkChore_RevealMachine(MorbRoverMaker.Instance smi)
	{
		smi.CreateWorkChore_RevealMachine();
	}

	public static void CancelWorkChore_RevealMachine(MorbRoverMaker.Instance smi)
	{
		smi.CancelWorkChore_RevealMachine();
	}

	public static void SetUncovered(MorbRoverMaker.Instance smi)
	{
		smi.Uncover();
	}

	public static void SpawnRover(MorbRoverMaker.Instance smi)
	{
		smi.SpawnRover();
	}

	public static void EnableManualDelivery(MorbRoverMaker.Instance smi, string reason)
	{
		smi.EnableManualDelivery(reason);
	}

	public static void DisableManualDelivery(MorbRoverMaker.Instance smi, string reason)
	{
		smi.DisableManualDelivery(reason);
	}

	public static void CraftingUpdate(MorbRoverMaker.Instance smi, float dt)
	{
		float num = Mathf.Clamp((smi.RoverDevelopment_Progress * smi.def.ROVER_CRAFTING_DURATION + dt) / smi.def.ROVER_CRAFTING_DURATION, 0f, 1f);
		smi.SetRoverDevelopmentProgress(num);
	}

	public static void GermsRequiredFeedbackUpdate(MorbRoverMaker.Instance smi, float dt)
	{
		if ((GameClock.Instance.GetTime() - smi.lastTimeGermsAdded > smi.def.FEEDBACK_NO_GERMS_DETECTED_TIMEOUT) & (smi.MorbDevelopment_Progress < 1f) & !smi.IsInsideState(smi.sm.operational.doctor) & smi.HasBeenRevealed)
		{
			smi.ShowGermRequiredStatusItemAlert();
			return;
		}
		smi.HideGermRequiredStatusItemAlert();
	}

	private const string ROBOT_PROGRESS_METER_TARGET_NAME = "meter_robot_target";

	private const string ROBOT_PROGRESS_METER_ANIMATION_NAME = "meter_robot";

	private const string COVERED_IDLE_ANIM_NAME = "dusty";

	private const string IDLE_ANIM_NAME = "idle";

	private const string CRAFT_PRE_ANIM_NAME = "crafting_pre";

	private const string CRAFT_LOOP_ANIM_NAME = "crafting_loop";

	private const string CRAFT_PST_ANIM_NAME = "crafting_pst";

	private const string CRAFT_COMPLETED_ANIM_NAME = "crafting_complete";

	private const string WAITING_FOR_DOCTOR_ANIM_NAME = "waiting";

	public StateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.BoolParameter UncoverOrderRequested;

	public StateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.BoolParameter WasUncoverByDuplicant;

	public StateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.LongParameter Germs;

	public StateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.FloatParameter CraftProgress;

	public GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State no_operational;

	public MorbRoverMaker.OperationalStates operational;

	public class Def : StateMachine.BaseDef
	{
		public float GetConduitMaxPackageMass()
		{
			ConduitType germ_INTAKE_CONDUIT_TYPE = this.GERM_INTAKE_CONDUIT_TYPE;
			if (germ_INTAKE_CONDUIT_TYPE == ConduitType.Gas)
			{
				return 1f;
			}
			if (germ_INTAKE_CONDUIT_TYPE != ConduitType.Liquid)
			{
				return 1f;
			}
			return 10f;
		}

		public float FEEDBACK_NO_GERMS_DETECTED_TIMEOUT = 2f;

		public Tag ROVER_PREFAB_ID;

		public float INITIAL_MORB_DEVELOPMENT_PERCENTAGE;

		public float ROVER_CRAFTING_DURATION;

		public float METAL_PER_ROVER;

		public long GERMS_PER_ROVER;

		public int MAX_GERMS_TAKEN_PER_PACKAGE;

		public int GERM_TYPE;

		public SimHashes ROVER_MATERIAL;

		public ConduitType GERM_INTAKE_CONDUIT_TYPE;
	}

	public class CoverStates : GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State
	{
		public GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State idle;

		public GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State careOrderGiven;

		public GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State complete;
	}

	public class OperationalStates : GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State
	{
		public MorbRoverMaker.CoverStates covered;

		public GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State idle;

		public MorbRoverMaker.CraftingStates crafting;

		public GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State waitingForMorb;

		public MorbRoverMaker.DoctorStates doctor;

		public GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State finish;
	}

	public class DoctorStates : GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State
	{
		public GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State needed;

		public GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State working;
	}

	public class CraftingStates : GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State
	{
		public GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State pre;

		public GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State loop;

		public GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.State pst;
	}

	public new class Instance : GameStateMachine<MorbRoverMaker, MorbRoverMaker.Instance, IStateMachineTarget, MorbRoverMaker.Def>.GameInstance, ISidescreenButtonControl
	{
		public long MorbDevelopment_GermsCollected
		{
			get
			{
				return base.sm.Germs.Get(base.smi);
			}
		}

		public long MorbDevelopment_RemainingGerms
		{
			get
			{
				return base.def.GERMS_PER_ROVER - this.MorbDevelopment_GermsCollected;
			}
		}

		public float MorbDevelopment_Progress
		{
			get
			{
				return Mathf.Clamp((float)this.MorbDevelopment_GermsCollected / (float)base.def.GERMS_PER_ROVER, 0f, 1f);
			}
		}

		public bool HasMaterialsForRover
		{
			get
			{
				return this.storage.GetMassAvailable(base.def.ROVER_MATERIAL) >= base.def.METAL_PER_ROVER;
			}
		}

		public float RoverDevelopment_Progress
		{
			get
			{
				return base.sm.CraftProgress.Get(base.smi);
			}
		}

		public bool HasBeenRevealed
		{
			get
			{
				return base.sm.WasUncoverByDuplicant.Get(base.smi);
			}
		}

		public bool CanPumpGerms
		{
			get
			{
				return this.operational && this.MorbDevelopment_Progress < 1f && this.HasBeenRevealed;
			}
		}

		public Workable GetWorkable_RevealMachine()
		{
			return this.workable_reveal;
		}

		public Workable GetWorkable_ReleaseRover()
		{
			return this.workable_release;
		}

		public void ShowGermRequiredStatusItemAlert()
		{
			if (this.germsRequiredAlertStatusItemHandle == default(Guid))
			{
				this.germsRequiredAlertStatusItemHandle = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.MorbRoverMakerNoGermsConsumedAlert, base.smi);
			}
		}

		public void HideGermRequiredStatusItemAlert()
		{
			if (this.germsRequiredAlertStatusItemHandle != default(Guid))
			{
				this.selectable.RemoveStatusItem(this.germsRequiredAlertStatusItemHandle, false);
				this.germsRequiredAlertStatusItemHandle = default(Guid);
			}
		}

		public Instance(IStateMachineTarget master, MorbRoverMaker.Def def)
			: base(master, def)
		{
			this.RobotProgressMeter = new MeterController(this.buildingAnimCtr, "meter_robot_target", "meter_robot", Meter.Offset.UserSpecified, Grid.SceneLayer.BuildingFront, Array.Empty<string>());
		}

		public override void StartSM()
		{
			Building component = base.GetComponent<Building>();
			this.inputCell = component.GetUtilityInputCell();
			this.outputCell = component.GetUtilityOutputCell();
			base.StartSM();
			if (!this.HasBeenRevealed)
			{
				base.sm.Germs.Set(0L, base.smi, false);
				this.AddGerms((long)((float)base.def.GERMS_PER_ROVER * base.def.INITIAL_MORB_DEVELOPMENT_PERCENTAGE), false);
			}
			Conduit.GetFlowManager(base.def.GERM_INTAKE_CONDUIT_TYPE).AddConduitUpdater(new Action<float>(this.Flow), ConduitFlowPriority.Default);
			this.UpdateMeters();
		}

		public void AddGerms(long amount, bool playAnimations = true)
		{
			long num = this.MorbDevelopment_GermsCollected + amount;
			base.sm.Germs.Set(num, base.smi, false);
			this.UpdateMeters();
			if (amount > 0L)
			{
				if (playAnimations)
				{
					this.capsule.PlayPumpGermsAnimation();
				}
				Action<long> germsAdded = this.GermsAdded;
				if (germsAdded != null)
				{
					germsAdded(amount);
				}
				this.lastTimeGermsAdded = GameClock.Instance.GetTime();
			}
		}

		public long RemoveGerms(long amount)
		{
			long num = amount.Min(this.MorbDevelopment_GermsCollected);
			long num2 = this.MorbDevelopment_GermsCollected - num;
			base.sm.Germs.Set(num2, base.smi, false);
			this.UpdateMeters();
			return num;
		}

		public void EnableManualDelivery(string reason)
		{
			this.manualDelivery.Pause(false, reason);
		}

		public void DisableManualDelivery(string reason)
		{
			this.manualDelivery.Pause(true, reason);
		}

		public void SetRoverDevelopmentProgress(float value)
		{
			base.sm.CraftProgress.Set(value, base.smi, false);
			this.UpdateMeters();
		}

		public void UpdateMeters()
		{
			this.RobotProgressMeter.SetPositionPercent(this.RoverDevelopment_Progress);
			this.capsule.SetMorbDevelopmentProgress(this.MorbDevelopment_Progress);
			this.capsule.SetGermMeterProgress(this.HasBeenRevealed ? this.MorbDevelopment_Progress : 0f);
		}

		public void Uncover()
		{
			base.sm.WasUncoverByDuplicant.Set(true, base.smi, false);
			global::System.Action onUncovered = this.OnUncovered;
			if (onUncovered == null)
			{
				return;
			}
			onUncovered();
		}

		public void CreateWorkChore_ReleaseRover()
		{
			if (this.workChore_releaseRover == null)
			{
				this.workChore_releaseRover = new WorkChore<MorbRoverMakerWorkable>(Db.Get().ChoreTypes.Doctor, this.workable_release, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
			}
		}

		public void CancelWorkChore_ReleaseRover()
		{
			if (this.workChore_releaseRover != null)
			{
				this.workChore_releaseRover.Cancel("MorbRoverMaker.CancelWorkChore_ReleaseRover");
				this.workChore_releaseRover = null;
			}
		}

		public void CreateWorkChore_RevealMachine()
		{
			if (this.workChore_revealMachine == null)
			{
				this.workChore_revealMachine = new WorkChore<MorbRoverMakerRevealWorkable>(Db.Get().ChoreTypes.Repair, this.workable_reveal, null, true, null, null, null, true, null, false, true, null, false, true, true, PriorityScreen.PriorityClass.basic, 5, false, true);
			}
		}

		public void CancelWorkChore_RevealMachine()
		{
			if (this.workChore_revealMachine != null)
			{
				this.workChore_revealMachine.Cancel("MorbRoverMaker.CancelWorkChore_RevealMachine");
				this.workChore_revealMachine = null;
			}
		}

		public void SpawnRover()
		{
			if (this.HasMaterialsForRover)
			{
				float num = 0f;
				float num2 = 0f;
				SimUtil.DiseaseInfo diseaseInfo;
				this.storage.ConsumeAndGetDisease(base.def.ROVER_MATERIAL.CreateTag(), base.def.METAL_PER_ROVER, out num2, out diseaseInfo, out num);
				this.RemoveGerms(base.def.GERMS_PER_ROVER);
				GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(base.def.ROVER_PREFAB_ID), base.gameObject.transform.GetPosition(), Grid.SceneLayer.Creatures, null, 0);
				PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
				component.AddDisease(diseaseInfo.idx, diseaseInfo.count, "From the materials provided for its creation");
				component.SetMassTemperature(component.Mass, num);
				gameObject.SetActive(true);
				this.SetRoverDevelopmentProgress(0f);
				Action<GameObject> onRoverSpawned = this.OnRoverSpawned;
				if (onRoverSpawned == null)
				{
					return;
				}
				onRoverSpawned(gameObject);
			}
		}

		private void Flow(float dt)
		{
			if (this.CanPumpGerms)
			{
				ConduitFlow flowManager = Conduit.GetFlowManager(base.def.GERM_INTAKE_CONDUIT_TYPE);
				int num = 0;
				if (flowManager.HasConduit(this.inputCell) && flowManager.HasConduit(this.outputCell))
				{
					ConduitFlow.ConduitContents contents = flowManager.GetContents(this.inputCell);
					ConduitFlow.ConduitContents contents2 = flowManager.GetContents(this.outputCell);
					float num2 = Mathf.Min(contents.mass, base.def.GetConduitMaxPackageMass() * dt);
					if (flowManager.CanMergeContents(contents, contents2, num2))
					{
						float amountAllowedForMerging = flowManager.GetAmountAllowedForMerging(contents, contents2, num2);
						if (amountAllowedForMerging > 0f)
						{
							ConduitFlow conduitFlow = ((base.def.GERM_INTAKE_CONDUIT_TYPE == ConduitType.Liquid) ? Game.Instance.liquidConduitFlow : Game.Instance.gasConduitFlow);
							int num3 = contents.diseaseCount;
							if (contents.diseaseIdx != 255 && (int)contents.diseaseIdx == base.def.GERM_TYPE)
							{
								num = (int)this.MorbDevelopment_RemainingGerms.Min((long)base.def.MAX_GERMS_TAKEN_PER_PACKAGE).Min((long)contents.diseaseCount);
								num3 -= num;
							}
							float num4 = conduitFlow.AddElement(this.outputCell, contents.element, amountAllowedForMerging, contents.temperature, contents.diseaseIdx, num3);
							if (amountAllowedForMerging != num4)
							{
								global::Debug.Log("[Morb Rover Maker] Mass Differs By: " + (amountAllowedForMerging - num4).ToString());
							}
							flowManager.RemoveElement(this.inputCell, num4);
						}
					}
				}
				if (num > 0)
				{
					this.AddGerms((long)num, true);
				}
			}
		}

		protected override void OnCleanUp()
		{
			base.OnCleanUp();
			Conduit.GetFlowManager(base.def.GERM_INTAKE_CONDUIT_TYPE).RemoveConduitUpdater(new Action<float>(this.Flow));
		}

		public string SidescreenButtonText
		{
			get
			{
				return base.sm.UncoverOrderRequested.Get(base.smi) ? CODEX.STORY_TRAITS.MORB_ROVER_MAKER.UI_SIDESCREENS.CANCEL_REVEAL_BTN : CODEX.STORY_TRAITS.MORB_ROVER_MAKER.UI_SIDESCREENS.REVEAL_BTN;
			}
		}

		public string SidescreenButtonTooltip
		{
			get
			{
				return base.sm.UncoverOrderRequested.Get(base.smi) ? CODEX.STORY_TRAITS.MORB_ROVER_MAKER.UI_SIDESCREENS.CANCEL_REVEAL_BTN_TOOLTIP : CODEX.STORY_TRAITS.MORB_ROVER_MAKER.UI_SIDESCREENS.REVEAL_BTN_TOOLTIP;
			}
		}

		public bool SidescreenEnabled()
		{
			return !base.smi.sm.WasUncoverByDuplicant.Get(base.smi);
		}

		public bool SidescreenButtonInteractable()
		{
			return !base.smi.sm.WasUncoverByDuplicant.Get(base.smi);
		}

		public int HorizontalGroupID()
		{
			return 0;
		}

		public int ButtonSideScreenSortOrder()
		{
			return 20;
		}

		public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
		{
			throw new NotImplementedException();
		}

		public void OnSidescreenButtonPressed()
		{
			bool flag = base.smi.sm.UncoverOrderRequested.Get(base.smi);
			base.smi.sm.UncoverOrderRequested.Set(!flag, base.smi, false);
		}

		public Action<long> GermsAdded;

		public global::System.Action OnUncovered;

		public Action<GameObject> OnRoverSpawned;

		[MyCmpGet]
		private MorbRoverMakerRevealWorkable workable_reveal;

		[MyCmpGet]
		private MorbRoverMakerWorkable workable_release;

		[MyCmpGet]
		private Operational operational;

		[MyCmpGet]
		private KBatchedAnimController buildingAnimCtr;

		[MyCmpGet]
		private ManualDeliveryKG manualDelivery;

		[MyCmpGet]
		private Storage storage;

		[MyCmpGet]
		private MorbRoverMaker_Capsule capsule;

		[MyCmpGet]
		private KSelectable selectable;

		private MeterController RobotProgressMeter;

		private int inputCell = -1;

		private int outputCell = -1;

		private Chore workChore_revealMachine;

		private Chore workChore_releaseRover;

		public float lastTimeGermsAdded = -1f;

		private Guid germsRequiredAlertStatusItemHandle;
	}
}
