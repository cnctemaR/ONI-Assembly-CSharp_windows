using System;

public interface IManageGrowingStates
{
	float TimeUntilNextHarvest();

	float PercentGrown();

	Crop GetGropComponent();

	void OverrideMaturityLevel(float percentage);

	float DomesticGrowthTime();

	float WildGrowthTime();
}
