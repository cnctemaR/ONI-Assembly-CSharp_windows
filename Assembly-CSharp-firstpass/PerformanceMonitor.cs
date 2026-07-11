using System;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceMonitor : MonoBehaviour
{
	private void Update()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		float unscaledDeltaTime = Time.unscaledDeltaTime;
		if (unscaledDeltaTime <= 0.033333335f)
		{
			this.numFramesAbove30 += 1UL;
		}
		else
		{
			this.numFramesBelow30 += 1UL;
		}
		if (this.frameTimes.Count == PerformanceMonitor.frameRateWindowSize)
		{
			LinkedListNode<float> first = this.frameTimes.First;
			this.frameTimeTotal -= first.Value;
			this.frameTimes.RemoveFirst();
			first.Value = unscaledDeltaTime;
			this.frameTimes.AddLast(first);
		}
		else
		{
			this.frameTimes.AddLast(unscaledDeltaTime);
		}
		this.frameTimeTotal += unscaledDeltaTime;
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

	public float FPS
	{
		get
		{
			if (this.frameTimeTotal != 0f)
			{
				return (float)this.frameTimes.Count / this.frameTimeTotal;
			}
			return 0f;
		}
	}

	private ulong numFramesAbove30;

	private ulong numFramesBelow30;

	private LinkedList<float> frameTimes = new LinkedList<float>();

	private float frameTimeTotal;

	private static readonly int frameRateWindowSize = 150;

	private const float GOOD_FRAME_TIME = 0.033333335f;
}
