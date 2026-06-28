using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Effects : KMonoBehaviour, ISaveLoadable
	{
		public IEnumerator<EffectInstance> GetEnumerator()
		{
			return this.effects.GetEnumerator();
		}

		protected override void OnSpawn()
		{
			Modifiers component = base.GetComponent<Modifiers>();
			if (this.saveLoadEffects != null)
			{
				foreach (Effects.SaveLoadEffect saveLoadEffect in this.saveLoadEffects)
				{
					if (component.modifierSet.effects.Exists(saveLoadEffect.id))
					{
						Effect effect = component.modifierSet.effects.Get(saveLoadEffect.id);
						EffectInstance effectInstance = this.Add(effect, true);
						if (effectInstance != null)
						{
							effectInstance.startTime = Time.time - (effect.duration - saveLoadEffect.timeRemaining);
						}
					}
				}
			}
		}

		public bool Has(string effect_id)
		{
			return this.Get(effect_id) != null;
		}

		public bool Has(Effect effect)
		{
			return this.Get(effect) != null;
		}

		public EffectInstance Get(string effect_id)
		{
			foreach (EffectInstance effectInstance in this.effects)
			{
				if (effectInstance.effect.Id == effect_id)
				{
					return effectInstance;
				}
			}
			return null;
		}

		public EffectInstance Get(Effect effect)
		{
			foreach (EffectInstance effectInstance in this.effects)
			{
				if (effectInstance.effect == effect)
				{
					return effectInstance;
				}
			}
			return null;
		}

		public EffectInstance Add(string effect_id, bool should_save)
		{
			Effect effect = base.GetComponent<Modifiers>().modifierSet.effects.Get(effect_id);
			return this.Add(effect, should_save);
		}

		public EffectInstance Add(Effect effect, bool should_save)
		{
			bool flag = true;
			foreach (Trait trait in base.GetComponent<Traits>())
			{
				if (trait.ignoredEffects != null && Array.IndexOf<string>(trait.ignoredEffects, effect.Id) != -1)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				Attributes attributes = this.GetAttributes();
				EffectInstance effectInstance = this.Get(effect);
				if (effectInstance == null)
				{
					effectInstance = new EffectInstance(base.gameObject, effect, should_save);
					effect.AddTo(attributes);
					this.effects.Add(effectInstance);
					this.Trigger(-1901442097, effect);
				}
				effectInstance.startTime = Time.time;
				return effectInstance;
			}
			return null;
		}

		public void Remove(Effect effect)
		{
			Attributes attributes = this.GetAttributes();
			for (int i = 0; i < this.effects.Count; i++)
			{
				EffectInstance effectInstance = this.effects[i];
				if (effectInstance.effect == effect)
				{
					effect.RemoveFrom(attributes);
					this.effects.RemoveAt(i);
					effectInstance.Remove();
					this.Trigger(-1157678353, effect);
				}
			}
		}

		public void Remove(string effect_id)
		{
			Attributes attributes = this.GetAttributes();
			for (int i = 0; i < this.effects.Count; i++)
			{
				EffectInstance effectInstance = this.effects[i];
				if (effectInstance.effect.Id == effect_id)
				{
					effectInstance.effect.RemoveFrom(attributes);
					this.effects.RemoveAt(i);
					effectInstance.Remove();
					this.Trigger(-1157678353, effectInstance.effect);
				}
			}
		}

		public bool HasEffect(string effect_id)
		{
			foreach (EffectInstance effectInstance in this.effects)
			{
				if (effectInstance.effect.Id == effect_id)
				{
					return true;
				}
			}
			return false;
		}

		public bool HasEffect(Effect effect)
		{
			foreach (EffectInstance effectInstance in this.effects)
			{
				if (effectInstance.effect == effect)
				{
					return true;
				}
			}
			return false;
		}

		private void Update()
		{
			for (int i = 0; i < this.effects.Count; i++)
			{
				if (this.effects[i].IsExpired())
				{
					this.Remove(this.effects[i].effect);
				}
			}
		}

		[OnSerializing]
		internal void OnSerializing()
		{
			List<Effects.SaveLoadEffect> list = new List<Effects.SaveLoadEffect>();
			foreach (EffectInstance effectInstance in this.effects)
			{
				if (effectInstance.shouldSave)
				{
					Effects.SaveLoadEffect saveLoadEffect = new Effects.SaveLoadEffect
					{
						id = effectInstance.effect.Id,
						timeRemaining = effectInstance.effect.duration - (Time.time - effectInstance.startTime)
					};
					list.Add(saveLoadEffect);
				}
			}
			this.saveLoadEffects = list.ToArray();
		}

		[Serialize]
		private Effects.SaveLoadEffect[] saveLoadEffects;

		private List<EffectInstance> effects = new List<EffectInstance>();

		[Serializable]
		private struct SaveLoadEffect
		{
			public string id;

			public float timeRemaining;
		}
	}
}
