using System;
using System.Collections.Generic;
using System.Diagnostics;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	[DebuggerDisplay("{base.Id}")]
	public abstract class Sickness : Resource
	{
		public Sickness(string id, Sickness.SicknessType type, Sickness.Severity severity, float immune_attack_strength, List<Sickness.InfectionVector> infection_vectors, float sickness_duration)
			: base(id, null, null)
		{
			this.name = new StringKey("STRINGS.DUPLICANTS.DISEASES." + id.ToUpper() + ".NAME");
			this.id = id;
			this.sicknessType = type;
			this.severity = severity;
			this.infectionVectors = infection_vectors;
			this.sicknessDuration = sickness_duration;
			this.descriptiveSymptoms = new StringKey("STRINGS.DUPLICANTS.DISEASES." + id.ToUpper() + ".DESCRIPTIVE_SYMPTOMS");
			this.cureSpeedBase = new Attribute(id + "CureSpeed", false, Attribute.Display.Normal, false, 0f, null, null);
			this.cureSpeedBase.BaseValue = 1f;
			this.cureSpeedBase.SetFormatter(new ToPercentAttributeFormatter(1f, GameUtil.TimeSlice.None));
			Db.Get().Attributes.Add(this.cureSpeedBase);
		}

		public new string Name
		{
			get
			{
				return Strings.Get(this.name);
			}
		}

		public float SicknessDuration
		{
			get
			{
				return this.sicknessDuration;
			}
		}

		public object[] Infect(GameObject go, SicknessInstance diseaseInstance, SicknessExposureInfo exposure_info)
		{
			object[] array = new object[this.components.Count];
			for (int i = 0; i < this.components.Count; i++)
			{
				array[i] = this.components[i].OnInfect(go, diseaseInstance);
			}
			return array;
		}

		public void Cure(GameObject go, object[] componentData)
		{
			for (int i = 0; i < this.components.Count; i++)
			{
				this.components[i].OnCure(go, componentData[i]);
			}
		}

		public List<Descriptor> GetSymptoms()
		{
			List<Descriptor> list = new List<Descriptor>();
			for (int i = 0; i < this.components.Count; i++)
			{
				List<Descriptor> symptoms = this.components[i].GetSymptoms();
				if (symptoms != null)
				{
					list.AddRange(symptoms);
				}
			}
			if (this.fatalityDuration > 0f)
			{
				list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.DEATH_SYMPTOM, GameUtil.GetFormattedCycles(this.fatalityDuration, "F1")), string.Format(DUPLICANTS.DISEASES.DEATH_SYMPTOM_TOOLTIP, GameUtil.GetFormattedCycles(this.fatalityDuration, "F1")), Descriptor.DescriptorType.SymptomAidable, false));
			}
			return list;
		}

		protected void AddSicknessComponent(Sickness.SicknessComponent cmp)
		{
			this.components.Add(cmp);
		}

		public T GetSicknessComponent<T>() where T : Sickness.SicknessComponent
		{
			for (int i = 0; i < this.components.Count; i++)
			{
				if (this.components[i] is T)
				{
					return this.components[i] as T;
				}
			}
			return (T)((object)null);
		}

		public virtual List<Descriptor> GetSicknessSourceDescriptors()
		{
			return new List<Descriptor>();
		}

		public List<Descriptor> GetQualitativeDescriptors()
		{
			List<Descriptor> list = new List<Descriptor>();
			using (List<Sickness.InfectionVector>.Enumerator enumerator = this.infectionVectors.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					switch (enumerator.Current)
					{
					case Sickness.InfectionVector.Contact:
						list.Add(new Descriptor(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.SKINBORNE, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.SKINBORNE_TOOLTIP, Descriptor.DescriptorType.Information, false));
						break;
					case Sickness.InfectionVector.Digestion:
						list.Add(new Descriptor(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.FOODBORNE, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.FOODBORNE_TOOLTIP, Descriptor.DescriptorType.Information, false));
						break;
					case Sickness.InfectionVector.Inhalation:
						list.Add(new Descriptor(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.AIRBORNE, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.AIRBORNE_TOOLTIP, Descriptor.DescriptorType.Information, false));
						break;
					case Sickness.InfectionVector.Exposure:
						list.Add(new Descriptor(DUPLICANTS.DISEASES.DESCRIPTORS.INFO.SUNBORNE, DUPLICANTS.DISEASES.DESCRIPTORS.INFO.SUNBORNE_TOOLTIP, Descriptor.DescriptorType.Information, false));
						break;
					}
				}
			}
			list.Add(new Descriptor(Strings.Get(this.descriptiveSymptoms), string.Empty, Descriptor.DescriptorType.Information, false));
			return list;
		}

		private StringKey name;

		private StringKey descriptiveSymptoms;

		private float sicknessDuration = 600f;

		public float fatalityDuration;

		public HashedString id;

		public Sickness.SicknessType sicknessType;

		public Sickness.Severity severity;

		public List<Sickness.InfectionVector> infectionVectors;

		private List<Sickness.SicknessComponent> components = new List<Sickness.SicknessComponent>();

		public Amount amount;

		public Attribute amountDeltaAttribute;

		public Attribute cureSpeedBase;

		public abstract class SicknessComponent
		{
			public abstract object OnInfect(GameObject go, SicknessInstance diseaseInstance);

			public abstract void OnCure(GameObject go, object instance_data);

			public virtual List<Descriptor> GetSymptoms()
			{
				return null;
			}
		}

		public enum InfectionVector
		{
			Contact,
			Digestion,
			Inhalation,
			Exposure
		}

		public enum SicknessType
		{
			Pathogen,
			Ailment,
			Injury
		}

		public enum Severity
		{
			Benign,
			Minor,
			Major,
			Critical
		}
	}
}
