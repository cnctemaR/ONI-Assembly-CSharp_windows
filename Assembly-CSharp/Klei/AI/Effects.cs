using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class Effects : KMonoBehaviour, ISaveLoadable, ISim1000ms
	{
		protected override void OnPrefabInit()
		{
			this.autoRegisterSimRender = false;
		}

		protected override void OnSpawn()
		{
			if (this.saveLoadEffects != null)
			{
				foreach (Effects.SaveLoadEffect saveLoadEffect in this.saveLoadEffects)
				{
					if (Db.Get().effects.Exists(saveLoadEffect.id))
					{
						Effect effect = Db.Get().effects.Get(saveLoadEffect.id);
						EffectInstance effectInstance = this.Add(effect, true);
						if (effectInstance != null)
						{
							effectInstance.timeRemaining = saveLoadEffect.timeRemaining;
						}
					}
				}
			}
			if (this.effectsThatExpire.Count > 0)
			{
				SimAndRenderScheduler.instance.Add(this, this.simRenderLoadBalance);
			}
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
			Effect effect = Db.Get().effects.Get(effect_id);
			return this.Add(effect, should_save);
		}

		public EffectInstance Add(Effect effect, bool should_save)
		{
			if (this.effectImmunites.Contains(effect))
			{
				return null;
			}
			bool flag = true;
			Traits component = base.GetComponent<Traits>();
			if (component != null)
			{
				foreach (Trait trait in component.TraitList)
				{
					if (trait.ignoredEffects != null && Array.IndexOf<string>(trait.ignoredEffects, effect.Id) != -1)
					{
						flag = false;
						break;
					}
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
					if (effect.duration > 0f)
					{
						this.effectsThatExpire.Add(effectInstance);
						if (this.effectsThatExpire.Count == 1)
						{
							SimAndRenderScheduler.instance.Add(this, this.simRenderLoadBalance);
						}
					}
					base.Trigger(-1901442097, effect);
				}
				effectInstance.timeRemaining = effect.duration;
				return effectInstance;
			}
			return null;
		}

		public void Remove(Effect effect)
		{
			this.Remove(effect.Id);
		}

		public void Remove(string effect_id)
		{
			int num = this.effectsThatExpire.FindIndex((EffectInstance e) => e.effect.Id == effect_id);
			if (num != -1)
			{
				int num2 = this.effectsThatExpire.Count - 1;
				this.effectsThatExpire[num] = this.effectsThatExpire[num2];
				this.effectsThatExpire.RemoveAt(num2);
				if (this.effectsThatExpire.Count == 0)
				{
					SimAndRenderScheduler.instance.Remove(this);
				}
			}
			num = this.effects.FindIndex((EffectInstance e) => e.effect.Id == effect_id);
			if (num != -1)
			{
				Attributes attributes = this.GetAttributes();
				EffectInstance effectInstance = this.effects[num];
				effectInstance.OnCleanUp();
				Effect effect = effectInstance.effect;
				effect.RemoveFrom(attributes);
				int num3 = this.effects.Count - 1;
				this.effects[num] = this.effects[num3];
				this.effects.RemoveAt(num3);
				base.Trigger(-1157678353, effect);
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

		public void Sim1000ms(float dt)
		{
			for (int i = 0; i < this.effectsThatExpire.Count; i++)
			{
				EffectInstance effectInstance = this.effectsThatExpire[i];
				if (effectInstance.IsExpired())
				{
					this.Remove(effectInstance.effect);
				}
				effectInstance.timeRemaining -= dt;
			}
		}

		public void AddImmunity(Effect effect)
		{
			this.effectImmunites.Add(effect);
		}

		public void RemoveImmunity(Effect effect)
		{
			this.effectImmunites.Remove(effect);
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
						timeRemaining = effectInstance.timeRemaining
					};
					list.Add(saveLoadEffect);
				}
			}
			this.saveLoadEffects = list.ToArray();
		}

		[Serialize]
		private Effects.SaveLoadEffect[] saveLoadEffects;

		private List<EffectInstance> effects = new List<EffectInstance>();

		private List<EffectInstance> effectsThatExpire = new List<EffectInstance>();

		private List<Effect> effectImmunites = new List<Effect>();

		[Serializable]
		private struct SaveLoadEffect
		{
			public string id;

			public float timeRemaining;
		}
	}
}
