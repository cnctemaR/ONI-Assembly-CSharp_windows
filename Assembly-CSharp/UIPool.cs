using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPool<T> where T : MonoBehaviour
{
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

	public UIPool(T prefab)
	{
		this.prefab = prefab;
		this.freeElements = new Stack<T>();
		this.activeElements = new List<T>();
	}

	public T GetFreeElement(GameObject instantiateParent = null, bool forceActive = false)
	{
		T t;
		if (this.freeElements.Count == 0)
		{
			t = Util.KInstantiateUI<T>(this.prefab.gameObject, instantiateParent, false);
		}
		else
		{
			t = this.freeElements.Pop();
			if (t.transform.parent != instantiateParent)
			{
				t.transform.SetParent((instantiateParent != null) ? instantiateParent.transform : null);
			}
		}
		if (t.gameObject.activeInHierarchy != forceActive)
		{
			t.gameObject.SetActive(forceActive);
		}
		this.activeElements.Add(t);
		return t;
	}

	public void ClearElement(T element)
	{
		if (!this.activeElements.Contains(element))
		{
			global::Debug.LogError(this.freeElements.Contains(element) ? "The element provided is already inactive" : "The element provided does not belong to this pool");
			return;
		}
		if (this.disabledElementParent != null)
		{
			element.transform.SetParent(this.disabledElementParent);
		}
		element.gameObject.SetActive(false);
		this.freeElements.Push(element);
		this.activeElements.Remove(element);
	}

	public void ClearAll()
	{
		for (int i = this.activeElements.Count - 1; i >= 0; i--)
		{
			T t = this.activeElements[i];
			t.gameObject.SetActive(false);
			if (this.disabledElementParent != null)
			{
				t.transform.SetParent(this.disabledElementParent);
			}
			this.freeElements.Push(t);
		}
		this.activeElements.Clear();
	}

	public void DestroyAll()
	{
		this.DestroyAllActive();
		this.DestroyAllFree();
	}

	public void DestroyAllActive()
	{
		foreach (T t in this.activeElements)
		{
			global::UnityEngine.Object.Destroy(t.gameObject);
		}
		this.activeElements.Clear();
	}

	public void DestroyAllFree()
	{
		foreach (T t in this.freeElements)
		{
			global::UnityEngine.Object.Destroy(t.gameObject);
		}
		this.freeElements.Clear();
	}

	public void ForEachActiveElement(Action<T> predicate)
	{
		for (int i = 0; i < this.activeElements.Count; i++)
		{
			predicate(this.activeElements[i]);
		}
	}

	public void ForEachFreeElement(Action<T> predicate)
	{
		foreach (T t in this.freeElements)
		{
			predicate(t);
		}
	}

	private T prefab;

	private Stack<T> freeElements;

	private List<T> activeElements;

	public Transform disabledElementParent;
}
