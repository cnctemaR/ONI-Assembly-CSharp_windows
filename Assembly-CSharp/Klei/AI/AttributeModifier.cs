using System;
using System.Diagnostics;

namespace Klei.AI
{
	[DebuggerDisplay("{AttributeId}")]
	public class AttributeModifier
	{
		public string AttributeId { get; private set; }

		public float Value { get; private set; }

		public bool IsMultiplier { get; private set; }

		public bool UIOnly { get; private set; }

		public bool IsReadonly { get; private set; }

		public AttributeModifier(string attribute_id, float value, string description = null, bool is_multiplier = false, bool uiOnly = false, bool is_readonly = true)
		{
			this.AttributeId = attribute_id;
			this.Value = value;
			this.Description = ((description == null) ? attribute_id : description);
			this.DescriptionCB = null;
			this.IsMultiplier = is_multiplier;
			this.UIOnly = uiOnly;
			this.IsReadonly = is_readonly;
		}

		public AttributeModifier(string attribute_id, float value, Func<string> description_cb, bool is_multiplier = false, bool uiOnly = false)
		{
			this.AttributeId = attribute_id;
			this.Value = value;
			this.DescriptionCB = description_cb;
			this.Description = null;
			this.IsMultiplier = is_multiplier;
			this.UIOnly = uiOnly;
			if (description_cb == null)
			{
				global::Debug.LogWarning("AttributeModifier being constructed without a description callback: " + attribute_id);
			}
		}

		public void SetValue(float value)
		{
			this.Value = value;
		}

		public string GetName()
		{
			Attribute attribute = Db.Get().Attributes.TryGet(this.AttributeId);
			if (attribute != null && attribute.ShowInUI != Attribute.Display.Never)
			{
				return attribute.Name;
			}
			return "";
		}

		public string GetDescription()
		{
			if (this.DescriptionCB == null)
			{
				return this.Description;
			}
			return this.DescriptionCB();
		}

		public string GetFormattedString()
		{
			IAttributeFormatter attributeFormatter = null;
			Attribute attribute = Db.Get().Attributes.TryGet(this.AttributeId);
			if (!this.IsMultiplier)
			{
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
					else
					{
						attribute = Db.Get().PlantAttributes.TryGet(this.AttributeId);
						if (attribute != null)
						{
							attributeFormatter = attribute.formatter;
						}
					}
				}
			}
			string text = "";
			if (attributeFormatter != null)
			{
				text = attributeFormatter.GetFormattedModifier(this);
			}
			else if (this.IsMultiplier)
			{
				text += GameUtil.GetFormattedPercent(this.Value * 100f, GameUtil.TimeSlice.None);
			}
			else
			{
				text += GameUtil.GetFormattedSimple(this.Value, GameUtil.TimeSlice.None, null);
			}
			if (text != null && text.Length > 0 && text[0] != '-')
			{
				text = GameUtil.AddPositiveSign(text, this.Value > 0f);
			}
			return text;
		}

		public AttributeModifier Clone()
		{
			return new AttributeModifier(this.AttributeId, this.Value, this.Description, false, false, true);
		}

		public string Description;

		public Func<string> DescriptionCB;
	}
}
