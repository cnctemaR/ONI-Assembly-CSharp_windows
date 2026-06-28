using System;
using System.Collections.Generic;
using KSerialization;
using STRINGS;
using UnityEngine;

[SerializationConfig(MemberSerialization.OptIn)]
public class SuitTank : KMonoBehaviour, IGameObjectEffectDescriptor
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
		this.initialAmount = this.amount;
	}

	public float PercentFull()
	{
		if (this.amount == 0f)
		{
			return 0f;
		}
		return this.amount / this.initialAmount;
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

	public float GetInitialAmount()
	{
		return this.initialAmount;
	}

	public void Empty()
	{
		this.amount = 0f;
	}

	public void Refill()
	{
		this.amount = this.initialAmount;
	}

	public void RemovePercentage(float percentage)
	{
		this.amount -= percentage * this.initialAmount;
	}

	public void Add(float amt)
	{
		this.amount = Math.Min(this.initialAmount, this.amount + amt);
	}

	public List<Descriptor> GetDescriptors(GameObject go)
	{
		List<Descriptor> list = new List<Descriptor>();
		if (this.element.ToLower() == "oxygen")
		{
			string text = ((!this.underwaterSupport) ? string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.OXYGEN_TANK, GameUtil.GetFormattedMass(this.amount, GameUtil.TimeSlice.None, true, "{0:0.#}")) : string.Format(UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.OXYGEN_TANK_UNDERWATER, GameUtil.GetFormattedMass(this.amount, GameUtil.TimeSlice.None, true, "{0:0.#}")));
			list.Add(new Descriptor(text, text, Descriptor.DescriptorType.Effect, false));
		}
		return list;
	}

	[Serialize]
	public string element;

	[Serialize]
	public float amount;

	[Serialize]
	private float lowThreshold = 0.333f;

	[Serialize]
	private float initialAmount;

	public bool underwaterSupport;
}
