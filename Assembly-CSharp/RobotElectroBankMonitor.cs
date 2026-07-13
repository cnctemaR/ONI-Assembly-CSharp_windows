using System;
using System.Collections.Generic;
using Klei.AI;
using UnityEngine;

public class RobotElectroBankMonitor : GameStateMachine<RobotElectroBankMonitor, RobotElectroBankMonitor.Instance, IStateMachineTarget, RobotElectroBankMonitor.Def>
{
	public override void InitializeStates(out StateMachine.BaseState default_state)
	{
		default_state = this.powered;
		this.root.Enter(delegate(RobotElectroBankMonitor.Instance smi)
		{
			smi.ElectroBankStorageChange(null);
		}).TagTransition(GameTags.Dead, this.deceased, false).TagTransition(GameTags.Creatures.Die, this.deceased, false);
		this.powered.DefaultState(this.powered.highBattery).ParamTransition<bool>(this.hasElectrobank, this.powerdown, GameStateMachine<RobotElectroBankMonitor, RobotElectroBankMonitor.Instance, IStateMachineTarget, RobotElectroBankMonitor.Def>.IsFalse).Update(delegate(RobotElectroBankMonitor.Instance smi, float dt)
		{
			RobotElectroBankMonitor.ConsumePower(smi, dt);
		}, UpdateRate.SIM_200ms, false);
		this.powered.highBattery.Transition(this.powered.lowBattery, GameStateMachine<RobotElectroBankMonitor, RobotElectroBankMonitor.Instance, IStateMachineTarget, RobotElectroBankMonitor.Def>.Not(new StateMachine<RobotElectroBankMonitor, RobotElectroBankMonitor.Instance, IStateMachineTarget, RobotElectroBankMonitor.Def>.Transition.ConditionCallback(RobotElectroBankMonitor.ChargeDecent)), UpdateRate.SIM_200ms).Enter(delegate(RobotElectroBankMonitor.Instance smi)
		{
			this.UpdateBatteryMeter(smi, RobotElectroBankMonitor.BATTER_FULL_SYMBOL);
		});
		this.powered.lowBattery.Enter(delegate(RobotElectroBankMonitor.Instance smi)
		{
			RobotElectroBankMonitor.RequestBattery(smi);
			this.UpdateBatteryMeter(smi, RobotElectroBankMonitor.BATTER_LOW_SYMBOL);
		}).Transition(this.powered.highBattery, new StateMachine<RobotElectroBankMonitor, RobotElectroBankMonitor.Instance, IStateMachineTarget, RobotElectroBankMonitor.Def>.Transition.ConditionCallback(RobotElectroBankMonitor.ChargeDecent), UpdateRate.SIM_200ms).ToggleStatusItem((RobotElectroBankMonitor.Instance smi) => Db.Get().RobotStatusItems.LowBatteryNoCharge, null);
		this.powerdown.Enter(delegate(RobotElectroBankMonitor.Instance smi)
		{
			RobotElectroBankMonitor.RequestBattery(smi);
		}).ToggleBehaviour(GameTags.Robots.Behaviours.NoElectroBank, (RobotElectroBankMonitor.Instance smi) => true, delegate(RobotElectroBankMonitor.Instance smi)
		{
			smi.GoTo(this.powered);
		});
		this.deceased.DoNothing();
	}

	private void UpdateBatteryMeter(RobotElectroBankMonitor.Instance smi, HashedString symbol)
	{
		smi.UpdateBatteryState(symbol);
	}

	public static bool ChargeDecent(RobotElectroBankMonitor.Instance smi)
	{
		float num = 0f;
		foreach (GameObject gameObject in smi.electroBankStorage.items)
		{
			if (!(gameObject == null))
			{
				num += gameObject.GetComponent<Electrobank>().Charge;
			}
		}
		return num >= smi.def.lowBatteryWarningPercent * 120000f;
	}

	public static void ConsumePower(RobotElectroBankMonitor.Instance smi, float dt)
	{
		if (smi.electrobank == null)
		{
			RobotElectroBankMonitor.RequestBattery(smi);
			return;
		}
		float num = Mathf.Min(dt * Mathf.Abs(smi.bankAmount.GetDelta()), smi.electrobank.Charge);
		smi.electrobank.RemovePower(num, true);
		if (smi.electrobank != null)
		{
			smi.bankAmount.value = smi.electrobank.Charge;
		}
	}

	public static void RequestBattery(RobotElectroBankMonitor.Instance smi)
	{
		if (smi.fetchBatteryChore.IsPaused)
		{
			smi.fetchBatteryChore.Pause(smi.electrobank != null && RobotElectroBankMonitor.ChargeDecent(smi), "FlydoBattery");
		}
	}

	public static readonly HashedString BATTER_SYMBOL = "meter_target";

	public static readonly HashedString BATTER_FULL_SYMBOL = "battery_full";

	public static readonly HashedString BATTER_LOW_SYMBOL = "battery_low";

	public static readonly HashedString BATTER_DEAD_SYMBOL = "battery_dead";

	public RobotElectroBankMonitor.PoweredState powered;

	public GameStateMachine<RobotElectroBankMonitor, RobotElectroBankMonitor.Instance, IStateMachineTarget, RobotElectroBankMonitor.Def>.State deceased;

	public GameStateMachine<RobotElectroBankMonitor, RobotElectroBankMonitor.Instance, IStateMachineTarget, RobotElectroBankMonitor.Def>.State powerdown;

	public StateMachine<RobotElectroBankMonitor, RobotElectroBankMonitor.Instance, IStateMachineTarget, RobotElectroBankMonitor.Def>.BoolParameter hasElectrobank;

