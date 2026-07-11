using System;
using KSerialization;
using STRINGS;
using UnityEngine;

public class FixedCapturePoint : GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.operational;
		base.serializable = true;
		this.unoperational.TagTransition(GameTags.Operational, this.operational, false);
		this.operational.DefaultState(this.operational.manual).TagTransition(GameTags.Operational, this.unoperational, true);
		this.operational.manual.ParamTransition<bool>(this.automated, this.operational.automated, (FixedCapturePoint.Instance smi, bool p) => p);
		this.operational.automated.ParamTransition<bool>(this.automated, this.operational.manual, (FixedCapturePoint.Instance smi, bool p) => !p).ToggleChore((FixedCapturePoint.Instance smi) => smi.CreateChore(), this.unoperational, this.unoperational).Update("FindFixedCapturable", delegate(FixedCapturePoint.Instance smi, float dt)
		{
			smi.FindFixedCapturable();
		}, UpdateRate.SIM_1000ms, false);
	}

	private StateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.BoolParameter automated;

	public GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.State unoperational;

	public FixedCapturePoint.OperationalState operational;

	public class Def : StateMachine.BaseDef
	{
		public Func<GameObject, FixedCapturePoint.Instance, bool> isCreatureEligibleToBeCapturedCb;

		public Action<GameObject> onCaptureCompleteCb;

		public HashedString ranchedPreAnim = "idle_loop";

		public HashedString ranchedLoopAnim = "idle_loop";

		public HashedString ranchedPstAnim = "idle_loop";

		public int interactLoopCount = 1;

		public bool synchronizeBuilding;

		public Func<FixedCapturePoint.Instance, int> getTargetCapturePoint = (FixedCapturePoint.Instance smi) => Grid.PosToCell(smi);
	}

	public class OperationalState : GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.State
	{
		public GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.State manual;

		public GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.State automated;
	}

	[SerializationConfig(MemberSerialization.OptIn)]
	public new class Instance : GameStateMachine<FixedCapturePoint, FixedCapturePoint.Instance, IStateMachineTarget, FixedCapturePoint.Def>.GameInstance, ICheckboxControl
	{
		public Instance(IStateMachineTarget master, FixedCapturePoint.Def def)
			: base(master, def)
		{
		}

		public FixedCapturableMonitor.Instance targetCapturable { get; private set; }

		public bool shouldCreatureGoGetCaptured { get; private set; }

		public Chore CreateChore()
		{
			return new FixedCaptureChore(base.GetComponent<KPrefabID>());
		}

		public bool IsCreatureAvailableForFixedCapture()
		{
			if (!this.targetCapturable.IsNullOrStopped())
			{
				int num = Grid.PosToCell(base.transform.GetPosition());
				CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
				return FixedCapturePoint.Instance.CanCapturableBeCapturedAtCapturePoint(this.targetCapturable, this, cavityForCell, num);
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
			if (capture_point.def.isCreatureEligibleToBeCapturedCb != null && !capture_point.def.isCreatureEligibleToBeCapturedCb(capturable.gameObject, capture_point))
			{
				return false;
			}
			if (!capturable.GetComponent<ChoreConsumer>().IsChoreEqualOrAboveCurrentChorePriority<FixedCaptureStates>())
			{
				return false;
			}
			int num = Grid.PosToCell(capturable.transform.GetPosition());
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
			if (cavityForCell == null || cavityForCell != capture_cavity_info)
			{
				return false;
			}
			int navigationCost = capturable.GetComponent<Navigator>().GetNavigationCost(capture_cell);
			if (navigationCost == PathProber.InvalidCost)
			{
				return false;
			}
			TreeFilterable component = capture_point.GetComponent<TreeFilterable>();
			IUserControlledCapacity component2 = capture_point.GetComponent<IUserControlledCapacity>();
			return !component.ContainsTag(capturable.GetComponent<KPrefabID>().PrefabTag) || component2.AmountStored > component2.UserMaxCapacity;
		}

		public void FindFixedCapturable()
		{
			int num = Grid.PosToCell(base.transform.GetPosition());
			CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(num);
			if (cavityForCell == null)
			{
				this.TriggerFixedCapturePointNoLongerAvailable();
				return;
			}
			if (!this.targetCapturable.IsNullOrStopped() && !FixedCapturePoint.Instance.CanCapturableBeCapturedAtCapturePoint(this.targetCapturable, this, cavityForCell, num))
			{
				this.TriggerFixedCapturePointNoLongerAvailable();
			}
			if (this.targetCapturable.IsNullOrStopped())
			{
				FixedCapturePoint.Instance.CapturableIterator capturableIterator = new FixedCapturePoint.Instance.CapturableIterator(this, cavityForCell, num);
				GameScenePartitioner.Instance.Iterate<FixedCapturePoint.Instance.CapturableIterator>(cavityForCell.minX, cavityForCell.minY, cavityForCell.maxX - cavityForCell.minX + 1, cavityForCell.maxY - cavityForCell.minY + 1, GameScenePartitioner.Instance.collisionLayer, ref capturableIterator);
				capturableIterator.Cleanup();
				this.targetCapturable = capturableIterator.result;
				if (!this.targetCapturable.IsNullOrStopped())
				{
					this.targetCapturable.targetCapturePoint = this;
				}
			}
		}

		public void TriggerFixedCapturePointNoLongerAvailable()
		{
			if (!this.targetCapturable.IsNullOrStopped())
			{
				this.targetCapturable.targetCapturePoint = null;
				this.targetCapturable.Trigger(1034952693, null);
				this.targetCapturable = null;
			}
		}

		public void CaptureCreature()
		{
			if (!this.targetCapturable.IsNullOrStopped())
			{
				this.targetCapturable.Trigger(643180843, null);
				int num = Grid.PosToCell(base.transform.GetPosition());
				if (Grid.Solid[num])
				{
					int num2 = Grid.CellAbove(num);
					if (Grid.IsValidCell(num2) && !Grid.Solid[num2])
					{
						num = num2;
					}
				}
				Baggable component = this.targetCapturable.GetComponent<Baggable>();
				component.SetWrangled();
				component.transform.SetPosition(Grid.CellToPosCCC(num, Grid.SceneLayer.Ore));
				this.targetCapturable = null;
			}
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
			return base.sm.automated.Get(this);
		}

		void ICheckboxControl.SetCheckboxValue(bool value)
		{
			base.sm.automated.Set(value, this);
		}

		private struct CapturableIterator : GameScenePartitioner.Iterator
		{
			public CapturableIterator(FixedCapturePoint.Instance capture_point, CavityInfo capture_cavity_info, int capture_cell)
			{
				this.capturePoint = capture_point;
				this.captureCavityInfo = capture_cavity_info;
				this.captureCell = capture_cell;
				this.result = null;
			}

			public FixedCapturableMonitor.Instance result { get; private set; }

			public void Iterate(object target_obj)
			{
				KMonoBehaviour kmonoBehaviour = target_obj as KMonoBehaviour;
				if (kmonoBehaviour == null)
				{
					return;
				}
				FixedCapturableMonitor.Instance smi = kmonoBehaviour.GetSMI<FixedCapturableMonitor.Instance>();
				if (smi == null)
				{
					return;
				}
				if (FixedCapturePoint.Instance.CanCapturableBeCapturedAtCapturePoint(smi, this.capturePoint, this.captureCavityInfo, this.captureCell))
				{
					this.result = smi;
				}
			}

			public void Cleanup()
			{
			}

			private CavityInfo captureCavityInfo;

			private int captureCell;

			private FixedCapturePoint.Instance capturePoint;
		}
	}
}
