using System;
using System.Collections.Generic;

public class KComponents
{
	public virtual void Shutdown()
	{
		this.managers.Clear();
	}

	protected T Add<T>(T manager) where T : IComponentManager
	{
		this.managers.Add(manager);
		return manager;
	}

	public void Spawn()
	{
		if (this.spawned)
		{
			return;
		}
		this.spawned = true;
		foreach (IComponentManager componentManager in this.managers)
		{
			componentManager.Spawn();
		}
	}

	public void Sim33ms(float dt)
	{
		foreach (IComponentManager componentManager in this.managers)
		{
			componentManager.FixedUpdate(dt);
		}
	}

	public void RenderEveryTick(float dt)
	{
		foreach (IComponentManager componentManager in this.managers)
		{
			componentManager.RenderEveryTick(dt);
		}
	}

	public void Sim200ms(float dt)
	{
		foreach (IComponentManager componentManager in this.managers)
		{
			componentManager.Sim200ms(dt);
		}
	}

	public void CleanUp()
	{
		foreach (IComponentManager componentManager in this.managers)
		{
			componentManager.CleanUp();
		}
		this.spawned = false;
	}

	public void Clear()
	{
		foreach (IComponentManager componentManager in this.managers)
		{
			componentManager.Clear();
		}
		this.spawned = false;
	}

	private List<IComponentManager> managers = new List<IComponentManager>();

	private bool spawned;
}
