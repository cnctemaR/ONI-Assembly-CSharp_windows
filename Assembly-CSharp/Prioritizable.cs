using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using KSerialization;
using UnityEngine;

public class Prioritizable : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		base.Subscribe(-905833192, new Action<object>(this.OnCopySettings));
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

	[OnDeserialized]
	private void OnDeserialized()
	{
		if (this.masterPriority != -2147483648)
		{
			this.masterPrioritySetting = new PrioritySetting(PriorityScreen.PriorityClass.basic, 5);
			this.masterPriority = int.MinValue;
		}
		PrioritySetting prioritySetting;
		if (SaveLoader.Instance.GameInfo.IsVersionExactly(7, 2) && Prioritizable.conversions.TryGetValue(this.masterPrioritySetting, out prioritySetting))
		{
			this.masterPrioritySetting = prioritySetting;
		}
	}

	protected override void OnSpawn()
	{
		if (this.onPriorityChanged != null)
		{
			this.onPriorityChanged(this.masterPrioritySetting);
		}
		Components.Prioritizables.Add(this);
	}

	public PrioritySetting GetMasterPriority()
	{
		return this.masterPrioritySetting;
	}

	public void SetMasterPriority(PrioritySetting priority)
	{
		if (!priority.Equals(this.masterPrioritySetting))
		{
			this.masterPrioritySetting = priority;
			if (this.onPriorityChanged != null)
			{
				this.onPriorityChanged(this.masterPrioritySetting);
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
	private int masterPriority = int.MinValue;

	[SerializeField]
	[Serialize]
	private PrioritySetting masterPrioritySetting = new PrioritySetting(PriorityScreen.PriorityClass.basic, 5);

	public Action<PrioritySetting> onPriorityChanged;

	public bool showIcon = true;

	public Vector2 iconOffset;

	public float iconScale = 1f;

	[SerializeField]
	private int refCount;

	private static Dictionary<PrioritySetting, PrioritySetting> conversions = new Dictionary<PrioritySetting, PrioritySetting>
	{
		{
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 1),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 4)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 2),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 5)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 3),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 6)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 4),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 7)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 5),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 8)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.high, 1),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 6)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.high, 2),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 7)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.high, 3),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 8)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.high, 4),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 9)
		},
		{
			new PrioritySetting(PriorityScreen.PriorityClass.high, 5),
			new PrioritySetting(PriorityScreen.PriorityClass.basic, 9)
		}
	};
}
