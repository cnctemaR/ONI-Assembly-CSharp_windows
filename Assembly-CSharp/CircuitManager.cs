using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class CircuitManager
{
	public void Connect(Generator generator)
	{
		if (Game.IsQuitting())
		{
			return;
		}
		this.generators.Add(generator);
		this.dirty = true;
	}

	public void Disconnect(Generator generator)
	{
		if (Game.IsQuitting())
		{
			return;
		}
		this.generators.Remove(generator);
		this.dirty = true;
	}

	public void Connect(IEnergyConsumer consumer)
	{
		if (Game.IsQuitting())
		{
			return;
		}
		this.consumers.Add(consumer);
		this.dirty = true;
	}

	public void Disconnect(IEnergyConsumer consumer)
	{
		if (Game.IsQuitting())
		{
			return;
		}
		this.consumers.Remove(consumer);
		this.dirty = true;
	}

	public float GetPowerDraw(ushort circuitID, Generator generator)
	{
		float num = 0f;
		if ((int)circuitID < this.circuitInfo.Count)
		{
			CircuitManager.CircuitInfo circuitInfo = this.circuitInfo[(int)circuitID];
			this.circuitInfo[(int)circuitID] = circuitInfo;
			this.circuitInfo[(int)circuitID] = circuitInfo;
		}
		return num;
	}

	public ushort GetCircuitID(int cell)
	{
		UtilityNetwork networkForCell = Game.Instance.electricalConduitSystem.GetNetworkForCell(cell);
		return (ushort)((networkForCell != null) ? networkForCell.id : 65535);
	}

	public void Update()
	{
		UtilityNetworkManager<ElectricalUtilityNetwork, Wire> electricalConduitSystem = Game.Instance.electricalConduitSystem;
		if (electricalConduitSystem.IsDirty || this.dirty)
		{
			electricalConduitSystem.Update();
			while (this.circuitInfo.Count < electricalConduitSystem.networks.Count)
			{
				this.circuitInfo.Add(new CircuitManager.CircuitInfo
				{
					generators = new List<Generator>(),
					consumers = new List<IEnergyConsumer>(),
					batteries = new List<Battery>()
				});
			}
			this.Rebuild();
		}
	}

	public void Rebuild()
	{
		for (int i = 0; i < this.circuitInfo.Count; i++)
		{
			CircuitManager.CircuitInfo circuitInfo = this.circuitInfo[i];
			circuitInfo.generators.Clear();
			circuitInfo.consumers.Clear();
			circuitInfo.batteries.Clear();
			circuitInfo.minBatteryPercentFull = 1f;
			this.circuitInfo[i] = circuitInfo;
		}
		foreach (IEnergyConsumer energyConsumer in this.consumers)
		{
			int powerCell = energyConsumer.PowerCell;
			ushort circuitID = this.GetCircuitID(powerCell);
			if (circuitID != 65535)
			{
				Battery battery = energyConsumer as Battery;
				if (battery != null)
				{
					CircuitManager.CircuitInfo circuitInfo2 = this.circuitInfo[(int)circuitID];
					circuitInfo2.batteries.Add(battery);
					circuitInfo2.minBatteryPercentFull = Mathf.Min(this.circuitInfo[(int)circuitID].minBatteryPercentFull, battery.PercentFull);
					this.circuitInfo[(int)circuitID] = circuitInfo2;
				}
				else
				{
					this.circuitInfo[(int)circuitID].consumers.Add(energyConsumer);
				}
			}
			else
			{
				energyConsumer.SetConnectionStatus(CircuitManager.ConnectionStatus.NotConnected);
			}
		}
		foreach (Generator generator in this.generators)
		{
			int powerCell2 = generator.PowerCell;
			ushort circuitID2 = this.GetCircuitID(powerCell2);
			if (circuitID2 != 65535)
			{
				Battery component = generator.GetComponent<Battery>();
				if (component == null)
				{
					this.circuitInfo[(int)circuitID2].generators.Add(generator);
				}
			}
		}
	}

	private float GetBatteryJoulesAvailable(IList<Battery> batteries, out int num_powered)
	{
		float num = 0f;
		num_powered = 0;
		for (int i = 0; i < batteries.Count; i++)
		{
			if (batteries[i].JoulesAvailable > 0f)
			{
				num = batteries[i].JoulesAvailable;
				num_powered = batteries.Count - i;
				break;
			}
		}
		return num;
	}

	public void SimUpdateLast(float dt)
	{
		for (int i = 0; i < this.circuitInfo.Count; i++)
		{
			CircuitManager.CircuitInfo circuitInfo = this.circuitInfo[i];
			this.activeGenerators.Clear();
			List<Generator> list = circuitInfo.generators;
			List<IEnergyConsumer> list2 = circuitInfo.consumers;
			List<Battery> batteries = circuitInfo.batteries;
			bool flag = false;
			bool flag2 = list.Count > 0;
			foreach (Generator generator in list)
			{
				if (generator.JoulesAvailable > 0f)
				{
					flag = true;
					this.activeGenerators.Add(generator);
				}
			}
			this.activeGenerators.Sort((Generator a, Generator b) => a.JoulesAvailable.CompareTo(b.JoulesAvailable));
			batteries.Sort((Battery a, Battery b) => a.JoulesAvailable.CompareTo(b.JoulesAvailable));
			float num = 1f;
			foreach (Battery battery in batteries)
			{
				if (battery.JoulesAvailable > 0f)
				{
					flag = true;
				}
				num = Mathf.Min(num, battery.PercentFull);
			}
			circuitInfo.minBatteryPercentFull = num;
			if (flag)
			{
				foreach (IEnergyConsumer energyConsumer in list2)
				{
					float num2 = energyConsumer.WattsUsed * dt;
					if (num2 > 0f)
					{
						bool flag3 = false;
						foreach (Generator generator2 in this.activeGenerators)
						{
							float num3 = Mathf.Min(generator2.JoulesAvailable, num2);
							num2 -= num3;
							generator2.JoulesAvailable -= num3;
							ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyCreated, -num3, null);
							if (num2 <= 0f)
							{
								flag3 = true;
								break;
							}
						}
						if (!flag3)
						{
							int num4;
							float batteryJoulesAvailable = this.GetBatteryJoulesAvailable(batteries, out num4);
							float num5 = batteryJoulesAvailable * (float)num4;
							float num6 = Mathf.Min(num5, num2);
							num2 -= num6;
							float num7 = num6 / (float)num4;
							for (int j = batteries.Count - num4; j < batteries.Count; j++)
							{
								Battery battery2 = batteries[j];
								battery2.ConsumeEnergy(num7);
							}
							if (Mathf.Abs(num2) <= 0.01f)
							{
								flag3 = true;
							}
						}
						if (flag3)
						{
							energyConsumer.SetConnectionStatus(CircuitManager.ConnectionStatus.Powered);
						}
						else
						{
							energyConsumer.SetConnectionStatus(CircuitManager.ConnectionStatus.OverDraw);
						}
					}
					else
					{
						energyConsumer.SetConnectionStatus((!flag) ? CircuitManager.ConnectionStatus.Unpowered : CircuitManager.ConnectionStatus.Powered);
					}
				}
			}
			else if (flag2)
			{
				foreach (IEnergyConsumer energyConsumer2 in list2)
				{
					energyConsumer2.SetConnectionStatus(CircuitManager.ConnectionStatus.Unpowered);
				}
			}
			else
			{
				foreach (IEnergyConsumer energyConsumer3 in list2)
				{
					energyConsumer3.SetConnectionStatus(CircuitManager.ConnectionStatus.NotConnected);
				}
			}
			this.circuitInfo[i] = circuitInfo;
		}
		this.ChargeBatteries(dt);
		for (int k = 0; k < this.circuitInfo.Count; k++)
		{
			List<Generator> list3 = this.circuitInfo[k].generators;
			foreach (Generator generator3 in list3)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyWasted, generator3.JoulesAvailable, null);
			}
		}
	}

	private float GetBatteryChargeCapacity(IList<Battery> batteries, out int num_to_charge)
	{
		float num = 0f;
		num_to_charge = 0;
		for (int i = 0; i < batteries.Count; i++)
		{
			Battery battery = batteries[i];
			if (battery.Capacity > battery.JoulesAvailable)
			{
				num = battery.Capacity - battery.JoulesAvailable;
				num_to_charge = batteries.Count - i;
				break;
			}
		}
		return num;
	}

	private void ChargeBatteries(float dt)
	{
		for (int i = 0; i < this.circuitInfo.Count; i++)
		{
			CircuitManager.CircuitInfo circuitInfo = this.circuitInfo[i];
			List<Generator> list = circuitInfo.generators;
			List<Battery> batteries = circuitInfo.batteries;
			batteries.Sort((Battery a, Battery b) => (a.Capacity - a.JoulesAvailable).CompareTo(b.Capacity - b.JoulesAvailable));
			foreach (Generator generator in list)
			{
				if (generator.JoulesAvailable > 0f)
				{
					int num;
					float batteryChargeCapacity = this.GetBatteryChargeCapacity(batteries, out num);
					if (batteryChargeCapacity <= 0f)
					{
						break;
					}
					float num2 = Mathf.Min(batteryChargeCapacity, generator.JoulesAvailable / (float)num);
					generator.JoulesAvailable -= num2 * (float)num;
					for (int j = batteries.Count - num; j < batteries.Count; j++)
					{
						Battery battery = batteries[j];
						battery.AddEnergy(num2);
					}
				}
			}
			bool flag = circuitInfo.generators.Count > 0 || circuitInfo.consumers.Count > 0;
			foreach (Battery battery2 in batteries)
			{
				battery2.SetConnectionStatus((!flag) ? CircuitManager.ConnectionStatus.NotConnected : CircuitManager.ConnectionStatus.Powered);
			}
		}
	}

	public float GetWattsUsedByCircuit(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return -1f;
		}
		float num = 0f;
		foreach (IEnergyConsumer energyConsumer in this.circuitInfo[(int)circuitID].consumers)
		{
			num += energyConsumer.WattsUsed;
		}
		return num;
	}

	public float GetWattsNeededWhenActive(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return -1f;
		}
		float num = 0f;
		foreach (IEnergyConsumer energyConsumer in this.circuitInfo[(int)circuitID].consumers)
		{
			num += energyConsumer.WattsNeededWhenActive;
		}
		return num;
	}

	public float GetWattsGeneratedByCircuit(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return -1f;
		}
		float num = 0f;
		List<Generator> list = this.circuitInfo[(int)circuitID].generators;
		List<Battery> batteries = this.circuitInfo[(int)circuitID].batteries;
		if (list.Count == 0 && batteries.Count == 0)
		{
			return -1f;
		}
		foreach (Battery battery in batteries)
		{
			if (battery.PercentFull < battery.PreviousPercentFull)
			{
				num += battery.WattsNeededWhenActive;
			}
		}
		foreach (Generator generator in list)
		{
			ManualGenerator component = generator.GetComponent<ManualGenerator>();
			if (component == null)
			{
				if (generator.GetComponent<Operational>().IsActive)
				{
					num += generator.WattageRating;
				}
			}
			else if (generator.GetComponent<Operational>().IsOperational && component.IsPowered)
			{
				num += generator.WattageRating;
			}
		}
		return num;
	}

	public bool HasPowerSource(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return false;
		}
		List<Generator> list = this.circuitInfo[(int)circuitID].generators;
		List<Battery> batteries = this.circuitInfo[(int)circuitID].batteries;
		return (list.Count > 0 && list.Find(new Predicate<Generator>(this.FindActiveGenerator)) != null) || (batteries.Count > 0 && batteries.Find(new Predicate<Battery>(this.FindActiveBattery)) != null);
	}

	private bool FindActiveGenerator(Generator g)
	{
		Operational component = g.GetComponent<Operational>();
		ManualGenerator component2 = g.GetComponent<ManualGenerator>();
		if (component2 == null)
		{
			return component.IsActive;
		}
		return component.IsOperational && component2.IsPowered;
	}

	private bool FindActiveBattery(Battery b)
	{
		return b.GetComponent<Operational>().IsOperational && b.PercentFull > 0f;
	}

	public float GetJoulesAvailableOnCircuit(ushort circuitID)
	{
		int num;
		float batteryJoulesAvailable = this.GetBatteryJoulesAvailable(this.GetBatteriesOnCircuit(circuitID), out num);
		return batteryJoulesAvailable * (float)num;
	}

	public ReadOnlyCollection<Generator> GetGeneratorsOnCircuit(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return null;
		}
		return this.circuitInfo[(int)circuitID].generators.AsReadOnly();
	}

	public ReadOnlyCollection<IEnergyConsumer> GetConsumersOnCircuit(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return null;
		}
		return this.circuitInfo[(int)circuitID].consumers.AsReadOnly();
	}

	public ReadOnlyCollection<Battery> GetBatteriesOnCircuit(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return null;
		}
		return this.circuitInfo[(int)circuitID].batteries.AsReadOnly();
	}

	public float GetMinBatteryPercentFullOnCircuit(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return 0f;
		}
		return this.circuitInfo[(int)circuitID].minBatteryPercentFull;
	}

	public bool HasBatteries(ushort circuitID)
	{
		return circuitID != ushort.MaxValue && this.circuitInfo[(int)circuitID].batteries.Count > 0;
	}

	public bool HasGenerators()
	{
		return this.generators.Count > 0;
	}

	public bool HasConsumers(ushort circuitID)
	{
		return circuitID != ushort.MaxValue && this.circuitInfo[(int)circuitID].consumers.Count > 0;
	}

	public const ushort INVALID_ID = 65535;

	private const int SimUpdateSortKey = 1000;

	private bool dirty = true;

	private HashSet<Generator> generators = new HashSet<Generator>();

	private HashSet<IEnergyConsumer> consumers = new HashSet<IEnergyConsumer>();

	private List<CircuitManager.CircuitInfo> circuitInfo = new List<CircuitManager.CircuitInfo>();

	private List<Generator> activeGenerators = new List<Generator>();

	private struct CircuitInfo
	{
		public List<Generator> generators;

		public List<IEnergyConsumer> consumers;

		public List<Battery> batteries;

		public float minBatteryPercentFull;
	}

	public enum ConnectionStatus
	{
		NotConnected,
		Powered,
		OverDraw,
		Unpowered
	}
}
