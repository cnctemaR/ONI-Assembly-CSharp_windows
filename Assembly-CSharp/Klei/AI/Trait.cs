using System;
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
		}

		public string GetTooltip()
		{
			string text = this.description;
			foreach (AttributeModifier attributeModifier in this.SelfModifiers)
			{
				Attribute attribute = Db.Get().Attributes.Get(attributeModifier.AttributeId);
				text += string.Format("\n{0}{1}: {2}", "• ", attribute.Name, attributeModifier.GetFormattedString());
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
					text += string.Format("\n{0}{1}: {2}", "• ", text2, choreGroup.Name);
				}
			}
			if (this.ExtendedTooltip != null)
			{
				foreach (Delegate @delegate in this.ExtendedTooltip.GetInvocationList())
				{
					Func<string> func = (Func<string>)@delegate;
					text = text + "\n\n" + func();
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
					component.SetEnabled(choreGroup, false);
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
					component.SetEnabled(choreGroup, true);
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
	}
}
