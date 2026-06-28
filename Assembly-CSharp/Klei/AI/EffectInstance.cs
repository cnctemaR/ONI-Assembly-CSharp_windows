using System;
using System.Diagnostics;
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
			this.gameObject.GetComponent<KSelectable>().RemoveStatusItem(this.statusItem);
		}

		private void ConfigureStatusItem()
		{
			StatusItem.IconType iconType = ((!this.effect.isBad) ? StatusItem.IconType.Info : StatusItem.IconType.Exclamation);
			this.statusItem = new StatusItem(this.effect.Id, this.effect.Name, string.Empty, this.ResolveTooltip(), false, iconType, (!this.effect.isBad) ? NotificationType.Neutral : NotificationType.Bad, SimViewMode.None, SimViewMode.None);
			this.statusItem.resolveStringCallback = new Func<string, object, string>(this.ResolveString);
		}

		private string ResolveTooltip()
		{
			string text = string.Empty;
			text += Strings.Get("STRINGS.DUPLICANTS.MODIFIERS." + this.effect.Id.ToUpper() + ".TOOLTIP");
			if (this.effect.SelfModifiers.Count > 0)
			{
				text += "\n";
			}
			foreach (AttributeModifier attributeModifier in this.effect.SelfModifiers)
			{
				string text2 = text;
				text = string.Concat(new string[]
				{
					text2,
					"\n",
					Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + attributeModifier.AttributeId.ToUpper() + ".NAME"),
					" ",
					attributeModifier.GetFormattedString()
				});
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

		private StatusItem statusItem;
	}
}
