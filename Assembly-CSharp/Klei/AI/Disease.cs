using System;
using System.Diagnostics;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	[DebuggerDisplay("{base.Id}")]
	public abstract class Disease : Resource
	{
		public Disease(string id, float infection_probability, float sickness_duration, Disease.EffectProbabilityDelta[] effect_probability_deltas = null)
			: base(id, null, null)
		{
			this.name = new StringKey("STRINGS.DUPLICANTS.DISEASES." + id.ToUpper() + ".NAME");
			this.sicknessDuration = sickness_duration;
			this.infectionProbability = infection_probability;
			this.effectProbabilityDeltas = effect_probability_deltas;
		}

		public new string Name
		{
			get
			{
				return this.name;
			}
		}

		public float SicknessDuration
		{
			get
			{
				return this.sicknessDuration;
			}
		}

		public bool ShouldInfect(GameObject infectee, float exposure_count)
		{
			float num = 0f;
			if (this.effectProbabilityDeltas != null)
			{
				Effects component = infectee.GetComponent<Effects>();
				foreach (Disease.EffectProbabilityDelta effectProbabilityDelta in this.effectProbabilityDeltas)
				{
					if (component.Has(effectProbabilityDelta.effectID))
					{
						num += effectProbabilityDelta.probabilityDelta;
					}
				}
			}
			DiseaseImmunity component2 = infectee.GetComponent<DiseaseImmunity>();
			float num2 = component2.ExposureModifier * exposure_count * (this.infectionProbability + num);
			float value = global::UnityEngine.Random.value;
			return value <= num2;
		}

		public object Infect(GameObject go, DiseaseExposureInfo exposure_info)
		{
			return this.OnInfect(go);
		}

		public void Cure(GameObject go, object instance_data)
		{
			this.OnCure(go, instance_data);
		}

		protected abstract object OnInfect(GameObject go);

		protected abstract void OnCure(GameObject go, object instance_data);

		public virtual string GetSymptoms()
		{
			return Strings.Get("STRINGS.DUPLICANTS.DISEASES." + this.Id.ToUpper() + ".DESCRIPTION");
		}

		public virtual string InfectionSourceString()
		{
			return DUPLICANTS.DISEASES.INFECTIONSOURCES.FOOD.text;
		}

		protected KAnimControllerBase StartCommonSickEffect(GameObject parent_go)
		{
			KBatchedAnimController kbatchedAnimController = FXHelpers.CreateEffect("contaminated_crew_fx_kanim", parent_go.transform.position + new Vector3(0f, 0f, -0.1f), parent_go.transform, true, Grid.SceneLayer.Front);
			kbatchedAnimController.Play("fx_loop", KAnim.PlayMode.Loop, 1f, 0f);
			return kbatchedAnimController;
		}

		private StringKey name;

		private Disease.EffectProbabilityDelta[] effectProbabilityDeltas;

		private float sicknessDuration = 600f;

		private float infectionProbability = 1f;

		public struct EffectProbabilityDelta
		{
			public string effectID;

			public float probabilityDelta;
		}
	}
}
