using System;
using UnityEngine;

public struct DiseaseContainer
{
	public DiseaseContainer(GameObject go, ushort elemIdx)
	{
		this.elemIdx = elemIdx;
		this.isContainer = go.GetComponent<IUserControlledCapacity>() != null && go.GetComponent<Storage>() != null;
		Conduit component = go.GetComponent<Conduit>();
		if (component != null)
		{
			this.conduitType = component.type;
		}
		else
		{
			this.conduitType = ConduitType.None;
		}
		this.controller = go.GetComponent<KBatchedAnimController>();
		this.overpopulationCount = 1;
		this.instanceGrowthRate = 1f;
		this.accumulatedError = 0f;
		this.visualDiseaseProvider = null;
		this.autoDisinfectable = go.GetComponent<AutoDisinfectable>();
		if (this.autoDisinfectable != null)
		{
			AutoDisinfectableManager.Instance.AddAutoDisinfectable(this.autoDisinfectable);
		}
	}

	public void Clear()
	{
		this.controller = null;
	}

	public AutoDisinfectable autoDisinfectable;

	public ushort elemIdx;

	public bool isContainer;

	public ConduitType conduitType;

	public KBatchedAnimController controller;

	public GameObject visualDiseaseProvider;

	public int overpopulationCount;

	public float instanceGrowthRate;

	public float accumulatedError;
}
