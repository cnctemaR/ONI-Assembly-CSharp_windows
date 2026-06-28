using System;
using UnityEngine;

namespace Klei.AI
{
	public abstract class AttributeModifierDisease : Disease
	{
		public AttributeModifierDisease(string id, bool use_custom_effect, AttributeModifier[] attribute_modifiers, float infection_probability, float sickness_duration, Disease.EffectProbabilityDelta[] effect_probability_deltas = null)
			: base(id, infection_probability, sickness_duration, effect_probability_deltas)
		{
			this.attributeModifiers = attribute_modifiers;
			this.useCustomEffect = use_custom_effect;
		}

		protected override object OnInfect(GameObject go)
		{
			Attributes attributes = go.GetAttributes();
			for (int i = 0; i < this.attributeModifiers.Length; i++)
			{
				AttributeModifier attributeModifier = this.attributeModifiers[i];
				attributes.Add(attributeModifier.AttributeId, attributeModifier);
			}
			KAnimControllerBase kanimControllerBase = null;
			if (!this.useCustomEffect)
			{
				kanimControllerBase = base.StartCommonSickEffect(go);
			}
			return kanimControllerBase;
		}

		protected override void OnCure(GameObject go, object instance_data)
		{
			if (!this.useCustomEffect)
			{
				KAnimControllerBase kanimControllerBase = (KAnimControllerBase)instance_data;
				kanimControllerBase.gameObject.DeleteObject();
			}
			Attributes attributes = go.GetAttributes();
			for (int i = 0; i < this.attributeModifiers.Length; i++)
			{
				AttributeModifier attributeModifier = this.attributeModifiers[i];
				attributes.Remove(attributeModifier);
			}
		}

		public AttributeModifier[] Modifers
		{
			get
			{
				return this.attributeModifiers;
			}
		}

		public override string GetSymptoms()
		{
			string text = base.GetSymptoms();
			foreach (AttributeModifier attributeModifier in this.attributeModifiers)
			{
				float value = attributeModifier.Value;
				text += "\n    ";
				text += Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + attributeModifier.AttributeId.ToUpper() + ".NAME");
				text = text + " " + ((value <= 0f) ? string.Empty : "+") + value.ToString();
			}
			return text;
		}

		private AttributeModifier[] attributeModifiers;

		private bool useCustomEffect;
	}
}
