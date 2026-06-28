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
			return this.AttributeTable.Values.GetEnumerator();
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
			AttributeInstance attributeInstance = null;
			if (!this.AttributeTable.TryGetValue(attribute.Id, out attributeInstance))
			{
				attributeInstance = new AttributeInstance(this.gameObject, attribute);
				this.AttributeTable[attribute.Id] = attributeInstance;
			}
			return attributeInstance;
		}

		public void Add(string id, AttributeModifier modifier)
		{
			foreach (KeyValuePair<string, AttributeInstance> keyValuePair in this.AttributeTable)
			{
				if (keyValuePair.Key == modifier.AttributeId)
				{
					keyValuePair.Value.Add(id, modifier);
					break;
				}
			}
		}

		public float GetInverseValuePercent(string attribute_id)
		{
			return 1f / this.GetValuePercent(attribute_id);
		}

		public float GetValuePercent(string attribute_id)
		{
			float num = 1f;
			AttributeInstance attributeInstance = null;
			if (this.AttributeTable.TryGetValue(attribute_id, out attributeInstance))
			{
				num = attributeInstance.GetTotalValue() / attributeInstance.GetBaseValue();
			}
			else
			{
				Debug.LogError("Could not find attribute " + attribute_id);
			}
			return num;
		}

		public AttributeInstance Get(string attribute_id)
		{
			AttributeInstance attributeInstance = null;
			this.AttributeTable.TryGetValue(attribute_id, out attributeInstance);
			return attributeInstance;
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
				Debug.LogError("Could not find attribute " + id);
			}
			return num;
		}

		public void Remove(AttributeModifier modifier)
		{
			if (modifier == null)
			{
				return;
			}
			foreach (KeyValuePair<string, AttributeInstance> keyValuePair in this.AttributeTable)
			{
				if (keyValuePair.Key == modifier.AttributeId)
				{
					keyValuePair.Value.Remove(modifier);
					break;
				}
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

		public string GetProfessionString()
		{
			AttributeInstance profession = this.GetProfession();
			if ((int)profession.GetTotalValue() == 0)
			{
				return string.Format(UI.ATTRIBUTELEVEL, 0, DUPLICANTS.ATTRIBUTES.UNPROFESSIONAL_NAME);
			}
			return string.Format(UI.ATTRIBUTELEVEL, (int)profession.GetTotalValue(), profession.modifier.ProfessionName);
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

		public Dictionary<string, AttributeInstance> AttributeTable = new Dictionary<string, AttributeInstance>();

		public GameObject gameObject;
	}
}
