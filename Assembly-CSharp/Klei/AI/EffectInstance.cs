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
			if (effect.showInUI)
			{
				KSelectable component = base.gameObject.GetComponent<KSelectable>();
				if (!component.GetStatusItemGroup().HasStatusItemID(this.statusItem))
				{
					component.AddStatusItem(this.statusItem, this);
				}
			}
		}

		public float GetTimeRemaining()
		{
			return this.timeRemaining;
		}

		public bool IsExpired()
		{
			return this.effect.duration > 0f && this.timeRemaining <= 0f;
		}

		public void Remove()
		{
			base.gameObject.GetComponent<Effects>().Remove(this.effect);
			base.gameObject.GetComponent<KSelectable>().RemoveStatusItem(this.statusItem, false);
		}

		private void ConfigureStatusItem()
		{
			this.statusItem = new StatusItem(this.effect.Id, this.effect.Name, this.effect.description, string.Empty, (!this.effect.isBad) ? StatusItem.IconType.Info : StatusItem.IconType.Exclamation, (!this.effect.isBad) ? NotificationType.Neutral : NotificationType.Bad, false, SimViewMode.None, 63486);
			this.statusItem.resolveStringCallback = new Func<string, object, string>(this.ResolveString);
			this.statusItem.resolveTooltipCallback = new Func<string, object, string>(this.ResolveTooltip);
		}

		private string ResolveString(string str, object data)
		{
			return str;
		}

		private string ResolveTooltip(string str, object data)
		{
			string text = str;
			EffectInstance effectInstance = (EffectInstance)data;
			string text2 = Effect.CreateTooltip(effectInstance.effect, false);
			if (!string.IsNullOrEmpty(text2))
			{
				text = text + "\n" + text2;
			}
			if (effectInstance.effect.duration > 0f)
			{
				text = text + "\n" + string.Format(DUPLICANTS.MODIFIERS.TIME_REMAINING, GameUtil.GetFormattedCycles(this.GetTimeRemaining(), "F1"));
			}
			return text;
		}

		public Effect effect;

		public bool shouldSave;

		public StatusItem statusItem;

		public float timeRemaining;
	}
}
