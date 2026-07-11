using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AutoDisinfectableManager")]
public class AutoDisinfectableManager : KMonoBehaviour, ISim1000ms
{
	public static void DestroyInstance()
	{
		AutoDisinfectableManager.Instance = null;
	}

	protected override void OnPrefabInit()
	{
		base.OnPrefabInit();
		AutoDisinfectableManager.Instance = this;
	}

	public void AddAutoDisinfectable(AutoDisinfectable auto_disinfectable)
	{
		this.autoDisinfectables.Add(auto_disinfectable);
	}

	public void RemoveAutoDisinfectable(AutoDisinfectable auto_disinfectable)
	{
		auto_disinfectable.CancelChore();
		this.autoDisinfectables.Remove(auto_disinfectable);
	}

	public void Sim1000ms(float dt)
	{
		for (int i = 0; i < this.autoDisinfectables.Count; i++)
		{
			this.autoDisinfectables[i].RefreshChore();
		}
	}

	private List<AutoDisinfectable> autoDisinfectables = new List<AutoDisinfectable>();

	public static AutoDisinfectableManager Instance;
}
