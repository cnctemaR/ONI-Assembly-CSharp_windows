using System;
using UnityEngine;

public class InstantiateUIPrefabChild : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		if (this.InstantiateOnAwake)
		{
			this.Instantiate();
		}
	}

	public void Instantiate()
	{
		if (this.alreadyInstantiated)
		{
			Debug.LogWarning(base.gameObject.name + "trying to instantiate UI prefabs multiple times.");
			return;
		}
		this.alreadyInstantiated = true;
		foreach (GameObject gameObject in this.prefabs)
		{
			if (!(gameObject == null))
			{
				Vector3 vector = gameObject.rectTransform().anchoredPosition;
				GameObject gameObject2 = global::UnityEngine.Object.Instantiate<GameObject>(gameObject);
				gameObject2.transform.SetParent(this.transform);
				gameObject2.rectTransform().anchoredPosition = vector;
				gameObject2.rectTransform().localScale = Vector3.one;
				if (this.setAsFirstSibling)
				{
					gameObject2.transform.SetAsFirstSibling();
				}
			}
		}
	}

	public GameObject[] prefabs;

	public bool InstantiateOnAwake = true;

	private bool alreadyInstantiated;

	public bool setAsFirstSibling;
}
