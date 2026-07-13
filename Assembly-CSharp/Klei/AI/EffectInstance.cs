using System;
using System.Diagnostics;
using STRINGS;
using UnityEngine;

namespace Klei.AI
{
	[DebuggerDisplay("{effect.Id}")]
	public class EffectInstance : ModifierInstance<Effect>
	{
		public EffectInstance(GameObject game_object, Effect effect, bool should_save, Func<string, object, string> resolveTooltipCallback = null)
			: base(game_object, effect)
		{
			this.effect = effect;
			this.shouldSave = should_save;
			this.DefineEffectImmunities();
			this.ApplyImmunities();
			this.ConfigureStatusItem(resolveTooltipCallback);
			if (effect.showInUI)
			{
				KSelectable component = base.gameObject.GetComponent<KSelectable>();
				if (!component.GetStatusItemGroup().HasStatusItem(this.statusItem))
				{
					component.AddStatusItem(this.statusItem, this);
				}
			}
			if (effect.triggerFloatingText && PopFXManager.Instance != null)
			{
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, effect.Name, game_object.transform, 1.5f, false);
			}
			if (effect.emote != null)
			{
				this.RegisterEmote(effect.emote, effect.emoteCooldown);
			}
			if (!string.IsNullOrEmpty(effect.emoteAnim))
			{
				this.RegisterEmote(effect.emoteAnim, effect.emoteCooldown);
			}
		}

		protected void DefineEffectImmunities()
		{
			if (this.immunityEffects == null && this.effect.immunityEffectsNames != null)
			{
				this.immunityEffects = new Effect[this.effect.immunityEffectsNames.Length];
				for (int i = 0; i < this.immunityEffects.Length; i++)
				{
					this.immunityEffects[i] = Db.Get().effects.Get(this.effect.immunityEffectsNames[i]);
				}
			}
		}

		protected void ApplyImmunities()
		{
			if (base.gameObject != null && this.immunityEffects != null)
			{
				Effects component = base.gameObject.GetComponent<Effects>();
				for (int i = 0; i < this.immunityEffects.Length; i++)
				{
					component.Remove(this.immunityEffects[i]);
					component.AddImmunity(this.immunityEffects[i], this.effect.IdHash.ToString(), false);
				}
			}
		}

		protected void RemoveImmunities()
		{
			if (base.gameObject != null && this.immunityEffects != null)
			{
				Effects component = base.gameObject.GetComponent<Effects>();
				for (int i = 0; i < this.immunityEffects.Length; i++)
				{
					component.RemoveImmunity(this.immunityEffects[i], this.effect.IdHash.ToString());
				}
			}
		}

		public void RegisterEmote(string emoteAnim, float cooldown = -1f)
		{
			ReactionMonitor.Instance smi = base.gameObject.GetSMI<ReactionMonitor.Instance>();
			if (smi == null)
			{
				return;
			}
			bool flag = cooldown < 0f;
			float num = (flag ? 100000f : cooldown);
			EmoteReactable emoteReactable = smi.AddSelfEmoteReactable(base.gameObject, this.effect.Name + "_Emote", emoteAnim, flag, Db.Get().ChoreTypes.Emote, num, 20f, float.NegativeInfinity, this.effect.maxInitialDelay, this.effect.emotePreconditions);
			if (emoteReactable == null)
			{
				return;
			}
			emoteReactable.InsertPrecondition(0, new Reactable.ReactablePrecondition(this.NotInATube));
			if (!flag)
			{
				this.reactable = emoteReactable;
			}
		}

		public void RegisterEmote(Emote emote, float cooldown = -1f)
		{
			ReactionMonitor.Instance smi = base.gameObject.GetSMI<ReactionMonitor.Instance>();
			if (smi == null)
			{
				return;
			}
			bool flag = cooldown < 0f;
			float num = (flag ? 100000f : cooldown);
			EmoteReactable emoteReactable = smi.AddSelfEmoteReactable(base.gameObject, this.effect.Name + "_Emote", emote, flag, Db.Get().ChoreTypes.Emote, num, 20f, float.NegativeInfinity, this.effect.maxInitialDelay, this.effect.emotePreconditions);
			if (emoteReactable == null)
			{
				return;
			}
			emoteReactable.InsertPrecondition(0, new Reactable.ReactablePrecondition(this.NotInATube));
			if (!flag)
			{
				this.reactable = emoteReactable;
			}
		}

		private bool NotInATube(GameObject go, Navigator.ActiveTransition transition)
		{
			return transition.navGridTransition.start != NavType.Tube && transition.navGridTransition.end != NavType.Tube;
		}

		public override void OnCleanUp()
		{
			if (this.statusItem != null)
			{
				base.gameObject.GetComponent<KSelectable>().RemoveStatusItem(this.statusItem, false);
				this.statusItem = null;
			}
			if (this.reactable != null)
			{
				this.reactable.Cleanup();
				this.reactable = null;
			}
			this.RemoveImmunities();
		}

		public float GetTimeRemaining()
		{
			return this.timeRemaining;
		}

		public bool IsExpired()
		{
			return this.effect.duration > 0f && this.timeRemaining <= 0f;
		}

		private void ConfigureStatusItem(Func<string, object, string> resolveTooltipCallback)
		{
			StatusItem.IconType iconType = (this.effect.isBad ? StatusItem.IconType.Exclamation : StatusItem.IconType.Info);
			if (!this.effect.customIcon.IsNullOrWhiteSpace())
			{
				iconType = StatusItem.IconType.Custom;
			}
			string id = this.effect.Id;
			string name = this.effect.Name;
			string description = this.effect.description;
			string customIcon = this.effect.customIcon;
			StatusItem.IconType iconType2 = iconType;
			NotificationType notificationType = (this.effect.isBad ? NotificationType.Bad : NotificationType.Neutral);
			bool flag = false;
			bool showStatusInWorld = this.effect.showStatusInWorld;
			this.statusItem = new StatusItem(id, name, description, customIcon, iconType2, notificationType, flag, OverlayModes.None.ID, 2, showStatusInWorld, null);
			this.statusItem.resolveStringCallback = new Func<string, object, string>(this.ResolveString);
			this.statusItem.resolveTooltipCallback = resolveTooltipCallback ?? new Func<string, object, string>(this.ResolveTooltip);
		}

		private string ResolveString(string str, object data)
		{
			return str;
		}

		public string ResolveTooltip(string str, object data)
		{
			string text = str;
			EffectInstance effectInstance = (EffectInstance)data;
			string text2 = Effect.CreateTooltip(effectInstance.effect, false, "\n    • ", true);
			if (!string.IsNullOrEmpty(text2))
			{
				text = text + "\n\n" + text2;
			}
			if (effectInstance.effect.duration > 0f)
			{
				text = text + "\n\n" + string.Format(DUPLICANTS.MODIFIERS.TIME_REMAINING, GameUtil.GetFormattedCycles(this.GetTimeRemaining(), "F1", false));
			}
			return text;
		}

		public Effect effect;

		public bool shouldSave;

		public StatusItem statusItem;

		public float timeRemaining;

		public EmoteReactable reactable;

		protected Effect[] immunityEffects;
	}
}
