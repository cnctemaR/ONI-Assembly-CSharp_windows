using System;
using UnityEngine;

public interface IHexCellCollector
{
	bool CheckIsCollecting();

	string GetProperName();

	Sprite GetUISprite();

	float GetCapacity();

	float GetMassStored();

	float TimeInState();

	string GetCapacityBarText();
}
