using System;
using UnityEngine;

public class GravityComponents : KGameObjectComponentManager<GravityComponent>
{
	public HandleVector<int>.Handle Add(GameObject go, Vector2 initial_velocity, global::System.Action on_landed = null)
	{
		return base.Add(go, new GravityComponent(go.transform, on_landed, initial_velocity));
	}

	public override void FixedUpdate(float dt)
	{
		for (int i = 0; i < this.data.Count; i++)
		{
			this.data[i].FixedUpdate(dt);
		}
	}
}
