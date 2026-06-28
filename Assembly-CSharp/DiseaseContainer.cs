using System;
using UnityEngine;

public struct DiseaseContainer
{
	public DiseaseContainer(GameObject go, byte disease_idx, int disease_count)
	{
		this.primaryElement = go.GetComponent<PrimaryElement>();
		this.elemIdx = (byte)ElementLoader.GetElementIndex(this.primaryElement.ElementID);
		this.isContainer = go.GetComponent<IUserControlledCapacity>() != null;
		Conduit component = go.GetComponent<Conduit>();
		if (component != null)
		{
			this.conduitType = component.type;
		}
		else
		{
			this.conduitType = ConduitType.None;
		}
		this.diseaseIdx = disease_idx;
		this.diseaseCount = disease_count;
		this.controller = go.GetComponent<KBatchedAnimController>();
		this.overpopulationCount = 1;
		this.instanceGrowthRate = 1f;
		this.accumulatedError = 0f;
	}

	public void Clear()
	{
		this.controller = null;
	}

	public PrimaryElement primaryElement;

	public byte elemIdx;

	public byte diseaseIdx;

	public int diseaseCount;

	public bool isContainer;

	public ConduitType conduitType;

	public KBatchedAnimController controller;

	public int overpopulationCount;

	public float instanceGrowthRate;

	public float accumulatedError;
}
