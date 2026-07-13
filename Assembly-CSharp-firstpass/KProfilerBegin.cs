using System;
using System.Collections;
using UnityEngine;

public class KProfilerBegin : MonoBehaviour
{
	private void Start()
	{
		global::Debug.Log("KProfiler: Start");
		base.StartCoroutine(this.FrameTicker());
		KProfiler.BeginThread("Main", "Game");
	}

	private IEnumerator FrameTicker()
	{
		for (;;)
		{
			yield return this.wait;
			KProfiler.NextFrame();
		}
		yield break;
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
	}

	public static int begin_counter;

	private WaitForEndOfFrame wait = new WaitForEndOfFrame();
}
