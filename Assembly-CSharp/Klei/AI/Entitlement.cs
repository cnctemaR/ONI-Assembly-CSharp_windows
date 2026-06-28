using System;
using UnityEngine;

namespace Klei.AI
{
	public abstract class Entitlement
	{
		public abstract void Apply(GameObject target);

		public abstract void Unapply(GameObject target);
	}
}
