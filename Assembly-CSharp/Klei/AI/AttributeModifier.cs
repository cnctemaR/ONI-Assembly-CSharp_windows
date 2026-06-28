using System;
using System.Diagnostics;
using UnityEngine;

namespace Klei.AI
{
	[DebuggerDisplay("{AttributeId}")]
	public class AttributeModifier
	{
		public AttributeModifier(string attribute_id, float value, string description = null, bool is_multiplier = false, bool uiOnly = false)
		{
			this.AttributeId = attribute_id;
			this.Value = value;
			this.Description = string.Intern((description != null) ? description : string.Empty);
			this.IsMultiplier = is_multiplier;
			this.UIOnly = uiOnly;
		}

		public void SetValue(float value)
		{
			this.Value = value;
		}

		public string GetFormattedString(GameObject parent_instance)
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
			string text = ((attributeFormatter == null) ? GameUtil.GetFormattedSimple(this.Value, GameUtil.TimeSlice.None, null) : attributeFormatter.GetFormattedModifier(this, parent_instance));
			if (text != null)
			{
				GameUtil.AddPositiveSign(text, this.Value > 0f);
			}
			return text;
		}

		public AttributeModifier Clone()
		{
			return new AttributeModifier(this.AttributeId, this.Value, this.Description, false, false);
		}

		public string AttributeId;

		public float Value;

		public string Description;

		public bool IsMultiplier;

		public bool UIOnly;
	}
}
