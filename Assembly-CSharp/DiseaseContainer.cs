using System;
using UnityEngine;

public struct DiseaseContainer
{
	public DiseaseContainer(GameObject go, byte disease_idx, int disease_count)
	{
		this.primaryElement = go.GetComponent<PrimaryElement>();
		this.elemIdx = this.primaryElement.Element.idx;
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
		this.visualDiseaseProvider = null;
		this.autoDisinfectable = go.GetComponent<AutoDisinfectable>();
		if (this.autoDisinfectable != null)
		{
			AutoDisinfectableManager.Instance.AddAutoDisinfectable(this.autoDisinfectable);
		}
	}

	public void GetVisualDiseaseIdxAndCount(out int disease_idx, out int disease_count)
	{
		disease_idx = (int)this.diseaseIdx;
		disease_count = this.diseaseCount;
		if (this.visualDiseaseProvider != null)
		{
			disease_idx = 255;
			disease_count = 0;
			HandleVector<int>.Handle handle = GameComps.DiseaseContainers.GetHandle(this.visualDiseaseProvider);
			if (handle != HandleVector<int>.InvalidHandle)
			{
				DiseaseContainer data = GameComps.DiseaseContainers.GetData(handle);
				disease_idx = (int)data.diseaseIdx;
				disease_count = data.diseaseCount;
			}
		}
	}

	public void Clear()
	{
		this.controller = null;
	}

	public PrimaryElement primaryElement;

	public AutoDisinfectable autoDisinfectable;

	public byte elemIdx;

	public byte diseaseIdx;

	public int diseaseCount;

	public bool isContainer;

	public ConduitType conduitType;

	public KBatchedAnimController controller;

	public GameObject visualDiseaseProvider;

	public int overpopulationCount;

	public float instanceGrowthRate;

	public float accumulatedError;
}
