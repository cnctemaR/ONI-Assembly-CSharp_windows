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
			this.Attribute = attribute;
		}

		public IEnumerator<AttributeInstance.AttributeModifierEntry> GetEnumerator()
		{
			return this.Modifiers.GetEnumerator();
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

		public float GetSkillLevel()
		{
			float num = 0f;
			for (int i = 0; i < this.Modifiers.Count; i++)
			{
				AttributeInstance.AttributeModifierEntry attributeModifierEntry = this.Modifiers[i];
				if (attributeModifierEntry.Name == "Skill Level")
				{
					num = attributeModifierEntry.Modifier.Value;
					break;
				}
			}
			return num;
		}

		public float GetTotalDisplayValue()
		{
			float num = this.Attribute.BaseValue;
			float num2 = 0f;
			for (int i = 0; i < this.Modifiers.Count; i++)
			{
				AttributeModifier modifier = this.Modifiers[i].Modifier;
				if (!modifier.IsMultiplier)
				{
					num += modifier.Value;
				}
				else
				{
					num2 += modifier.Value;
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
			for (int i = 0; i < this.Modifiers.Count; i++)
			{
				AttributeModifier modifier = this.Modifiers[i].Modifier;
				if (!modifier.UIOnly)
				{
					if (!modifier.IsMultiplier)
					{
						num += modifier.Value;
					}
					else
					{
						num2 += modifier.Value;
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
			for (int i = 0; i < this.Modifiers.Count; i++)
			{
				AttributeModifier modifier = this.Modifiers[i].Modifier;
				if (!modifier.IsMultiplier)
				{
					num += modifier.Value;
				}
			}
			return num * testModifier.Value;
		}

		public float GetPercentOfBase()
		{
			return this.GetTotalValue() / this.GetBaseValue();
		}

		public void Add(string name, AttributeModifier modifier)
		{
			AttributeInstance.AttributeModifierEntry attributeModifierEntry = new AttributeInstance.AttributeModifierEntry();
			attributeModifierEntry.Name = string.Intern(name);
			attributeModifierEntry.Modifier = modifier;
			this.Modifiers.Add(attributeModifierEntry);
			if (this.OnDirty != null)
			{
				this.OnDirty();
			}
		}

		public void Remove(AttributeModifier modifier)
		{
			for (int i = 0; i < this.Modifiers.Count; i++)
			{
				if (this.Modifiers[i].Modifier == modifier)
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

		public string GetFormattedValue()
		{
			IAttributeFormatter formatter = this.Attribute.formatter;
			if (formatter != null)
			{
				return formatter.GetFormattedAttribute(this);
			}
			return GameUtil.GetFormattedSimple(this.GetTotalValue(), GameUtil.TimeSlice.None, null);
		}

		public string GetAttributeValueTooltip()
		{
			string text = string.Format(DUPLICANTS.ATTRIBUTES.VALUE, this.Name, this.GetFormattedValue());
			if (this.GetBaseValue() != 0f)
			{
				text += string.Format(DUPLICANTS.ATTRIBUTES.BASE_VALUE, this.GetBaseValue());
			}
			foreach (AttributeInstance.AttributeModifierEntry attributeModifierEntry in this.Modifiers)
			{
				string formattedString = attributeModifierEntry.Modifier.GetFormattedString(this.gameObject);
				if (formattedString != null)
				{
					text += string.Format(DUPLICANTS.ATTRIBUTES.MODIFIER_ENTRY, attributeModifierEntry.Modifier.Description, formattedString);
				}
			}
			string text2 = string.Empty;
			AttributeConverters component = this.gameObject.GetComponent<AttributeConverters>();
			if (component != null && this.Attribute.converters.Count > 0)
			{
				foreach (AttributeConverterInstance attributeConverterInstance in this.gameObject.GetComponent<AttributeConverters>())
				{
					if (attributeConverterInstance.converter.attribute == this.Attribute)
					{
						string text3 = attributeConverterInstance.ToString();
						if (text3 != null)
						{
							text2 = text2 + "\n" + text3;
						}
					}
				}
			}
			if (text2.Length > 0)
			{
				text = text + "\n" + text2;
			}
			return text;
		}

		public Attribute Attribute;

		public global::System.Action OnDirty;

		public List<AttributeInstance.AttributeModifierEntry> Modifiers = new List<AttributeInstance.AttributeModifierEntry>();

		public class AttributeModifierEntry
		{
			public string Name;

			public AttributeModifier Modifier;
		}
	}
}
