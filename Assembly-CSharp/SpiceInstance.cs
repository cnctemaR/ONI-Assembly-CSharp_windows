using System;
using Klei.AI;

[Serializable]
public struct SpiceInstance
{
	public AttributeModifier CalorieModifier
	{
		get
		{
			return SpiceGrinder.SettingOptions[this.Id].Spice.CalorieModifier;
		}
	}

	public AttributeModifier FoodModifier
	{
		get
		{
			return SpiceGrinder.SettingOptions[this.Id].Spice.FoodModifier;
		}
	}

	public Effect StatBonus
	{
		get
		{
			return SpiceGrinder.SettingOptions[this.Id].StatBonus;
		}
	}

	public Tag Id;

	public float TotalKG;
}
