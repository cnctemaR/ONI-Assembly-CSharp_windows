using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIPrefabLocalPool
{
	public UIPrefabLocalPool(GameObject sourcePrefab, GameObject parent)
	{
		this.sourcePrefab = sourcePrefab;
		this.parent = parent;
	}

	public GameObject Borrow()
	{
		GameObject gameObject;
		if (this.availableInstances.Count == 0)
		{
			gameObject = Util.KInstantiateUI(this.sourcePrefab, this.parent, true);
		}
		else
		{
			gameObject = this.availableInstances.First<KeyValuePair<int, GameObject>>().Value;
			this.availableInstances.Remove(gameObject.GetInstanceID());
		}
		this.checkedOutInstances.Add(gameObject.GetInstanceID(), gameObject);
		gameObject.SetActive(true);
		gameObject.transform.SetAsLastSibling();
		return gameObject;
	}

	public void Return(GameObject instance)
	{
		this.checkedOutInstances.Remove(instance.GetInstanceID());
		this.availableInstances.Add(instance.GetInstanceID(), instance);
		instance.SetActive(false);
	}

	public void ReturnAll()
	{
		foreach (KeyValuePair<int, GameObject> keyValuePair in this.checkedOutInstances)
		{
			int num;
			GameObject gameObject;
			keyValuePair.Deconstruct<int, GameObject>(out num, out gameObject);
			int num2 = num;
			GameObject gameObject2 = gameObject;
			this.availableInstances.Add(num2, gameObject2);
			gameObject2.SetActive(false);
		}
		this.checkedOutInstances.Clear();
	}

	public IEnumerable<GameObject> GetBorrowedObjects()
	{
		return this.checkedOutInstances.Values;
	}

	public readonly GameObject sourcePrefab;

	public readonly GameObject parent;

	private Dictionary<int, GameObject> checkedOutInstances = new Dictionary<int, GameObject>();

	private Dictionary<int, GameObject> availableInstances = new Dictionary<int, GameObject>();
}
