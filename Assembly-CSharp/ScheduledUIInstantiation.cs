using System;
using UnityEngine;

public class ScheduledUIInstantiation : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		if (this.InstantiateOnAwake)
		{
			this.InstantiateElements(null);
		}
		else
		{
			Game.Instance.Subscribe((int)this.InstantiationEvent, new Action<object>(this.InstantiateElements));
		}
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
			foreach (GameObject gameObject in instantiation.prefabs)
			{
				Vector3 vector = gameObject.rectTransform().anchoredPosition;
				GameObject gameObject2 = Util.KInstantiateUI(gameObject, instantiation.parent.gameObject, false);
				gameObject2.rectTransform().anchoredPosition = vector;
				gameObject2.rectTransform().localScale = Vector3.one;
			}
		}
		if (!this.InstantiateOnAwake)
		{
			this.Unsubscribe((int)this.InstantiationEvent, new Action<object>(this.InstantiateElements));
		}
	}

	public ScheduledUIInstantiation.Instantiation[] UIElements;

	public bool InstantiateOnAwake;

	public GameHashes InstantiationEvent = GameHashes.StartGameUser;

	private bool completed;

	[Serializable]
	public struct Instantiation
	{
		public string Name;

		public string Comment;

		public GameObject[] prefabs;

		public Transform parent;
	}
}
