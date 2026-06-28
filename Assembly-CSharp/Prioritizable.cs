using System;
using KSerialization;
using UnityEngine;

public class Prioritizable : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		this.Subscribe(-905833192, new Action<object>(this.OnCopySettings));
	}

	private void OnCopySettings(object data)
	{
		GameObject gameObject = (GameObject)data;
		Prioritizable component = gameObject.GetComponent<Prioritizable>();
		if (component != null)
		{
			this.SetMasterPriority(component.GetMasterPriority());
		}
	}

	protected override void OnSpawn()
	{
		if (this.onPriorityChanged != null)
		{
			this.onPriorityChanged(this.masterPriority);
		}
		Components.Prioritizables.Add(this);
	}

	public int GetMasterPriority()
	{
		return this.masterPriority;
	}

	public void SetMasterPriority(int priority)
	{
		if (priority != this.masterPriority)
		{
			this.masterPriority = priority;
			if (this.onPriorityChanged != null)
			{
				this.onPriorityChanged(this.masterPriority);
			}
		}
	}

	public void AddRef()
	{
		this.refCount++;
	}

	public void RemoveRef()
	{
		this.refCount--;
	}

	public bool IsPrioritizable()
	{
		return this.refCount > 0;
	}

	protected override void OnCleanUp()
	{
		base.OnCleanUp();
		Components.Prioritizables.Remove(this);
	}

	public static void AddRef(GameObject go)
	{
		Prioritizable component = go.GetComponent<Prioritizable>();
		if (component != null)
		{
			component.AddRef();
		}
	}

	public static void RemoveRef(GameObject go)
	{
		Prioritizable component = go.GetComponent<Prioritizable>();
		if (component != null)
		{
			component.RemoveRef();
		}
	}

	[SerializeField]
	[Serialize]
	private int masterPriority = 5;

	public Action<int> onPriorityChanged;

	public bool showIcon = true;

	public Vector2 iconOffset;

	public float iconScale = 1f;

	[SerializeField]
	private int refCount;
}
