using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class Battery : KMonoBehaviour, IEnergyConsumer, IEffectDescriptor
{
	public int DescriptionOrder { get; set; }

	public float WattsUsed { get; private set; }

	public float WattsNeededWhenActive
	{
		get
		{
			return this.generator.WattageRating;
		}
	}

	public float PercentFull
	{
		get
		{
			return this.generator.JoulesAvailable / this.generator.Capacity;
		}
	}

	public float PreviousPercentFull
	{
		get
		{
			return this.PreviousJoulesAvailable / this.generator.Capacity;
		}
	}

	public float JoulesAvailable
	{
		get
		{
			return this.generator.JoulesAvailable;
		}
	}

	public float Capacity
	{
		get
		{
			return this.generator.Capacity;
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

	protected override void OnSpawn()
	{
		base.OnSpawn();
		Building component = base.GetComponent<Building>();
		this.PowerCell = component.GetPowerInputCell();
		this.Subscribe(-592767678, new EventSystem.EventHandler(this.OnOperationalChanged));
		this.OnOperationalChanged(null);
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, new string[] { "meter_target", "meter_fill", "meter_frame", "meter_OL" });
		Game.Instance.circuitManager.Connect(this);
	}

	private void OnOperationalChanged(object data)
	{
		if (this.operational.IsOperational)
		{
			Game.Instance.circuitManager.Connect(this);
			base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.JoulesAvailable, this.generator);
		}
		else
		{
			Game.Instance.circuitManager.Disconnect(this);
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.JoulesAvailable);
		}
	}

	protected override void OnCleanUp()
	{
		Game.Instance.circuitManager.Disconnect(this);
		base.OnCleanUp();
	}

	private void SimUpdate(float dt)
	{
		this.dt = dt;
		this.joulesConsumed = 0f;
		if (this.connectionStatus != CircuitManager.ConnectionStatus.NotConnected && this.generator.JoulesAvailable < this.generator.Capacity)
		{
			this.WattsUsed = Mathf.Max(0f, Mathf.Ceil(this.generator.Capacity - this.generator.JoulesAvailable));
		}
		else
		{
			this.WattsUsed = 0f;
		}
		float num = this.generator.JoulesAvailable / this.generator.Capacity;
		this.meter.SetPositionPercent(num);
		this.UpdateSounds();
		this.PreviousJoulesAvailable = this.generator.JoulesAvailable;
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
		this.generator.JoulesAvailable = Mathf.Min(this.generator.Capacity, this.generator.JoulesAvailable + this.joulesConsumed);
		this.WattsUsed = this.joulesConsumed / this.dt;
		if (status != CircuitManager.ConnectionStatus.NotConnected)
		{
			this.operational.SetActive(this.operational.IsOperational && this.generator.JoulesAvailable > 0f, false);
			base.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoPowerSource);
		}
		else
		{
			this.operational.SetActive(false, false);
			base.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoPowerSource, null);
		}
	}

	public void AddEnergy(float joules)
	{
		this.joulesConsumed += joules;
	}

	public void ConsumeEnergy(float joules)
	{
		this.generator.JoulesAvailable = Mathf.Max(0f, this.generator.JoulesAvailable - joules);
	}

	public List<Descriptor> GetRequirementDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, UI.BUILDINGEFFECTS.REQUIRESPOWERGENERATOR), UI.BUILDINGEFFECTS.TOOLTIPS.REQUIRESPOWERGENERATOR);
		list.Add(descriptor);
		return list;
	}

	public List<Descriptor> GetEffectDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Generator component = base.GetComponent<Generator>();
		if (component != null)
		{
			Descriptor descriptor = default(Descriptor);
			string text = string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, string.Format(UI.BUILDINGEFFECTS.BATTERYEFFECT, GameUtil.GetFormattedJoules(def.GeneratorBaseCapacity)));
			string text2 = string.Format(UI.BUILDINGEFFECTS.BATTERYEFFECT, GameUtil.GetFormattedJoules(def.GeneratorBaseCapacity));
			descriptor.SetupDescriptor(text, text2);
			list.Add(descriptor);
		}
		return list;
	}

	[MyCmpGet]
	private Generator generator;

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
