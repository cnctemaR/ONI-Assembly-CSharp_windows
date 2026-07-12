using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
	public class Attribute : Resource
	{
		public Attribute(string id, bool is_trainable, Attribute.Display show_in_ui, bool is_profession, float base_value = 0f, string uiSprite = null, string thoughtSprite = null, string uiFullColourSprite = null, string[] overrideDLCIDs = null)
			: base(id, null, null)
		{
			string text = "STRINGS.DUPLICANTS.ATTRIBUTES." + id.ToUpper();
			this.Name = Strings.Get(new StringKey(text + ".NAME"));
			this.ProfessionName = Strings.Get(new StringKey(text + ".NAME"));
			this.Description = Strings.Get(new StringKey(text + ".DESC"));
			this.IsTrainable = is_trainable;
			this.IsProfession = is_profession;
			this.ShowInUI = show_in_ui;
			this.BaseValue = base_value;
			this.formatter = Attribute.defaultFormatter;
			this.uiSprite = uiSprite;
			this.thoughtSprite = thoughtSprite;
			this.uiFullColourSprite = uiFullColourSprite;
			if (overrideDLCIDs != null)
			{
				this.DLCIds = overrideDLCIDs;
			}
		}

		public Attribute(string id, string name, string profession_name, string attribute_description, float base_value, Attribute.Display show_in_ui, bool is_trainable, string uiSprite = null, string thoughtSprite = null, string uiFullColourSprite = null)
			: base(id, name)
		{
			this.Description = attribute_description;
			this.ProfessionName = profession_name;
			this.BaseValue = base_value;
			this.ShowInUI = show_in_ui;
			this.IsTrainable = is_trainable;
			this.uiSprite = uiSprite;
			this.thoughtSprite = thoughtSprite;
			this.uiFullColourSprite = uiFullColourSprite;
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
			if (attributes != null)
			{
				return attributes.Get(this);
			}
			return null;
		}

		public string GetDescription(AttributeInstance instance)
		{
			return instance.GetDescription();
		}

		public string GetTooltip(AttributeInstance instance)
		{
			return this.formatter.GetTooltip(this, instance);
		}

		private static readonly StandardAttributeFormatter defaultFormatter = new StandardAttributeFormatter(GameUtil.UnitClass.SimpleFloat, GameUtil.TimeSlice.None);

		public string Description;

		public float BaseValue;

		public Attribute.Display ShowInUI;

		public bool IsTrainable;

		public bool IsProfession;

		public string ProfessionName;

		public List<AttributeConverter> converters = new List<AttributeConverter>();

		public string uiSprite;

		public string thoughtSprite;

		public string uiFullColourSprite;

		public string[] DLCIds = DlcManager.AVAILABLE_ALL_VERSIONS;

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
