using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class Attributes
	{
		public Attributes(GameObject game_object)
		{
			this.gameObject = game_object;
		}

		public IEnumerator<AttributeInstance> GetEnumerator()
		{
			return this.AttributeTable.GetEnumerator();
		}

		public int Count
		{
			get
			{
				return this.AttributeTable.Count;
			}
		}

		public AttributeInstance Add(Attribute attribute)
		{
			AttributeInstance attributeInstance = this.Get(attribute.Id);
			if (attributeInstance == null)
			{
				attributeInstance = new AttributeInstance(this.gameObject, attribute);
				this.AttributeTable.Add(attributeInstance);
			}
			return attributeInstance;
		}

		public void Add(AttributeModifier modifier)
		{
			AttributeInstance attributeInstance = this.Get(modifier.AttributeId);
			if (attributeInstance != null)
			{
				attributeInstance.Add(modifier);
			}
		}

		public float GetValuePercent(string attribute_id)
		{
			float num = 1f;
			AttributeInstance attributeInstance = this.Get(attribute_id);
			if (attributeInstance != null)
			{
				num = attributeInstance.GetTotalValue() / attributeInstance.GetBaseValue();
			}
			else
			{
				global::Debug.LogError("Could not find attribute " + attribute_id);
			}
			return num;
		}

		public AttributeInstance Get(string attribute_id)
		{
			for (int i = 0; i < this.AttributeTable.Count; i++)
			{
				if (this.AttributeTable[i].Id == attribute_id)
				{
					return this.AttributeTable[i];
				}
			}
			return null;
		}

		public AttributeInstance Get(Attribute attribute)
		{
			return this.Get(attribute.Id);
		}

		public float GetValue(string id)
		{
			float num = 0f;
			AttributeInstance attributeInstance = this.Get(id);
			if (attributeInstance != null)
			{
				num = attributeInstance.GetTotalValue();
			}
			else
			{
				global::Debug.LogError("Could not find attribute " + id);
			}
			return num;
		}

		public void Remove(AttributeModifier modifier)
		{
			if (modifier == null)
			{
				return;
			}
			AttributeInstance attributeInstance = this.Get(modifier.AttributeId);
			if (attributeInstance != null)
			{
				attributeInstance.Remove(modifier);
			}
		}

		public AttributeInstance GetProfession()
		{
			AttributeInstance attributeInstance = null;
			foreach (AttributeInstance attributeInstance2 in this)
			{
				if (attributeInstance2.modifier.IsProfession)
				{
					if (attributeInstance == null)
					{
						attributeInstance = attributeInstance2;
					}
					else if (attributeInstance.GetTotalValue() < attributeInstance2.GetTotalValue())
					{
						attributeInstance = attributeInstance2;
					}
				}
			}
			return attributeInstance;
		}

		public string GetProfessionString(bool longform = true)
		{
			AttributeInstance profession = this.GetProfession();
			if ((int)profession.GetTotalValue() == 0)
			{
				return string.Format((!longform) ? UI.ATTRIBUTELEVEL_SHORT : UI.ATTRIBUTELEVEL, 0, DUPLICANTS.ATTRIBUTES.UNPROFESSIONAL_NAME);
			}
			return string.Format((!longform) ? UI.ATTRIBUTELEVEL_SHORT : UI.ATTRIBUTELEVEL, (int)profession.GetTotalValue(), profession.modifier.ProfessionName);
		}

		public string GetProfessionDescriptionString()
		{
			AttributeInstance profession = this.GetProfession();
			if ((int)profession.GetTotalValue() == 0)
			{
				return DUPLICANTS.ATTRIBUTES.UNPROFESSIONAL_DESC;
			}
			return string.Format(DUPLICANTS.ATTRIBUTES.PROFESSION_DESC, profession.modifier.Name);
		}

		public List<AttributeInstance> AttributeTable = new List<AttributeInstance>();

		public GameObject gameObject;
	}
}
