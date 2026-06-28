using System;
using UnityEngine;

namespace Klei.AI
{
	public class Amounts : Modifications<Amount, AmountInstance>
	{
		public Amounts(GameObject go)
			: base(go, null)
		{
		}

		public float GetValue(string amount_id)
		{
			AmountInstance amountInstance = base.Get(amount_id);
			return amountInstance.value;
		}

		public void SetValue(string amount_id, float value)
		{
			AmountInstance amountInstance = base.Get(amount_id);
			amountInstance.value = value;
		}

		public override AmountInstance Add(AmountInstance instance)
		{
			DebugUtil.Assert(!instance.isActive, "Assert!");
			Game.Instance.amounts.Add(instance);
			instance.isActive = true;
			return base.Add(instance);
		}

		public override void Remove(AmountInstance instance)
		{
			base.Remove(instance);
			DebugUtil.Assert(instance.isActive, "Assert!");
			instance.isActive = false;
		}

		public void Cleanup()
		{
			for (int i = 0; i < base.Count; i++)
			{
				base[i].isActive = false;
			}
		}
	}
}
