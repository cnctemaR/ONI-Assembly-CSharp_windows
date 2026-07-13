using System;

public interface IManageGrowingStates
{
	float TimeUntilNextHarvest();

	float PercentGrown();

	Crop GetCropComponent();

	void OverrideMaturityLevel(float percentage);

	float DomesticGrowthTime();

	float WildGrowthTime();

	bool IsWildPlanted();
}
