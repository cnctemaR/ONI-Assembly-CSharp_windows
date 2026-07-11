using System;
using UnityEngine;

namespace Klei.AI
{
	public class ModifierInstance<ModifierType> : IStateMachineTarget
	{
		public GameObject gameObject { get; private set; }

		public ModifierInstance(GameObject game_object, ModifierType modifier)
		{
			this.gameObject = game_object;
			this.modifier = modifier;
		}

		public ComponentType GetComponent<ComponentType>()
		{
			return this.gameObject.GetComponent<ComponentType>();
		}

		public int Subscribe(int hash, Action<object> handler)
		{
			return this.gameObject.GetComponent<KMonoBehaviour>().Subscribe(hash, handler);
		}

		public void Unsubscribe(int hash, Action<object> handler)
		{
			this.gameObject.GetComponent<KMonoBehaviour>().Unsubscribe(hash, handler);
		}

		public void Unsubscribe(int id)
		{
			this.gameObject.GetComponent<KMonoBehaviour>().Unsubscribe(id);
		}

		public void Trigger(int hash, object data = null)
		{
			this.gameObject.GetComponent<KPrefabID>().Trigger(hash, data);
		}

		public Transform transform
		{
			get
			{
				return this.gameObject.transform;
			}
		}

		public bool isNull
		{
			get
			{
				return this.gameObject == null;
			}
		}

		public string name
		{
			get
			{
				return this.gameObject.name;
			}
		}

		public virtual void OnCleanUp()
		{
		}

		public ModifierType modifier;
	}
}
