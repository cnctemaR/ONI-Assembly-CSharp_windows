using System;
using System.Collections.Generic;
using System.Diagnostics;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	[DebuggerDisplay("{Attribute.Id}")]
	public class AttributeInstance : ModifierInstance<Attribute>
	{
		public AttributeInstance(GameObject game_object, Attribute attribute)
			: base(game_object, attribute)
		{
			DebugUtil.Assert(attribute != null, "Assert!");
			this.Attribute = attribute;
		}

		public string Id
		{
			get
			{
				return this.Attribute.Id;
			}
		}

		public string Name
		{
			get
			{
				return this.Attribute.Name;
			}
		}

		public string Description
		{
			get
			{
				return this.Attribute.Description;
			}
		}

		public float GetBaseValue()
		{
			return this.Attribute.BaseValue;
		}

		public float GetTotalDisplayValue()
		{
			float num = this.Attribute.BaseValue;
			float num2 = 0f;
			foreach (AttributeModifier attributeModifier in this.Modifiers)
			{
				if (!attributeModifier.IsMultiplier)
				{
					num += attributeModifier.Value;
				}
				else
				{
					num2 += attributeModifier.Value;
				}
			}
			if (num2 != 0f)
			{
				num += Mathf.Abs(num) * num2;
			}
			return num;
		}

		public float GetTotalValue()
		{
			float num = this.Attribute.BaseValue;
			float num2 = 0f;
			foreach (AttributeModifier attributeModifier in this.Modifiers)
			{
				if (!attributeModifier.UIOnly)
				{
					if (!attributeModifier.IsMultiplier)
					{
						num += attributeModifier.Value;
					}
					else
					{
						num2 += attributeModifier.Value;
					}
				}
			}
			if (num2 != 0f)
			{
				num += Mathf.Abs(num) * num2;
			}
			return num;
		}

		public float GetModifierContribution(AttributeModifier testModifier)
		{
			if (!testModifier.IsMultiplier)
			{
				return testModifier.Value;
			}
			float num = this.Attribute.BaseValue;
			foreach (AttributeModifier attributeModifier in this.Modifiers)
			{
				if (!attributeModifier.IsMultiplier)
				{
					num += attributeModifier.Value;
				}
			}
			return num * testModifier.Value;
		}

		public void Add(AttributeModifier modifier)
		{
			this.Modifiers.Add(modifier);
			if (this.OnDirty != null)
			{
				this.OnDirty();
			}
		}

		public void Remove(AttributeModifier modifier)
		{
			for (int i = 0; i < this.Modifiers.Count; i++)
			{
				if (this.Modifiers[i] == modifier)
				{
					this.Modifiers.RemoveAt(i);
					if (this.OnDirty != null)
					{
						this.OnDirty();
					}
					break;
				}
			}
		}

		public void ClearModifiers()
		{
			if (this.Modifiers.Count > 0)
			{
				this.Modifiers.Clear();
				if (this.OnDirty != null)
				{
					this.OnDirty();
				}
			}
		}

		public string GetDescription()
		{
			return string.Format(DUPLICANTS.ATTRIBUTES.VALUE, this.Name, this.GetFormattedValue());
		}

		public string GetFormattedValue()
		{
			return this.Attribute.formatter.GetFormattedAttribute(this);
		}

		public string GetAttributeValueTooltip()
		{
			return this.Attribute.GetTooltip(this);
		}

		public Attribute Attribute;

		public global::System.Action OnDirty;

		public List<AttributeModifier> Modifiers = new List<AttributeModifier>();

		public bool hide;
	}
}
