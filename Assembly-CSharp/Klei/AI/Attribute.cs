using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
	public class Attribute : Resource
	{
		public Attribute(string id, bool is_trainable, Attribute.Display show_in_ui, bool is_profession)
			: base(id, null, null)
		{
			string text = "STRINGS.DUPLICANTS.ATTRIBUTES." + id.ToUpper();
			this.Name = Strings.Get(new StringKey(text + ".NAME"));
			this.ProfessionName = Strings.Get(new StringKey(text + ".PROFESSION_NAME"));
			this.Description = Strings.Get(new StringKey(text + ".DESC"));
			this.IsTrainable = is_trainable;
			this.IsProfession = is_profession;
			this.ShowInUI = show_in_ui;
		}

		public Attribute(string id, string name, string profession_name, string attribute_description, float base_value, Attribute.Display show_in_ui, bool is_trainable)
			: base(id, name)
		{
			this.Description = attribute_description;
			this.ProfessionName = profession_name;
			this.BaseValue = base_value;
			this.ShowInUI = show_in_ui;
			this.IsTrainable = is_trainable;
			if (this.ProfessionName == "")
			{
				this.ProfessionName = null;
			}
		}

		public void SetFormatter(IAttributeFormatter formatter)
		{
			this.formatter = formatter;
		}

		public AttributeInstance Lookup(Component cmp)
		{
			return this.Lookup(cmp.gameObject);
		}

		public AttributeInstance Lookup(GameObject go)
		{
			Attributes attributes = go.GetAttributes();
			AttributeInstance attributeInstance;
			if (attributes != null)
			{
				attributeInstance = attributes.Get(this);
			}
			else
			{
				attributeInstance = null;
			}
			return attributeInstance;
		}

		public string Description;

		public float BaseValue;

		public Attribute.Display ShowInUI;

		public bool IsTrainable;

		public bool IsProfession;

		public string ProfessionName;

		public List<AttributeConverter> converters = new List<AttributeConverter>();

		public IAttributeFormatter formatter;

		public enum Display
		{
			Normal,
			Skill,
			Expectation,
			General,
			Details,
			Never
		}
	}
}
