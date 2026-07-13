using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class AttributeModifierSickness : Sickness.SicknessComponent
	{
		public AttributeModifierSickness(Tag minionModel, AttributeModifier[] attribute_modifiers)
		{
			this.GetAttributeModifierForMinionModel[minionModel] = attribute_modifiers;
			this.attributeModifiers = new AttributeModifier[0];
		}

		public AttributeModifierSickness(AttributeModifier[] attribute_modifiers)
		{
			this.attributeModifiers = attribute_modifiers;
		}

		public override object OnInfect(GameObject go, SicknessInstance diseaseInstance)
		{
			Attributes attributes = go.GetAttributes();
			Tag tag = go.PrefabID();
			if (this.GetAttributeModifierForMinionModel.ContainsKey(tag))
			{
				for (int i = 0; i < this.GetAttributeModifierForMinionModel[tag].Length; i++)
				{
					AttributeModifier attributeModifier = this.GetAttributeModifierForMinionModel[tag][i];
					attributes.Add(attributeModifier);
				}
			}
			for (int j = 0; j < this.attributeModifiers.Length; j++)
			{
				AttributeModifier attributeModifier2 = this.attributeModifiers[j];
				attributes.Add(attributeModifier2);
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

		public override List<Descriptor> GetSymptoms(GameObject victim)
		{
			if (victim == null)
			{
				return this.GetSymptoms();
			}
			List<Descriptor> list = new List<Descriptor>();
			Tag tag = victim.PrefabID();
			if (this.GetAttributeModifierForMinionModel.ContainsKey(tag))
			{
				foreach (AttributeModifier attributeModifier in this.GetAttributeModifierForMinionModel[tag])
				{
					Attribute attribute = Db.Get().Attributes.Get(attributeModifier.AttributeId);
					list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.ATTRIBUTE_MODIFIER_SYMPTOMS, attribute.Name, attributeModifier.GetFormattedString()), string.Format(DUPLICANTS.DISEASES.ATTRIBUTE_MODIFIER_SYMPTOMS_TOOLTIP, attribute.Name, attributeModifier.GetFormattedString()), Descriptor.DescriptorType.Symptom, false));
				}
			}
			foreach (AttributeModifier attributeModifier2 in this.attributeModifiers)
			{
				Attribute attribute2 = Db.Get().Attributes.Get(attributeModifier2.AttributeId);
				list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.ATTRIBUTE_MODIFIER_SYMPTOMS, attribute2.Name, attributeModifier2.GetFormattedString()), string.Format(DUPLICANTS.DISEASES.ATTRIBUTE_MODIFIER_SYMPTOMS_TOOLTIP, attribute2.Name, attributeModifier2.GetFormattedString()), Descriptor.DescriptorType.Symptom, false));
			}
			return list;
		}

		public override List<Descriptor> GetSymptoms()
		{
			List<Descriptor> list = new List<Descriptor>();
			foreach (Tag tag in this.GetAttributeModifierForMinionModel.Keys)
			{
				string properName = Assets.GetPrefab(tag).GetProperName();
				foreach (AttributeModifier attributeModifier in this.GetAttributeModifierForMinionModel[tag])
				{
					Attribute attribute = Db.Get().Attributes.Get(attributeModifier.AttributeId);
					list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.ATTRIBUTE_BY_MODEL_MODIFIER_SYMPTOMS, properName, attribute.Name, attributeModifier.GetFormattedString()), string.Format(DUPLICANTS.DISEASES.ATTRIBUTE_MODIFIER_SYMPTOMS_TOOLTIP, attribute.Name, attributeModifier.GetFormattedString()), Descriptor.DescriptorType.Symptom, false));
				}
			}
			foreach (AttributeModifier attributeModifier2 in this.attributeModifiers)
			{
				Attribute attribute2 = Db.Get().Attributes.Get(attributeModifier2.AttributeId);
				list.Add(new Descriptor(string.Format(DUPLICANTS.DISEASES.ATTRIBUTE_MODIFIER_SYMPTOMS, attribute2.Name, attributeModifier2.GetFormattedString()), string.Format(DUPLICANTS.DISEASES.ATTRIBUTE_MODIFIER_SYMPTOMS_TOOLTIP, attribute2.Name, attributeModifier2.GetFormattedString()), Descriptor.DescriptorType.Symptom, false));
			}
			return list;
		}

		private Dictionary<Tag, AttributeModifier[]> GetAttributeModifierForMinionModel = new Dictionary<Tag, AttributeModifier[]>();

		private AttributeModifier[] attributeModifiers;
	}
}
