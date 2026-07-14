using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class FixedCapturePoint : GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.operational;
		base.serializable = StateMachine.SerializeType.Both_DEPRECATED;
		this.unoperational.EventTransition(GameHashes.OperationalChanged, this.operational, new StateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.Transition.ConditionCallback(FixedCapturePoint.ShouldBeOn)).EventTransition(GameHashes.BuildingStrawChange, this.operational, new StateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.Transition.ConditionCallback(FixedCapturePoint.ShouldBeOn)).EventHandler(GameHashes.BuildingStrawChange, new GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.GameEvent.Callback(FixedCapturePoint.HandleBuildingStrawChange))
			.Enter(new StateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.State.Callback(FixedCapturePoint.Refresh))
			.DefaultState(this.unoperational.noOperational);
		this.unoperational.noOperational.EventTransition(GameHashes.OperationalChanged, this.unoperational.strawBlocked, (FixedCapturePoint.Instance smi) => FixedCapturePoint.IsOperational(smi) && FixedCapturePoint.IsStrawBlocked(smi)).EventTransition(GameHashes.OperationalChanged, this.unoperational.noLiquidOnStraw, (FixedCapturePoint.Instance smi) => FixedCapturePoint.IsOperational(smi) && FixedCapturePoint.IsStrawOutsideLiquid(smi));
		this.unoperational.strawBlocked.ToggleStatusItem(Db.Get().BuildingStatusItems.OutputTileBlocked, null).EventTransition(GameHashes.OperationalChanged, this.unoperational.noOperational, GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.Not(new StateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.Transition.ConditionCallback(FixedCapturePoint.IsOperational))).EventTransition(GameHashes.BuildingStrawChange, this.unoperational.noLiquidOnStraw, (FixedCapturePoint.Instance smi) => !FixedCapturePoint.IsStrawBlocked(smi) && FixedCapturePoint.IsStrawOutsideLiquid(smi));
		this.unoperational.noLiquidOnStraw.ToggleStatusItem(Db.Get().BuildingStatusItems.NotSubmerged, null).EventTransition(GameHashes.OperationalChanged, this.unoperational.noOperational, GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.Not(new StateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.Transition.ConditionCallback(FixedCapturePoint.IsOperational))).EventTransition(GameHashes.BuildingStrawChange, this.unoperational.strawBlocked, new StateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.Transition.ConditionCallback(FixedCapturePoint.IsStrawBlocked));
		this.operational.DefaultState(this.operational.manual).EventTransition(GameHashes.OperationalChanged, this.unoperational, GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.Not(new StateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.Transition.ConditionCallback(FixedCapturePoint.ShouldBeOn))).EventTransition(GameHashes.BuildingStrawChange, this.unoperational, GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.Not(new StateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.Transition.ConditionCallback(FixedCapturePoint.ShouldBeOn)))
			.EventHandler(GameHashes.BuildingStrawChange, new GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.GameEvent.Callback(FixedCapturePoint.HandleBuildingStrawChange))
			.Enter(new StateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.State.Callback(FixedCapturePoint.Refresh));
		this.operational.manual.ParamTransition<bool>(this.automated, this.operational.automated, GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.IsTrue);
		this.operational.automated.ParamTransition<bool>(this.automated, this.operational.manual, GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.IsFalse).ToggleChore((FixedCapturePoint.Instance smi) => smi.CreateChore(), this.unoperational, this.unoperational).Update("FindFixedCapturable", delegate(FixedCapturePoint.Instance smi, float dt)
		{
			smi.FindFixedCapturable();
		}, UpdateRate.SIM_1000ms, false);
	}

	public static bool ShouldBeOn(FixedCapturePoint.Instance smi)
	{
		return FixedCapturePoint.IsOperational(smi) && !FixedCapturePoint.IsStrawBlocked(smi) && !FixedCapturePoint.IsStrawOutsideLiquid(smi);
	}

	public static bool IsOperational(FixedCapturePoint.Instance smi)
	{
		return smi.IsOperational;
	}

	public static bool IsStrawBlocked(FixedCapturePoint.Instance smi)
	{
		return smi.IsStrawBlocked;
	}

	public static bool IsStrawOutsideLiquid(FixedCapturePoint.Instance smi)
	{
		return smi.IsStrawOutsideLiquid;
	}

	public static void HandleBuildingStrawChange(FixedCapturePoint.Instance smi, object o)
	{
		FixedCapturePoint.Refresh(smi);
	}

	public static void Refresh(FixedCapturePoint.Instance smi)
	{
		if (smi.IsStrawInstalled)
		{
			smi.UpdateCaptureCell(smi.Straw.GetBottomCellOffset());
		}
		smi.PlayOnOffAnim();
	}

	public static readonly Operational.Flag enabledFlag = new Operational.Flag("enabled", Operational.Flag.Type.Requirement);

	private StateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.BoolParameter automated;

	public FixedCapturePoint.UnoperationalStates unoperational;

	public FixedCapturePoint.OperationalState operational;

	public class Def : StateMachine.BaseDef
	{
		public Func<FixedCapturePoint.Instance, FixedCapturableMonitor.Instance, bool> isAmountStoredOverCapacity;

		public Func<FixedCapturePoint.Instance, int> getTargetCapturePoint = delegate(FixedCapturePoint.Instance smi)
		{
			int num = Grid.PosToCell(smi);
			Navigator navigator = smi.targetCapturable.Navigator;
			if (Grid.IsValidCell(num - 1) && navigator.CanReach(num - 1))
			{
				return num - 1;
			}
			if (Grid.IsValidCell(num + 1) && navigator.CanReach(num + 1))
			{
				return num + 1;
			}
			return num;
		};

		public bool allowBabies;

		public CellOffset captureCellOffset = new CellOffset(0, 0);

		public CellOffset rancherInteractOffset = new CellOffset(0, 0);

		public HashedString logicPortId = "CritterPickUpInput";

		public CellOffset? postCaptureOffset;

		public string preCaptureAnimName;

		public Func<FixedCapturePoint.Instance, string> getPreCaptureAnimSuffix;

		public string offAnimName;

		public string onAnimName;
	}

	public class OperationalState : GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.State
	{
		public GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.State manual;

		public GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.State automated;
	}

	public class UnoperationalStates : GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.State
	{
		public GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.State noOperational;

		public GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.State strawBlocked;

		public GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.State noLiquidOnStraw;
	}

	[SerializationConfig(MemberSerialization.OptIn)]
	public new class Instance : GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.GameInstance
	{
		public bool IsOperational
		{
			get
			{
				return this.operationComp != null && this.operationComp.IsOperational;
			}
		}

		public bool IsStrawInstalled
		{
			get
			{
				return this.Straw != null;
			}
		}

		public bool IsStrawOutsideLiquid
		{
			get
			{
				return this.IsStrawInstalled && !this.Straw.isInLiquid;
			}
		}

		public bool IsStrawBlocked
		{
			get
			{
				return this.IsStrawInstalled && this.Straw.currentDepth <= 0;
			}
		}

		public FixedCapturableMonitor.Instance targetCapturable { get; private set; }

		public bool shouldCreatureGoGetCaptured { get; private set; }

		public BuildingPointStraw Straw { get; private set; }

		public Instance(IStateMachineTarget master, FixedCapturePoint.Def def)
			: base(master, def)
		{
			base.Subscribe(-905833192, new Action<object>(this.OnCopySettings));
			this.captureCell = Grid.OffsetCell(Grid.PosToCell(base.transform.GetPosition()), def.captureCellOffset);
			this.critterCapactiy = base.GetComponent<BaggableCritterCapacityTracker>();
			this.Straw = base.GetComponent<BuildingPointStraw>();
			this.operationComp = base.GetComponent<Operational>();
			this.logicPorts = base.GetComponent<LogicPorts>();
			if (this.logicPorts != null)
			{
				base.Subscribe(-801688580, new Action<object>(this.OnLogicEvent));
				this.operationComp.SetFlag(FixedCapturePoint.enabledFlag, !this.logicPorts.IsPortConnected(def.logicPortId) || this.logicPorts.GetInputValue(def.logicPortId) > 0);
				return;
			}
			this.operationComp.SetFlag(FixedCapturePoint.enabledFlag, true);
		}

		public int GetRancherInteractCell()
		{
			return Grid.OffsetCell(Grid.PosToCell(base.transform.GetPosition()), base.def.rancherInteractOffset);
		}

		private void OnLogicEvent(object data)
		{
			LogicValueChanged logicValueChanged = (LogicValueChanged)data;
			if (logicValueChanged.portID == base.def.logicPortId && this.logicPorts.IsPortConnected(base.def.logicPortId))
			{
				this.operationComp.SetFlag(FixedCapturePoint.enabledFlag, logicValueChanged.newValue > 0);
			}
		}

		public void PlayOnOffAnim()
		{
			string text = ((this.Straw != null) ? this.Straw.GetAnimSuffix() : "");
			string text2;
			if (FixedCapturePoint.ShouldBeOn(this))
			{
				text2 = ((base.def.onAnimName != null) ? (base.def.onAnimName + text) : null);
			}
			else
			{
				text2 = ((base.def.offAnimName != null) ? (base.def.offAnimName + text) : null);
			}
			if (string.IsNullOrEmpty(text2))
			{
				return;
			}
			base.GetComponent<KBatchedAnimController>().Play(text2, KAnim.PlayMode.Once, 1f, 0f);
		}

		public override void StartSM()
		{
			base.StartSM();
			if (base.GetComponent<FixedCapturePoint.AutoWrangleCapture>() == null)
			{
				base.sm.automated.Set(true, this, false);
			}
		}

		private void OnCopySettings(object data)
		{
			GameObject gameObject = (GameObject)data;
			if (gameObject == null)
			{
				return;
			}
			FixedCapturePoint.Instance smi = gameObject.GetSMI<FixedCapturePoint.Instance>();
			if (smi == null)
			{
				return;
			}
			base.sm.automated.Set(base.sm.automated.Get(smi), this, false);
		}

		public bool GetAutomated()
		{
			return base.sm.automated.Get(this);
		}

		public void SetAutomated(bool automate)
		{
			base.sm.automated.Set(automate, this, false);
		}

		public Chore CreateChore()
		{
			this.FindFixedCapturable();
			return new FixedCaptureChore(base.GetComponent<KPrefabID>());
		}

		public bool IsCreatureAvailableForFixedCapture()
		{
			if (!this.targetCapturable.IsNullOrStopped())
			{
				CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(this.captureCell);
				return FixedCapturePoint.Instance.CanCapturableBeCapturedAtCapturePoint(this.targetCapturable, this, cavityForCell, this.captureCell);
			}
			return false;
		}

		public void SetRancherIsAvailableForCapturing()
		{
			this.shouldCreatureGoGetCaptured = true;
		}

		public void ClearRancherIsAvailableForCapturing()
		{
			this.shouldCreatureGoGetCaptured = false;
		}

		private static bool CanCapturableBeCapturedAtCapturePoint(FixedCapturableMonitor.Instance capturable, FixedCapturePoint.Instance capture_point, CavityInfo capture_cavity_info, int capture_cell)
		{
			if (!capturable.IsRunning())
			{
				return false;
			}
			if (capturable.targetCapturePoint != capture_point && !capturable.targetCapturePoint.IsNullOrStopped())
			{
				return false;
			}
			int num = Grid.PosToCell(capturable.transform.GetPosition());
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
			return cavityForCell != null && cavityForCell == capture_cavity_info && !capturable.HasTag(GameTags.Creatures.Bagged) && (!capturable.isBaby || capture_point.def.allowBabies) && capturable.ChoreConsumer.IsChoreEqualOrAboveCurrentChorePriority<FixedCaptureStates>() && capturable.Navigator.GetNavigationCost(capture_cell) != -1 && capture_point.def.isAmountStoredOverCapacity(capture_point, capturable);
		}

		public void FindFixedCapturable()
		{
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(this.captureCell);
			if (cavityForCell == null)
			{
				this.ResetCapturePoint();
				return;
			}
			if (!this.targetCapturable.IsNullOrStopped() && !this.isCurrentlyCapturingCreature && !FixedCapturePoint.Instance.CanCapturableBeCapturedAtCapturePoint(this.targetCapturable, this, cavityForCell, this.captureCell))
			{
				this.ResetCapturePoint();
			}
			if (this.targetCapturable.IsNullOrStopped())
			{
				foreach (object obj in Components.FixedCapturableMonitors)
				{
					FixedCapturableMonitor.Instance instance = (FixedCapturableMonitor.Instance)obj;
					if (FixedCapturePoint.Instance.CanCapturableBeCapturedAtCapturePoint(instance, this, cavityForCell, this.captureCell))
					{
						this.targetCapturable = instance;
						if (!this.targetCapturable.IsNullOrStopped())
						{
							this.targetCapturable.targetCapturePoint = this;
							break;
						}
						break;
					}
				}
			}
		}

		public void UpdateCaptureCell(CellOffset offset)
		{
			this.captureCell = Grid.OffsetCell(Grid.PosToCell(base.transform.GetPosition()), offset);
		}

		public void ResetCapturePoint()
		{
			base.Trigger(643180843, null);
			if (!this.targetCapturable.IsNullOrStopped())
			{
				this.targetCapturable.targetCapturePoint = null;
				this.targetCapturable.Trigger(1034952693, null);
				this.targetCapturable = null;
			}
		}

		public bool isCurrentlyCapturingCreature;

		public BaggableCritterCapacityTracker critterCapactiy;

		private int captureCell;

		private Operational operationComp;

		private LogicPorts logicPorts;
	}

	public class AutoWrangleCapture : KMonoBehaviour, ICheckboxControl
	{
		protected override void OnSpawn()
		{
			base.OnSpawn();
			this.fcp = this.GetSMI<FixedCapturePoint.Instance>();
		}

		string ICheckboxControl.CheckboxTitleKey
		{
			get
			{
				return UI.UISIDESCREENS.CAPTURE_POINT_SIDE_SCREEN.TITLE.key.String;
			}
		}

		string ICheckboxControl.CheckboxLabel
		{
			get
			{
				return UI.UISIDESCREENS.CAPTURE_POINT_SIDE_SCREEN.AUTOWRANGLE;
			}
		}

		string ICheckboxControl.CheckboxTooltip
		{
			get
			{
				return UI.UISIDESCREENS.CAPTURE_POINT_SIDE_SCREEN.AUTOWRANGLE_TOOLTIP;
			}
		}

		bool ICheckboxControl.GetCheckboxValue()
		{
			return this.fcp.GetAutomated();
		}

		void ICheckboxControl.SetCheckboxValue(bool value)
		{
			this.fcp.SetAutomated(value);
		}

		private FixedCapturePoint.Instance fcp;
	}
}
