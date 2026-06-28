using System;
using Klei.AI;
using UnityEngine;

public class Hit
{
	public Hit(AttackProperties properties, GameObject target)
	{
		this.properties = properties;
		this.target = target;
		this.DeliverHit();
	}

	private float rollDamage()
	{
		return (float)Mathf.RoundToInt(global::UnityEngine.Random.Range(this.properties.base_damage_min, this.properties.base_damage_max));
	}

	private void DeliverHit()
	{
		Health component = this.target.GetComponent<Health>();
		if (!component)
		{
			return;
		}
		this.target.Trigger(-787691065, this.properties.attacker.GetComponent<FactionAlignment>());
		float num = this.rollDamage();
		component.Damage(num);
		if (this.properties.effects == null)
		{
			return;
		}
		Effects component2 = this.target.GetComponent<Effects>();
		if (component2)
		{
			foreach (AttackEffect attackEffect in this.properties.effects)
			{
				if (global::UnityEngine.Random.Range(0f, 100f) < attackEffect.effectProbability * 100f)
				{
					component2.Add(attackEffect.effectID, true);
				}
			}
		}
	}

	private AttackProperties properties;

	private GameObject target;
}
