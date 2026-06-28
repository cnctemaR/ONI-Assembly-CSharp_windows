using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class EnergyGenerator : Generator, IEffectDescriptor
{
	public float MassBurnRate
	{
		get
		{
			return this.massBurnRate;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(824508782, new EventSystem.EventHandler(this.OnActiveChanged));
	}

	protected void OnActiveChanged(object is_active)
	{
		if ((bool)is_active)
		{
			base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.Wattage, this);
		}
		else
		{
			base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.GeneratorOffline, this);
		}
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.meter = new MeterController(base.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, new string[] { "meter_target", "meter_fill", "meter_frame", "meter_OL" });
	}

	protected override void SimUpdate(float dt)
	{
		base.SimUpdate(dt);
		Element element = ElementLoader.FindElementByHash(this.energySourceElement);
		List<GameObject> list = this.storage.Find(element.tag);
		float num = 0f;
		foreach (GameObject gameObject in list)
		{
			PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
			num += component.Mass;
		}
		float num2 = num / this.storage.Capacity();
		this.meter.SetPositionPercent(num2);
		if (this.operational.IsOperational)
		{
			bool flag = false;
			ushort circuitID = base.CircuitID;
			if (circuitID != 65535)
			{
				ReadOnlyCollection<Battery> batteriesOnCircuit = Game.Instance.circuitManager.GetBatteriesOnCircuit(circuitID);
				if (batteriesOnCircuit.Count > 0)
				{
					foreach (Battery battery in batteriesOnCircuit)
					{
						if (battery.PercentFull < this.batteryRefillPercent)
						{
							flag = true;
							break;
						}
					}
				}
				else
				{
					flag = true;
				}
			}
			if (this.delivery != null)
			{
				this.delivery.Pause(!flag, "Circuit has sufficient energy");
			}
			float num3 = this.massBurnRate * dt;
			num = Mathf.Max(0f, num - num3);
			if (num > 0f)
			{
				base.ApplyImmediateJoulesAvailable(base.WattageRating * dt, false);
				base.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, Db.Get().BuildingStatusItems.Wattage, this);
				this.storage.Consume(element.tag, num3);
			}
			this.operational.SetActive(num > 0f, false);
		}
		else
		{
			this.operational.SetActive(false, false);
		}
	}

	public int DescriptionOrder { get; set; }

	public List<Descriptor> GetRequirementDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		Element element = ElementLoader.FindElementByHash(this.energySourceElement);
		string text = element.tag.ProperName();
		string text2 = GameUtil.GetKeywordStyle(this.energySourceElement);
		if (element.IsVacuum)
		{
			text2 = GameUtil.GetKeywordStyle(SimHashes.Oxygen);
			text = ELEMENTS.STATEGAS;
		}
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, string.Format(UI.BUILDINGEFFECTS.ELEMENTCONSUMED, text2, text, GameUtil.GetFormattedMass(this.massBurnRate, GameUtil.TimeSlice.PerSecond, true, "F1"))), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMED, text2, text, GameUtil.GetFormattedMass(this.massBurnRate, GameUtil.TimeSlice.PerSecond, true, "F1")));
		list.Add(descriptor);
		return list;
	}

	public List<Descriptor> GetEffectDescriptions(BuildingDef def)
	{
		return null;
	}

	[MyCmpAdd]
	private Storage storage;

	[MyCmpGet]
	private ManualDeliveryKG delivery;

	[HashedEnum]
	[SerializeField]
	public SimHashes energySourceElement;

	[SerializeField]
	[Tooltip("kg/s")]
	public float massBurnRate;

	[Serialize]
	[SerializeField]
	public float batteryRefillPercent = 0.5f;

	private MeterController meter;
}
