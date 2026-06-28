using System;
using UnityEngine;

public class Cycler : MonoBehaviour
{
	private void Update()
	{
		if (BatchedAnimBulkDisplayer.RunCycler)
		{
			this.timer += Time.deltaTime;
			if (this.timer > this.maxPerAnim)
			{
				this.timer = 0f;
				this.Next();
			}
		}
	}

	protected virtual void Next()
	{
	}

	private float timer = 0f;

	public float maxPerAnim = 5f;
}
