using System;
using UnityEngine;

public interface IApproachableBehaviour
{
	bool IsValidTarget();

	GameObject GetTarget();

	StatusItem GetApproachStatusItem();

	StatusItem GetBehaviourStatusItem();

	CellOffset[] GetApproachOffsets()
	{
		return OffsetGroups.Use;
	}

	void OnArrive()
	{
	}

	void OnSuccess()
	{
	}

	void OnFailure()
	{
	}
}
