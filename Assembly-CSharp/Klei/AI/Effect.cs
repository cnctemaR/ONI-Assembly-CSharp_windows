using System;
using System.Collections.Generic;
using System.Diagnostics;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	[DebuggerDisplay("{Id}")]
	public class Effect : Modifier
	{
		public Effect(string id, string name, string description, float duration, bool show_in_ui, bool trigger_floating_text, bool is_bad, Emote emote = null, float emote_cooldown = -1f, float max_initial_delay = 0f, string stompGroup = null, string custom_icon = "")
			: this(id, name, description, duration, show_in_ui, trigger_floating_text, is_bad, emote, max_initial_delay, stompGroup, false, custom_icon, emote_cooldown)
		{
		}

		public Effect(string id, string name, string description, float duration, bool show_in_ui, bool trigger_floating_text, bool is_bad, Emote emote, float max_initial_delay, string stompGroup, bool showStatusInWorld, string custom_icon = "", float emote_cooldown = -1f)
			: this(id, name, description, duration, null, show_in_ui, trigger_floating_text, is_bad, emote, max_initial_delay, stompGroup, showStatusInWorld, custom_icon, emote_cooldown)
		{
		}

		public Effect(string id, string name, string description, float duration, string[] immunityEffects, bool show_in_ui, bool trigger_floating_text, bool is_bad, Emote emote, float max_initial_delay, string stompGroup, bool showStatusInWorld, string custom_icon = "", float emote_cooldown = -1f)
			: base(id, name, description)
		{
			this.duration = duration;
			this.showInUI = show_in_ui;
			this.triggerFloatingText = trigger_floating_text;
			this.isBad = is_bad;
			this.emote = emote;
			this.emoteCooldown = emote_cooldown;
			this.maxInitialDelay = max_initial_delay;
			this.stompGroup = stompGroup;
			this.customIcon = custom_icon;
			this.showStatusInWorld = showStatusInWorld;
			this.immunityEffectsNames = immunityEffects;
		}

		public Effect(string id, string name, string description, float duration, bool show_in_ui, bool trigger_floating_text, bool is_bad, string emoteAnim, float emote_cooldown = -1f, string stompGroup = null, string custom_icon = "")
			: base(id, name, description)
		{
			this.duration = duration;
			this.showInUI = show_in_ui;
			this.triggerFloatingText = trigger_floating_text;
			this.isBad = is_bad;
			this.emoteAnim = emoteAnim;
			this.emoteCooldown = emote_cooldown;
			this.stompGroup = stompGroup;
			this.customIcon = custom_icon;
		}

		public override void AddTo(Attributes attributes)
		{
			base.AddTo(attributes);
		}

		public override void RemoveFrom(Attributes attributes)
		{
			base.RemoveFrom(attributes);
		}

		public void SetEmote(Emote emote, float emoteCooldown = -1f)
		{
			this.emote = emote;
			this.emoteCooldown = emoteCooldown;
		}

		public void AddEmotePrecondition(Reactable.ReactablePrecondition precon)
		{
			if (this.emotePreconditions == null)
			{
				this.emotePreconditions = new List<Reactable.ReactablePrecondition>();
			}
			this.emotePreconditions.Add(precon);
		}

		public static string CreateTooltip(Effect effect, bool showDuration, string linePrefix = "\n    • ", bool showHeader = true)
		{
			StringEntry stringEntry;
			Strings.TryGet("STRINGS.DUPLICANTS.MODIFIERS." + effect.Id.ToUpper() + ".ADDITIONAL_EFFECTS", out stringEntry);
			string text = ((showHeader && (effect.SelfModifiers.Count > 0 || stringEntry != null)) ? DUPLICANTS.MODIFIERS.EFFECT_HEADER.text : "");
			foreach (AttributeModifier attributeModifier in effect.SelfModifiers)
			{
				Attribute attribute = AttributeModifier.FetchAttribute(attributeModifier.AttributeId);
				if (attribute != null && attribute.ShowInUI != Attribute.Display.Never)
				{
					text = text + linePrefix + string.Format(DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, attributeModifier.GetName(), attributeModifier.GetFormattedString());
				}
			}
			if (effect.immunityEffectsNames != null)
			{
				text += (string.IsNullOrEmpty(text) ? "" : (linePrefix + linePrefix));
				text += ((showHeader && effect.immunityEffectsNames != null && effect.immunityEffectsNames.Length != 0) ? DUPLICANTS.MODIFIERS.EFFECT_IMMUNITIES_HEADER.text : "");
				foreach (string text2 in effect.immunityEffectsNames)
				{
					Effect effect2 = Db.Get().effects.TryGet(text2);
					if (effect2 != null)
					{
						text = text + linePrefix + string.Format(DUPLICANTS.MODIFIERS.IMMUNITY_FORMAT, effect2.Name);
					}
				}
			}
			if (stringEntry != null)
			{
				text = text + linePrefix + stringEntry;
			}
			if (showDuration && effect.duration > 0f)
			{
				text = text + "\n" + string.Format(DUPLICANTS.MODIFIERS.TIME_TOTAL, GameUtil.GetFormattedCycles(effect.duration, "F1", false));
			}
			return text;
		}

		public static string CreateFullTooltip(Effect effect, bool showDuration)
		{
			return string.Concat(new string[]
			{
				effect.Name,
				"\n\n",
				effect.description,
				"\n\n",
				Effect.CreateTooltip(effect, showDuration, "\n    • ", true)
			});
		}

		public static void AddModifierDescriptions(GameObject parent, List<Descriptor> descs, string effect_id, bool increase_indent = false)
		{
			Effect.AddModifierDescriptions(descs, effect_id, increase_indent, "STRINGS.DUPLICANTS.ATTRIBUTES.");
		}

		public static void AddModifierDescriptions(List<Descriptor> descs, string effect_id, bool increase_indent = false, string prefix = "STRINGS.DUPLICANTS.ATTRIBUTES.")
		{
			foreach (AttributeModifier attributeModifier in Db.Get().effects.Get(effect_id).SelfModifiers)
			{
				Descriptor descriptor = new Descriptor(Strings.Get(prefix + attributeModifier.AttributeId.ToUpper() + ".NAME") + ": " + attributeModifier.GetFormattedString(), "", Descriptor.DescriptorType.Effect, false);
				if (increase_indent)
				{
					descriptor.IncreaseIndent();
				}
				descs.Add(descriptor);
			}
		}

		public float duration;

		public bool showInUI;

		public bool triggerFloatingText;

		public bool isBad;

		public bool showStatusInWorld;

		public string customIcon;

		public string[] immunityEffectsNames;

		public Tag? tag;

		public string emoteAnim;

		public Emote emote;

		public float emoteCooldown;

		public float maxInitialDelay;

		public List<Reactable.ReactablePrecondition> emotePreconditions;

		public string stompGroup;

		public int stompPriority;
	}
}
