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
		}

		public void AddIgnoredEffects(string[] effects)
		{
			List<string> list = new List<string>(this.ignoredEffects);
			list.AddRange(effects);
			this.ignoredEffects = list.ToArray();
		}

		public string GetTooltip()
		{
			string text = this.description;
			foreach (AttributeModifier attributeModifier in this.SelfModifiers)
			{
				Attribute attribute = Db.Get().Attributes.Get(attributeModifier.AttributeId);
				text += string.Format(DUPLICANTS.TRAITS.ATTRIBUTE_MODIFIERS, attribute.Name, attributeModifier.GetFormattedString(null));
			}
			if (this.disabledChoreGroups != null)
			{
				string text2 = DUPLICANTS.TRAITS.CANNOT_DO_TASK;
				if (this.isTaskBeingRefused)
				{
					text2 = DUPLICANTS.TRAITS.REFUSES_TO_DO_TASK;
				}
				foreach (ChoreGroup choreGroup in this.disabledChoreGroups)
				{
					text += string.Format(text2, choreGroup.Name);
				}
			}
			if (this.ignoredEffects != null && this.ignoredEffects.Length > 0)
			{
				foreach (string text3 in this.ignoredEffects)
				{
					string text4 = Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + text3.ToUpper() + ".NAME");
					text += string.Format(DUPLICANTS.TRAITS.IGNORED_EFFECTS, text4);
				}
			}
			if (this.ExtendedTooltip != null)
			{
				foreach (Delegate @delegate in this.ExtendedTooltip.GetInvocationList())
				{
					Func<string> func = (Func<string>)@delegate;
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

		public Func<string> ExtendedTooltip;

		public ChoreGroup[] disabledChoreGroups;

		public bool isTaskBeingRefused;

		public string[] ignoredEffects;
	}
}
