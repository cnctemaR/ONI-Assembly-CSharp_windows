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
	}
}
