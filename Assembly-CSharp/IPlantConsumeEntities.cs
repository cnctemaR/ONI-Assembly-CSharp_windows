using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPlantConsumeEntities
{
	string GetConsumableEntitiesCategoryName();

	string GetRequirementText();

	List<KPrefabID> GetPrefabsOfPossiblePrey();

	string[] GetFormattedPossiblePreyList();

	bool IsEntityEdible(GameObject entity);

	string GetConsumedEntityName();

	bool AreEntitiesConsumptionRequirementsSatisfied();
}
