using System;
using System.Diagnostics;
using STRINGS;

namespace Klei.AI
{
	[DebuggerDisplay("{Id}")]
	public class Effect : Modifier
	{
		public Effect(string id, string name, string description, float duration, bool show_in_ui, bool trigger_floating_text, bool is_bad)
			: base(id, name, description)
		{
			this.duration = duration;
			this.showInUI = show_in_ui;
			this.triggerFloatingText = trigger_floating_text;
			this.isBad = is_bad;
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<Effects, Effect, bool> OnAddRemove;

		public override void AddTo(Attributes attributes)
		{
			base.AddTo(attributes);
			if (this.OnAddRemove != null)
			{
				this.OnAddRemove(attributes.gameObject.GetComponent<Effects>(), this, true);
			}
		}

		public override void RemoveFrom(Attributes attributes)
		{
			base.RemoveFrom(attributes);
			if (this.OnAddRemove != null)
			{
				this.OnAddRemove(attributes.gameObject.GetComponent<Effects>(), this, false);
			}
		}

		public static string CreateTooltip(Effect effect, bool showDuration)
		{
			string text = string.Empty;
			foreach (AttributeModifier attributeModifier in effect.SelfModifiers)
			{
				if (Db.Get().Attributes.Get(attributeModifier.AttributeId).ShowInUI != Attribute.Display.Never)
				{
					text = text + "\n" + string.Format(DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, Db.Get().Attributes.Get(attributeModifier.AttributeId).Name, attributeModifier.GetFormattedString(null));
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
	}
}
