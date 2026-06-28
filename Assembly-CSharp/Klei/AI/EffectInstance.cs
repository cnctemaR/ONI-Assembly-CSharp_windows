using System;
using System.Diagnostics;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	[DebuggerDisplay("{effect.Id}")]
	public class EffectInstance : ModifierInstance<Effect>
	{
		public EffectInstance(GameObject game_object, Effect effect, bool should_save)
			: base(game_object, effect)
		{
			this.effect = effect;
			this.shouldSave = should_save;
			this.ConfigureStatusItem();
			if (effect.showInUI && !this.gameObject.GetComponent<KSelectable>().GetStatusItemGroup().HasStatusItemID(this.statusItem))
			{
				this.gameObject.GetComponent<KSelectable>().AddStatusItem(this.statusItem, this);
			}
		}

		public float GetTimeRemaining()
		{
			return this.effect.duration - (Time.time - this.startTime);
		}

		public bool IsExpired()
		{
			return this.effect.duration > 0f && Time.time - this.startTime > this.effect.duration;
		}

		public void Remove()
		{
			this.gameObject.GetComponent<Effects>().Remove(this.effect);
			this.gameObject.GetComponent<KSelectable>().RemoveStatusItem(this.statusItem, false);
		}

		private void ConfigureStatusItem()
		{
			this.statusItem = new StatusItem(this.effect.Id, this.effect.Name, Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + this.effect.Id.ToUpper() + ".TOOLTIP"), string.Empty, (!this.effect.isBad) ? StatusItem.IconType.Info : StatusItem.IconType.Exclamation, (!this.effect.isBad) ? NotificationType.Neutral : NotificationType.Bad, false, SimViewMode.None, SimViewMode.None, 2046);
			this.statusItem.resolveStringCallback = new Func<string, object, string>(this.ResolveString);
			this.statusItem.resolveTooltipCallback = new Func<string, object, string>(this.ResolveTooltip);
		}

		private string ResolveTooltip(string str, object data)
		{
			string text = str;
			EffectInstance effectInstance = (EffectInstance)data;
			if (effectInstance.effect.SelfModifiers.Count > 0)
			{
				text += "\n";
			}
			foreach (AttributeModifier attributeModifier in effectInstance.effect.SelfModifiers)
			{
				text += string.Format(DUPLICANTS.MODIFIERS.MODIFIER_FORMAT, Db.Get().Attributes.Get(attributeModifier.AttributeId).Name, attributeModifier.GetFormattedString(this.gameObject));
			}
			StringEntry stringEntry;
			if (Strings.TryGet("STRINGS.DUPLICANTS.MODIFIERS." + effectInstance.effect.Id.ToUpper() + ".ADDITIONAL_EFFECTS", out stringEntry))
			{
				text = text + "\n" + stringEntry;
			}
			if (effectInstance.effect.duration > 0f)
			{
				text = text + "\n" + string.Format(DUPLICANTS.MODIFIERS.TIME_REMAINING, GameUtil.GetFormattedCycles(this.GetTimeRemaining(), "F1"));
			}
			return text;
		}

		private string ResolveString(string str, object data)
		{
			return str;
		}

		public Effect effect;

		public float startTime;

		public bool shouldSave;

		public StatusItem statusItem;
	}
}
