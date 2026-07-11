using System;
using UnityEngine;

public class EffectPrefabs : MonoBehaviour
{
	public static EffectPrefabs Instance { get; private set; }

	private void Awake()
	{
		EffectPrefabs.Instance = this;
	}

	public GameObject ThoughtBubble;

	public GameObject ThoughtBubbleConvo;

	public GameObject MeteorBackground;

	public GameObject SparkleStreakFX;
}
