using System;
using UnityEngine.Profiling;

public abstract class KGameObjectComponentManager2<T> : KGameObjectComponentManager<T> where T : IComponent, new()
{
	public KGameObjectComponentManager2(bool do_update, bool do_sim_update, bool do_fixed_update)
	{
		this.doUpdate = do_update;
		this.doSimUpdate = do_sim_update;
		this.doFixedUpdate = do_fixed_update;
	}

	public override void Update(float dt)
	{
		if (this.doUpdate)
		{
			Profiler.BeginSample(base.GetType().Name);
			for (int i = 0; i < this.data.Count; i++)
			{
				T t = this.data[i];
				t.Update(dt);
			}
			Profiler.EndSample();
		}
	}

	public override void FixedUpdate(float dt)
	{
		if (!this.doFixedUpdate)
		{
		}
	}

	public override void SimUpdate(float dt)
	{
		if (!this.doSimUpdate)
		{
		}
	}

	private bool doUpdate;

	private bool doSimUpdate;

	private bool doFixedUpdate;
}
