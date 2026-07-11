using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class AttributeModifierDisease : Disease.DiseaseComponent
	{
		public AttributeModifierDisease(AttributeModifier[] attribute_modifiers)
		{
			this.attributeModifiers = attribute_modifiers;
		}

		public override object OnInfect(GameObject go, DiseaseInstance diseaseInstance)
		{
			Attributes attributes = go.GetAttributes();
			for (int i = 0; i < this.attributeModifiers.Length; i++)
			{
				AttributeModifier attributeModifier = this.attributeModifiers[i];
				attributes.Add(attributeModifier);
			}
			return null;
		}

		public override void OnCure(GameObject go, object instance_data)
		{
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

		public override List<Descriptor> GetSymptoms()
		{
			List<Descriptor> list = new List<Descriptor>();
			foreach (AttributeModifier attributeModifier in this.attributeModifiers)
			{
				Attribute attribute = Db.Get().Attributes.Get(attributeModifier.AttributeId);
				list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.ATTRIBUTE_MODIFIER_SYMPTOMS, attribute.Name, attributeModifier.GetFormattedString(null)), string.Format(DUPLICANTS.DISEASES.ATTRIBUTE_MODIFIER_SYMPTOMS_TOOLTIP, attribute.Name, attributeModifier.GetFormattedString(null)), Descriptor.DescriptorType.Symptom, false));
			}
			return list;
		}

		private AttributeModifier[] attributeModifiers;
	}
}
