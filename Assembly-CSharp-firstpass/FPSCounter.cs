using System;
using UnityEngine;

public class FPSCounter
{
	public static void Create()
	{
		if (FPSCounter.instance == null)
		{
			FPSCounter.instance = new FPSCounter();
			FPSCounter.instance.FPSNextPeriod = Time.realtimeSinceStartup + FPSCounter.FPSMeasurePeriod;
		}
	}

	public static void Update()
	{
		FPSCounter.Create();
		FPSCounter.instance.FPSAccumulator++;
		if (Time.realtimeSinceStartup > FPSCounter.instance.FPSNextPeriod)
		{
			FPSCounter.currentFPS = (int)((float)FPSCounter.instance.FPSAccumulator / FPSCounter.FPSMeasurePeriod);
			FPSCounter.instance.FPSAccumulator = 0;
			FPSCounter.instance.FPSNextPeriod += FPSCounter.FPSMeasurePeriod;
		}
	}

	public static float FPSMeasurePeriod = 0.1f;

	private int FPSAccumulator;

	private float FPSNextPeriod;

	public static int currentFPS;

	private static FPSCounter instance;
}
