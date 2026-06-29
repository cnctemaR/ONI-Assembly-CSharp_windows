using System;
using System.Collections.Generic;

public class Diet
{
	public Diet(params Diet.Info[] infos)
	{
		this.infos = infos;
		this.consumedTags = new List<KeyValuePair<Tag, float>>();
		this.producedTags = new List<KeyValuePair<Tag, float>>();
		for (int i = 0; i < infos.Length; i++)
		{
			Diet.Info info = infos[i];
			List<Tag> tagsVerySlow = info.consumedTagBits.GetTagsVerySlow();
			using (List<Tag>.Enumerator enumerator = tagsVerySlow.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Tag tag = enumerator.Current;
					if (this.consumedTags.FindIndex((KeyValuePair<Tag, float> e) => e.Key == tag) == -1)
					{
						this.consumedTags.Add(new KeyValuePair<Tag, float>(tag, info.caloriesPerKg));
					}
				}
			}
			if (info.producedElement != Tag.Invalid && this.producedTags.FindIndex((KeyValuePair<Tag, float> e) => e.Key == info.producedElement) == -1)
			{
				this.producedTags.Add(new KeyValuePair<Tag, float>(info.producedElement, info.producedConversionRate));
			}
		}
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

	public List<KeyValuePair<Tag, float>> consumedTags;

	public List<KeyValuePair<Tag, float>> producedTags;

	public class Info
	{
		public Info(TagBits consumed_tag_bits, Tag produced_element, float calories_per_kg, float produced_conversion_rate = 1f, string disease_id = null, float disease_per_kg_produced = 0f)
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

		public Tag producedElement { get; private set; }

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
