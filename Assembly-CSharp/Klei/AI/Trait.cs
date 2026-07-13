using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class Trait : Modifier
	{
		public Trait(string id, string name, string description, float rating, bool should_save, ChoreGroup[] disallowed_chore_groups, bool positive_trait, bool is_valid_starter_trait)
			: base(id, name, description)
		{
			this.Rating = rating;
			this.ShouldSave = should_save;
			this.disabledChoreGroups = disallowed_chore_groups;
			this.PositiveTrait = positive_trait;
			this.ValidStarterTrait = is_valid_starter_trait;
			this.ignoredEffects = new string[0];
			this.requiredDlcIds = null;
			this.forbiddenDlcIds = null;
		}

		public Trait(string id, string name, string description, float rating, bool should_save, ChoreGroup[] disallowed_chore_groups, bool positive_trait, bool is_valid_starter_trait, string[] requiredDlcIds, string[] forbiddenDlcIds)
			: base(id, name, description)
		{
			this.Rating = rating;
			this.ShouldSave = should_save;
			this.disabledChoreGroups = disallowed_chore_groups;
			this.PositiveTrait = positive_trait;
			this.ValidStarterTrait = is_valid_starter_trait;
			this.ignoredEffects = new string[0];
			this.requiredDlcIds = requiredDlcIds;
			this.forbiddenDlcIds = forbiddenDlcIds;
		}

		public void AddIgnoredEffects(string[] effects)
		{
			List<string> list = new List<string>(this.ignoredEffects);
			list.AddRange(effects);
			this.ignoredEffects = list.ToArray();
		}

		public string GetName()
		{
			if (this.NameCB != null)
			{
				return this.NameCB();
			}
			return this.Name;
		}

		public string GetTooltip()
		{
			string text;
			if (this.TooltipCB != null)
			{
				text = this.TooltipCB();
			}
			else
			{
				text = this.description;
				text += this.GetAttributeModifiersString(true);
				text += this.GetDisabledChoresString(true);
				text += this.GetIgnoredEffectsString(true);
				text += this.GetExtendedTooltipStr();
			}
			return text;
		}

		public string GetAttributeModifiersString(bool list_entry)
		{
			string text = "";
			foreach (AttributeModifier attributeModifier in this.SelfModifiers)
			{
				Attribute attribute = Db.Get().Attributes.Get(attributeModifier.AttributeId);
				if (list_entry)
				{
					text += DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY;
				}
				text += string.Format(DUPLICANTS.TRAITS.ATTRIBUTE_MODIFIERS, attribute.Name, attributeModifier.GetFormattedString());
			}
			return text;
		}

		public string GetDisabledChoresString(bool list_entry)
		{
			string text = "";
			if (this.disabledChoreGroups != null)
			{
				string text2 = DUPLICANTS.TRAITS.CANNOT_DO_TASK;
				if (this.isTaskBeingRefused)
				{
					text2 = DUPLICANTS.TRAITS.REFUSES_TO_DO_TASK;
				}
				foreach (ChoreGroup choreGroup in this.disabledChoreGroups)
				{
					if (list_entry)
					{
						text += DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY;
					}
					text += string.Format(text2, choreGroup.Name);
				}
			}
			return text;
		}

		public string GetIgnoredEffectsString(bool list_entry)
		{
			string text = "";
			if (this.ignoredEffects != null && this.ignoredEffects.Length != 0)
			{
				for (int i = 0; i < this.ignoredEffects.Length; i++)
				{
					string text2 = this.ignoredEffects[i];
					if (list_entry)
					{
						text += DUPLICANTS.TRAITS.TRAIT_DESCRIPTION_LIST_ENTRY;
					}
					string text3 = Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text2.ToUpper() + ".NAME");
					text += string.Format(DUPLICANTS.TRAITS.IGNORED_EFFECTS, text3);
					if (!list_entry && i < this.ignoredEffects.Length - 1)
					{
						text += "\n";
					}
				}
			}
			return text;
		}

		public string GetExtendedTooltipStr()
		{
			string text = "";
			if (this.ExtendedTooltip != null)
			{
				foreach (Func<string> func in this.ExtendedTooltip.GetInvocationList())
				{
					text = text + "\n" + func();
				}
			}
			return text;
		}

		public override void AddTo(Attributes attributes)
		{
			base.AddTo(attributes);
			ChoreConsumer component = attributes.gameObject.GetComponent<ChoreConsumer>();
			if (component != null && this.disabledChoreGroups != null)
			{
				foreach (ChoreGroup choreGroup in this.disabledChoreGroups)
				{
					component.SetPermittedByTraits(choreGroup, false);
				}
			}
		}

		public override void RemoveFrom(Attributes attributes)
		{
			base.RemoveFrom(attributes);
			ChoreConsumer component = attributes.gameObject.GetComponent<ChoreConsumer>();
			if (component != null && this.disabledChoreGroups != null)
			{
				foreach (ChoreGroup choreGroup in this.disabledChoreGroups)
				{
					component.SetPermittedByTraits(choreGroup, true);
				}
			}
		}

		public float Rating;

		public bool ShouldSave;

		public bool PositiveTrait;

		public bool ValidStarterTrait;

		public Action<GameObject> OnAddTrait;

		public Func<string> TooltipCB;

		public Func<string> ExtendedTooltip;

		public Func<string> ShortDescCB;

		public Func<string> ShortDescTooltipCB;

		public Func<string> NameCB;

		public ChoreGroup[] disabledChoreGroups;

		public bool isTaskBeingRefused;

		public string[] ignoredEffects;

		public string[] requiredDlcIds;

		public string[] forbiddenDlcIds;
	}
}
