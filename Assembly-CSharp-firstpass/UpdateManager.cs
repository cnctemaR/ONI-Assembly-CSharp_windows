using System;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
	public static UpdateManager instance { get; private set; }

	private void Awake()
	{
		UpdateManager.instance = this;
		UpdateManager.Init();
	}

	public static void Init()
	{
		UpdateManager.TypeInfos.Clear();
		UpdateManager.UpdateGroups = new SimUpdateGroup[3];
		for (int i = 0; i < UpdateManager.UpdateGroups.Length; i++)
		{
			SimUpdateGroup[] updateGroups = UpdateManager.UpdateGroups;
			int num = i;
			UpdateManager.ListType listType = (UpdateManager.ListType)i;
			updateGroups[num] = new SimUpdateGroup(listType.ToString());
		}
		UpdateManager.instance.queuedAdded.Clear();
		UpdateManager.instance.queuedRemoved.Clear();
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
		}
		else
		{
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
			if (!this.skipAllUpdates)
			{
				foreach (SimUpdateGroup simUpdateGroup in UpdateManager.UpdateGroups)
				{
					simUpdateGroup.Update(dt);
				}
				KComponentSpawn.instance.comps.SimUpdate(dt);
			}
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

	private bool skipNextUpdate = false;

	private static Dictionary<Type, SimUpdateTypeInfo> TypeInfos = new Dictionary<Type, SimUpdateTypeInfo>();

	private static SimUpdateGroup[] UpdateGroups = null;

	private bool skipAllUpdates = false;

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
