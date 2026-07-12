using System;
using Database;
using UnityEngine;

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
		ValueTuple<bool, float> visibilityOf = smi.def.skyVisibilityInfo.GetVisibilityOf(smi.gameObject);
		bool item = visibilityOf.Item1;
		float item2 = visibilityOf.Item2;
		smi.Internal_SetPercentClearSky(item2);
		KSelectable component = smi.GetComponent<KSelectable>();
		component.ToggleStatusItem(Db.Get().BuildingStatusItems.SkyVisNone, !item, smi);
		component.ToggleStatusItem(Db.Get().BuildingStatusItems.SkyVisLimited, item && item2 < 1f, smi);
		if (hasSkyVisibility == item)
		{
			return;
		}
		smi.TriggerVisibilityChange();
	}

	public class Def : StateMachine.BaseDef
	{
		public Operational.State AffectedOperationalState;

		public SkyVisibilityInfo skyVisibilityInfo;
	}

	public new class Instance : GameStateMachine<SkyVisibilityMonitor, SkyVisibilityMonitor.Instance, IStateMachineTarget, SkyVisibilityMonitor.Def>.GameInstance, BuildingStatusItems.ISkyVisInfo
	{
		public bool HasSkyVisibility
		{
			get
			{
				return this.PercentClearSky > 0f && !Mathf.Approximately(0f, this.PercentClearSky);
			}
		}

		public float PercentClearSky
		{
			get
			{
				return this.percentClearSky01;
			}
		}

		public void Internal_SetPercentClearSky(float percent01)
		{
			this.percentClearSky01 = percent01;
		}

		float BuildingStatusItems.ISkyVisInfo.GetPercentVisible01()
		{
			return this.percentClearSky01;
		}

		public Instance(IStateMachineTarget master, SkyVisibilityMonitor.Def def)
			: base(master, def)
		{
			if (def.AffectedOperationalState != Operational.State.None)
			{
				this.skyVisibilityFlag = new Operational.Flag("sky visibility", Operational.Flag.GetFlagType(def.AffectedOperationalState));
			}
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

		private float percentClearSky01;

		public global::System.Action SkyVisibilityChanged;

		private StatusItem visibilityStatusItem;

		private Operational.Flag skyVisibilityFlag;
	}
}
