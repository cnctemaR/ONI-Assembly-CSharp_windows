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

		public GameUtil.TimeSlice? OverrideTimeSlice { get; set; }

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
			this.OverrideTimeSlice = null;
		}

		public AttributeModifier(string attribute_id, float value, Func<string> description_cb, bool is_multiplier = false, bool uiOnly = false)
			: this(attribute_id, value, null, description_cb, is_multiplier, uiOnly)
		{
		}

		public AttributeModifier(string attribute_id, float value, Func<string> name_cb, Func<string> description_cb, bool is_multiplier = false, bool uiOnly = false)
		{
			this.AttributeId = attribute_id;
			this.Value = value;
			this.NameCB = name_cb;
			this.DescriptionCB = description_cb;
			this.Description = null;
			this.IsMultiplier = is_multiplier;
			this.UIOnly = uiOnly;
			this.OverrideTimeSlice = null;
			if (description_cb == null)
			{
				global::Debug.LogWarning("AttributeModifier being constructed without a description callback: " + attribute_id);
			}
		}

		public void Reconstruct(string attribute_id, float value, string description = null, bool is_multiplier = false, bool uiOnly = false, bool is_readonly = true)
		{
			this.AttributeId = attribute_id;
			this.Value = value;
			this.Description = ((description == null) ? attribute_id : description);
			this.DescriptionCB = null;
			this.NameCB = null;
			this.IsMultiplier = is_multiplier;
			this.UIOnly = uiOnly;
			this.IsReadonly = is_readonly;
			this.OverrideTimeSlice = null;
		}

		public void SetValue(float value)
		{
			this.Value = value;
		}

		public static Attribute FetchAttribute(string attributeId)
		{
			Attribute attribute = Db.Get().Attributes.TryGet(attributeId);
			if (attribute != null)
			{
				return attribute;
			}
			Attribute attribute2 = Db.Get().BuildingAttributes.TryGet(attributeId);
			if (attribute2 != null)
			{
				return attribute2;
			}
			Attribute attribute3 = Db.Get().PlantAttributes.TryGet(attributeId);
			if (attribute3 != null)
			{
				return attribute3;
			}
			Attribute attribute4 = Db.Get().CritterAttributes.TryGet(attributeId);
			if (attribute4 != null)
			{
				return attribute4;
			}
			return null;
		}

		private Attribute FetchAttribute()
		{
			return AttributeModifier.FetchAttribute(this.AttributeId);
		}

		public string GetName()
		{
			Attribute attribute = this.FetchAttribute();
			if (attribute == null || attribute.ShowInUI == Attribute.Display.Never)
			{
				return "";
			}
			if (this.NameCB != null)
			{
				return this.NameCB();
			}
			return attribute.Name;
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
			Attribute attribute = this.FetchAttribute();
			IAttributeFormatter attributeFormatter = ((!this.IsMultiplier && attribute != null) ? attribute.formatter : null);
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
				GameUtil.TimeSlice? overrideTimeSlice = this.OverrideTimeSlice;
				GameUtil.TimeSlice timeSlice = GameUtil.TimeSlice.None;
				if (!((overrideTimeSlice.GetValueOrDefault() == timeSlice) & (overrideTimeSlice != null)))
				{
					text = GameUtil.AddPositiveSign(text, this.Value > 0f);
				}
			}
			return text;
		}

		public AttributeModifier Clone()
		{
			return new AttributeModifier(this.AttributeId, this.Value, this.Description, false, false, true);
		}

		public Func<string> NameCB;

		public string Description;

		public Func<string> DescriptionCB;
	}
}
