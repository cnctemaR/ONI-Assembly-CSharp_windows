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
			if (effect.triggerFloatingText && PopFXManager.Instance != null)
			{
				PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, effect.Name, game_object.transform, 1.5f, false);
			}
			if (!string.IsNullOrEmpty(effect.emoteAnim))
			{
				ReactionMonitor.Instance smi = base.gameObject.GetSMI<ReactionMonitor.Instance>();
				if (smi != null)
				{
					if (effect.emoteCooldown < 0f)
					{
						SelfEmoteReactable selfEmoteReactable = (SelfEmoteReactable)new SelfEmoteReactable(game_object, effect.Name + "_Emote", Db.Get().ChoreTypes.Emote, effect.emoteAnim, 100000f, 20f, float.PositiveInfinity).AddStep(new EmoteReactable.EmoteStep
						{
							anim = "react"
						});
						selfEmoteReactable.AddPrecondition(new Reactable.ReactablePrecondition(this.NotInATube));
						if (effect.emotePreconditions != null)
						{
							foreach (Reactable.ReactablePrecondition reactablePrecondition in effect.emotePreconditions)
							{
								selfEmoteReactable.AddPrecondition(reactablePrecondition);
							}
						}
						smi.AddOneshotReactable(selfEmoteReactable);
					}
					else
					{
						this.reactable = new SelfEmoteReactable(game_object, effect.Name + "_Emote", Db.Get().ChoreTypes.Emote, effect.emoteAnim, effect.emoteCooldown, 20f, float.PositiveInfinity).AddStep(new EmoteReactable.EmoteStep
						{
							anim = "react"
						});
						this.reactable.AddPrecondition(new Reactable.ReactablePrecondition(this.NotInATube));
						if (effect.emotePreconditions != null)
						{
							foreach (Reactable.ReactablePrecondition reactablePrecondition2 in effect.emotePreconditions)
							{
								this.reactable.AddPrecondition(reactablePrecondition2);
							}
						}
					}
				}
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
				KSelectable component = base.gameObject.GetComponent<KSelectable>();
				component.RemoveStatusItem(this.statusItem, false);
				this.statusItem = null;
			}
			if (this.reactable != null)
			{
				this.reactable.Cleanup();
				this.reactable = null;
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

		private void ConfigureStatusItem()
		{
			this.statusItem = new StatusItem(this.effect.Id, this.effect.Name, this.effect.description, string.Empty, (!this.effect.isBad) ? StatusItem.IconType.Info : StatusItem.IconType.Exclamation, (!this.effect.isBad) ? NotificationType.Neutral : NotificationType.Bad, false, OverlayModes.None.ID, 2);
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
			string text2 = Effect.CreateTooltip(effectInstance.effect, false, "\n");
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

		public Reactable reactable;
	}
}
