using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SuitTank : KMonoBehaviour, IGameObjectEffectDescriptor, OxygenBreather.IGasProvider
{
	public float LowThreshold
	{
		get
		{
			return this.lowThreshold;
		}
		set
		{
			this.lowThreshold = value;
		}
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.amount = this.capacity;
		base.Subscribe(-1617557748, new Action<object>(this.OnEquipped));
		base.Subscribe(-170173755, new Action<object>(this.OnUnequipped));
	}

	public float PercentFull()
	{
		if (this.amount == 0f)
		{
			return 0f;
		}
		return this.amount / this.capacity;
	}

	public bool IsElement(string elementComparisson)
	{
		return this.element == elementComparisson;
	}

	public bool IsEmpty()
	{
		return this.amount <= 0f;
	}

	public bool IsLow()
	{
		return this.PercentFull() < this.lowThreshold;
	}

	public bool NeedsRecharging()
	{
		return this.PercentFull() < 0.25f;
	}

	public void Refill()
	{
		this.amount = this.capacity;
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.element.ToLower() == "oxygen")
		{
			string text = ((!this.underwaterSupport) ? string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.OXYGEN_TANK, GameUtil.GetFormattedMass(this.amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")) : string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.OXYGEN_TANK_UNDERWATER, GameUtil.GetFormattedMass(this.amount, GameUtil.TimeSlice.None, GameUtil.MetricMassFormat.UseThreshold, true, "{0:0.#}")));
			list.Add(new Descriptor(text, text, Descriptor.DescriptorType.Effect, false));
		}
		return list;
	}

	private void OnEquipped(object data)
	{
		Equipment equipment = (Equipment)data;
		NameDisplayScreen.Instance.SetSuitTankDisplay(equipment.gameObject, new Func<float>(this.PercentFull), true);
		equipment.GetComponent<OxygenBreather>().SetGasProvider(this);
	}

	private void OnUnequipped(object data)
	{
		Equipment equipment = (Equipment)data;
		if (!equipment.destroyed)
		{
			NameDisplayScreen.Instance.SetSuitTankDisplay(equipment.gameObject, new Func<float>(this.PercentFull), false);
			equipment.GetComponent<OxygenBreather>().SetGasProvider(new GasBreatherFromWorldProvider());
		}
	}

	public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
	{
		this.suitSuffocationMonitor = new SuitSuffocationMonitor.Instance(oxygen_breather, this);
		this.suitSuffocationMonitor.StartSM();
	}

	public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
	{
		this.suitSuffocationMonitor.StopSM("Removed suit tank");
		this.suitSuffocationMonitor = null;
	}

	public bool ConsumeGas(OxygenBreather oxygen_breather, float gas_consumed)
	{
		if (this.IsEmpty())
		{
			return false;
		}
		gas_consumed = Mathf.Min(gas_consumed, this.amount);
		this.amount -= gas_consumed;
		Game.Instance.accumulators.Accumulate(oxygen_breather.O2Accumulator, gas_consumed);
		ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, -gas_consumed, oxygen_breather.GetProperName(), null);
		return true;
	}

	public bool ShouldEmitCO2()
	{
		return false;
	}

	[ContextMenu("SetToRefillAmount")]
	public void SetToRefillAmount()
	{
		this.amount = 0.25f * this.capacity;
	}

	[ContextMenu("Empty")]
	public void Empty()
	{
		this.amount = 0f;
	}

	[Serialize]
	public string element;

	[Serialize]
	public float amount;

	[Serialize]
	private float lowThreshold = 0.333f;

	public float capacity;

	public const float REFILL_PERCENT = 0.25f;

	public bool underwaterSupport;

	private SuitSuffocationMonitor.Instance suitSuffocationMonitor;
}
