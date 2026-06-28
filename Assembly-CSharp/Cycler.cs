using System;
using UnityEngine;

public class Cycler : MonoBehaviour
{
	private void Update()
	{
		if (!BatchedAnimBulkDisplayer.RunCycler)
		{
			return;
		}
		this.timer += Time.deltaTime;
		if (this.timer > this.maxPerAnim)
		{
			this.timer = 0f;
			this.Next();
		}
	}

	protected virtual void Next()
	{
	}

	private float timer;

	public float maxPerAnim = 5f;
}
