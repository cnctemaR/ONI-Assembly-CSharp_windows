using System;
using UnityEngine;

namespace Klei.AI
{
	public class AttributeConverter : Resource
	{
		public AttributeConverter(string id, string name, string description, float multiplier, float base_value, Attribute attribute, IAttributeFormatter formatter = null)
			: base(id, name)
		{
			this.description = description;
			this.multiplier = multiplier;
			this.baseValue = base_value;
			this.attribute = attribute;
			this.formatter = formatter;
		}

		public AttributeConverterInstance Lookup(Component cmp)
		{
			return this.Lookup(cmp.gameObject);
		}

		public AttributeConverterInstance Lookup(GameObject go)
		{
			AttributeConverters component = go.GetComponent<AttributeConverters>();
			AttributeConverterInstance attributeConverterInstance;
			if (component != null)
			{
				attributeConverterInstance = component.Get(this);
			}
			else
			{
				attributeConverterInstance = null;
			}
			return attributeConverterInstance;
		}

		public string description;

		public float multiplier;

		public float baseValue;

		public Attribute attribute;

		public IAttributeFormatter formatter;
	}
}
