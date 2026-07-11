using System;
using System.Collections.Generic;
using Database;
using Klei;
using Klei.AI;
using Klei.AI.DiseaseGrowthRules;
using UnityEngine;

public class DiseaseContainers : KGameObjectSplitComponentManager<DiseaseHeader, DiseaseContainer>
{
	public HandleVector<int>.Handle Add(GameObject go, byte disease_idx, int disease_count)
	{
		DiseaseHeader diseaseHeader = new DiseaseHeader
		{
			diseaseIdx = disease_idx,
			diseaseCount = disease_count,
			primaryElement = go.GetComponent<PrimaryElement>()
		};
		DiseaseContainer diseaseContainer = new DiseaseContainer(go, diseaseHeader.primaryElement.Element.idx);
		if (disease_idx != 255)
		{
			this.EvaluateGrowthConstants(diseaseHeader, ref diseaseContainer);
		}
		return base.Add(go, diseaseHeader, ref diseaseContainer);
	}

	protected override void OnCleanUp(HandleVector<int>.Handle h)
	{
		AutoDisinfectable autoDisinfectable = base.GetPayload(h).autoDisinfectable;
		if (autoDisinfectable != null)
		{
			AutoDisinfectableManager.Instance.RemoveAutoDisinfectable(autoDisinfectable);
		}
		base.OnCleanUp(h);
	}

	public override void Sim200ms(float dt)
	{
		ListPool<int, DiseaseContainers>.PooledList pooledList = ListPool<int, DiseaseContainers>.Allocate();
		pooledList.Capacity = Math.Max(pooledList.Capacity, this.headers.Count);
		for (int i = 0; i < this.headers.Count; i++)
		{
			DiseaseHeader diseaseHeader = this.headers[i];
			if (diseaseHeader.diseaseIdx != 255 && diseaseHeader.primaryElement != null)
			{
				pooledList.Add(i);
			}
		}
		foreach (int num in pooledList)
		{
			DiseaseContainer diseaseContainer = this.payloads[num];
			DiseaseHeader diseaseHeader2 = this.headers[num];
			Disease disease = Db.Get().Diseases[(int)diseaseHeader2.diseaseIdx];
			float num2 = DiseaseContainers.CalculateDelta(diseaseHeader2, ref diseaseContainer, disease, dt);
			num2 += diseaseContainer.accumulatedError;
			int num3 = (int)num2;
			diseaseContainer.accumulatedError = num2 - (float)num3;
			bool flag = diseaseHeader2.diseaseCount > diseaseContainer.overpopulationCount;
			bool flag2 = diseaseHeader2.diseaseCount + num3 > diseaseContainer.overpopulationCount;
			if (flag != flag2)
			{
				this.EvaluateGrowthConstants(diseaseHeader2, ref diseaseContainer);
			}
			diseaseHeader2.diseaseCount += num3;
			if (diseaseHeader2.diseaseCount <= 0)
			{
				diseaseContainer.accumulatedError = 0f;
				diseaseHeader2.diseaseCount = 0;
				diseaseHeader2.diseaseIdx = byte.MaxValue;
			}
			this.headers[num] = diseaseHeader2;
			this.payloads[num] = diseaseContainer;
		}
		pooledList.Recycle();
	}

	public static float CalculateDelta(DiseaseHeader header, ref DiseaseContainer container, Disease disease, float dt)
	{
		return DiseaseContainers.CalculateDelta(header.diseaseCount, (int)container.elemIdx, header.primaryElement.Mass, Grid.PosToCell(header.primaryElement.transform.GetPosition()), header.primaryElement.Temperature, container.instanceGrowthRate, disease, dt);
	}

	public static float CalculateDelta(int disease_count, int element_idx, float mass, int environment_cell, float temperature, float tags_multiplier_base, Disease disease, float dt)
	{
		float num = 0f;
		ElemGrowthInfo elemGrowthInfo = disease.elemGrowthInfo[element_idx];
		num += elemGrowthInfo.CalculateDiseaseCountDelta(disease_count, mass, dt);
		float num2 = Disease.CalculateRangeHalfLife(temperature, ref disease.temperatureRange, ref disease.temperatureHalfLives);
		float num3 = Disease.HalfLifeToGrowthRate(num2, dt);
		num += (float)disease_count * num3 - (float)disease_count;
		float num4 = Mathf.Pow(tags_multiplier_base, dt);
		num += (float)disease_count * num4 - (float)disease_count;
		if (Grid.IsValidCell(environment_cell))
		{
			byte b = Grid.ElementIdx[environment_cell];
			ElemExposureInfo elemExposureInfo = disease.elemExposureInfo[(int)b];
			num += elemExposureInfo.CalculateExposureDiseaseCountDelta(disease_count, dt);
		}
		return num;
	}

	public int ModifyDiseaseCount(HandleVector<int>.Handle h, int disease_count_delta)
	{
		DiseaseHeader header = base.GetHeader(h);
		header.diseaseCount = Math.Max(0, header.diseaseCount + disease_count_delta);
		if (header.diseaseCount == 0)
		{
			header.diseaseIdx = byte.MaxValue;
			DiseaseContainer payload = base.GetPayload(h);
			payload.accumulatedError = 0f;
			base.SetPayload(h, ref payload);
		}
		base.SetHeader(h, header);
		return header.diseaseCount;
	}

