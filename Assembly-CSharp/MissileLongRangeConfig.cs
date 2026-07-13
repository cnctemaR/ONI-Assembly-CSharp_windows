using System;
using System.Collections.Generic;
using STRINGS;
using UnityEngine;

public class MissileLongRangeConfig : IEntityConfig
{
	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateLooseEntity("MissileLongRange", ITEMS.MISSILE_LONGRANGE.NAME, ITEMS.MISSILE_LONGRANGE.DESC, 200f, true, Assets.GetAnim("longrange_missile_kanim"), "object", Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, 0.8f, 1f, true, 0, SimHashes.Iron, new List<Tag>());
		gameObject.AddTag(GameTags.IndustrialProduct);
		gameObject.AddOrGetDef<MissileLongRangeProjectile.Def>();
		gameObject.AddOrGet<EntitySplitter>().maxStackSize = 200f;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const string ID = "MissileLongRange";

	public const float MASS_PER_MISSILE = 200f;

	public const int DAMAGE_PER_MISSILE = 10;

	public class DamageEventPayload
	{
		public DamageEventPayload(int damage = 10)
		{
			this.damage = damage;
		}

		public int damage;

		public static MissileLongRangeConfig.DamageEventPayload sharedInstance = new MissileLongRangeConfig.DamageEventPayload(10);
	}
}
