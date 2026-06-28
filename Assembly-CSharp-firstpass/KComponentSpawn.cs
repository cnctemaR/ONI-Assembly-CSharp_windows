using System;
using UnityEngine;

public class KComponentSpawn : MonoBehaviour
{
	private void FixedUpdate()
	{
		KComponentCleanUp.SetInCleanUpPhase(false);
		this.comps.Spawn();
		this.comps.FixedUpdate(Time.fixedDeltaTime);
	}

	private void Update()
	{
		KComponentCleanUp.SetInCleanUpPhase(false);
		this.comps.Spawn();
		this.comps.Update(Time.deltaTime);
	}

	private void SimUpdate(float dt)
	{
		this.comps.SimUpdate(dt);
	}

	private void OnApplicationQuit()
	{
		this.comps.Shutdown();
	}

	public static KComponentSpawn instance;

	public KComponents comps;
}
