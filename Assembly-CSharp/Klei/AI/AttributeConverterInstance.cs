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

		public string DescriptionFromAttribute()
		{
			float num = this.Evaluate();
			string text;
			if (this.converter.formatter != null)
			{
				text = this.converter.formatter.GetFormattedValue(num, this.converter.formatter.DeltaTimeSlice, base.gameObject);
			}
			else if (this.attributeInstance.Attribute.formatter != null)
			{
				text = this.attributeInstance.Attribute.formatter.GetFormattedValue(num, this.attributeInstance.Attribute.formatter.DeltaTimeSlice, base.gameObject);
			}
			else
			{
				text = GameUtil.GetFormattedSimple(num, GameUtil.TimeSlice.None, null);
			}
			if (text != null)
			{
				text = GameUtil.AddPositiveSign(text, num > 0f);
				return string.Format(this.converter.description, text);
			}
			return null;
		}

		public AttributeConverter converter;

		public AttributeInstance attributeInstance;
	}
}
