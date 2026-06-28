using System;
using UnityEngine;

public interface IStateMachineTarget
{
	int Subscribe(int hash, Action<object> handler);

	void Unsubscribe(int hash, Action<object> handler);

	void Unsubscribe(int id);

	void Trigger(int hash, object data = null);

	ComponentType GetComponent<ComponentType>();

	GameObject gameObject { get; }

	Transform transform { get; }

	string name { get; }

	bool isNull { get; }
}
