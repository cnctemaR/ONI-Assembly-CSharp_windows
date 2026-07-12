using System;
using System.Collections.Generic;
using Klei;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/GeyserConfigurator")]
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
			global::Debug.LogError(string.Format("Tried finding a geyser with id {0} but it doesn't exist!", typeId.ToString()));
		}
		return geyserType;
	}

	public GeyserConfigurator.GeyserInstanceConfiguration MakeConfiguration()
	{
		return this.CreateRandomInstance(this.presetType, this.presetMin, this.presetMax);
	}

	private GeyserConfigurator.GeyserInstanceConfiguration CreateRandomInstance(HashedString typeId, float min, float max)
	{
		KRandom krandom = new KRandom(SaveLoader.Instance.clusterDetailSave.globalWorldSeed + (int)base.transform.GetPosition().x + (int)base.transform.GetPosition().y);
		return new GeyserConfigurator.GeyserInstanceConfiguration
		{
			typeId = typeId,
			rateRoll = this.Roll(krandom, min, max),
			iterationLengthRoll = this.Roll(krandom, 0f, 1f),
			iterationPercentRoll = this.Roll(krandom, min, max),
			yearLengthRoll = this.Roll(krandom, 0f, 1f),
			yearPercentRoll = this.Roll(krandom, min, max)
		};
	}

	private float Roll(KRandom randomSource, float min, float max)
	{
		return (float)(randomSource.NextDouble() * (double)(max - min)) + min;
	}

	private static List<GeyserConfigurator.GeyserType> geyserTypes;

	public HashedString presetType;

	public float presetMin;

	public float presetMax = 1f;

	public enum GeyserShape
	{
		Gas,
		Liquid,
		Molten
	}

	public class GeyserType
	{
		public GeyserType(string id, SimHashes element, GeyserConfigurator.GeyserShape shape, float temperature, float minRatePerCycle, float maxRatePerCycle, float maxPressure, float minIterationLength = 60f, float maxIterationLength = 1140f, float minIterationPercent = 0.1f, float maxIterationPercent = 0.9f, float minYearLength = 15000f, float maxYearLength = 135000f, float minYearPercent = 0.4f, float maxYearPercent = 0.8f, float geyserTemperature = 372.15f, string DlcID = "")
		{
			this.id = id;
			this.idHash = id;
			this.element = element;
			this.shape = shape;
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
			this.DlcID = DlcID;
			this.geyserTemperature = geyserTemperature;
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

		public GeyserType()
		{
			this.id = "Blank";
			this.element = SimHashes.Void;
			this.temperature = 0f;
			this.minRatePerCycle = 0f;
			this.maxRatePerCycle = 0f;
			this.maxPressure = 0f;
			this.minIterationLength = 0f;
			this.maxIterationLength = 0f;
			this.minIterationPercent = 0f;
			this.maxIterationPercent = 0f;
			this.minYearLength = 0f;
			this.maxYearLength = 0f;
			this.minYearPercent = 0f;
			this.maxYearPercent = 0f;
			this.geyserTemperature = 0f;
			this.DlcID = "";
		}

		public string id;

		public HashedString idHash;

		public SimHashes element;

		public GeyserConfigurator.GeyserShape shape;

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

		public float geyserTemperature;

		public string DlcID;

		public const string BLANK_ID = "Blank";

		public const SimHashes BLANK_ELEMENT = SimHashes.Void;

		public const string BLANK_DLCID = "";
	}

	[Serializable]
	public class GeyserInstanceConfiguration
	{
		public Geyser.GeyserModification GetModifier()
		{
			return this.modifier;
		}

		public void Init(bool reinit = false)
		{
			if (this.didInit && !reinit)
			{
				return;
			}
			this.didInit = true;
			this.scaledRate = this.Resample(this.rateRoll, this.geyserType.minRatePerCycle, this.geyserType.maxRatePerCycle);
			this.scaledIterationLength = this.Resample(this.iterationLengthRoll, this.geyserType.minIterationLength, this.geyserType.maxIterationLength);
			this.scaledIterationPercent = this.Resample(this.iterationPercentRoll, this.geyserType.minIterationPercent, this.geyserType.maxIterationPercent);
			this.scaledYearLength = this.Resample(this.yearLengthRoll, this.geyserType.minYearLength, this.geyserType.maxYearLength);
			this.scaledYearPercent = this.Resample(this.yearPercentRoll, this.geyserType.minYearPercent, this.geyserType.maxYearPercent);
		}

		public void SetModifier(Geyser.GeyserModification modifier)
		{
			this.modifier = modifier;
		}

		public GeyserConfigurator.GeyserType geyserType
		{
			get
			{
				return GeyserConfigurator.FindType(this.typeId);
			}
		}

		private float GetModifiedValue(float geyserVariable, float modifier, Geyser.ModificationMethod method)
		{
			float num = geyserVariable;
			if (method != Geyser.ModificationMethod.Values)
			{
				if (method == Geyser.ModificationMethod.Percentages)
				{
					num += geyserVariable * modifier;
				}
			}
			else
			{
				num += modifier;
			}
			return num;
		}

		public float GetMaxPressure()
		{
			return this.GetModifiedValue(this.geyserType.maxPressure, this.modifier.maxPressureModifier, Geyser.maxPressureModificationMethod);
		}

		public float GetIterationLength()
		{
			this.Init(false);
			return this.GetModifiedValue(this.scaledIterationLength, this.modifier.iterationDurationModifier, Geyser.IterationDurationModificationMethod);
		}

		public float GetIterationPercent()
		{
			this.Init(false);
			return Mathf.Clamp(this.GetModifiedValue(this.scaledIterationPercent, this.modifier.iterationPercentageModifier, Geyser.IterationPercentageModificationMethod), 0f, 1f);
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
			this.Init(false);
			return this.GetModifiedValue(this.scaledRate, this.modifier.massPerCycleModifier, Geyser.massModificationMethod);
		}

		public float GetEmitRate()
		{
			float num = 600f / this.GetIterationLength();
			return this.GetMassPerCycle() / num / this.GetOnDuration();
		}

		public float GetYearLength()
		{
			this.Init(false);
			return this.GetModifiedValue(this.scaledYearLength, this.modifier.yearDurationModifier, Geyser.yearDurationModificationMethod);
		}

		public float GetYearPercent()
		{
			this.Init(false);
			return Mathf.Clamp(this.GetModifiedValue(this.scaledYearPercent, this.modifier.yearPercentageModifier, Geyser.yearPercentageModificationMethod), 0f, 1f);
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
			if (!this.modifier.modifyElement || this.modifier.newElement == (SimHashes)0)
			{
				return this.geyserType.element;
			}
			return this.modifier.newElement;
		}

		public float GetTemperature()
		{
			return this.GetModifiedValue(this.geyserType.temperature, this.modifier.temperatureModifier, Geyser.temperatureModificationMethod);
		}

		public byte GetDiseaseIdx()
		{
			return this.geyserType.diseaseInfo.idx;
		}

		public int GetDiseaseCount()
		{
			return this.geyserType.diseaseInfo.count;
		}

		public float GetAverageEmission()
		{
			float num = this.GetEmitRate() * this.GetOnDuration();
			return this.GetYearOnDuration() / this.GetIterationLength() * num / this.GetYearLength();
		}

		private float Resample(float t, float min, float max)
		{
			float num = 6f;
			float num2 = 0.002472623f;
			float num3 = t * (1f - num2 * 2f) + num2;
			return (-Mathf.Log(1f / num3 - 1f) + num) / (num * 2f) * (max - min) + min;
		}

		public HashedString typeId;

		public float rateRoll;

		public float iterationLengthRoll;

		public float iterationPercentRoll;

		public float yearLengthRoll;

		public float yearPercentRoll;

		public float scaledRate;

		public float scaledIterationLength;

		public float scaledIterationPercent;

		public float scaledYearLength;

		public float scaledYearPercent;

		private bool didInit;

		private Geyser.GeyserModification modifier;
	}
}
