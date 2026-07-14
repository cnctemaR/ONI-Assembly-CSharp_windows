using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	public class TraitUtil
	{
		public static global::System.Action CreateDisabledTaskTrait(string id, string name, string desc, string disabled_chore_group, bool is_valid_starter_trait)
		{
			return delegate
			{
				ChoreGroup[] array = new ChoreGroup[] { Db.Get().ChoreGroups.Get(disabled_chore_group) };
				Db.Get().CreateTrait(id, name, desc, null, true, array, false, is_valid_starter_trait);
			};
		}

		public static global::System.Action CreateTrait(string id, string name, string desc, string attributeId, float delta, string[] chore_groups, bool positiveTrait = false)
		{
			return delegate
			{
				List<ChoreGroup> list = new List<ChoreGroup>();
				foreach (string text in chore_groups)
				{
					list.Add(Db.Get().ChoreGroups.Get(text));
				}
				Db.Get().CreateTrait(id, name, desc, null, true, list.ToArray(), positiveTrait, true).Add(new AttributeModifier(attributeId, delta, name, false, false, true));
			};
		}

		public static global::System.Action CreateAttributeEffectTrait(string id, string name, string desc, string attributeId, float delta, string attributeId2, float delta2, bool positiveTrait = false)
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, true, null, positiveTrait, true);
				trait.Add(new AttributeModifier(attributeId, delta, name, false, false, true));
				trait.Add(new AttributeModifier(attributeId2, delta2, name, false, false, true));
			};
		}

		public static global::System.Action CreateAttributeEffectTrait(string id, string name, string desc, string[] attributeIds, float[] deltas, bool positiveTrait = false)
		{
			return TraitUtil.CreateAttributeEffectTrait(id, name, desc, attributeIds, deltas, positiveTrait, null);
		}

		public static global::System.Action CreateAttributeEffectTrait(string id, string name, string desc, string[] attributeIds, float[] deltas, bool positiveTrait = false, Action<GameObject> on_add = null)
		{
			return delegate
			{
				global::Debug.Assert(attributeIds.Length == deltas.Length, "CreateAttributeEffectTrait must have an equal number of attributeIds and deltas");
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, true, null, positiveTrait, true);
				for (int i = 0; i < attributeIds.Length; i++)
				{
					trait.Add(new AttributeModifier(attributeIds[i], deltas[i], name, false, false, true));
				}
				trait.OnAddTrait = on_add;
			};
		}

		public static global::System.Action CreateAttributeEffectTrait(string id, string name, string desc, string attributeId, float delta, bool positiveTrait = false, Action<GameObject> on_add = null, bool is_valid_starter_trait = true)
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, true, null, positiveTrait, is_valid_starter_trait);
				trait.Add(new AttributeModifier(attributeId, delta, name, false, false, true));
				trait.OnAddTrait = on_add;
			};
		}

		public static global::System.Action CreateEffectModifierTrait(string id, string name, string desc, string[] ignoredEffects, bool positiveTrait = false)
		{
			return delegate
			{
				Db.Get().CreateTrait(id, name, desc, null, true, null, positiveTrait, true).AddIgnoredEffects(ignoredEffects);
			};
		}

		public static global::System.Action CreateNamedTrait(string id, string name, string desc, bool positiveTrait = false)
		{
			return delegate
			{
				Db.Get().CreateTrait(id, name, desc, null, true, null, positiveTrait, true);
			};
		}

		public static global::System.Action CreateTrait(string id, string name, string desc, Action<GameObject> on_add, ChoreGroup[] disabled_chore_groups = null, bool positiveTrait = false, Func<string> extendedDescFn = null)
		{
			return TraitUtil.CreateTrait(id, name, desc, on_add, null, null, disabled_chore_groups, positiveTrait, extendedDescFn);
		}

		public static global::System.Action CreateTrait(string id, string name, string desc, Action<GameObject> on_add, string[] requiredDlcIds, string[] forbiddenDlcIds = null, ChoreGroup[] disabled_chore_groups = null, bool positiveTrait = false, Func<string> extendedDescFn = null)
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, true, disabled_chore_groups, positiveTrait, true, requiredDlcIds, forbiddenDlcIds);
				trait.OnAddTrait = on_add;
				if (extendedDescFn != null)
				{
					Trait trait2 = trait;
					trait2.ExtendedTooltip = (Func<string>)Delegate.Combine(trait2.ExtendedTooltip, extendedDescFn);
				}
			};
		}

		public static global::System.Action CreateComponentTrait<T>(string id, string name, string desc, bool positiveTrait = false, Func<string> extendedDescFn = null) where T : KMonoBehaviour
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, true, null, positiveTrait, true);
				trait.OnAddTrait = delegate(GameObject go)
				{
					go.FindOrAddUnityComponent<T>();
				};
				if (extendedDescFn != null)
				{
					Trait trait2 = trait;
					trait2.ExtendedTooltip = (Func<string>)Delegate.Combine(trait2.ExtendedTooltip, extendedDescFn);
				}
			};
		}

		public static global::System.Action CreateSkillGrantingTrait(string id, string name, string desc, string skillId)
		{
			return delegate
			{
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, true, null, true, true);
				trait.TooltipCB = () => string.Format(DUPLICANTS.TRAITS.GRANTED_SKILL_SHARED_DESC, desc, SkillWidget.SkillPerksString(Db.Get().Skills.Get(skillId)));
				trait.OnAddTrait = delegate(GameObject go)
				{
					MinionResume component = go.GetComponent<MinionResume>();
					if (component != null)
					{
						component.GrantSkill(skillId);
					}
				};
			};
		}

		public static string GetSkillGrantingTraitNameById(string id)
		{
			string text = "";
			StringEntry stringEntry;
			if (Strings.TryGet("STRINGS.DUPLICANTS.TRAITS.GRANTSKILL_" + id.ToUpper() + ".NAME", out stringEntry))
			{
				text = stringEntry.String;
			}
			return text;
		}

		public static global::System.Action CreateBionicUpgradeTrait(string id, string effectsDescription)
		{
			return delegate
			{
				string name = Strings.Get("STRINGS.DUPLICANTS.TRAITS." + id.ToUpper() + ".NAME");
				string desc = Strings.Get("STRINGS.DUPLICANTS.TRAITS." + id.ToUpper() + ".DESC");
				Trait trait = Db.Get().CreateTrait(id, name, desc, null, true, null, true, true);
				trait.TooltipCB = () => desc + "\n\n" + effectsDescription;
				trait.NameCB = () => name;
				string shortDescTooltip = Strings.Get("STRINGS.DUPLICANTS.TRAITS." + trait.Id.ToUpper() + ".SHORT_DESC_TOOLTIP");
				trait.ShortDescTooltipCB = () => shortDescTooltip + "\n\n" + effectsDescription;
			};
		}
	}
}
