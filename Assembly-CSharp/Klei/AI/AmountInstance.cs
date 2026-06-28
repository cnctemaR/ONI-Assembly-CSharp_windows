using System;
using KSerialization;
using UnityEngine;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class AmountInstance : ModifierInstance<Amount>, ISaveLoadableJson
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
			if (num < this.GetMax() && this.value >= this.GetMax() && this.OnMaxValueReached != null)
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

		[Serialize]
		public float value;

		public AttributeInstance minAttribute;

		public AttributeInstance maxAttribute;

		public AttributeInstance deltaAttribute;

		public Action<float> OnDelta;

		public global::System.Action OnMaxValueReached;
	}
}
