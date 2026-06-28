using System;
using System.Collections.Generic;

[Serializable]
public class AttackProperties
{
	public Weapon attacker;

	public AttackProperties.DamageType damageType;

	public AttackProperties.TargetType targetType;

	public float base_damage_min;

	public float base_damage_max;

	public int maxHits;

	public float aoe_radius = 2f;

	public List<AttackEffect> effects;

	public enum DamageType
	{
		Standard
	}

	public enum TargetType
	{
		Single,
		AreaOfEffect
	}
}
