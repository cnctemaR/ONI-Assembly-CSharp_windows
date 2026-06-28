using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using STRINGS;
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

	public void Connect(WireUtilityNetworkLink bridge)
	{
		this.bridges.Add(bridge);
		this.dirty = true;
	}

	public void Disconnect(WireUtilityNetworkLink bridge)
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

	public void Sim200msFirst(float dt)
	{
		this.Refresh(dt);
	}

	public void RenderEveryTick(float dt)
	{
		this.Refresh(dt);
	}

	private void Refresh(float dt)
	{
		UtilityNetworkManager<ElectricalUtilityNetwork, Wire> electricalConduitSystem = Game.Instance.electricalConduitSystem;
		if (electricalConduitSystem.IsDirty || this.dirty)
		{
			electricalConduitSystem.Update();
			IList<UtilityNetwork> networks = electricalConduitSystem.GetNetworks();
			while (this.circuitInfo.Count < networks.Count)
			{
				CircuitManager.CircuitInfo circuitInfo = new CircuitManager.CircuitInfo
				{
					generators = new List<Generator>(),
					consumers = new List<IEnergyConsumer>(),
					batteries = new List<Battery>(),
					inputTransformers = new List<Battery>(),
					outputTransformers = new List<Generator>()
				};
				circuitInfo.bridgeGroups = new List<WireUtilityNetworkLink>[5];
				for (int i = 0; i < circuitInfo.bridgeGroups.Length; i++)
				{
					circuitInfo.bridgeGroups[i] = new List<WireUtilityNetworkLink>();
				}
				this.circuitInfo.Add(circuitInfo);
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
			circuitInfo.minBatteryPercentFull = 1f;
			for (int j = 0; j < circuitInfo.bridgeGroups.Length; j++)
			{
				circuitInfo.bridgeGroups[j].Clear();
			}
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
					Operational component = battery.GetComponent<Operational>();
					if (component == null || component.IsOperational)
					{
						CircuitManager.CircuitInfo circuitInfo2 = this.circuitInfo[(int)circuitID];
						PowerTransformer powerTransformer = battery.powerTransformer;
						if (powerTransformer != null)
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
		foreach (WireUtilityNetworkLink wireUtilityNetworkLink in this.bridges)
		{
			int num;
			int num2;
			wireUtilityNetworkLink.GetCells(out num, out num2);
			ushort circuitID3 = this.GetCircuitID(num);
			if (circuitID3 != 65535)
			{
				Wire.WattageRating maxWattageRating = wireUtilityNetworkLink.GetMaxWattageRating();
				this.circuitInfo[(int)circuitID3].bridgeGroups[(int)maxWattageRating].Add(wireUtilityNetworkLink);
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

	public void Sim200msLast(float dt)
	{
		this.elapsedTime += dt;
		if (this.elapsedTime < 0.2f)
		{
			return;
		}
		this.elapsedTime -= 0.2f;
		for (int i = 0; i < this.circuitInfo.Count; i++)
		{
			CircuitManager.CircuitInfo circuitInfo = this.circuitInfo[i];
			circuitInfo.wattsUsed = 0f;
			this.activeGenerators.Clear();
			List<Generator> list = circuitInfo.generators;
			List<IEnergyConsumer> list2 = circuitInfo.consumers;
			List<Battery> batteries = circuitInfo.batteries;
			List<Generator> outputTransformers = circuitInfo.outputTransformers;
			batteries.Sort((Battery a, Battery b) => a.JoulesAvailable.CompareTo(b.JoulesAvailable));
			bool flag = false;
			bool flag2 = list.Count > 0;
			for (int j = 0; j < list.Count; j++)
			{
				Generator generator = list[j];
				if (generator.JoulesAvailable > 0f)
				{
					flag = true;
					this.activeGenerators.Add(generator);
				}
			}
			this.activeGenerators.Sort((Generator a, Generator b) => a.JoulesAvailable.CompareTo(b.JoulesAvailable));
			if (!flag)
			{
				for (int k = 0; k < outputTransformers.Count; k++)
				{
					Generator generator2 = outputTransformers[k];
					if (generator2.JoulesAvailable > 0f)
					{
						flag = true;
					}
				}
			}
			float num = 1f;
			for (int l = 0; l < batteries.Count; l++)
			{
				Battery battery = batteries[l];
				if (battery.JoulesAvailable > 0f)
				{
					flag = true;
				}
				num = Mathf.Min(num, battery.PercentFull);
			}
			for (int m = 0; m < circuitInfo.inputTransformers.Count; m++)
			{
				Battery battery2 = circuitInfo.inputTransformers[m];
				num = Mathf.Min(num, battery2.PercentFull);
			}
			circuitInfo.minBatteryPercentFull = num;
			if (flag)
			{
				for (int n = 0; n < list2.Count; n++)
				{
					IEnergyConsumer energyConsumer = list2[n];
					float num2 = energyConsumer.WattsUsed * 0.2f;
					if (num2 > 0f)
					{
						circuitInfo.wattsUsed += energyConsumer.WattsUsed;
						bool flag3 = false;
						for (int num3 = 0; num3 < this.activeGenerators.Count; num3++)
						{
							Generator generator3 = this.activeGenerators[num3];
							num2 = this.PowerFromGenerator(num2, generator3, energyConsumer);
							if (num2 <= 0f)
							{
								flag3 = true;
								break;
							}
						}
						if (!flag3)
						{
							for (int num4 = 0; num4 < outputTransformers.Count; num4++)
							{
								Generator generator4 = outputTransformers[num4];
								num2 = this.PowerFromGenerator(num2, generator4, energyConsumer);
								if (num2 <= 0f)
								{
									flag3 = true;
									break;
								}
							}
						}
						if (!flag3)
						{
							num2 = this.PowerFromBatteries(num2, batteries, energyConsumer);
							flag3 = Mathf.Abs(num2) <= 0.01f;
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
				for (int num5 = 0; num5 < list2.Count; num5++)
				{
					IEnergyConsumer energyConsumer2 = list2[num5];
					energyConsumer2.SetConnectionStatus(CircuitManager.ConnectionStatus.Unpowered);
				}
			}
			else
			{
				for (int num6 = 0; num6 < list2.Count; num6++)
				{
					IEnergyConsumer energyConsumer3 = list2[num6];
					energyConsumer3.SetConnectionStatus(CircuitManager.ConnectionStatus.NotConnected);
				}
			}
			this.circuitInfo[i] = circuitInfo;
		}
		for (int num7 = 0; num7 < this.circuitInfo.Count; num7++)
		{
			CircuitManager.CircuitInfo circuitInfo2 = this.circuitInfo[num7];
			circuitInfo2.batteries.Sort((Battery a, Battery b) => (a.Capacity - a.JoulesAvailable).CompareTo(b.Capacity - b.JoulesAvailable));
			circuitInfo2.inputTransformers.Sort((Battery a, Battery b) => (a.Capacity - a.JoulesAvailable).CompareTo(b.Capacity - b.JoulesAvailable));
			float num8 = 0f;
			this.ChargeBatteries(num7, circuitInfo2.generators, circuitInfo2.inputTransformers, ref num8);
			this.ChargeBatteries(num7, circuitInfo2.outputTransformers, circuitInfo2.inputTransformers, ref num8);
			float num9 = 0f;
			this.ChargeBatteries(num7, circuitInfo2.generators, circuitInfo2.batteries, ref num9);
			this.ChargeBatteries(num7, circuitInfo2.outputTransformers, circuitInfo2.batteries, ref num9);
			circuitInfo2.minBatteryPercentFull = 1f;
			for (int num10 = 0; num10 < circuitInfo2.batteries.Count; num10++)
			{
				Battery battery3 = circuitInfo2.batteries[num10];
				float percentFull = battery3.PercentFull;
				if (percentFull < circuitInfo2.minBatteryPercentFull)
				{
					circuitInfo2.minBatteryPercentFull = percentFull;
				}
			}
			for (int num11 = 0; num11 < circuitInfo2.inputTransformers.Count; num11++)
			{
				Battery battery4 = circuitInfo2.inputTransformers[num11];
				float percentFull2 = battery4.PercentFull;
				if (percentFull2 < circuitInfo2.minBatteryPercentFull)
				{
					circuitInfo2.minBatteryPercentFull = percentFull2;
				}
			}
			circuitInfo2.wattsUsed += num8 / 0.2f;
			this.circuitInfo[num7] = circuitInfo2;
		}
		for (int num12 = 0; num12 < this.circuitInfo.Count; num12++)
		{
			CircuitManager.CircuitInfo circuitInfo3 = this.circuitInfo[num12];
			float num13 = 0f;
			for (int num14 = 0; num14 < circuitInfo3.inputTransformers.Count; num14++)
			{
				Battery battery5 = circuitInfo3.inputTransformers[num14];
				this.ChargeTransformer(battery5, circuitInfo3.batteries, ref num13);
			}
			circuitInfo3.wattsUsed += num13 / 0.2f;
			this.circuitInfo[num12] = circuitInfo3;
		}
		for (int num15 = 0; num15 < this.circuitInfo.Count; num15++)
		{
			CircuitManager.CircuitInfo circuitInfo4 = this.circuitInfo[num15];
			bool flag4 = circuitInfo4.generators.Count + circuitInfo4.consumers.Count + circuitInfo4.outputTransformers.Count > 0;
			this.UpdateBatteryConnectionStatus(circuitInfo4.batteries, flag4, num15);
			this.UpdateBatteryConnectionStatus(circuitInfo4.inputTransformers, flag4, num15);
			this.circuitInfo[num15] = circuitInfo4;
			for (int num16 = 0; num16 < circuitInfo4.generators.Count; num16++)
			{
				Generator generator5 = circuitInfo4.generators[num16];
				ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyWasted, -generator5.JoulesAvailable, BUILDINGS.PREFABS.GENERATOR.OVERPRODUCTION.ToString().Replace("{Generator}", generator5.gameObject.GetProperName()), null);
			}
		}
		for (int num17 = 0; num17 < this.circuitInfo.Count; num17++)
		{
			this.CheckCircuitOverloaded(0.2f, num17, this.circuitInfo[num17].wattsUsed);
		}
	}

	private float PowerFromBatteries(float joules_needed, IList<Battery> batteries, IEnergyConsumer c)
	{
		int num;
		do
		{
			float batteryJoulesAvailable = this.GetBatteryJoulesAvailable(batteries, out num);
			float num2 = batteryJoulesAvailable * (float)num;
			float num3 = ((num2 >= joules_needed) ? joules_needed : num2);
			joules_needed -= num3;
			ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyCreated, -num3, c.Name, null);
			float num4 = num3 / (float)num;
			for (int i = batteries.Count - num; i < batteries.Count; i++)
			{
				Battery battery = batteries[i];
				battery.ConsumeEnergy(num4, false);
			}
		}
		while (joules_needed >= 0.01f && num > 0);
		return joules_needed;
	}

	private float PowerFromGenerator(float joules_needed, Generator g, IEnergyConsumer c)
	{
		float num = Mathf.Min(g.JoulesAvailable, joules_needed);
		joules_needed -= num;
		g.ApplyDeltaJoules(-num, false);
		ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyCreated, -num, c.Name, null);
		return joules_needed;
	}

	private float GetBatteryChargeCapacity(Generator g, IList<Battery> batteries, out int num_to_charge)
	{
		float num = 0f;
		num_to_charge = 0;
		for (int i = 0; i < batteries.Count; i++)
		{
			Battery battery = batteries[i];
			if (battery != null && g != null && battery.gameObject != g.gameObject && battery.Capacity > battery.JoulesAvailable)
			{
				num = battery.Capacity - battery.JoulesAvailable;
				num_to_charge = batteries.Count - i;
				break;
			}
		}
		return num;
	}

	private void ChargeBatteries(int circuit_id, IList<Generator> generators, IList<Battery> batteries, ref float joules_used)
	{
		if (batteries.Count == 0)
		{
			return;
		}
		foreach (Generator generator in generators)
		{
			for (bool flag = true; flag && generator.JoulesAvailable >= 1f; flag = this.ChargeBattery(generator, batteries, ref joules_used))
			{
			}
		}
	}

	private bool ChargeBattery(Generator g, IList<Battery> batteries, ref float joules_used)
	{
		int num;
		float batteryChargeCapacity = this.GetBatteryChargeCapacity(g, batteries, out num);
		if (batteryChargeCapacity <= 0f)
		{
			return false;
		}
		float num2 = Mathf.Min(batteryChargeCapacity, g.JoulesAvailable / (float)num);
		g.ApplyDeltaJoules(-num2 * (float)num, false);
		joules_used += num2 * (float)num;
		for (int i = batteries.Count - num; i < batteries.Count; i++)
		{
			Battery battery = batteries[i];
			if (g != null && battery != null && g.gameObject != battery.gameObject)
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
				if (battery.powerTransformer == null)
				{
					battery.SetConnectionStatus((!is_connected_to_something_useful) ? CircuitManager.ConnectionStatus.NotConnected : CircuitManager.ConnectionStatus.Powered);
				}
				else
				{
					ushort circuitID = this.GetCircuitID(battery.PowerCell);
					if ((int)circuitID == circuit_id)
					{
						battery.SetConnectionStatus((!is_connected_to_something_useful) ? CircuitManager.ConnectionStatus.Unpowered : CircuitManager.ConnectionStatus.Powered);
					}
				}
			}
		}
	}

	private void ChargeTransformer(Battery transformer, IList<Battery> batteries, ref float joules_used)
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
		joules_used += num4 * (float)num3;
		for (int j = batteries.Count - num3; j < batteries.Count; j++)
		{
			Battery battery2 = batteries[j];
			battery2.ConsumeEnergy(num4, false);
		}
	}

	private void CheckCircuitOverloaded(float dt, int id, float watts_used)
	{
		UtilityNetworkManager<ElectricalUtilityNetwork, Wire> electricalConduitSystem = Game.Instance.electricalConduitSystem;
		UtilityNetwork networkByID = electricalConduitSystem.GetNetworkByID(id);
		if (networkByID != null)
		{
			ElectricalUtilityNetwork electricalUtilityNetwork = (ElectricalUtilityNetwork)networkByID;
			electricalUtilityNetwork.UpdateOverloadTime(dt, watts_used, this.circuitInfo[id].bridgeGroups);
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

	private const float MIN_POWERED_THRESHOLD = 0.01f;

	private bool dirty = true;

	private HashSet<Generator> generators = new HashSet<Generator>();

	private HashSet<IEnergyConsumer> consumers = new HashSet<IEnergyConsumer>();

	private HashSet<WireUtilityNetworkLink> bridges = new HashSet<WireUtilityNetworkLink>();

	private float elapsedTime;

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

		public List<WireUtilityNetworkLink>[] bridgeGroups;

		public float minBatteryPercentFull;

		public float wattsUsed;
	}

	public enum ConnectionStatus
	{
		NotConnected,
		Unpowered,
		Powered
	}
}
