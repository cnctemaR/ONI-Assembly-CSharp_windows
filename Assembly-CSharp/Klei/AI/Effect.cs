using System;
using System.Collections.Generic;
using System.Diagnostics;
using STRINGS;

namespace Klei.AI
{
	[DebuggerDisplay("{Id}")]
	public class Effect : Modifier
	{
		public Effect(string id, string name, string description, float duration, bool show_in_ui, bool trigger_floating_text, bool is_bad, string emote_anim = null, float emote_cooldown = 0f)
			: base(id, name, description)
		{
			this.duration = duration;
			this.showInUI = show_in_ui;
			this.triggerFloatingText = trigger_floating_text;
			this.isBad = is_bad;
			this.emoteAnim = emote_anim;
			this.emoteCooldown = emote_cooldown;
		}

		public override void AddTo(Attributes attributes)
		{
			base.AddTo(attributes);
		}

		public override void RemoveFrom(Attributes attributes)
		{
			base.RemoveFrom(attributes);
		}

		public void AddEmotePrecondition(Reactable.ReactablePrecondition precon)
		{
			if (this.emotePreconditions == null)
			{
				this.emotePreconditions = new List<Reactable.ReactablePrecondition>();
			}
			this.emotePreconditions.Add(precon);
		}

		public static string CreateTooltip(Effect effect, bool showDuration)
		{
			string text = string.Empty;
			foreach (AttributeModifier attributeModifier in effect.SelfModifiers)
			{
				Attribute attribute = Db.Get().Attributes.TryGet(attributeModifier.AttributeId);
				if (attribute == null)
				{
					attribute = Db.Get().CritterAttributes.TryGet(attributeModifier.AttributeId);
				}
				if (attribute != null && attribute.ShowInUI != Attribute.Display.Never)
				{
					text = text + "\n" + string.Format(DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, attribute.Name, attributeModifier.GetFormattedString(null));
				}
			}
			StringEntry stringEntry;
			if (Strings.TryGet("STRINGS.DUPLICANTS.MODIFIERS." + effect.Id.ToUpper() + ".ADDITIONAL_EFFECTS", out stringEntry))
			{
				text = text + "\n" + stringEntry;
			}
			if (showDuration && effect.duration > 0f)
			{
				text = text + "\n" + string.Format(DUPLICANTS.MODIFIERS.TIME_TOTAL, GameUtil.GetFormattedCycles(effect.duration, "F1"));
			}
			return text;
		}

		public float duration;

		public bool showInUI;

		public bool triggerFloatingText;

		public bool isBad;

		public string emoteAnim;

		public float emoteCooldown;

		public List<Reactable.ReactablePrecondition> emotePreconditions;
	}
}
