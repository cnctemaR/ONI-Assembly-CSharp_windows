using System;
using TUNING;
using UnityEngine;

public class RocketStats
{
	public RocketStats(CommandModule commandModule)
	{
		this.commandModule = commandModule;
	}

	public float GetRocketMaxDistance()
	{
		float totalMass = this.GetTotalMass();
		float totalThrust = this.GetTotalThrust();
		float num = ROCKETRY.CalculateMassWithPenalty(totalMass);
		return Mathf.Max(0f, totalThrust - num);
	}

	public float GetTotalMass()
	{
		return this.GetDryMass() + this.GetWetMass();
	}

	public float GetDryMass()
	{
		float num = 0f;
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this.commandModule.GetComponent<AttachableBuilding>()))
		{
			RocketModule component = gameObject.GetComponent<RocketModule>();
			if (component != null)
			{
				num += component.GetComponent<PrimaryElement>().Mass;
			}
		}
		return num;
	}

	public float GetWetMass()
	{
		float num = 0f;
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this.commandModule.GetComponent<AttachableBuilding>()))
		{
			RocketModule component = gameObject.GetComponent<RocketModule>();
			if (component != null)
			{
				FuelTank component2 = component.GetComponent<FuelTank>();
				OxidizerTank component3 = component.GetComponent<OxidizerTank>();
				SolidBooster component4 = component.GetComponent<SolidBooster>();
				if (component2 != null)
				{
					num += component2.MassStored();
				}
				if (component3 != null)
				{
					num += component3.MassStored();
				}
				if (component4 != null)
				{
					num += component4.fuelStorage.MassStored();
				}
			}
		}
		return num;
	}

	public Tag GetEngineFuelTag()
	{
		RocketEngine mainEngine = this.GetMainEngine();
		if (mainEngine != null)
		{
			return mainEngine.fuelTag;
		}
		return null;
	}

	public float GetTotalFuel(bool includeBoosters = false)
	{
		float num = 0f;
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this.commandModule.GetComponent<AttachableBuilding>()))
		{
			FuelTank component = gameObject.GetComponent<FuelTank>();
			Tag engineFuelTag = this.GetEngineFuelTag();
			if (component != null)
			{
				num += component.GetAmountAvailable(engineFuelTag);
			}
			if (includeBoosters)
			{
				SolidBooster component2 = gameObject.GetComponent<SolidBooster>();
				if (component2 != null)
				{
					num += component2.fuelStorage.GetAmountAvailable(component2.fuelTag);
				}
			}
		}
		return num;
	}

	public float GetTotalOxidizer(bool includeBoosters = false)
	{
		float num = 0f;
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this.commandModule.GetComponent<AttachableBuilding>()))
		{
			OxidizerTank component = gameObject.GetComponent<OxidizerTank>();
			if (component != null)
			{
				num += component.GetAmountAvailable(ElementLoader.FindElementByHash(SimHashes.LiquidOxygen).tag);
				num += component.GetAmountAvailable(GameTags.OxyRock);
			}
			if (includeBoosters)
			{
				SolidBooster component2 = gameObject.GetComponent<SolidBooster>();
				if (component2 != null)
				{
					num += component2.fuelStorage.GetAmountAvailable(GameTags.OxyRock);
				}
			}
		}
		return num;
	}

	public float GetAverageOxidizerEfficiency()
	{
		float num = 0f;
		float num2 = 0f;
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this.commandModule.GetComponent<AttachableBuilding>()))
		{
			OxidizerTank component = gameObject.GetComponent<OxidizerTank>();
			if (component != null)
			{
				num += component.GetAmountAvailable(ElementLoader.FindElementByHash(SimHashes.LiquidOxygen).tag);
				num2 += component.GetAmountAvailable(GameTags.OxyRock);
			}
		}
		if (num + num2 == 0f)
		{
			return 0f;
		}
		float num3 = (num2 * ROCKETRY.OXIDIZER_EFFICIENCY.LOW + num * ROCKETRY.OXIDIZER_EFFICIENCY.HIGH) / (num2 + num);
		return num3 * 100f;
	}

	public float GetTotalThrust()
	{
		float totalFuel = this.GetTotalFuel(false);
		float totalOxidizer = this.GetTotalOxidizer(false);
		float averageOxidizerEfficiency = this.GetAverageOxidizerEfficiency();
		RocketEngine mainEngine = this.GetMainEngine();
		if (mainEngine == null)
		{
			return 0f;
		}
		float num = ((!mainEngine.requireOxidizer) ? (totalFuel * mainEngine.efficiency) : (Mathf.Min(totalFuel, totalOxidizer) * (mainEngine.efficiency * (averageOxidizerEfficiency / 100f))));
		return num + this.GetBoosterThrust();
	}

	public float GetBoosterThrust()
	{
		float num = 0f;
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this.commandModule.GetComponent<AttachableBuilding>()))
		{
			SolidBooster component = gameObject.GetComponent<SolidBooster>();
			if (component != null)
			{
				float amountAvailable = component.fuelStorage.GetAmountAvailable(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag);
				float amountAvailable2 = component.fuelStorage.GetAmountAvailable(ElementLoader.FindElementByHash(SimHashes.Iron).tag);
				num += component.efficiency * Mathf.Min(amountAvailable, amountAvailable2);
			}
		}
		return num;
	}

	public float GetEngineEfficiency()
	{
		RocketEngine mainEngine = this.GetMainEngine();
		if (mainEngine != null)
		{
			return mainEngine.efficiency;
		}
		return 0f;
	}

	public RocketEngine GetMainEngine()
	{
		RocketEngine rocketEngine = null;
		foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(this.commandModule.GetComponent<AttachableBuilding>()))
		{
			rocketEngine = gameObject.GetComponent<RocketEngine>();
			if (rocketEngine != null && rocketEngine.mainEngine)
			{
				break;
			}
		}
		return rocketEngine;
	}

	public float GetTotalOxidizableFuel()
	{
		float totalFuel = this.GetTotalFuel(false);
		float totalOxidizer = this.GetTotalOxidizer(false);
		return Mathf.Min(totalFuel, totalOxidizer);
	}

	private CommandModule commandModule;
}
