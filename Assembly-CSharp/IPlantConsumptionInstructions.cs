using System;

public interface IPlantConsumptionInstructions
{
	CellOffset[] GetAllowedOffsets();

	float ConsumePlant(float desiredUnitsToConsume);

	float PlantProductGrowthPerCycle();

	bool CanPlantBeEaten();

	string GetFormattedConsumptionPerCycle(float consumer_caloriesLossPerCaloriesPerKG);

	Diet.Info.FoodType GetDietFoodType();
}
