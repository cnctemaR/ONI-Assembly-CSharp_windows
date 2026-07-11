using System;
using System.Collections.Generic;
using System.Diagnostics;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
[DebuggerDisplay("{name}")]
public class Battery : KMonoBehaviour, IEnergyConsumer, IEffectDescriptor, IEnergyProducer
{
	public float WattsUsed { get; private set; }

	public float WattsNeededWhenActive
	{
		get
		{
			return 0f;
		}
	}

	public float PercentFull
	{
		get
		{
			return this.joulesAvailable / this.capacity;
		}
	}

	public float PreviousPercentFull
	{
		get
		{
			return this.PreviousJoulesAvailable / this.capacity;
		}
	}

	public float JoulesAvailable
	{
		get
		{
			return this.joulesAvailable;
		}
	}

	public float Capacity
	{
		get
		{
			return this.capacity;
		}
	}

	public float ChargeCapacity { get; private set; }

	public int PowerSortOrder
	{
		get
		{
			return this.powerSortOrder;
		}
	}

	public string Name
	{
		get
		{
			return base.GetComponent<KSelectable>().GetName();
		}
	}

	public int PowerCell { get; private set; }

	public bool IsConnected
	{
		get
		{
			GameObject gameObject = Grid.Objects[this.PowerCell, 24];
			return gameObject != null;
		}
	}

	public bool IsPowered
	{
		get
		{
			return this.connectionStatus == CircuitManager.ConnectionStatus.Powered;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.Batteries.Add(this);
		Building component = base.GetComponent<Building>();
		this.PowerCell = component.GetPowerInputCell();
		base.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		this.OnOperationalChanged(null);
		bool flag = base.GetComponent<PowerTransformer>();
		this.meter = ((!flag) ? new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, new string[] { "meter_target", "meter_fill", "meter_frame", "meter_OL" }) : null);
		Game.Instance.circuitManager.Connect(this);
		Game.Instance.emergySim.AddBattery(this);
	}

	private void OnOperationalChanged(object data)
	{
		if (this.operational.IsOperational)
		{
			Game.Instance.circuitManager.Connect(this);
			base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.JoulesAvailable, this);
		}
		else
		{
			Game.Instance.circuitManager.Disconnect(this);
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.JoulesAvailable, false);
		}
	}

	protected override void OnCleanUp()
	{
		Game.Instance.emergySim.RemoveBattery(this);
		Game.Instance.circuitManager.Disconnect(this);
		Components.Batteries.Remove(this);
		base.OnCleanUp();
	}

	public virtual void EnergySim200ms(float dt)
	{
		this.dt = dt;
		this.joulesConsumed = 0f;
		this.WattsUsed = 0f;
		this.ChargeCapacity = this.chargeWattage * dt;
		if (this.meter != null)
		{
			float percentFull = this.PercentFull;
			this.meter.SetPositionPercent(percentFull);
		}
		this.UpdateSounds();
		this.PreviousJoulesAvailable = this.JoulesAvailable;
		this.ConsumeEnergy(this.joulesLostPerSecond * dt, true);
	}

	private void UpdateSounds()
	{
		float previousPercentFull = this.PreviousPercentFull;
		float percentFull = this.PercentFull;
		if (percentFull == 0f && previousPercentFull != 0f)
		{
			base.GetComponent<LoopingSounds>().PlayEvent(GameSoundEvents.BatteryDischarged);
		}
		if (percentFull > 0.999f && previousPercentFull <= 0.999f)
		{
			base.GetComponent<LoopingSounds>().PlayEvent(GameSoundEvents.BatteryFull);
		}
		if (percentFull < 0.25f && previousPercentFull >= 0.25f)
		{
			base.GetComponent<LoopingSounds>().PlayEvent(GameSoundEvents.BatteryWarning);
		}
	}

	public void SetConnectionStatus(CircuitManager.ConnectionStatus status)
	{
		this.connectionStatus = status;
		if (status != CircuitManager.ConnectionStatus.NotConnected)
		{
			this.operational.SetActive(this.operational.IsOperational && this.JoulesAvailable > 0f, false);
		}
		else
		{
			this.operational.SetActive(false, false);
		}
	}

	public void AddEnergy(float joules)
	{
		this.joulesAvailable = Mathf.Min(this.capacity, this.JoulesAvailable + joules);
		this.joulesConsumed += joules;
		this.ChargeCapacity -= joules;
		this.WattsUsed = this.joulesConsumed / this.dt;
	}

	public void ConsumeEnergy(float joules, bool report = false)
	{
		if (report)
		{
			float num = Mathf.Min(this.JoulesAvailable, joules);
			ReportManager.Instance.ReportValue(ReportManager.ReportType.EnergyWasted, -num, StringFormatter.Replace(BUILDINGS.PREFABS.BATTERY.CHARGE_LOSS, "{Battery}", this.GetProperName()), null);
		}
		this.joulesAvailable = Mathf.Max(0f, this.JoulesAvailable - joules);
	}

	public void ConsumeEnergy(float joules)
	{
		this.ConsumeEnergy(joules, false);
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		return new List<Descriptor>
		{
			new Descriptor(UI.BUILDINGEFFECTS.REQUIRESPOWERGENERATOR, UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESPOWERGENERATOR, Descriptor.DescriptorType.Requirement, false),
			new Descriptor(string.Format(UI.BUILDINGEFFECTS.BATTERYCAPACITY, GameUtil.GetFormattedJoules(this.capacity, string.Empty, GameUtil.TimeSlice.None)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.BATTERYCAPACITY, GameUtil.GetFormattedJoules(this.capacity, string.Empty, GameUtil.TimeSlice.None)), Descriptor.DescriptorType.Effect, false),
			new Descriptor(string.Format(UI.BUILDINGEFFECTS.BATTERYLEAK, GameUtil.GetFormattedJoules(this.joulesLostPerSecond, "F1", GameUtil.TimeSlice.PerCycle)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.BATTERYLEAK, GameUtil.GetFormattedJoules(this.joulesLostPerSecond, "F1", GameUtil.TimeSlice.PerCycle)), Descriptor.DescriptorType.Effect, false)
		};
	}

	[SerializeField]
	public float capacity;

	[SerializeField]
	public float chargeWattage = float.PositiveInfinity;

	[Serialize]
	private float joulesAvailable;

	[MyCmpGet]
	protected Operational operational;

	[MyCmpGet]
	public PowerTransformer powerTransformer;

	private MeterController meter;

	public float joulesLostPerSecond;

	[SerializeField]
	public int powerSortOrder;

	private float PreviousJoulesAvailable;

	private CircuitManager.ConnectionStatus connectionStatus;

	private float dt;

	private float joulesConsumed;
}
