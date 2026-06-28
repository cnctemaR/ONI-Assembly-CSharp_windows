using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
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

		public float GetTotalValue()
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
			return num + num * num2;
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

		public string GetAttributeValueTooltip()
		{
			string text = this.Name + " " + this.GetTotalValue();
			text += "\n";
			if (this.GetBaseValue() != 0f)
			{
				text = text + "\nBase " + this.GetBaseValue();
			}
			foreach (AttributeInstance.AttributeModifierEntry attributeModifierEntry in this.Modifiers)
			{
				text = text + "\n" + string.Format("{0}: {1}", attributeModifierEntry.Modifier.Description, attributeModifierEntry.Modifier.GetFormattedString());
			}
			AttributeConverters component = this.gameObject.GetComponent<AttributeConverters>();
			if (component != null && this.Attribute.converters.Count > 0)
			{
				text += "\n";
				foreach (AttributeConverterInstance attributeConverterInstance in this.gameObject.GetComponent<AttributeConverters>())
				{
					if (attributeConverterInstance.converter.attribute == this.Attribute)
					{
						float num = attributeConverterInstance.Evaluate();
						if (attributeConverterInstance.converter.isPercent)
						{
							num *= 100f;
						}
						string text2 = attributeConverterInstance.converter.description.Replace("$value", num.ToString());
						text2 = GameUtil.AddPositiveSign(text2, num > 0f);
						text = text + "\n" + text2;
					}
				}
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
