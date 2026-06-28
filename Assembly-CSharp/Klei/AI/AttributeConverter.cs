using System;
using UnityEngine;

namespace Klei.AI
{
	public class AttributeConverter : Resource
	{
		public AttributeConverter(string id, string name, string description, float multiplier, float base_value, Attribute attribute, bool is_percent)
			: base(id, name)
		{
			this.description = description;
			this.multiplier = multiplier;
			this.baseValue = base_value;
			this.attribute = attribute;
			this.isPercent = is_percent;
		}

		public AttributeConverterInstance Lookup(Component cmp)
		{
			return this.Lookup(cmp.gameObject);
		}

		public AttributeConverterInstance Lookup(GameObject go)
		{
			AttributeConverters component = go.GetComponent<AttributeConverters>();
			if (component != null)
			{
				return component.Get(this);
			}
			return null;
		}

		public string description;

		public float multiplier;

		public float baseValue;

		public Attribute attribute;

		public bool isPercent;
	}
}
