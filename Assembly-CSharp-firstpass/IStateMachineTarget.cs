using System;
using UnityEngine;

public interface IStateMachineTarget
{
	int Subscribe(int hash, Action<object> handler);

	int Subscribe(int hash, Action<object, object> handler, object context);

	void Unsubscribe(int hash, Action<object> handler);

	void Unsubscribe(int id);

	void Unsubscribe(ref int id)
	{
		this.Unsubscribe(id);
		id = -1;
	}

	void Trigger(int hash, object data = null);

	ComponentType GetComponent<ComponentType>();

	GameObject gameObject { get; }

	Transform transform { get; }

	string name { get; }

	bool isNull { get; }
}
