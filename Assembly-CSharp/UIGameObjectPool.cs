using System;
using System.Collections.Generic;
using UnityEngine;

public class UIGameObjectPool
{
	public UIGameObjectPool(GameObject prefab)
	{
		this.prefab = prefab;
		this.freeElements = new List<GameObject>();
		this.activeElements = new List<GameObject>();
	}

	public int ActiveElementsCount
	{
		get
		{
			return this.activeElements.Count;
		}
	}

	public int FreeElementsCount
	{
		get
		{
			return this.freeElements.Count;
		}
	}

	public int TotalElementsCount
	{
		get
		{
			return this.ActiveElementsCount + this.FreeElementsCount;
		}
	}

	public GameObject GetFreeElement(GameObject instantiateParent = null, bool forceActive = false)
	{
		if (this.freeElements.Count == 0)
		{
			this.activeElements.Add(Util.KInstantiateUI(this.prefab.gameObject, instantiateParent, false));
		}
		else
		{
			GameObject gameObject = this.freeElements[0];
			this.activeElements.Add(gameObject);
			if (gameObject.transform.parent != instantiateParent)
			{
				gameObject.transform.SetParent(instantiateParent.transform);
			}
			this.freeElements.RemoveAt(0);
		}
		GameObject gameObject2 = this.activeElements[this.activeElements.Count - 1];
		if (gameObject2.gameObject.activeInHierarchy != forceActive)
		{
			gameObject2.gameObject.SetActive(forceActive);
		}
		return gameObject2;
	}

	public void ClearElement(GameObject element)
	{
		if (!this.activeElements.Contains(element))
		{
			string text = ((!this.freeElements.Contains(element)) ? (element.name + ": The element provided does not belong to this pool") : (element.name + ": The element provided is already inactive"));
			element.SetActive(false);
			if (this.disabledElementParent != null)
			{
				element.transform.SetParent(this.disabledElementParent);
			}
			global::Debug.LogError(text, null);
			return;
		}
		if (this.disabledElementParent != null)
		{
			element.transform.SetParent(this.disabledElementParent);
		}
		element.SetActive(false);
		this.freeElements.Add(element);
		this.activeElements.Remove(element);
	}

	public void ClearAll()
	{
		while (this.activeElements.Count > 0)
		{
			if (this.disabledElementParent != null)
			{
				this.activeElements[0].transform.SetParent(this.disabledElementParent);
			}
			this.activeElements[0].SetActive(false);
			this.freeElements.Add(this.activeElements[0]);
			this.activeElements.RemoveAt(0);
		}
	}

	public void DestroyAll()
	{
		this.DestroyAllActive();
		this.DestroyAllFree();
	}

	public void DestroyAllActive()
	{
		this.activeElements.ForEach(delegate(GameObject ae)
		{
			global::UnityEngine.Object.Destroy(ae);
		});
		this.activeElements.Clear();
	}

	public void DestroyAllFree()
	{
		this.freeElements.ForEach(delegate(GameObject ae)
		{
			global::UnityEngine.Object.Destroy(ae);
		});
		this.freeElements.Clear();
	}

	public void ForEachActiveElement(Action<GameObject> predicate)
	{
		this.activeElements.ForEach(predicate);
	}

	public void ForEachFreeElement(Action<GameObject> predicate)
	{
		this.freeElements.ForEach(predicate);
	}

	private GameObject prefab;

	private List<GameObject> freeElements = new List<GameObject>();

	private List<GameObject> activeElements = new List<GameObject>();

	public Transform disabledElementParent;
}
