using System;
using System.Collections.Generic;
using Klei;
using UnityEngine;

public class GeyserConfigurator : KMonoBehaviour
{
	public static GeyserConfigurator.GeyserType FindType(HashedString typeId)
	{
		GeyserConfigurator.GeyserType geyserType = null;
		if (typeId != HashedString.Invalid)
		{
			geyserType = GeyserConfigurator.geyserTypes.Find((GeyserConfigurator.GeyserType t) => t.id == typeId);
		}
		if (geyserType == null)
		{
			Output.LogError(new object[] { string.Format("Tried finding a geyser with id {0} but it doesn't exist!", typeId.ToString()) });
		}
		return geyserType;
	}

	public GeyserConfigurator.GeyserInstanceConfiguration MakeConfiguration()
	{
		return this.CreateRandomInstance(this.presetType, this.presetMin, this.presetMax);
	}

	private GeyserConfigurator.GeyserInstanceConfiguration CreateRandomInstance(HashedString typeId, float min, float max)
	{
		int num = SaveLoader.Instance.worldDetailSave.globalWorldSeed;
		num = num + (int)base.transform.GetPosition().x + (int)base.transform.GetPosition().y;
		global::System.Random random = new global::System.Random(num);
		return new GeyserConfigurator.GeyserInstanceConfiguration
		{
			typeId = typeId,
			rateRoll = this.Roll(random, min, max),
			iterationLengthRoll = this.Roll(random, 0f, 1f),
			iterationPercentRoll = this.Roll(random, min, max),
			yearLengthRoll = this.Roll(random, 0f, 1f),
			yearPercentRoll = this.Roll(random, min, max)
		};
	}

	private float Roll(global::System.Random randomSource, float min, float max)
	{
		return (float)(randomSource.NextDouble() * (double)(max - min)) + min;
	}

	private static List<GeyserConfigurator.GeyserType> geyserTypes;

	public HashedString presetType;

	public float presetMin;

	public float presetMax = 1f;

	public class GeyserType
	{
		public GeyserType(string id, SimHashes element, float temperature, float minRatePerCycle, float maxRatePerCycle, float maxPressure, float minIterationLength = 60f, float maxIterationLength = 1140f, float minIterationPercent = 0.1f, float maxIterationPercent = 0.9f, float minYearLength = 15000f, float maxYearLength = 135000f, float minYearPercent = 0.4f, float maxYearPercent = 0.8f)
		{
			this.id = id;
			this.idHash = id;
			this.element = element;
			this.temperature = temperature;
			this.minRatePerCycle = minRatePerCycle;
			this.maxRatePerCycle = maxRatePerCycle;
			this.maxPressure = maxPressure;
			this.minIterationLength = minIterationLength;
			this.maxIterationLength = maxIterationLength;
			this.minIterationPercent = minIterationPercent;
			this.maxIterationPercent = maxIterationPercent;
			this.minYearLength = minYearLength;
			this.maxYearLength = maxYearLength;
			this.minYearPercent = minYearPercent;
			this.maxYearPercent = maxYearPercent;
			if (GeyserConfigurator.geyserTypes == null)
			{
				GeyserConfigurator.geyserTypes = new List<GeyserConfigurator.GeyserType>();
			}
			GeyserConfigurator.geyserTypes.Add(this);
		}

		public GeyserConfigurator.GeyserType AddDisease(SimUtil.DiseaseInfo diseaseInfo)
		{
			this.diseaseInfo = diseaseInfo;
			return this;
		}

		public string id;

		public HashedString idHash;

		public SimHashes element;

		public float temperature;

		public float minRatePerCycle;

		public float maxRatePerCycle;

		public float maxPressure;

		public SimUtil.DiseaseInfo diseaseInfo = SimUtil.DiseaseInfo.Invalid;

		public float minIterationLength;

		public float maxIterationLength;

		public float minIterationPercent;

		public float maxIterationPercent;

		public float minYearLength;

		public float maxYearLength;

		public float minYearPercent;

		public float maxYearPercent;
	}

	[Serializable]
	public class GeyserInstanceConfiguration
	{
		public GeyserConfigurator.GeyserType geyserType
		{
			get
			{
				return GeyserConfigurator.FindType(this.typeId);
			}
		}

		public float GetMaxPressure()
		{
			return this.geyserType.maxPressure;
		}

		public float GetIterationLength()
		{
			return Mathf.Lerp(this.geyserType.minIterationLength, this.geyserType.maxIterationLength, this.iterationLengthRoll);
		}

		public float GetIterationPercent()
		{
			return Mathf.Lerp(this.geyserType.minIterationPercent, this.geyserType.maxIterationPercent, this.iterationPercentRoll);
		}

		public float GetOnDuration()
		{
			return this.GetIterationLength() * this.GetIterationPercent();
		}

		public float GetOffDuration()
		{
			return this.GetIterationLength() * (1f - this.GetIterationPercent());
		}

		public float GetMassPerCycle()
		{
			return Mathf.Lerp(this.geyserType.minRatePerCycle, this.geyserType.maxRatePerCycle, this.rateRoll);
		}

		public float GetEmitRate()
		{
			float num = 600f / this.GetIterationLength();
			float num2 = this.GetMassPerCycle() / num;
			return num2 / this.GetOnDuration();
		}

		public float GetYearLength()
		{
			return Mathf.Lerp(this.geyserType.minYearLength, this.geyserType.maxYearLength, this.yearLengthRoll);
		}

		public float GetYearPercent()
		{
			return Mathf.Lerp(this.geyserType.minYearPercent, this.geyserType.maxYearPercent, this.yearPercentRoll);
		}

		public float GetYearOnDuration()
		{
			return this.GetYearLength() * this.GetYearPercent();
		}

		public float GetYearOffDuration()
		{
			return this.GetYearLength() * (1f - this.GetYearPercent());
		}

		public SimHashes GetElement()
		{
			return this.geyserType.element;
		}

		public float GetTemperature()
		{
			return this.geyserType.temperature;
		}

		public byte GetDiseaseIdx()
		{
			return this.geyserType.diseaseInfo.idx;
		}

		public int GetDiseaseCount()
		{
			return this.geyserType.diseaseInfo.count;
		}

		public HashedString typeId;

		public float rateRoll;

		public float iterationLengthRoll;

		public float iterationPercentRoll;

		public float yearLengthRoll;

		public float yearPercentRoll;
	}
}