	public int AddDisease(HandleVector<int>.Handle h, byte disease_idx, int disease_count)
	{
		DiseaseHeader diseaseHeader;
		DiseaseContainer diseaseContainer;
		base.GetData(h, out diseaseHeader, out diseaseContainer);
		SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(disease_idx, disease_count, diseaseHeader.diseaseIdx, diseaseHeader.diseaseCount);
		bool flag = diseaseHeader.diseaseIdx != diseaseInfo.idx;
		diseaseHeader.diseaseIdx = diseaseInfo.idx;
		diseaseHeader.diseaseCount = diseaseInfo.count;
		if (flag && diseaseInfo.idx != 255)
		{
			this.EvaluateGrowthConstants(diseaseHeader, ref diseaseContainer);
			base.SetData(h, diseaseHeader, ref diseaseContainer);
		}
		else
		{
			base.SetHeader(h, diseaseHeader);
		}
		if (flag)
		{
			diseaseHeader.primaryElement.Trigger(-283306403, null);
		}
		return diseaseHeader.diseaseCount;
	}

	private void GetVisualDiseaseIdxAndCount(DiseaseHeader header, ref DiseaseContainer payload, out int disease_idx, out int disease_count)
	{
		if (payload.visualDiseaseProvider == null)
		{
			disease_idx = (int)header.diseaseIdx;
			disease_count = header.diseaseCount;
		}
		else
		{
			disease_idx = 255;
			disease_count = 0;
			HandleVector<int>.Handle handle = GameComps.DiseaseContainers.GetHandle(payload.visualDiseaseProvider);
			if (handle != HandleVector<int>.InvalidHandle)
			{
				DiseaseHeader header2 = GameComps.DiseaseContainers.GetHeader(handle);
				disease_idx = (int)header2.diseaseIdx;
				disease_count = header2.diseaseCount;
			}
		}
	}

	public void UpdateOverlayColours()
	{
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		Diseases diseases = Db.Get().Diseases;
		Color32 color = new Color32(0, 0, 0, byte.MaxValue);
		for (int i = 0; i < this.headers.Count; i++)
		{
			DiseaseContainer diseaseContainer = this.payloads[i];
			DiseaseHeader diseaseHeader = this.headers[i];
			KBatchedAnimController controller = diseaseContainer.controller;
			if (controller != null)
			{
				Color32 color2 = color;
				Vector3 position = controller.transform.GetPosition();
				if (visibleArea.Min <= position && position <= visibleArea.Max)
				{
					int num = 0;
					int num2 = 255;
					int num3 = 0;
					this.GetVisualDiseaseIdxAndCount(diseaseHeader, ref diseaseContainer, out num2, out num3);
					if (num2 != 255)
					{
						color2 = diseases[num2].overlayColour;
						num = num3;
					}
					if (diseaseContainer.isContainer)
					{
						Storage component = diseaseHeader.primaryElement.GetComponent<Storage>();
						List<GameObject> items = component.items;
						for (int j = 0; j < items.Count; j++)
						{
							GameObject gameObject = items[j];
							if (gameObject != null)
							{
								HandleVector<int>.Handle handle = base.GetHandle(gameObject);
								if (handle.IsValid())
								{
									DiseaseHeader header = base.GetHeader(handle);
									if (header.diseaseCount > num && header.diseaseIdx != 255)
									{
										num = header.diseaseCount;
										color2 = diseases[(int)header.diseaseIdx].overlayColour;
									}
								}
							}
						}
					}
					color2.a = SimUtil.DiseaseCountToAlpha254(num);
					if (diseaseContainer.conduitType != ConduitType.None)
					{
						ConduitFlow flowManager = Conduit.GetFlowManager(diseaseContainer.conduitType);
						int num4 = Grid.PosToCell(position);
						ConduitFlow.ConduitContents contents = flowManager.GetContents(num4);
						if (contents.diseaseIdx != 255 && contents.diseaseCount > num)
						{
							num = contents.diseaseCount;
							color2 = diseases[(int)contents.diseaseIdx].overlayColour;
							color2.a = byte.MaxValue;
						}
					}
				}
				controller.OverlayColour = color2;
			}
		}
	}

	private void EvaluateGrowthConstants(DiseaseHeader header, ref DiseaseContainer container)
	{
		Disease disease = Db.Get().Diseases[(int)header.diseaseIdx];
		KPrefabID component = header.primaryElement.GetComponent<KPrefabID>();
		ElemGrowthInfo elemGrowthInfo = disease.elemGrowthInfo[(int)header.diseaseIdx];
		container.overpopulationCount = (int)(elemGrowthInfo.maxCountPerKG * header.primaryElement.Mass);
		container.instanceGrowthRate = disease.GetGrowthRateForTags(component.Tags, header.diseaseCount > container.overpopulationCount);
	}

	public override void Clear()
	{
		base.Clear();
		for (int i = 0; i < this.payloads.Count; i++)
		{
			this.payloads[i].Clear();
		}
		this.headers.Clear();
		this.payloads.Clear();
		this.handles.Clear();
	}
}
