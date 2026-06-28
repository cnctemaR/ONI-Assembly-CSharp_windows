using System;

public class Diet
{
	public Diet(params Diet.Info[] infos)
	{
		this.infos = infos;
	}

	public Diet.Info[] infos { get; private set; }

	public Diet.Info GetDietInfo(TagBits tag_bits)
	{
		foreach (Diet.Info info in this.infos)
		{
			if (info.IsMatch(tag_bits))
			{
				return info;
			}
		}
		return null;
	}

	public class Info
	{
		public Info(TagBits consumed_tag_bits, SimHashes produced_element, float calories_per_kg, float produced_conversion_rate = 1f, string disease_id = null, float disease_per_kg_produced = 0f)
		{
			this.consumedTagBits = consumed_tag_bits;
			this.producedElement = produced_element;
			this.caloriesPerKg = calories_per_kg;
			this.producedConversionRate = produced_conversion_rate;
			if (!string.IsNullOrEmpty(disease_id))
			{
				this.diseaseIdx = Db.Get().Diseases.GetIndex(disease_id);
			}
			else
			{
				this.diseaseIdx = byte.MaxValue;
			}
		}

		public TagBits consumedTagBits { get; private set; }

		public SimHashes producedElement { get; private set; }

		public float caloriesPerKg { get; private set; }

		public float producedConversionRate { get; private set; }

		public byte diseaseIdx { get; private set; }

		public float diseasePerKgProduced { get; private set; }

		public bool IsMatch(TagBits tag_bits)
		{
			return tag_bits.HasAny(this.consumedTagBits);
		}

		public float ConvertCaloriesToConsumptionMass(float calories)
		{
			return calories / this.caloriesPerKg;
		}

		public float ConvertConsumptionMassToCalories(float mass)
		{
			return this.caloriesPerKg * mass;
		}

		public float ConvertConsumptionMassToProducedMass(float consumed_mass)
		{
			return consumed_mass * this.producedConversionRate;
		}
	}
}
