using System;
using UnityEngine;

public class Timer
{
	public void Start()
	{
		if (!this.isStarted)
		{
			this.startTime = Time.time;
			this.isStarted = true;
		}
	}

	public void Stop()
	{
		this.isStarted = false;
	}

	public float GetElapsed()
	{
		return Time.time - this.startTime;
	}

	public bool TryStop(float elapsed_time)
	{
		bool flag;
		if (this.isStarted && this.GetElapsed() >= elapsed_time)
		{
			this.Stop();
			flag = true;
		}
		else
		{
			flag = false;
		}
		return flag;
	}

	private float startTime;

	private bool isStarted;
}
