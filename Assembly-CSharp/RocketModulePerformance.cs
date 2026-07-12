using System;

[Serializable]
public class RocketModulePerformance
{
	public RocketModulePerformance(float burden, float fuelKilogramPerDistance, float enginePower)
	{
		this.burden = burden;
		this.fuelKilogramPerDistance = fuelKilogramPerDistance;
		this.enginePower = enginePower;
	}

	public float Burden
	{
		get
		{
			return this.burden;
		}
	}

	public float FuelKilogramPerDistance
	{
		get
		{
			return this.fuelKilogramPerDistance;
		}
	}

	public float EnginePower
	{
		get
		{
			return this.enginePower;
		}
	}

	public float burden;

	public float fuelKilogramPerDistance;

	public float enginePower;
}
