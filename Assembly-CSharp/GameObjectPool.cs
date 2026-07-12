using System;
using UnityEngine;

public class GameObjectPool : ObjectPool<GameObject>
{
	public GameObjectPool(Func<GameObject> instantiator, int initial_count = 0)
		: base(instantiator, initial_count)
	{
	}

	public override GameObject GetInstance()
	{
		return base.GetInstance();
	}

	public void Destroy()
	{
		for (int i = this.unused.Count - 1; i >= 0; i--)
		{
			global::UnityEngine.Object.Destroy(this.unused.Pop());
		}
	}
}
