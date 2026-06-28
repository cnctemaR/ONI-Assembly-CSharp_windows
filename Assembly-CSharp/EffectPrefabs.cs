using System;
using UnityEngine;

public class EffectPrefabs : MonoBehaviour
{
	public static EffectPrefabs Instance { get; private set; }

	private void Start()
	{
		EffectPrefabs.Instance = this;
	}

	public GameObject Explosion;

	public GameObject ResourceMelted;

	public GameObject BurnerFlame;

	public GameObject BurnerFlameContact;

	public GameObject BuildEffect;

	public GameObject PaintEffect;

	public GameObject HarvestEffect;

	public GameObject HarvestGlow;

	public GameObject AttackEffect;

	public GameObject WhirlpoolEffect;

	public GameObject DigEffect;

	public GameObject OreAbsorb;

	public GameObject Fx;

	public GameObject ThoughtBubble;

	public GameObject PlantDeath;

	public GameObject PickupEffect;
}
