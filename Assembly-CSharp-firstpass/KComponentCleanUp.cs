using System;
using UnityEngine;

public class KComponentCleanUp : MonoBehaviour
{
	public static void SetInCleanUpPhase(bool in_cleanup_phase)
	{
		KComponentCleanUp.inCleanUpPhase = in_cleanup_phase;
	}

	public static bool InCleanUpPhase
	{
		get
		{
			return KComponentCleanUp.inCleanUpPhase;
		}
	}

	private void Awake()
	{
		KComponentCleanUp.instance = this;
		this.comps = base.GetComponent<KComponentSpawn>().comps;
	}

	private void FixedUpdate()
	{
		KComponentCleanUp.SetInCleanUpPhase(true);
	}

	private void Update()
	{
		KComponentCleanUp.SetInCleanUpPhase(true);
		this.comps.CleanUp();
	}

	public static KComponentCleanUp instance;

	private static bool inCleanUpPhase = false;

	private KComponents comps;
}
