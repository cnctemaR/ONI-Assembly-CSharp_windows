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

		public override string ToString()
		{
			float num = this.Evaluate();
			if (this.converter.isPercent)
			{
				num *= 100f;
			}
			string text = this.converter.description.Replace("$value", num.ToString("0.0"));
			return GameUtil.AddPositiveSign(text, num > 0f);
		}

		public AttributeConverter converter;

		public AttributeInstance attributeInstance;
	}
}
