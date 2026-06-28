using System;
using UnityEngine;

public class KComponentSpawn : MonoBehaviour, ISim200ms, ISim33ms
{
	private void FixedUpdate()
	{
		KComponentCleanUp.SetInCleanUpPhase(false);
		this.comps.Spawn();
	}

	private void Update()
	{
		KComponentCleanUp.SetInCleanUpPhase(false);
		this.comps.Spawn();
		this.comps.RenderEveryTick(Time.deltaTime);
	}

	public void Sim33ms(float dt)
	{
		this.comps.Sim33ms(dt);
	}

	public void Sim200ms(float dt)
	{
		this.comps.Sim200ms(dt);
	}

	private void OnApplicationQuit()
	{
		this.comps.Shutdown();
	}

	public static KComponentSpawn instance;

	public KComponents comps;
}
