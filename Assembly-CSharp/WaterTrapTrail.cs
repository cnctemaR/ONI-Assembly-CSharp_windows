using System;

public class WaterTrapTrail : GameStateMachine<WaterTrapTrail, WaterTrapTrail.Instance, IStateMachineTarget, WaterTrapTrail.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.retracted;
		base.serializable = StateMachine.SerializeType.Never;
		this.retracted.EventHandler(GameHashes.TrapArmWorkPST, delegate(WaterTrapTrail.Instance smi)
		{
			WaterTrapTrail.RefreshDepthAvailable(smi, 0f);
		}).EventHandlerTransition(GameHashes.TagsChanged, this.loose, new Func<WaterTrapTrail.Instance, object, bool>(WaterTrapTrail.ShouldBeVisible)).Enter(delegate(WaterTrapTrail.Instance smi)
		{
			WaterTrapTrail.RefreshDepthAvailable(smi, 0f);
		});
		this.loose.EventHandlerTransition(GameHashes.TagsChanged, this.retracted, new Func<WaterTrapTrail.Instance, object, bool>(WaterTrapTrail.OnTagsChangedWhenOnLooseState)).EventHandler(GameHashes.TrapCaptureCompleted, delegate(WaterTrapTrail.Instance smi)
		{
			WaterTrapTrail.RefreshDepthAvailable(smi, 0f);
		}).Enter(delegate(WaterTrapTrail.Instance smi)
		{
			WaterTrapTrail.RefreshDepthAvailable(smi, 0f);
		});
	}

	public static bool OnTagsChangedWhenOnLooseState(WaterTrapTrail.Instance smi, object tagOBJ)
	{
		ReusableTrap.Instance smi2 = smi.gameObject.GetSMI<ReusableTrap.Instance>();
		if (smi2 != null)
		{
			smi2.CAPTURING_SYMBOL_NAME = WaterTrapTrail.CAPTURING_SYMBOL_OVERRIDE_NAME + smi.sm.depthAvailable.Get(smi).ToString();
		}
		return WaterTrapTrail.ShouldBeInvisible(smi, tagOBJ);
	}

	public static bool ShouldBeInvisible(WaterTrapTrail.Instance smi, object tagOBJ)
	{
		return !WaterTrapTrail.ShouldBeVisible(smi, tagOBJ);
	}

	public static bool ShouldBeVisible(WaterTrapTrail.Instance smi, object tagOBJ)
	{
		ReusableTrap.Instance smi2 = smi.gameObject.GetSMI<ReusableTrap.Instance>();
		bool isOperational = smi.IsOperational;
		bool flag = smi.HasTag(GameTags.TrapArmed);
		bool flag2 = smi2 != null && smi2.IsInsideState(smi2.sm.operational.capture) && !smi2.IsInsideState(smi2.sm.operational.capture.idle) && !smi2.IsInsideState(smi2.sm.operational.capture.release);
		bool flag3 = smi2 != null && smi2.IsInsideState(smi2.sm.operational.unarmed) && smi2.GetWorkable().WorkInPstAnimation;
		return isOperational && (flag || flag2 || flag3);
	}

	public static void RefreshDepthAvailable(WaterTrapTrail.Instance smi, float dt)
	{
		bool flag = WaterTrapTrail.ShouldBeVisible(smi, null);
		int num = Grid.PosToCell(smi);
		int num2 = (flag ? WaterTrapGuide.GetDepthAvailable(num, smi.gameObject) : 0);
		int num3 = 4;
		if (num2 != smi.sm.depthAvailable.Get(smi))
		{
			KAnimControllerBase component = smi.GetComponent<KAnimControllerBase>();
			for (int i = 1; i <= num3; i++)
			{
				component.SetSymbolVisiblity("pipe" + i.ToString(), i <= num2);
				component.SetSymbolVisiblity(WaterTrapTrail.CAPTURING_SYMBOL_OVERRIDE_NAME + i.ToString(), i == num2);
			}
			int num4 = Grid.OffsetCell(num, 0, -num2);
			smi.ChangeTrapCellPosition(num4);
			WaterTrapGuide.OccupyArea(smi.gameObject, num2);
			smi.sm.depthAvailable.Set(num2, smi, false);
		}
		smi.SetRangeVisualizerOffset(new Vector2I(0, -num2));
		smi.SetRangeVisualizerVisibility(flag);
	}

	private static string CAPTURING_SYMBOL_OVERRIDE_NAME = "creatureSymbol";

	public GameStateMachine<WaterTrapTrail, WaterTrapTrail.Instance, IStateMachineTarget, WaterTrapTrail.Def>.State retracted;

	public GameStateMachine<WaterTrapTrail, WaterTrapTrail.Instance, IStateMachineTarget, WaterTrapTrail.Def>.State loose;

	private StateMachine<WaterTrapTrail, WaterTrapTrail.Instance, IStateMachineTarget, WaterTrapTrail.Def>.IntParameter depthAvailable = new StateMachine<WaterTrapTrail, WaterTrapTrail.Instance, IStateMachineTarget, WaterTrapTrail.Def>.IntParameter(-1);

	public class Def : StateMachine.BaseDef
	{
	}

	public new class Instance : GameStateMachine<WaterTrapTrail, WaterTrapTrail.Instance, IStateMachineTarget, WaterTrapTrail.Def>.GameInstance
	{
		public bool IsOperational
		{
			get
			{
				return this.operational.IsOperational;
			}
		}

		public Lure.Instance lureSMI
		{
			get
			{
				if (this._lureSMI == null)
				{
					this._lureSMI = base.gameObject.GetSMI<Lure.Instance>();
				}
				return this._lureSMI;
			}
		}

		public Instance(IStateMachineTarget master, WaterTrapTrail.Def def)
			: base(master, def)
		{
		}

		public override void StartSM()
		{
			base.StartSM();
			this.RegisterListenersToCellChanges();
		}

		private void RegisterListenersToCellChanges()
		{
			int widthInCells = base.GetComponent<BuildingComplete>().Def.WidthInCells;
			CellOffset[] array = new CellOffset[widthInCells * 4];
			for (int i = 0; i < 4; i++)
			{
				int num = -(i + 1);
				for (int j = 0; j < widthInCells; j++)
				{
					array[i * widthInCells + j] = new CellOffset(j, num);
				}
			}
			Extents extents = new Extents(Grid.PosToCell(base.transform.GetPosition()), array);
			this.partitionerEntry_solids = GameScenePartitioner.Instance.Add("WaterTrapTrail", base.gameObject, extents, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnLowerCellChanged));
			this.partitionerEntry_buildings = GameScenePartitioner.Instance.Add("WaterTrapTrail", base.gameObject, extents, GameScenePartitioner.Instance.objectLayers[1], new Action<object>(this.OnLowerCellChanged));
		}

		private void UnregisterListenersToCellChanges()
		{
			GameScenePartitioner.Instance.Free(ref this.partitionerEntry_solids);
			GameScenePartitioner.Instance.Free(ref this.partitionerEntry_buildings);
		}

		private void OnLowerCellChanged(object o)
		{
			WaterTrapTrail.RefreshDepthAvailable(base.smi, 0f);
		}

		protected override void OnCleanUp()
		{
			this.UnregisterListenersToCellChanges();
			base.OnCleanUp();
		}

		public void SetRangeVisualizerVisibility(bool visible)
		{
			this.rangeVisualizer.RangeMax.x = (visible ? 0 : (-1));
		}

		public void SetRangeVisualizerOffset(Vector2I offset)
		{
			this.rangeVisualizer.OriginOffset = offset;
		}

		public void ChangeTrapCellPosition(int cell)
		{
			if (this.lureSMI != null)
			{
				this.lureSMI.ChangeLureCellPosition(cell);
			}
			base.gameObject.GetComponent<TrapTrigger>().SetTriggerCell(cell);
		}

		[MyCmpGet]
		private Operational operational;

		[MyCmpGet]
		private RangeVisualizer rangeVisualizer;

		private HandleVector<int>.Handle partitionerEntry_buildings;

		private HandleVector<int>.Handle partitionerEntry_solids;

		private Lure.Instance _lureSMI;
	}
}
