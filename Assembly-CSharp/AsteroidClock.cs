using System;
using UnityEngine;
using UnityEngine.UI;

public class AsteroidClock : MonoBehaviour
{
	private void Awake()
	{
		this.UpdateOverlay();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (GameClock.Instance != null)
		{
			this.rotationTransform.rotation = Quaternion.Euler(0f, 0f, 360f * -GameClock.Instance.GetCurrentCycleAsPercentage());
		}
	}

	private void UpdateOverlay()
	{
		float num = 0.125f;
		this.NightOverlay.fillAmount = num;
	}

	public Transform rotationTransform;

	public Image NightOverlay;
}
