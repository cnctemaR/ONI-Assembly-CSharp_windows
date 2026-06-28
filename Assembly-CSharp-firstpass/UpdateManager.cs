using System;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
	public static UpdateManager instance { get; private set; }

	private void Awake()
	{
		UpdateManager.TypeInfos.Clear();
		UpdateManager.UpdateGroups = new SimUpdateGroup[3];
		UpdateManager.instance = this;
		this.elapsedTime = 0f;
		for (int i = 0; i < UpdateManager.UpdateGroups.Length; i++)
		{
			UpdateManager.UpdateGroups[i] = new SimUpdateGroup(((UpdateManager.ListType)i).ToString());
		}
	}

	public static void Destroy()
	{
		UpdateManager.instance = null;
		UpdateManager.TypeInfos.Clear();
		UpdateManager.UpdateGroups = null;
	}

	public void SkipNextUpdate()
	{
		this.skipNextUpdate = true;
	}

	public void SkipAllUpdates()
	{
		this.skipAllUpdates = true;
		this.skipNextUpdate = true;
	}

	public void Step(float dt)
	{
		if (this.skipNextUpdate)
		{
			this.skipNextUpdate = this.skipAllUpdates;
			return;
		}
		foreach (UpdateManager.QueuedData queuedData in this.queuedAdded)
		{
			queuedData.typeInfo.Add(queuedData.behaviour);
		}
		this.queuedAdded.Clear();
		foreach (UpdateManager.QueuedData queuedData2 in this.queuedRemoved)
		{
			queuedData2.typeInfo.Remove(queuedData2.behaviour);
		}
		this.queuedRemoved.Clear();
		this.elapsedTime += dt;
		this.ticks = (int)(this.elapsedTime / 0.25f);
		this.ticks = Mathf.Min(3, this.ticks);
		this.elapsedTime -= (float)this.ticks * 0.25f;
		int num = 0;
		while (!this.skipAllUpdates && num < this.ticks)
		{
			foreach (SimUpdateGroup simUpdateGroup in UpdateManager.UpdateGroups)
			{
				simUpdateGroup.Update(0.25f);
			}
			num++;
		}
	}

	public static void AddSimUpdater(KMonoBehaviour behaviour)
	{
		SimUpdateTypeInfo simUpdaterTypeInfo = UpdateManager.GetSimUpdaterTypeInfo(behaviour);
		if (simUpdaterTypeInfo.IsValid)
		{
			UpdateManager.instance.queuedAdded.Add(new UpdateManager.QueuedData(simUpdaterTypeInfo, behaviour));
		}
	}

	public static void RemoveSimUpdater(KMonoBehaviour behaviour)
	{
		SimUpdateTypeInfo simUpdaterTypeInfo = UpdateManager.GetSimUpdaterTypeInfo(behaviour);
		if (simUpdaterTypeInfo.IsValid)
		{
			UpdateManager.instance.queuedRemoved.Add(new UpdateManager.QueuedData(simUpdaterTypeInfo, behaviour));
		}
	}

	private static SimUpdateTypeInfo GetSimUpdaterTypeInfo(KMonoBehaviour behaviour)
	{
		Type type = behaviour.GetType();
		SimUpdateTypeInfo simUpdateTypeInfo = null;
		if (!UpdateManager.TypeInfos.TryGetValue(type, out simUpdateTypeInfo))
		{
			simUpdateTypeInfo = new SimUpdateTypeInfo(type);
			UpdateManager.TypeInfos[type] = simUpdateTypeInfo;
			if (simUpdateTypeInfo.IsValid)
			{
				for (int i = 0; i < UpdateManager.UpdateGroups.Length; i++)
				{
					UpdateManager.UpdateGroups[i].Add(simUpdateTypeInfo.UpdateArrays[i]);
				}
			}
		}
		return simUpdateTypeInfo;
	}

	public const int SimTicksPerSecond = 4;

	public const float SecondsPerTick = 0.25f;

	private bool skipNextUpdate;

	private float elapsedTime;

	private int ticks;

	private static Dictionary<Type, SimUpdateTypeInfo> TypeInfos = new Dictionary<Type, SimUpdateTypeInfo>();

	private static SimUpdateGroup[] UpdateGroups = null;

	private bool skipAllUpdates;

	private List<UpdateManager.QueuedData> queuedAdded = new List<UpdateManager.QueuedData>();

	private List<UpdateManager.QueuedData> queuedRemoved = new List<UpdateManager.QueuedData>();

	public enum ListType
	{
		SimUpdateFirst,
		SimUpdate,
		SimUpdateLast,
		Num
	}

	private struct QueuedData
	{
		public QueuedData(SimUpdateTypeInfo info, KMonoBehaviour behaviour)
		{
			this.typeInfo = info;
			this.behaviour = behaviour;
		}

		public SimUpdateTypeInfo typeInfo;

		public KMonoBehaviour behaviour;
	}
}
