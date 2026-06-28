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

	public void Connect(UtilityNetworkLink bridge)
	{
		this.bridges.Add(bridge);
		this.dirty = true;
	}

	public void Disconnect(UtilityNetworkLink bridge)
	{
		this.bridges.Remove(bridge);
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
					batteries = new List<Battery>(),
					inputTransformers = new List<Battery>(),
					outputTransformers = new List<Generator>(),
					bridges = new List<UtilityNetworkLink>()
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
			circuitInfo.inputTransformers.Clear();
			circuitInfo.outputTransformers.Clear();
			circuitInfo.bridges.Clear();
			circuitInfo.minBatteryPercentFull = 1f;
			this.circuitInfo[i] = circuitInfo;
		}
		this.consumersShadow.AddRange(this.consumers);
		foreach (IEnergyConsumer energyConsumer in this.consumersShadow)
		{
			int powerCell = energyConsumer.PowerCell;
			ushort circuitID = this.GetCircuitID(powerCell);
			if (circuitID != 65535)
			{
				Battery battery = energyConsumer as Battery;
				if (battery != null)
				{
					CircuitManager.CircuitInfo circuitInfo2 = this.circuitInfo[(int)circuitID];
					PowerTransformer component = battery.GetComponent<PowerTransformer>();
					if (component != null)
					{
						circuitInfo2.inputTransformers.Add(battery);
					}
					else
					{
						circuitInfo2.batteries.Add(battery);
						circuitInfo2.minBatteryPercentFull = Mathf.Min(this.circuitInfo[(int)circuitID].minBatteryPercentFull, battery.PercentFull);
					}
					this.circuitInfo[(int)circuitID] = circuitInfo2;
				}
				else
				{
					this.circuitInfo[(int)circuitID].consumers.Add(energyConsumer);
				}
			}
		}
		this.consumersShadow.Clear();
		foreach (Generator generator in this.generators)
		{
			int powerCell2 = generator.PowerCell;
			ushort circuitID2 = this.GetCircuitID(powerCell2);
			if (circuitID2 != 65535)
			{
				if (generator.GetType() == typeof(PowerTransformer))
				{
					this.circuitInfo[(int)circuitID2].outputTransformers.Add(generator);
				}
				else
				{
					this.circuitInfo[(int)circuitID2].generators.Add(generator);
				}
			}
		}
		foreach (UtilityNetworkLink utilityNetworkLink in this.bridges)
		{
			int num = Grid.PosToCell(utilityNetworkLink.transform.position);
			ushort circuitID3 = this.GetCircuitID(num);
			if (circuitID3 != 65535)
			{
				this.circuitInfo[(int)circuitID3].bridges.Add(utilityNetworkLink);
			}
		}
		this.dirty = false;
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
			List<Generator> outputTransformers = circuitInfo.outputTransformers;
			batteries.Sort((Battery a, Battery b) => a.JoulesAvailable.CompareTo(b.JoulesAvailable));
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
			if (!flag)
			{
				foreach (Generator generator2 in outputTransformers)
				{
					if (generator2.JoulesAvailable > 0f)
					{
						flag = true;
					}
				}
			}
			float num = 1f;
			foreach (Battery battery in batteries)
			{
				if (battery.JoulesAvailable > 0f)
				{
					flag = true;
				}
				num = Mathf.Min(num, battery.PercentFull);
			}
			foreach (Battery battery2 in circuitInfo.inputTransformers)
			{
				num = Mathf.Min(num, battery2.PercentFull);
			}
			circuitInfo.minBatteryPercentFull = num;
			float num2 = 0f;
			if (flag)
			{
				foreach (IEnergyConsumer energyConsumer in list2)
				{
					float num3 = energyConsumer.WattsUsed * dt;
					if (num3 > 0f)
					{
						num2 += energyConsumer.WattsUsed;
						bool flag3 = false;
						foreach (Generator generator3 in this.activeGenerators)
						{
							num3 = this.PowerFromGenerator(num3, generator3);
							if (num3 <= 0f)
							{
								flag3 = true;
								break;
							}
						}
						if (!flag3)
						{
							foreach (Generator generator4 in outputTransformers)
							{
								num3 = this.PowerFromGenerator(num3, generator4);
								if (num3 <= 0f)
								{
									flag3 = true;
									break;
								}
							}
						}
						if (!flag3)
						{
							num3 = this.PowerFromBatteries(num3, batteries);
							flag3 = Mathf.Abs(num3) <= 0.01f;
						}
						energyConsumer.SetConnectionStatus((!flag3) ? CircuitManager.ConnectionStatus.Unpowered : CircuitManager.ConnectionStatus.Powered);
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
			this.CheckCircuitOverloaded(dt, i, num2);
			this.circuitInfo[i] = circuitInfo;
		}
		for (int j = 0; j < this.circuitInfo.Count; j++)
		{
			CircuitManager.CircuitInfo circuitInfo2 = this.circuitInfo[j];
			circuitInfo2.batteries.Sort((Battery a, Battery b) => (a.Capacity - a.JoulesAvailable).CompareTo(b.Capacity - b.JoulesAvailable));
			circuitInfo2.inputTransformers.Sort((Battery a, Battery b) => (a.Capacity - a.JoulesAvailable).CompareTo(b.Capacity - b.JoulesAvailable));
			this.ChargeBatteries(j, circuitInfo2.generators, circuitInfo2.inputTransformers);
			this.ChargeBatteries(j, circuitInfo2.outputTransformers, circuitInfo2.inputTransformers);
			this.ChargeBatteries(j, circuitInfo2.generators, circuitInfo2.batteries);
			this.ChargeBatteries(j, circuitInfo2.outputTransformers, circuitInfo2.batteries);
			circuitInfo2.minBatteryPercentFull = 1f;
			foreach (Battery battery3 in circuitInfo2.batteries)
			{
				float percentFull = battery3.PercentFull;
				if (percentFull < circuitInfo2.minBatteryPercentFull)
				{
					circuitInfo2.minBatteryPercentFull = percentFull;
				}
			}
			foreach (Battery battery4 in circuitInfo2.inputTransformers)
			{
				float percentFull2 = battery4.PercentFull;
				if (percentFull2 < circuitInfo2.minBatteryPercentFull)
				{
					circuitInfo2.minBatteryPercentFull = percentFull2;
				}
			}
			this.circuitInfo[j] = circuitInfo2;
		}
		for (int k = 0; k < this.circuitInfo.Count; k++)
		{
			CircuitManager.CircuitInfo circuitInfo3 = this.circuitInfo[k];
			foreach (Battery battery5 in circuitInfo3.inputTransformers)
			{
				this.ChargeTransformer(battery5, circuitInfo3.batteries);
			}
		}
		for (int l = 0; l < this.circuitInfo.Count; l++)
		{
			CircuitManager.CircuitInfo circuitInfo4 = this.circuitInfo[l];
			bool flag4 = circuitInfo4.generators.Count + circuitInfo4.consumers.Count + circuitInfo4.outputTransformers.Count > 0;
			this.UpdateBatteryConnectionStatus(circuitInfo4.batteries, flag4, l);
			this.UpdateBatteryConnectionStatus(circuitInfo4.inputTransformers, flag4, l);
			foreach (Generator generator5 in circuitInfo4.generators)
			{
				ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyWasted, generator5.JoulesAvailable, null);
			}
		}
	}

	private float PowerFromBatteries(float joules_needed, IList<Battery> batteries)
	{
		int num;
		float batteryJoulesAvailable = this.GetBatteryJoulesAvailable(batteries, out num);
		float num2 = batteryJoulesAvailable * (float)num;
		float num3 = Mathf.Min(num2, joules_needed);
		joules_needed -= num3;
		float num4 = num3 / (float)num;
		for (int i = batteries.Count - num; i < batteries.Count; i++)
		{
			Battery battery = batteries[i];
			battery.ConsumeEnergy(num4);
		}
		return joules_needed;
	}

	private float PowerFromGenerator(float joules_needed, Generator g)
	{
		float num = Mathf.Min(g.JoulesAvailable, joules_needed);
		joules_needed -= num;
		g.ApplyDeltaJoules(-num, false);
		ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyCreated, -num, null);
		return joules_needed;
	}

	private float GetBatteryChargeCapacity(Generator g, IList<Battery> batteries, out int num_to_charge)
	{
		float num = 0f;
		num_to_charge = 0;
		for (int i = 0; i < batteries.Count; i++)
		{
			Battery battery = batteries[i];
			if (battery.gameObject != g.gameObject && battery.Capacity > battery.JoulesAvailable)
			{
				num = battery.Capacity - battery.JoulesAvailable;
				num_to_charge = batteries.Count - i;
				break;
			}
		}
		return num;
	}

	private void ChargeBatteries(int circuit_id, IList<Generator> generators, IList<Battery> batteries)
	{
		if (batteries.Count == 0)
		{
			return;
		}
		foreach (Generator generator in generators)
		{
			for (bool flag = true; flag && generator.JoulesAvailable >= 1f; flag = this.ChargeBattery(generator, batteries))
			{
			}
		}
	}

	private bool ChargeBattery(Generator g, IList<Battery> batteries)
	{
		int num;
		float batteryChargeCapacity = this.GetBatteryChargeCapacity(g, batteries, out num);
		if (batteryChargeCapacity <= 0f)
		{
			return false;
		}
		float num2 = Mathf.Min(batteryChargeCapacity, g.JoulesAvailable / (float)num);
		g.ApplyDeltaJoules(-num2 * (float)num, false);
		for (int i = batteries.Count - num; i < batteries.Count; i++)
		{
			Battery battery = batteries[i];
			if (g.gameObject != battery.gameObject)
			{
				battery.AddEnergy(num2);
			}
		}
		return true;
	}

	private void UpdateBatteryConnectionStatus(IList<Battery> batteries, bool is_connected_to_something_useful, int circuit_id)
	{
		foreach (Battery battery in batteries)
		{
			if (!(battery == null))
			{
				if (battery.GetComponent<PowerTransformer>() == null)
				{
					battery.SetConnectionStatus((!is_connected_to_something_useful) ? CircuitManager.ConnectionStatus.NotConnected : CircuitManager.ConnectionStatus.Powered);
				}
				else
				{
					ushort circuitID = this.GetCircuitID(battery.PowerCell);
					if ((int)circuitID == circuit_id)
					{
						battery.SetConnectionStatus((!is_connected_to_something_useful) ? CircuitManager.ConnectionStatus.NotConnected : CircuitManager.ConnectionStatus.Powered);
					}
				}
			}
		}
	}

	private void ChargeTransformer(Battery transformer, IList<Battery> batteries)
	{
		if (batteries.Count <= 0)
		{
			return;
		}
		float num = transformer.Capacity - transformer.JoulesAvailable;
		if (num <= 0f)
		{
			return;
		}
		float num2 = 0f;
		int num3 = 0;
		for (int i = 0; i < batteries.Count; i++)
		{
			Battery battery = batteries[i];
			if (battery.JoulesAvailable > 0f)
			{
				num3 = batteries.Count - i;
				num2 = battery.JoulesAvailable;
				break;
			}
		}
		if (num3 <= 0)
		{
			return;
		}
		float num4 = Mathf.Min(num2, num / (float)num3);
		transformer.AddEnergy(num4 * (float)num3);
		for (int j = batteries.Count - num3; j < batteries.Count; j++)
		{
			Battery battery2 = batteries[j];
			battery2.ConsumeEnergy(num4);
		}
	}

	private void CheckCircuitOverloaded(float dt, int id, float watts_used)
	{
		UtilityNetworkManager<ElectricalUtilityNetwork, Wire> electricalConduitSystem = Game.Instance.electricalConduitSystem;
		UtilityNetwork networkByID = electricalConduitSystem.GetNetworkByID(id);
		if (networkByID != null)
		{
			ElectricalUtilityNetwork electricalUtilityNetwork = (ElectricalUtilityNetwork)networkByID;
			electricalUtilityNetwork.UpdateOverloadTime(dt, watts_used, this.circuitInfo[id].bridges);
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
		foreach (Battery battery in this.circuitInfo[(int)circuitID].inputTransformers)
		{
			num += battery.WattsUsed;
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
		foreach (Battery battery in this.circuitInfo[(int)circuitID].inputTransformers)
		{
			num += battery.WattsNeededWhenActive;
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
		foreach (Generator generator in list)
		{
			if (!(generator == null))
			{
				if (generator.GetComponent<Operational>().IsActive)
				{
					num += generator.WattageRating;
				}
			}
		}
		return num;
	}

	public float GetPotentialWattsGeneratedByCircuit(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return -1f;
		}
		float num = 0f;
		List<Generator> list = this.circuitInfo[(int)circuitID].generators;
		foreach (Generator generator in list)
		{
			num += generator.WattageRating;
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

	public ReadOnlyCollection<Battery> GetTransformersOnCircuit(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return null;
		}
		return this.circuitInfo[(int)circuitID].inputTransformers.AsReadOnly();
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
		return circuitID != ushort.MaxValue && this.circuitInfo[(int)circuitID].batteries.Count + this.circuitInfo[(int)circuitID].inputTransformers.Count > 0;
	}

	public bool HasGenerators(ushort circuitID)
	{
		return circuitID != ushort.MaxValue && this.circuitInfo[(int)circuitID].generators.Count + this.circuitInfo[(int)circuitID].outputTransformers.Count > 0;
	}

	public bool HasGenerators()
	{
		return this.generators.Count > 0;
	}

	public bool HasConsumers(ushort circuitID)
	{
		return circuitID != ushort.MaxValue && this.circuitInfo[(int)circuitID].consumers.Count > 0;
	}

	public float GetMaxSafeWattageForCircuit(ushort circuitID)
	{
		if (circuitID == 65535)
		{
			return 0f;
		}
		ElectricalUtilityNetwork electricalUtilityNetwork = Game.Instance.electricalConduitSystem.GetNetworkByID((int)circuitID) as ElectricalUtilityNetwork;
		return (electricalUtilityNetwork == null) ? 0f : electricalUtilityNetwork.GetMaxSafeWattage();
	}

	public const ushort INVALID_ID = 65535;

	private const int SimUpdateSortKey = 1000;

	private bool dirty = true;

	private HashSet<Generator> generators = new HashSet<Generator>();

	private HashSet<IEnergyConsumer> consumers = new HashSet<IEnergyConsumer>();

	private HashSet<UtilityNetworkLink> bridges = new HashSet<UtilityNetworkLink>();

	private List<CircuitManager.CircuitInfo> circuitInfo = new List<CircuitManager.CircuitInfo>();

	private List<IEnergyConsumer> consumersShadow = new List<IEnergyConsumer>();

	private List<Generator> activeGenerators = new List<Generator>();

	private struct CircuitInfo
	{
		public List<Generator> generators;

		public List<IEnergyConsumer> consumers;

		public List<Battery> batteries;

		public List<Battery> inputTransformers;

		public List<Generator> outputTransformers;

		public List<UtilityNetworkLink> bridges;

		public float minBatteryPercentFull;
	}

	public enum ConnectionStatus
	{
		NotConnected,
		Unpowered,
		Powered,
		OverDraw
	}
}
