using System;
using System.Collections.Generic;
using UnityEngine;

public class PreUpdateScheduler : MonoBehaviour
{
	public static PreUpdateScheduler Instance { get; private set; }

	private void Awake()
	{
		PreUpdateScheduler.Instance = this;
	}

	public void Register(IPreUpdater pre_updater)
	{
		this.preUpdaters.Add(pre_updater);
	}

	private void Update()
	{
		int count = this.preUpdaters.Count;
		for (int i = 0; i < count; i++)
		{
			IPreUpdater preUpdater = this.preUpdaters[i];
			preUpdater.OnPreUpdate();
		}
	}

	private void OnDestroy()
	{
		PreUpdateScheduler.Instance = null;
	}

	private List<IPreUpdater> preUpdaters = new List<IPreUpdater>();
}
