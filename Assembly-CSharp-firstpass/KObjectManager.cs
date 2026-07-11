using System;
using System.Collections.Generic;
using UnityEngine;

public class KObjectManager : MonoBehaviour
{
	public static KObjectManager Instance { get; private set; }

	public static void DestroyInstance()
	{
		KObjectManager.Instance = null;
	}

	private void Awake()
	{
		KObjectManager.Instance = this;
	}

	private void OnDestroy()
	{
		this.Cleanup();
		KObjectManager.Instance = null;
	}

	public void Cleanup()
	{
		foreach (KeyValuePair<int, KObject> keyValuePair in this.objects)
		{
			keyValuePair.Value.OnCleanUp();
		}
		this.objects.Clear();
		this.pendingDestroys.Clear();
	}

	public KObject GetOrCreateObject(GameObject go)
	{
		int instanceID = go.GetInstanceID();
		KObject kobject = null;
		if (!this.objects.TryGetValue(instanceID, out kobject))
		{
			kobject = new KObject(go);
			this.objects[instanceID] = kobject;
		}
		return kobject;
	}

	public KObject Get(GameObject go)
	{
		KObject kobject = null;
		this.objects.TryGetValue(go.GetInstanceID(), out kobject);
		return kobject;
	}

	public void QueueDestroy(KObject obj)
	{
		int id = obj.id;
		if (!this.pendingDestroys.Contains(id))
		{
			this.pendingDestroys.Add(id);
		}
	}

	private void LateUpdate()
	{
		for (int i = 0; i < this.pendingDestroys.Count; i++)
		{
			int num = this.pendingDestroys[i];
			KObject kobject = null;
			if (this.objects.TryGetValue(num, out kobject))
			{
				this.objects.Remove(num);
				kobject.OnCleanUp();
			}
		}
		this.pendingDestroys.Clear();
	}

	public void DumpEventData()
	{
	}

	private Dictionary<int, KObject> objects = new Dictionary<int, KObject>();

	private List<int> pendingDestroys = new List<int>();
}
