using System;
using KSerialization;
using UnityEngine;

namespace Klei.AI
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class ModifierInstance<ModifierType> : IStateMachineTarget, ISaveLoadableJson
	{
		public ModifierInstance(GameObject game_object, ModifierType modifier)
		{
			this.gameObject = game_object;
			this.modifier = modifier;
		}

		public GameObject gameObject { get; private set; }

		public ComponentType GetComponent<ComponentType>()
		{
			return this.gameObject.GetComponent<ComponentType>();
		}

		public void Subscribe(int hash, EventSystem.EventHandler handler)
		{
			this.gameObject.GetComponent<KMonoBehaviour>().Subscribe(hash, handler);
		}

		public void Unsubscribe(int hash, EventSystem.EventHandler handler)
		{
			this.gameObject.GetComponent<KMonoBehaviour>().Unsubscribe(hash, handler);
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
