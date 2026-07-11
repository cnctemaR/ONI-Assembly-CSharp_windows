using System;
using UnityEngine;

namespace Klei.AI
{
	public class AttributeConverterInstance : ModifierInstance<AttributeConverter>
	{
		public AttributeConverterInstance(GameObject game_object, AttributeConverter converter, AttributeInstance attribute_instance)
			: base(game_object, converter)
		{
			this.converter = converter;
			this.attributeInstance = attribute_instance;
		}

		public float Evaluate()
		{
			return this.converter.multiplier * this.attributeInstance.GetTotalValue() + this.converter.baseValue;
		}

		public string DescriptionFromAttribute(float value, GameObject go)
		{
			return this.converter.DescriptionFromAttribute(this.Evaluate(), go);
		}

		public AttributeConverter converter;

		public AttributeInstance attributeInstance;
	}
}
