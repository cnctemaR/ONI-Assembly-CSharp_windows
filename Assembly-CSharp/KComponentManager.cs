using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

public abstract class KComponentManager<T> : KCompactedVector<T> where T : new()
{
	protected KComponentManager()
		: base(0)
	{
	}

	protected HandleVector<int>.Handle InternalAddComponent(object instance, T cmp_values)
	{
		Assert.IsTrue(instance != null);
		Assert.IsTrue(!this.instanceHandleMap.ContainsKey(instance));
		HandleVector<int>.Handle handle = base.Allocate(cmp_values);
		this.instanceHandleMap[instance] = handle;
		Assert.IsTrue(handle.IsValid());
		return handle;
	}

	protected void InternalRemoveComponent(object instance)
	{
		if (!this.instanceHandleMap.ContainsKey(instance))
		{
			Output.LogWarning(new object[]
			{
				"Tried to remove component of type",
				typeof(T).ToString(),
				"on instance",
				instance.ToString(),
				"but instance has not been registered yet. Yog is investigating an Transition ordering issue that may be the source of this issue but for now we will exit with a warning"
			});
			return;
		}
		HandleVector<int>.Handle handle = this.instanceHandleMap[instance];
		base.Free(handle);
		this.instanceHandleMap.Remove(instance);
	}

	protected Dictionary<object, HandleVector<int>.Handle> instanceHandleMap = new Dictionary<object, HandleVector<int>.Handle>();
}
