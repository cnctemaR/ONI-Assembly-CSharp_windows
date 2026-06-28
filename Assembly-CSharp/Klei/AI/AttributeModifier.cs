using System;

namespace Klei.AI
{
	public class AttributeModifier
	{
		public AttributeModifier(string attribute_id, float value, string description = null, bool is_multiplier = false)
		{
			this.AttributeId = attribute_id;
			this.Value = value;
			this.Description = string.Intern((description != null) ? description : string.Empty);
			this.IsMultiplier = is_multiplier;
		}

		public void SetValue(float value)
		{
			this.Value = value;
		}

		public string GetFormattedString()
		{
			IAttributeFormatter attributeFormatter = null;
			Attribute attribute = Db.Get().Attributes.TryGet(this.AttributeId);
			if (attribute != null)
			{
				attributeFormatter = attribute.formatter;
			}
			else
			{
				attribute = Db.Get().BuildingAttributes.TryGet(this.AttributeId);
				if (attribute != null)
				{
					attributeFormatter = attribute.formatter;
				}
			}
			if (attributeFormatter != null)
			{
				return GameUtil.AddPositiveSign(attributeFormatter.GetFormattedModifier(this), this.Value > 0f);
			}
			return GameUtil.AddPositiveSign(GameUtil.GetFormattedSimple(this.Value, GameUtil.TimeSlice.None), this.Value > 0f);
		}

		public AttributeModifier Clone()
		{
			return new AttributeModifier(this.AttributeId, this.Value, this.Description, false);
		}

		public string AttributeId;

		public float Value;

		public string Description;

		public bool IsMultiplier;
	}
}
