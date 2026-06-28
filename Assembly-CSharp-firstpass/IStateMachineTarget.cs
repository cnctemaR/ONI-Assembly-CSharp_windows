using System;
using UnityEngine;

public interface IStateMachineTarget
{
	void Subscribe(int hash, EventSystem.EventHandler handler);

	void Unsubscribe(int hash, EventSystem.EventHandler handler);

	void Trigger(int hash, object data = null);

	ComponentType GetComponent<ComponentType>();

	GameObject gameObject { get; }

	Transform transform { get; }

	string name { get; }

	bool isNull { get; }
}
