using System;
using UnityEngine;

public class EffectPrefabs : MonoBehaviour
{
	public static EffectPrefabs Instance { get; private set; }

	private void Awake()
	{
		EffectPrefabs.Instance = this;
	}

	public GameObject Explosion;

	public GameObject AttackEffect;

	public GameObject OreAbsorb;

	public GameObject Fx;

	public GameObject ThoughtBubble;

	public GameObject PlantDeath;

	public GameObject PickupEffect;

	public GameObject MeteorBackground;

	public GameObject MeteorImpact;
}
