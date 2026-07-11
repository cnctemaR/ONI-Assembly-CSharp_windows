using System;
using UnityEngine;

public class PerformanceMonitor : MonoBehaviour
{
	private void Update()
	{
		float deltaTime = Time.deltaTime;
		if (deltaTime <= 0.033333335f)
		{
			this.numFramesAbove30 += 1UL;
		}
		else
		{
			this.numFramesBelow30 += 1UL;
		}
	}

	public void Reset()
	{
		this.numFramesAbove30 = 0UL;
		this.numFramesBelow30 = 0UL;
	}

	public ulong NumFramesAbove30
	{
		get
		{
			return this.numFramesAbove30;
		}
	}

	public ulong NumFramesBelow30
	{
		get
		{
			return this.numFramesBelow30;
		}
	}

	private ulong numFramesAbove30;

	private ulong numFramesBelow30;

	private const float GOOD_FRAME_TIME = 0.033333335f;
}
