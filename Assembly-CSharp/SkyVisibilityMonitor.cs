using System;

public class SkyVisibilityMonitor : GameStateMachine<SkyVisibilityMonitor, SkyVisibilityMonitor.Instance, IStateMachineTarget, SkyVisibilityMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.root;
		this.root.Update(new Action<SkyVisibilityMonitor.Instance, float>(SkyVisibilityMonitor.CheckSkyVisibility), UpdateRate.SIM_1000ms, false);
	}

	public static void CheckSkyVisibility(SkyVisibilityMonitor.Instance smi, float dt)
	{
		bool hasSkyVisibility = smi.HasSkyVisibility;
		Grid.IsRangeExposedToSunlight(Grid.OffsetCell(Grid.PosToCell(smi), smi.def.ScanOriginOffset), smi.def.ScanRadius, smi.def.ScanShape, out smi.NumClearCells, 1);
		if (hasSkyVisibility == smi.HasSkyVisibility)
		{
			return;
		}
		smi.TriggerVisibilityChange();
	}

	public class Def : StateMachine.BaseDef
	{
		public Operational.State AffectedOperationalState;

		public string StatusItemId = "SPACE_VISIBILITY_NONE";

		public int ScanRadius = 15;

		public CellOffset ScanShape = new CellOffset(1, 0);

		public CellOffset ScanOriginOffset = new CellOffset(0, 0);
	}

	public new class Instance : GameStateMachine<SkyVisibilityMonitor, SkyVisibilityMonitor.Instance, IStateMachineTarget, SkyVisibilityMonitor.Def>.GameInstance
	{
		public bool HasSkyVisibility
		{
			get
			{
				return this.PercentClearSky > 0f;
			}
		}

		public float PercentClearSky
		{
			get
			{
				return (float)this.NumClearCells * (float)(base.def.ScanRadius + 1);
			}
		}

		public Instance(IStateMachineTarget master, SkyVisibilityMonitor.Def def)
			: base(master, def)
		{
			if (string.IsNullOrEmpty(def.StatusItemId))
			{
				return;
			}
			if (def.AffectedOperationalState != Operational.State.None)
			{
				this.skyVisibilityFlag = new Operational.Flag("sky visibility", Operational.Flag.GetFlagType(def.AffectedOperationalState));
			}
			this.visibilityStatusItem = new StatusItem(def.StatusItemId, "BUILDING", "status_item_no_sky", StatusItem.IconType.Custom, NotificationType.BadMinor, false, OverlayModes.None.ID, true, 129022, new Func<string, object, string>(SkyVisibilityMonitor.Instance.GetStatusItemString));
		}

		public override void StartSM()
		{
			base.StartSM();
			SkyVisibilityMonitor.CheckSkyVisibility(this, 0f);
			this.TriggerVisibilityChange();
		}

		public void TriggerVisibilityChange()
		{
			if (this.visibilityStatusItem != null)
			{
				base.smi.GetComponent<KSelectable>().ToggleStatusItem(this.visibilityStatusItem, !this.HasSkyVisibility, this);
			}
			if (base.def.AffectedOperationalState != Operational.State.None)
			{
				base.smi.GetComponent<Operational>().SetFlag(this.skyVisibilityFlag, this.HasSkyVisibility);
			}
			if (this.SkyVisibilityChanged != null)
			{
				this.SkyVisibilityChanged();
			}
		}

		private static string GetStatusItemString(string src_str, object data)
		{
			SkyVisibilityMonitor.Instance instance = (SkyVisibilityMonitor.Instance)data;
			return src_str.Replace("{VISIBILITY}", GameUtil.GetFormattedPercent(instance.PercentClearSky * 100f, GameUtil.TimeSlice.None)).Replace("{RADIUS}", instance.def.ScanRadius.ToString());
		}

		public int NumClearCells;

		public global::System.Action SkyVisibilityChanged;

		private StatusItem visibilityStatusItem;

		private Operational.Flag skyVisibilityFlag;
	}
}
