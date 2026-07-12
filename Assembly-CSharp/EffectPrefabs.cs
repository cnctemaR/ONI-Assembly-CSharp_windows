using System;
using UnityEngine;

public class EffectPrefabs : MonoBehaviour
{
	public static EffectPrefabs Instance { get; private set; }

	private void Awake()
	{
		EffectPrefabs.Instance = this;
	}

	public GameObject DreamBubble;

	public GameObject ThoughtBubble;

	public GameObject ThoughtBubbleConvo;

	public GameObject MeteorBackground;

	public GameObject SparkleStreakFX;

	public GameObject HappySingerFX;

	public GameObject HugFrenzyFX;

	public GameObject GameplayEventDisplay;

	public GameObject OpenTemporalTearBeam;

	public GameObject MissileSmokeTrailFX;
}
