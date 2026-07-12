using System;
using UnityEngine;

[Serializable]
public class DefComponent<T> where T : Component
{
	public DefComponent(T cmp)
	{
		this.cmp = cmp;
	}

	public T Get(StateMachine.Instance smi)
	{
		T[] components = this.cmp.GetComponents<T>();
		int num = 0;
		while (num < components.Length && !(components[num] == this.cmp))
		{
			num++;
		}
		return smi.gameObject.GetComponents<T>()[num];
	}

	public static implicit operator DefComponent<T>(T cmp)
	{
		return new DefComponent<T>(cmp);
	}

	[SerializeField]
	private T cmp;
}