	public class Def : StateMachine.BaseDef
	{
		public float lowBatteryWarningPercent;
	}

	public class PoweredState : GameStateMachine<RobotElectroBankMonitor, RobotElectroBankMonitor.Instance, IStateMachineTarget, RobotElectroBankMonitor.Def>.State
	{
		public GameStateMachine<RobotElectroBankMonitor, RobotElectroBankMonitor.Instance, IStateMachineTarget, RobotElectroBankMonitor.Def>.State highBattery;

		public GameStateMachine<RobotElectroBankMonitor, RobotElectroBankMonitor.Instance, IStateMachineTarget, RobotElectroBankMonitor.Def>.State lowBattery;
	}

	public new class Instance : GameStateMachine<RobotElectroBankMonitor, RobotElectroBankMonitor.Instance, IStateMachineTarget, RobotElectroBankMonitor.Def>.GameInstance
	{
		public Instance(IStateMachineTarget master, RobotElectroBankMonitor.Def def)
			: base(master, def)
		{
			this.fetchBatteryChore = base.GetComponent<ManualDeliveryKG>();
			foreach (Storage storage in master.gameObject.GetComponents<Storage>())
			{
				if (storage.storageID == GameTags.ChargedPortableBattery)
				{
					this.electroBankStorage = storage;
					break;
				}
			}
			foreach (GameObject gameObject in Assets.GetPrefabsWithTag(GameTags.ChargedPortableBattery))
			{
				KPrefabID component = gameObject.GetComponent<KPrefabID>();
				this.batteryTags.Add(component.PrefabTag);
			}
			this.bankAmount = Db.Get().Amounts.InternalElectroBank.Lookup(master.gameObject);
			this.electroBankStorage.Subscribe(-1697596308, new Action<object>(this.ElectroBankStorageChange));
			this.ElectroBankStorageChange(null);
			TreeFilterable component2 = base.GetComponent<TreeFilterable>();
			component2.OnFilterChanged = (Action<HashSet<Tag>>)Delegate.Combine(component2.OnFilterChanged, new Action<HashSet<Tag>>(this.OnFilterChanged));
		}

		public void ElectroBankStorageChange(object data = null)
		{
			GameObject gameObject = (GameObject)data;
			if (gameObject != null)
			{
				Pickupable component = gameObject.GetComponent<Pickupable>();
				if (component.storage != null && component.storage.storageID == GameTags.ChargedPortableBattery)
				{
					if (this.electroBankStorage.Count > 0 && this.electroBankStorage.items[0] != null)
					{
						this.electrobank = this.electroBankStorage.items[0].GetComponent<Electrobank>();
						this.bankAmount.value = this.electrobank.Charge;
					}
					else
					{
						this.electrobank = null;
					}
				}
				else if (this.electroBankStorage.Count <= 0)
				{
					this.electrobank = null;
					this.bankAmount.value = 0f;
					this.DropDischargedElectroBank(gameObject);
				}
				this.fetchBatteryChore.Pause(this.electrobank != null && RobotElectroBankMonitor.ChargeDecent(this), "Robot has sufficienct electrobank");
				base.sm.hasElectrobank.Set(this.electrobank != null, this, false);
				return;
			}
			if (this.electrobank == null)
			{
				if (this.electroBankStorage.Count > 0 && this.electroBankStorage.items[0] != null)
				{
					this.electrobank = this.electroBankStorage.items[0].GetComponent<Electrobank>();
					this.bankAmount.value = this.electrobank.Charge;
				}
				else
				{
					this.electrobank = null;
					this.bankAmount.value = 0f;
				}
				this.fetchBatteryChore.Pause(this.electrobank != null && RobotElectroBankMonitor.ChargeDecent(this), "Robot has sufficienct electrobank");
				base.sm.hasElectrobank.Set(this.electrobank != null, this, false);
			}
		}

		private void DropDischargedElectroBank(GameObject go)
		{
			Electrobank component = go.GetComponent<Electrobank>();
			if (component != null && component.HasTag(GameTags.ChargedPortableBattery) && !component.IsFullyCharged)
			{
				component.RemovePower(component.Charge, true);
			}
		}

		public void UpdateBatteryState(HashedString newState)
		{
			if (this.currentSymbolSwap.IsValid)
			{
				this.symbolOverrideController.RemoveSymbolOverride(this.currentSymbolSwap, 0);
			}
			KAnim.Build.Symbol symbol = this.animController.AnimFiles[0].GetData().build.GetSymbol(newState);
			this.symbolOverrideController.AddSymbolOverride(RobotElectroBankMonitor.BATTER_SYMBOL, symbol, 0);
			this.currentSymbolSwap = newState;
		}

		private void OnFilterChanged(HashSet<Tag> allowed_tags)
		{
			if (this.fetchBatteryChore != null)
			{
				List<Tag> list = new List<Tag>();
				foreach (Tag tag in this.batteryTags)
				{
					if (!allowed_tags.Contains(tag))
					{
						list.Add(tag);
					}
				}
				this.fetchBatteryChore.ForbiddenTags = list.ToArray();
			}
		}

		public Storage electroBankStorage;

		public Electrobank electrobank;

		public ManualDeliveryKG fetchBatteryChore;

		public AmountInstance bankAmount;

		[MyCmpReq]
		private SymbolOverrideController symbolOverrideController;

		[MyCmpReq]
		private KBatchedAnimController animController;

		private HashedString currentSymbolSwap;

		private HashSet<Tag> batteryTags = new HashSet<Tag>();
	}
}
