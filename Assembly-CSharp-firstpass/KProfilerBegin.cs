using System;
using UnityEngine;

public class KProfilerBegin : MonoBehaviour
{
	private void Start()
	{
		global::Debug.Log("KProfiler: Start");
		KProfiler.BeginThread("Main", "Game");
	}

	private void Update()
	{
		KProfiler.BeginFrame();
	}

	public static int begin_counter;
}
