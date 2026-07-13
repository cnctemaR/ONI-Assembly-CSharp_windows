using System;
using STRINGS;
using UnityEngine;

public class HighEnergyParticleConfig : IEntityConfig, IHasDlcRestrictions
{
	public string[] GetRequiredDlcIds()
	{
		return DlcManager.EXPANSION1;
	}

	public string[] GetForbiddenDlcIds()
	{
		return null;
	}

	public GameObject CreatePrefab()
	{
		GameObject gameObject = EntityTemplates.CreateBasicEntity("HighEnergyParticle", ITEMS.RADIATION.HIGHENERGYPARITCLE.NAME, ITEMS.RADIATION.HIGHENERGYPARITCLE.DESC, 1f, false, Assets.GetAnim("spark_radial_high_energy_particles_kanim"), "travel_pre", Grid.SceneLayer.FXFront2, SimHashes.Creature, null, 293f);
		EntityTemplates.AddCollision(gameObject, EntityTemplates.CollisionShape.CIRCLE, 0.2f, 0.2f);
		gameObject.AddOrGet<LoopingSounds>();
		RadiationEmitter radiationEmitter = gameObject.AddOrGet<RadiationEmitter>();
		radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
		radiationEmitter.radiusProportionalToRads = false;
		radiationEmitter.emitRadiusX = 3;
		radiationEmitter.emitRadiusY = 3;
		radiationEmitter.emitRads = 0.4f * ((float)radiationEmitter.emitRadiusX / 6f);
		gameObject.AddComponent<HighEnergyParticle>().speed = 8f;
		return gameObject;
	}

	public void OnPrefabInit(GameObject inst)
	{
	}

	public void OnSpawn(GameObject inst)
	{
	}

	public const int PARTICLE_SPEED = 8;

	public const float PARTICLE_COLLISION_SIZE = 0.2f;

	public const float PER_CELL_FALLOFF = 0.1f;

	public const float FALLOUT_RATIO = 0.5f;

	public const int MAX_PAYLOAD = 500;

	public const int EXPLOSION_FALLOUT_TEMPERATURE = 5000;

	public const float EXPLOSION_FALLOUT_MASS_PER_PARTICLE = 0.001f;

	public const float EXPLOSION_EMIT_DURRATION = 1f;

	public const short EXPLOSION_EMIT_RADIUS = 6;

	public const string ID = "HighEnergyParticle";
}
