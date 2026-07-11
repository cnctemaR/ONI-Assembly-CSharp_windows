using System;
using System.Collections.Generic;
using UnityEngine;

public class UIPool<T> where T : MonoBehaviour
{
	public UIPool(T prefab)
	{
		this.prefab = prefab;
		this.freeElements = new List<T>();
		this.activeElements = new List<T>();
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

	public T GetFreeElement(GameObject instantiateParent = null, bool forceActive = false)
	{
		if (this.freeElements.Count == 0)
		{
			this.activeElements.Add(Util.KInstantiateUI<T>(this.prefab.gameObject, instantiateParent, false));
		}
		else
		{
			T t = this.freeElements[0];
			this.activeElements.Add(t);
			if (t.transform.parent != instantiateParent)
			{
				t.transform.SetParent(instantiateParent.transform);
			}
			this.freeElements.RemoveAt(0);
		}
		T t2 = this.activeElements[this.activeElements.Count - 1];
		if (t2.gameObject.activeInHierarchy != forceActive)
		{
			t2.gameObject.SetActive(forceActive);
		}
		return t2;
	}

	public void ClearElement(T element)
	{
		if (!this.activeElements.Contains(element))
		{
			string text = ((!this.freeElements.Contains(element)) ? "The element provided does not belong to this pool" : "The element provided is already inactive");
			global::Debug.LogError(text);
			return;
		}
		if (this.disabledElementParent != null)
		{
			element.gameObject.transform.SetParent(this.disabledElementParent);
		}
		element.gameObject.SetActive(false);
		this.freeElements.Add(element);
		this.activeElements.Remove(element);
	}

	public void ClearAll()
	{
		while (this.activeElements.Count > 0)
		{
			if (this.disabledElementParent != null)
			{
				T t = this.activeElements[0];
				t.gameObject.transform.SetParent(this.disabledElementParent);
			}
			T t2 = this.activeElements[0];
			t2.gameObject.SetActive(false);
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
		this.activeElements.ForEach(delegate(T ae)
		{
			global::UnityEngine.Object.Destroy(ae.gameObject);
		});
		this.activeElements.Clear();
	}

	public void DestroyAllFree()
	{
		this.freeElements.ForEach(delegate(T ae)
		{
			global::UnityEngine.Object.Destroy(ae.gameObject);
		});
		this.freeElements.Clear();
	}

	public void ForEachActiveElement(Action<T> predicate)
	{
		this.activeElements.ForEach(predicate);
	}

	public void ForEachFreeElement(Action<T> predicate)
	{
		this.freeElements.ForEach(predicate);
	}

	private T prefab;

	private List<T> freeElements = new List<T>();

	private List<T> activeElements = new List<T>();

	public Transform disabledElementParent;
}
