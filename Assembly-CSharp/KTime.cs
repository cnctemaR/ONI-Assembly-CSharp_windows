using System;
using UnityEngine;

public class KTime : KMonoBehaviour
{
	public float UnscaledGameTime { get; set; }

	public static KTime Instance { get; private set; }

	protected override void OnPrefabInit()
	{
		KTime.Instance = this;
		this.UnscaledGameTime = Time.unscaledTime;
	}

	protected override void OnCleanUp()
	{
		KTime.Instance = null;
	}

	public void Update()
	{
		if (!SpeedControlScreen.Instance.IsPaused)
		{
			this.UnscaledGameTime += Time.unscaledDeltaTime;
		}
	}
}
