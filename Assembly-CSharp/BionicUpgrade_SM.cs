using System;

public class BionicUpgrade_SM<SMType, StateMachineInstanceType> : GameStateMachine<SMType, StateMachineInstanceType, IStateMachineTarget, BionicUpgrade_SM<SMType, StateMachineInstanceType>.Def> where SMType : GameStateMachine<SMType, StateMachineInstanceType, IStateMachineTarget, BionicUpgrade_SM<SMType, StateMachineInstanceType>.Def> where StateMachineInstanceType : BionicUpgrade_SM<SMType, StateMachineInstanceType>.BaseInstance
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		base.serializable = StateMachine.SerializeType.ParamsOnly;
		default_state = this.Inactive;
	}

	public static bool IsOnline(BionicUpgrade_SM<SMType, StateMachineInstanceType>.BaseInstance smi)
	{
		return smi.IsOnline;
	}

	public static bool IsInBedTimeChore(BionicUpgrade_SM<SMType, StateMachineInstanceType>.BaseInstance smi)
	{
		return smi.IsInBedTimeChore;
	}

	public GameStateMachine<SMType, StateMachineInstanceType, IStateMachineTarget, BionicUpgrade_SM<SMType, StateMachineInstanceType>.Def>.State Active;

	public GameStateMachine<SMType, StateMachineInstanceType, IStateMachineTarget, BionicUpgrade_SM<SMType, StateMachineInstanceType>.Def>.State Inactive;

	public class Def : StateMachine.BaseDef
	{
		public Def(string upgradeID)
		{
			this.UpgradeID = upgradeID;
		}

		public virtual string GetDescription()
		{
			return "";
		}

		public string UpgradeID;

		public Func<StateMachine.Instance, StateMachine.Instance>[] StateMachinesWhenActive;
	}

	public abstract class BaseInstance : GameStateMachine<SMType, StateMachineInstanceType, IStateMachineTarget, BionicUpgrade_SM<SMType, StateMachineInstanceType>.Def>.GameInstance, BionicUpgradeComponent.IWattageController
	{
		public bool IsInBedTimeChore
		{
			get
			{
				return this.bedTimeMonitor != null && this.bedTimeMonitor.IsBedTimeChoreRunning;
			}
		}

		public bool IsOnline
		{
			get
			{
				return this.batteryMonitor != null && this.batteryMonitor.IsOnline;
			}
		}

		public BionicUpgradeComponentConfig.BionicUpgradeData Data
		{
			get
			{
				return BionicUpgradeComponentConfig.UpgradesData[base.def.UpgradeID];
			}
		}

		public BaseInstance(IStateMachineTarget master, BionicUpgrade_SM<SMType, StateMachineInstanceType>.Def def)
			: base(master, def)
		{
			this.batteryMonitor = base.gameObject.GetSMI<BionicBatteryMonitor.Instance>();
			this.bedTimeMonitor = base.gameObject.GetSMI<BionicBedTimeMonitor.Instance>();
			this.RegisterMonitorToUpgradeComponent();
		}

		private void RegisterMonitorToUpgradeComponent()
		{
			foreach (BionicUpgradesMonitor.UpgradeComponentSlot upgradeComponentSlot in base.gameObject.GetSMI<BionicUpgradesMonitor.Instance>().upgradeComponentSlots)
			{
				if (upgradeComponentSlot.HasUpgradeInstalled)
				{
					BionicUpgradeComponent installedUpgradeComponent = upgradeComponentSlot.installedUpgradeComponent;
					if (installedUpgradeComponent != null && !installedUpgradeComponent.HasWattageController)
					{
						this.upgradeComponent = installedUpgradeComponent;
						installedUpgradeComponent.SetWattageController(this);
						return;
					}
				}
			}
		}

		private void UnregisterMonitorToUpgradeComponent()
		{
			if (this.upgradeComponent != null)
			{
				this.upgradeComponent.SetWattageController(null);
			}
		}

		public abstract float GetCurrentWattageCost();

		public abstract string GetCurrentWattageCostName();

		protected override void OnCleanUp()
		{
			this.UnregisterMonitorToUpgradeComponent();
			base.OnCleanUp();
		}

		protected BionicBedTimeMonitor.Instance bedTimeMonitor;

		protected BionicBatteryMonitor.Instance batteryMonitor;

		protected BionicUpgradeComponent upgradeComponent;
	}
}
