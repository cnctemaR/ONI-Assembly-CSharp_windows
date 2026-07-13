using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/ScheduledUIInstantiation")]
public class ScheduledUIInstantiation : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (this.InstantiateOnAwake)
		{
			this.InstantiateElements(null);
			return;
		}
		Game.Instance.Subscribe((int)this.InstantiationEvent, new Action<object>(this.InstantiateElements));
	}

	public void InstantiateElements(object data)
	{
		if (this.completed)
		{
			return;
		}
		this.completed = true;
		foreach (ScheduledUIInstantiation.Instantiation instantiation in this.UIElements)
		{
			if (instantiation.RequiredDlcId.IsNullOrWhiteSpace() || Game.IsDlcActiveForCurrentSave(instantiation.RequiredDlcId))
			{
				foreach (GameObject gameObject in instantiation.prefabs)
				{
					Vector3 vector = gameObject.rectTransform().anchoredPosition;
					GameObject gameObject2 = Util.KInstantiateUI(gameObject, instantiation.parent.gameObject, false);
					gameObject2.rectTransform().anchoredPosition = vector;
					gameObject2.rectTransform().localScale = Vector3.one;
					this.instantiatedObjects.Add(gameObject2);
				}
			}
		}
		if (!this.InstantiateOnAwake)
		{
			base.Unsubscribe((int)this.InstantiationEvent, new Action<object>(this.InstantiateElements));
		}
	}

	public T GetInstantiatedObject<T>() where T : Component
	{
		for (int i = 0; i < this.instantiatedObjects.Count; i++)
		{
			if (this.instantiatedObjects[i].GetComponent(typeof(T)) != null)
			{
				return this.instantiatedObjects[i].GetComponent(typeof(T)) as T;
			}
		}
		return default(T);
	}

	public ScheduledUIInstantiation.Instantiation[] UIElements;

	public bool InstantiateOnAwake;

	public GameHashes InstantiationEvent = GameHashes.StartGameUser;

	private bool completed;

	private List<GameObject> instantiatedObjects = new List<GameObject>();

	[Serializable]
	public struct Instantiation
	{
		public string Name;

		public string Comment;

		public GameObject[] prefabs;

		public Transform parent;

		public string RequiredDlcId;
	}
}
