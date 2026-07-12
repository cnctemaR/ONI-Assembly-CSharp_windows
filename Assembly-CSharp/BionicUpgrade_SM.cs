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

	public static bool IsInBatterySaveMode(BionicUpgrade_SM<SMType, StateMachineInstanceType>.BaseInstance smi)
	{
		return smi.IsInBatterySavingMode;
	}

	public GameStateMachine<SMType, StateMachineInstanceType, IStateMachineTarget, BionicUpgrade_SM<SMType, StateMachineInstanceType>.Def>.State Active;

	public GameStateMachine<SMType, StateMachineInstanceType, IStateMachineTarget, BionicUpgrade_SM<SMType, StateMachineInstanceType>.Def>.State Inactive;

	public class Def : StateMachine.BaseDef
	{
		public Def(string upgradeID)
		{
			this.UpgradeID = upgradeID;
		}

		public string UpgradeID;

		public Func<StateMachine.Instance, StateMachine.Instance>[] StateMachinesWhenActive;
	}

	public abstract class BaseInstance : GameStateMachine<SMType, StateMachineInstanceType, IStateMachineTarget, BionicUpgrade_SM<SMType, StateMachineInstanceType>.Def>.GameInstance, BionicUpgradeComponent.IWattageController
	{
		public bool IsInBatterySavingMode
		{
			get
			{
				return this.batteryMonitor != null && this.batteryMonitor.IsBatterySaveModeActive;
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
			this.RegisterMonitorToUpgradeComponent();
			base.Subscribe(-426516281, new Action<object>(this.OnBatterySavingModeChanged));
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

		protected virtual void OnEnteringBatterySavingMode()
		{
		}

		protected virtual void OnExitingBatterySavingMode()
		{
		}

		private void OnBatterySavingModeChanged(object o)
		{
			if ((bool)o)
			{
				this.OnEnteringBatterySavingMode();
				return;
			}
			this.OnExitingBatterySavingMode();
		}

		protected override void OnCleanUp()
		{
			this.UnregisterMonitorToUpgradeComponent();
			base.Unsubscribe(-426516281, new Action<object>(this.OnBatterySavingModeChanged));
			base.OnCleanUp();
		}

		protected BionicBatteryMonitor.Instance batteryMonitor;

		protected BionicUpgradeComponent upgradeComponent;
	}
}
