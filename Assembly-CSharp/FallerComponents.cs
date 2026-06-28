using System;
using UnityEngine;

public class FallerComponents : KGameObjectComponentManager<FallerComponent>
{
	public HandleVector<int>.Handle Add(GameObject go, Vector2 initial_velocity)
	{
		return base.Add(go, new FallerComponent(go.transform, initial_velocity));
	}

	public override void Remove(GameObject go)
	{
		int num = this.handles.Items[this.instanceHandleMap[go].index];
		if (num >= 0 && num < this.data.Count)
		{
			this.data[num].Destroy();
			base.Remove(go);
		}
		else
		{
			Debug.LogError("Invalid handle index.");
		}
	}

	public override void FixedUpdate(float dt)
	{
		for (int i = 0; i < this.data.Count; i++)
		{
			this.data[i].FixedUpdate(dt);
		}
	}
}
