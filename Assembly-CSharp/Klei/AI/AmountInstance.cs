using System;
using System.Diagnostics;
using KSerialization;
using UnityEngine;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	[DebuggerDisplay("{amount.Name} {value} ({deltaAttribute.value}/{minAttribute.value}/{maxAttribute.value})")]
	public class AmountInstance : ModifierInstance<Amount>, ISaveLoadable, ISim200ms
	{
		public AmountInstance(Amount amount, GameObject game_object)
			: base(game_object, amount)
		{
			Attributes attributes = game_object.GetAttributes();
			this.minAttribute = attributes.Add(amount.minAttribute);
			this.maxAttribute = attributes.Add(amount.maxAttribute);
			this.deltaAttribute = attributes.Add(amount.deltaAttribute);
		}

		public Amount amount
		{
			get
			{
				return this.modifier;
			}
		}

		public bool paused
		{
			get
			{
				return this._paused;
			}
			set
			{
				this._paused = this.paused;
				if (this._paused)
				{
					this.Deactivate();
				}
				else
				{
					this.Activate();
				}
			}
		}

		public float GetMin()
		{
			return this.minAttribute.GetTotalValue();
		}

		public float GetMax()
		{
			return this.maxAttribute.GetTotalValue();
		}

		public float GetDelta()
		{
			return this.deltaAttribute.GetTotalValue();
		}

		public float SetValue(float value)
		{
			this.value = value;
			this.value = Mathf.Max(this.value, this.GetMin());
			this.value = Mathf.Min(this.value, this.GetMax());
			return this.value;
		}

		public float ApplyDelta(float delta)
		{
			float num = this.value;
			this.SetValue(this.value + delta);
			if (this.OnDelta != null)
			{
				this.OnDelta(delta);
			}
			if (this.OnMaxValueReached != null && num < this.GetMax() && this.value >= this.GetMax())
			{
				this.OnMaxValueReached();
			}
			return this.value;
		}

		public string GetValueString()
		{
			return this.amount.GetValueString(this);
		}

		public string GetDescription()
		{
			return this.amount.GetDescription(this);
		}

		public string GetTooltip()
		{
			return this.amount.GetTooltip(this);
		}

		public void Activate()
		{
			SimAndRenderScheduler.instance.Add(this, false);
		}

		public void Sim200ms(float dt)
		{
			float delta = this.GetDelta();
			if (delta != 0f)
			{
				this.ApplyDelta(delta * dt);
			}
		}

		public void Deactivate()
		{
			SimAndRenderScheduler.instance.Remove(this);
		}

		[Serialize]
		public float value;

		public AttributeInstance minAttribute;

		public AttributeInstance maxAttribute;

		public AttributeInstance deltaAttribute;

		public Action<float> OnDelta;

		public global::System.Action OnMaxValueReached;

		private bool _paused;
	}
}
