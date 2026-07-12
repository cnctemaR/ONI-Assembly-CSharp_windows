using System;
using Klei;
using Klei.AI;
using Klei.AI.DiseaseGrowthRules;

public class ConduitDiseaseManager : KCompactedVector<ConduitDiseaseManager.Data>
{
	private static ElemGrowthInfo GetGrowthInfo(byte disease_idx, ushort elem_idx)
	{
		ElemGrowthInfo elemGrowthInfo;
		if (disease_idx != 255)
		{
			elemGrowthInfo = Db.Get().Diseases[(int)disease_idx].elemGrowthInfo[(int)elem_idx];
		}
		else
		{
			elemGrowthInfo = Disease.DEFAULT_GROWTH_INFO;
		}
		return elemGrowthInfo;
	}

	public ConduitDiseaseManager(ConduitTemperatureManager temperature_manager)
		: base(0)
	{
		this.temperatureManager = temperature_manager;
	}

	public HandleVector<int>.Handle Allocate(HandleVector<int>.Handle temperature_handle, ref ConduitFlow.ConduitContents contents)
	{
		ushort elementIndex = ElementLoader.GetElementIndex(contents.element);
		ConduitDiseaseManager.Data data = new ConduitDiseaseManager.Data(temperature_handle, elementIndex, contents.mass, contents.diseaseIdx, contents.diseaseCount);
		return base.Allocate(data);
	}

	public void SetData(HandleVector<int>.Handle handle, ref ConduitFlow.ConduitContents contents)
	{
		ConduitDiseaseManager.Data data = base.GetData(handle);
		data.diseaseCount = contents.diseaseCount;
		if (contents.diseaseIdx != data.diseaseIdx)
		{
			data.diseaseIdx = contents.diseaseIdx;
			ushort elementIndex = ElementLoader.GetElementIndex(contents.element);
			data.growthInfo = ConduitDiseaseManager.GetGrowthInfo(contents.diseaseIdx, elementIndex);
		}
		base.SetData(handle, data);
	}

	public void Sim200ms(float dt)
	{
		using (new KProfiler.Region("ConduitDiseaseManager.SimUpdate", null))
		{
			for (int i = 0; i < this.data.Count; i++)
			{
				ConduitDiseaseManager.Data data = this.data[i];
				if (data.diseaseIdx != 255)
				{
					float num = data.accumulatedError;
					num += data.growthInfo.CalculateDiseaseCountDelta(data.diseaseCount, data.mass, dt);
					Disease disease = Db.Get().Diseases[(int)data.diseaseIdx];
					float num2 = Disease.HalfLifeToGrowthRate(Disease.CalculateRangeHalfLife(this.temperatureManager.GetTemperature(data.temperatureHandle), ref disease.temperatureRange, ref disease.temperatureHalfLives), dt);
					num += (float)data.diseaseCount * num2 - (float)data.diseaseCount;
					int num3 = (int)num;
					data.accumulatedError = num - (float)num3;
					data.diseaseCount += num3;
					if (data.diseaseCount <= 0)
					{
						data.diseaseCount = 0;
						data.diseaseIdx = byte.MaxValue;
						data.accumulatedError = 0f;
					}
					this.data[i] = data;
				}
			}
		}
	}

	public void ModifyDiseaseCount(HandleVector<int>.Handle h, int disease_count_delta)
	{
		ConduitDiseaseManager.Data data = base.GetData(h);
		data.diseaseCount = Math.Max(0, data.diseaseCount + disease_count_delta);
		if (data.diseaseCount == 0)
		{
			data.diseaseIdx = byte.MaxValue;
		}
		base.SetData(h, data);
	}

	public void AddDisease(HandleVector<int>.Handle h, byte disease_idx, int disease_count)
	{
		ConduitDiseaseManager.Data data = base.GetData(h);
		SimUtil.DiseaseInfo diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(disease_idx, disease_count, data.diseaseIdx, data.diseaseCount);
		data.diseaseIdx = diseaseInfo.idx;
		data.diseaseCount = diseaseInfo.count;
		base.SetData(h, data);
	}

	private ConduitTemperatureManager temperatureManager;

	public struct Data
	{
		public Data(HandleVector<int>.Handle temperature_handle, ushort elem_idx, float mass, byte disease_idx, int disease_count)
		{
			this.diseaseIdx = disease_idx;
			this.elemIdx = elem_idx;
			this.mass = mass;
			this.diseaseCount = disease_count;
			this.accumulatedError = 0f;
			this.temperatureHandle = temperature_handle;
			this.growthInfo = ConduitDiseaseManager.GetGrowthInfo(disease_idx, elem_idx);
		}

		public byte diseaseIdx;

		public ushort elemIdx;

		public int diseaseCount;

		public float accumulatedError;

		public float mass;

		public HandleVector<int>.Handle temperatureHandle;

		public ElemGrowthInfo growthInfo;
	}
}
