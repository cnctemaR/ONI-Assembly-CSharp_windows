using System;
using System.Collections.Generic;
using UnityEngine;

public class OnDemandUpdater : MonoBehaviour
{
	public static void DestroyInstance()
	{
		OnDemandUpdater.Instance = null;
	}

	private void Awake()
	{
		OnDemandUpdater.Instance = this;
	}

	public void Register(IUpdateOnDemand updater)
	{
		if (!this.Updaters.Contains(updater))
		{
			this.Updaters.Add(updater);
		}
	}

	public void Unregister(IUpdateOnDemand updater)
	{
		if (this.Updaters.Contains(updater))
		{
			this.Updaters.Remove(updater);
		}
	}

	private void Update()
	{
		for (int i = 0; i < this.Updaters.Count; i++)
		{
			if (this.Updaters[i] != null)
			{
				this.Updaters[i].UpdateOnDemand();
			}
		}
	}

	private List<IUpdateOnDemand> Updaters = new List<IUpdateOnDemand>();

	public static OnDemandUpdater Instance;
}
