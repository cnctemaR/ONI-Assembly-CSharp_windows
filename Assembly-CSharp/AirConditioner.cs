using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class AirConditioner : KMonoBehaviour, ISaveLoadableJson, IEffectDescriptor
{
	public float TargetTemperature
	{
		get
		{
			return this.targetTemperature;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(-592767678, new EventSystem.EventHandler(this.OnOperationalChanged));
		this.Subscribe(824508782, new EventSystem.EventHandler(this.OnActiveChanged));
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		BuildingComplete component = base.GetComponent<BuildingComplete>();
		this.cooledAirOutputCell = component.GetUtilityOutputCell();
	}

	private void SimUpdate(float dt)
	{
		if (this.operational != null && !this.operational.IsOperational)
		{
			this.operational.SetActive(false, false);
			return;
		}
		this.UpdateState();
	}

	private void UpdateState()
	{
		bool flag = this.consumer.IsSatisfied;
		List<GameObject> items = this.storage.items;
		for (int i = 0; i < items.Count; i++)
		{
			PrimaryElement component = items[i].GetComponent<PrimaryElement>();
			if (component.Mass > 0f && component.Element.IsGas)
			{
				flag = true;
				float num = component.Temperature + this.temperatureDelta;
				float num2 = Game.Instance.gasConduitFlow.AddElement(this.cooledAirOutputCell, component.ElementID, component.Mass, num);
				component.KeepZeroMassObject = true;
				component.Mass -= num2;
				break;
			}
		}
		this.operational.SetActive(flag, false);
	}

	private void OnOperationalChanged(object data)
	{
		if (this.operational.IsOperational)
		{
			this.UpdateState();
		}
	}

	private void OnActiveChanged(object data)
	{
		bool flag = (bool)data;
		if (flag)
		{
			this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Cooling, null);
		}
		else
		{
			this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, null, null);
		}
	}

	public int DescriptionOrder { get; set; }

	public List<Descriptor> GetRequirementDescriptions(BuildingDef def)
	{
		return null;
	}

	public List<Descriptor> GetEffectDescriptions(BuildingDef def)
	{
		List<Descriptor> list = new List<Descriptor>();
		string formattedTemperature = GameUtil.GetFormattedTemperature(this.temperatureDelta, GameUtil.TimeSlice.None, GameUtil.TemperatureInterpretation.Relative);
		Descriptor descriptor = default(Descriptor);
		descriptor.SetupDescriptor(string.Format(UI.LISTENTRYSTRINGNOLINEBREAK, string.Format(UI.BUILDINGEFFECTS.GASCOOLING, formattedTemperature)), string.Format(UI.BUILDINGEFFECTS.TOOLTIPS.GASCOOLING, formattedTemperature));
		list.Add(descriptor);
		return list;
	}

	[MyCmpReq]
	private KSelectable selectable;

	public float temperatureDelta = -14f;

	[Serialize]
	private float targetTemperature;

	[MyCmpReq]
	private Storage storage;

	[MyCmpReq]
	private Operational operational;

	[MyCmpReq]
	private ConduitConsumer consumer;

	private int cooledAirOutputCell = -1;
}
