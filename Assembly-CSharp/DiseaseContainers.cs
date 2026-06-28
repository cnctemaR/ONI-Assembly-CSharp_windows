using System;
using System.Collections.Generic;
using Database;
using Klei;
using Klei.AI;
using Klei.AI.DiseaseGrowthRules;
using UnityEngine;

public class DiseaseContainers : KGameObjectComponentManager<DiseaseContainer>
{
	public HandleVector<int>.Handle Add(GameObject go, byte disease_idx, int disease_count)
	{
		DiseaseContainer diseaseContainer = new DiseaseContainer(go, disease_idx, disease_count);
		if (disease_idx != 255)
		{
			diseaseContainer = this.EvaluateGrowthConstants(diseaseContainer);
		}
		return base.Add(go, diseaseContainer);
	}

	protected override void OnCleanUp(HandleVector<int>.Handle h)
	{
		AutoDisinfectable autoDisinfectable = base.GetData(h).autoDisinfectable;
		if (autoDisinfectable != null)
		{
			AutoDisinfectableManager.Instance.RemoveAutoDisinfectable(autoDisinfectable);
		}
		base.OnCleanUp(h);
	}

	public override void Sim200ms(float dt)
	{
		for (int i = 0; i < this.data.Count; i++)
		{
			DiseaseContainer diseaseContainer = this.data[i];
			if (diseaseContainer.diseaseIdx != 255 && !(diseaseContainer.primaryElement == null))
			{
				Disease disease = Db.Get().Diseases[(int)diseaseContainer.diseaseIdx];
				float num = DiseaseContainers.CalculateDelta(diseaseContainer, disease, dt);
				num += diseaseContainer.accumulatedError;
				int num2 = (int)num;
				diseaseContainer.accumulatedError = num - (float)num2;
				bool flag = diseaseContainer.diseaseCount > diseaseContainer.overpopulationCount;
				bool flag2 = diseaseContainer.diseaseCount + num2 > diseaseContainer.overpopulationCount;
				if (flag != flag2)
				{
					diseaseContainer = this.EvaluateGrowthConstants(diseaseContainer);
				}
				diseaseContainer.diseaseCount += num2;
				if (diseaseContainer.diseaseCount <= 0)
				{
					diseaseContainer.diseaseCount = 0;
					diseaseContainer.diseaseIdx = byte.MaxValue;
					diseaseContainer.accumulatedError = 0f;
				}
				this.data[i] = diseaseContainer;
			}
		}
	}

	public static float CalculateDelta(DiseaseContainer container, Disease disease, float dt)
	{
		int num = Grid.PosToCell(container.primaryElement.transform.GetPosition());
		return DiseaseContainers.CalculateDelta(container.diseaseCount, (int)container.elemIdx, container.primaryElement.Mass, num, container.primaryElement.Temperature, container.instanceGrowthRate, disease, dt);
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
			byte elementIdx = Grid.Cell[environment_cell].elementIdx;
			ElemExposureInfo elemExposureInfo = disease.elemExposureInfo[(int)elementIdx];
			num += elemExposureInfo.CalculateExposureDiseaseCountDelta(disease_count, dt);
		}
		return num;
	}

	public int ModifyDiseaseCount(HandleVector<int>.Handle h, int disease_count_delta)
	{
		DiseaseContainer data = base.GetData(h);
		data.diseaseCount = Math.Max(0, data.diseaseCount + disease_count_delta);
		if (data.diseaseCount == 0)
		{
			data.diseaseIdx = byte.MaxValue;
			data.accumulatedError = 0f;
		}
		base.SetData(h, data);
		return data.diseaseCount;
	}

	public int AddDisease(HandleVector<int>.Handle h, byte disease_idx, int disease_count)
	{
		DiseaseContainer diseaseContainer = base.GetData(h);
		SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(disease_idx, disease_count, diseaseContainer.diseaseIdx, diseaseContainer.diseaseCount);
		bool flag = diseaseContainer.diseaseIdx != diseaseInfo.idx;
		diseaseContainer.diseaseIdx = diseaseInfo.idx;
		diseaseContainer.diseaseCount = diseaseInfo.count;
		if (flag && diseaseInfo.idx != 255)
		{
			diseaseContainer = this.EvaluateGrowthConstants(diseaseContainer);
		}
		base.SetData(h, diseaseContainer);
		if (flag)
		{
			diseaseContainer.primaryElement.Trigger(-283306403, null);
		}
		return diseaseContainer.diseaseCount;
	}

	public void UpdateOverlayColours()
	{
		GridArea visibleArea = GridVisibleArea.GetVisibleArea();
		global::Database.Diseases diseases = Db.Get().Diseases;
		Color32 color = new Color32(0, 0, 0, byte.MaxValue);
		for (int i = 0; i < this.data.Count; i++)
		{
			DiseaseContainer diseaseContainer = this.data[i];
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
					diseaseContainer.GetVisualDiseaseIdxAndCount(out num2, out num3);
					if (num2 != 255)
					{
						color2 = diseases[num2].overlayColour;
						num = num3;
					}
					if (diseaseContainer.isContainer)
					{
						Storage component = diseaseContainer.primaryElement.GetComponent<Storage>();
						List<GameObject> items = component.items;
						for (int j = 0; j < items.Count; j++)
						{
							GameObject gameObject = items[j];
							if (gameObject != null)
							{
								HandleVector<int>.Handle handle = base.GetHandle(gameObject);
								if (handle.IsValid())
								{
									DiseaseContainer data = base.GetData(handle);
									if (data.diseaseCount > num && data.diseaseIdx != 255)
									{
										num = data.diseaseCount;
										color2 = diseases[(int)data.diseaseIdx].overlayColour;
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

	private DiseaseContainer EvaluateGrowthConstants(DiseaseContainer container)
	{
		Disease disease = Db.Get().Diseases[(int)container.diseaseIdx];
		KPrefabID component = container.primaryElement.GetComponent<KPrefabID>();
		ElemGrowthInfo elemGrowthInfo = disease.elemGrowthInfo[(int)container.diseaseIdx];
		container.overpopulationCount = (int)(elemGrowthInfo.maxCountPerKG * container.primaryElement.Mass);
		container.instanceGrowthRate = disease.GetGrowthRateForTags(component.Tags, container.diseaseCount > container.overpopulationCount);
		return container;
	}

	public override void Clear()
	{
		base.Clear();
		for (int i = 0; i < this.data.Count; i++)
		{
			this.data[i].Clear();
		}
		this.data.Clear();
		this.handles.Clear();
	}
}
