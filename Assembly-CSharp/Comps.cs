using System;
using System.Collections.Generic;

public class Comps
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

	public void FixedUpdate(float dt)
	{
		foreach (IComponentManager componentManager in this.managers)
		{
			componentManager.FixedUpdate(dt);
		}
	}

	public void Update(float dt)
	{
		foreach (IComponentManager componentManager in this.managers)
		{
			componentManager.Update(dt);
		}
	}

	private List<IComponentManager> managers = new List<IComponentManager>();
}
