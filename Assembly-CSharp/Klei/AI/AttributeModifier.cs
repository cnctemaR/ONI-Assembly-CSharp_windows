using System;
using System.Diagnostics;
using UnityEngine;

namespace Klei.AI
{
	[DebuggerDisplay("{AttributeId}")]
	public class AttributeModifier
	{
		public AttributeModifier(string attribute_id, float value, string description = null, bool is_multiplier = false, bool uiOnly = false, bool is_readonly = true)
		{
			this.AttributeId = attribute_id;
			this.Value = value;
			this.Description = string.Intern((description != null) ? description : string.Empty);
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
		}

		public string AttributeId { get; private set; }

		public float Value { get; private set; }

		public bool IsMultiplier { get; private set; }

		public bool UIOnly { get; private set; }

		public bool IsReadonly { get; private set; }

		public void SetValue(float value)
		{
			DebugUtil.Assert(!this.IsReadonly, "Assert!");
			this.Value = value;
			if (this.OnDirty != null)
			{
				this.OnDirty();
			}
		}

		public string GetDescription()
		{
			return (this.DescriptionCB == null) ? this.Description : this.DescriptionCB();
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
			string text = string.Empty;
			if (attributeFormatter != null)
			{
				text = attributeFormatter.GetFormattedModifier(this, parent_instance);
			}
			else if (this.IsMultiplier)
			{
				text += GameUtil.GetFormattedPercent(this.Value * 100f, GameUtil.TimeSlice.None);
			}
			else
			{
				text += GameUtil.GetFormattedSimple(this.Value, GameUtil.TimeSlice.None, null);
			}
			if (text != null)
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

		public global::System.Action OnDirty;
	}
}
