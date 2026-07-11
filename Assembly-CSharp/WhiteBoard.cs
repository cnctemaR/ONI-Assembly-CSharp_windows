using System;
using System.Collections.Generic;
using UnityEngine;

public class WhiteBoard : KGameObjectComponentManager<WhiteBoard.Data>, IKComponentManager
{
	public HandleVector<int>.Handle Add(GameObject go)
	{
		return base.Add(go, new WhiteBoard.Data
		{
			keyValueStore = new Dictionary<HashedString, object>()
		});
	}

	protected override void OnCleanUp(HandleVector<int>.Handle h)
	{
		WhiteBoard.Data data = base.GetData(h);
		data.keyValueStore.Clear();
		data.keyValueStore = null;
		base.SetData(h, data);
	}

	public bool HasValue(HandleVector<int>.Handle h, HashedString key)
	{
		return h.IsValid() && base.GetData(h).keyValueStore.ContainsKey(key);
	}

	public object GetValue(HandleVector<int>.Handle h, HashedString key)
	{
		return base.GetData(h).keyValueStore[key];
	}

	public void SetValue(HandleVector<int>.Handle h, HashedString key, object value)
	{
		if (!h.IsValid())
		{
			return;
		}
		WhiteBoard.Data data = base.GetData(h);
		data.keyValueStore[key] = value;
		base.SetData(h, data);
	}

	public void RemoveValue(HandleVector<int>.Handle h, HashedString key)
	{
		if (!h.IsValid())
		{
			return;
		}
		WhiteBoard.Data data = base.GetData(h);
		data.keyValueStore.Remove(key);
		base.SetData(h, data);
	}

	public struct Data
	{
		public Dictionary<HashedString, object> keyValueStore;
	}
}
