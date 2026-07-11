using System;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : KMonoBehaviour
{
	public void Configure(float base_damage_min, float base_damage_max, AttackProperties.DamageType attackType = AttackProperties.DamageType.Standard, AttackProperties.TargetType targetType = AttackProperties.TargetType.Single, int maxHits = 1, float aoeRadius = 0f)
	{
		this.properties = new AttackProperties();
		this.properties.base_damage_min = base_damage_min;
		this.properties.base_damage_max = base_damage_max;
		this.properties.maxHits = maxHits;
		this.properties.damageType = attackType;
		this.properties.aoe_radius = aoeRadius;
		this.properties.attacker = this;
	}

	public void AddEffect(string effectID = "WasAttacked", float probability = 1f)
	{
		if (this.properties.effects == null)
		{
			this.properties.effects = new List<AttackEffect>();
		}
		this.properties.effects.Add(new AttackEffect(effectID, probability));
	}

	public void AttackArea(Vector3 centerPoint)
	{
		Vector3 vector = Vector3.zero;
		this.alignment = base.GetComponent<FactionAlignment>();
		if (this.alignment == null)
		{
			return;
		}
		List<GameObject> list = new List<GameObject>();
		foreach (Health health in Components.Health.Items)
		{
			if (!(health.gameObject == base.gameObject) && !health.IsDefeated())
			{
				FactionAlignment component = health.GetComponent<FactionAlignment>();
				if (!(component == null) && component.IsAlignmentActive() && FactionManager.Instance.GetDisposition(this.alignment.Alignment, component.Alignment) == FactionManager.Disposition.Attack)
				{
					vector = health.transform.GetPosition();
					vector.z = centerPoint.z;
					if (Vector3.Distance(centerPoint, vector) <= this.properties.aoe_radius)
					{
						list.Add(health.gameObject);
					}
				}
			}
		}
		this.AttackTargets(list.ToArray());
	}

	public void AttackTarget(GameObject target)
	{
		this.AttackTargets(new GameObject[] { target });
	}

	public void AttackTargets(GameObject[] targets)
	{
		if (this.properties == null)
		{
			global::Debug.LogWarning(string.Format("Attack properties not configured. {0} cannot attack with weapon.", base.gameObject.name));
			return;
		}
		new Attack(this.properties, targets);
	}

	protected override void OnSpawn()
	{
		base.OnSpawn();
		this.properties.attacker = this;
	}

	[MyCmpReq]
	private FactionAlignment alignment;

	public AttackProperties properties;
}
