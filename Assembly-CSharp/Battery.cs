using System;
using System.Collections.Generic;
using System.Diagnostics;
using KSerialization;
using STRINGS;
using UnityEngine;

[DebuggerDisplay("{name}")]
[SerializationConfig(MemberSerialization.OptIn)]
public class Battery : KMonoBehaviour, IEnergyConsumer, IEnergyProducer, IEffectDescriptor
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
			return this.connectionStatus != CircuitManager.ConnectionStatus.NotConnected;
		}
	}

	public bool IsPowered
	{
		get
		{
			return this.connectionStatus == CircuitManager.ConnectionStatus.Powered || this.connectionStatus == CircuitManager.ConnectionStatus.OverDraw;
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Components.Batteries.Add(this);
		Building component = base.GetComponent<Building>();
		this.PowerCell = component.GetPowerInputCell();
		this.Subscribe(-592767678, new Action<object>(this.OnOperationalChanged));
		this.OnOperationalChanged(null);
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, new string[] { "meter_target", "meter_fill", "meter_frame", "meter_OL" });
		Game.Instance.circuitManager.Connect(this);
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
		Game.Instance.circuitManager.Disconnect(this);
		Components.Batteries.Remove(this);
		base.OnCleanUp();
	}

	private void SimUpdate(float dt)
	{
		this.dt = dt;
		this.joulesConsumed = 0f;
		if (this.connectionStatus != CircuitManager.ConnectionStatus.NotConnected && this.JoulesAvailable < this.capacity)
		{
			this.WattsUsed = Mathf.Max(0f, Mathf.Ceil(this.capacity - this.JoulesAvailable));
		}
		else
		{
			this.WattsUsed = 0f;
		}
		float percentFull = this.PercentFull;
		this.meter.SetPositionPercent(percentFull);
		this.UpdateSounds();
		this.PreviousJoulesAvailable = this.JoulesAvailable;
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
		this.WattsUsed = this.joulesConsumed / this.dt;
	}

	public void ConsumeEnergy(float joules)
	{
		this.joulesAvailable = Mathf.Max(0f, this.JoulesAvailable - joules);
	}

	public List<Descriptor> GetDescriptors(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(UI.BUILDINGEFFECTS.REQUIRESPOWERGENERATOR, UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESPOWERGENERATOR, Descriptor.DescriptorType.Requirement);
		list.Add(descriptor);
		Descriptor descriptor2 = default(Descriptor);
		string text = string.Format(UI.BUILDINGEFFECTS.BATTERYEFFECT, GameUtil.GetFormattedJoules(this.capacity, string.Empty));
		descriptor2.SetupDescriptor(text, text, Descriptor.DescriptorType.Effect);
		list.Add(descriptor2);
		return list;
	}

	[SerializeField]
	public float capacity;

	[Serialize]
	private float joulesAvailable;

	[MyCmpGet]
	private Operational operational;

	private MeterController meter;

	[SerializeField]
	public int powerSortOrder;

	private float PreviousJoulesAvailable;

	private CircuitManager.ConnectionStatus connectionStatus;

	private float dt;

	private float joulesConsumed;
}
